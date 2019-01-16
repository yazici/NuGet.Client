// Copyright(c) .NET Foundation.All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using NuGet.Shared;
using NuGet.Versioning;

namespace NuGet.LibraryModel
{
    public class DownloadDependency : IEquatable<DownloadDependency>, IComparable<DownloadDependency>
    {
        public string Name { get; }

        public VersionRange VersionRange { get; }

        public DownloadDependency(
            string name,
            VersionRange versionRange)
        {
            Name = name;
            VersionRange = versionRange;
        }

        public static implicit operator LibraryRange(DownloadDependency library) // Do not do this.
        {
            return new LibraryRange
            {
                Name = library.Name,
                TypeConstraint = LibraryDependencyTarget.Package,
                VersionRange = library.VersionRange
            };
        }

        public int CompareTo(DownloadDependency other) // write a comparer.
        {

            var compare = string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
            if (compare == 0)
            {
                
                if (VersionRange == null
                    && other.VersionRange == null)
                {
                    // NOOP;
                }
                else if (VersionRange == null)
                {
                    compare = -1;
                }
                else if (other.VersionRange == null)
                {
                    compare = 1;
                }
                else
                {
                    // TODO NK: Not the best way to do it. There is a perf problem here. Consider alternatives.
                    compare = VersionRange.ToNormalizedString().CompareTo(other.VersionRange.ToNormalizedString());
                }
            }
            return compare;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(" ");
            sb.Append(VersionRange);
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCodeCombiner();

            hashCode.AddObject(Name);
            hashCode.AddObject(VersionRange);

            return hashCode.CombinedHash;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DownloadDependency);
        }

        public bool Equals(DownloadDependency other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Name == other.Name &&
                   EqualityUtility.EqualsWithNullCheck(VersionRange, other.VersionRange);
        }

        public DownloadDependency Clone()
        {
            return new DownloadDependency(Name, VersionRange);
        }
    }
}
