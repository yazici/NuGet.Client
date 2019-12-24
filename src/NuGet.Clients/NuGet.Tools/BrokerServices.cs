// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.ServiceHub.Framework;

namespace NuGetVSExtension
{
    internal class BrokerServices
    {
        internal const string IVsAsyncPackageInstallerServiceName = "IVsAsyncPackageInstaller";
        internal const string IVsAsyncPackageInstallerServiceVersion = "1.0";

        internal static ServiceRpcDescriptor IVsAsyncPackageInstallerServiceDescriptor { get; } = new ServiceJsonRpcDescriptor(
            new ServiceMoniker(IVsAsyncPackageInstallerServiceName, new Version(IVsAsyncPackageInstallerServiceVersion)),
            ServiceJsonRpcDescriptor.Formatters.MessagePack,
            ServiceJsonRpcDescriptor.MessageDelimiters.BigEndianInt32LengthHeader);
    }
}
