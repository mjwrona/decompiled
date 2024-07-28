// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.AggregatedRoute
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  internal class AggregatedRoute : RouteBase, IContributedRoute, IParameterizedRoute
  {
    private Dictionary<string, RouteParameter> m_parameters;
    private IEnumerable<ContributedRoute> m_routes;
    private string m_contributionId;
    private int m_count;
    private string[] m_templates;

    public AggregatedRoute(string contributionId, IEnumerable<ContributedRoute> routes)
    {
      this.m_contributionId = contributionId;
      this.m_routes = routes;
      this.m_templates = routes.SelectMany<ContributedRoute, string>((Func<ContributedRoute, IEnumerable<string>>) (r => (IEnumerable<string>) r.Templates)).ToArray<string>();
      this.m_parameters = new Dictionary<string, RouteParameter>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ContributedRoute route in routes)
      {
        ++this.m_count;
        if (route.RouteParameters != null)
        {
          foreach (KeyValuePair<string, RouteParameter> routeParameter in route.RouteParameters)
            this.m_parameters[routeParameter.Key] = routeParameter.Value;
        }
      }
    }

    public string ContributionId => this.m_contributionId;

    public string[] Templates => this.m_templates;

    public Dictionary<string, RouteParameter> RouteParameters => this.m_parameters;

    public override RouteData GetRouteData(HttpContextBase httpContext)
    {
      foreach (RouteBase route in this.m_routes)
      {
        RouteData routeData = route.GetRouteData(httpContext);
        if (routeData != null)
          return routeData;
      }
      return (RouteData) null;
    }

    public override VirtualPathData GetVirtualPath(
      RequestContext routingContext,
      RouteValueDictionary values)
    {
      foreach (RouteBase route in this.m_routes)
      {
        VirtualPathData virtualPath = route.GetVirtualPath(routingContext, values);
        if (virtualPath != null)
          return virtualPath;
      }
      return (VirtualPathData) null;
    }

    public override string ToString() => string.Format("{0} ({1} routes)", (object) this.m_contributionId, (object) this.m_count);
  }
}
