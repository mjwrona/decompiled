// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.SignalR.TfsSignalRUtility
// Assembly: Microsoft.TeamFoundation.Server.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0378695-D47C-46CB-A501-9188B19EA4AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.SignalR.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.SignalR
{
  public static class TfsSignalRUtility
  {
    public static string GetHubsUrl(IVssRequestContext requestContext)
    {
      ProjectInfo projectInfo;
      string url;
      return requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.RootContext.Items.TryGetValue<ProjectInfo>("RequestProject", out projectInfo) && TfsSignalRUtility.TryGetUrl(requestContext, "tfs.signalr.hubs.project", (object) new
      {
        signalrObject = "hubs",
        project = projectInfo.Id.ToString("D")
      }, out url) ? url : TfsSignalRUtility.GetUrl(requestContext, (object) new
      {
        signalrObject = "hubs"
      }, "signalr/{signalrObject}");
    }

    public static string GetConnectionUrl(IVssRequestContext requestContext)
    {
      ProjectInfo projectInfo;
      string url;
      return requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.RootContext.Items.TryGetValue<ProjectInfo>("RequestProject", out projectInfo) && TfsSignalRUtility.TryGetUrl(requestContext, "tfs.signalr.project", (object) new
      {
        project = projectInfo.Id.ToString("D")
      }, out url) ? url : TfsSignalRUtility.GetUrl(requestContext, (object) new
      {
      }, "signalr/{*operation}");
    }

    private static bool TryGetUrl(
      IVssRequestContext requestContext,
      string routeName,
      object routeValues,
      out string url)
    {
      url = (string) null;
      if (!(RouteTable.Routes[routeName] is Route route))
        return false;
      url = TfsSignalRUtility.GetUrl(requestContext, routeValues, route.Url);
      return true;
    }

    private static string GetUrl(
      IVssRequestContext requestContext,
      object routeValues,
      string routeUrl)
    {
      string hostPath = VirtualPathUtility.ToAbsolute(requestContext.VirtualPath());
      if (TfsSignalRUtility.UseSignalRAppPool(requestContext))
        hostPath = TfsSignalRUtility.AddSignalRToHostPath(requestContext, hostPath);
      string str = VssHttpUriUtility.ReplaceRouteValues(routeUrl, VssHttpUriUtility.ToRouteDictionary(routeValues)).TrimStart('/');
      return hostPath.TrimEnd('/') + "/" + str;
    }

    private static string AddSignalRToHostPath(IVssRequestContext requestContext, string hostPath)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        string str1 = requestContext.WebApplicationPath();
        if (hostPath.IndexOf(str1, StringComparison.Ordinal) == 0)
        {
          string str2 = hostPath.Replace(requestContext.WebApplicationPath(), "");
          hostPath = str1 + "_signalr" + "/" + str2;
        }
      }
      else if (hostPath.StartsWith("/"))
        hostPath = "/" + "_signalr" + hostPath;
      return hostPath;
    }

    private static bool UseSignalRAppPool(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.SignalR.AppPool") && requestContext.ExecutionEnvironment.IsHostedDeployment;
  }
}
