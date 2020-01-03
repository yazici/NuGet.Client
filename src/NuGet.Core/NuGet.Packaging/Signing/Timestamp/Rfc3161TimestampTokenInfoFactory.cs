using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Packaging.Signing
{
    public class Rfc3161TimestampTokenInfoFactory
    {
        public static IRfc3161TimestampTokenInfo CreateIRfc3161TimestampTokenInfo(byte[] bytes)
        {
            IRfc3161TimestampTokenInfo iRfc3161TimestampTokenInfo = null;
#if IS_SIGNING_SUPPORTED && IS_DESKTOP
            iRfc3161TimestampTokenInfo = new Rfc3161TimestampTokenInfoNet472Wrapper(bytes);
#endif

#if IS_SIGNING_SUPPORTED && IS_CORECLR
            iRfc3161TimestampTokenInfo = new Rfc3161TimestampTokenInfoNetstandard21Wrapper(bytes);
#endif
            return iRfc3161TimestampTokenInfo;
        }
    }
}
