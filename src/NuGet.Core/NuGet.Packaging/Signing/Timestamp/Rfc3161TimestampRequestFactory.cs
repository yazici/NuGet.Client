// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace NuGet.Packaging.Signing
{
    public class Rfc3161TimestampRequestFactory
    {
        public static IRfc3161TimestampRequest CreateIRfc3161TimestampRequest(
            byte[] messageHash,
            HashAlgorithmName hashAlgorithm,
            Oid requestedPolicyId,
            byte[] nonce,
            bool requestSignerCertificates,
            X509ExtensionCollection extensions)
        {
            IRfc3161TimestampRequest iRfc3161TimestampRequest = null;
#if IS_SIGNING_SUPPORTED && IS_DESKTOP
            iRfc3161TimestampRequest = new Rfc3161TimestampRequestNet472Wrapper(
                messageHash,
                hashAlgorithm,
                requestedPolicyId,
                nonce,
                requestSignerCertificates,
                extensions);
#endif

#if IS_SIGNING_SUPPORTED && IS_CORECLR
            iRfc3161TimestampRequest = new Rfc3161TimestampRequestNetstandard21Wrapper(
                messageHash,
                hashAlgorithm,
                requestedPolicyId,
                nonce,
                requestSignerCertificates,
                extensions);
#endif
            return iRfc3161TimestampRequest;
        }

    }
}
