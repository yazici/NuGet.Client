// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;

namespace NuGet.SolutionRestoreManager
{
    /// <summary>
    /// Contains target framework metadata needed for restore operation. Compared to IVsTargetFrameworkInfo2, this adds support for GlobalPackageReferences
    /// </summary>
    [ComImport]
    [Guid("451ACBA6-FE6A-4412-99D2-3882790BF338")] //ToChange
    public interface IVsTargetFrameworkInfo3 : IVsTargetFrameworkInfo2
    {
        /// <summary>
        /// Collection of global package downloads.
        /// </summary>
        IVsReferenceItems GlobalPackageReferences { get; }
    }
}
