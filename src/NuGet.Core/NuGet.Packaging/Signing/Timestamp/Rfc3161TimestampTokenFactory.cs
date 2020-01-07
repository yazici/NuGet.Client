// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Cryptography.X509Certificates;

namespace NuGet.Packaging.Signing
{
    internal sealed class Rfc3161TimestampTokenFactory
    {
        public static IRfc3161TimestampToken CreateIRfc3161TimestampToken(
            IRfc3161TimestampTokenInfo tstInfo,
            X509Certificate2 signerCertificate,
            X509Certificate2Collection additionalCerts,
            byte[] encoded)
        {
            IRfc3161TimestampToken iRfc3161TimestampToken = null;
#if IS_SIGNING_SUPPORTED && IS_DESKTOP
            iRfc3161TimestampToken = new Rfc3161TimestampTokenNet472Wrapper(
                                            tstInfo,
                                            signerCertificate,
                                            additionalCerts,
                                            encoded);
#endif

#if IS_SIGNING_SUPPORTED && IS_CORECLR
            iRfc3161TimestampToken = new Rfc3161TimestampTokenNetstandard21Wrapper(
                                            tstInfo,
                                            signerCertificate,
                                            additionalCerts,
                                            encoded);
#endif
            return iRfc3161TimestampToken;
        }
    }
}
