// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostManagementTraceHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class HostManagementTraceHelper
  {
    private static readonly string s_Area = "HostManagement";
    private static readonly string s_Layer = "BusinessLogic";

    public static void TraceThreadPool(bool traceAlways = false)
    {
      if (traceAlways)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        HostManagementTraceHelper.TraceThreadPool(HostManagementTraceHelper.\u003C\u003EO.\u003C0\u003E__TraceRawAlwaysOn ?? (HostManagementTraceHelper.\u003C\u003EO.\u003C0\u003E__TraceRawAlwaysOn = new HostManagementTraceHelper.Tracer(TeamFoundationTracingService.TraceRawAlwaysOn)));
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        HostManagementTraceHelper.TraceThreadPool(HostManagementTraceHelper.\u003C\u003EO.\u003C1\u003E__TraceRaw ?? (HostManagementTraceHelper.\u003C\u003EO.\u003C1\u003E__TraceRaw = new HostManagementTraceHelper.Tracer(TeamFoundationTracingService.TraceRaw)));
      }
    }

    private static void TraceThreadPool(HostManagementTraceHelper.Tracer tracer)
    {
      int workerThreads1;
      int completionPortThreads1;
      ThreadPool.GetAvailableThreads(out workerThreads1, out completionPortThreads1);
      int workerThreads2;
      int completionPortThreads2;
      ThreadPool.GetMinThreads(out workerThreads2, out completionPortThreads2);
      int workerThreads3;
      int completionPortThreads3;
      ThreadPool.GetMaxThreads(out workerThreads3, out completionPortThreads3);
      tracer(57474, TraceLevel.Info, HostManagementTraceHelper.s_Area, HostManagementTraceHelper.s_Layer, string.Format("{{\"AvailableWorker\": {0},\"Worker\": {1}, \"AvailableIO\": {2}, \"IO\": {3}, \"MinWorker\": {4}, \"MinIO\": {5}, \"MaxWorker\": {6}, \"MaxIO\": {7}}}", (object) workerThreads1, (object) (workerThreads3 - workerThreads1), (object) completionPortThreads1, (object) (completionPortThreads3 - completionPortThreads1), (object) workerThreads2, (object) completionPortThreads2, (object) workerThreads3, (object) completionPortThreads3), Array.Empty<object>());
    }

    private delegate void Tracer(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message,
      params object[] args);
  }
}
