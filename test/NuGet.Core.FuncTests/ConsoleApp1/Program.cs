using System;
using System.IO;
using NuGet.Packaging.FuncTest;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start testing on MAC !");

            var fixture = new SigningTestFixture();

            var defaultTrustedRootCert = fixture.TrustedTestCertificate;

            var file = new FileInfo(Path.Combine(".", "defaultTrusted.cer"));

            File.WriteAllBytes(file.FullName, defaultTrustedRootCert.TrustedCert.RawData);

            Console.WriteLine("cert path is :" + file.FullName);
        }
    }
}
