// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using System.Threading;
using EnvDTE;
using NuGet.Common;
using NuGet.PackageManagement;
using NuGet.PackageManagement.VisualStudio;
using NuGet.ProjectManagement;

namespace NuGet.VisualStudio
{
    [Export(typeof(IVsPackageRestorer))]
    public class VsPackageRestorer : IVsPackageRestorer
    {
        private readonly Configuration.ISettings _settings;
        private readonly ISolutionManager _solutionManager;
        private readonly IPackageRestoreManager _restoreManager;

        [ImportingConstructor]
        public VsPackageRestorer(Configuration.ISettings settings, ISolutionManager solutionManager, IPackageRestoreManager restoreManager)
        {
            _settings = settings;
            _solutionManager = solutionManager;
            _restoreManager = restoreManager;
        }

        public bool IsUserConsentGranted()
        {
            var packageRestoreConsent = new PackageManagement.PackageRestoreConsent(_settings);
            return packageRestoreConsent.IsGranted;
        }

        public void RestorePackages(Project project)
        {
            try
            {
                var solutionDirectory = _solutionManager.SolutionDirectory;
                var nuGetProjectContext = new EmptyNuGetProjectContext();
// TODO NK - Won't init.ps1 scripts deadlock here?
                // We simply use ThreadHelper.JoinableTaskFactory.Run instead of PumpingJTF.Run, unlike,
                // VsPackageInstaller and VsPackageUninstaller. Because, no powershell scripts get executed
                // as part of the operations performed below. Powershell scripts need to be executed on the
                // pipeline execution thread and they might try to access DTE. Doing that under
                // ThreadHelper.JoinableTaskFactory.Run will consistently result in a hang
                NuGetUIThreadHelper.JoinableTaskFactory.Run(() =>
                    _restoreManager.RestoreMissingPackagesInSolutionAsync(solutionDirectory,
                    nuGetProjectContext,
                    NullLogger.Instance,
                    CancellationToken.None));
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteErrorToActivityLog(ex);
            }
        }
    }
}
