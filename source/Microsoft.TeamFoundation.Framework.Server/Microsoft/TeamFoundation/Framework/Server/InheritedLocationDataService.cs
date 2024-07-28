// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.InheritedLocationDataService
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

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class InheritedLocationDataService : IInheritedLocationDataService, IVssFrameworkService
  {
    private static readonly ServiceDefinition s_profileDefinition = new ServiceDefinition()
    {
      ServiceType = "LocationService2",
      Identifier = new Guid("8CCFEF3D-2B87-4E99-8CCB-66E343D2DAA8")
    };
    private const string c_disableProfileLocationOverride = "VisualStudio.ProfileService.DisableProfileLocationOverride";
    private ILocationDataCache<InheritedLocationDataService.LocationDataKey> m_cache;
    private static readonly string s_area = "LocationService";
    private static readonly string s_layer = nameof (InheritedLocationDataService);
    private SemaphoreSlim m_loadSemaphore;
    private INotificationRegistration m_locationDataRegistration;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_locationDataRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.LocationDataChanged, new SqlNotificationHandler(this.OnLocationDataChanged), false, false);
      this.m_cache = (ILocationDataCache<InheritedLocationDataService.LocationDataKey>) new InheritedLocationDataService.InheritedLocationDataCache(systemRequestContext);
      this.m_loadSemaphore = new SemaphoreSlim(systemRequestContext.GetService<IVssRegistryService>().GetValue<int>(systemRequestContext, in FrameworkServerConstants.LocationServiceLoadSemaphore, 16));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_locationDataRegistration.Unregister(systemRequestContext);
      if (this.m_loadSemaphore == null)
        return;
      this.m_loadSemaphore.Dispose();
      this.m_loadSemaphore = (SemaphoreSlim) null;
    }

    public LocationData GetData(
      IVssRequestContext requestContext,
      Guid cacheIdentifier,
      TeamFoundationHostType hostType)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.TraceEnter(1885429560, InheritedLocationDataService.s_area, InheritedLocationDataService.s_layer, nameof (GetData));
      try
      {
        InheritedLocationDataService.LocationDataKey cacheKeyIdentifier = hostType == TeamFoundationHostType.Deployment || hostType == TeamFoundationHostType.Application || hostType == TeamFoundationHostType.ProjectCollection || hostType == TeamFoundationHostType.All ? new InheritedLocationDataService.LocationDataKey(cacheIdentifier, hostType) : throw new ArgumentOutOfRangeException(nameof (hostType), string.Format("Cannot provide {0}", (object) hostType));
        LocationData locationData = this.m_cache.GetLocationData(requestContext, cacheKeyIdentifier);
        if (locationData == null || locationData.CacheExpirationDate < DateTime.UtcNow)
        {
          bool flag = this.m_loadSemaphore.Wait(5000, requestContext.CancellationToken);
          try
          {
            Func<IVssRequestContext, InheritedLocationDataService.LocationDataKey, LocationData> loadData = (Func<IVssRequestContext, InheritedLocationDataService.LocationDataKey, LocationData>) ((request, cacheKey) =>
            {
              LocationData data = this.GetInheritedDefinitionsFromStore(request, cacheKey.InstanceId);
              if (data != null)
                data = this.PostProcessLocationData(request, cacheKey, data);
              return data;
            });
            locationData = this.m_cache.GetLocationData(requestContext, cacheKeyIdentifier, loadData);
          }
          finally
          {
            if (flag)
            {
              try
              {
                this.m_loadSemaphore.Release();
              }
              catch (ObjectDisposedException ex)
              {
              }
            }
            else
              requestContext.Trace(71550337, TraceLevel.Error, InheritedLocationDataService.s_area, InheritedLocationDataService.s_layer, "Waited more than 5 seconds to acquire the load semaphore.");
          }
        }
        return locationData;
      }
      finally
      {
        requestContext.TraceLeave(593335244, InheritedLocationDataService.s_area, InheritedLocationDataService.s_layer, nameof (GetData));
      }
    }

    private LocationData PostProcessLocationData(
      IVssRequestContext requestContext,
      InheritedLocationDataService.LocationDataKey cacheIdentifier,
      LocationData data)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<LocationData>(data, nameof (data));
      ArgumentUtility.CheckForNonPositiveInt((int) cacheIdentifier.HostType, nameof (cacheIdentifier));
      TeamFoundationHostType hostType = cacheIdentifier.HostType;
      if (hostType == TeamFoundationHostType.Deployment && requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        hostType |= TeamFoundationHostType.Application;
      HashSet<ServiceDefinition> source = new HashSet<ServiceDefinition>(data.GetAllServiceDefinitions().Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (o => ((uint) o.InheritLevel & (uint) hostType) > 0U)), (IEqualityComparer<ServiceDefinition>) ServiceDefinitionComparer.Instance);
      if (!requestContext.IsFeatureEnabled("VisualStudio.ProfileService.DisableProfileLocationOverride") && cacheIdentifier.HostType == TeamFoundationHostType.ProjectCollection && requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
      {
        ServiceDefinition serviceDefinition = source.FirstOrDefault<ServiceDefinition>((Func<ServiceDefinition, bool>) (o => ServiceDefinitionComparer.Instance.Equals(o, InheritedLocationDataService.s_profileDefinition)));
        if (serviceDefinition == null)
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          serviceDefinition = vssRequestContext.GetService<IInternalLocationService>().FindNonInheritedDefinitions(vssRequestContext).FirstOrDefault<ServiceDefinition>((Func<ServiceDefinition, bool>) (o => ServiceDefinitionComparer.Instance.Equals(o, InheritedLocationDataService.s_profileDefinition)));
          if (serviceDefinition != null)
          {
            serviceDefinition = serviceDefinition.Clone();
            serviceDefinition.InheritLevel = InheritLevel.All;
            source.Add(serviceDefinition);
          }
        }
        if (serviceDefinition != null)
        {
          serviceDefinition.RelativeToSetting = RelativeToSetting.Context;
          serviceDefinition.RelativePath = "/";
          serviceDefinition.ParentIdentifier = Guid.Empty;
        }
      }
      return new LocationData((Dictionary<string, AccessMapping>) null, (AccessMapping) null, (AccessMapping) null, (AccessMapping) null, (IEnumerable<ServiceDefinition>) source, (string) null, data.CacheExpirationDate, data.LastChangeId, data.HostId, data.InstanceType, data.DeploymentId);
    }

    internal virtual LocationData GetInheritedDefinitionsFromStore(
      IVssRequestContext requestContext,
      Guid cacheIdentifier)
    {
      List<ServiceDefinition> serviceDefinitions = (List<ServiceDefinition>) null;
      List<LocationMappingData> locationMappings = (List<LocationMappingData>) null;
      long databaseLastChangeId = 0;
      DateTime cacheExpirationDate = DateTime.MaxValue;
      Guid instanceId;
      Guid serviceOwner;
      Guid deploymentId;
      if (cacheIdentifier == Guid.Empty || cacheIdentifier == LocationServiceConstants.SelfReferenceIdentifier || cacheIdentifier == requestContext.ServiceInstanceType())
      {
        LocalLocationDataProvider.LoadLocationDataFromDatabase(requestContext, out serviceDefinitions, out List<AccessMapping> _, out locationMappings, out string _, out databaseLastChangeId);
        instanceId = requestContext.ServiceHost.InstanceId;
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        serviceOwner = vssRequestContext.GetService<IVssRegistryService>().GetValue<Guid>(vssRequestContext, (RegistryQuery) ConfigurationConstants.InstanceType, false, new Guid());
        deploymentId = requestContext.ServiceHost.DeploymentServiceHost.InstanceId;
      }
      else
      {
        string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, cacheIdentifier, AccessMappingConstants.PublicAccessMappingMoniker);
        requestContext.Trace(1722918557, TraceLevel.Info, InheritedLocationDataService.s_area, InheritedLocationDataService.s_layer, "Got url {0} for instance {1}", (object) locationServiceUrl, (object) cacheIdentifier);
        ConnectionData connectionData = LocationServiceHelper.CreateLocationClient(requestContext.Elevate(), locationServiceUrl, false).GetConnectionDataAsync(ConnectOptions.IncludeServices | ConnectOptions.IncludeInheritedDefinitionsOnly, 0L).SyncResult<ConnectionData>();
        databaseLastChangeId = connectionData.LocationServiceData.LastChangeId64;
        serviceDefinitions = connectionData.LocationServiceData.ServiceDefinitions.ToList<ServiceDefinition>();
        LocationServiceHelper.PostProcessRemoteDefinitions((IEnumerable<ServiceDefinition>) serviceDefinitions, connectionData.LocationServiceData.ServiceOwner);
        cacheExpirationDate = DateTime.UtcNow.AddSeconds((double) connectionData.LocationServiceData.ClientCacheTimeToLive);
        instanceId = connectionData.InstanceId;
        serviceOwner = connectionData.LocationServiceData.ServiceOwner;
        deploymentId = connectionData.DeploymentId;
      }
      serviceDefinitions = serviceDefinitions.Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (x => x.InheritLevel != 0)).ToList<ServiceDefinition>();
      if (locationMappings != null)
        LocationServiceHelper.SetLocationMappings(serviceDefinitions, locationMappings);
      return new LocationData((Dictionary<string, AccessMapping>) null, (AccessMapping) null, (AccessMapping) null, (AccessMapping) null, (IEnumerable<ServiceDefinition>) serviceDefinitions, (string) null, cacheExpirationDate, databaseLastChangeId, instanceId, serviceOwner, deploymentId);
    }

    internal void OnLocationDataChanged(IVssRequestContext requestContext, LocationDataKind kind) => this.m_cache.Invalidate(requestContext, kind, false);

    private void OnLocationDataChanged(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      requestContext.TraceEnter(569553073, InheritedLocationDataService.s_area, InheritedLocationDataService.s_layer, nameof (OnLocationDataChanged));
      try
      {
        LocationDataKind kind = LocationService.FromEventData(args.Data);
        this.OnLocationDataChanged(requestContext, kind);
      }
      finally
      {
        requestContext.TraceLeave(1597886472, InheritedLocationDataService.s_area, InheritedLocationDataService.s_layer, nameof (OnLocationDataChanged));
      }
    }

    private class LocationDataKey
    {
      public readonly Guid InstanceId;
      public readonly TeamFoundationHostType HostType;

      public LocationDataKey(Guid instanceId, TeamFoundationHostType hostType)
      {
        this.InstanceId = instanceId;
        this.HostType = hostType;
      }

      public override string ToString() => string.Format("{0}:{1}", (object) this.HostType, (object) this.InstanceId);
    }

    private class LocationDataKeyComparer : 
      IEqualityComparer<InheritedLocationDataService.LocationDataKey>
    {
      public bool Equals(
        InheritedLocationDataService.LocationDataKey x,
        InheritedLocationDataService.LocationDataKey y)
      {
        return x.InstanceId.Equals(y.InstanceId) && x.HostType.Equals((object) y.HostType);
      }

      public int GetHashCode(InheritedLocationDataService.LocationDataKey obj) => obj.InstanceId.GetHashCode() + (int) (byte) obj.HostType;
    }

    private class InheritedLocationDataCache : 
      LocationDataCache<InheritedLocationDataService.LocationDataKey>
    {
      public InheritedLocationDataCache(IVssRequestContext requestContext)
        : base(requestContext, comparer: (IEqualityComparer<InheritedLocationDataService.LocationDataKey>) new InheritedLocationDataService.LocationDataKeyComparer())
      {
      }

      public override bool CanUseRedis(LocationDataKind kind) => false;

      public override LocationDataKind GetKind(InheritedLocationDataService.LocationDataKey key) => !(key.InstanceId == Guid.Empty) ? LocationDataKind.Remote : LocationDataKind.Local;
    }
  }
}
