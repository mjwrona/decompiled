// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Http.SmartRouterRequestHandlerService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.SmartRouter.IpResolution;
using Microsoft.TeamFoundation.Framework.Server.SmartRouterExtensions;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using System;
using System.Diagnostics;
using System.Web;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Http
{
  public class SmartRouterRequestHandlerService : 
    SmartRouterFrameworkServiceBase,
    ISmartRouterRequestHandlerService,
    IVssFrameworkService
  {
    public SmartRouterRequestHandlerService()
      : base(SmartRouterBase.TraceLayer.Http)
    {
    }

    public bool BeginRequest(IVssWebRequestContext requestContext)
    {
      requestContext.CheckRequestContext();
      try
      {
        if (!this.IsEnabled((IVssRequestContext) requestContext))
          return false;
        SmartRouterContext smartRouterContext = requestContext.GetSmartRouterContext();
        SmartRouterRequestHandlerService.PerfCounters.TotalRequests.Increment();
        if (!this.ShouldRouteRequest(requestContext, smartRouterContext))
          return false;
        SmartRouterRequestHandlerService.PerfCounters.TotalRoutable.Increment();
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
          ServerNodeWithHash targetServerNode = requestContext.GetService<ISmartRouterService>().GetTargetServerNode((IVssRequestContext) requestContext, smartRouterContext);
          if (targetServerNode == null)
          {
            this.Tracer.TraceVerbose((IVssRequestContext) requestContext, SmartRouterBase.TracePoint.NoTargetServer, "Unable to route this request, unable to pick a target server.");
            return false;
          }
          SmartRouterRequestHandlerService.PerfCounters.TotalTargetSelected.Increment();
          this.Tracer.TraceVerbose((IVssRequestContext) requestContext, SmartRouterBase.TracePoint.TargetServer, "Routed request to roleInstance='{0}', ipAddress='{1}', affinityCookie='{2}'", (object) targetServerNode.Server.RoleInstance, (object) targetServerNode.Server.IPAddress, (object) targetServerNode.AffinityCookie);
          return this.BeginSmartRoutedHttpContext(requestContext, smartRouterContext, targetServerNode);
        }
        finally
        {
          SmartRouterRequestHandlerService.PerfCounters.AverageRoutingTime.IncrementTicks(stopwatch);
          SmartRouterRequestHandlerService.PerfCounters.AverageRoutingTimeBase.Increment();
        }
      }
      catch (Exception ex)
      {
        requestContext.GetSmartRouterContext().SetException(ex);
        SmartRouterRequestHandlerService.PerfCounters.TotalExceptions.Increment();
        this.Tracer.TraceException((IVssRequestContext) requestContext, SmartRouterBase.TracePoint.BeginRequestException, ex, (string) null);
        throw;
      }
    }

    internal bool ShouldRouteRequest(
      IVssWebRequestContext requestContext,
      SmartRouterContext smartRouterContext,
      string? userHostAddress = null,
      LocalHostAddressCallback? localHostCallback = null)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        smartRouterContext.SetNotRoutable("IsDeploymentHost");
        this.Tracer.TraceVerbose((IVssRequestContext) requestContext, SmartRouterBase.TracePoint.ShouldNotRouteRequest, "Do not route, host type not supported");
        return false;
      }
      if (!requestContext.ServiceHost.IsHostProcessType(HostProcessType.ApplicationTier))
      {
        smartRouterContext.SetNotRoutable("IsNotApplicationTier");
        this.Tracer.TraceVerbose((IVssRequestContext) requestContext, SmartRouterBase.TracePoint.ShouldNotRouteRequest, "Do not route, process type not supported, processType={0}", (object) requestContext.ServiceHost.DeploymentServiceHost.ProcessType);
        return false;
      }
      if (requestContext.IsSystemContext)
      {
        smartRouterContext.SetNotRoutable("IsSystemContext");
        this.Tracer.TraceVerbose((IVssRequestContext) requestContext, SmartRouterBase.TracePoint.ShouldNotRouteRequest, "Do not route, system context, url={0}", (object) requestContext.RequestUriForTracing());
        return false;
      }
      if (requestContext.IsStaticContent())
      {
        smartRouterContext.SetNotRoutable("IsStaticContent");
        this.Tracer.TraceVerbose((IVssRequestContext) requestContext, SmartRouterBase.TracePoint.ShouldNotRouteRequest, "Do not route, request is static content: url={0}", (object) requestContext.RequestUriForTracing());
        return false;
      }
      if (requestContext.IsRequestArrRouted())
      {
        smartRouterContext.SetNotRoutable("IsArrRouted");
        this.Tracer.TraceVerbose((IVssRequestContext) requestContext, SmartRouterBase.TracePoint.ShouldNotRouteRequest, "Do not route, request is already ARR routed: url={0}", (object) requestContext.RequestUriForTracing());
        return false;
      }
      if (requestContext.IsRequestArrForwardedFor())
      {
        smartRouterContext.SetNotRoutable("IsArrForwardedFor");
        this.Tracer.TraceVerbose((IVssRequestContext) requestContext, SmartRouterBase.TracePoint.ShouldNotRouteRequest, "Do not route, request is ARR forwarded: url={0}", (object) requestContext.RequestUriForTracing());
        return false;
      }
      string forwardedFor;
      if (requestContext.IsReverseProxyTarget(out forwardedFor))
      {
        if (requestContext.IsSmartRouterForwardingIpValidationEnabled())
        {
          if (userHostAddress == null)
          {
            try
            {
              userHostAddress = requestContext is IWebRequestContextInternal requestContextInternal ? requestContextInternal.HttpContext.Request.UserHostAddress : (string) null;
            }
            catch
            {
            }
          }
          SmartRouterIpStatus smartRouterIpStatus = this.IpValidator.Validate((IVssRequestContext) requestContext, forwardedFor, userHostAddress, localHostCallback);
          switch (smartRouterIpStatus)
          {
            case SmartRouterIpStatus.Valid:
              smartRouterContext.SetReverseProxyReceived();
              SmartRouterRequestHandlerService.PerfCounters.TotalReverseProxyReceived.Increment();
              this.Tracer.TraceVerbose((IVssRequestContext) requestContext, SmartRouterBase.TracePoint.ShouldNotRouteRequest, "Do not route, request is already smart routed, url={0}", (object) requestContext.RequestUriForTracing());
              return false;
            case SmartRouterIpStatus.Invalid:
              this.Tracer.TraceVerbose((IVssRequestContext) requestContext, SmartRouterBase.TracePoint.InvalidIpAddress, "Invalid X-SmartRouter-ForwardedFor={0}, or invalid userHostAddress={1}", (object) forwardedFor, (object) userHostAddress);
              break;
            case SmartRouterIpStatus.InvalidUserHost:
            case SmartRouterIpStatus.InvalidLocalHost:
              smartRouterContext.SetNotRoutable("IpValidationFailed");
              this.Tracer.TraceWarning((IVssRequestContext) requestContext, SmartRouterBase.TracePoint.ShouldNotRouteRequest, "Do not route, ip validation failed, userHostAddress={0}, ipStatus={1}", (object) userHostAddress, (object) smartRouterIpStatus);
              return false;
          }
        }
        else
        {
          smartRouterContext.SetReverseProxyReceived();
          SmartRouterRequestHandlerService.PerfCounters.TotalReverseProxyReceived.Increment();
          this.Tracer.TraceVerbose((IVssRequestContext) requestContext, SmartRouterBase.TracePoint.ShouldNotRouteRequest, "Do not route, request is already smart routed, url={0}", (object) requestContext.RequestUriForTracing());
          return false;
        }
      }
      smartRouterContext.SetRoutabled();
      return true;
    }

    internal bool BeginSmartRoutedHttpContext(
      IVssWebRequestContext requestContext,
      SmartRouterContext smartRouterContext,
      ServerNodeWithHash targetServer)
    {
      if (!requestContext.IsSmartRouterReverseProxyFeatureEnabled())
      {
        smartRouterContext.SetRoutedReason("!ReverseProxyDisabled");
        return false;
      }
      if (!(requestContext is IWebRequestContextInternal requestContextInternal))
      {
        smartRouterContext.SetRoutedReason("!NotWebContext");
        return false;
      }
      if (!this.ShouldReverseProxy((IVssRequestContext) requestContext, smartRouterContext, targetServer.Server))
        return false;
      HttpContextBase httpContext = requestContextInternal.HttpContext;
      try
      {
        SmartRouterRequestHandlerService.SetHttpContextForArrReverseProxyRouting((IVssRequestContext) requestContext, httpContext, smartRouterContext, targetServer);
      }
      catch (Exception ex)
      {
        SmartRouterRequestHandlerService.ClearHttpContextForArrReverseProxyRouting(httpContext, smartRouterContext, ex.Message);
        throw;
      }
      return true;
    }

    private bool ShouldReverseProxy(
      IVssRequestContext requestContext,
      SmartRouterContext smartRouterContext,
      ServerNode targetServer)
    {
      IVssRequestContext deploymentHostContext = requestContext.ToDeploymentHostContext();
      ServerNode publishedLocalServerNode = deploymentHostContext.GetServerNodePublisherService().GetLastPublishedLocalServerNode(deploymentHostContext);
      if ((object) targetServer == null)
        return false;
      if (targetServer != publishedLocalServerNode || requestContext.IsDebugEnvironment())
        return true;
      smartRouterContext.SetRoutedReason("!ReverseProxyToSelf");
      this.Tracer.TraceVerbose(requestContext, SmartRouterBase.TracePoint.NoReverseProxyToSelf, "Do not reverse proxy to self, roleInstance={0}", (object) targetServer.RoleInstance);
      return false;
    }

    private static Uri GetVssRewriteUri(Uri requestUrl, string targetIPAddress) => new UriBuilder(requestUrl)
    {
      Host = targetIPAddress
    }.Uri;

    private static void SetHttpContextForArrReverseProxyRouting(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      SmartRouterContext context,
      ServerNodeWithHash targetServer)
    {
      Uri vssRewriteUri = SmartRouterRequestHandlerService.GetVssRewriteUri(httpContext.Request.Url, targetServer.Server.IPAddress);
      httpContext.Request.ServerVariables[HttpContextConstants.VssRewriteUrl] = vssRewriteUri.OriginalString;
      string str = IpHelper.ResolveClientIp(requestContext, httpContext.Request);
      httpContext.Request.Headers.Set("X-VSS-RequestSmartRouted", bool.TrueString);
      httpContext.Request.Headers.Set("X-SmartRouter-Forwarded-For", str);
      httpContext.Response.Headers.Set("X-VSS-RequestSmartRouted", targetServer.AffinityCookie);
      httpContext.Items[(object) HttpContextConstants.ArrRequestRouted] = (object) true;
      context.SetRoutedReason("ProxySent");
      SmartRouterRequestHandlerService.PerfCounters.TotalReverseProxySent.Increment();
    }

    private static void ClearHttpContextForArrReverseProxyRouting(
      HttpContextBase httpContext,
      SmartRouterContext context,
      string reason)
    {
      httpContext.Request.ServerVariables.Remove(HttpContextConstants.VssRewriteUrl);
      httpContext.Request.Headers.Remove("X-VSS-RequestSmartRouted");
      httpContext.Response.Headers.Remove("X-VSS-RequestSmartRouted");
      httpContext.Items.Remove((object) HttpContextConstants.ArrRequestRouted);
    }

    private ISmartRouterForwardedIpValidator IpValidator { get; } = (ISmartRouterForwardedIpValidator) new SmartRouterForwardedIpValidator();

    private static class PerfCounters
    {
      public static VssPerformanceCounter TotalRequests { get; } = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Cloud.SmartRouter.PerfCounters.TotalRequests");

      public static VssPerformanceCounter TotalRoutable { get; } = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Cloud.SmartRouter.PerfCounters.TotalRoutable");

      public static VssPerformanceCounter TotalTargetSelected { get; } = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Cloud.SmartRouter.PerfCounters.TotalTargetSelected");

      public static VssPerformanceCounter TotalReverseProxySent { get; } = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Cloud.SmartRouter.PerfCounters.TotalReverseProxySent");

      public static VssPerformanceCounter TotalReverseProxyReceived { get; } = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Cloud.SmartRouter.PerfCounters.TotalReverseProxyReceived");

      public static VssPerformanceCounter TotalExceptions { get; } = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Cloud.SmartRouter.PerfCounters.TotalExceptions");

      public static VssPerformanceCounter AverageRoutingTime { get; } = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Cloud.SmartRouter.PerfCounters.AverageRoutingTime");

      public static VssPerformanceCounter AverageRoutingTimeBase { get; } = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Cloud.SmartRouter.PerfCounters.AverageRoutingTimeBase");
    }
  }
}
