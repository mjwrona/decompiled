// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.LocationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal class LocationService : 
    IInternalLocationService,
    ILocationService,
    IVssFrameworkService,
    ILocationDataProvider
  {
    private ILocationDataProvider m_localDataProvider;
    private ILocationDataCache<string> m_locationCache;
    private LocationService.ProviderCache m_providerLookup;
    private INotificationRegistration m_dataChangedRegistration;
    private INotificationRegistration m_artifactKindsLocationRegistration;
    private INotificationRegistration m_parentDataChangedRegistration;
    internal const string Area = "LocationService";
    internal const string Layer = "IVssFrameworkService";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_locationCache = (ILocationDataCache<string>) new LocationService.LocationDataCache(systemRequestContext);
      this.m_localDataProvider = this.CreateLocalDataProvider(systemRequestContext);
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      this.m_dataChangedRegistration = service.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.LocationDataChanged, new SqlNotificationCallback(this.OnLocationDataChanged), true, false);
      this.m_artifactKindsLocationRegistration = service.CreateRegistration(systemRequestContext, "Default", ArtifactKinds.Location, new SqlNotificationCallback(this.OnLocationPropertiesChanged), true, false);
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Parent);
        this.m_parentDataChangedRegistration = vssRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(vssRequestContext, "Default", SqlNotificationEventClasses.LocationDataChanged, new SqlNotificationCallback(this.OnLocationDataChanged), false, false);
      }
      systemRequestContext.ServiceHost.ServiceHostInternal().PropertiesChanged += new EventHandler<HostPropertiesChangedEventArgs>(this.OnHostPropertiesChanged);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.ServiceHost.ServiceHostInternal().PropertiesChanged -= new EventHandler<HostPropertiesChangedEventArgs>(this.OnHostPropertiesChanged);
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        this.m_parentDataChangedRegistration.Unregister(systemRequestContext.To(TeamFoundationHostType.Parent));
      this.m_dataChangedRegistration.Unregister(systemRequestContext);
      this.m_artifactKindsLocationRegistration.Unregister(systemRequestContext);
      if (this.m_localDataProvider == null)
        return;
      if (this.m_localDataProvider is IDisposable localDataProvider)
        localDataProvider.Dispose();
      this.m_localDataProvider = (ILocationDataProvider) null;
    }

    public ILocationDataProvider GetLocationData(
      IVssRequestContext requestContext,
      Guid serviceAreaIdentifier,
      bool throwOnMissingArea = true)
    {
      if (serviceAreaIdentifier == Guid.Empty || serviceAreaIdentifier == Microsoft.VisualStudio.Services.Location.LocationServiceConstants.SelfReferenceIdentifier || serviceAreaIdentifier == Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.SelfReferenceLocationServiceIdentifier || serviceAreaIdentifier == this.LocalDataProvider.HostId || serviceAreaIdentifier == this.LocalDataProvider.InstanceType || requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return this.LocalDataProvider;
      ILocationDataProvider locationDataProvider = this.ResolveLocationData(requestContext, serviceAreaIdentifier);
      return !(locationDataProvider == null & throwOnMissingArea) ? locationDataProvider : throw new ServiceOwnerNotFoundException(serviceAreaIdentifier, requestContext.ServiceHost.InstanceId);
    }

    private ILocationDataProvider ResolveLocationData(
      IVssRequestContext requestContext,
      Guid serviceAreaIdentifier)
    {
      ILocationDataProvider provider1 = (ILocationDataProvider) null;
      LocationService.ProviderCache providerCache1 = this.m_providerLookup;
      if (providerCache1 == null)
      {
        providerCache1 = new LocationService.ProviderCache();
        string locationPointerUrl = LocationServiceHelper.GetLocationPointerUrl(requestContext, this.LocalDataProvider, Microsoft.VisualStudio.Services.Location.LocationServiceConstants.SelfReferenceIdentifier);
        if (locationPointerUrl != null)
          providerCache1.GetOrAdd(locationPointerUrl, this.LocalDataProvider);
        LocationService.ProviderCache providerCache2 = Interlocked.CompareExchange<LocationService.ProviderCache>(ref this.m_providerLookup, providerCache1, (LocationService.ProviderCache) null);
        if (providerCache2 != null)
          providerCache1 = providerCache2;
      }
      if (!providerCache1.TryGetValue(serviceAreaIdentifier, out provider1))
      {
        string locationPointerUrl = LocationServiceHelper.GetLocationPointerUrl(requestContext, this.LocalDataProvider, serviceAreaIdentifier);
        requestContext.Trace(1521913330, TraceLevel.Verbose, nameof (LocationService), "IVssFrameworkService", "Local provider resolved {0} to {1}", (object) serviceAreaIdentifier, (object) locationPointerUrl);
        if (locationPointerUrl == null && serviceAreaIdentifier != Microsoft.VisualStudio.Services.Location.LocationServiceConstants.ApplicationIdentifier && serviceAreaIdentifier != Microsoft.VisualStudio.Services.Location.LocationServiceConstants.RootIdentifier)
        {
          ILocationDataProvider provider2 = this.ResolveLocationData(requestContext, Microsoft.VisualStudio.Services.Location.LocationServiceConstants.RootIdentifier);
          if (provider2 != null && provider2 != this.LocalDataProvider)
          {
            locationPointerUrl = LocationServiceHelper.GetLocationPointerUrl(requestContext, provider2, serviceAreaIdentifier);
            requestContext.Trace(1521913331, TraceLevel.Verbose, nameof (LocationService), "IVssFrameworkService", "Root provider resolved {0} to {1}", (object) serviceAreaIdentifier, (object) locationPointerUrl);
          }
        }
        requestContext.Trace(1521913332, TraceLevel.Verbose, nameof (LocationService), "IVssFrameworkService", "Service {0} resolved to {1}", (object) serviceAreaIdentifier, (object) locationPointerUrl);
        if (locationPointerUrl != null)
        {
          if (!providerCache1.TryGetValue(locationPointerUrl, out provider1))
          {
            ILocationDataProvider remoteDataProvider = this.CreateRemoteDataProvider(requestContext, locationPointerUrl);
            provider1 = providerCache1.GetOrAdd(locationPointerUrl, remoteDataProvider);
            if (provider1 != remoteDataProvider && remoteDataProvider is IDisposable disposable)
              disposable.Dispose();
          }
          providerCache1[serviceAreaIdentifier] = provider1;
        }
      }
      return provider1;
    }

    public string GetLocationServiceUrl(
      IVssRequestContext requestContext,
      Guid serviceAreaIdentifier,
      string accessMappingMoniker)
    {
      if (accessMappingMoniker == null)
      {
        accessMappingMoniker = AccessMappingConstants.PublicAccessMappingMoniker;
        requestContext.Trace(1521913333, TraceLevel.Error, nameof (LocationService), "IVssFrameworkService", "Unexpected null accessMappingMoniker, stack: {0}", (object) EnvironmentWrapper.ToReadableStackTrace());
      }
      ILocationDataProvider locationData = this.GetLocationData(requestContext, serviceAreaIdentifier, false);
      if (locationData == null)
        return (string) null;
      AccessMapping accessMapping = !VssStringComparer.AccessMappingMoniker.Equals(accessMappingMoniker, AccessMappingConstants.PublicAccessMappingMoniker) ? (!VssStringComparer.AccessMappingMoniker.Equals(accessMappingMoniker, AccessMappingConstants.ServerAccessMappingMoniker) ? (!VssStringComparer.AccessMappingMoniker.Equals(accessMappingMoniker, AccessMappingConstants.ClientAccessMappingMoniker) ? locationData.GetAccessMapping(requestContext, accessMappingMoniker) : locationData.DetermineAccessMapping(requestContext)) : locationData.GetServerAccessMapping(requestContext)) : locationData.GetPublicAccessMapping(requestContext);
      if (accessMapping != null)
        return locationData.GetSelfReferenceUrl(requestContext, accessMapping);
      requestContext.Trace(416757559, TraceLevel.Info, nameof (LocationService), "IVssFrameworkService", "Access mapping could not be found for accessMappingMoniker {0}", (object) accessMappingMoniker);
      return (string) null;
    }

    protected internal virtual ILocationDataProvider CreateRemoteDataProvider(
      IVssRequestContext requestContext,
      string location)
    {
      return (ILocationDataProvider) new RemoteLocationDataProvider(requestContext, this.m_locationCache, location);
    }

    protected internal virtual ILocationDataProvider CreateLocalDataProvider(
      IVssRequestContext requestContext)
    {
      return (ILocationDataProvider) new LocalLocationDataProvider(requestContext, this.m_locationCache);
    }

    public void OnLocationDataChanged(
      IVssRequestContext requestContext,
      LocationDataKind kind,
      ILocationCacheManager<string> cacheManager = null)
    {
      this.OnLocationDataChanged(requestContext, kind, cacheManager, true);
    }

    private void OnLocationPropertiesChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(1521913213, nameof (LocationService), "IVssFrameworkService", nameof (OnLocationPropertiesChanged));
      try
      {
        this.OnLocationDataChanged(requestContext, LocationDataKind.Local, (ILocationCacheManager<string>) null, false);
      }
      finally
      {
        requestContext.TraceLeave(1216794147, nameof (LocationService), "IVssFrameworkService", nameof (OnLocationPropertiesChanged));
      }
    }

    private void OnLocationDataChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.OnLocationDataChanged(requestContext, LocationService.FromEventData(eventData), (ILocationCacheManager<string>) null, false);
    }

    private void OnLocationDataChanged(
      IVssRequestContext requestContext,
      LocationDataKind kind,
      ILocationCacheManager<string> cacheManager,
      bool fireNotification)
    {
      if (cacheManager != null)
        cacheManager.Reset(requestContext, this.m_locationCache, kind);
      else
        this.m_locationCache.Invalidate(requestContext, kind, fireNotification);
      if (kind.HasFlag((Enum) LocationDataKind.Remote))
        this.m_providerLookup = (LocationService.ProviderCache) null;
      if (!fireNotification)
        return;
      this.NotifyLocationDataChanged(requestContext, kind);
    }

    protected virtual void NotifyLocationDataChanged(
      IVssRequestContext requestContext,
      LocationDataKind kind)
    {
      using (LocationComponent component = requestContext.CreateComponent<LocationComponent>())
        component.LogLocationChange(LocationService.ToEventData(kind));
    }

    internal static LocationDataKind FromEventData(string eventData) => !string.IsNullOrEmpty(eventData) ? (!TeamFoundationSerializationUtility.Deserialize<bool>(eventData) ? LocationDataKind.Remote : LocationDataKind.All) : LocationDataKind.Local;

    internal static string ToEventData(LocationDataKind kind)
    {
      switch (kind)
      {
        case LocationDataKind.Local:
          return (string) null;
        case LocationDataKind.Remote:
          return TeamFoundationSerializationUtility.SerializeToString<bool>(false);
        case LocationDataKind.All:
          return TeamFoundationSerializationUtility.SerializeToString<bool>(true);
        default:
          throw new InvalidOperationException(string.Format("Unexpected location data kind {0}", (object) kind));
      }
    }

    private void OnHostPropertiesChanged(object sender, HostPropertiesChangedEventArgs e)
    {
      if (string.IsNullOrEmpty(e.ServiceHostProperties.Name))
        return;
      this.OnLocationDataChanged(e.RequestContext, LocationDataKind.All, (ILocationCacheManager<string>) null, false);
      if (!e.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || !e.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      IVssRequestContext vssRequestContext = e.RequestContext.To(TeamFoundationHostType.Parent);
      ((LocationService) vssRequestContext.GetService<IInternalLocationService>()).OnLocationDataChanged(vssRequestContext, LocationDataKind.Local, (ILocationCacheManager<string>) null, false);
    }

    private ILocationDataProvider LocalDataProvider => this.m_localDataProvider;

    protected ILocationDataCache<string> LocationCache => this.m_locationCache;

    Guid ILocationDataProvider.HostId => this.LocalDataProvider.HostId;

    Guid ILocationDataProvider.InstanceType => this.LocalDataProvider.InstanceType;

    void ILocationDataProvider.SaveServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      this.LocalDataProvider.SaveServiceDefinitions(requestContext, serviceDefinitions);
    }

    void ILocationDataProvider.RemoveServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      this.LocalDataProvider.RemoveServiceDefinitions(requestContext, serviceDefinitions);
    }

    ServiceDefinition ILocationDataProvider.FindServiceDefinition(
      IVssRequestContext requestContext,
      string serviceType,
      Guid serviceIdentifier)
    {
      return this.LocalDataProvider.FindServiceDefinition(requestContext, serviceType, serviceIdentifier);
    }

    ServiceDefinition ILocationDataProvider.FindServiceDefinition(
      IVssRequestContext requestContext,
      string serviceType,
      string toolId)
    {
      return this.LocalDataProvider.FindServiceDefinition(requestContext, serviceType, toolId);
    }

    IEnumerable<ServiceDefinition> ILocationDataProvider.FindServiceDefinitions(
      IVssRequestContext requestContext,
      string serviceType)
    {
      return this.LocalDataProvider.FindServiceDefinitions(requestContext, serviceType);
    }

    IEnumerable<ServiceDefinition> ILocationDataProvider.FindServiceDefinitionsByToolId(
      IVssRequestContext requestContext,
      string toolId)
    {
      return this.LocalDataProvider.FindServiceDefinitionsByToolId(requestContext, toolId);
    }

    string ILocationDataProvider.LocationForAccessMapping(
      IVssRequestContext requestContext,
      string serviceType,
      Guid serviceIdentifier,
      AccessMapping accessMapping)
    {
      return this.LocalDataProvider.LocationForAccessMapping(requestContext, serviceType, serviceIdentifier, accessMapping);
    }

    string ILocationDataProvider.LocationForAccessMapping(
      IVssRequestContext requestContext,
      string serviceType,
      string toolId,
      AccessMapping accessMapping)
    {
      return this.LocalDataProvider.LocationForAccessMapping(requestContext, serviceType, toolId, accessMapping);
    }

    string ILocationDataProvider.LocationForAccessMapping(
      IVssRequestContext requestContext,
      string relativePath,
      Microsoft.VisualStudio.Services.Location.RelativeToSetting relativeToSetting,
      AccessMapping accessMapping)
    {
      return this.LocalDataProvider.LocationForAccessMapping(requestContext, relativePath, relativeToSetting, accessMapping);
    }

    string ILocationDataProvider.LocationForAccessMapping(
      IVssRequestContext requestContext,
      ServiceDefinition serviceDefinition,
      AccessMapping accessMapping)
    {
      return this.LocalDataProvider.LocationForAccessMapping(requestContext, serviceDefinition, accessMapping);
    }

    AccessMapping ILocationDataProvider.ConfigureAccessMapping(
      IVssRequestContext requestContext,
      AccessMapping accessMapping,
      bool makeDefault)
    {
      return this.LocalDataProvider.ConfigureAccessMapping(requestContext, accessMapping, makeDefault);
    }

    void ILocationDataProvider.SetDefaultAccessMapping(
      IVssRequestContext requestContext,
      AccessMapping accessMapping)
    {
      this.LocalDataProvider.SetDefaultAccessMapping(requestContext, accessMapping);
    }

    AccessMapping ILocationDataProvider.GetPublicAccessMapping(IVssRequestContext requestContext) => this.LocalDataProvider.GetPublicAccessMapping(requestContext);

    AccessMapping ILocationDataProvider.GetServerAccessMapping(IVssRequestContext requestContext) => this.LocalDataProvider.GetServerAccessMapping(requestContext);

    AccessMapping ILocationDataProvider.GetDefaultAccessMapping(IVssRequestContext requestContext) => this.LocalDataProvider.GetDefaultAccessMapping(requestContext);

    AccessMapping ILocationDataProvider.DetermineAccessMapping(IVssRequestContext requestContext) => this.LocalDataProvider.DetermineAccessMapping(requestContext);

    AccessMapping ILocationDataProvider.GetAccessMapping(
      IVssRequestContext requestContext,
      string moniker)
    {
      return this.LocalDataProvider.GetAccessMapping(requestContext, moniker);
    }

    IEnumerable<AccessMapping> ILocationDataProvider.GetAccessMappings(
      IVssRequestContext requestContext)
    {
      return this.LocalDataProvider.GetAccessMappings(requestContext);
    }

    void ILocationDataProvider.RemoveAccessMapping(
      IVssRequestContext requestContext,
      AccessMapping accessMapping)
    {
      this.LocalDataProvider.RemoveAccessMapping(requestContext, accessMapping);
    }

    string ILocationDataProvider.GetSelfReferenceUrl(
      IVssRequestContext requestContext,
      AccessMapping accessMapping)
    {
      return this.LocalDataProvider.GetSelfReferenceUrl(requestContext, accessMapping);
    }

    ApiResourceLocationCollection ILocationDataProvider.GetResourceLocations(
      IVssRequestContext requestContext)
    {
      return this.LocalDataProvider.GetResourceLocations(requestContext);
    }

    Uri ILocationDataProvider.GetResourceUri(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      object routeValues)
    {
      return this.LocalDataProvider.GetResourceUri(requestContext, serviceType, identifier, routeValues);
    }

    Uri ILocationDataProvider.GetResourceUri(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      object routeValues,
      bool requireExplicitRouteParams)
    {
      return this.LocalDataProvider.GetResourceUri(requestContext, serviceType, identifier, routeValues, requireExplicitRouteParams);
    }

    Uri ILocationDataProvider.GetResourceUri(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      object routeValues,
      bool appendUnusedAsQueryParams,
      bool requireExplicitRouteParams)
    {
      return this.LocalDataProvider.GetResourceUri(requestContext, serviceType, identifier, routeValues, appendUnusedAsQueryParams, requireExplicitRouteParams);
    }

    Uri ILocationDataProvider.GetResourceUri(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      object routeValues,
      bool appendUnusedAsQueryParams,
      bool requireExplicitRouteParams,
      bool wildcardAsQueryParams)
    {
      return this.LocalDataProvider.GetResourceUri(requestContext, serviceType, identifier, routeValues, appendUnusedAsQueryParams, requireExplicitRouteParams, wildcardAsQueryParams);
    }

    string ILocationDataProvider.GetWebApplicationRelativeDirectory(
      IVssRequestContext requestContext)
    {
      return this.LocalDataProvider.GetWebApplicationRelativeDirectory(requestContext);
    }

    DateTime ILocationDataProvider.GetExpirationDate(IVssRequestContext requestContext) => this.LocalDataProvider.GetExpirationDate(requestContext);

    long ILocationDataProvider.GetLastChangeId(IVssRequestContext requestContext) => this.LocalDataProvider.GetLastChangeId(requestContext);

    IEnumerable<ServiceDefinition> IInternalLocationService.FindNonInheritedDefinitions(
      IVssRequestContext requestContext)
    {
      return ((IInternalLocationDataProvider) this.LocalDataProvider).FindNonInheritedDefinitions(requestContext);
    }

    IEnumerable<string> IInternalLocationService.GetActiveProviderUrls(
      IVssRequestContext requestContext)
    {
      LocationService.ProviderCache providerLookup = this.m_providerLookup;
      return providerLookup != null ? providerLookup.Urls : Enumerable.Empty<string>();
    }

    private class ProviderCache
    {
      private ConcurrentDictionary<Guid, ILocationDataProvider> m_guidCache = new ConcurrentDictionary<Guid, ILocationDataProvider>();
      private ConcurrentDictionary<string, ILocationDataProvider> m_urlCache = new ConcurrentDictionary<string, ILocationDataProvider>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

      public IEnumerable<string> Urls => this.m_urlCache.Select<KeyValuePair<string, ILocationDataProvider>, string>((Func<KeyValuePair<string, ILocationDataProvider>, string>) (x => x.Key));

      public bool TryGetValue(Guid locationAreaIdentfier, out ILocationDataProvider provider) => this.m_guidCache.TryGetValue(locationAreaIdentfier, out provider);

      public bool TryGetValue(string locationUrl, out ILocationDataProvider provider) => this.m_urlCache.TryGetValue(LocationService.ProviderCache.NormalizeUrl(locationUrl), out provider);

      public ILocationDataProvider GetOrAdd(string locationUrl, ILocationDataProvider provider) => this.m_urlCache.GetOrAdd(LocationService.ProviderCache.NormalizeUrl(locationUrl), provider);

      public ILocationDataProvider this[Guid locationAreaIdentifier]
      {
        get => this.m_guidCache[locationAreaIdentifier];
        set => this.m_guidCache[locationAreaIdentifier] = value;
      }

      private static string NormalizeUrl(string locationUrl) => UriUtility.AppendSlashToPathIfNeeded(locationUrl);
    }

    internal class LocationDataCache : Microsoft.VisualStudio.Services.Location.Server.LocationDataCache<string>
    {
      public LocationDataCache(
        IVssRequestContext requestContext,
        LocationServiceRedisHelper<string> redisHelper = null)
        : base(requestContext, (ILocationServiceRedisHelper<string>) redisHelper)
      {
      }

      public override bool CanUseRedis(LocationDataKind kind) => kind == LocationDataKind.Remote || kind == LocationDataKind.All;

      public override LocationDataKind GetKind(string key) => !(key == string.Empty) ? LocationDataKind.Remote : LocationDataKind.Local;
    }
  }
}
