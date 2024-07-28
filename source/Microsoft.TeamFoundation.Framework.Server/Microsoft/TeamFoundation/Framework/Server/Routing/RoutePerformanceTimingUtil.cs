// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Routing.RoutePerformanceTimingUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Net.Http;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Routing
{
  public static class RoutePerformanceTimingUtil
  {
    private const string c_routeStartTimerKey = "route-start-timer";

    public static void StartRouteMatchingTimer(IVssRequestContext requestContext)
    {
      if (requestContext.Items.TryGetValue("route-start-timer", out object _))
        return;
      PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "RouteMatching");
      requestContext.Items["route-start-timer"] = (object) performanceTimer;
    }

    public static void StopRouteMatchingTimer(HttpContextBase httpContext)
    {
      if (!(httpContext.Items[(object) HttpContextConstants.IVssRequestContext] is IVssRequestContext requestContext))
        return;
      RoutePerformanceTimingUtil.StopRouteMatchingTimer(requestContext);
    }

    public static void StopRouteMatchingTimer(HttpRequestMessage request)
    {
      IVssRequestContext ivssRequestContext = request.GetIVssRequestContext();
      if (ivssRequestContext == null)
        return;
      RoutePerformanceTimingUtil.StopRouteMatchingTimer(ivssRequestContext);
    }

    private static void StopRouteMatchingTimer(IVssRequestContext requestContext)
    {
      object obj;
      if (!requestContext.Items.TryGetValue("route-start-timer", out obj))
        return;
      ((PerformanceTimer) obj).End();
    }
  }
}
