// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PerfCountersFilter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PerfCountersFilter : ITeamFoundationRequestFilter
  {
    private static readonly HashSet<string> ExcludedMethods = new HashSet<string>()
    {
      "Dequeue",
      "Enqueue"
    };
    private const string CountersIncremented = "TfsPerfCountersCurrentServerRequestsIncremented";

    void ITeamFoundationRequestFilter.BeginRequest(IVssRequestContext requestContext)
    {
    }

    public Task BeginRequestAsync(IVssRequestContext requestContext) => Task.CompletedTask;

    void ITeamFoundationRequestFilter.EndRequest(IVssRequestContext requestContext)
    {
    }

    void ITeamFoundationRequestFilter.EnterMethod(IVssRequestContext requestContext)
    {
      if (requestContext is AspNetRequestContext)
      {
        if (requestContext.Method != null && PerfCountersFilter.ExcludedMethods.Contains(requestContext.Method.Name))
          return;
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentServerRequests").Increment();
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentRequestsPerSec").Increment();
        requestContext.Items["TfsPerfCountersCurrentServerRequestsIncremented"] = (object) true;
      }
      if (requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.Performance.TimingsInHttpResponse"))
        requestContext.Items[RequestContextItemsKeys.IncludePerformanceTimingsInResponse] = (object) true;
      string b;
      if (!requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.Performance.TimingsInServerTimingHeader") || !requestContext.GetSessionValue("TRACEPOINT-COLLECTOR", out b) || string.IsNullOrWhiteSpace(b) || string.Equals("disabled", b, StringComparison.OrdinalIgnoreCase))
        return;
      requestContext.Items[RequestContextItemsKeys.IncludePerformanceTimingsInServerTimingHeader] = (object) true;
    }

    void ITeamFoundationRequestFilter.LeaveMethod(IVssRequestContext requestContext)
    {
      bool flag;
      if (((!(requestContext is AspNetRequestContext) ? 0 : (requestContext.TryGetItem<bool>("TfsPerfCountersCurrentServerRequestsIncremented", out flag) ? 1 : 0)) & (flag ? 1 : 0)) == 0)
        return;
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentServerRequests").Decrement();
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageResponseTime");
      performanceCounter.IncrementTicks(requestContext.RequestTimer.ExecutionSpan);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageResponseTimeBase");
      performanceCounter.Increment();
    }

    Task ITeamFoundationRequestFilter.PostAuthenticateRequest(IVssRequestContext requestContext) => (Task) Task.FromResult<int>(0);

    Task ITeamFoundationRequestFilter.PostLogRequestAsync(IVssRequestContext requestContext) => (Task) Task.FromResult<int>(0);

    void ITeamFoundationRequestFilter.PostAuthorizeRequest(IVssRequestContext requestContext)
    {
    }
  }
}
