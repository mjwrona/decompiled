// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Routing.RouteDirectionConstraint
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System.Web;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Routing
{
  public class RouteDirectionConstraint : IRouteConstraint
  {
    private readonly RouteDirection m_routeDirection;

    public RouteDirectionConstraint(RouteDirection routeDirection) => this.m_routeDirection = routeDirection;

    public bool Match(
      HttpContextBase httpContext,
      Route route,
      string parameterName,
      RouteValueDictionary values,
      RouteDirection routeDirection)
    {
      return routeDirection == this.m_routeDirection;
    }
  }
}
