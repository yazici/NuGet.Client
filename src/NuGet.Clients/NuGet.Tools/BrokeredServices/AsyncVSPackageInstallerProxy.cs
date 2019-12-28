// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
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
        Lazy<IVsPackageInstaller> Installer { get; set; } // This is a trick to multiple instances of the installer service. 

        private bool _initialized = false;
        private async Task InitializeAsync()
        {
            if (_initialized)
            {
                return;
            }
            // TODO NK - Check if this call is free threaded?
            var componentModel = await ServiceLocator.GetGlobalServiceAsync<SComponentModel, IComponentModel>();
            // ensure we satisfy our imports
            componentModel?.DefaultCompositionService.SatisfyImportsOnce(this);
            _initialized = true;
        }

        public async Task<bool> InstallLatestPackageAsync(string source, string project, string packageId, bool includePrerelease)
        {
            if (!_initialized)
            {
                await InitializeAsync();
            }

            var vsPackageInstaller = (IVsAsyncPackageInstaller)Installer.Value;
            return await vsPackageInstaller.InstallLatestPackageAsync(source, project, packageId, includePrerelease);
        }

        public async Task<bool> InstallPackageAsync(string source, string project, string packageId, string version)
        {
            if (!_initialized)
            {
                await InitializeAsync();
            }

            var vsPackageInstaller = (IVsAsyncPackageInstaller)Installer.Value;
            return await vsPackageInstaller.InstallPackageAsync(source, project, packageId, version);
        }
    }
}
