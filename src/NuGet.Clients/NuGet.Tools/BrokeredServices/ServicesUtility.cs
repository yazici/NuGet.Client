// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.ServiceHub.Framework;
using Microsoft.VisualStudio.Shell;

namespace NuGetVSExtension.BrokeredServices
{
    internal class ServicesUtility
    {
        internal const string IVsAsyncPackageInstallerServiceName = "IVsAsyncPackageInstaller"; // TODO NK - Check when the assembly gets loaded.
        internal const string IVsAsyncPackageInstallerServiceVersion = "1.0";

        internal static ServiceRpcDescriptor IVsAsyncPackageInstallerServiceDescriptor { get; } = new ServiceJsonRpcDescriptor(
            new ServiceMoniker(IVsAsyncPackageInstallerServiceName, new Version(IVsAsyncPackageInstallerServiceVersion)),
            ServiceJsonRpcDescriptor.Formatters.MessagePack,
            ServiceJsonRpcDescriptor.MessageDelimiters.BigEndianInt32LengthHeader);

        internal static BrokeredServiceFactory GetIVsAsyncPackageInstallerFactory()
        {
            return (mk, options, sb, ct) => new ValueTask<object>(new AsyncVSPackageInstallerProxy());
        }
    }
}
