// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Routing.GroupedMVCRoute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Web;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Framework.Server.Routing
{
  public class GroupedMVCRoute : RouteBase
  {
    public GroupedMVCRoute(string fixedSegment)
    {
      this.FixedSegment = "/" + fixedSegment;
      this.Routes = new RouteCollection();
    }

    public string FixedSegment { get; private set; }

    public RouteCollection Routes { get; private set; }

    public override RouteData GetRouteData(HttpContextBase httpContext)
    {
      RouteData routeData = (RouteData) null;
      PerformanceTimer? nullable = new PerformanceTimer?();
      if (httpContext.Items[(object) HttpContextConstants.IVssRequestContext] is IVssRequestContext requestContext)
      {
        RoutePerformanceTimingUtil.StartRouteMatchingTimer(requestContext);
        nullable = new PerformanceTimer?(PerformanceTimer.StartMeasure(requestContext, nameof (GroupedMVCRoute), this.FixedSegment));
      }
      string executionFilePath = httpContext.Request.AppRelativeCurrentExecutionFilePath;
      if (executionFilePath.IndexOf(this.FixedSegment + "/", StringComparison.OrdinalIgnoreCase) >= 0 || executionFilePath.EndsWith(this.FixedSegment, StringComparison.OrdinalIgnoreCase))
        routeData = this.Routes.GetRouteData(httpContext);
      if (nullable.HasValue)
        nullable.Value.End();
      return routeData;
    }

    public override VirtualPathData GetVirtualPath(
      RequestContext requestContext,
      RouteValueDictionary values)
    {
      return this.Routes.GetVirtualPath(requestContext, values);
    }

    public override string ToString() => string.Format("{0} MVC routes ({1})", (object) this.FixedSegment, (object) this.Routes.Count);
  }
}
