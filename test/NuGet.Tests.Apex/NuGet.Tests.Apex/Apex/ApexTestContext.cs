// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Test.Apex.VisualStudio;
using Microsoft.Test.Apex.VisualStudio.Solution;
using NuGet.Common;
using NuGet.Test.Utility;

namespace NuGet.Tests.Apex
{
    internal class ApexTestContext : IDisposable
    {
        private const string ParentDirectoryName = "Apex";

        private readonly VisualStudioHost _visualStudio;
        private readonly ILogger _logger;
        private readonly SimpleTestPathContext _pathContext;

        public SolutionService SolutionService { get; }
        public ProjectTestExtension Project { get; }
        public string PackageSource => _pathContext.PackageSource;

        public NuGetApexTestService NuGetApexTestService { get; }

        public ApexTestContext(VisualStudioHost visualStudio, ProjectTemplate projectTemplate, ILogger logger, bool noAutoRestore = false)
        {
            logger.LogInformation("Creating test context");
            _pathContext = new SimpleTestPathContext(ParentDirectoryName);

            DisablePackageManagementDialog(_pathContext);

            if (noAutoRestore)
            {
                _pathContext.Settings.DisableAutoRestore();
            }

            _visualStudio = visualStudio;
            _logger = logger;
            SolutionService = _visualStudio.Get<SolutionService>();
            NuGetApexTestService = _visualStudio.Get<NuGetApexTestService>();

            Project = CommonUtility.CreateAndInitProject(projectTemplate, _pathContext, SolutionService, logger);

            NuGetApexTestService.WaitForAutoRestore();
        }

        public void Dispose()
        {
            _logger.LogInformation("Test complete, closing solution.");
            SolutionService.Save();
            SolutionService.Close();

            _pathContext.Dispose();
        }

        private static void DisablePackageManagementDialog(SimpleTestPathContext pathContext)
        {
            var parentDirectory = new DirectoryInfo(pathContext.WorkingDirectory.Path);

            while (parentDirectory.Name != ParentDirectoryName)
            {
                parentDirectory = parentDirectory.Parent;
            }

            var nugetConfigFile = new FileInfo(Path.Combine(parentDirectory.FullName, "NuGet.Config"));

            if (nugetConfigFile.Exists)
            {
                return;
            }

            var content = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageManagement>
    <add key=""format"" value=""0"" />
    <add key=""disabled"" value=""False"" />
  </packageManagement>
</configuration>";

            File.WriteAllText(nugetConfigFile.FullName, content);
        }
    }
}
