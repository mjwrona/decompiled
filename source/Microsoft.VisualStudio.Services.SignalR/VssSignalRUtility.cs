// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.VssSignalRUtility
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.SignalR
{
  public static class VssSignalRUtility
  {
    public static string GetHubsUrl(IVssRequestContext requestContext)
    {
      string hubsUrl = (string) null;
      if (RouteTable.Routes["signalr.hubs"] is Route route)
      {
        IVssRequestContext requestContext1 = requestContext;
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          requestContext1 = requestContext1.To(TeamFoundationHostType.Application);
        hubsUrl = VirtualPathUtility.ToAbsolute(requestContext1.VirtualPath()).TrimEnd('/') + "/" + VssHttpUriUtility.ReplaceRouteValues(route.Url, VssHttpUriUtility.ToRouteDictionary((object) new
        {
          signalrObject = "hubs"
        })).TrimStart('/');
      }
      return hubsUrl;
    }
  }
}
