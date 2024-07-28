// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AppVsspsHostResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ExtensionPriority(199)]
  [ExtensionStrategy("Hosted")]
  internal class AppVsspsHostResolver : VstsDomainHostResolver
  {
    private string[] m_shardIncludedAPIPaths;
    private string[] m_shardExcludedAPIPaths;
    private string[] m_deploymentAccessPoints;
    private bool m_onlyServeARRTraffic;
    private static readonly Guid SpsSu1InstanceId = new Guid("A5CA35EB-148E-4CCD-BBB3-D31576D75958");
    private static readonly IReadOnlyCollection<Guid> s_allowedInstanceTypes = (IReadOnlyCollection<Guid>) new HashSet<Guid>()
    {
      ServiceInstanceTypes.SPS
    };
    private const string SPSDeploymentUrl = "app.vssps.visualstudio.com";

    public override string Name => nameof (AppVsspsHostResolver);

    public override void Initialize(IVssRequestContext requestContext)
    {
      if (!AppVsspsHostResolver.s_allowedInstanceTypes.Contains<Guid>(requestContext.ServiceInstanceType()))
        return;
      base.Initialize(requestContext);
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnShardIncludeAPIEntriesChanged), "/Configuration/UrlHostResolution/AppVsspsShardedApis");
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnShardExcludeAPIEntriesChanged), "/Configuration/UrlHostResolution/AppVsspsShardExcludeApis");
      this.m_onlyServeARRTraffic = requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Configuration/UrlHostResolution/AppVsspsOnlyServeARRCalls", false, true);
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnOnlyServeARRCallsChanged), "/Configuration/UrlHostResolution/AppVsspsOnlyServeARRCalls");
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.LocationDataChanged, new SqlNotificationCallback(this.OnDeploymentAccessPointsChanged), true);
      Interlocked.CompareExchange<string[]>(ref this.m_shardIncludedAPIPaths, AppVsspsHostResolver.GetApiEntries(requestContext, "/Configuration/UrlHostResolution/AppVsspsShardedApis"), (string[]) null);
      Interlocked.CompareExchange<string[]>(ref this.m_shardExcludedAPIPaths, AppVsspsHostResolver.GetApiEntries(requestContext, "/Configuration/UrlHostResolution/AppVsspsShardExcludeApis"), (string[]) null);
      Interlocked.CompareExchange<string[]>(ref this.m_deploymentAccessPoints, AppVsspsHostResolver.GetDeploymentAccessPoints(requestContext), (string[]) null);
    }

    public override void Shutdown(IVssRequestContext requestContext)
    {
      base.Shutdown(requestContext);
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnShardIncludeAPIEntriesChanged));
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnShardExcludeAPIEntriesChanged));
    }

    public override bool TryResolveHost(
      IVssRequestContext requestContext,
      Uri requestUri,
      string applicationVirtualPath,
      out HostRouteContext routeContext)
    {
      if (!AppVsspsHostResolver.s_allowedInstanceTypes.Contains<Guid>(requestContext.ServiceInstanceType()))
      {
        requestContext.Trace(409291501, TraceLevel.Info, "UrlHostResolutionService", nameof (AppVsspsHostResolver), string.Format("Skipped routing for Instance: {0}, Host: {1}, InstanceId: {2}", (object) requestContext.ServiceInstanceType(), (object) requestUri.Host, (object) requestContext.ServiceHost.InstanceId));
        routeContext = (HostRouteContext) null;
        return false;
      }
      requestContext.Trace(409291502, TraceLevel.Info, "UrlHostResolutionService", nameof (AppVsspsHostResolver), string.Format("Routing request for Instance: {0}, Host: {1}, InstanceId: {2}", (object) requestContext.ServiceInstanceType(), (object) requestUri.Host, (object) requestContext.ServiceHost.InstanceId));
      requestContext.Trace(409291502, TraceLevel.Info, "UrlHostResolutionService", nameof (AppVsspsHostResolver), string.Format("Shard include Apis: {0}, Shard exclude Apis: {1}, Deployment access points: {2}", (object) string.Join(";", this.m_shardIncludedAPIPaths ?? Array.Empty<string>()), (object) string.Join(";", this.m_shardExcludedAPIPaths ?? Array.Empty<string>()), (object) string.Join(";", this.m_deploymentAccessPoints ?? Array.Empty<string>())));
      string empty = string.Empty;
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      if (!instanceId.Equals(AppVsspsHostResolver.SpsSu1InstanceId) && this.IsSPSDeploymentHost(requestUri.Host) && this.IsExcludedAPIPath(requestUri.AbsolutePath, out empty))
      {
        requestContext.Trace(409291503, TraceLevel.Info, "UrlHostResolutionService", nameof (AppVsspsHostResolver), string.Format("Excluded API based on excluded API path {0}", (object) empty));
        int num = base.TryResolveHost(requestContext, new UriBuilder(requestUri)
        {
          Host = "app.vssps.visualstudio.com"
        }.Uri, applicationVirtualPath, out routeContext) ? 1 : 0;
        if (num == 0)
          return num != 0;
        if (routeContext == null)
          return num != 0;
        routeContext.ExpectedRouteKey = (string) null;
        return num != 0;
      }
      instanceId = requestContext.ServiceHost.InstanceId;
      if (!instanceId.Equals(AppVsspsHostResolver.SpsSu1InstanceId) && StringComparer.OrdinalIgnoreCase.Equals(requestUri.Host, "app.vssps.visualstudio.com") && this.IsShardedAPIPath(requestUri.AbsolutePath, out empty))
      {
        requestContext.Trace(409291503, TraceLevel.Info, "UrlHostResolutionService", nameof (AppVsspsHostResolver), string.Format("Sharding API based on included API path {0}", (object) empty));
        routeContext = new HostRouteContext()
        {
          HostId = requestContext.ServiceHost.InstanceId,
          VirtualPath = this.m_rootUri.AbsolutePath,
          WebApplicationPath = this.m_rootUri.AbsolutePath,
          RouteFlags = RouteFlags.DeploymentHost
        };
        return true;
      }
      instanceId = requestContext.ServiceHost.InstanceId;
      if (instanceId.Equals(AppVsspsHostResolver.SpsSu1InstanceId) && this.IsExcludedAPIPath(requestUri.AbsolutePath, out empty) && (!this.m_onlyServeARRTraffic || this.IsARRProxyRequest(requestContext)))
      {
        requestContext.Trace(409291503, TraceLevel.Info, "UrlHostResolutionService", nameof (AppVsspsHostResolver), string.Format("Serving the proxied call {0} locally for SU1", (object) empty));
        routeContext = new HostRouteContext()
        {
          HostId = requestContext.ServiceHost.InstanceId,
          VirtualPath = this.m_rootUri.AbsolutePath,
          WebApplicationPath = this.m_rootUri.AbsolutePath,
          RouteFlags = RouteFlags.DeploymentHost
        };
        return true;
      }
      routeContext = (HostRouteContext) null;
      return false;
    }

    private bool IsARRProxyRequest(IVssRequestContext requestContext)
    {
      if (!(requestContext is IWebRequestContextInternal requestContextInternal))
        return false;
      HttpContextBase httpContext = requestContextInternal.HttpContext;
      return httpContext != null && httpContext.Items.Contains((object) HttpContextConstants.ArrRequestRouted);
    }

    private bool IsShardedAPIPath(string path, out string shardedAPIPath)
    {
      path = VirtualPathUtility.RemoveTrailingSlash(path);
      ref string local = ref shardedAPIPath;
      string[] includedApiPaths = this.m_shardIncludedAPIPaths;
      string str = includedApiPaths != null ? ((IEnumerable<string>) includedApiPaths).FirstOrDefault<string>((Func<string, bool>) (shardedPath => path.StartsWith(shardedPath, StringComparison.OrdinalIgnoreCase))) : (string) null;
      local = str;
      return !string.IsNullOrWhiteSpace(shardedAPIPath);
    }

    private bool IsExcludedAPIPath(string path, out string apiPath)
    {
      path = VirtualPathUtility.RemoveTrailingSlash(path);
      ref string local = ref apiPath;
      string[] excludedApiPaths = this.m_shardExcludedAPIPaths;
      string str = excludedApiPaths != null ? ((IEnumerable<string>) excludedApiPaths).FirstOrDefault<string>((Func<string, bool>) (excludedApiPath => path.StartsWith(excludedApiPath, StringComparison.OrdinalIgnoreCase))) : (string) null;
      local = str;
      return !string.IsNullOrWhiteSpace(apiPath);
    }

    private void OnShardIncludeAPIEntriesChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      try
      {
        this.m_shardIncludedAPIPaths = this.GetApiEntriesOnChange(requestContext, changedEntries, "/Configuration/UrlHostResolution/AppVsspsShardedApis");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(409291501, "UrlHostResolutionService", nameof (AppVsspsHostResolver), ex);
      }
    }

    private void OnShardExcludeAPIEntriesChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      try
      {
        this.m_shardExcludedAPIPaths = this.GetApiEntriesOnChange(requestContext, changedEntries, "/Configuration/UrlHostResolution/AppVsspsShardExcludeApis");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(409291501, "UrlHostResolutionService", nameof (AppVsspsHostResolver), ex);
      }
    }

    private void OnOnlyServeARRCallsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      try
      {
        requestContext.GetService<IVssRegistryService>();
        this.m_onlyServeARRTraffic = requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Configuration/UrlHostResolution/AppVsspsOnlyServeARRCalls", false, true);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(409291501, "UrlHostResolutionService", nameof (AppVsspsHostResolver), ex);
      }
    }

    private string[] GetApiEntriesOnChange(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries,
      string registryKey)
    {
      try
      {
        if (changedEntries != null)
        {
          if (changedEntries.Any<RegistryEntry>())
            return AppVsspsHostResolver.GetApiEntries(requestContext, registryKey);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(409291501, "UrlHostResolutionService", nameof (AppVsspsHostResolver), ex);
      }
      return Array.Empty<string>();
    }

    private static string[] GetApiEntries(IVssRequestContext requestContext, string registryKey) => requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) registryKey, string.Empty).Split(';');

    private bool IsSPSDeploymentHost(string domain)
    {
      if (StringComparer.OrdinalIgnoreCase.Equals(domain, "app.vssps.visualstudio.com"))
        return true;
      foreach (string deploymentAccessPoint in this.m_deploymentAccessPoints)
      {
        if (StringComparer.OrdinalIgnoreCase.Equals(domain, deploymentAccessPoint))
          return true;
      }
      return false;
    }

    private static string[] GetDeploymentAccessPoints(IVssRequestContext requestContext)
    {
      try
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return requestContext.GetService<ILocationService>().GetAccessMappings(requestContext).Where<AccessMapping>(AppVsspsHostResolver.\u003C\u003EO.\u003C0\u003E__FilterAccessMapping ?? (AppVsspsHostResolver.\u003C\u003EO.\u003C0\u003E__FilterAccessMapping = new Func<AccessMapping, bool>(AppVsspsHostResolver.FilterAccessMapping))).Select<AccessMapping, string>((Func<AccessMapping, string>) (x => x.AccessPoint.AsUri().Host)).ToArray<string>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(409291501, "UrlHostResolutionService", nameof (AppVsspsHostResolver), ex);
      }
      return Array.Empty<string>();
    }

    private static bool FilterAccessMapping(AccessMapping accessMapping) => VssStringComparer.AccessMappingMoniker.Equals(accessMapping.Moniker, AccessMappingConstants.PublicAccessMappingMoniker) || VssStringComparer.AccessMappingMoniker.Equals(accessMapping.Moniker, AccessMappingConstants.HostGuidAccessMappingMoniker);

    private void OnDeploymentAccessPointsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      Interlocked.Exchange<string[]>(ref this.m_deploymentAccessPoints, AppVsspsHostResolver.GetDeploymentAccessPoints(requestContext));
    }
  }
}
