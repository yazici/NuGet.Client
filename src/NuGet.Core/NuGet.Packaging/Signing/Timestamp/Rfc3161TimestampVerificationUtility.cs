// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

#if IS_DESKTOP
using System.Security.Cryptography.Pkcs;
#endif

using System.Security.Cryptography.X509Certificates;

namespace NuGet.Packaging.Signing
{
    /// <summary>
    /// Provides convenience methods for verification of a RFC 3161 timestamp.
    /// </summary>
    internal static class Rfc3161TimestampVerificationUtility
    {
        private const double MillisecondsPerMicrosecond = 0.001;

#if IS_DESKTOP

        internal static bool IsTimestampInValidityPeriod(X509Certificate2 certificate, Timestamp timestamp)
        {
            return IsTimestampInValidityPeriod(certificate, timestamp.LowerLimit, timestamp.UpperLimit);
        }

        internal static bool IsTimestampInValidityPeriod(X509Certificate2 certificate, Rfc3161TimestampTokenInfo tstInfo)
        {
            double accuracyInMilliseconds = GetAccuracyInMilliseconds(tstInfo);

            var lowerLimit = tstInfo.Timestamp.AddMilliseconds(-accuracyInMilliseconds);
            var upperLimit = tstInfo.Timestamp.AddMilliseconds(accuracyInMilliseconds);

            return IsTimestampInValidityPeriod(certificate, lowerLimit, upperLimit);
        }

        internal static bool IsTimestampInValidityPeriod(
            X509Certificate2 certificate,
            DateTimeOffset lowerLimit,
            DateTimeOffset upperLimit)
        {
            DateTimeOffset notAfter = DateTime.SpecifyKind(certificate.NotAfter, DateTimeKind.Local);
            DateTimeOffset notBefore = DateTime.SpecifyKind(certificate.NotBefore, DateTimeKind.Local);

            return upperLimit <= notAfter && lowerLimit >= notBefore;
        }

        internal static bool TryReadTSTInfoFromSignedCms(
            SignedCms timestampCms,
            out Rfc3161TimestampTokenInfo tstInfo)
        {
            tstInfo = null;
            if (timestampCms.ContentInfo.ContentType.Value.Equals(Oids.TSTInfoContentType))
            {
                tstInfo = new Rfc3161TimestampTokenInfo(timestampCms.ContentInfo.Content);
                return true;
            }
            // return false if the signedCms object does not contain the right ContentType
            return false;
        }

        internal static double GetAccuracyInMilliseconds(Rfc3161TimestampTokenInfo tstInfo)
        {
            double accuracyInMilliseconds;

            if (!tstInfo.AccuracyInMicroseconds.HasValue)
            {
                if (StringComparer.Ordinal.Equals(tstInfo.PolicyId, Oids.BaselineTimestampPolicy))
                {
                    accuracyInMilliseconds = 1000;
                }
                else
                {
                    accuracyInMilliseconds = 0;
                }
            }
            else
            {
                accuracyInMilliseconds = tstInfo.AccuracyInMicroseconds.Value * MillisecondsPerMicrosecond;
            }

            if (accuracyInMilliseconds < 0)
            {
                throw new InvalidDataException(Strings.VerifyError_TimestampInvalid);
            }

            return accuracyInMilliseconds;
        }
#endif
    }
}
