// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ImsCacheSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class ImsCacheSettings : IVssFrameworkService
  {
    private static readonly Dictionary<Type, Guid> DefaultRemoteNamespaces = new Dictionary<Type, Guid>()
    {
      {
        typeof (ImsCacheChildren),
        new Guid("E84F2AAA-E54F-4E45-A475-893154431B09")
      },
      {
        typeof (ImsCacheDescendants),
        new Guid("C2C17EDF-0333-4769-BE6C-5C20291430D6")
      },
      {
        typeof (ImsCacheIdentity),
        new Guid("C1753211-5CD5-41FA-8F98-2409CE63F8CB")
      },
      {
        typeof (ImsCacheIdentitiesInScope),
        new Guid("7706B97D-B4A3-40C9-B839-ECD775E811AC")
      },
      {
        typeof (ImsCacheIdentityId),
        new Guid("EA443551-5B88-4222-A5C6-45E23EE4192F")
      },
      {
        typeof (ImsCacheScopeMembership),
        new Guid("E6AD1096-E907-4BD7-9851-865F08C8F2DB")
      },
      {
        typeof (ImsCacheIdentitiesByDisplayName),
        new Guid("39034CE1-C858-4CA6-8978-A0F3CC1CDB2A")
      },
      {
        typeof (ImsCacheIdentitiesByAccountName),
        new Guid("CC9DEC6B-4E0D-44F3-A717-5EDBDF31EAD7")
      },
      {
        typeof (ImsCacheDisplayNameSearchIndex),
        new Guid("F899064F-A50B-49EC-A129-94B4C58C5462")
      },
      {
        typeof (ImsCacheEmailSearchIndex),
        new Guid("30D830B8-0118-49B8-8B1E-2D1AA1CDC6AB")
      },
      {
        typeof (ImsCacheAccountNameSearchIndex),
        new Guid("94CF0D97-C915-4F8B-9142-1ABE65CB5247")
      },
      {
        typeof (ImsCacheMruIdentityIds),
        new Guid("3A7A9A9F-75E8-444E-891D-B6AD02D882F9")
      }
    };
    private static readonly Dictionary<Type, bool> DefaultRemoteCacheUseCompressionSettings = new Dictionary<Type, bool>()
    {
      {
        typeof (ImsCacheChildren),
        true
      },
      {
        typeof (ImsCacheDescendants),
        true
      },
      {
        typeof (ImsCacheIdentity),
        true
      },
      {
        typeof (ImsCacheIdentityId),
        true
      },
      {
        typeof (ImsCacheIdentitiesInScope),
        true
      },
      {
        typeof (ImsCacheDisplayNameSearchIndex),
        true
      },
      {
        typeof (ImsCacheEmailSearchIndex),
        true
      },
      {
        typeof (ImsCacheAccountNameSearchIndex),
        true
      }
    };
    private static readonly Dictionary<Type, TimeSpan> DefaultLocalDataCacheTimeToLive = new Dictionary<Type, TimeSpan>()
    {
      {
        typeof (ImsCacheChildren),
        TimeSpan.FromMinutes(30.0)
      }
    };

    public void ServiceStart(IVssRequestContext requestContext)
    {
      IVssRegistryService registryService = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? requestContext.GetService<IVssRegistryService>() : throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      registryService.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnCacheServiceRegistryChanged), "/Configuration/Identity/Cache/ImsCacheService/...");
      registryService.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnCacheServiceRegistryChanged), "/Configuration/Identity/Cache/Settings/...");
      registryService.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRemoteCacheRegistryChanged), "/Configuration/Identity/Cache/Settings/...");
      registryService.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnLocalDataCacheRegistryChanged), "/Configuration/Identity/Cache/Settings/...");
      registryService.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnLocalSearchCacheRegistryChanged), "/Configuration/Identity/Cache/Settings/...");
      this.ReloadLocalSearchCacheSettings(requestContext);
      this.ReloadLocalDataCacheSettings(requestContext, (IEnumerable<Type>) null);
      this.ReloadRemoteCacheSettings(requestContext, (HashSet<Type>) null);
      this.ReloadCacheServiceSettings(requestContext);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      service.UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnLocalDataCacheRegistryChanged));
      service.UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRemoteCacheRegistryChanged));
      service.UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnCacheServiceRegistryChanged));
      service.UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnLocalSearchCacheRegistryChanged));
    }

    private void OnCacheServiceRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<RegistryEntryCollection>(changedEntries, nameof (changedEntries));
      if (!changedEntries.Any<RegistryEntry>())
        return;
      this.ReloadCacheServiceSettings(context);
    }

    private void OnRemoteCacheRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<RegistryEntryCollection>(changedEntries, nameof (changedEntries));
      if (!changedEntries.Any<RegistryEntry>())
        return;
      this.ReloadRemoteCacheSettings(context, new HashSet<Type>((IEnumerable<Type>) this.ImsRemoteCacheSettings.Namespaces.Keys));
    }

    private void OnLocalDataCacheRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<RegistryEntryCollection>(changedEntries, nameof (changedEntries));
      if (!changedEntries.Any<RegistryEntry>())
        return;
      this.ReloadLocalDataCacheSettings(context, (IEnumerable<Type>) this.ImsLocalDataCacheSettings.CacheEntryTimeToLive.Keys.ToList<Type>());
    }

    private void OnLocalSearchCacheRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<RegistryEntryCollection>(changedEntries, nameof (changedEntries));
      if (!changedEntries.Any<RegistryEntry>())
        return;
      this.ReloadLocalSearchCacheSettings(context);
    }

    private void ReloadCacheServiceSettings(IVssRequestContext context)
    {
      IVssRegistryService service = context.GetService<IVssRegistryService>();
      this.ImsCacheServiceSettings = new ImsCacheSettings.ImsCacheServiceRegistrySettings()
      {
        ChildrenTimeToLive = service.GetValue<TimeSpan>(context, (RegistryQuery) "/Configuration/Identity/Cache/ImsCacheService/ChildrenTimeToLive", ImsCacheConstants.Registry.CacheService.Defaults.DefaultChildrenTimeToLive),
        DescendantsTimeToLive = service.GetValue<TimeSpan>(context, (RegistryQuery) "/Configuration/Identity/Cache/ImsCacheService/DescendantsTimeToLive", ImsCacheConstants.Registry.CacheService.Defaults.DefaultDescendantsTimeToLive),
        IdentitiesInScopeTimeToLive = service.GetValue<TimeSpan>(context, (RegistryQuery) "/Configuration/Identity/Cache/ImsCacheService/IdentitiesInScopeTimeToLive", ImsCacheConstants.Registry.CacheService.Defaults.DefaultIdentitiesInScopeTimeToLive),
        IdentityIdTimeToLive = service.GetValue<TimeSpan>(context, (RegistryQuery) "/Configuration/Identity/Cache/ImsCacheService/IdentityIdTimeToLive", ImsCacheConstants.Registry.CacheService.Defaults.DefaultIdentityIdTimeToLive),
        IdentityTimeToLive = service.GetValue<TimeSpan>(context, (RegistryQuery) "/Configuration/Identity/Cache/ImsCacheService/IdentityTimeToLive", ImsCacheConstants.Registry.CacheService.Defaults.DefaultIdentityTimeToLive),
        ScopeMembershipTimeToLive = service.GetValue<TimeSpan>(context, (RegistryQuery) "/Configuration/Identity/Cache/ImsCacheService/ScopeMembershipTimeToLive", ImsCacheConstants.Registry.CacheService.Defaults.DefaultScopeMembershipTimeToLive),
        DisplayNameQueryResultsTimeToLive = service.GetValue<TimeSpan>(context, (RegistryQuery) "/Configuration/Identity/Cache/ImsCacheService/DisplayNameQueryResultsTimeToLive", ImsCacheConstants.Registry.CacheService.Defaults.DefaultDisplayNameQueryResultsTimeToLive),
        AccountNameQueryResultsTimeToLive = service.GetValue<TimeSpan>(context, (RegistryQuery) "/Configuration/Identity/Cache/ImsCacheService/AccountNameQueryResultsTimeToLive", ImsCacheConstants.Registry.CacheService.Defaults.DefaultAccountNameQueryResultsTimeToLive),
        MruIdentityIdsTimeToLive = service.GetValue<TimeSpan>(context, (RegistryQuery) "/Configuration/Identity/Cache/ImsCacheService/MruIdentityIdsTimeToLive", ImsCacheConstants.Registry.CacheService.Defaults.DefaultMruIdentityIdsTimeToLive),
        FreshEntryTimeSpan = service.GetValue<TimeSpan>(context, (RegistryQuery) "/Configuration/Identity/Cache/ImsCacheService/FreshEntryTimeSpan", ImsCacheConstants.Registry.CacheService.Defaults.FreshEntryTimeSpan),
        MaxDescendantLevelsToIterate = service.GetValue<int>(context, (RegistryQuery) "/Configuration/Identity/Cache/ImsCacheService/MaxDescendantLevelsToIterate", 100),
        IdentitesInScopeCountThresholdBeyondWhichCache = service.GetValue<int>(context, (RegistryQuery) "/Configuration/Identity/Cache/ImsCacheService/IdentitesInScopeCountThresholdBeyondWhichCache", 10000),
        WarningDescendantsCountThreshold = service.GetValue<int>(context, (RegistryQuery) "/Configuration/Identity/Cache/ImsCacheService/WarningDescendantsCountThreshold", 10000),
        WarningDescendantsLevelsThreshold = service.GetValue<int>(context, (RegistryQuery) "/Configuration/Identity/Cache/ImsCacheService/WarningDescendantsLevelsThreshold", 7),
        IndexSearchCacheSettings = this.ImsLocalSearchCacheSettings.SearchCacheSettings
      };
    }

    internal void ReloadRemoteCacheSettings(
      IVssRequestContext context,
      HashSet<Type> registeredTypes)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment).Elevate();
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      if (registeredTypes == null)
        registeredTypes = new HashSet<Type>();
      this.ImsRemoteCacheSettings = this.LoadRemoteCacheSettings(vssRequestContext, service, registeredTypes, ImsCacheSettings.DefaultRemoteNamespaces);
    }

    private ImsCacheSettings.ImsRemoteCacheRegistrySettings LoadRemoteCacheSettings(
      IVssRequestContext elevatedDeploymentContext,
      IVssRegistryService registryService,
      HashSet<Type> registeredTypes,
      Dictionary<Type, Guid> defaultRemoteNamespaces)
    {
      ImsCacheSettings.ImsRemoteCacheRegistrySettings registrySettings = new ImsCacheSettings.ImsRemoteCacheRegistrySettings()
      {
        DefaultCacheEntryTimeToLive = registryService.GetValue<TimeSpan>(elevatedDeploymentContext, (RegistryQuery) ImsCacheConstants.Registry.LocalOrRemoteCache.Keys.DefaultRemoteCacheEntryTimeToLiveFormat, ImsCacheConstants.Registry.LocalOrRemoteCache.Defaults.DefaultRemoteCacheEntryTimeToLive)
      };
      foreach (Type registeredType in registeredTypes)
      {
        string str = string.Format("/Configuration/Identity/Cache/Settings/CacheItems/{0}/RemoteNamespace", (object) registeredType.Name);
        string query1 = string.Format("/Configuration/Identity/Cache/Settings/CacheItems/{0}/RemoteCacheEntryTTL", (object) registeredType.Name);
        string query2 = string.Format("/Configuration/Identity/Cache/Settings/CacheItems/{0}/RemoteCacheUseCompression", (object) registeredType.Name);
        Guid defaultValue;
        defaultRemoteNamespaces.TryGetValue(registeredType, out defaultValue);
        registrySettings.Namespaces[registeredType] = registryService.GetValue<Guid>(elevatedDeploymentContext, (RegistryQuery) str, defaultValue);
        if (registrySettings.Namespaces[registeredType] == Guid.Empty)
        {
          defaultValue = Guid.NewGuid();
          registryService.SetValue<Guid>(elevatedDeploymentContext, str, defaultValue);
          registrySettings.Namespaces[registeredType] = defaultValue;
        }
        registrySettings.CacheEntryTimeToLive[registeredType] = registryService.GetValue<TimeSpan>(elevatedDeploymentContext, (RegistryQuery) query1, registrySettings.DefaultCacheEntryTimeToLive);
        bool orDefault = ImsCacheSettings.DefaultRemoteCacheUseCompressionSettings.TryGetOrDefault<Type, bool>(registeredType);
        registrySettings.CompressCacheEntry[registeredType] = registryService.GetValue<bool>(elevatedDeploymentContext, (RegistryQuery) query2, orDefault);
      }
      return registrySettings;
    }

    internal void ReloadLocalDataCacheSettings(
      IVssRequestContext context,
      IEnumerable<Type> registeredTypes)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      ImsCacheSettings.ImsLocalDataCacheRegistrySettings registrySettings = new ImsCacheSettings.ImsLocalDataCacheRegistrySettings()
      {
        DefaultCacheEntryTimeToLive = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) ImsCacheConstants.Registry.LocalOrRemoteCache.Keys.DefaultLocalCacheEntryTimeToLiveFormat, ImsCacheConstants.Registry.LocalOrRemoteCache.Defaults.DefaultLocalCacheEntryTimeToLive),
        CacheCleanUpInterval = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/Settings/CleanUpInterval", ImsCacheConstants.Registry.LocalOrRemoteCache.Defaults.DefaultLocalCacheCleanUpInterval)
      };
      if (registeredTypes == null)
        registeredTypes = (IEnumerable<Type>) new List<Type>();
      foreach (Type registeredType in registeredTypes)
      {
        string query = string.Format(ImsCacheConstants.Registry.LocalOrRemoteCache.Keys.DefaultLocalCacheEntryTimeToLiveFormat, (object) registeredType.Name);
        TimeSpan defaultValue = ImsCacheSettings.DefaultLocalDataCacheTimeToLive.ContainsKey(registeredType) ? ImsCacheSettings.DefaultLocalDataCacheTimeToLive[registeredType] : this.ImsLocalDataCacheSettings.DefaultCacheEntryTimeToLive;
        registrySettings.CacheEntryTimeToLive[registeredType] = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) query, defaultValue);
      }
      this.ImsLocalDataCacheSettings = registrySettings;
    }

    internal void ReloadLocalSearchCacheSettings(IVssRequestContext context)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      this.ImsLocalSearchCacheSettings = new ImsCacheSettings.ImsLocalSearchCacheRegistrySettings()
      {
        CacheCleanUpInterval = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/Settings/CleanUpInterval", ImsCacheConstants.Registry.LocalOrRemoteCache.Defaults.DefaultLocalCacheCleanUpInterval),
        SearchCacheSettings = {
          SearchCacheTinyTimeToLive = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/Settings/SearchCacheTinyTimeToLive", ImsCacheConstants.Registry.LocalOrRemoteCache.Defaults.DefaultSearchCacheTinyTimeToLive),
          SearchCacheSmallTimeToLive = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/Settings/SearchCacheSmallTimeToLive", ImsCacheConstants.Registry.LocalOrRemoteCache.Defaults.DefaultSearchCacheSmallTimeToLive),
          SearchCacheMediumTimeToLive = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/Settings/SearchCacheMediumTimeToLive", ImsCacheConstants.Registry.LocalOrRemoteCache.Defaults.DefaultSearchCacheMediumTimeToLive),
          SearchCacheLargeTimeToLive = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/Settings/SearchCacheLargeTimeToLive", ImsCacheConstants.Registry.LocalOrRemoteCache.Defaults.DefaultSearchCacheLargeTimeToLive),
          SearchCacheMegaTimeToLive = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/Settings/SearchCacheMegaTimeToLive", ImsCacheConstants.Registry.LocalOrRemoteCache.Defaults.DefaultSearchCacheMegaTimeToLive),
          SearchCacheSoonToExpireAlertDuration = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/Settings/SearchCacheSoonToExpireAlertDuration", ImsCacheConstants.Registry.LocalOrRemoteCache.Defaults.DefaultSearchCacheSoonToExpireAlertDuration),
          SearchCacheTinySizeThreshold = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/Settings/SearchCacheTinySizeThreshold", 100),
          SearchCacheSmallSizeThreshold = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/Settings/SearchCacheSmallSizeThreshold", 200),
          SearchCacheMediumSizeThreshold = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/Settings/SearchCacheMediumSizeThreshold", 2000),
          SearchCacheLargeSizeThreshold = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/Settings/SearchCacheLargeSizeThreshold", 5000)
        }
      };
    }

    internal ImsCacheSettings.ImsCacheServiceRegistrySettings ImsCacheServiceSettings { get; private set; }

    internal ImsCacheSettings.ImsRemoteCacheRegistrySettings ImsRemoteCacheSettings { get; private set; }

    internal ImsCacheSettings.ImsLocalDataCacheRegistrySettings ImsLocalDataCacheSettings { get; private set; }

    internal ImsCacheSettings.ImsLocalSearchCacheRegistrySettings ImsLocalSearchCacheSettings { get; private set; }

    internal class ImsCacheServiceRegistrySettings
    {
      internal ImsCacheServiceRegistrySettings() => this.IndexSearchCacheSettings = new ImsCacheSettings.SearchCacheSettings();

      internal TimeSpan ChildrenTimeToLive { get; set; }

      internal TimeSpan DescendantsTimeToLive { get; set; }

      internal TimeSpan IdentitiesInScopeTimeToLive { get; set; }

      internal TimeSpan IdentityIdTimeToLive { get; set; }

      internal TimeSpan IdentityTimeToLive { get; set; }

      internal TimeSpan ScopeMembershipTimeToLive { get; set; }

      internal TimeSpan DisplayNameQueryResultsTimeToLive { get; set; }

      internal TimeSpan AccountNameQueryResultsTimeToLive { get; set; }

      internal TimeSpan MruIdentityIdsTimeToLive { get; set; }

      internal TimeSpan FreshEntryTimeSpan { get; set; }

      internal ImsCacheSettings.SearchCacheSettings IndexSearchCacheSettings { get; set; }

      internal int MaxDescendantLevelsToIterate { get; set; }

      internal int IdentitesInScopeCountThresholdBeyondWhichCache { get; set; }

      internal int WarningDescendantsCountThreshold { get; set; }

      internal int WarningDescendantsLevelsThreshold { get; set; }
    }

    internal class ImsRemoteCacheRegistrySettings
    {
      internal ImsRemoteCacheRegistrySettings()
      {
        this.Namespaces = new Dictionary<Type, Guid>();
        this.CacheEntryTimeToLive = new Dictionary<Type, TimeSpan>();
        this.CompressCacheEntry = new Dictionary<Type, bool>();
      }

      internal TimeSpan DefaultCacheEntryTimeToLive { get; set; }

      internal Dictionary<Type, Guid> Namespaces { get; }

      internal Dictionary<Type, TimeSpan> CacheEntryTimeToLive { get; }

      internal Dictionary<Type, bool> CompressCacheEntry { get; }
    }

    internal class ImsLocalDataCacheRegistrySettings
    {
      internal ImsLocalDataCacheRegistrySettings() => this.CacheEntryTimeToLive = new Dictionary<Type, TimeSpan>();

      internal TimeSpan DefaultCacheEntryTimeToLive { get; set; }

      internal TimeSpan CacheCleanUpInterval { get; set; }

      internal Dictionary<Type, TimeSpan> CacheEntryTimeToLive { get; }
    }

    internal class ImsLocalSearchCacheRegistrySettings
    {
      internal ImsLocalSearchCacheRegistrySettings() => this.SearchCacheSettings = new ImsCacheSettings.SearchCacheSettings();

      internal TimeSpan CacheCleanUpInterval { get; set; }

      internal ImsCacheSettings.SearchCacheSettings SearchCacheSettings { get; }
    }

    internal class SearchCacheSettings
    {
      internal TimeSpan SearchCacheTinyTimeToLive { get; set; }

      internal TimeSpan SearchCacheSmallTimeToLive { get; set; }

      internal TimeSpan SearchCacheMediumTimeToLive { get; set; }

      internal TimeSpan SearchCacheLargeTimeToLive { get; set; }

      internal TimeSpan SearchCacheMegaTimeToLive { get; set; }

      internal TimeSpan SearchCacheSoonToExpireAlertDuration { get; set; }

      internal int SearchCacheTinySizeThreshold { get; set; }

      internal int SearchCacheSmallSizeThreshold { get; set; }

      internal int SearchCacheMediumSizeThreshold { get; set; }

      internal int SearchCacheLargeSizeThreshold { get; set; }

      internal TimeSpan GetSearchCacheTimeToLive(int size)
      {
        if (size <= this.SearchCacheTinySizeThreshold)
          return this.SearchCacheTinyTimeToLive;
        if (size <= this.SearchCacheSmallSizeThreshold)
          return this.SearchCacheSmallTimeToLive;
        if (size <= this.SearchCacheMediumSizeThreshold)
          return this.SearchCacheMediumTimeToLive;
        return size <= this.SearchCacheLargeSizeThreshold ? this.SearchCacheLargeTimeToLive : this.SearchCacheMegaTimeToLive;
      }
    }
  }
}
