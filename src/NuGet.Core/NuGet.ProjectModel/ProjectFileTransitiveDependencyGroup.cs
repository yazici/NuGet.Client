// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Frameworks;
using NuGet.LibraryModel;
using NuGet.Shared;

namespace NuGet.ProjectModel
{
    public class ProjectFileTransitiveDependencyGroup : IEquatable<ProjectFileTransitiveDependencyGroup>
    {
        private NuGetFramework _nuGetFramework;

        public ProjectFileTransitiveDependencyGroup(NuGetFramework framework, IEnumerable<LibraryDependency> transitiveDependencies)
        {
            FrameworkName = framework.GetShortFolderName();
            var framN = framework.GetShortFolderName();
            _nuGetFramework = framework;
            TransitiveDependencies = transitiveDependencies;
            //TargetFrameworks = new List<CentralTransitiveDependencyInformation>();
        }

        public string FrameworkName { get; }

        public IEnumerable<LibraryDependency> TransitiveDependencies { get; }

       // public IList<CentralTransitiveDependencyInformation> TargetFrameworks { get; } 

        public bool Equals(ProjectFileTransitiveDependencyGroup other)
        {
            if (other == null)
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            //if (string.Equals(FrameworkName, other.FrameworkName, StringComparison.OrdinalIgnoreCase))
            //{
            //    if (TransitiveDependencies == null || other.TransitiveDependencies == null)
            //    {
            //        return TransitiveDependencies == other.TransitiveDependencies;
            //    }

            //    return TransitiveDependencies.OrderedEquals(other.TransitiveDependencies, s => s, StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);
            //}

            //return false;

            return string.Equals(FrameworkName, other.FrameworkName, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ProjectFileDependencyGroup);
        }

        public override int GetHashCode()
        {
            var combiner = new HashCodeCombiner();

            combiner.AddStringIgnoreCase(FrameworkName);

            //if (TransitiveDependencies != null)
            //{
            //    foreach (var dependency in TransitiveDependencies.OrderBy(s => s, StringComparer.OrdinalIgnoreCase))
            //    {
            //        combiner.AddStringIgnoreCase(dependency);
            //    }
            //}

            return combiner.CombinedHash;
        }
    }
}
