// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Internal.InstanceManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Client;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.Internal
{
  internal class InstanceManagementService : 
    IInternalInstanceManagementService,
    IInstanceManagementService,
    IVssFrameworkService
  {
    private List<string> m_registeredServiceDomains;
    private DateTime m_lastCacheFlush;
    private static readonly TimeSpan s_cacheFlushInterval = TimeSpan.FromMinutes(10.0);
    internal const string Area = "InstanceManagementService";
    internal const string Layer = "IVssFrameworkService";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.CheckHostedDeployment();
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), ConfigurationConstants.RegisteredServiceDomains);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    public void RegisterServiceInstanceType(
      IVssRequestContext requestContext,
      ServiceInstanceType instanceTypeToRegister)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<ServiceInstanceType>(instanceTypeToRegister, "instanceType");
      if (instanceTypeToRegister.InstanceType == ServiceInstanceTypes.SPS)
        return;
      InstanceManagementHelper.ExecuteForAllSpsInstances(requestContext, (Action<LocationHttpClient>) (client => InstanceManagementHelper.SetServiceInstanceType(requestContext, client, instanceTypeToRegister, false)));
      this.FlushLocationServiceData(requestContext, Guid.Empty);
    }

    public void UnregisterServiceInstanceType(
      IVssRequestContext requestContext,
      ServiceInstanceType instanceTypeToUnregister)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<ServiceInstanceType>(instanceTypeToUnregister, nameof (instanceTypeToUnregister));
      if (instanceTypeToUnregister.InstanceType == ServiceInstanceTypes.SPS)
        return;
      InstanceManagementHelper.ExecuteForAllSpsInstances(requestContext, (Action<LocationHttpClient>) (client => InstanceManagementHelper.RemoveServiceInstanceType(requestContext, client, instanceTypeToUnregister)));
      this.FlushLocationServiceData(requestContext, Guid.Empty);
    }

    public void UpdateServiceInstanceType(
      IVssRequestContext requestContext,
      ServiceInstanceType instanceTypeToUpdate)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<ServiceInstanceType>(instanceTypeToUpdate, "instanceType");
      if (instanceTypeToUpdate.InstanceType == ServiceInstanceTypes.SPS)
        return;
      InstanceManagementHelper.ExecuteForAllSpsInstances(requestContext, (Action<LocationHttpClient>) (client => InstanceManagementHelper.SetServiceInstanceType(requestContext, client, instanceTypeToUpdate, true)));
      this.FlushLocationServiceData(requestContext, Guid.Empty);
    }

    public ServiceInstanceType GetServiceInstanceType(
      IVssRequestContext requestContext,
      Guid instanceType)
    {
      ArgumentUtility.CheckForEmptyGuid(instanceType, nameof (instanceType));
      return this.GetServiceInstanceTypesInternal(requestContext, new Guid?(instanceType)).FirstOrDefault<ServiceInstanceType>();
    }

    public IList<ServiceInstanceType> GetServiceInstanceTypes(IVssRequestContext requestContext) => this.GetServiceInstanceTypesInternal(requestContext);

    private IList<ServiceInstanceType> GetServiceInstanceTypesInternal(
      IVssRequestContext requestContext,
      Guid? instanceType = null)
    {
      requestContext.CheckDeploymentRequestContext();
      ILocationDataProvider locationData = requestContext.GetService<ILocationService>().GetLocationData(requestContext, ServiceInstanceTypes.SPS);
      List<ServiceInstanceType> instanceTypesInternal = new List<ServiceInstanceType>();
      if (instanceType.HasValue)
      {
        ServiceDefinition serviceDefinition = locationData.FindServiceDefinition(requestContext, "VsService", instanceType.Value);
        if (serviceDefinition != null)
          instanceTypesInternal.Add(new ServiceInstanceType(serviceDefinition));
      }
      else
      {
        foreach (ServiceDefinition serviceDefinition in locationData.FindServiceDefinitions(requestContext, "VsService"))
          instanceTypesInternal.Add(new ServiceInstanceType(serviceDefinition));
      }
      return (IList<ServiceInstanceType>) instanceTypesInternal;
    }

    public void RegisterServiceInstance(
      IVssRequestContext requestContext,
      ServiceInstance instanceToRegister)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<ServiceInstance>(instanceToRegister, nameof (instanceToRegister));
      ArgumentUtility.CheckForNull<Uri>(instanceToRegister.PublicUri, "instanceToRegister.PublicUri");
      ArgumentUtility.CheckForNull<Uri>(instanceToRegister.AzureUri, "instanceToRegister.AzureUri");
      if (instanceToRegister.InstanceType == ServiceInstanceTypes.SPS)
        return;
      InstanceManagementHelper.ExecuteForAllSpsInstances(requestContext, (Action<LocationHttpClient>) (client => InstanceManagementHelper.SetServiceInstance(requestContext, client, instanceToRegister, false)));
      this.FlushLocationServiceData(requestContext, Guid.Empty);
    }

    public void UnregisterServiceInstance(
      IVssRequestContext requestContext,
      ServiceInstance instanceToUnregister)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<ServiceInstance>(instanceToUnregister, nameof (instanceToUnregister));
      if (instanceToUnregister.InstanceType == ServiceInstanceTypes.SPS)
        return;
      InstanceManagementHelper.ExecuteForAllSpsInstances(requestContext, (Action<LocationHttpClient>) (client => InstanceManagementHelper.RemoveServiceInstance(requestContext, client, instanceToUnregister)));
      this.FlushLocationServiceData(requestContext, Guid.Empty);
    }

    public void UpdateServiceInstance(
      IVssRequestContext requestContext,
      ServiceInstance instanceToUpdate)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<ServiceInstance>(instanceToUpdate, nameof (instanceToUpdate));
      if (instanceToUpdate.InstanceType == ServiceInstanceTypes.SPS)
        return;
      InstanceManagementHelper.ExecuteForAllSpsInstances(requestContext, (Action<LocationHttpClient>) (client => InstanceManagementHelper.SetServiceInstance(requestContext, client, instanceToUpdate, true)));
      this.FlushLocationServiceData(requestContext, Guid.Empty);
    }

    public ServiceInstance GetServiceInstance(IVssRequestContext requestContext, Guid instanceId)
    {
      ArgumentUtility.CheckForEmptyGuid(instanceId, nameof (instanceId));
      return this.GetServiceInstancesInternal(requestContext, new Guid?(instanceId)).FirstOrDefault<ServiceInstance>();
    }

    public IList<ServiceInstance> GetServiceInstances(
      IVssRequestContext requestContext,
      Guid instanceType = default (Guid))
    {
      return this.GetServiceInstancesInternal(requestContext, instanceType: new Guid?(instanceType));
    }

    public IList<ServiceInstance> GetServiceInstancesForRegion(
      IVssRequestContext requestContext,
      string region,
      Guid instanceType = default (Guid))
    {
      ArgumentUtility.CheckStringForNullOrEmpty(region, nameof (region));
      return (IList<ServiceInstance>) this.GetServiceInstances(requestContext, instanceType).Where<ServiceInstance>((Func<ServiceInstance, bool>) (serviceInstance => serviceInstance.IsRegionSupported(region))).ToList<ServiceInstance>();
    }

    private IList<ServiceInstance> GetServiceInstancesInternal(
      IVssRequestContext requestContext,
      Guid? instanceId = null,
      Guid? instanceType = null)
    {
      requestContext.CheckDeploymentRequestContext();
      ILocationDataProvider locationData = requestContext.GetService<ILocationService>().GetLocationData(requestContext, ServiceInstanceTypes.SPS);
      List<ServiceInstance> instancesInternal = new List<ServiceInstance>();
      if (instanceId.HasValue)
      {
        ServiceDefinition serviceDefinition = locationData.FindServiceDefinition(requestContext, "LocationService2", instanceId.Value);
        if (serviceDefinition != null && InstanceManagementHelper.IsServiceInstanceDefinition(serviceDefinition))
          instancesInternal.Add(new ServiceInstance(serviceDefinition));
      }
      else
      {
        foreach (ServiceDefinition serviceDefinition in locationData.FindServiceDefinitions(requestContext, "LocationService2"))
        {
          if (InstanceManagementHelper.IsServiceInstanceDefinition(serviceDefinition))
          {
            Guid? nullable = instanceType;
            Guid empty = Guid.Empty;
            if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
            {
              Guid parentIdentifier = serviceDefinition.ParentIdentifier;
              nullable = instanceType;
              if ((nullable.HasValue ? (parentIdentifier == nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
                continue;
            }
            instancesInternal.Add(new ServiceInstance(serviceDefinition));
          }
        }
      }
      return (IList<ServiceInstance>) instancesInternal;
    }

    public void SetHostInstanceMapping(
      IVssRequestContext requestContext,
      Guid hostId,
      ServiceInstance serviceInstance = null)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      if (serviceInstance == null)
        serviceInstance = new ServiceInstance()
        {
          InstanceId = requestContext.ServiceInstanceId(),
          InstanceType = requestContext.ServiceInstanceType()
        };
      if (serviceInstance.InstanceType == ServiceInstanceTypes.SPS)
        return;
      ServiceDefinition serviceDefinition = new HostInstanceMapping()
      {
        HostId = hostId,
        ServiceInstance = serviceInstance,
        Status = ServiceStatus.Active
      }.ToServiceDefinition();
      serviceDefinition.LocationMappings.Clear();
      IVssRequestContext hostContext;
      if (this.TryCreateHostRequestContext(requestContext, hostId, out hostContext))
      {
        using (hostContext)
          hostContext.GetService<ILocationService>().GetLocationData(hostContext, ServiceInstanceTypes.SPS).SaveServiceDefinitions(hostContext, (IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
          {
            serviceDefinition
          });
      }
      else
      {
        LocationServiceHelper.GetSpsLocationClient(requestContext, hostId).UpdateServiceDefinitionsAsync((IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
        {
          serviceDefinition
        }).SyncResult();
        this.FlushLocationServiceData(requestContext, hostId);
      }
    }

    public void UpdateHostInstanceMappingStatus(
      IVssRequestContext requestContext,
      Guid hostId,
      ServiceStatus status,
      ServiceInstance serviceInstance = null)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      if (serviceInstance == null)
        serviceInstance = new ServiceInstance()
        {
          InstanceId = requestContext.ServiceInstanceId(),
          InstanceType = requestContext.ServiceInstanceType()
        };
      if (serviceInstance.InstanceType == ServiceInstanceTypes.SPS)
        return;
      ServiceDefinition serviceDefinition = new HostInstanceMapping()
      {
        HostId = hostId,
        ServiceInstance = serviceInstance,
        Status = status
      }.ToServiceDefinition();
      serviceDefinition.LocationMappings.Clear();
      LocationServiceHelper.GetSpsLocationClient(requestContext, hostId).UpdateServiceDefinitionsAsync((IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
      {
        serviceDefinition
      }).SyncResult();
      this.FlushLocationServiceData(requestContext, hostId);
    }

    public void SetHostInstanceMappings(
      IVssRequestContext requestContext,
      IEnumerable<HostInstanceMapping> instanceMappings)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<IEnumerable<HostInstanceMapping>>(instanceMappings, nameof (instanceMappings));
      foreach (IGrouping<Guid, HostInstanceMapping> source in instanceMappings.GroupBy<HostInstanceMapping, Guid>((Func<HostInstanceMapping, Guid>) (instanceMapping => instanceMapping.HostId)))
      {
        IVssRequestContext hostContext = (IVssRequestContext) null;
        if (!this.TryCreateHostRequestContext(requestContext, source.Key, out hostContext))
          throw new InvalidOperationException();
        using (hostContext)
        {
          ILocationDataProvider locationData = hostContext.GetService<ILocationService>().GetLocationData(hostContext, ServiceInstanceTypes.SPS);
          IEnumerable<ServiceDefinition> serviceDefinitions1 = source.Select<HostInstanceMapping, ServiceDefinition>((Func<HostInstanceMapping, ServiceDefinition>) (mapping => mapping.ToServiceDefinition()));
          IVssRequestContext requestContext1 = hostContext;
          IEnumerable<ServiceDefinition> serviceDefinitions2 = serviceDefinitions1;
          locationData.SaveServiceDefinitions(requestContext1, serviceDefinitions2);
        }
      }
    }

    public void RemoveHostInstanceMapping(
      IVssRequestContext requestContext,
      Guid hostId,
      ServiceInstance serviceInstance,
      bool overrideInstanceCheck)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      if (serviceInstance == null)
        serviceInstance = new ServiceInstance()
        {
          InstanceId = requestContext.ServiceInstanceId(),
          InstanceType = requestContext.ServiceInstanceType()
        };
      if (serviceInstance.InstanceType == ServiceInstanceTypes.SPS)
        return;
      LocationHttpClient spsLocationClient = LocationServiceHelper.GetSpsLocationClient(requestContext, hostId);
      Guid instanceType = serviceInstance.InstanceType;
      ServiceDefinition serviceDefinition = spsLocationClient.GetServiceDefinitionAsync("LocationService2", instanceType, false, false).SyncResult<ServiceDefinition>();
      if (serviceDefinition == null || serviceDefinition.ParentIdentifier != requestContext.ServiceInstanceId() && !overrideInstanceCheck)
        return;
      TaskExtensions.SyncResult(spsLocationClient.DeleteServiceDefinitionAsync("LocationService2", instanceType));
      this.FlushLocationServiceData(requestContext, hostId);
    }

    public void RemoveHostInstanceMappings(
      IVssRequestContext requestContext,
      IEnumerable<HostInstanceMapping> instanceMappings)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<IEnumerable<HostInstanceMapping>>(instanceMappings, nameof (instanceMappings));
      foreach (IGrouping<Guid, HostInstanceMapping> source in instanceMappings.GroupBy<HostInstanceMapping, Guid>((Func<HostInstanceMapping, Guid>) (instanceMapping => instanceMapping.HostId)))
      {
        IVssRequestContext hostContext = (IVssRequestContext) null;
        if (!this.TryCreateHostRequestContext(requestContext, source.Key, out hostContext))
          throw new InvalidOperationException();
        using (hostContext)
        {
          ILocationDataProvider locationData = hostContext.GetService<ILocationService>().GetLocationData(hostContext, ServiceInstanceTypes.SPS);
          IEnumerable<ServiceDefinition> serviceDefinitions1 = source.Select<HostInstanceMapping, ServiceDefinition>((Func<HostInstanceMapping, ServiceDefinition>) (mapping => mapping.ToServiceDefinition()));
          IVssRequestContext requestContext1 = hostContext;
          IEnumerable<ServiceDefinition> serviceDefinitions2 = serviceDefinitions1;
          locationData.RemoveServiceDefinitions(requestContext1, serviceDefinitions2);
        }
      }
    }

    public IList<HostInstanceMapping> GetHostInstanceMappings(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      return this.GetHostInstanceMappingsInternal(requestContext, hostId);
    }

    public HostInstanceMapping GetHostInstanceMapping(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid instanceType = default (Guid),
      bool previewFaultIn = false)
    {
      if (instanceType == Guid.Empty)
        instanceType = requestContext.ServiceInstanceType();
      return this.GetHostInstanceMappingsInternal(requestContext, hostId, new Guid?(instanceType), previewFaultIn: previewFaultIn).FirstOrDefault<HostInstanceMapping>();
    }

    HostInstanceMapping IInternalInstanceManagementService.GetHostInstanceMappingFromSps(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid instanceType)
    {
      if (instanceType == Guid.Empty)
        instanceType = requestContext.ServiceInstanceType();
      bool useSps = requestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS;
      return this.GetHostInstanceMappingsInternal(requestContext, hostId, new Guid?(instanceType), useSps).FirstOrDefault<HostInstanceMapping>();
    }

    private IList<HostInstanceMapping> GetHostInstanceMappingsInternal(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid? instanceType = null,
      bool useSps = false,
      bool previewFaultIn = false)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      IEnumerable<ServiceDefinition> serviceDefinitions = (IEnumerable<ServiceDefinition>) null;
      IVssRequestContext hostContext = (IVssRequestContext) null;
      if (!useSps && this.TryCreateHostRequestContext(requestContext, hostId, out hostContext))
      {
        using (hostContext)
        {
          ILocationDataProvider locationData = hostContext.GetService<ILocationService>().GetLocationData(hostContext, ServiceInstanceTypes.SPS);
          if (!instanceType.HasValue)
            serviceDefinitions = locationData.FindServiceDefinitions(hostContext, "LocationService2");
          else
            serviceDefinitions = (IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
            {
              locationData.FindServiceDefinitionWithFaultIn(hostContext, "LocationService2", instanceType.Value, previewFaultIn)
            };
        }
      }
      else if (requestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS)
      {
        string locationServiceUrl = LocationServiceHelper.GetRootLocationServiceUrl(requestContext, hostId, false);
        if (locationServiceUrl != null)
        {
          LocationHttpClient locationClient = LocationServiceHelper.CreateLocationClient(requestContext, locationServiceUrl);
          if (!instanceType.HasValue)
            serviceDefinitions = locationClient.GetServiceDefinitionsAsync("LocationService2").SyncResult<IEnumerable<ServiceDefinition>>();
          else
            serviceDefinitions = (IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
            {
              locationClient.GetServiceDefinitionAsync("LocationService2", instanceType.Value).SyncResult<ServiceDefinition>()
            };
        }
      }
      List<HostInstanceMapping> mappingsInternal = new List<HostInstanceMapping>();
      if (serviceDefinitions != null)
      {
        foreach (ServiceDefinition instanceMapping in serviceDefinitions)
        {
          if (instanceMapping != null && instanceMapping.ParentIdentifier != Guid.Empty)
          {
            ServiceInstance serviceInstance = this.GetServiceInstance(requestContext, instanceMapping.ParentIdentifier);
            if (serviceInstance == null && InstanceManagementHelper.IsValidServiceInstanceType(instanceMapping.Identifier))
            {
              if (this.m_lastCacheFlush + InstanceManagementService.s_cacheFlushInterval < DateTime.UtcNow)
              {
                requestContext.Trace(959475, TraceLevel.Info, nameof (InstanceManagementService), "IVssFrameworkService", "Flushing the location service cache due to missing service instance definition.");
                this.FlushLocationServiceData(requestContext, Guid.Empty);
                this.m_lastCacheFlush = DateTime.UtcNow;
                serviceInstance = this.GetServiceInstance(requestContext, instanceMapping.ParentIdentifier);
                if (serviceInstance == null)
                  requestContext.Trace(607901, TraceLevel.Error, nameof (InstanceManagementService), "IVssFrameworkService", "We flushed the cache and still cannot resolve the service instance!");
              }
              else
                requestContext.Trace(476344, TraceLevel.Warning, nameof (InstanceManagementService), "IVssFrameworkService", "We are still cache missing a service instance but have flushed too frequently!");
            }
            if (serviceInstance != null)
              mappingsInternal.Add(new HostInstanceMapping(hostId, instanceMapping, serviceInstance));
          }
        }
      }
      return (IList<HostInstanceMapping>) mappingsInternal;
    }

    public IList<string> GetRegisteredServiceDomains(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      if (this.m_registeredServiceDomains == null)
      {
        string str = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) ConfigurationConstants.RegisteredServiceDomains, true, "visualstudio.com");
        if (!string.IsNullOrEmpty(str))
        {
          string[] collection = str.Split(new char[1]{ ';' }, StringSplitOptions.RemoveEmptyEntries);
          if (collection != null && collection.Length != 0)
          {
            List<string> stringList = new List<string>(collection.Length);
            stringList.AddRange((IEnumerable<string>) collection);
            this.m_registeredServiceDomains = stringList;
          }
        }
      }
      return (IList<string>) this.m_registeredServiceDomains;
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.m_registeredServiceDomains = (List<string>) null;
    }

    public virtual void FlushLocationServiceData(IVssRequestContext requestContext, Guid hostId)
    {
      if (hostId == Guid.Empty)
      {
        requestContext.GetService<IInternalLocationService>().OnLocationDataChanged(requestContext, LocationDataKind.All);
      }
      else
      {
        IVssRequestContext hostContext;
        if (!this.TryCreateHostRequestContext(requestContext, hostId, out hostContext))
          return;
        using (hostContext)
          hostContext.GetService<IInternalLocationService>().OnLocationDataChanged(hostContext, LocationDataKind.All);
      }
    }

    internal bool TryCreateHostRequestContext(
      IVssRequestContext requestContext,
      Guid hostId,
      out IVssRequestContext hostContext)
    {
      ITeamFoundationHostManagementService service1 = requestContext.GetService<ITeamFoundationHostManagementService>();
      IInternalHostSyncService service2 = requestContext.GetService<IInternalHostSyncService>();
      if (requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS && service1.QueryServiceHostPropertiesCached(requestContext, hostId) != null || service2.LocalHostExistsAndWellFormed(requestContext, hostId, out TeamFoundationServiceHostProperties _))
      {
        hostContext = service1.BeginRequest(requestContext, hostId, RequestContextType.ServicingContext, throwIfShutdown: false);
        return true;
      }
      hostContext = (IVssRequestContext) null;
      return false;
    }
  }
}
