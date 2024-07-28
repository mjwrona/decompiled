// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Telemetry.GitTraceExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server.Telemetry
{
  internal static class GitTraceExtensions
  {
    public const int ReportProgressTracepoint = 1013956;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReportProgress(
      this IVssRequestContext context,
      int progressPc,
      string stage,
      string layer = "GitServerTelemetry",
      bool done = true,
      string result = "")
    {
      context.Trace(1013956, TraceLevel.Info, GitServerUtils.TraceArea, layer, "Progress reporting: {0}%. {1} - {2}. {3}", (object) progressPc, (object) stage, done ? (object) "Done" : (object) "Running", (object) result);
    }
  }
}
