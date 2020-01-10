// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace NuGet.Packaging.Signing
{
    internal sealed class Rfc3161TimestampTokenInfoFactory
    {
        public static IRfc3161TimestampTokenInfo Create(byte[] bytes)
        {
#if IS_SIGNING_SUPPORTED
            IRfc3161TimestampTokenInfo iRfc3161TimestampTokenInfo = null;
#if IS_DESKTOP
            iRfc3161TimestampTokenInfo = new Rfc3161TimestampTokenInfoNet472Wrapper(bytes);
#else
            iRfc3161TimestampTokenInfo = new Rfc3161TimestampTokenInfoNetstandard21Wrapper(bytes);
#endif
            return iRfc3161TimestampTokenInfo;
#else
            throw new NotSupportedException();
#endif
        }
    }
}
