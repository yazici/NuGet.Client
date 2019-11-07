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
    public class ProjectFileTransitiveDependencyGroupReader
    {
        public static List<ProjectFileTransitiveDependencyGroup> ReadJson(JObject rawJason, string assetsFilePath)
        {
            var frameworks = rawJason[ProjectFileTransitiveDependencyGroupWriter.FrameworksElement] as JObject;
            var result = new List<ProjectFileTransitiveDependencyGroup>();
            if (frameworks != null)
            {
                foreach (var framework in frameworks)
                {
                    try
                    {
                        var nugetFramework = NuGetFramework.Parse(framework.Key);
                        var dependencies = framework.Value.Value<JObject>();
                        var dependencyList = new List<LibraryDependency>();

                        JsonPackageSpecReader.PopulateDependencies(
                            packageSpecPath: assetsFilePath,
                            results: dependencyList,
                            dependencies: dependencies,
                            isGacOrFrameworkReference: false
                            );
                        result.Add(new ProjectFileTransitiveDependencyGroup(nugetFramework, dependencyList));
                    }
                    catch (Exception ex)
                    {
                        throw FileFormatException.Create(ex, framework.Value, assetsFilePath);
                    }
                }
            }

            return result;
        }
    }
}
