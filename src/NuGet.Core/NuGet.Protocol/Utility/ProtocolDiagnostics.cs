// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace NuGet.Protocol.Utility
{
    public static class ProtocolDiagnostics
    {
        //public delegate void ProtocolDiagnosticEventHandler(ProtocolDiagnosticEvent pdEvent);

        //public static event ProtocolDiagnosticEventHandler Event;

        private static readonly DiagnosticListener dl = new DiagnosticListener("NuGet.Protocol");

        internal static void RaiseEvent(ProtocolDiagnosticEvent pdEvent)
        {
            //Event?.Invoke(pdEvent);

            const string name = "ProtocolDiagnostic";
            if (dl.IsEnabled() && dl.IsEnabled(name))
            {
                dl.Write(name, pdEvent);
            }
        }
    }
}
