// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if IS_DESKTOP

using System;
using System.Security.Cryptography.X509Certificates;
using NuGet.Packaging.Signing;
using Xunit;

namespace NuGet.Packaging.Test
{
    public class Rfc3161TimestampVerificationUtilityTests : IClassFixture<CertificatesFixture>
    {
        private readonly CertificatesFixture _fixture;

        public Rfc3161TimestampVerificationUtilityTests(CertificatesFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public void IsTimestampInValidityPeriod_WhenLowerLimitIsBeforeNotBefore_ReturnsFalse()
        {
            Test(notBefore => notBefore.AddSeconds(-1), upperLimit => upperLimit.AddSeconds(-1), Assert.False);
        }

        [Fact]
        public void IsTimestampInValidityPeriod_WhenLowerLimitIsOnNotBefore_ReturnsTrue()
        {
            Test(notBefore => notBefore, upperLimit => upperLimit.AddSeconds(-1), Assert.True);
        }

        [Fact]
        public void IsTimestampInValidityPeriod_WhenLowerLimitAndUpperLimitAreInsideNotBeforeAndNotAfter_ReturnsTrue()
        {
            Test(notBefore => notBefore.AddSeconds(1), upperLimit => upperLimit.AddSeconds(-1), Assert.True);
        }

        [Fact]
        public void IsTimestampInValidityPeriod_WhenUpperLimitIsOnNotAfter_ReturnsTrue()
        {
            Test(notBefore => notBefore.AddSeconds(1), upperLimit => upperLimit, Assert.True);
        }

        [Fact]
        public void IsTimestampInValidityPeriod_WhenUpperLimitIsAfterNotAfter_ReturnsFalse()
        {
            Test(notBefore => notBefore.AddSeconds(1), upperLimit => upperLimit.AddSeconds(1), Assert.False);
        }

        private void Test(
            Func<DateTimeOffset, DateTimeOffset> lowerLimitGenerator,
            Func<DateTimeOffset, DateTimeOffset> upperLimitGenerator,
            Action<bool> assert)
        {
            using (X509Certificate2 certificate = _fixture.GetDefaultCertificate())
            {
                DateTimeOffset lowerLimit = lowerLimitGenerator(DateTime.SpecifyKind(certificate.NotBefore, DateTimeKind.Local));
                DateTimeOffset upperLimit = upperLimitGenerator(DateTime.SpecifyKind(certificate.NotAfter, DateTimeKind.Local));

                assert(Rfc3161TimestampVerificationUtility.IsTimestampInValidityPeriod(certificate, lowerLimit, upperLimit));
            }
        }
    }
}

#endif
