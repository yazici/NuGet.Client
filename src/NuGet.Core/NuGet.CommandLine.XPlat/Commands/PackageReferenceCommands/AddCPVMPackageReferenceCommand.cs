// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace NuGet.CommandLine.XPlat
{
    public static class AddCPVMPackageReferenceCommand
    {
        public static void Register(CommandLineApplication app, Func<ILogger> getLogger,
            Func<IPackageReferenceCommandRunner> getCommandRunner)
        {
            app.Command("addcpvm", addpkg =>
            {
                addpkg.Description = Strings.AddPkg_Description;
                addpkg.HelpOption(XPlatUtility.HelpOption);

                addpkg.Option(
                    CommandConstants.ForceEnglishOutputOption,
                    Strings.ForceEnglishOutput_Description,
                    CommandOptionType.NoValue);

                var id = addpkg.Option(
                    "--package",
                    Strings.AddPkg_PackageIdDescription,
                    CommandOptionType.SingleValue);

                var version = addpkg.Option(
                    "--version",
                    Strings.AddPkg_PackageVersionDescription,
                    CommandOptionType.SingleValue);

                var dgFilePath = addpkg.Option(
                   "-d|--dg-file",
                   Strings.AddPkg_DgFileDescription,
                   CommandOptionType.SingleValue);

                var frameworks = addpkg.Option(
                   "-f|--framework",
                   Strings.AddPkg_FrameworksDescription,
                   CommandOptionType.MultipleValue);

                var projectPath = addpkg.Option(
                    "-p|--project",
                    Strings.AddPkg_ProjectPathDescription,
                    CommandOptionType.SingleValue);

                //var projectPath = addpkg.Option(
                //    "-cpvm|--cpvm",
                //    Strings.AddPkg_ProjectPathDescription,
                //    CommandOptionType.SingleValue);

                var noRestore = addpkg.Option(
                    "-n|--no-restore",
                    Strings.AddPkg_NoRestoreDescription,
                    CommandOptionType.NoValue);

                //var sources = addpkg.Option(
                //    "-s|--source",
                //    Strings.AddPkg_SourcesDescription,
                //    CommandOptionType.MultipleValue);

                var packageDirectory = addpkg.Option(
                    "--package-directory",
                    Strings.AddPkg_PackageDirectoryDescription,
                    CommandOptionType.SingleValue);

                var interactive = addpkg.Option(
                    "--interactive",
                    Strings.AddPkg_InteractiveDescription,
                    CommandOptionType.NoValue);

                addpkg.OnExecute(() =>
                {
                    ValidateArgument(id, addpkg.Name);
                    ValidateArgument(projectPath, addpkg.Name);
                    ValidateProjectPath(projectPath, addpkg.Name);
                    //if (!noRestore.HasValue())
                    //{
                    //    ValidateArgument(dgFilePath, addpkg.Name);
                    //}
                    var logger = getLogger();
                    var noVersion = !version.HasValue();
                    var packageVersion = version.HasValue() ? version.Value() : "*";
                    var packageDependency = new PackageDependency(id.Values[0], VersionRange.Parse(packageVersion));
                    var packageRefArgs = new CPVMPackageReferenceArgs(projectPath.Value(), packageDependency, logger)
                    {
                        Frameworks = CommandLineUtility.SplitAndJoinAcrossMultipleValues(frameworks.Values),
                        Sources = new string[0], //CommandLineUtility.SplitAndJoinAcrossMultipleValues(sources.Values),
                        PackageDirectory = packageDirectory.Value(),
                        NoRestore = noRestore.HasValue(),
                        NoVersion = noVersion,
                        DgFilePath = dgFilePath.Value(),
                        Interactive = interactive.HasValue()
                    };
                    var msBuild = new MSBuildAPIUtility(logger);
                    var addPackageRefCommandRunner = getCommandRunner();
                    return addPackageRefCommandRunner.ExecuteCommand(packageRefArgs, msBuild);
                });
            });
        }

        private static void ValidateArgument(CommandOption arg, string commandName)
        {
            if (arg.Values.Count < 1)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.Error_PkgMissingArgument,
                    commandName,
                    arg.Template));
            }
        }

        private static void ValidateProjectPath(CommandOption projectPath, string commandName)
        {
            if (!File.Exists(projectPath.Value()) ||
                !(projectPath.Value().EndsWith("props", StringComparison.OrdinalIgnoreCase) || projectPath.Value().EndsWith("csproj", StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    Strings.Error_PkgMissingOrInvalidProjectFile,
                    commandName,
                    projectPath.Value()));
            }
        }
    }
}
