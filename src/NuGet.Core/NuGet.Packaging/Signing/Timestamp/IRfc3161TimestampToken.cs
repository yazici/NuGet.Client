using System.Security.Cryptography.Pkcs;

namespace NuGet.Packaging.Signing
{
    public interface IRfc3161TimestampToken
    {
#if IS_SIGNING_SUPPORTED
        IRfc3161TimestampTokenInfo TokenInfo { get; }

        SignedCms AsSignedCms();

#endif
    }
}

