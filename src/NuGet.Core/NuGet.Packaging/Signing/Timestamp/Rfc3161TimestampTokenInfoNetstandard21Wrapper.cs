// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Security.Cryptography;

namespace NuGet.Packaging.Signing
{
#if IS_SIGNING_SUPPORTED && IS_CORECLR
    internal class Rfc3161TimestampTokenInfoNetstandard21Wrapper : IRfc3161TimestampTokenInfo
    {
        private System.Security.Cryptography.Pkcs.Rfc3161TimestampTokenInfo _rfc3161TimestampTokenInfo = null;

        public Rfc3161TimestampTokenInfoNetstandard21Wrapper(byte[] timestampTokenInfo)
        {
            bool success = System.Security.Cryptography.Pkcs.Rfc3161TimestampTokenInfo.TryDecode(
                new ReadOnlyMemory<byte>(timestampTokenInfo),
                out _rfc3161TimestampTokenInfo,
                out int bytesConsumed);
        }

        public Rfc3161TimestampTokenInfoNetstandard21Wrapper(System.Security.Cryptography.Pkcs.Rfc3161TimestampTokenInfo timestampTokenInfo)
        {
            _rfc3161TimestampTokenInfo = timestampTokenInfo;
        }
        public string PolicyId
        {
            get
            {
                return _rfc3161TimestampTokenInfo.PolicyId.ToString();
            }
        }

        public DateTimeOffset Timestamp
        {
            get
            {
                return _rfc3161TimestampTokenInfo.Timestamp;
            }
        }

        public long? AccuracyInMicroseconds
        {
            get
            {
                return _rfc3161TimestampTokenInfo.AccuracyInMicroseconds;
            }
        }
        public Oid HashAlgorithmId
        {
            get
            {
                return _rfc3161TimestampTokenInfo.HashAlgorithmId;
            }
        }

        public bool HasMessageHash(byte[] hash)
        {
            if (hash == null)
                return false;

            //var value = Decoded.MessageImprint.HashedMessage;
            var value = _rfc3161TimestampTokenInfo.GetMessageHash().ToArray();


            if (hash.Length != value.Length)
            {
                return false;
            }

            return value.SequenceEqual(hash);
        }

        public byte[] GetNonce()
        {
            var nonce = _rfc3161TimestampTokenInfo.GetNonce();
            if (nonce.HasValue)
            {
                return nonce.Value.ToArray();
            }
            return new byte[0];
        }
    }
#endif
}
