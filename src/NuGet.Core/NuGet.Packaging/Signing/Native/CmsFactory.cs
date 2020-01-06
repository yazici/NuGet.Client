using System.Security.Cryptography.Pkcs;

namespace NuGet.Packaging.Signing
{
    public class CmsFactory
    {
        internal static ICms CreateICms(byte[] input)
        {
            ICms cms = null;
#if IS_SIGNING_SUPPORTED && IS_DESKTOP
            NativeCms nativeCms = NativeCms.Decode(input);
            cms = new NativeCmsWrapper(nativeCms);
#endif

#if IS_SIGNING_SUPPORTED && IS_CORECLR
            SignedCms signedCms = new SignedCms();
            signedCms.Decode(input);
            cms = new SignedCmsWrapper(signedCms);
#endif
            return cms;
        }

    }
}
