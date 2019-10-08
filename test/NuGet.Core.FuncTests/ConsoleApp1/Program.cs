using System;
using NuGet.Packaging.FuncTest;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start testing on MAC !");

            SigningTestFixture fixture = new SigningTestFixture();

            var defaultTrustedRootCert = fixture.TrustedTestCertificate;

        }
    }
}
