// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityPropertiesCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityPropertiesCache : VssCacheBase, IIdentityPropertiesCache
  {
    private readonly VssMemoryCacheList<string, object> _cache;
    private const string TraceArea = "IdentityService";
    private const string TraceLayer = "FrameworkIdentityCache";
    private const int DefaultPropertyCacheExpiryIntervalInHours = 8;
    private const int DefaultPropertyEvictionOperationIntervalInHours = 4;
    internal const string CacheExtendedPropertiesFeatureName = "VisualStudio.IdentityStore.CacheExtendedProperties";
    internal static readonly string[] DefaultExtendedPropertiesCacheWhitelist = new string[6]
    {
      "ConfirmedNotificationAddress",
      "CustomNotificationAddresses",
      "http://schemas.microsoft.com/identity/claims/objectidentifier",
      "AuthenticationCredentialValidFrom",
      "Microsoft.TeamFoundation.Identity.Image.Id",
      "Microsoft.TeamFoundation.Identity.Image.Type"
    };

    public IdentityPropertiesCache(IVssRequestContext requestContext, int propertyCacheSize)
    {
      bool flag = false;
      VssCacheExpiryProvider<string, object> expiryProvider = (VssCacheExpiryProvider<string, object>) null;
      IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
      RegistryQuery registryQuery;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRegistryService registryService1 = service1;
        IVssRequestContext requestContext1 = requestContext;
        registryQuery = (RegistryQuery) "/Service/Integration/Settings/PropertyEvictionEnabled";
        ref RegistryQuery local1 = ref registryQuery;
        flag = registryService1.GetValue<bool>(requestContext1, in local1, false);
        if (flag)
        {
          IVssRegistryService registryService2 = service1;
          IVssRequestContext requestContext2 = requestContext;
          registryQuery = (RegistryQuery) "/Service/Integration/Settings/PropertyEvictionOperationIntervalInHours";
          ref RegistryQuery local2 = ref registryQuery;
          int num = Math.Max(1, registryService2.GetValue<int>(requestContext2, in local2, 8));
          expiryProvider = new VssCacheExpiryProvider<string, object>(Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry), Capture.Create<TimeSpan>(TimeSpan.FromHours((double) num)));
        }
      }
      if (flag)
      {
        this._cache = new VssMemoryCacheList<string, object>((IVssCachePerformanceProvider) this, propertyCacheSize, expiryProvider);
        ITeamFoundationTaskService service2 = requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>();
        IVssRegistryService registryService = service1;
        IVssRequestContext requestContext3 = requestContext;
        registryQuery = (RegistryQuery) "/Service/Integration/Settings/PropertyEvictionOperationIntervalInHours";
        ref RegistryQuery local = ref registryQuery;
        int num = Math.Max(1, registryService.GetValue<int>(requestContext3, in local, 4));
        IVssRequestContext requestContext4 = requestContext;
        TeamFoundationTask task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.EvictExpiredProperties), (object) null, num * 60 * 60 * 1000);
        service2.AddTask(requestContext4, task);
      }
      else
        this._cache = new VssMemoryCacheList<string, object>((IVssCachePerformanceProvider) this, propertyCacheSize);
    }

    public void Dispose(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      requestContext.GetService<ITeamFoundationTaskService>().RemoveTask(requestContext, new TeamFoundationTaskCallback(this.EvictExpiredProperties));
    }

    public void OnCachedIdentityEvicted(object sender, IdentityRemovedEventArgs args)
    {
      TeamFoundationTracingService.TraceEnterRaw(80200, "IdentityService", "FrameworkIdentityCache", "OnCacheIdentityEvicted", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      foreach (KeyValuePair<string, string> propertyCacheKey in (IEnumerable<KeyValuePair<string, string>>) this.BuildIdentityPropertyCacheKeys(args.IdentityId, (IEnumerable<string>) IdentityPropertiesCache.DefaultExtendedPropertiesCacheWhitelist))
        this.Remove(propertyCacheKey.Value);
      TeamFoundationTracingService.TraceLeaveRaw(80209, "IdentityService", "FrameworkIdentityCache", "OnCacheIdentityEvicted");
    }

    private void EvictExpiredProperties(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.TraceEnter(80218, "IdentityService", "FrameworkIdentityCache", nameof (EvictExpiredProperties));
      try
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache.EnableTTLForPropertyCache"))
        {
          requestContext.Trace(80220, TraceLevel.Info, "IdentityService", "FrameworkIdentityCache", "Starting eviction of properties");
          this._cache.Sweep();
          requestContext.Trace(80220, TraceLevel.Info, "IdentityService", "FrameworkIdentityCache", "Eviction of properties is done");
        }
        else
          requestContext.Trace(80220, TraceLevel.Info, "IdentityService", "FrameworkIdentityCache", "Feature flag is disabled, skipping eviction of properties");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(80221, TraceLevel.Error, "IdentityService", "FrameworkIdentityCache", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(80219, "IdentityService", "FrameworkIdentityCache", nameof (EvictExpiredProperties));
      }
    }

    public void FilterUnchangedIdentityProperties(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IEnumerable<string> propertyNameFilters,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (!requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.IdentityStore.CacheExtendedProperties"))
        return;
      requestContext.TraceEnter(80120, "IdentityService", "FrameworkIdentityCache", nameof (FilterUnchangedIdentityProperties));
      if (identity == null || propertyNameFilters == null || !propertyNameFilters.Any<string>())
      {
        requestContext.TraceConditionally(80121, TraceLevel.Warning, "IdentityService", "FrameworkIdentityCache", (Func<string>) (() => string.Format("Invalid FilterUnchangedIdentityProperties invocation with Identity : {0}, PropertyNameFilters : {1} ", (object) (identity?.Id.ToString() ?? "NULL"), propertyNameFilters == null || !propertyNameFilters.Any<string>() ? (object) "NULL" : (object) string.Join(",", propertyNameFilters))));
        requestContext.TraceLeave(80129, "IdentityService", "FrameworkIdentityCache", nameof (FilterUnchangedIdentityProperties));
      }
      else
      {
        requestContext.TraceConditionally(80121, TraceLevel.Warning, "IdentityService", "FrameworkIdentityCache", (Func<string>) (() => string.Format("Extended properties filter check on identity: {0} properties: {1}", (object) identity.Descriptor, (object) string.Join(",", propertyNameFilters))));
        IDictionary<string, string> dictionary = this.BuildIdentityPropertyCacheKeys(identity.Id, propertyNameFilters);
        if (dictionary.Count == 0)
        {
          requestContext.TraceConditionally(80121, TraceLevel.Warning, "IdentityService", "FrameworkIdentityCache", (Func<string>) (() => string.Format("No applicable properties found for filter. Identity: {0}, properties: {1}", (object) identity.Descriptor, (object) string.Join(",", propertyNameFilters))));
          requestContext.TraceLeave(80129, "IdentityService", "FrameworkIdentityCache", nameof (FilterUnchangedIdentityProperties));
        }
        else
        {
          Dictionary<string, object> values = this.GetValues((IEnumerable<string>) dictionary.Keys);
          if (values == null || values.Count == 0)
          {
            requestContext.TraceConditionally(80122, TraceLevel.Info, "IdentityService", "FrameworkIdentityCache", (Func<string>) (() => string.Format("No matching extended properties found in cache to filter on identity: {0} properties: {1}", (object) identity.Descriptor, (object) string.Join(",", propertyNameFilters))));
          }
          else
          {
            List<string> filteredProperties = new List<string>(dictionary.Count);
            foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) dictionary)
            {
              if (values.ContainsKey(keyValuePair.Key) && values[keyValuePair.Key].Equals(identity.Properties[keyValuePair.Value]))
              {
                filteredProperties.Add(keyValuePair.Value);
                identity.Properties.Remove(keyValuePair.Value);
              }
            }
            requestContext.TraceConditionally(80122, TraceLevel.Info, "IdentityService", "FrameworkIdentityCache", (Func<string>) (() => string.Format("Filtered properties for identity {0} : {1}", (object) identity.Id, filteredProperties.Count == 0 ? (object) "None" : (object) string.Join(",", (IEnumerable<string>) filteredProperties))));
          }
          requestContext.TraceLeave(80129, "IdentityService", "FrameworkIdentityCache", nameof (FilterUnchangedIdentityProperties));
        }
      }
    }

    public bool EnrichIdentityProperties(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IEnumerable<string> propertyNameFilters,
      Microsoft.VisualStudio.Services.Identity.Identity identityToEnrich)
    {
      if (!requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.IdentityStore.CacheExtendedProperties") || propertyNameFilters == null || !propertyNameFilters.Any<string>())
        return false;
      requestContext.TraceEnter(80110, "IdentityService", "FrameworkIdentityCache", nameof (EnrichIdentityProperties));
      if (identityToEnrich == null || propertyNameFilters == null || !propertyNameFilters.Any<string>())
      {
        requestContext.TraceConditionally(80116, TraceLevel.Warning, "IdentityService", "FrameworkIdentityCache", (Func<string>) (() => string.Format("Invalid EnrichIdentityProperties invocation with Identity : {0}, PropertyNameFilters : {1} ", (object) (identityToEnrich?.Id.ToString() ?? "NULL"), propertyNameFilters == null || !propertyNameFilters.Any<string>() ? (object) "NULL" : (object) string.Join(",", propertyNameFilters))));
        requestContext.TraceLeave(80119, "IdentityService", "FrameworkIdentityCache", nameof (EnrichIdentityProperties));
        return true;
      }
      bool flag = true;
      IDictionary<string, string> source = this.BuildIdentityPropertyCacheKeys(identityToEnrich.Id, propertyNameFilters);
      if (!source.Any<KeyValuePair<string, string>>())
      {
        requestContext.TraceConditionally(80115, TraceLevel.Info, "IdentityService", "FrameworkIdentityCache", (Func<string>) (() => string.Format("No applicable identity property keys matched to enrich for identity : {0}, PropertyNameFilters : {1} ", (object) identityToEnrich.Id.ToString(), propertyNameFilters == null || !propertyNameFilters.Any<string>() ? (object) "NULL" : (object) string.Join(",", propertyNameFilters))));
        requestContext.TraceLeave(80119, "IdentityService", "FrameworkIdentityCache", nameof (EnrichIdentityProperties));
        return false;
      }
      Dictionary<string, object> values = this.GetValues((IEnumerable<string>) source.Keys);
      List<string> cacheMissList = new List<string>();
      List<string> cacheHitList = new List<string>();
      requestContext.Trace(80115, TraceLevel.Verbose, "IdentityService", "FrameworkIdentityCache", "Identity properties to enrich: Requested {0}, Filtered {1}, Cached: {2}", (object) propertyNameFilters.Count<string>(), (object) source.Count, (object) values.Count);
      if (propertyNameFilters.Count<string>() != source.Count || source.Count != values.Count)
      {
        flag = false;
        foreach (string propertyNameFilter in propertyNameFilters)
        {
          if (!source.ContainsKey(propertyNameFilter) || !values.ContainsKey(source[propertyNameFilter]))
            cacheMissList.Add(propertyNameFilter);
        }
      }
      foreach (KeyValuePair<string, object> keyValuePair in values)
      {
        if (keyValuePair.Value != IdentityPropertiesCache.EmptyCacheItem)
        {
          string key = source[keyValuePair.Key];
          cacheHitList.Add(key);
          identityToEnrich.Properties.Remove(key);
          identityToEnrich.Properties.Add(key, keyValuePair.Value);
        }
      }
      if (cacheHitList.Count == source.Count)
        this.IncrementExtPropCacheHitPerfCounters();
      if (cacheMissList.Count > 0)
        this.IncrementExtPropCacheMissPerfCounters();
      if (cacheMissList.Count > 0)
        requestContext.TraceConditionally(80111, TraceLevel.Warning, "IdentityService", "FrameworkIdentityCache", (Func<string>) (() => string.Format("Cache miss detected for identity {0}. Missed {1} properties: {2} ", (object) identityToEnrich.Id.ToString(), (object) cacheMissList.Count, (object) string.Join(",", (IEnumerable<string>) cacheMissList))));
      if (cacheHitList.Count > 0)
        requestContext.TraceConditionally(80112, TraceLevel.Warning, "IdentityService", "FrameworkIdentityCache", (Func<string>) (() => string.Format("Cache hit detected for identity {0}. Hit {1} properties: {2} ", (object) identityToEnrich.Id.ToString(), (object) cacheHitList.Count, (object) string.Join(",", (IEnumerable<string>) cacheHitList))));
      requestContext.TraceLeave(80119, "IdentityService", "FrameworkIdentityCache", nameof (EnrichIdentityProperties));
      return flag;
    }

    public bool UpdateIdentityProperties(
      IVssRequestContext requestContext,
      IDictionary<Guid, Dictionary<string, object>> changedIdentityProperties)
    {
      if (!requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.IdentityStore.CacheExtendedProperties"))
        return false;
      requestContext.TraceEnter(80210, "IdentityService", "FrameworkIdentityCache", nameof (UpdateIdentityProperties));
      foreach (KeyValuePair<Guid, Dictionary<string, object>> identityProperty in (IEnumerable<KeyValuePair<Guid, Dictionary<string, object>>>) changedIdentityProperties)
      {
        KeyValuePair<Guid, Dictionary<string, object>> identityProperties = identityProperty;
        IDictionary<string, string> dictionary = this.BuildIdentityPropertyCacheKeys(identityProperties.Key, (IEnumerable<string>) identityProperties.Value.Keys);
        List<string> stringList = new List<string>();
        List<string> removedProperties = new List<string>();
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) dictionary)
        {
          object obj = identityProperties.Value[keyValuePair.Value];
          if (obj == null)
          {
            this.Remove(keyValuePair.Key);
            removedProperties.Add(keyValuePair.Value);
          }
          else
          {
            this.Set(keyValuePair.Key, obj);
            stringList.Add(keyValuePair.Value);
          }
        }
        if (removedProperties.Count > 0)
          requestContext.TraceConditionally(80212, TraceLevel.Info, "IdentityService", "FrameworkIdentityCache", (Func<string>) (() => string.Format("Removed properties for identity {0} : {1}", (object) identityProperties.Key, (object) string.Join(",", (IEnumerable<string>) removedProperties))));
        if (stringList.Count > 0)
          requestContext.TraceConditionally(80212, TraceLevel.Info, "IdentityService", "FrameworkIdentityCache", (Func<string>) (() => string.Format("Removed properties for identity {0} : {1}", (object) identityProperties.Key, (object) string.Join(",", (IEnumerable<string>) removedProperties))));
      }
      requestContext.TraceLeave(80211, "IdentityService", "FrameworkIdentityCache", nameof (UpdateIdentityProperties));
      return true;
    }

    public void UpdateIdentityProperties(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (!requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.IdentityStore.CacheExtendedProperties") || propertyNameFilters == null)
        return;
      requestContext.TraceEnter(80215, "IdentityService", "FrameworkIdentityCache", nameof (UpdateIdentityProperties));
      IdentityPropertiesCache.ExtendedIdentity extendedIdentity = new IdentityPropertiesCache.ExtendedIdentity(identity.Clone());
      IDictionary<string, string> source = this.BuildIdentityPropertyCacheKeys(identity.Id, propertyNameFilters);
      if (!source.Any<KeyValuePair<string, string>>())
      {
        requestContext.TraceConditionally(80217, TraceLevel.Info, "IdentityService", "FrameworkIdentityCache", (Func<string>) (() => string.Format("No applicable properties found to update for identity {0}. Property filters {1}", (object) identity.Id, (object) string.Join(",", propertyNameFilters))));
        requestContext.TraceLeave(80216, "IdentityService", "FrameworkIdentityCache", nameof (UpdateIdentityProperties));
      }
      List<string> notFoundProperties = new List<string>();
      if (propertyNameFilters != null && propertyNameFilters.Any<string>())
      {
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) source)
        {
          if (identity.Properties.ContainsKey(keyValuePair.Value))
          {
            extendedIdentity.Properties[keyValuePair.Key] = identity.Properties[keyValuePair.Value];
            extendedIdentity.Identity.Properties.Remove(keyValuePair.Value);
          }
          else
            notFoundProperties.Add(keyValuePair.Key);
        }
      }
      if (extendedIdentity.Properties.Count > 0)
      {
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) extendedIdentity.Properties)
          this.Set(property.Key, property.Value);
        requestContext.TraceConditionally(80217, TraceLevel.Info, "IdentityService", "FrameworkIdentityCache", (Func<string>) (() => string.Format("Updated extended properties for identity {0} - {1}", (object) identity.Id, (object) string.Join(",", (IEnumerable<string>) extendedIdentity.Properties.Keys))));
      }
      if (notFoundProperties.Count > 0)
      {
        foreach (string key in notFoundProperties)
          this.Set(key, IdentityPropertiesCache.EmptyCacheItem);
        requestContext.TraceConditionally(80217, TraceLevel.Info, "IdentityService", "FrameworkIdentityCache", (Func<string>) (() => string.Format("Updated empty extended properties for identity {0} - {1}", (object) identity.Id, (object) string.Join(",", (IEnumerable<string>) notFoundProperties))));
      }
      requestContext.TraceLeave(80216, "IdentityService", "FrameworkIdentityCache", nameof (UpdateIdentityProperties));
    }

    public IEnumerable<string> GetPrefetchedProperties() => (IEnumerable<string>) IdentityPropertiesCache.DefaultExtendedPropertiesCacheWhitelist;

    private string BuildIdentityPropertyCacheKey(Guid identityId, string propertyName) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) identityId.ToString("N"), (object) propertyName);

    private IDictionary<string, string> BuildIdentityPropertyCacheKeys(
      Guid identityId,
      IEnumerable<string> propertyNames)
    {
      return (IDictionary<string, string>) propertyNames.Where<string>((Func<string, bool>) (candidateProperty => ((IEnumerable<string>) IdentityPropertiesCache.DefaultExtendedPropertiesCacheWhitelist).Any<string>((Func<string, bool>) (x => x.Equals(candidateProperty, StringComparison.OrdinalIgnoreCase))))).ToDictionary<string, string, string>((Func<string, string>) (k => this.BuildIdentityPropertyCacheKey(identityId, k)), (Func<string, string>) (v => v));
    }

    internal static object EmptyCacheItem { get; } = new object();

    internal virtual void Set(string key, object value) => this._cache.Add(key, value, true);

    internal virtual void Remove(string key) => this._cache.Remove(key);

    internal virtual Dictionary<string, object> GetValues(IEnumerable<string> keys)
    {
      Dictionary<string, object> values = new Dictionary<string, object>();
      foreach (string key in keys)
      {
        object obj;
        if (this._cache.TryGetValue(key, out obj))
          values.Add(key, obj);
      }
      return values;
    }

    private void IncrementExtPropCacheHitPerfCounters() => VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_extprop_cache_hits").Increment();

    private void IncrementExtPropCacheMissPerfCounters()
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_extprop_cache_misses").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_extprop_cache_misses_persec").Increment();
    }

    private class ExtendedIdentity
    {
      public ExtendedIdentity(Microsoft.VisualStudio.Services.Identity.Identity identity)
      {
        this.Identity = identity;
        this.Properties = new PropertiesCollection();
      }

      public Microsoft.VisualStudio.Services.Identity.Identity Identity { get; }

      public PropertiesCollection Properties { get; }
    }

    private static class TracePoints
    {
      public const int CacheablePropertiesList = 80114;
      public const int FilterUnchangedIdentityPropertiesStart = 80120;
      public const int FilterUnchangedIdentityPropertiesInfo = 80121;
      public const int FilterUnchangedIdentityPropertiesResult = 80122;
      public const int FilterUnchangedIdentityPropertiesEnd = 80129;
      public const int EnrichIdentityPropertiesStart = 80110;
      public const int EnrichIdentityPropertiesMiss = 80111;
      public const int EnrichIdentityPropertiesHit = 80112;
      public const int EnrichIdentityPropertiesInfo = 80115;
      public const int EnrichIdentityPropertiesWarn = 80116;
      public const int EnrichIdentityPropertiesEnd = 80119;
      public const int CacheIdentityEvictedStart = 80200;
      public const int CacheIdentityEvictedInfo = 80202;
      public const int CacheIdentityEvictedEnd = 80209;
      public const int UpdateIdentityPropertiesMultipleStart = 80210;
      public const int UpdateIdentityPropertiesMultipleEnd = 80211;
      public const int UpdateIdentityPropertiesMultipleInfo = 80212;
      public const int UpdateIdentityPropertiesStart = 80215;
      public const int UpdateIdentityPropertiesEnd = 80216;
      public const int UpdateIdentityPropertiesInfo = 80217;
      public const int SweepIdentityPropertiesStart = 80218;
      public const int SweepIdentityPropertiesEnd = 80219;
      public const int SweepIdentityPropertiesInfo = 80220;
      public const int SweepIdentityPropertiesError = 80221;
    }
  }
}
