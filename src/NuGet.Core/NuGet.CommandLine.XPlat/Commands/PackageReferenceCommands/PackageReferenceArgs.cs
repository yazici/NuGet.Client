// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging.Core;

namespace NuGet.CommandLine.XPlat
{
    public class CPVMPackageReferenceArgs : IPackageReferenceArgs
    {
        public string DgFilePath { get; set; }
        public string ProjectPath { get; }
        public ILogger Logger { get; }
        public bool NoVersion { get; set; }
        public PackageDependency PackageDependency { get; set; }
        public string[] Frameworks { get; set; }
        public string[] Sources { get; set; }
        public string PackageDirectory { get; set; }
        public bool NoRestore { get; set; }
        public bool Interactive { get; set; }

        public CPVMPackageReferenceArgs(string projectPath, PackageDependency packageDependency, ILogger logger, bool noVersion)
        {
            ValidateArgument(projectPath);
            ValidateArgument(packageDependency);
            ValidateArgument(logger);

            ProjectPath = projectPath;
            PackageDependency = packageDependency;
            Logger = logger;
            NoVersion = noVersion;
        }

        public CPVMPackageReferenceArgs(string projectPath, PackageDependency packageDependency, ILogger logger) :
            this(projectPath, packageDependency, logger, noVersion: false)
        {
        }

        public CPVMPackageReferenceArgs(string projectPath, string packageId, ILogger logger) :
            this(projectPath, new PackageDependency(packageId), logger, noVersion: true)
        {
        }

        private void ValidateArgument(object arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(nameof(arg));
            }
        }
    }

    public class PackageReferenceArgs : IPackageReferenceArgs
    {
        public string ProjectPath { get; }
        public ILogger Logger { get; }
        public bool NoVersion { get; set; }
        public PackageDependency PackageDependency { get; set; }
        public string DgFilePath { get; set; }
        public string[] Frameworks { get; set; }
        public string[] Sources { get; set; }
        public string PackageDirectory { get; set; }
        public bool NoRestore { get; set; }

        public bool Interactive { get; set; }

        public PackageReferenceArgs(string projectPath, PackageDependency packageDependency, ILogger logger, bool noVersion)
        {
            ValidateArgument(projectPath);
            ValidateArgument(packageDependency);
            ValidateArgument(logger);

            ProjectPath = projectPath;
            PackageDependency = packageDependency;
            Logger = logger;
            NoVersion = noVersion;
        }

        public PackageReferenceArgs(string projectPath, PackageDependency packageDependency, ILogger logger) :
            this(projectPath, packageDependency, logger, noVersion: false)
        {
        }

        public PackageReferenceArgs(string projectPath, string packageId, ILogger logger) :
            this(projectPath, new PackageDependency(packageId), logger, noVersion: true)
        {
        }

        private void ValidateArgument(object arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(nameof(arg));
            }
        }
    }

    public interface IPackageReferenceArgs
    {
        string DgFilePath { get; set; }

        string ProjectPath { get; }
        ILogger Logger { get; }
        bool NoVersion { get; set; }
        PackageDependency PackageDependency { get; set; }     
        string[] Frameworks { get; set; }
        string[] Sources { get; set; }
        string PackageDirectory { get; set; }
        bool NoRestore { get; set; }
        bool Interactive { get; set; }

    }
}
