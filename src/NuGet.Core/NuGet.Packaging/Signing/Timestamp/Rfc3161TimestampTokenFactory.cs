using System.Security.Cryptography.X509Certificates;

namespace NuGet.Packaging.Signing
{
    public class Rfc3161TimestampTokenFactory
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
