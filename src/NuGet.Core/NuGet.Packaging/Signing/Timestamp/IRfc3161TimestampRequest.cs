using System;
using System.Threading.Tasks;

namespace NuGet.Packaging.Signing
{
    public interface IRfc3161TimestampRequest
    {
        Task<IRfc3161TimestampToken> SubmitRequestAsync(Uri timestampUri, TimeSpan timeout);

    }
}
