// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGet.CommandLine
{
    /// <summary>
    /// Handles updating the executing instance of NuGet.exe
    /// </summary>
    public class SelfUpdater
    {
        private const string NuGetCommandLinePackageId = "NuGet.CommandLine";
        private const string NuGetExe = "NuGet.exe";

        private string _assemblyLocation;
        private readonly Lazy<string> _lazyAssemblyLocation = new Lazy<string>(() =>
        {
            return typeof(SelfUpdater).Assembly.Location;
        });

        private readonly IConsole _console;

        public SelfUpdater(IConsole console)
        {
            if (console == null)
            {
                throw new ArgumentNullException(nameof(console));
            }
            _console = console;
        }

        /// <summary>
        /// This property is used only for testing (so that the self updater does not replace the running test
        /// assembly).
        /// </summary>
        internal string AssemblyLocation
        {
            get
            {
                return _assemblyLocation ?? _lazyAssemblyLocation.Value;
            }
            set
            {
                _assemblyLocation = value;
            }
        }

        public async Task UpdateSelfAsync(bool prerelease, string updateFeed)
        {
            Assembly assembly = typeof(SelfUpdater).Assembly;
            var version = GetNuGetVersion(assembly) ?? new NuGetVersion(assembly.GetName().Version);
            await SelfUpdateAsync(AssemblyLocation, prerelease, version, updateFeed, CancellationToken.None);
        }

        internal async Task SelfUpdateAsync(string exePath, bool prerelease, NuGetVersion currentVersion, string updateFeed, CancellationToken cancellationToken)
        {
            currentVersion = NuGetVersion.Parse("5.0.0");

            _console.WriteLine(LocalizedResourceManager.GetString("UpdateCommandCheckingForUpdates"), updateFeed);

            var sourceRepository = Repository.Factory.GetCoreV3(updateFeed);
            var feed = await sourceRepository.GetResourceAsync<FindPackageByIdResource>(cancellationToken);
            var versions = await feed.GetAllVersionsAsync(NuGetCommandLinePackageId, new SourceCacheContext(), _console, cancellationToken);

            var latestVersion = versions.LastOrDefault(e => e.IsPrerelease == prerelease);

            _console.WriteLine(LocalizedResourceManager.GetString("UpdateCommandCurrentlyRunningNuGetExe"), currentVersion);

            // Check to see if an update is needed
            if (latestVersion == null || currentVersion >= latestVersion)
            {
                _console.WriteLine(LocalizedResourceManager.GetString("UpdateCommandNuGetUpToDate"));
            }
            else
            {
                _console.WriteLine(LocalizedResourceManager.GetString("UpdateCommandUpdatingNuGet"), latestVersion);

                var tempDir = Path.Combine(NuGetEnvironment.GetFolderPath(NuGetFolderPath.Temp), "updateSelf");
                var tempPath = FileUtility.GetTempFilePath(tempDir);
                try
                {
                    DirectoryUtility.CreateSharedDirectory(tempDir);
                    // Get NuGet.exe file from the package
                    var downloader = await feed.GetPackageDownloaderAsync(new PackageIdentity(NuGetCommandLinePackageId, latestVersion), new SourceCacheContext(), _console, cancellationToken);
                    await downloader.CopyNupkgFileToAsync(tempPath, cancellationToken);

                    // Get the exe path and move it to a temp file (NuGet.exe.old) so we can replace the running exe with the bits we got 
                    // from the package repository
                    var nugetExeFile = (await downloader.CoreReader.GetFilesAsync(CancellationToken.None)).FirstOrDefault(f => Path.GetFileName(f).Equals(NuGetExe, StringComparison.OrdinalIgnoreCase));

                    // If for some reason this package doesn't have NuGet.exe then we don't want to use it
                    if (nugetExeFile == null)
                    {
                        throw new CommandLineException(LocalizedResourceManager.GetString("UpdateCommandUnableToLocateNuGetExe"));
                    }
                    string renamedPath = exePath + ".old";

                    Move(exePath, renamedPath);

                    using (Stream fromStream = await downloader.CoreReader.GetStreamAsync(nugetExeFile, cancellationToken), toStream = File.Create(exePath))
                    {
                        fromStream.CopyTo(toStream);
                    }
                }
                finally
                {
                    // Delete the temporary directory
                    FileUtility.Delete(tempDir);
                }
            }
            _console.WriteLine(LocalizedResourceManager.GetString("UpdateCommandUpdateSuccessful"));
        }

        internal static NuGetVersion GetNuGetVersion(Assembly assembly)
        {
            try
            {
                var assemblyInformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                return new NuGetVersion(assemblyInformationalVersion.InformationalVersion);
            }
            catch
            {
                // Don't let GetCustomAttributes throw.
            }
            return null;
        }

        protected virtual void Move(string oldPath, string newPath)
        {
            try
            {
                if (File.Exists(newPath))
                {
                    File.Delete(newPath);
                }
            }
            catch (FileNotFoundException)
            {

            }

            File.Move(oldPath, newPath);
        }
    }
}
