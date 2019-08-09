using NuGet.Test.Utility;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Test.Utility.Signing;
using Xunit.Sdk;
using NuGet.Packaging.Signing;
using NuGet.Common;

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
            /*
            try
            {

                Console.WriteLine("start testing ...");

                using (var crlServer = new MockServer())
                {
                    using (var testDirectory = TestDirectory.Create())
                    {
                        int length = 3;
                        
                        var certificateChain = SigningTestUtility.GenerateCertificateChain(length, crlServer.Uri, testDirectory, false);
                       
                        using (X509Chain chaine = new X509Chain())
                        {
                            chaine.ChainPolicy.RevocationMode = X509RevocationMode.Offline;
                            var collection = chaine.ChainPolicy.ExtraStore;
                            foreach (var cert in certificateChain)
                            {
                                collection.Add(cert.TrustedCert);
                            }

                            var leafCert = certificateChain[certificateChain.Count - 1].TrustedCert;

                            var wasBuilt = chaine.Build(leafCert);

                            Console.WriteLine($"Chain was built: {wasBuilt}");

                            foreach (X509ChainStatus chainStatus in chaine.ChainStatus)
                            {
                                Console.WriteLine($"Chain status: {chainStatus.Status}: {chainStatus.StatusInformation}");
                            }
                        }
                    }
                }
            }
            try
            {
                using (var fixture = new SignCommandTestFixture())
                {
                    var trustedCertChain = fixture.TrustedTestCertificateChain;

                    using (X509Chain chaine = new X509Chain())
                    {
                        chaine.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        var collection = chaine.ChainPolicy.ExtraStore;
                        foreach (var cert in trustedCertChain.Certificates)
                        {
                            collection.Add(cert.TrustedCert);
                        }

                        var leafCert = trustedCertChain.Leaf.TrustedCert;
                        // var rootCert = trustedCertChain.Root.TrustedCert

                        var wasBuilt = chaine.Build(leafCert);
                        Console.WriteLine($"Chain was built: {wasBuilt}");

                        foreach (X509ChainStatus chainStatus in chaine.ChainStatus)
                        {
                            Console.WriteLine($"Chain status: {chainStatus.Status}: {chainStatus.StatusInformation}");
                        }
                        var i = 0;

                        foreach (var cert in trustedCertChain.Certificates)
                        {
                            var file = new FileInfo(Path.Combine(".", $"{i}.cer"));

                            ++i;
                            Console.WriteLine($"Path is: {file.FullName}");
                            File.WriteAllBytes(file.FullName, cert.TrustedCert.RawData);
                        }

                        Console.WriteLine("Press [ENTER] to continue");
                        Console.ReadLine();
                    }
                }
            } */
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
                            $"runtimeIsWindows = {runtimeIsWindows}"+ "\n" +
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
