// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TfsApiHostTypeConstraint
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TfsApiHostTypeConstraint : IHttpRouteConstraint, IRouteConstraint
  {
    private readonly TeamFoundationHostType m_hostType;

    public TfsApiHostTypeConstraint(TeamFoundationHostType hostType)
    {
      if (hostType == TeamFoundationHostType.Unknown)
        hostType = TeamFoundationHostType.All;
      this.m_hostType = hostType;
    }

    public TeamFoundationHostType HostType => this.m_hostType;

    bool IHttpRouteConstraint.Match(
      HttpRequestMessage request,
      IHttpRoute route,
      string parameterName,
      IDictionary<string, object> values,
      HttpRouteDirection routeDirection)
    {
      IVssRequestContext ivssRequestContext = request.GetIVssRequestContext();
      if (!ivssRequestContext.ExecutionEnvironment.IsHostedDeployment || (this.m_hostType & TeamFoundationHostType.Application) == TeamFoundationHostType.Unknown || (this.m_hostType & TeamFoundationHostType.ProjectCollection) != TeamFoundationHostType.Unknown)
        return (TfsApiHostTypeConstraint.GetNormalizedHostType(ivssRequestContext) & this.m_hostType) != 0;
      values[TfsApiPropertyKeys.TfsUseApplicationHost] = (object) true;
      return true;
    }

    bool IRouteConstraint.Match(
      HttpContextBase httpContext,
      Route route,
      string parameterName,
      RouteValueDictionary values,
      RouteDirection routeDirection)
    {
      IVssRequestContext requestContext = (IVssRequestContext) httpContext.Items[(object) HttpContextConstants.IVssRequestContext];
      return requestContext != null && (TfsApiHostTypeConstraint.GetNormalizedHostType(requestContext) & this.m_hostType) != 0;
    }

    private static TeamFoundationHostType GetNormalizedHostType(IVssRequestContext requestContext)
    {
      TeamFoundationHostType hostType = requestContext.ServiceHost.HostType;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && hostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        hostType &= ~TeamFoundationHostType.Application;
      return hostType;
    }
  }
}
