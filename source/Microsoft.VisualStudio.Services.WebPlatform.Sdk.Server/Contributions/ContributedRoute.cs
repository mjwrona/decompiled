// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContributedRoute
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Routing;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  internal class ContributedRoute : VssfMVCRoute, IParameterizedRoute, IContributedRoute
  {
    private Dictionary<string, RouteParameter> m_parameters;
    private RouteParameter m_wildcardParameter;
    private string[] m_templates;
    private string m_contributionId;
    private int m_hashCode;

    public ContributedRoute(
      string contributionId,
      TeamFoundationHostType hostType,
      string routeTemplate,
      Dictionary<string, RouteParameter> parameters,
      RouteValueDictionary defaults,
      RouteValueDictionary routeConstraints,
      RouteValueDictionary dataTokens)
      : base(routeTemplate, defaults, routeConstraints, dataTokens, (IRouteHandler) new MvcRouteHandler(), useServiceHostForVirtualPath: false)
    {
      this.m_templates = new string[1]{ routeTemplate };
      this.m_contributionId = contributionId;
      this.m_parameters = parameters;
      if (this.m_parameters == null)
        return;
      foreach (KeyValuePair<string, RouteParameter> parameter in this.m_parameters)
      {
        this.m_hashCode ^= parameter.Key.GetHashCode();
        if (parameter.Value.Wildcard)
          this.m_wildcardParameter = parameter.Value;
      }
    }

    public string ContributionId => this.m_contributionId;

    public string[] Templates => this.m_templates;

    public Dictionary<string, RouteParameter> RouteParameters => this.m_parameters;

    public override VirtualPathData GetVirtualPath(
      RequestContext requestContext,
      RouteValueDictionary values)
    {
      if (this.m_wildcardParameter != null && !values.ContainsKey(this.m_wildcardParameter.ParameterName))
      {
        values = new RouteValueDictionary((IDictionary<string, object>) values);
        values[this.m_wildcardParameter.ParameterName] = (object) string.Empty;
      }
      return base.GetVirtualPath(requestContext, values);
    }

    public override string ToString() => string.Format("{0} ({1} -> {2})", (object) this.m_contributionId, (object) this.m_templates[0], (object) this.Url);
  }
}
