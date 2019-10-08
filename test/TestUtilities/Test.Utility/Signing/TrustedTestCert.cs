// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using NuGet.Common;

namespace Test.Utility.Signing
{
    public static class TrustedTestCert
    {
        public static TrustedTestCert<X509Certificate2> Create(
            X509Certificate2 cert,
            StoreName storeName = StoreName.TrustedPeople,
            StoreLocation storeLocation = StoreLocation.CurrentUser,
            TimeSpan? maximumValidityPeriod = null)
        {
            return new TrustedTestCert<X509Certificate2>(
                cert,
                x => x,
                storeName,
                storeLocation,
                maximumValidityPeriod);
        }
    }

    /// <summary>
    /// Give a certificate full trust for the life of the object.
    /// </summary>
    public class TrustedTestCert<T> : IDisposable
    {
        private X509Store _store;

        public X509Certificate2 TrustedCert { get; }

        public T Source { get; }

        public StoreName StoreName { get; }

        public StoreLocation StoreLocation { get; }

        private bool _isDisposed;

        public TrustedTestCert(T source,
            Func<T, X509Certificate2> getCert,
            StoreName storeName = StoreName.TrustedPeople,
            StoreLocation storeLocation = StoreLocation.CurrentUser,
            TimeSpan? maximumValidityPeriod = null)
        {
            Source = source;
            TrustedCert = getCert(source);

            if (!maximumValidityPeriod.HasValue)
            {
                maximumValidityPeriod = TimeSpan.FromHours(2);
            }

#if IS_SIGNING_SUPPORTED
            if (TrustedCert.NotAfter - TrustedCert.NotBefore > maximumValidityPeriod.Value)
            {
                throw new InvalidOperationException($"The certificate used is valid for more than {maximumValidityPeriod}.");
            }
#endif
            StoreName = storeName;
            StoreLocation = storeLocation;
            AddCertificateToStore();
            ExportCrl();
        }

        private void AddCertificateToStore()
        {
            if (RuntimeEnvironmentHelper.IsMacOSX)
            {
                var certFile = new FileInfo(Path.Combine("/tmp", $"{TrustedCert.SerialNumber}.cer"));

                File.WriteAllBytes(certFile.FullName, TrustedCert.RawData);

                string addToKeyChainCmd = $"sudo security add-trusted-cert -d -r trustRoot" +
                                          $"-k \"/Library/Keychains/System.keychain\"" +
                                          $"\"{certFile.FullName}\"";
"
                var isAddedToKeyChain = RunMacCommand(addToKeyChainCmd);
            }
            else
            {
                _store = new X509Store(StoreName, StoreLocation);
                _store.Open(OpenFlags.ReadWrite);
                _store.Add(TrustedCert);
            }
              
        }

        private static bool RunMacCommand(string cmd)
        {
            try
            {
                string output;
                using (var process = new Process())
                {
                    process.StartInfo.FileName = "/bin/bash";
                    process.StartInfo.Arguments = "-c\"" + cmd + "\"";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;

                    process.Start();
                    /*
                    if (!process.WaitForExit(1000) || process.ExitCode != 0)
                    {
                        return false;
                    }
                    */
                    output = process.StandardOutput.ReadToEnd();

                    Console.WriteLine(output);

                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ExportCrl()
        {
            var testCertificate = Source as TestCertificate;

            if (testCertificate != null && testCertificate.Crl != null)
            {
                testCertificate.Crl.ExportCrl();
            }
        }

        private void DisposeCrl()
        {
            var testCertificate = Source as TestCertificate;

            if (testCertificate != null && testCertificate.Crl != null)
            {
                testCertificate.Crl.Dispose();
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                using (_store)
                {
                    _store.Remove(TrustedCert);
                }

                DisposeCrl();

                TrustedCert.Dispose();

                _isDisposed = true;
            }
        }
    }
}
