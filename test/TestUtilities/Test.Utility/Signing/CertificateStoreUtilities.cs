using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NuGet.Common;

namespace Test.Utility.Signing
{
    public static class CertificateStoreUtilities
    {
        public static StoreLocation GetTrustedCertificateStoreLocation()
        {
            // According to https://github.com/dotnet/runtime/blob/master/docs/design/features/cross-platform-cryptography.md#x509store   
            // use different approaches for Windows, Mac and Linux.
            return (RuntimeEnvironmentHelper.IsWindows || RuntimeEnvironmentHelper.IsMacOSX) ? StoreLocation.LocalMachine : StoreLocation.CurrentUser;
        }
    }
}
