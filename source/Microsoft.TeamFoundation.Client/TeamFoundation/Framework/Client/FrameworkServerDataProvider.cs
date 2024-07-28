// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.FrameworkServerDataProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class FrameworkServerDataProvider : 
    IServerDataProvider,
    ILocationService,
    ILocationServiceInternal
  {
    private readonly TfsConnection m_parent;
    private readonly Uri m_baseUri;
    private readonly string m_fullyQualifiedUrl;
    private TeamFoundationIdentity m_authenticatedIdentity;
    private TeamFoundationIdentity m_authorizedIdentity;
    private Guid m_instanceId;
    private string m_instanceCacheDirectory;
    private string m_instanceVolatileCacheDirectory;
    private string m_userCacheDirectory;
    private Guid m_catalogResourceId;
    private ServerCapabilities m_serverCapabilities;
    private string m_serverVersion;
    private Lazy<LocationWebService> m_locationServicePortal;
    private ConnectOptions m_validConnectionData;
    private bool m_connectionMade;
    private LocationCacheManager m_locationDataCacheManager;
    private object m_lockObject = new object();
    private static readonly ServiceTypeFilter[] s_allServiceTypesFilter = new ServiceTypeFilter[1]
    {
      new ServiceTypeFilter()
      {
        ServiceType = "*",
        Identifier = new Guid("567713db-d56d-4bb0-8f35-604e0e116174")
      }
    };

    public FrameworkServerDataProvider(TfsConnection server, string fullyQualifiedUrl)
    {
      this.m_parent = server;
      this.m_baseUri = server.Uri;
      this.m_fullyQualifiedUrl = fullyQualifiedUrl;
      this.m_locationServicePortal = new Lazy<LocationWebService>((Func<LocationWebService>) (() => new LocationWebService(this.m_parent)), LazyThreadSafetyMode.PublicationOnly);
      this.m_validConnectionData = ConnectOptions.None;
      this.m_connectionMade = false;
      this.m_locationDataCacheManager = new LocationCacheManager(LocationServerMapCache.ReadServerGuid(this.m_fullyQualifiedUrl), this.m_baseUri);
    }

    public bool LocalCacheAvailable => this.m_locationDataCacheManager.LocalCacheAvailable;

    public Guid InstanceId
    {
      get
      {
        this.EnsureConnected(ConnectOptions.None);
        return this.m_instanceId;
      }
    }

    public Guid CachedInstanceId
    {
      get
      {
        if (!this.NeedToConnect(ConnectOptions.None))
          return this.m_instanceId;
        Guid guid = LocationServerMapCache.ReadServerGuid(this.m_fullyQualifiedUrl);
        return Guid.Empty != guid ? guid : this.InstanceId;
      }
    }

    public Guid CatalogResourceId
    {
      get
      {
        this.EnsureConnected(ConnectOptions.None);
        return this.m_catalogResourceId;
      }
    }

    public ServerCapabilities ServerCapabilities
    {
      get
      {
        this.EnsureConnected(ConnectOptions.None);
        return this.m_serverCapabilities;
      }
    }

    public string ServerVersion
    {
      get
      {
        this.EnsureConnected(ConnectOptions.None);
        return this.m_serverVersion;
      }
    }

    public string ClientCacheDirectoryForInstance
    {
      get
      {
        if (this.m_instanceCacheDirectory == null)
          this.m_instanceCacheDirectory = TfsClientCacheUtility.GetCacheDirectory(this.m_baseUri, this.InstanceId);
        return this.m_instanceCacheDirectory;
      }
    }

    public string ClientVolatileCacheDirectoryForInstance
    {
      get
      {
        if (this.m_instanceVolatileCacheDirectory == null)
          this.m_instanceVolatileCacheDirectory = TfsClientCacheUtility.GetVolatileCacheDirectory(this.m_baseUri, this.InstanceId);
        return this.m_instanceVolatileCacheDirectory;
      }
    }

    public string ClientCacheDirectoryForUser
    {
      get
      {
        if (this.m_userCacheDirectory == null)
          this.m_userCacheDirectory = TfsClientCacheUtility.GetCacheDirectory(this.m_baseUri, this.InstanceId, this.AuthenticatedIdentity.TeamFoundationId);
        return this.m_userCacheDirectory;
      }
    }

    public TeamFoundationIdentity AuthorizedIdentity
    {
      get
      {
        this.EnsureAuthenticated();
        return this.m_authorizedIdentity;
      }
    }

    public TeamFoundationIdentity AuthenticatedIdentity
    {
      get
      {
        this.EnsureConnected(ConnectOptions.None);
        return this.m_authenticatedIdentity;
      }
    }

    public bool HasAuthenticated => this.m_authenticatedIdentity != null;

    public void EnsureAuthenticated() => this.EnsureConnected(ConnectOptions.None);

    public void Authenticate() => this.Connect(ConnectOptions.None);

    public AccessMapping DefaultAccessMapping
    {
      get
      {
        AccessMapping defaultAccessMapping = this.m_locationDataCacheManager.DefaultAccessMapping;
        if (defaultAccessMapping == null)
        {
          this.EnsureConnected(ConnectOptions.IncludeServices);
          defaultAccessMapping = this.m_locationDataCacheManager.DefaultAccessMapping;
        }
        return defaultAccessMapping;
      }
    }

    public AccessMapping ClientAccessMapping
    {
      get
      {
        AccessMapping clientAccessMapping = this.m_locationDataCacheManager.ClientAccessMapping;
        if (clientAccessMapping == null)
        {
          this.EnsureConnected(ConnectOptions.IncludeServices);
          clientAccessMapping = this.m_locationDataCacheManager.ClientAccessMapping;
        }
        return clientAccessMapping;
      }
    }

    public IEnumerable<AccessMapping> ConfiguredAccessMappings
    {
      get
      {
        this.EnsureConnected(ConnectOptions.IncludeServices);
        return this.m_locationDataCacheManager.AccessMappings;
      }
    }

    public AccessMapping GetAccessMapping(string moniker)
    {
      ArgumentUtility.CheckForNull<string>(moniker, nameof (moniker));
      this.EnsureConnected(ConnectOptions.IncludeServices);
      return this.m_locationDataCacheManager.GetAccessMapping(moniker);
    }

    public void SaveServiceDefinition(ServiceDefinition serviceDefinition) => this.SaveServiceDefinitions((IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
    {
      serviceDefinition
    });

    public void SaveServiceDefinitions(IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      ArgumentUtility.CheckForNull<IEnumerable<ServiceDefinition>>(serviceDefinitions, nameof (serviceDefinitions));
      foreach (ServiceDefinition serviceDefinition in serviceDefinitions)
      {
        ArgumentUtility.CheckForNull<ServiceDefinition>(serviceDefinition, "serviceDefinition");
        ArgumentUtility.CheckForEmptyGuid(serviceDefinition.Identifier, "serviceDefinition.Identifier");
      }
      this.EnsureConnected(ConnectOptions.None);
      this.m_locationDataCacheManager.LoadServicesData(this.m_locationServicePortal.Value.SaveServiceDefinitions(serviceDefinitions.ToArray<ServiceDefinition>(), this.m_locationDataCacheManager.GetLastChangeId()), false);
      foreach (ServiceDefinition serviceDefinition1 in serviceDefinitions)
      {
        ServiceDefinition serviceDefinition2;
        this.m_locationDataCacheManager.TryFindService(serviceDefinition1.ServiceType, serviceDefinition1.Identifier, false, out serviceDefinition2);
        if (serviceDefinition1.RelativeToSetting == RelativeToSetting.FullyQualified)
          serviceDefinition1.LocationMappings = serviceDefinition2.LocationMappings;
      }
    }

    public void RemoveServiceDefinition(string serviceType, Guid serviceIdentifier) => this.RemoveServiceDefinition(new ServiceDefinition()
    {
      ServiceType = serviceType,
      Identifier = serviceIdentifier
    });

    public void RemoveServiceDefinition(ServiceDefinition serviceDefinition) => this.RemoveServiceDefinitions((IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
    {
      serviceDefinition
    });

    public void RemoveServiceDefinitions(IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      ArgumentUtility.CheckForNull<IEnumerable<ServiceDefinition>>(serviceDefinitions, nameof (serviceDefinitions));
      foreach (ServiceDefinition serviceDefinition in serviceDefinitions)
      {
        ArgumentUtility.CheckForNull<ServiceDefinition>(serviceDefinition, "serviceDefinition");
        ArgumentUtility.CheckForNull<string>(serviceDefinition.ServiceType, "serviceDefinition.ServiceType");
      }
      this.EnsureConnected(ConnectOptions.None);
      LocationServiceData locationServiceData = this.m_locationServicePortal.Value.RemoveServiceDefinitions(serviceDefinitions.ToArray<ServiceDefinition>(), this.m_locationDataCacheManager.GetLastChangeId());
      this.m_locationDataCacheManager.RemoveServices(serviceDefinitions, locationServiceData.LastChangeId);
    }

    public string FindServerLocation(Guid serverGuid) => this.LocationForCurrentConnection("LocationService", serverGuid, false);

    public string LocationForAccessMapping(
      string serviceType,
      Guid serviceIdentifier,
      AccessMapping accessMapping)
    {
      return this.LocationForAccessMapping(this.FindServiceDefinition(serviceType, serviceIdentifier, false) ?? throw new ServiceDefinitionDoesNotExistException(TFCommonResources.ServiceDefinitionDoesNotExist((object) serviceType, (object) serviceIdentifier)), accessMapping);
    }

    public string LocationForAccessMapping(
      ServiceDefinition serviceDefinition,
      AccessMapping accessMapping)
    {
      ArgumentUtility.CheckForNull<ServiceDefinition>(serviceDefinition, nameof (serviceDefinition));
      ArgumentUtility.CheckForNull<AccessMapping>(accessMapping, nameof (accessMapping));
      if (serviceDefinition.RelativeToSetting == RelativeToSetting.FullyQualified)
        return serviceDefinition.GetLocationMapping(accessMapping)?.Location;
      if (string.IsNullOrEmpty(accessMapping.AccessPoint))
        throw new InvalidAccessPointException(TFCommonResources.InvalidAccessMappingLocationServiceUrl());
      string path2 = this.m_locationDataCacheManager.WebApplicationRelativeDirectory;
      if (accessMapping.VirtualDirectory != null)
        path2 = accessMapping.VirtualDirectory;
      Uri uri = new Uri(accessMapping.AccessPoint);
      string path1 = string.Empty;
      switch (serviceDefinition.RelativeToSetting)
      {
        case RelativeToSetting.Context:
          path1 = TFCommonUtil.CombinePaths(uri.AbsoluteUri, path2);
          break;
        case RelativeToSetting.WebApplication:
          path1 = accessMapping.AccessPoint;
          break;
      }
      return TFCommonUtil.CombinePaths(path1, serviceDefinition.RelativePath);
    }

    string ILocationService.LocationForCurrentConnection(string serviceType, Guid serviceIdentifier) => this.LocationForCurrentConnection(serviceType, serviceIdentifier, false);

    public string LocationForCurrentConnection(
      string serviceType,
      Guid serviceIdentifier,
      bool ignoreCacheExpiration = false)
    {
      ServiceDefinition serviceDefinition = this.FindServiceDefinition(serviceType, serviceIdentifier, ignoreCacheExpiration);
      return serviceDefinition == null ? (string) null : this.LocationForCurrentConnection(serviceDefinition);
    }

    public string LocationForCurrentConnection(ServiceDefinition serviceDefinition)
    {
      string str = this.LocationForAccessMapping(serviceDefinition, this.ClientAccessMapping);
      if (str == null)
      {
        str = this.LocationForAccessMapping(serviceDefinition, this.DefaultAccessMapping);
        if (str == null)
          str = (serviceDefinition.LocationMappings.FirstOrDefault<LocationMapping>() ?? throw new InvalidServiceDefinitionException(TFCommonResources.ServiceDefinitionWithNoLocations((object) serviceDefinition.ServiceType))).Location;
      }
      return str;
    }

    public IEnumerable<ServiceDefinition> FindServiceDefinitions(string serviceType)
    {
      IEnumerable<ServiceDefinition> services = this.m_locationDataCacheManager != null ? this.m_locationDataCacheManager.FindServices(serviceType) : (IEnumerable<ServiceDefinition>) null;
      if (services != null)
        return services;
      this.CheckForServerUpdates();
      return this.m_locationDataCacheManager.FindServices(serviceType);
    }

    public IEnumerable<ServiceDefinition> FindServiceDefinitionsByToolType(string toolId)
    {
      IEnumerable<ServiceDefinition> definitionsByToolType = this.m_locationDataCacheManager.FindServicesByToolId(toolId);
      if (definitionsByToolType == null)
      {
        this.CheckForServerUpdates();
        definitionsByToolType = this.m_locationDataCacheManager.FindServicesByToolId(toolId) ?? (IEnumerable<ServiceDefinition>) Array.Empty<ServiceDefinition>();
      }
      return definitionsByToolType;
    }

    ServiceDefinition ILocationService.FindServiceDefinition(
      string serviceType,
      Guid serviceIdentifier)
    {
      return this.FindServiceDefinition(serviceType, serviceIdentifier, false);
    }

    public ServiceDefinition FindServiceDefinition(
      string serviceType,
      Guid serviceIdentifier,
      bool ignoreCacheExpiration = false)
    {
      ArgumentUtility.CheckForNull<string>(serviceType, nameof (serviceType));
      int lastChangeId = this.m_locationDataCacheManager.GetLastChangeId();
      ServiceDefinition serviceDefinition;
      if (this.m_locationDataCacheManager.TryFindService(serviceType, serviceIdentifier, ignoreCacheExpiration, out serviceDefinition))
        return serviceDefinition;
      this.CheckForServerUpdates();
      if (this.m_locationDataCacheManager.TryFindService(serviceType, serviceIdentifier, ignoreCacheExpiration, out serviceDefinition))
        return serviceDefinition;
      this.m_locationDataCacheManager.AddCachedMiss(serviceType, serviceIdentifier, lastChangeId);
      return (ServiceDefinition) null;
    }

    public AccessMapping ConfigureAccessMapping(
      string moniker,
      string displayName,
      string accessPoint,
      bool makeDefault)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(moniker, nameof (moniker));
      this.EnsureConnected(ConnectOptions.None);
      this.m_locationDataCacheManager.LoadServicesData(this.m_locationServicePortal.Value.ConfigureAccessMapping(new AccessMapping(moniker, displayName, accessPoint), this.m_locationDataCacheManager.GetLastChangeId(), makeDefault), false);
      return this.m_locationDataCacheManager.GetAccessMapping(moniker);
    }

    public void SetDefaultAccessMapping(AccessMapping accessMapping)
    {
      ArgumentUtility.CheckForNull<AccessMapping>(accessMapping, nameof (accessMapping));
      ArgumentUtility.CheckStringForNullOrEmpty(accessMapping.Moniker, "accessMapping.Moniker");
      this.EnsureConnected(ConnectOptions.None);
      this.m_locationDataCacheManager.LoadServicesData(this.m_locationServicePortal.Value.SetDefaultAccessMapping(accessMapping, this.m_locationDataCacheManager.GetLastChangeId()), false);
    }

    public void RemoveAccessMapping(string moniker)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(moniker, nameof (moniker));
      this.EnsureConnected(ConnectOptions.None);
      AccessMapping accessMapping = new AccessMapping(moniker, (string) null, (string) null);
      this.m_locationServicePortal.Value.RemoveAccessMapping(accessMapping, this.m_locationDataCacheManager.GetLastChangeId());
      this.m_locationDataCacheManager.RemoveAccessMapping(accessMapping.Moniker);
    }

    public void ReactToPossibleServerUpdate(int serverLastChangeId) => this.m_locationDataCacheManager.ClearIfCacheNotFresh(serverLastChangeId);

    private void CheckForServerUpdates()
    {
      bool flag = false;
      if (this.NeedToConnect(ConnectOptions.IncludeServices))
      {
        lock (this.m_lockObject)
        {
          if (this.NeedToConnect(ConnectOptions.IncludeServices))
          {
            this.Connect(ConnectOptions.IncludeServices);
            flag = true;
          }
        }
      }
      if (flag)
        return;
      this.m_locationDataCacheManager.LoadServicesData(this.m_locationServicePortal.Value.QueryServices(FrameworkServerDataProvider.s_allServiceTypesFilter, this.m_locationDataCacheManager.GetLastChangeId()), true);
    }

    private void EnsureConnected(ConnectOptions optionsNeeded)
    {
      if (!this.NeedToConnect(optionsNeeded))
        return;
      lock (this.m_lockObject)
      {
        if (!this.NeedToConnect(optionsNeeded))
          return;
        this.Connect(optionsNeeded);
      }
    }

    private bool NeedToConnect(ConnectOptions optionsNeeded) => !this.m_connectionMade || (optionsNeeded & this.m_validConnectionData) != optionsNeeded;

    public void Connect(ConnectOptions connectOptions)
    {
      if (!this.m_locationDataCacheManager.AccessMappings.Any<AccessMapping>())
        connectOptions |= ConnectOptions.IncludeServices;
      bool allServicesIncluded = (connectOptions & ConnectOptions.IncludeServices) == ConnectOptions.IncludeServices;
      int lastChangeId = this.m_locationDataCacheManager.GetLastChangeId();
      ConnectionData connectionData = this.m_locationServicePortal.Value.Connect((int) connectOptions, lastChangeId, 1);
      LocationServiceData locationServiceData = connectionData.LocationServiceData;
      if (connectionData.AuthenticatedUser != null)
        connectionData.AuthenticatedUser.InitializeFromWebService();
      if (this.m_authenticatedIdentity != null && !IdentityDescriptorComparer.Instance.Equals(this.m_authenticatedIdentity.Descriptor, connectionData.AuthenticatedUser.Descriptor))
        throw new TeamFoundationServerUnauthorizedException(ClientResources.CannotAuthenticateAsAnotherUser((object) this.m_authenticatedIdentity.GetAttribute("Account", string.Empty), (object) connectionData.AuthenticatedUser.GetAttribute("Account", string.Empty)));
      this.m_authenticatedIdentity = connectionData.AuthenticatedUser;
      if (connectionData.AuthorizedUser != null)
        connectionData.AuthorizedUser.InitializeFromWebService();
      this.m_authorizedIdentity = connectionData.AuthorizedUser;
      this.m_instanceId = connectionData.InstanceId;
      this.m_catalogResourceId = connectionData.CatalogResourceId;
      this.m_serverCapabilities = connectionData.ServerCapabilities;
      this.m_serverVersion = connectionData.ServerVersion;
      if (LocationServerMapCache.EnsureServerMappingExists(this.m_fullyQualifiedUrl, this.m_instanceId))
      {
        if (allServicesIncluded && connectionData.LocationServiceData.ServiceDefinitions.Length == 0)
          locationServiceData = this.m_locationServicePortal.Value.QueryServices((ServiceTypeFilter[]) null, -1);
        this.m_locationDataCacheManager = new LocationCacheManager(this.m_instanceId, this.m_baseUri);
        this.m_validConnectionData = ConnectOptions.None;
      }
      this.m_locationDataCacheManager.WebApplicationRelativeDirectory = connectionData.WebApplicationRelativeDirectory;
      if (locationServiceData != null)
        this.m_locationDataCacheManager.LoadServicesData(locationServiceData, allServicesIncluded);
      this.m_validConnectionData |= connectOptions;
      this.m_connectionMade = true;
    }

    public void Disconnect()
    {
      this.m_connectionMade = false;
      this.m_authenticatedIdentity = (TeamFoundationIdentity) null;
      this.m_authorizedIdentity = (TeamFoundationIdentity) null;
      this.m_userCacheDirectory = (string) null;
      this.m_locationServicePortal = new Lazy<LocationWebService>((Func<LocationWebService>) (() => new LocationWebService(this.m_parent)), LazyThreadSafetyMode.PublicationOnly);
    }
  }
}
