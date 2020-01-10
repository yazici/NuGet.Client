// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ComponentModelHost;
using NuGet.VisualStudio;
using NuGet.VisualStudio.Contracts;
using Task = System.Threading.Tasks.Task;

namespace NuGetVSExtension.BrokeredServices
{
    internal class AsyncVSPackageInstallerProxy : IVsAsyncPackageInstaller
    {
        [Import]
        Lazy<IVsPackageInstaller> VSPackageInstaller { get; set; } // A trick to avoid diverging implementation codepaths for our extensibility APIs.

        private bool _initialized = false;

        private async Task InitializeAsync()
        {
            if (_initialized)
            {
                return;
            }
            var componentModel = await ServiceLocator.GetGlobalServiceAsync<SComponentModel, IComponentModel>();
            // ensure we satisfy our imports
            componentModel?.DefaultCompositionService.SatisfyImportsOnce(this);
            _initialized = true;
        }

        public async Task<bool> InstallLatestPackageAsync(string source, string projectUniqueName, string packageId, bool includePrerelease, CancellationToken cancellationToken)
        {
            if (!_initialized)
            {
                await InitializeAsync();
            }

            var vsPackageInstaller = (IVsAsyncPackageInstaller)VSPackageInstaller.Value; // This can be cast because VSPackageInstaller implements all the interfaces.
            return await vsPackageInstaller.InstallLatestPackageAsync(source, projectUniqueName, packageId, includePrerelease, cancellationToken);
        }

        public async Task<bool> InstallPackageAsync(string source, string projectUniqueName, string packageId, string version, CancellationToken cancellationToken)
        {
            if (!_initialized)
            {
                await InitializeAsync();
            }

            var vsPackageInstaller = (IVsAsyncPackageInstaller)VSPackageInstaller.Value; // This can be cast because VSPackageInstaller implements all the interfaces.
            return await vsPackageInstaller.InstallPackageAsync(source, projectUniqueName, packageId, version, cancellationToken);
        }
    }
}
