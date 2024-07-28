// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Routing.GroupedWebApiRoute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Framework.Server.Routing
{
  public class GroupedWebApiRoute : HttpRoute
  {
    public GroupedWebApiRoute(string fixedSegment)
    {
      this.FixedSegment = !string.IsNullOrEmpty(fixedSegment) ? "/" + fixedSegment : string.Empty;
      this.Routes = new HttpRouteCollection();
    }

    public string FixedSegment { get; private set; }

    public HttpRouteCollection Routes { get; private set; }

    public override IHttpRouteData GetRouteData(string virtualPathRoot, HttpRequestMessage request)
    {
      IHttpRouteData routeData = (IHttpRouteData) null;
      PerformanceTimer? nullable = new PerformanceTimer?();
      IVssRequestContext ivssRequestContext = request.GetIVssRequestContext();
      if (ivssRequestContext != null)
      {
        RoutePerformanceTimingUtil.StartRouteMatchingTimer(ivssRequestContext);
        nullable = new PerformanceTimer?(PerformanceTimer.StartMeasure(ivssRequestContext, nameof (GroupedWebApiRoute), this.FixedSegment));
      }
      string str = WebUtility.UrlDecode(request.RequestUri.AbsolutePath);
      if (string.IsNullOrEmpty(this.FixedSegment) || str.IndexOf(this.FixedSegment + "/", StringComparison.OrdinalIgnoreCase) >= 0 || str.EndsWith(this.FixedSegment, StringComparison.OrdinalIgnoreCase))
      {
        for (int index = 0; index < this.Routes.Count; ++index)
        {
          routeData = this.Routes[index].GetRouteData(virtualPathRoot, request);
          if (routeData != null)
            break;
        }
      }
      if (nullable.HasValue)
        nullable.Value.End();
      return routeData;
    }

    public override IHttpVirtualPathData GetVirtualPath(
      HttpRequestMessage request,
      IDictionary<string, object> values)
    {
      for (int index = 0; index < this.Routes.Count; ++index)
      {
        IHttpVirtualPathData virtualPath = this.Routes[index].GetVirtualPath(request, values);
        if (virtualPath != null)
          return virtualPath;
      }
      return (IHttpVirtualPathData) null;
    }

    public override string ToString() => string.Format("{0} WebApi routes ({1})", (object) this.FixedSegment, (object) this.Routes.Count);
  }
}
