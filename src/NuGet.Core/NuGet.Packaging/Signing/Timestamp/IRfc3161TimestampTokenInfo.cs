using System;
using System.Security.Cryptography;

namespace NuGet.Packaging.Signing
{
    public interface IRfc3161TimestampTokenInfo
    {
#if IS_SIGNING_SUPPORTED
        string PolicyId { get; }

        DateTimeOffset Timestamp { get; }

        long? AccuracyInMicroseconds { get; }

        Oid HashAlgorithmId { get; }

        bool HasMessageHash(byte[] hash);

        byte[] GetNonce();

#endif
    }
}
