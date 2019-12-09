// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;

namespace NuGet.Protocol
{
    public class LocalPackageSearchMetadata : IPackageSearchMetadata
    {
        private readonly NuspecReader _nuspec;

        public LocalPackageSearchMetadata(LocalPackageInfo package)
        {
            LocalPackageInfo = package ?? throw new ArgumentNullException(nameof(package));
            _nuspec = package.Nuspec;
        }

        public string Authors => _nuspec.GetAuthors();

        public IEnumerable<PackageDependencyGroup> DependencySets => _nuspec.GetDependencyGroups().ToArray();

        public string Description => _nuspec.GetDescription();

        /// <remarks>
        /// Local packages always have 0 as the download count
        /// </remarks>
        public long? DownloadCount => 0;

        public Uri IconUrl =>  GetIcon();

        public PackageIdentity Identity => _nuspec.GetIdentity();

        public Uri LicenseUrl => Convert(_nuspec.GetLicenseUrl());

        public string Owners => _nuspec.GetOwners();

        public Uri ProjectUrl => Convert(_nuspec.GetProjectUrl());

        public DateTimeOffset? Published => LocalPackageInfo.LastWriteTimeUtc;

        /// <remarks>
        /// There is no report abuse url for local packages.
        /// </remarks>
        public Uri ReportAbuseUrl => null;

        public Uri PackageDetailsUrl => null;

        public bool RequireLicenseAcceptance => _nuspec.GetRequireLicenseAcceptance();

        public string Summary => !string.IsNullOrEmpty(_nuspec.GetSummary()) ? _nuspec.GetSummary() : Description;

        public string Tags
        {
            get
            {
                var tags = _nuspec.GetTags()?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[] { };
                return string.Join(" ", tags);
            }
        }

        public string Title => !string.IsNullOrEmpty(_nuspec.GetTitle()) ? _nuspec.GetTitle() : _nuspec.GetId();

        public LocalPackageInfo LocalPackageInfo { get; private set; }

        public Task<IEnumerable<VersionInfo>> GetVersionsAsync() => Task.FromResult(Enumerable.Empty<VersionInfo>());

        /// <summary>
        /// Convert a string to a URI safely. This will return null if there are errors.
        /// </summary>
        private static Uri Convert(string uri)
        {
            Uri fullUri = null;

            if (!string.IsNullOrEmpty(uri))
            {
                Uri.TryCreate(uri, UriKind.Absolute, out fullUri);
            }

            return fullUri;
        }

        public bool IsListed => true;

        /// <remarks>
        /// The prefix reservation is not applicable to local packages
        /// </remarks>
        public bool PrefixReserved => false;

        public LicenseMetadata LicenseMetadata => _nuspec.GetLicenseMetadata();

        /// <remarks>
        /// Deprecation metadata is not stored within the package and requires an online package source.
        /// </remarks>
        public Task<PackageDeprecationMetadata> GetDeprecationMetadataAsync() => Task.FromResult<PackageDeprecationMetadata>(null);

        private const int FiveMegabytes = 5242880; // 1024 * 1024 * 5, 5MB

        public string LoadFileAsText(string path)
        {
            string fileContent = null;
            try
            {
                if (LocalPackageInfo.GetReader() is PackageArchiveReader reader) // This will never be anything else in reality. The search resource always uses a PAR
                {
                    using (reader)
                    {
                        var entry = reader.GetEntry(PathUtility.StripLeadingDirectorySeparators(path));
                        if (entry != null)
                        {
                            if (entry.Length >= FiveMegabytes)
                            {
                                fileContent = string.Format(CultureInfo.CurrentCulture, Strings.LoadFileFromNupkg_FileTooLarge, path, "5");
                            }
                            else
                            {
                                using (var licenseStream = entry.Open())
                                using (TextReader textReader = new StreamReader(licenseStream))
                                {
                                    fileContent = textReader.ReadToEnd();
                                }
                            }
                        }
                        else
                        {
                            fileContent = string.Format(CultureInfo.CurrentCulture, Strings.LoadFileFromNupkg_FileNotFound, path);
                        }
                    }
                }
            }
            catch
            {
                fileContent = string.Format(CultureInfo.CurrentCulture, Strings.LoadFileFromNupkg_UnknownProblemLoadingTheFile, path);
            }
            finally
            {
                if (fileContent == null)
                {
                    fileContent = string.Format(CultureInfo.CurrentCulture, Strings.LoadFileFromNupkg_UnknownProblemLoadingTheFile, path);
                }
            }
            return fileContent;
        }

        /// <summary>
        /// Points to an Icon, either Embedded Icon or IconUrl
        /// </summary>
        public Uri GetIcon()
        {
            string embeddedIconPath = _nuspec.GetIcon();

            if (embeddedIconPath == null)
            {
                return Convert(_nuspec.GetIconUrl());
            }

            var baseUri = Convert(LocalPackageInfo.Path);

            UriBuilder builder = new UriBuilder(baseUri)
            {
                Fragment = embeddedIconPath
            };

            // get the special icon url
            return builder.Uri;
        }
    }
}
