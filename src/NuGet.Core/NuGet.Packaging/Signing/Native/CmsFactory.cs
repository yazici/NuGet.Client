// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Cryptography.Pkcs;

namespace NuGet.Packaging.Signing
{
    internal static class CmsFactory
    {
        internal static ICms CreateICms(byte[] cmsBytes)
        {
            ICms cms = null;
#if IS_SIGNING_SUPPORTED && IS_DESKTOP
            NativeCms nativeCms = NativeCms.Decode(cmsBytes);
            cms = new NativeCmsWrapper(nativeCms);
#endif

#if IS_SIGNING_SUPPORTED && IS_CORECLR
            SignedCms signedCms = new SignedCms();
            signedCms.Decode(cmsBytes);
            cms = new ManagedCmsWrapper(signedCms);
#endif
            return cms;
        }
    }
}