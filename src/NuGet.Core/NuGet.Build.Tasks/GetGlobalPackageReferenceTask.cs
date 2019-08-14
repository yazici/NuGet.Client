// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using NuGet.Packaging;

namespace NuGet.Build.Tasks
{
   
    public class GetGlobalPackageReferenceTask : Task
    {
        /// <summary>
        /// Full path to the msbuild project.
        /// </summary>
        [Required]
        public string ProjectUniqueName { get; set; }

        [Required]
        public ITaskItem[] PackageReferences { get; set; }

        [Required]
        public ITaskItem[] GlobalPackageReferences { get; set; }

        /// <summary>
        /// Target frameworks to apply this for. If empty this applies to all.
        /// </summary>
        public string TargetFrameworks { get; set; }

        /// <summary>
        /// The path to the cpvmf.
        /// </summary>
        [Required]
        public string CPVMF { get; set; }


        /// <summary>
        /// Output items
        /// </summary>
        [Output]
        public ITaskItem[] RestoreGraphItems { get; set; }

        /// <summary>
        /// First iteration
        /// </summary>
        /// <returns></returns>
        public bool Execute1()
        {
            System.Diagnostics.Debugger.Launch();

            var log = new MSBuildLogger(Log);
            log.LogDebug($"(in) ProjectUniqueName '{ProjectUniqueName}'");
            log.LogDebug($"(in) TargetFrameworks '{TargetFrameworks}'");
            log.LogDebug($"(in) PackageReferences '{string.Join(";", PackageReferences.Select(p => p.ItemSpec))}'");

            var entries = new List<ITaskItem>();
            var seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var msbuildItem in MergeReferences())
            {
                var packageId = msbuildItem.ItemSpec;

                if (string.IsNullOrEmpty(packageId) || !seenIds.Add(packageId))
                {
                    // Skip empty or already processed ids
                    continue;
                }

                var properties = new Dictionary<string, string>();
                properties.Add("ProjectUniqueName", ProjectUniqueName);
                properties.Add("Type", "Dependency");
                properties.Add("Id", packageId);
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "Version", "VersionRange");

                if (!string.IsNullOrEmpty(TargetFrameworks))
                {
                    properties.Add("TargetFrameworks", TargetFrameworks);
                }

                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "IncludeAssets");
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "ExcludeAssets");
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "PrivateAssets");
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "NoWarn");
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "IsImplicitlyDefined");
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "GeneratePathProperty");
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "Global");
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "CPVMF");

                entries.Add(new TaskItem(Guid.NewGuid().ToString(), properties));
            }

            RestoreGraphItems = entries.ToArray();

            return true;
        }


        public override bool Execute()
        {
            System.Diagnostics.Debugger.Launch();

            var log = new MSBuildLogger(Log);
            log.LogDebug($"(in) ProjectUniqueName '{ProjectUniqueName}'");
            log.LogDebug($"(in) TargetFrameworks '{TargetFrameworks}'");
            log.LogDebug($"(in) PackageReferences '{string.Join(";", PackageReferences.Select(p => p.ItemSpec))}'");

            var entries = new List<ITaskItem>();
            var seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var globalSeenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var msbuildItem in PackageReferences)
            {
                var packageId = msbuildItem.ItemSpec;

                if (string.IsNullOrEmpty(packageId) || !seenIds.Add(packageId))
                {
                    // Skip empty or already processed ids
                    continue;
                }

                var properties = new Dictionary<string, string>();
                properties.Add("ProjectUniqueName", ProjectUniqueName);
                properties.Add("Type", "Dependency");
                properties.Add("Id", packageId);
                properties.Add("GlobalPackageReference", "false");
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "Version", "VersionRange");

                if (!string.IsNullOrEmpty(TargetFrameworks))
                {
                    properties.Add("TargetFrameworks", TargetFrameworks);
                }

                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "IncludeAssets");
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "ExcludeAssets");
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "PrivateAssets");
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "NoWarn");
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "IsImplicitlyDefined");
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "GeneratePathProperty");

                entries.Add(new TaskItem(Guid.NewGuid().ToString(), properties));
            }

            foreach (var msbuildItem in GlobalPackageReferences)
            {
                var packageId = msbuildItem.ItemSpec;

                if (string.IsNullOrEmpty(packageId) || !globalSeenIds.Add(packageId))
                {
                    // Skip empty or already processed ids
                    continue;
                }

                var properties = new Dictionary<string, string>();
                properties.Add("ProjectUniqueName", ProjectUniqueName);
                properties.Add("Type", "GlobalDependency");
                properties.Add("Id", packageId);
                properties.Add("GlobalPackageReference", "true");
                properties.Add("CPVMF", CPVMF);
                BuildTasksUtility.CopyPropertyIfExists(msbuildItem, properties, "Version", "VersionRange");

                if (!string.IsNullOrEmpty(TargetFrameworks))
                {
                    properties.Add("TargetFrameworks", TargetFrameworks);
                }

                entries.Add(new TaskItem(Guid.NewGuid().ToString(), properties));
            }

            RestoreGraphItems = entries.ToArray();

            return true;
        }



        private IEnumerable<ITaskItem> MergeReferences()
        {
            foreach(var item in PackageReferences)
            {
                if(GlobalPackageReferences.Contains(item, new PackageReferenceComparer()))
                {
                    var wrapper = new MSBuildTaskItem(item);
                    var ver = wrapper.GetProperty("Version");
                    item.SetMetadata("Version", GetVersionFromGlobalPackagereference(item));
                    item.SetMetadata("GlobalPackageReference", "true");
                    item.SetMetadata("CPVMF", CPVMF);
                    yield return item;
                }
                else
                {
                    yield return item;
                }
            }
        }

        private string GetVersionFromGlobalPackagereference(ITaskItem item)
        {
            var globalItem = GlobalPackageReferences.Where(x => x.ItemSpec == item.ItemSpec).First();
            var wrapper = new MSBuildTaskItem(globalItem);
            return wrapper.GetProperty("Version");
        }

        private class PackageReferenceComparer : IEqualityComparer<ITaskItem>
        {
            public bool Equals(ITaskItem x, ITaskItem y)
            {
                return x.ItemSpec == y.ItemSpec;
            }

            public int GetHashCode(ITaskItem obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}