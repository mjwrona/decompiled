// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.ResourceAreaService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  public class ResourceAreaService : IResourceAreaService, IVssFrameworkService
  {
    private static readonly ResourceArea s_spsResourceArea = new ResourceArea()
    {
      AreaId = ServiceInstanceTypes.SPS,
      AreaName = "SPS Location Service",
      ParentService = ServiceInstanceTypes.SPS
    };
    private static readonly string s_area = "ResourceAreas";
    private static readonly string s_layer = "IVssFrameworkService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<ResourceArea> GetResourceAreas(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetLocationData(requestContext, ServiceInstanceTypes.SPS).FindServiceDefinitions(requestContext, "LocationService2").Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (sd => sd.IsResourceArea())).Select<ServiceDefinition, ResourceArea>((Func<ServiceDefinition, ResourceArea>) (sd => sd.ToResourceArea()));

    public ResourceArea GetResourceArea(IVssRequestContext requestContext, Guid areaId) => this.GetResourceArea(requestContext, areaId, false);

    public ResourceArea GetResourceArea(
      IVssRequestContext requestContext,
      Guid areaId,
      bool previewFaultIn)
    {
      ArgumentUtility.CheckForEmptyGuid(areaId, nameof (areaId));
      ServiceDefinition definitionWithFaultIn = requestContext.GetService<ILocationService>().GetLocationData(requestContext, ServiceInstanceTypes.SPS).FindServiceDefinitionWithFaultIn(requestContext, "LocationService2", areaId, previewFaultIn);
      return definitionWithFaultIn != null && definitionWithFaultIn.IsResourceArea() ? definitionWithFaultIn.ToResourceArea() : (ResourceArea) null;
    }

    public IEnumerable<ResourceArea> GetInheritedResourceAreas(
      IVssRequestContext requestContext,
      TeamFoundationHostType hostType)
    {
      requestContext.CheckDeploymentRequestContext();
      LocationData data = requestContext.GetService<IInheritedLocationDataService>().GetData(requestContext, ServiceInstanceTypes.SPS, TeamFoundationHostTypeHelper.NormalizeHostType(hostType));
      IEnumerable<ServiceInstanceType> serviceInstanceTypes = (IEnumerable<ServiceInstanceType>) requestContext.GetService<IInstanceManagementService>().GetServiceInstanceTypes(requestContext);
      return data.FindServiceDefinitionsByType("LocationService2").Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (sd => sd.IsResourceArea())).Select<ServiceDefinition, ResourceArea>((Func<ServiceDefinition, ResourceArea>) (sd => sd.ToResourceArea())).Concat<ResourceArea>(serviceInstanceTypes.Select<ServiceInstanceType, ResourceArea>((Func<ServiceInstanceType, ResourceArea>) (x => x.ToResourceArea()))).Concat<ResourceArea>((IEnumerable<ResourceArea>) new ResourceArea[1]
      {
        ResourceAreaService.s_spsResourceArea.Clone()
      });
    }

    public ResourceArea GetInheritedResourceArea(
      IVssRequestContext requestContext,
      Guid areaId,
      TeamFoundationHostType hostType)
    {
      ArgumentUtility.CheckForEmptyGuid(areaId, nameof (areaId));
      requestContext.CheckDeploymentRequestContext();
      ServiceDefinition serviceDefinition = requestContext.GetService<IInheritedLocationDataService>().GetData(requestContext, ServiceInstanceTypes.SPS, TeamFoundationHostTypeHelper.NormalizeHostType(hostType)).FindServiceDefinition("LocationService2", areaId);
      if (serviceDefinition != null && serviceDefinition.IsResourceArea())
        return serviceDefinition.ToResourceArea();
      if (areaId == ServiceInstanceTypes.SPS)
        return ResourceAreaService.s_spsResourceArea.Clone();
      ServiceInstanceType serviceInstanceType = requestContext.GetService<IInstanceManagementService>().GetServiceInstanceType(requestContext, areaId);
      return serviceInstanceType != null ? serviceInstanceType.ToResourceArea() : (ResourceArea) null;
    }

    public void RegisterResourceAreas(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      bool registerWithSps = true)
    {
      this.RegisterResourceAreas(requestContext, areas, Guid.Empty, registerWithSps);
    }

    public void RegisterResourceAreas(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      Guid parentIdentifier,
      bool registerWithSps = true)
    {
      if (areas.Count <= 0)
        return;
      requestContext.GetService<ILocationService>().SaveServiceDefinitions(requestContext, areas.Select<ResourceArea, ServiceDefinition>((Func<ResourceArea, ServiceDefinition>) (area => ResourceAreaService.Convert(area, parentIdentifier))));
      if (!registerWithSps)
        return;
      this.RegisterResourceAreasWithSps(requestContext, areas);
    }

    internal void MarkResourceAreaForMove(IVssRequestContext requestContext, Guid areaId)
    {
      ILocationDataProvider locationData = requestContext.GetService<ILocationService>().GetLocationData(requestContext, ServiceInstanceTypes.SPS);
      ServiceDefinition serviceDefinition = locationData.FindServiceDefinition(requestContext, "LocationService2", areaId);
      if (serviceDefinition.Status == ServiceStatus.Moving)
        return;
      serviceDefinition.Status = ServiceStatus.Moving;
      serviceDefinition.Properties.Clear();
      serviceDefinition.LocationMappings.Clear();
      serviceDefinition.InheritLevel = InheritLevel.All;
      locationData.SaveServiceDefinitions(requestContext, (IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
      {
        serviceDefinition
      });
    }

    internal void RemoveResourceArea(IVssRequestContext requestContext, Guid areaId)
    {
      ServiceDefinition serviceDefinition1 = ResourceAreaService.Convert(new ResourceArea()
      {
        AreaId = areaId,
        AreaName = areaId.ToString("D")
      }, Guid.Empty);
      requestContext.GetService<ILocationService>().RemoveServiceDefinitions(requestContext, (IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
      {
        serviceDefinition1
      });
      if (!(requestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS))
        return;
      try
      {
        LocationHttpClient client = requestContext.GetClient<LocationHttpClient>(ServiceInstanceTypes.SPS);
        ServiceDefinition serviceDefinition2 = client.GetServiceDefinitionAsync(serviceDefinition1.ServiceType, serviceDefinition1.Identifier).SyncResult<ServiceDefinition>();
        if (serviceDefinition2 == null || !(serviceDefinition2.ParentIdentifier == requestContext.ServiceInstanceType()))
          return;
        TaskExtensions.SyncResult(client.DeleteServiceDefinitionAsync(serviceDefinition1.ServiceType, serviceDefinition1.Identifier));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(72301, ResourceAreaService.s_area, ResourceAreaService.s_layer, ex);
      }
    }

    private void RegisterResourceAreasWithSps(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas)
    {
      Guid serviceInstanceType = requestContext.ServiceInstanceType();
      if (!InstanceManagementHelper.RequiresSharedService(serviceInstanceType) || areas.Count <= 0)
        return;
      InstanceManagementHelper.ExecuteForAllSpsInstances(requestContext, (Action<LocationHttpClient>) (locationClient => locationClient.UpdateServiceDefinitionsAsync(areas.Select<ResourceArea, ServiceDefinition>((Func<ResourceArea, ServiceDefinition>) (x => ResourceAreaService.Convert(x, serviceInstanceType)))).SyncResult()));
    }

    internal static ServiceDefinition Convert(ResourceArea area, Guid parentService)
    {
      if (parentService == Guid.Empty)
      {
        ServiceDefinition serviceDefinition = new ServiceDefinition("LocationService2", area.AreaId, area.AreaName, "/", RelativeToSetting.Context, "Resource Area", "Framework");
        serviceDefinition.InheritLevel = InheritLevel.All;
        serviceDefinition.SetProperty("Microsoft.TeamFoundation.Location.AreaName", (object) area.AreaName);
        return serviceDefinition;
      }
      return new ServiceDefinition("LocationService2", area.AreaId, area.AreaName, (string) null, RelativeToSetting.FullyQualified, "Resource Area", "Framework")
      {
        ParentServiceType = "LocationService2",
        ParentIdentifier = parentService,
        InheritLevel = InheritLevel.All
      };
    }
  }
}
