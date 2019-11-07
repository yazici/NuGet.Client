// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.LibraryModel;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.RuntimeModel;
using NuGet.Versioning;

namespace NuGet.ProjectModel
{
    public class ProjectFileTransitiveDependencyGroupWriter
    {
        internal static string FrameworksElement = "frameworks";

        public static JObject Write(LockFile assetsFile)
        {
            var writer = new JsonObjectWriter();
            writer.WriteObjectStart(FrameworksElement);
            var frameworkNames = new HashSet<string>();
            var frameworkSorter = new NuGetFrameworkSorter();

            foreach (var projectTransitiveDepGroup in assetsFile.ProjectTransitiveDependencyGroups.OrderBy(ptdg => ptdg.FrameworkName))
            {
                PackageSpecWriter.SetDependencies(writer, projectTransitiveDepGroup.FrameworkName, projectTransitiveDepGroup.TransitiveDependencies);

            }

            writer.WriteObjectEnd();
            return writer.GetJObject();
        }
    }
}
