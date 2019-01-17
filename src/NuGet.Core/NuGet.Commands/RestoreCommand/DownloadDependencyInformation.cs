// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using NuGet.DependencyResolver;
using NuGet.Frameworks;
using NuGet.LibraryModel;

namespace NuGet.Commands
{
    public class DownloadDependencyInformation
    {
        public NuGetFramework Framework { get; }
        public IList<RemoteMatch> Dependencies { get; }
        public ISet<RemoteMatch> Install { get; }
        public ISet<LibraryRange> Unresolved { get; }


        private DownloadDependencyInformation(NuGetFramework framework, IList<RemoteMatch> dependencies, ISet<RemoteMatch> install, ISet<LibraryRange> unresolved)
        {
            Framework = framework;
            Dependencies = dependencies;
            Install = install;
            Unresolved = unresolved;
        }

        public static DownloadDependencyInformation Create(NuGetFramework framework, IList<RemoteMatch> dependencies, IList<IRemoteDependencyProvider> remoteDependencyProviders)
        {
            var install = new HashSet<RemoteMatch>();
            var unresolved = new HashSet<LibraryRange>();

            foreach (var dependency in dependencies)
            {
                if (LibraryType.Unresolved == dependency.Library.Type)
                {
                    unresolved.Add(dependency.Library);
                }
                else if (LibraryType.Package == dependency.Library.Type)
                {

                    var isRemote = remoteDependencyProviders.Contains(dependency.Provider);
                    if (isRemote)
                    {
                        install.Add(dependency);
                    }
                }
            }

            return new DownloadDependencyInformation(framework, dependencies, install, unresolved);
        }
    }
}
