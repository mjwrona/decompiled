// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AfdRouteKeyService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.GeoReplication;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class AfdRouteKeyService : IAfdRouteKeyService, IVssFrameworkService
  {
    private string m_azureInstanceHost;
    private INotificationRegistration m_locationDataChangedRegistration;
    private Dictionary<int, IRouteKeyVersionHandler> m_versionHandlerMap = new Dictionary<int, IRouteKeyVersionHandler>();
    private static readonly VssPerformanceCounter s_afdCacheMissCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureFrontDoorCacheMisses");
    private static readonly RegistryQuery s_drInstanceQuery = (RegistryQuery) "/Configuration/Afd/DisasterRecovery/SecondaryInstance";
    internal static readonly string Area = "HostRouting";
    internal static readonly string Layer = "AfdRouteKeyHandler";
    private static RegistryQuery s_isSingletonService = new RegistryQuery(FrameworkServerConstants.IsSingletonService, false);

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      this.m_locationDataChangedRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.LocationDataChanged, new SqlNotificationCallback(this.OnLocationDataChanged), true, false);
      Interlocked.CompareExchange<string>(ref this.m_azureInstanceHost, AfdRouteKeyService.GetAzureInstanceHost(systemRequestContext), (string) null);
      this.RegisterRouteKeyHandler<RouteKeyVersion0Handler>();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      this.m_locationDataChangedRegistration.Unregister(systemRequestContext);
    }

    public void RegisterRouteKeyHandler(IRouteKeyVersionHandler handler) => this.m_versionHandlerMap[handler.Version] = handler;

    internal void OnLocationDataChanged(
      IVssRequestContext systemRequestContext,
      Guid eventClass,
      string eventData)
    {
      Interlocked.Exchange<string>(ref this.m_azureInstanceHost, AfdRouteKeyService.GetAzureInstanceHost(systemRequestContext));
    }

    private static string GetAzureInstanceHost(IVssRequestContext systemRequestContext)
    {
      string host = systemRequestContext.GetService<ILocationService>().GetAccessMapping(systemRequestContext, AccessMappingConstants.AzureInstanceMappingMoniker).AccessPoint.AsUri().Host;
      string str1 = systemRequestContext.GetService<IVssRegistryService>().GetValue(systemRequestContext, in AfdRouteKeyService.s_drInstanceQuery);
      string azureInstanceHost = systemRequestContext.GetService<IGeoReplicationService>().GetSecondaryAzureInstanceHost(systemRequestContext);
      string str2 = !string.IsNullOrEmpty(azureInstanceHost) ? "," + azureInstanceHost : "";
      string str3 = !string.IsNullOrEmpty(str1) ? "," + str1 : "";
      return host + str2 + str3;
    }

    public void SetAfdRouteKeyHeaders(
      IVssRequestContext requestContext,
      HostRouteContext routeContext,
      HttpContextBase httpContext,
      IProxyData proxyData)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      string header1 = httpContext.Response.Headers["X-AS-RouteKey"];
      if (!string.IsNullOrEmpty(header1))
      {
        requestContext.Trace(470877433, TraceLevel.Info, AfdRouteKeyService.Area, AfdRouteKeyService.Layer, "Route key was already provided: '{0}'", (object) header1);
      }
      else
      {
        if (!routeContext.RouteKeyVersion.HasValue || string.IsNullOrEmpty(routeContext.ExpectedRouteKey))
          return;
        string header2 = httpContext.Request.Headers["X-FD-RouteKey"];
        string header3 = httpContext.Request.Headers["X-FD-RouteKeyApplicationEndpointList"];
        if (string.IsNullOrEmpty(header2))
          return;
        if (!this.UseRouteKeysCache(requestContext))
        {
          if (string.IsNullOrEmpty(header3))
            return;
          requestContext.Trace(1240097083, TraceLevel.Info, AfdRouteKeyService.Area, AfdRouteKeyService.Layer, "Setting outgoing route keys and endpoint AFD headers to empty the route key cache for route keys {0}", (object) header2);
          httpContext.Response.Headers.Set("X-AS-RouteKey", header2);
          httpContext.Response.Headers.Set("X-AS-RouteKeyApplicationEndpointList", "{clear}");
        }
        else
        {
          string unversionedRouteKey = AfdRouteKeyUtils.ExtractUnversionedRouteKey(header2, routeContext.RouteKeyVersion.Value);
          if (string.IsNullOrEmpty(unversionedRouteKey))
            requestContext.Trace(470877433, TraceLevel.Info, AfdRouteKeyService.Area, AfdRouteKeyService.Layer, "Provided route key '{0}' did not supply version {1}.", (object) header2, (object) routeContext.RouteKeyVersion);
          else if (!string.Equals(routeContext.ExpectedRouteKey.Trim(), unversionedRouteKey, StringComparison.OrdinalIgnoreCase))
          {
            if (!requestContext.ExecutionEnvironment.IsDevFabricDeployment && !requestContext.ServiceHost.IsProduction)
              return;
            int tracepoint = httpContext.Request.Cookies["X-VSS-DeploymentAffinity"] == null ? 2101416394 : 2101416395;
            requestContext.Trace(tracepoint, TraceLevel.Error, AfdRouteKeyService.Area, AfdRouteKeyService.Layer, "Route Key '" + unversionedRouteKey + "' does not match expected value '" + routeContext.ExpectedRouteKey + "'.");
          }
          else
          {
            IRouteKeyVersionHandler keyVersionHandler;
            if (this.m_versionHandlerMap.TryGetValue(routeContext.RouteKeyVersion.Value, out keyVersionHandler) && !keyVersionHandler.ShouldUpdateEndpointForRouteKey(requestContext, httpContext.Request.Url, routeContext.RouteFlags, proxyData))
              return;
            string str;
            if (proxyData == null)
            {
              str = (string) null;
            }
            else
            {
              string targetInstanceUrl = proxyData.TargetInstanceUrl;
              str = targetInstanceUrl != null ? targetInstanceUrl.AsUri()?.Host : (string) null;
            }
            if (str == null)
              str = this.m_azureInstanceHost;
            string y = str;
            if (StringComparer.OrdinalIgnoreCase.Equals(header3, y))
              return;
            httpContext.Response.Headers.Set("X-AS-RouteKey", header2);
            httpContext.Response.Headers.Set("X-AS-RouteKeyApplicationEndpointList", y);
            AfdRouteKeyService.s_afdCacheMissCounter.Increment();
          }
        }
      }
    }

    public void ResetAfdRouteKeyHeaders(
      IVssRequestContext requestContext,
      HttpContextBase httpContext)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      httpContext.Response.Headers.Remove("X-AS-RouteKey");
      httpContext.Response.Headers.Remove("X-AS-RouteKeyApplicationEndpointList");
    }

    private bool UseRouteKeysCache(IVssRequestContext requestContext)
    {
      int num = requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in AfdRouteKeyService.s_isSingletonService, false) ? 1 : 0;
      bool flag = requestContext.GetService<IGeoReplicationService>().GetGeoReplicationMode(requestContext) != 0;
      return num == 0 || !flag;
    }
  }
}
