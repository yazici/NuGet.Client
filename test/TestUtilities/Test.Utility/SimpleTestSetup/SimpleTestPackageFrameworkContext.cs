// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using NuGet.Frameworks;

namespace NuGet.Test.Utility
{
    /// <summary>
    /// Framework specific assets for a SimpleTestPackageContext
    /// </summary>
    public class SimpleTestPackageFrameworkContext
    {
        /// <summary>
        /// Target framework
        /// </summary>
        public NuGetFramework Framework { get; }

        /// <summary>
        /// Package dependencies.
        /// </summary>
        public IEnumerable<SimpleTestPackageContext> Dependencies { get; set;  }

        public IEnumerable<string> FrameworkReferences { get; set;  }

        public SimpleTestPackageFrameworkContext(NuGetFramework nugetFramework)
            : this(nugetFramework, new List<SimpleTestPackageContext>(), new List<string>())
        {
        }

        public SimpleTestPackageFrameworkContext(
            NuGetFramework framework,
            IEnumerable<SimpleTestPackageContext> dependencies,
            IEnumerable<string> frameworkReferences)
        {
            Framework = framework;
            Dependencies = dependencies;
            FrameworkReferences = frameworkReferences;
        }
    }
}
