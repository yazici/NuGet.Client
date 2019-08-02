// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using NuGet.ContentModel;
using NuGet.Frameworks;
using NuGet.Shared;


namespace NuGet.Commands
{
    internal class SelectionCriteriaCache
    {
        // OrderedCriteria is stored per target graph + override framework.
        private readonly Dictionary<CriteriaKey, List<List<SelectionCriteria>>> _criteriaSets =
            new Dictionary<CriteriaKey, List<List<SelectionCriteria>>>();

        /// <summary>
        /// Get ordered selection criteria.
        /// </summary>
        internal List<List<SelectionCriteria>> GetSelectionCriteria(RestoreTargetGraph graph, NuGetFramework framework)
        {
            // Criteria are unique on graph and framework override.
            var key = new CriteriaKey(graph.TargetGraphName, framework);

            if (!_criteriaSets.TryGetValue(key, out var criteria))
            {
                criteria = LockFileUtils.CreateOrderedCriteriaSets(graph.Conventions, framework, graph.RuntimeIdentifier);
                _criteriaSets.Add(key, criteria);
            }

            return criteria;
        }

        private class CriteriaKey : IEquatable<CriteriaKey>
        {
            public string TargetGraphName { get; }

            public NuGetFramework FrameworkOverride { get; }

            public CriteriaKey(string targetGraphName, NuGetFramework frameworkOverride)
            {
                TargetGraphName = targetGraphName;
                FrameworkOverride = frameworkOverride;
            }

            public bool Equals(CriteriaKey other)
            {
                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                if (ReferenceEquals(other, null))
                {
                    return false;
                }

                return StringComparer.Ordinal.Equals(TargetGraphName, other.TargetGraphName)
                    && FrameworkOverride.Equals(other.FrameworkOverride)
                    && other.FrameworkOverride.Equals(FrameworkOverride);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as CriteriaKey);
            }

            public override int GetHashCode()
            {
                var combiner = new HashCodeCombiner();

                combiner.AddObject(StringComparer.Ordinal.GetHashCode(TargetGraphName));
                combiner.AddObject(FrameworkOverride);

                return combiner.CombinedHash;
            }
        }
    }
}
