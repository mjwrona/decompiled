// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Internal.HostRoutingCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server.Internal
{
  internal class HostRoutingCacheService : IVssFrameworkService, IHostRoutingCache
  {
    internal static readonly Guid HostRoutingChangedEvent = new Guid("D56127F1-7FA8-4507-8A4E-9DF7E05A5BF0");
    private INotificationRegistration m_hostRoutingCacheRegistration;
    private const string c_traceLayer = "HostRoutingCacheService";
    private const string c_traceArea = "Microsoft.TeamFoundation.Framework.Server";

    public bool TryGetValue(
      IVssRequestContext deploymentContext,
      Guid key,
      out HostProxyData value)
    {
      return deploymentContext.GetService<HostRoutingCacheService.NestedHostRoutingCache>().TryGetValue(deploymentContext, key, out value);
    }

    public void Set(IVssRequestContext deploymentContext, Guid key, HostProxyData value) => deploymentContext.GetService<HostRoutingCacheService.NestedHostRoutingCache>().Set(deploymentContext, key, value);

    public void Remove(IVssRequestContext deploymentContext, Guid key) => deploymentContext.GetService<HostRoutingCacheService.NestedHostRoutingCache>().Remove(deploymentContext, key);

    internal void RemoveAndNotify(IVssRequestContext deploymentContext, Guid key)
    {
      this.Remove(deploymentContext, key);
      deploymentContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(deploymentContext, HostRoutingCacheService.HostRoutingChangedEvent, key.ToString("D"));
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      systemRequestContext.GetService<HostRoutingCacheService.NestedHostRoutingCache>().SetExpiryInterval(systemRequestContext, service.GetValue<TimeSpan>(systemRequestContext, (RegistryQuery) FrameworkServerConstants.RoutingServiceCacheEntryTimeout, HostRoutingCacheConstants.DefaultExpiryInterval));
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), FrameworkServerConstants.RoutingServiceCacheEntryTimeout);
      this.m_hostRoutingCacheRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", HostRoutingCacheService.HostRoutingChangedEvent, new SqlNotificationHandler(this.OnHostRoutingChanged), true, false);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_hostRoutingCacheRegistration.Unregister(systemRequestContext);
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    public HostProxyData Get(
      IVssRequestContext deploymentContext,
      Guid hostId,
      RouteFlags routeFlags)
    {
      deploymentContext.CheckDeploymentRequestContext();
      HostProxyData hostProxyData;
      if (!this.TryGetValue(deploymentContext, hostId, out hostProxyData))
      {
        hostProxyData = deploymentContext.GetService<IHostProxyDataProvider>().LoadHostProxyData(deploymentContext, hostId, routeFlags);
        if (hostProxyData == null)
        {
          Thread.Sleep(200);
          return (HostProxyData) null;
        }
        if (hostProxyData.TargetInstanceId != deploymentContext.ServiceHost.InstanceId)
          this.Set(deploymentContext, hostId, hostProxyData);
      }
      return hostProxyData;
    }

    private void OnRegistryChanged(
      IVssRequestContext deploymentContext,
      RegistryEntryCollection changedEntries)
    {
      RegistryEntry entry;
      if (!changedEntries.TryGetValue(FrameworkServerConstants.RoutingServiceCacheEntryTimeout, out entry))
        return;
      deploymentContext.GetService<HostRoutingCacheService.NestedHostRoutingCache>().SetExpiryInterval(deploymentContext, entry.GetValue<TimeSpan>(HostRoutingCacheConstants.DefaultExpiryInterval));
    }

    private void OnHostRoutingChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      Guid result;
      if (!Guid.TryParse(args.Data, out result))
        return;
      this.Remove(requestContext, result);
      requestContext.TraceAlways(41694535, TraceLevel.Info, "Microsoft.TeamFoundation.Framework.Server", nameof (HostRoutingCacheService), "Invalidated host routing cache for host {0}.", (object) result);
    }

    public class NestedHostRoutingCache : 
      LocalAndRedisCache<Guid, HostProxyData, HostRoutingCacheService.SecurityToken>
    {
      internal VssRefreshCache<object> m_redisEnabled;
      private const string RedisCachingFeatureName = "VisualStudio.FrameworkService.RoutingService.RedisCache";
      protected static readonly Guid s_namespace = new Guid("DC693C2E-A099-45BB-A916-6BC0AE16BDF2");

      public NestedHostRoutingCache()
        : base(new LocalAndRedisCacheConfiguration().WithRedisNamespace(HostRoutingCacheService.NestedHostRoutingCache.s_namespace))
      {
      }

      public override void ServiceStart(IVssRequestContext systemContext)
      {
        systemContext.CheckDeploymentRequestContext();
        base.ServiceStart(systemContext);
        this.m_redisEnabled = new VssRefreshCache<object>(TimeSpan.FromSeconds(30.0), (Func<IVssRequestContext, object>) (requestContext => requestContext.GetService<IRedisCacheService>().IsEnabled(requestContext) && requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.RoutingService.RedisCache") ? new object() : (object) null));
      }

      protected override bool IsRedisEnabled(IVssRequestContext deploymentContext)
      {
        if (this.m_redisEnabled.Get(deploymentContext) != null)
          return true;
        this.SetReinitializeRedis();
        return false;
      }
    }

    public class SecurityToken
    {
    }
  }
}
