// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Location.VssServerDataProvider
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Client;
using Microsoft.VisualStudio.Services.WebApi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi.Location
{
  internal class VssServerDataProvider : IVssServerDataProvider, ILocationDataProvider
  {
    private VssConnection m_connection;
    private Uri m_baseUri;
    private string m_fullyQualifiedUrl;
    private Microsoft.VisualStudio.Services.Identity.Identity m_authenticatedIdentity;
    private Microsoft.VisualStudio.Services.Identity.Identity m_authorizedIdentity;
    private Guid m_instanceId;
    private Guid m_serviceOwner;
    private LocationHttpClient m_locationClient;
    private ConnectOptions m_validConnectionData;
    private bool m_connectionMade;
    private LocationCacheManager m_locationDataCacheManager;
    private ApiResourceLocationCollection m_resourceLocations;
    private readonly AsyncLock m_connectionLock = new AsyncLock();

    public VssServerDataProvider(
      VssConnection connection,
      HttpMessageHandler pipeline,
      string fullyQualifiedUrl)
    {
      this.m_connection = connection;
      this.m_baseUri = connection.Uri;
      this.m_fullyQualifiedUrl = fullyQualifiedUrl;
      this.m_locationClient = new LocationHttpClient(this.m_baseUri, pipeline, false);
      ServerMapData serverMapData = LocationServerMapCache.ReadServerData(this.m_fullyQualifiedUrl);
      this.m_locationDataCacheManager = new LocationCacheManager(serverMapData.ServerId, serverMapData.ServiceOwner, this.m_baseUri);
    }

    internal VssConnection Connection => this.m_connection;

    public bool HasConnected => this.m_connectionMade;

    public async Task<Microsoft.VisualStudio.Services.Identity.Identity> GetAuthorizedIdentityAsync(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      int num = await this.EnsureConnectedAsync(ConnectOptions.None).ConfigureAwait(false) ? 1 : 0;
      return this.m_authorizedIdentity;
    }

    public async Task<Microsoft.VisualStudio.Services.Identity.Identity> GetAuthenticatedIdentityAsync(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      int num = await this.EnsureConnectedAsync(ConnectOptions.None).ConfigureAwait(false) ? 1 : 0;
      return this.m_authenticatedIdentity;
    }

    public Guid InstanceId => this.GetInstanceIdAsync(new CancellationToken()).SyncResult<Guid>();

    public Guid InstanceType => this.GetInstanceTypeAsync(new CancellationToken()).SyncResult<Guid>();

    public async Task<Guid> GetInstanceIdAsync(CancellationToken cancellationToken = default (CancellationToken))
    {
      if (!this.NeedToConnect(ConnectOptions.None))
        return this.m_instanceId;
      Guid serverId = LocationServerMapCache.ReadServerData(this.m_fullyQualifiedUrl).ServerId;
      if (Guid.Empty != serverId)
        return serverId;
      int num = await this.EnsureConnectedAsync(ConnectOptions.None, cancellationToken).ConfigureAwait(false) ? 1 : 0;
      return this.m_instanceId;
    }

    public async Task<Guid> GetInstanceTypeAsync(CancellationToken cancellationToken = default (CancellationToken))
    {
      if (!this.NeedToConnect(ConnectOptions.None))
        return this.m_serviceOwner;
      Guid serviceOwner = LocationServerMapCache.ReadServerData(this.m_fullyQualifiedUrl).ServiceOwner;
      if (Guid.Empty != serviceOwner)
        return serviceOwner;
      int num = await this.EnsureConnectedAsync(ConnectOptions.None, cancellationToken).ConfigureAwait(false) ? 1 : 0;
      return this.m_serviceOwner;
    }

    public AccessMapping DefaultAccessMapping => this.GetDefaultAccessMappingAsync(new CancellationToken()).SyncResult<AccessMapping>();

    public async Task<AccessMapping> GetDefaultAccessMappingAsync(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccessMapping defaultAccessMapping = this.m_locationDataCacheManager.DefaultAccessMapping;
      if (defaultAccessMapping == null)
      {
        int num = await this.EnsureConnectedAsync(ConnectOptions.IncludeServices, cancellationToken).ConfigureAwait(false) ? 1 : 0;
        defaultAccessMapping = this.m_locationDataCacheManager.DefaultAccessMapping;
      }
      return defaultAccessMapping;
    }

    public AccessMapping ClientAccessMapping => this.GetClientAccessMappingAsync(new CancellationToken()).SyncResult<AccessMapping>();

    public async Task<AccessMapping> GetClientAccessMappingAsync(CancellationToken cancellationToken = default (CancellationToken))
    {
      AccessMapping clientAccessMapping = this.m_locationDataCacheManager.ClientAccessMapping;
      if (clientAccessMapping == null)
      {
        int num = await this.EnsureConnectedAsync(ConnectOptions.IncludeServices, cancellationToken).ConfigureAwait(false) ? 1 : 0;
        clientAccessMapping = this.m_locationDataCacheManager.ClientAccessMapping;
      }
      return clientAccessMapping;
    }

    public IEnumerable<AccessMapping> ConfiguredAccessMappings => this.GetConfiguredAccessMappingsAsync(new CancellationToken()).SyncResult<IEnumerable<AccessMapping>>();

    public async Task<IEnumerable<AccessMapping>> GetConfiguredAccessMappingsAsync(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      int num = await this.EnsureConnectedAsync(ConnectOptions.IncludeServices, cancellationToken).ConfigureAwait(false) ? 1 : 0;
      return this.m_locationDataCacheManager.AccessMappings;
    }

    public AccessMapping GetAccessMapping(string moniker) => this.GetAccessMappingAsync(moniker, new CancellationToken()).SyncResult<AccessMapping>();

    public async Task<AccessMapping> GetAccessMappingAsync(
      string moniker,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<string>(moniker, nameof (moniker));
      int num = await this.EnsureConnectedAsync(ConnectOptions.IncludeServices, cancellationToken).ConfigureAwait(false) ? 1 : 0;
      return this.m_locationDataCacheManager.GetAccessMapping(moniker);
    }

    public string LocationForAccessMapping(
      string serviceType,
      Guid serviceIdentifier,
      AccessMapping accessMapping)
    {
      return this.LocationForAccessMappingAsync(serviceType, serviceIdentifier, accessMapping, new CancellationToken()).SyncResult<string>();
    }

    public async Task<string> LocationForAccessMappingAsync(
      string serviceType,
      Guid serviceIdentifier,
      AccessMapping accessMapping,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ServiceDefinition serviceDefinition = await this.FindServiceDefinitionAsync(serviceType, serviceIdentifier, cancellationToken).ConfigureAwait(false);
      if (serviceDefinition == null)
        throw new ServiceDefinitionDoesNotExistException(WebApiResources.ServiceDefinitionDoesNotExist((object) serviceType, (object) serviceIdentifier));
      return await this.LocationForAccessMappingAsync(serviceDefinition, accessMapping, cancellationToken).ConfigureAwait(false);
    }

    public string LocationForAccessMapping(
      ServiceDefinition serviceDefinition,
      AccessMapping accessMapping)
    {
      return this.LocationForAccessMappingAsync(serviceDefinition, accessMapping, new CancellationToken()).SyncResult<string>();
    }

    public Task<string> LocationForAccessMappingAsync(
      ServiceDefinition serviceDefinition,
      AccessMapping accessMapping,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<ServiceDefinition>(serviceDefinition, nameof (serviceDefinition));
      ArgumentUtility.CheckForNull<AccessMapping>(accessMapping, nameof (accessMapping));
      if (serviceDefinition.RelativeToSetting == RelativeToSetting.FullyQualified)
      {
        LocationMapping locationMapping = serviceDefinition.GetLocationMapping(accessMapping);
        return locationMapping != null ? Task.FromResult<string>(locationMapping.Location) : Task.FromResult<string>((string) null);
      }
      if (string.IsNullOrEmpty(accessMapping.AccessPoint))
        throw new InvalidAccessPointException(WebApiResources.InvalidAccessMappingLocationServiceUrl());
      string path2 = this.m_locationDataCacheManager.WebApplicationRelativeDirectory;
      if (accessMapping.VirtualDirectory != null)
        path2 = accessMapping.VirtualDirectory;
      Uri uri = new Uri(accessMapping.AccessPoint);
      string path1 = string.Empty;
      switch (serviceDefinition.RelativeToSetting)
      {
        case RelativeToSetting.Context:
          path1 = PathUtility.Combine(uri.AbsoluteUri, path2);
          break;
        case RelativeToSetting.WebApplication:
          path1 = accessMapping.AccessPoint;
          break;
      }
      return Task.FromResult<string>(PathUtility.Combine(path1, serviceDefinition.RelativePath));
    }

    public string LocationForCurrentConnection(string serviceType, Guid serviceIdentifier) => this.LocationForCurrentConnectionAsync(serviceType, serviceIdentifier, new CancellationToken()).SyncResult<string>();

    public async Task<string> LocationForCurrentConnectionAsync(
      string serviceType,
      Guid serviceIdentifier,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (StringComparer.CurrentCultureIgnoreCase.Equals(serviceType, "LocationService2") && serviceIdentifier == LocationServiceConstants.SelfReferenceIdentifier)
        return this.m_baseUri.AbsoluteUri;
      ServiceDefinition serviceDefinition = await this.FindServiceDefinitionAsync(serviceType, serviceIdentifier, cancellationToken).ConfigureAwait(false);
      return serviceDefinition == null ? (string) null : await this.LocationForCurrentConnectionAsync(serviceDefinition, cancellationToken).ConfigureAwait(false);
    }

    public string LocationForCurrentConnection(ServiceDefinition serviceDefinition) => this.LocationForCurrentConnectionAsync(serviceDefinition, new CancellationToken()).SyncResult<string>();

    public async Task<string> LocationForCurrentConnectionAsync(
      ServiceDefinition serviceDefinition,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ConfiguredTaskAwaitable<AccessMapping> configuredTaskAwaitable = this.GetClientAccessMappingAsync(cancellationToken).ConfigureAwait(false);
      AccessMapping accessMapping1 = await configuredTaskAwaitable;
      string str = await this.LocationForAccessMappingAsync(serviceDefinition, accessMapping1, cancellationToken).ConfigureAwait(false);
      if (str == null)
      {
        configuredTaskAwaitable = this.GetDefaultAccessMappingAsync(cancellationToken).ConfigureAwait(false);
        AccessMapping accessMapping2 = await configuredTaskAwaitable;
        str = await this.LocationForAccessMappingAsync(serviceDefinition, accessMapping2, cancellationToken).ConfigureAwait(false);
        if (str == null)
          str = (serviceDefinition.LocationMappings.FirstOrDefault<LocationMapping>() ?? throw new InvalidServiceDefinitionException(WebApiResources.ServiceDefinitionWithNoLocations((object) serviceDefinition.ServiceType))).Location;
      }
      return str;
    }

    public IEnumerable<ServiceDefinition> FindServiceDefinitions(string serviceType) => this.FindServiceDefinitionsAsync(serviceType, new CancellationToken()).SyncResult<IEnumerable<ServiceDefinition>>();

    public async Task<IEnumerable<ServiceDefinition>> FindServiceDefinitionsAsync(
      string serviceType,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IEnumerable<ServiceDefinition> definitionsAsync = (IEnumerable<ServiceDefinition>) null;
      if (this.m_locationDataCacheManager != null)
        definitionsAsync = this.m_locationDataCacheManager.FindServices(serviceType);
      if (definitionsAsync != null)
        return definitionsAsync;
      await this.CheckForServerUpdatesAsync(cancellationToken).ConfigureAwait(false);
      return this.m_locationDataCacheManager.FindServices(serviceType);
    }

    public ServiceDefinition FindServiceDefinition(string serviceType, Guid serviceIdentifier) => this.FindServiceDefinitionAsync(serviceType, serviceIdentifier, new CancellationToken()).SyncResult<ServiceDefinition>();

    public async Task<ServiceDefinition> FindServiceDefinitionAsync(
      string serviceType,
      Guid serviceIdentifier,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<string>(serviceType, nameof (serviceType));
      int lastChangeId = this.m_locationDataCacheManager.GetLastChangeId();
      ServiceDefinition serviceDefinition;
      if (this.m_locationDataCacheManager.TryFindService(serviceType, serviceIdentifier, out serviceDefinition))
        return serviceDefinition;
      await this.CheckForServerUpdatesAsync(cancellationToken).ConfigureAwait(false);
      if (!this.m_locationDataCacheManager.TryFindService(serviceType, serviceIdentifier, out serviceDefinition))
      {
        bool flag = string.Equals(serviceType, "LocationService2", StringComparison.OrdinalIgnoreCase) && serviceIdentifier != LocationServiceConstants.RootIdentifier && serviceIdentifier != LocationServiceConstants.ApplicationIdentifier;
        if (flag)
          flag = await this.GetInstanceTypeAsync(cancellationToken).ConfigureAwait(false) == LocationServiceConstants.RootIdentifier;
        if (flag)
        {
          serviceDefinition = await this.m_locationClient.GetServiceDefinitionAsync(serviceType, serviceIdentifier, cancellationToken).ConfigureAwait(false);
        }
        else
        {
          this.m_locationDataCacheManager.AddCachedMiss(serviceType, serviceIdentifier, lastChangeId);
          return (ServiceDefinition) null;
        }
      }
      return serviceDefinition;
    }

    public ApiResourceLocationCollection GetResourceLocations() => this.GetResourceLocationsAsync(new CancellationToken()).SyncResult<ApiResourceLocationCollection>();

    public async Task<ApiResourceLocationCollection> GetResourceLocationsAsync(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.m_resourceLocations == null)
      {
        IEnumerable<ServiceDefinition> source1 = await this.FindServiceDefinitionsAsync((string) null).ConfigureAwait(false);
        if (source1 != null)
        {
          IEnumerable<ServiceDefinition> source2 = source1.Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (x => x.ResourceVersion > 0));
          if (source2.Any<ServiceDefinition>())
          {
            ApiResourceLocationCollection locationCollection = new ApiResourceLocationCollection();
            foreach (ServiceDefinition definition in source2)
              locationCollection.AddResourceLocation(ApiResourceLocation.FromServiceDefinition(definition));
            this.m_resourceLocations = locationCollection;
          }
        }
      }
      return this.m_resourceLocations;
    }

    private async Task CheckForServerUpdatesAsync(CancellationToken cancellationToken = default (CancellationToken))
    {
      if (await this.EnsureConnectedAsync(ConnectOptions.IncludeServices, cancellationToken).ConfigureAwait(false) || this.m_locationDataCacheManager.GetLastChangeId() != -1)
        return;
      await this.ConnectAsync(ConnectOptions.IncludeServices, cancellationToken).ConfigureAwait(false);
    }

    private async Task<bool> EnsureConnectedAsync(
      ConnectOptions optionsNeeded,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.NeedToConnect(optionsNeeded))
      {
        using (await this.m_connectionLock.LockAsync(cancellationToken).ConfigureAwait(false))
        {
          if (this.NeedToConnect(optionsNeeded))
          {
            await this.ConnectAsync(optionsNeeded, cancellationToken).ConfigureAwait(false);
            return true;
          }
        }
      }
      return false;
    }

    private bool NeedToConnect(ConnectOptions optionsNeeded)
    {
      if (this.m_locationDataCacheManager.CacheDataExpired)
      {
        this.m_connectionMade = false;
        this.m_validConnectionData = ConnectOptions.None;
      }
      return !this.m_connectionMade || (optionsNeeded & this.m_validConnectionData) != optionsNeeded;
    }

    public async Task ConnectAsync(
      ConnectOptions connectOptions,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (!this.m_locationDataCacheManager.AccessMappings.Any<AccessMapping>())
        connectOptions |= ConnectOptions.IncludeServices;
      int lastChangeId = this.m_locationDataCacheManager.GetLastChangeId();
      if (lastChangeId == -1)
        connectOptions |= ConnectOptions.IncludeServices;
      bool includeServices = (connectOptions & ConnectOptions.IncludeServices) == ConnectOptions.IncludeServices;
      ConnectionData connectionData = await this.GetConnectionDataAsync(connectOptions, lastChangeId, cancellationToken).ConfigureAwait(false);
      LocationServiceData locationServiceData = connectionData.LocationServiceData;
      if (this.m_authenticatedIdentity != null && !IdentityDescriptorComparer.Instance.Equals(this.m_authenticatedIdentity.Descriptor, connectionData.AuthenticatedUser.Descriptor))
        throw new VssAuthenticationException(WebApiResources.CannotAuthenticateAsAnotherUser((object) this.m_authenticatedIdentity.DisplayName, (object) connectionData.AuthenticatedUser.DisplayName));
      this.m_authenticatedIdentity = connectionData.AuthenticatedUser;
      this.m_authorizedIdentity = connectionData.AuthorizedUser;
      this.m_instanceId = connectionData.InstanceId;
      if (locationServiceData != null)
      {
        Guid guid = connectionData.LocationServiceData.ServiceOwner;
        if (Guid.Empty == guid)
          guid = ServiceInstanceTypes.TFSOnPremises;
        this.m_serviceOwner = guid;
      }
      if (LocationServerMapCache.EnsureServerMappingExists(this.m_fullyQualifiedUrl, this.m_instanceId, this.m_serviceOwner))
      {
        if (includeServices && (connectionData.LocationServiceData.ServiceDefinitions == null || connectionData.LocationServiceData.ServiceDefinitions.Count == 0))
          locationServiceData = (await this.GetConnectionDataAsync(ConnectOptions.IncludeServices, -1, cancellationToken).ConfigureAwait(false)).LocationServiceData;
        this.m_locationDataCacheManager = new LocationCacheManager(this.m_instanceId, this.m_serviceOwner, this.m_baseUri);
      }
      this.m_locationDataCacheManager.WebApplicationRelativeDirectory = connectionData.WebApplicationRelativeDirectory;
      if (locationServiceData != null)
        this.m_locationDataCacheManager.LoadServicesData(locationServiceData, includeServices);
      this.m_validConnectionData |= connectOptions;
      this.m_connectionMade = true;
      connectionData = (ConnectionData) null;
    }

    public Task DisconnectAsync(CancellationToken cancellationToken = default (CancellationToken))
    {
      this.m_connectionMade = false;
      this.m_authenticatedIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      this.m_authorizedIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      return (Task) Task.FromResult<object>((object) null);
    }

    private async Task<ConnectionData> GetConnectionDataAsync(
      ConnectOptions connectOptions,
      int lastChangeId,
      CancellationToken cancellationToken)
    {
      int timeoutRetries = 1;
      ConnectionData connectionDataAsync;
      while (true)
      {
        try
        {
          connectionDataAsync = await this.m_locationClient.GetConnectionDataAsync(connectOptions, (long) lastChangeId, cancellationToken).ConfigureAwait(false);
          break;
        }
        catch (TimeoutException ex) when (timeoutRetries-- > 0)
        {
        }
      }
      return connectionDataAsync;
    }
  }
}
