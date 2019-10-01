using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging.FuncTest;
using NuGet.Test.Utility;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Test.Utility;
using Test.Utility.Signing;
using Xunit;
using System.IO;
using FluentAssertions;
using Org.BouncyCastle.Asn1;
using BcAccuracy = Org.BouncyCastle.Asn1.Tsp.Accuracy;
using DotNetUtilities = Org.BouncyCastle.Security.DotNetUtilities;

using NuGet.Packaging.Signing;
using NuGet.Packaging;
using NuGet.Common;

namespace ConsoleApp3
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string _untrustedChainCertError = "A certificate chain processed, but terminated in a root certificate which is not trusted by the trust provider.";
            SigningTestFixture _testFixture;
            TrustedTestCert<TestCertificate> _trustedTestCert;
            TestCertificate _untrustedTestCertificate;
            SignedPackageVerifierSettings _verifyCommandSettings = SignedPackageVerifierSettings.GetVerifyCommandDefaultPolicy(TestEnvironmentVariableReader.EmptyInstance);
            SignedPackageVerifierSettings _defaultSettings = SignedPackageVerifierSettings.GetDefault(TestEnvironmentVariableReader.EmptyInstance);
            IList<ISignatureVerificationProvider> _trustProviders;

            _testFixture = new SigningTestFixture();
            _trustedTestCert = _testFixture.TrustedTestCertificate;
            _untrustedTestCertificate = _testFixture.UntrustedTestCertificate;
            _trustProviders = new List<ISignatureVerificationProvider>()
            {
                new SignatureTrustAndValidityVerificationProvider()
            };


            var ca = await _testFixture.GetDefaultTrustedCertificateAuthorityAsync();
            var timestampService = await _testFixture.GetDefaultTrustedTimestampServiceAsync();
            var keyPair = SigningTestUtility.GenerateKeyPair(publicKeyLength: 2048);
            var now = DateTimeOffset.UtcNow;
            var issueOptions = new IssueCertificateOptions()
            {
                KeyPair = keyPair,
                NotAfter = now.AddSeconds(10),
                NotBefore = now.AddSeconds(-2),
                SubjectName = new X509Name("CN=NuGet Test Expired Certificate")
            };
            var bcCertificate = ca.IssueCertificate(issueOptions);

            using (var directory = TestDirectory.Create())
            {
#if IS_DESKTOP
                var privateKey = DotNetUtilities.ToRSA(keyPair.Private as RsaPrivateCrtKeyParameters);
#else
                var rsaParameters = DotNetUtilities.ToRSAParameters(keyPair.Private as RsaPrivateCrtKeyParameters);

                RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider();

                rsaCsp.ImportParameters(rsaParameters);

                var privateKey = rsaCsp;
#endif
#if IS_DESKTOP
                using (var certificate = new X509Certificate2(bcCertificate.GetEncoded()))
                {
                    certificate.PrivateKey = privateKey;
#else
                using (var certificateTmp = new X509Certificate2(bcCertificate.GetEncoded()))
                using (var certificate = RSACertificateExtensions.CopyWithPrivateKey(certificateTmp, privateKey))
                {
#endif
                    var notAfter = certificate.NotAfter.ToUniversalTime();

                    var packageContext = new SimpleTestPackageContext();
                    var signedPackagePath = await SignedArchiveTestUtility.AuthorSignPackageAsync(
                        certificate,
                        packageContext,
                        directory,
                        timestampService.Url);

                    var waitDuration = (notAfter - DateTimeOffset.UtcNow).Add(TimeSpan.FromSeconds(1));

                    // Wait for the certificate to expire.  Trust of the signature will require a valid timestamp.
                    await Task.Delay(waitDuration);

                    Assert.True(DateTime.UtcNow > notAfter);

                    var verifier = new PackageSignatureVerifier(_trustProviders);

                    using (var packageReader = new PackageArchiveReader(signedPackagePath))
                    {
                        var result = await verifier.VerifySignaturesAsync(packageReader, _verifyCommandSettings, CancellationToken.None);

                        var trustProvider = result.Results.Single();

                        Assert.True(result.IsValid);
                        Assert.Equal(SignatureVerificationStatus.Valid, trustProvider.Trust);
                        //test for linux
                        var sb = new System.Text.StringBuilder();
                        sb.AppendLine("result trust : " + trustProvider.Trust.ToString());
                        foreach (var error in trustProvider.Issues.Where(issue => issue.Level == LogLevel.Error))
                        {
                            sb.AppendLine("error : " + error.Code + " " + error.Message);
                        }
                        foreach (var warning in trustProvider.Issues.Where(issue => issue.Level == LogLevel.Warning))
                        {
                            sb.AppendLine("warning : " + warning.Code + " " + warning.Message);
                        }
                        try
                        {
                            Assert.Equal(0, trustProvider.Issues.Count(issue => issue.Level == LogLevel.Error));
                            Assert.Equal(0, trustProvider.Issues.Count(issue => issue.Level == LogLevel.Warning));
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message + "\n" + sb.ToString());
                        }
                        Console.WriteLine("chain build results: " + "\n" + sb.ToString());

                        
                    }
                }
            }

        }
    }
}
