using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using NuGet.Common;
using NuGet.Packaging.Signing;
using NuGet.Test.Utility;
using Test.Utility.Signing;
using Xunit;
namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Test();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static void Test()
        {
            try
            {
                using (var chainHolder = new X509ChainHolder())
                using (var rootCertificate = SigningTestUtility.GetCertificate("root.crt"))
                using (var intermediateCertificate = SigningTestUtility.GetCertificate("intermediate.crt"))
                using (var leafCertificate = SigningTestUtility.GetCertificate("leaf.crt"))
                {
                    var chain = chainHolder.Chain;
                    var extraStore = new X509Certificate2Collection() { rootCertificate, intermediateCertificate };
                    var logger = new TestLogger();
                    try
                    {
                        CertificateChainUtility.GetCertificateChain(
                        leafCertificate,
                        extraStore,
                        logger,
                        CertificateType.Signature);
                    }
                    catch (Exception e)
                    {
                        var runtimeIsWindows = RuntimeEnvironmentHelper.IsWindows;
                        var runtimeIsLinux = RuntimeEnvironmentHelper.IsLinux;
                        var runtime = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
                        var msg = "";
                        var error = "";
                        var warning = "";
                        foreach (var m in logger.Messages.ToArray())
                        {
                            msg += m.ToString();
                        }
                        foreach (var err in logger.ErrorMessages.ToArray())
                        {
                            error += err.ToString();
                        }
                        foreach (var w in logger.WarningMessages.ToArray())
                        {
                            warning += w.ToString();
                        }
                        throw new NuGet.Packaging.Signing.SignatureException(
                        $"another way to detect runtime = {runtime}" + "\n" +
                        $"runtimeIsWindows = {runtimeIsWindows}" + "\n" +
                        $"runtimeIsLinux = {runtimeIsLinux}" + "\n" +
                        "original e is : \n" +
                        e + "\n" +
                        "additional info is : \n" +
                        e.Message + "\n" +
                        $"logger.Errors = {logger.Errors}" + "\n" +
                        error + "\n" +
                        $"logger.Warnings = {logger.Warnings}" + "\n" +
                        warning + "\n" +
                        $"logger.Messages = {logger.Messages.Count}" + "\n" +
                        msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}


