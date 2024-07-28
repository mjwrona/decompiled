// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.RouteDirectionConstraint
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using System.Web;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.SignalR
{
  public sealed class RouteDirectionConstraint : IRouteConstraint
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
