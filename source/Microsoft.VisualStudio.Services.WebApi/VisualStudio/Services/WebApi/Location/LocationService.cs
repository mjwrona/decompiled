// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Location.LocationService
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi.Location
{
  internal class LocationService : ILocationService, IVssClientService
  {
    private VssConnection m_connection;
    private LocationService.ProviderCache m_providerLookup;

    public virtual void Initialize(VssConnection connection) => this.m_connection = connection;

    public ILocationDataProvider GetLocationData(Guid locationAreaIdentifier) => this.GetLocationDataAsync(locationAreaIdentifier, new CancellationToken()).SyncResult<ILocationDataProvider>();

    public async Task<ILocationDataProvider> GetLocationDataAsync(
      Guid locationAreaIdentifier,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (locationAreaIdentifier == Guid.Empty || locationAreaIdentifier == LocationServiceConstants.SelfReferenceIdentifier)
        return this.LocalDataProvider;
      Guid instanceId = await this.LocalDataProvider.GetInstanceIdAsync(cancellationToken).ConfigureAwait(false);
      Guid guid = await this.LocalDataProvider.GetInstanceTypeAsync(cancellationToken).ConfigureAwait(false);
      return locationAreaIdentifier == instanceId || locationAreaIdentifier == guid || guid == ServiceInstanceTypes.TFSOnPremises ? this.LocalDataProvider : await this.ResolveLocationDataAsync(locationAreaIdentifier, cancellationToken).ConfigureAwait(false);
    }

    private async Task<ILocationDataProvider> ResolveLocationDataAsync(
      Guid locationAreaIdentifier,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ILocationDataProvider locationData = (ILocationDataProvider) null;
      LocationService.ProviderCache providerLookup = this.m_providerLookup;
      if (providerLookup == null)
      {
        providerLookup = new LocationService.ProviderCache();
        string locationUrl = await this.LocalDataProvider.LocationForCurrentConnectionAsync("LocationService2", LocationServiceConstants.SelfReferenceIdentifier, cancellationToken).ConfigureAwait(false);
        if (locationUrl != null)
          providerLookup.GetOrAdd(locationUrl, this.LocalDataProvider);
        LocationService.ProviderCache providerCache = Interlocked.CompareExchange<LocationService.ProviderCache>(ref this.m_providerLookup, providerLookup, (LocationService.ProviderCache) null);
        if (providerCache != null)
          providerLookup = providerCache;
      }
      if (!providerLookup.TryGetValue(locationAreaIdentifier, out locationData))
      {
        string location = await this.LocalDataProvider.LocationForCurrentConnectionAsync("LocationService2", locationAreaIdentifier, cancellationToken).ConfigureAwait(false);
        if (location == null && locationAreaIdentifier != LocationServiceConstants.ApplicationIdentifier && locationAreaIdentifier != LocationServiceConstants.RootIdentifier)
        {
          ILocationDataProvider locationDataProvider = await this.ResolveLocationDataAsync(LocationServiceConstants.RootIdentifier, cancellationToken).ConfigureAwait(false);
          if (locationDataProvider != null && locationDataProvider != this.LocalDataProvider)
            location = await locationDataProvider.LocationForCurrentConnectionAsync("LocationService2", locationAreaIdentifier, cancellationToken).ConfigureAwait(false);
        }
        if (location != null)
        {
          if (!providerLookup.TryGetValue(location, out locationData))
          {
            locationData = await this.CreateDataProviderAsync(location, cancellationToken).ConfigureAwait(false);
            locationData = providerLookup.GetOrAdd(location, locationData);
          }
          providerLookup[locationAreaIdentifier] = locationData;
        }
        location = (string) null;
      }
      ILocationDataProvider locationDataProvider1 = locationData;
      locationData = (ILocationDataProvider) null;
      providerLookup = (LocationService.ProviderCache) null;
      return locationDataProvider1;
    }

    public string GetLocationServiceUrl(Guid locationAreaIdentifier) => this.GetLocationServiceUrlAsync(locationAreaIdentifier, (string) null, new CancellationToken()).SyncResult<string>();

    public string GetLocationServiceUrl(Guid locationAreaIdentifier, string accessMappingMoniker = null) => this.GetLocationServiceUrlAsync(locationAreaIdentifier, accessMappingMoniker, new CancellationToken()).SyncResult<string>();

    public async Task<string> GetLocationServiceUrlAsync(
      Guid locationAreaIdentifier,
      string accessMappingMoniker = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ILocationDataProvider locationData = await this.GetLocationDataAsync(locationAreaIdentifier, cancellationToken).ConfigureAwait(false);
      if (locationData == null)
        return (string) null;
      ConfiguredTaskAwaitable<AccessMapping> configuredTaskAwaitable = locationData.GetAccessMappingAsync(accessMappingMoniker ?? AccessMappingConstants.PublicAccessMappingMoniker).ConfigureAwait(false);
      AccessMapping accessMapping = await configuredTaskAwaitable;
      if (accessMapping == null)
      {
        configuredTaskAwaitable = locationData.GetClientAccessMappingAsync().ConfigureAwait(false);
        accessMapping = await configuredTaskAwaitable;
      }
      return await locationData.LocationForAccessMappingAsync("LocationService2", LocationServiceConstants.SelfReferenceIdentifier, accessMapping, cancellationToken).ConfigureAwait(false);
    }

    protected virtual async Task<ILocationDataProvider> CreateDataProviderAsync(
      string location,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      VssClientHttpRequestSettings settings = VssClientHttpRequestSettings.Default.Clone();
      settings.SendTimeout = TimeSpan.FromSeconds(30.0);
      IVssServerDataProvider dataProvider = new VssConnection(new Uri(location), this.m_connection.Credentials, (VssHttpRequestSettings) settings).ServerDataProvider;
      if (this.m_connection.ServerDataProvider.HasConnected)
        await dataProvider.ConnectAsync(ConnectOptions.None, cancellationToken).ConfigureAwait(false);
      ILocationDataProvider dataProviderAsync = (ILocationDataProvider) dataProvider;
      dataProvider = (IVssServerDataProvider) null;
      return dataProviderAsync;
    }

    protected virtual ILocationDataProvider LocalDataProvider => (ILocationDataProvider) this.m_connection.ServerDataProvider;

    private class ProviderCache
    {
      private ConcurrentDictionary<Guid, ILocationDataProvider> m_guidCache = new ConcurrentDictionary<Guid, ILocationDataProvider>();
      private ConcurrentDictionary<string, ILocationDataProvider> m_urlCache = new ConcurrentDictionary<string, ILocationDataProvider>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

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
  }
}
