// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography;

namespace NuGet.Packaging.Signing
{
    internal interface ICms : IDisposable
    {
        byte[] GetPrimarySignatureSignatureValue();
        byte[] GetRepositoryCountersignatureSignatureValue();

        void AddCertificates(IEnumerable<byte[]> encodedCertificates);

        void AddCountersignature(CmsSigner cmsSigner, CngKey privateKey);

        void AddTimestampToRepositoryCountersignature(SignedCms timestamp);

        void AddTimestamp(SignedCms timestamp);

        byte[] Encode();

    }
}
