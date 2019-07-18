using NuGet.CommandLine.Test;
using NuGet.Test.Utility;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Test.Utility.Signing;
using NuGet.CommandLine.FuncTest;
using Xunit.Sdk;
using NuGet.CommandLine.FuncTest.Commands;
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

                        crlServer.SetUpCrlDistributionPoint();
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
                        var msg = "";
                        var error = "";
                        var warning = "";
                        foreach (var m in logger.Messages.ToArray()) {
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
                        throw new NuGet.Packaging.Signing.SignatureException("original e is : \n" +
                            e + "\n" +
                            "additional info is : \n" +
                            e.Message + "\n" +
                            $"logger.Errors = {logger.Errors}" + "\n" +
                            error + "\n" +
                            $"logger.Warnings = {logger.Warnings}" + "\n" +
                            warning + "\n" +
                            $"logger.Messages = {logger.Messages.Count}" + "\n" +
                            msg );

                    }
                    /*
                    var exception = Assert.Throws<SignatureException>(
                        () => CertificateChainUtility.GetCertificateChain(
                            leafCertificate,
                            extraStore,
                            logger,
                            CertificateType.Signature));

                    Assert.Equal(NuGetLogCode.NU3018, exception.Code);
                    Assert.Equal("Certificate chain validation failed.", exception.Message);

                    Assert.Equal(1, logger.Errors);
                    SigningTestUtility.AssertUntrustedRoot(logger.LogMessages, LogLevel.Error);

                    if (RuntimeEnvironmentHelper.IsWindows || RuntimeEnvironmentHelper.IsLinux)
                    {
                        Assert.Equal(RuntimeEnvironmentHelper.IsWindows ? 2 : 1, logger.Warnings);

                        SigningTestUtility.AssertOfflineRevocation(logger.LogMessages, LogLevel.Warning);

                        if (RuntimeEnvironmentHelper.IsWindows)
                        {
                            SigningTestUtility.AssertRevocationStatusUnknown(logger.LogMessages, LogLevel.Warning);
                        }
                    } */
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
