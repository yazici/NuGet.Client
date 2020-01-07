// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace NuGet.Packaging.Signing
{
#if IS_SIGNING_SUPPORTED && IS_CORECLR
    internal sealed class ManagedCmsWrapper : ICms
    {
        private readonly SignedCms _signedCms;

        public ManagedCmsWrapper(SignedCms signedCms)
        {
            _signedCms = signedCms;
        }

        public byte[] GetPrimarySignatureSignatureValue()
        {
            return _signedCms.SignerInfos[0].GetSignature();
        }

        public byte[] GetRepositoryCountersignatureSignatureValue()
        {
            return _signedCms.SignerInfos[0].CounterSignerInfos[0].GetSignature();
        }

        public void AddCertificates(IEnumerable<byte[]> encodedCertificates)
        {
            foreach (var encodedCertificate in encodedCertificates)
            {
                _signedCms.AddCertificate(new X509Certificate2(encodedCertificate));
            }
        }

        public void AddCountersignature(CmsSigner cmsSigner, CngKey privateKey)
        {
            _signedCms.SignerInfos[0].ComputeCounterSignature(cmsSigner);
        }

        public void AddTimestampToRepositoryCountersignature(SignedCms timestamp)
        {
            var bytes = timestamp.Encode();

            var unsignedAttribute = new AsnEncodedData(Oids.SignatureTimeStampTokenAttribute, bytes);

            _signedCms.SignerInfos[0].CounterSignerInfos[0].AddUnsignedAttribute(unsignedAttribute);

        }

        public void AddTimestamp(SignedCms timestamp)
        {
            var bytes = timestamp.Encode();

            var unsignedAttribute = new AsnEncodedData(Oids.SignatureTimeStampTokenAttribute, bytes);

            _signedCms.SignerInfos[0].AddUnsignedAttribute(unsignedAttribute);
         
        }

        public byte[] Encode()
        {
            return _signedCms.Encode();
        }

        public void Dispose()
        {
           //TODO: complete the dispose method
        }
    }
#endif
}

