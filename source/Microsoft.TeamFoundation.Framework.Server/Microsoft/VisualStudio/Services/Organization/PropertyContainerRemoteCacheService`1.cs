// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PropertyContainerRemoteCacheService`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Organization
{
  internal class PropertyContainerRemoteCacheService<T> : IVssFrameworkService where T : ICloneable, IPropertyContainer
  {
    private Guid m_namespaceId;
    private Guid m_serviceHostId;
    private ContainerSettings m_containerSettings;
    private static readonly string s_registryRoot = "/Configuration/Organization/Cache/Remote";
    private static readonly string s_namespaceIdRegistryKey = PropertyContainerRemoteCacheService<T>.s_registryRoot + "NamespaceId";
    private static readonly string s_cacheEntryTimeToLiveRegistryKey = PropertyContainerRemoteCacheService<T>.s_registryRoot + "CacheEntryTimeToLive";
    private static readonly string s_registryUpdateFilter = PropertyContainerRemoteCacheService<T>.s_registryRoot + "...";
    private static readonly Guid s_defaultNamespaceId = new Guid("2B45B375-23DD-4938-A617-126009CAFA15");
    private static readonly TimeSpan s_defaultCacheEntryTimeToLive = TimeSpan.FromHours(9.0);
    private const string c_area = "Organization";
    private const string c_layer = "PropertyContainerMemoryCache";
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(OrganizationPerfCounters.StandardSet, "Organization", "PropertyContainerMemoryCache");

    public void ServiceStart(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckDeploymentRequestContext();
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      context.GetService<IVssRegistryService>().RegisterNotification(context, new RegistrySettingsChangedCallback(this.OnRegistryChanged), PropertyContainerRemoteCacheService<T>.s_registryUpdateFilter);
      this.ReloadSettings(context);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    private void OnRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<RegistryEntryCollection>(changedEntries, nameof (changedEntries));
      if (!changedEntries.Any<RegistryEntry>())
        return;
      this.ReloadSettings(context);
    }

    private void ReloadSettings(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      TimeSpan timeSpan = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) PropertyContainerRemoteCacheService<T>.s_cacheEntryTimeToLiveRegistryKey, PropertyContainerRemoteCacheService<T>.s_defaultCacheEntryTimeToLive);
      ContainerSettings containerSettings = new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(timeSpan),
        CiAreaName = nameof (PropertyContainerRemoteCacheService<T>)
      };
      Guid guid = service.GetValue<Guid>(vssRequestContext, (RegistryQuery) PropertyContainerRemoteCacheService<T>.s_namespaceIdRegistryKey, PropertyContainerRemoteCacheService<T>.s_defaultNamespaceId);
      this.m_containerSettings = containerSettings;
      this.m_namespaceId = guid;
    }

    internal bool TryGetValue(
      IVssRequestContext context,
      Guid id,
      out PropertyContainerCacheEntry<T> cachedEntry)
    {
      this.ValidateRequestContext(context);
      if (PropertyContainerRemoteCacheService<T>.IsFeatureDisabled(context))
      {
        cachedEntry = (PropertyContainerCacheEntry<T>) null;
        return false;
      }
      using (PropertyContainerRemoteCacheService<T>.s_tracer.TraceTimedAction(context, PropertyContainerRemoteCacheService<T>.TracePoints.TryGet.Slow, 500, nameof (TryGetValue)))
      {
        ActionTracer.FunctionWithOutParam<bool, PropertyContainerCacheEntry<T>> action = new ActionTracer.FunctionWithOutParam<bool, PropertyContainerCacheEntry<T>>(this.TryGetValue);
        return PropertyContainerRemoteCacheService<T>.s_tracer.TraceAction<bool, PropertyContainerCacheEntry<T>>(context, (ActionTracePoints) PropertyContainerRemoteCacheService<T>.TracePoints.TryGet, action, out cachedEntry, nameof (TryGetValue), (object) context, (object) id);
      }
    }

    private bool TryGetValue(out PropertyContainerCacheEntry<T> cachedEntry, params object[] args)
    {
      IVssRequestContext requestContext = (IVssRequestContext) args[0];
      Guid guid = (Guid) args[1];
      try
      {
        if (!this.GetCacheContainer(requestContext).TryGet<Guid, PropertyContainerCacheEntry<T>>(requestContext, guid, out cachedEntry))
        {
          PropertyContainerRemoteCacheService<T>.s_tracer.TraceCacheMiss(requestContext, 6660012, (object) guid, nameof (TryGetValue));
          return false;
        }
        PropertyContainerRemoteCacheService<T>.s_tracer.TraceCacheHit(requestContext, 6660013, (object) guid, nameof (TryGetValue));
        return true;
      }
      catch (Exception ex)
      {
        PropertyContainerRemoteCacheService<T>.s_tracer.TraceException(requestContext, PropertyContainerRemoteCacheService<T>.TracePoints.TryGet.Exception, ex, nameof (TryGetValue));
        PropertyContainerRemoteCacheService<T>.s_tracer.TraceCacheMiss(requestContext, 6660014, (object) guid, nameof (TryGetValue));
        cachedEntry = (PropertyContainerCacheEntry<T>) null;
        return false;
      }
    }

    internal void Set(
      IVssRequestContext context,
      Guid id,
      PropertyContainerCacheEntry<T> cacheEntry)
    {
      this.ValidateRequestContext(context);
      if (PropertyContainerRemoteCacheService<T>.IsFeatureDisabled(context))
        return;
      using (PropertyContainerRemoteCacheService<T>.s_tracer.TraceTimedAction(context, PropertyContainerRemoteCacheService<T>.TracePoints.Set.Slow, 500, nameof (Set)))
        PropertyContainerRemoteCacheService<T>.s_tracer.TraceAction(context, (ActionTracePoints) PropertyContainerRemoteCacheService<T>.TracePoints.Set, (Action) (() =>
        {
          try
          {
            IMutableDictionaryCacheContainer<Guid, PropertyContainerCacheEntry<T>> cacheContainer = this.GetCacheContainer(context);
            Dictionary<Guid, PropertyContainerCacheEntry<T>> dictionary = new Dictionary<Guid, PropertyContainerCacheEntry<T>>()
            {
              {
                id,
                cacheEntry
              }
            };
            IVssRequestContext requestContext = context;
            Dictionary<Guid, PropertyContainerCacheEntry<T>> items = dictionary;
            cacheContainer.Set(requestContext, (IDictionary<Guid, PropertyContainerCacheEntry<T>>) items);
          }
          catch (Exception ex)
          {
            PropertyContainerRemoteCacheService<T>.s_tracer.TraceException(context, PropertyContainerRemoteCacheService<T>.TracePoints.Set.Exception, ex, nameof (Set));
          }
        }), nameof (Set));
    }

    internal bool Remove(IVssRequestContext context, Guid containerId)
    {
      this.ValidateRequestContext(context);
      if (PropertyContainerRemoteCacheService<T>.IsFeatureDisabled(context))
        return false;
      using (PropertyContainerRemoteCacheService<T>.s_tracer.TraceTimedAction(context, PropertyContainerRemoteCacheService<T>.TracePoints.Remove.Slow, 500, nameof (Remove)))
        return PropertyContainerRemoteCacheService<T>.s_tracer.TraceAction<bool>(context, (ActionTracePoints) PropertyContainerRemoteCacheService<T>.TracePoints.Remove, (Func<bool>) (() =>
        {
          try
          {
            this.GetCacheContainer(context).Invalidate<Guid, PropertyContainerCacheEntry<T>>(context, containerId);
            PropertyContainerRemoteCacheService<T>.s_tracer.TraceCacheInvalidation(context, 6660032, new Func<string>(((object) containerId).ToString), nameof (Remove));
            return true;
          }
          catch (Exception ex)
          {
            PropertyContainerRemoteCacheService<T>.s_tracer.TraceException(context, PropertyContainerRemoteCacheService<T>.TracePoints.Remove.Exception, ex, nameof (Remove));
            return false;
          }
        }), nameof (Remove));
    }

    private IMutableDictionaryCacheContainer<Guid, PropertyContainerCacheEntry<T>> GetCacheContainer(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<Guid, PropertyContainerCacheEntry<T>, PropertyContainerRemoteCacheService<T>.PropertyCacheContainerSecurityToken>(requestContext, this.m_namespaceId, this.m_containerSettings);
    }

    private static bool IsFeatureDisabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Organization.PropertyContainerRemoteCache.Disable") || !requestContext.GetService<IRedisCacheService>().IsEnabled(requestContext);

    private void ValidateRequestContext(IVssRequestContext context) => context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);

    private class PropertyCacheContainerSecurityToken
    {
    }

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints TryGet = new TimedActionTracePoints(6660010, 6660017, 6660018, 6660019);
      internal static readonly TimedActionTracePoints Set = new TimedActionTracePoints(6660020, 6660027, 6660028, 6660029);
      internal static readonly TimedActionTracePoints Remove = new TimedActionTracePoints(6660030, 6660037, 6660038, 6660039);
    }

    private static class MethodNames
    {
      internal const string TryGetValue = "TryGetValue";
    }
  }
}
