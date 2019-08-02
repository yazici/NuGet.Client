// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using NuGet.ContentModel;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.ProjectModel;
using NuGet.Repositories;
using NuGet.Shared;

namespace NuGet.Commands
{
    /// <summary>
    /// Cache objects used for building the lock file.
    /// </summary>
    public class LockFileBuilderCache
    {
        // Package files
        private readonly Dictionary<PackageIdentity, ContentItemCollection> _contentItems
            = new Dictionary<PackageIdentity, ContentItemCollection>();

        private readonly SelectionCriteriaCache _selectionCriteriaCache = new SelectionCriteriaCache();

        /// <summary>
        /// Get ordered selection criteria.
        /// </summary>
        public List<List<SelectionCriteria>> GetSelectionCriteria(RestoreTargetGraph graph, NuGetFramework framework)
        {
            return _selectionCriteriaCache.GetSelectionCriteria(graph, framework);
        }

        /// <summary>
        /// Get a ContentItemCollection of the package files.
        /// </summary>
        /// <remarks>Library is optional.</remarks>
        public ContentItemCollection GetContentItems(LockFileLibrary library, LocalPackageInfo package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            var identity = new PackageIdentity(package.Id, package.Version);

            if (!_contentItems.TryGetValue(identity, out var collection))
            {
                collection = new ContentItemCollection();

                if (library == null)
                {
                    // Read folder
                    collection.Load(package.Files);
                }
                else
                {
                    // Use existing library
                    collection.Load(library.Files);
                }

                _contentItems.Add(identity, collection);
            }

            return collection;
        }
    }
}
