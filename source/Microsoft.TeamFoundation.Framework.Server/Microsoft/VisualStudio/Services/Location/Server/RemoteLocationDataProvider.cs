// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.RemoteLocationDataProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal class RemoteLocationDataProvider : LocationDataProvider
  {
    private Guid m_hostId;
    private Guid m_deploymentId;
    private Guid m_instanceType;
    private string m_cacheKey;
    private string m_remoteLocationUrl;
    private static readonly string s_area = "LocationService";
    private static readonly string s_layer = nameof (RemoteLocationDataProvider);
    private bool m_useInheritedData;

    public RemoteLocationDataProvider(
      IVssRequestContext requestContext,
      ILocationDataCache<string> locationCache,
      string remoteLocationUrl)
      : base(requestContext, locationCache)
    {
      this.m_remoteLocationUrl = remoteLocationUrl;
      this.m_cacheKey = remoteLocationUrl.ToLowerInvariant();
      LocationData locationData = this.GetLocationData(requestContext);
      this.m_hostId = locationData != null && locationData != LocationData.Null ? locationData.HostId : throw new ServiceOwnerNotFoundException(remoteLocationUrl, requestContext.ServiceHost.InstanceId);
      this.m_deploymentId = locationData.DeploymentId;
      this.m_instanceType = locationData.InstanceType;
      this.m_useInheritedData = this.m_deploymentId != Guid.Empty && !requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment);
      if (requestContext.IsVirtualServiceHost() || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || !(this.m_instanceType == ServiceInstanceTypes.SPS) || requestContext.ServiceHost.Status != TeamFoundationServiceHostStatus.Started)
        return;
      ServiceDefinition serviceDefinition = locationData.FindServiceDefinition("LocationService2", requestContext.ServiceInstanceType());
      if (serviceDefinition == null)
      {
        requestContext.Trace(924270, TraceLevel.Error, RemoteLocationDataProvider.s_area, RemoteLocationDataProvider.s_layer, "Our host instance mapping is null. This host should not exist.");
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.RemoteLocationProvider.DuplicateHostsCheck"))
          throw new TeamFoundationServiceException("The host instance mapping for this host is null. This host should not exist.");
      }
      else
      {
        if (!(serviceDefinition.ParentIdentifier != requestContext.ServiceHost.DeploymentServiceHost.InstanceId))
          return;
        requestContext.Trace(2298538, TraceLevel.Error, RemoteLocationDataProvider.s_area, RemoteLocationDataProvider.s_layer, "Our host instance mapping points to another scale unit ({0}). The current host may be a duplicate.", (object) serviceDefinition.ParentIdentifier);
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.RemoteLocationProvider.DuplicateHostsCheck"))
          throw new HostAlreadyExistsException(requestContext.ServiceHost.InstanceId.ToString());
      }
    }

    public override Guid HostId => this.m_hostId;

    public override Guid InstanceType => this.m_instanceType;

    protected override string CacheKey => this.m_cacheKey;

    protected override Guid DeploymentId => this.m_deploymentId;

    public override AccessMapping DetermineAccessMapping(IVssRequestContext requestContext)
    {
      AccessMapping accessMapping = requestContext.GetService<ILocationService>().GetLocationData(requestContext, requestContext.ServiceInstanceType()).DetermineAccessMapping(requestContext);
      if (VssStringComparer.AccessMappingMoniker.Equals(accessMapping?.Moniker, AccessMappingConstants.RootDomainMappingMoniker))
        accessMapping = (AccessMapping) null;
      return (accessMapping != null ? this.GetAccessMapping(requestContext, accessMapping.Moniker) : (AccessMapping) null) ?? this.GetDefaultAccessMapping(requestContext);
    }

    public override void SaveServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      this.LocationCache.Update(requestContext, this.CacheKey, (Action) (() => this.CreateLocationClient(requestContext, true).UpdateServiceDefinitionsAsync(serviceDefinitions).SyncResult()));
    }

    public override void RemoveServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      this.LocationCache.Update(requestContext, this.CacheKey, (Action) (() =>
      {
        LocationHttpClient locationClient = this.CreateLocationClient(requestContext, true);
        foreach (ServiceDefinition serviceDefinition in serviceDefinitions)
          TaskExtensions.SyncResult(locationClient.DeleteServiceDefinitionAsync(serviceDefinition.ServiceType, serviceDefinition.Identifier));
      }));
    }

    public override ServiceDefinition FindServiceDefinition(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier)
    {
      return this.FindServiceDefinitionWithFaultIn(requestContext, serviceType, identifier, false);
    }

    public override ServiceDefinition FindServiceDefinitionWithFaultIn(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      bool previewFaultIn)
    {
      ServiceDefinition definitionWithFaultIn = base.FindServiceDefinition(requestContext, serviceType, identifier);
      if (definitionWithFaultIn == null && this.CanFaultInDefinition(requestContext, serviceType, identifier))
      {
        RemoteLocationDataProvider.FaultInMissesMemoryCacheService service = requestContext.GetService<RemoteLocationDataProvider.FaultInMissesMemoryCacheService>();
        if (!service.TryGetValue(requestContext, identifier, out bool _))
        {
          definitionWithFaultIn = this.FaultInServiceDefinition(requestContext, serviceType, identifier, previewFaultIn);
          if (definitionWithFaultIn != null)
          {
            LocationServiceHelper.PostProcessRemoteDefinitions((IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
            {
              definitionWithFaultIn
            }, this.m_instanceType);
            if (!previewFaultIn)
              this.LocationCache.Invalidate(requestContext, this.CacheKey, true);
          }
          else
            service.TryAdd(requestContext, identifier, true);
        }
      }
      return definitionWithFaultIn;
    }

    internal virtual ServiceDefinition FaultInServiceDefinition(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      bool previewFaultIn)
    {
      return this.CreateLocationClient(requestContext.Elevate(), true).GetServiceDefinitionAsync(serviceType, identifier, true, previewFaultIn).SyncResult<ServiceDefinition>();
    }

    protected internal override LocationData FetchLocationData(IVssRequestContext requestContext)
    {
      LocationHttpClient locationClient = this.CreateLocationClient(requestContext.Elevate(), false);
      Microsoft.VisualStudio.Services.WebApi.ConnectOptions connectOptions = Microsoft.VisualStudio.Services.WebApi.ConnectOptions.IncludeServices;
      if (!requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        connectOptions |= Microsoft.VisualStudio.Services.WebApi.ConnectOptions.IncludeNonInheritedDefinitionsOnly;
      int num = (int) connectOptions;
      CancellationToken cancellationToken = new CancellationToken();
      ConnectionData connectionData = locationClient.GetConnectionDataAsync((Microsoft.VisualStudio.Services.WebApi.ConnectOptions) num, 0L, cancellationToken).SyncResult<ConnectionData>();
      if (this.m_instanceType != Guid.Empty && this.m_instanceType != connectionData.LocationServiceData.ServiceOwner)
      {
        requestContext.Trace(72005, TraceLevel.Error, RemoteLocationDataProvider.s_area, RemoteLocationDataProvider.s_layer, "How did the instance type guid change? CurrentInstanceType: {0}. NewInstanceType: {1}", (object) this.m_instanceType, (object) connectionData.LocationServiceData.ServiceOwner);
        return (LocationData) null;
      }
      if (connectionData.LocationServiceData.ServiceOwner == Guid.Empty)
      {
        requestContext.Trace(72006, TraceLevel.Error, RemoteLocationDataProvider.s_area, RemoteLocationDataProvider.s_layer, "Remote location server returned InstanceType == Guid.Empty! Url: {0}", (object) this.m_remoteLocationUrl);
        return (LocationData) null;
      }
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && connectionData.InstanceId != requestContext.ServiceHost.InstanceId)
      {
        requestContext.Trace(72004, TraceLevel.Error, RemoteLocationDataProvider.s_area, RemoteLocationDataProvider.s_layer, "Did not reach the right location service! LocalInstanceId: {0}. RemoteInstanceId: {1}. ServiceOwner: {2}", (object) requestContext.ServiceHost.InstanceId, (object) connectionData.InstanceId, (object) connectionData.LocationServiceData.ServiceOwner);
        return (LocationData) null;
      }
      if (connectionData.LocationServiceData.ClientCacheFresh)
      {
        requestContext.Trace(72004, TraceLevel.Error, RemoteLocationDataProvider.s_area, RemoteLocationDataProvider.s_layer, "Client cache should never be fresh on this call.");
        throw new InvalidOperationException("Client cache should never be fresh on this call.");
      }
      AccessMapping defaultAccessMapping1;
      AccessMapping publicAccessMapping1;
      AccessMapping serverAccessMapping1;
      Dictionary<string, AccessMapping> accessMappings = this.GetAccessMappings(requestContext, (IEnumerable<AccessMapping>) connectionData.LocationServiceData.AccessMappings, connectionData.LocationServiceData.DefaultAccessMappingMoniker, connectionData.WebApplicationRelativeDirectory, connectionData.LocationServiceData.ServiceOwner, out defaultAccessMapping1, out publicAccessMapping1, out serverAccessMapping1);
      LocationServiceHelper.PostProcessRemoteDefinitions((IEnumerable<ServiceDefinition>) connectionData.LocationServiceData.ServiceDefinitions, connectionData.LocationServiceData.ServiceOwner);
      AccessMapping defaultAccessMapping2 = defaultAccessMapping1;
      AccessMapping publicAccessMapping2 = publicAccessMapping1;
      AccessMapping serverAccessMapping2 = serverAccessMapping1;
      ICollection<ServiceDefinition> serviceDefinitions = connectionData.LocationServiceData.ServiceDefinitions;
      string relativeDirectory = connectionData.WebApplicationRelativeDirectory;
      DateTime cacheExpirationDate = DateTime.UtcNow.AddSeconds((double) connectionData.LocationServiceData.ClientCacheTimeToLive);
      long lastChangeId64 = connectionData.LocationServiceData.LastChangeId64;
      Guid instanceId = connectionData.InstanceId;
      Guid serviceOwner = connectionData.LocationServiceData.ServiceOwner;
      Guid deploymentId = connectionData.DeploymentId;
      return new LocationData(accessMappings, defaultAccessMapping2, publicAccessMapping2, serverAccessMapping2, (IEnumerable<ServiceDefinition>) serviceDefinitions, relativeDirectory, cacheExpirationDate, lastChangeId64, instanceId, serviceOwner, deploymentId);
    }

    protected internal override LocationData GetInheritedLocationData(
      IVssRequestContext requestContext)
    {
      if (!this.m_useInheritedData)
        return LocationData.Null;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<InheritedLocationDataService>().GetData(vssRequestContext, this.m_deploymentId, TeamFoundationHostTypeHelper.NormalizeHostType(requestContext.ServiceHost.HostType));
    }

    private LocationHttpClient CreateLocationClient(
      IVssRequestContext requestContext,
      bool requiresResourceLocations)
    {
      return LocationServiceHelper.CreateLocationClient(requestContext, this.m_remoteLocationUrl, requiresResourceLocations);
    }

    internal class FaultInMissesMemoryCacheService : VssMemoryCacheService<Guid, bool>
    {
      public FaultInMissesMemoryCacheService()
        : base(TimeSpan.FromSeconds(150.0))
      {
        this.ExpiryInterval.Value = TimeSpan.FromMinutes(5.0);
      }
    }
  }
}
