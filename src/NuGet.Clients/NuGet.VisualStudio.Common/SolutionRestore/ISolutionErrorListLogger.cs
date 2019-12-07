using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.VisualStudio
{
    public interface ISolutionErrorListLogger
    {
        void LogError(Guid g, string data);

        void LogWarning(Guid g, string data);
    }
}
