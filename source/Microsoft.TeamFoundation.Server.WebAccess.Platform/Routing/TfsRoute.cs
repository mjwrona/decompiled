// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Routing.TfsRoute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Routing;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Routing
{
  public class TfsRoute : VssfMVCRoute
  {
    internal const string ServiceHostToken = "{serviceHost}/";
    private TeamFoundationHostType m_hostType;

    public TfsRoute(TeamFoundationHostType hostType, string address, IRouteHandler routeHandler)
      : this(hostType, address, (RouteValueDictionary) null, (RouteValueDictionary) null, (RouteValueDictionary) null, routeHandler)
    {
    }

    public TfsRoute(
      TeamFoundationHostType hostType,
      string address,
      RouteValueDictionary defaults,
      IRouteHandler routeHandler)
      : this(hostType, address, defaults, (RouteValueDictionary) null, (RouteValueDictionary) null, routeHandler)
    {
    }

    public TfsRoute(
      TeamFoundationHostType hostType,
      string address,
      RouteValueDictionary defaults,
      RouteValueDictionary constraints,
      IRouteHandler routeHandler)
      : this(hostType, address, defaults, constraints, (RouteValueDictionary) null, routeHandler)
    {
    }

    public TfsRoute(
      TeamFoundationHostType hostType,
      string address,
      RouteValueDictionary defaults,
      RouteValueDictionary constraints,
      RouteValueDictionary dataTokens,
      IRouteHandler routeHandler)
      : base("{serviceHost}/" + address, defaults, constraints, dataTokens, routeHandler, address, false)
    {
      this.m_hostType = hostType;
    }

    public string RouteArea { get; set; }

    public bool IgnoreRouteArea { get; set; }

    public override RouteData GetRouteData(HttpContextBase httpContext)
    {
      RouteData routeData = (RouteData) null;
      IVssRequestContext requestContext = httpContext.TfsRequestContext();
      if (requestContext != null && (requestContext.IntendedHostType() & this.m_hostType) != TeamFoundationHostType.Unknown && (requestContext.WebRequestContextInternal().RequestRestrictions.AllowedHandlers & AllowedHandler.TfsController) != AllowedHandler.None)
      {
        routeData = base.GetRouteData(httpContext);
        if (routeData != null)
        {
          routeData.Values["serviceHost"] = (object) requestContext.ServiceHost;
          string controllerName = routeData.Values.GetValue<string>("controller", (string) null);
          routeData.DataTokens["originalController"] = (object) controllerName;
          if (!this.IgnoreRouteArea)
          {
            string routeArea = routeData.GetRouteArea();
            string actualControllerName;
            if (!string.IsNullOrEmpty(routeArea) && TfsRouteAreaConstraint.GetSupportedLevels(routeArea, controllerName, out actualControllerName) != NavigationContextLevels.None)
              routeData.Values["controller"] = (object) actualControllerName;
          }
        }
      }
      return routeData;
    }

    public override VirtualPathData GetVirtualPath(
      RequestContext requestContext,
      RouteValueDictionary values)
    {
      string y = (string) null;
      string routeArea = this.RouteArea;
      if (!this.IgnoreRouteArea)
      {
        y = PlatformRouteHelpers.ExtractTargetRouteArea(requestContext, values);
        switch (y)
        {
          case null:
            break;
          case "":
            if (!string.IsNullOrEmpty(routeArea))
              return (VirtualPathData) null;
            break;
          default:
            if (routeArea == null)
              return (VirtualPathData) null;
            if (!StringComparer.OrdinalIgnoreCase.Equals(this.RouteArea, y))
              return (VirtualPathData) null;
            break;
        }
      }
      RequestContext requestContext1 = requestContext;
      IVssRequestContext requestContext2 = requestContext1.TfsRequestContext();
      RouteValueDictionary values1 = new RouteValueDictionary((IDictionary<string, object>) values);
      TfsServiceHostDescriptor serviceHostDescriptor = TfsRoute.FixServiceHost(requestContext2, values1);
      if (requestContext.RouteData != null)
      {
        if (!requestContext.RouteData.Values.ContainsKey("serviceHost"))
          requestContext.RouteData.Values["serviceHost"] = (object) requestContext2.ServiceHost;
        requestContext1 = (RequestContext) new WrappedRequestContext(requestContext);
        RouteValueDictionary values2 = requestContext1.RouteData.Values;
        if (!string.IsNullOrEmpty(routeArea))
          this.RemoveControllerPrefix(values2, routeArea);
        foreach (string key in values1.Keys)
          values2.Remove(key);
        serviceHostDescriptor = TfsRoute.FixServiceHost(requestContext2, values2) ?? serviceHostDescriptor;
        if (serviceHostDescriptor != null && (requestContext2.IntendedHostType(serviceHostDescriptor.HostType) & this.m_hostType) == TeamFoundationHostType.Unknown)
          return (VirtualPathData) null;
        values2.Remove("routeArea");
        requestContext1.RouteData.DataTokens["routeArea"] = (object) y;
      }
      values1.Remove("routeArea");
      if (!string.IsNullOrEmpty(routeArea))
        this.RemoveControllerPrefix(values1, routeArea);
      VirtualPathData virtualPath = base.GetVirtualPath(requestContext1, values1);
      if (virtualPath != null && serviceHostDescriptor != null && string.IsNullOrEmpty(serviceHostDescriptor.ToString()) && virtualPath.VirtualPath.StartsWith("/", StringComparison.Ordinal))
        virtualPath.VirtualPath = virtualPath.VirtualPath.Substring(1);
      return virtualPath;
    }

    private void RemoveControllerPrefix(RouteValueDictionary values, string prefix)
    {
      string str = values.GetValue<string>("controller", (string) null);
      if (string.IsNullOrEmpty(str) || !str.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        return;
      values["controller"] = (object) str.Substring(prefix.Length);
    }

    private static TfsServiceHostDescriptor FixServiceHost(
      IVssRequestContext requestContext,
      RouteValueDictionary values)
    {
      TfsServiceHostDescriptor serviceHostDescriptor = (TfsServiceHostDescriptor) null;
      if (values.ContainsKey("serviceHost"))
      {
        object obj = values["serviceHost"];
        switch (obj)
        {
          case TfsServiceHostDescriptor _:
            serviceHostDescriptor = (TfsServiceHostDescriptor) obj;
            goto label_11;
          case IVssServiceHost serviceHost:
            if (requestContext.ServiceHost.InstanceId == serviceHost.InstanceId)
            {
              serviceHostDescriptor = new TfsServiceHostDescriptor(requestContext);
              break;
            }
            IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
            IHostUriData uriData1 = vssRequestContext1.GetService<IInternalUrlHostResolutionService>().ResolveUriData(vssRequestContext1, serviceHost.InstanceId);
            serviceHostDescriptor = new TfsServiceHostDescriptor(serviceHost, uriData1 != null ? uriData1.AbsoluteVirtualPath() : (string) null);
            break;
          case HostProperties serviceHostProperties:
            if (requestContext.ServiceHost.InstanceId == serviceHostProperties.Id)
            {
              serviceHostDescriptor = new TfsServiceHostDescriptor(requestContext);
              break;
            }
            IVssRequestContext vssRequestContext2 = requestContext.To(TeamFoundationHostType.Deployment);
            IHostUriData uriData2 = vssRequestContext2.GetService<IInternalUrlHostResolutionService>().ResolveUriData(vssRequestContext2, serviceHostProperties.Id);
            serviceHostDescriptor = new TfsServiceHostDescriptor(serviceHostProperties, uriData2 != null ? uriData2.AbsoluteVirtualPath() : (string) null);
            break;
          default:
            throw new NotSupportedException();
        }
        values["serviceHost"] = (object) serviceHostDescriptor;
      }
label_11:
      return serviceHostDescriptor;
    }
  }
}
