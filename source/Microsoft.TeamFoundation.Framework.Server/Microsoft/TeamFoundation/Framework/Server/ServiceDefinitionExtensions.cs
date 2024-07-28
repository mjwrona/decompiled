// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceDefinitionExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class ServiceDefinitionExtensions
  {
    public static bool IsResourceLocation(this ServiceDefinition definition) => definition.ResourceVersion > 0;

    public static bool IsResourceArea(this ServiceDefinition definition) => VssStringComparer.ServiceType.Equals(definition.ServiceType, "LocationService2") && StringComparer.OrdinalIgnoreCase.Equals(definition.Description, "Resource Area");

    public static ResourceArea ToResourceArea(this ServiceDefinition serviceDefinition)
    {
      if (serviceDefinition == null)
        return (ResourceArea) null;
      return new ResourceArea()
      {
        AreaId = serviceDefinition.Identifier,
        AreaName = serviceDefinition.DisplayName,
        ParentService = ServiceDefinitionExtensions.NormalizeParentService(serviceDefinition.ParentIdentifier, ServiceInstanceTypes.SPS)
      };
    }

    public static ResourceArea ToResourceArea(this ServiceInstanceType serviceType)
    {
      if (serviceType == null)
        return (ResourceArea) null;
      return new ResourceArea()
      {
        AreaId = serviceType.InstanceType,
        AreaName = "Location Service",
        ParentService = serviceType.InstanceType
      };
    }

    public static ResourceAreaInfo ToResourceAreaInfo(
      this ServiceDefinition serviceDefinition,
      IVssRequestContext requestContext,
      IDictionary<Guid, string> urlCache = null)
    {
      if (serviceDefinition == null)
        return (ResourceAreaInfo) null;
      Func<Guid, string> funcResolveUrl = (Func<Guid, string>) (serviceId =>
      {
        if (!(serviceId != ServiceInstanceTypes.SPS))
          return requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.SPS, AccessMappingConstants.PublicAccessMappingMoniker);
        return (serviceDefinition.GetLocationMapping(AccessMappingConstants.PublicAccessMappingMoniker) ?? serviceDefinition.LocationMappings.FirstOrDefault<LocationMapping>())?.Location;
      });
      return ServiceDefinitionExtensions.ToResourceAreaInfo(requestContext, serviceDefinition.ToResourceArea(), funcResolveUrl, urlCache);
    }

    public static ResourceAreaInfo ToResourceAreaInfo(
      this ResourceArea resourceArea,
      IVssRequestContext requestContext,
      IDictionary<Guid, string> urlCache = null)
    {
      return ServiceDefinitionExtensions.ToResourceAreaInfo(requestContext.To(TeamFoundationHostType.Deployment), resourceArea, requestContext.ServiceHost.InstanceId, requestContext.ServiceHost.HostType, urlCache);
    }

    public static ResourceAreaInfo ToResourceAreaInfo(
      this ResourceArea resourceArea,
      IVssRequestContext requestContext,
      NameResolutionEntry entry,
      IDictionary<Guid, string> urlCache = null)
    {
      return ServiceDefinitionExtensions.ToResourceAreaInfo(requestContext, resourceArea, entry.Value, entry.GetHostType(), urlCache);
    }

    private static ResourceAreaInfo ToResourceAreaInfo(
      IVssRequestContext requestContext,
      ResourceArea resourceArea,
      Guid hostId,
      TeamFoundationHostType hostType,
      IDictionary<Guid, string> urlCache = null)
    {
      Func<Guid, string> funcResolveUrl = (Func<Guid, string>) (serviceId =>
      {
        string resourceAreaInfo = ServiceDefinitionExtensions.GetHostUri(requestContext, hostId, serviceId);
        if (resourceAreaInfo != null && serviceId != ServiceInstanceTypes.SPS && !requestContext.GetService<IInstanceManagementService>().GetServiceInstances(requestContext, serviceId).Any<ServiceInstance>((Func<ServiceInstance, bool>) (si => si.GetProperty<TeamFoundationHostType>("Microsoft.TeamFoundation.Location.PhysicalHostTypesSupported", TeamFoundationHostType.Unknown).HasFlag((Enum) TeamFoundationHostTypeHelper.NormalizeHostType(hostType)))))
          resourceAreaInfo = (string) null;
        return resourceAreaInfo;
      });
      return ServiceDefinitionExtensions.ToResourceAreaInfo(requestContext, resourceArea, funcResolveUrl, urlCache);
    }

    private static string GetHostUri(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid serviceId)
    {
      if (requestContext.ServiceHost.InstanceId == hostId)
        return requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, serviceId, AccessMappingConstants.PublicAccessMappingMoniker);
      string location = requestContext.GetService<IUrlHostResolutionService>().GetHostUri(requestContext, hostId, false, serviceId)?.ToString();
      if (location == null)
      {
        IVssRequestContext requestContext1 = requestContext;
        Guid hostId1 = hostId;
        Guid? nullable = new Guid?(serviceId);
        bool? useAGuid = new bool?();
        Guid? serviceIdentifier = nullable;
        AccessMapping guidAccessMapping = LocationServiceHelper.GetHostGuidAccessMapping(requestContext1, hostId1, useAGuid: useAGuid, serviceIdentifier: serviceIdentifier);
        location = guidAccessMapping != null ? guidAccessMapping.ToLocationMapping()?.Location : (string) null;
      }
      return location;
    }

    private static ResourceAreaInfo ToResourceAreaInfo(
      IVssRequestContext requestContext,
      ResourceArea resourceArea,
      Func<Guid, string> funcResolveUrl,
      IDictionary<Guid, string> urlCache)
    {
      if (resourceArea == null)
        return (ResourceAreaInfo) null;
      Guid key = ServiceDefinitionExtensions.NormalizeParentService(resourceArea.ParentService, ServiceInstanceTypes.SPS);
      string str;
      if (urlCache == null || !urlCache.TryGetValue(key, out str))
      {
        str = funcResolveUrl(key);
        urlCache?.Add(key, str);
      }
      return new ResourceAreaInfo()
      {
        Id = resourceArea.AreaId,
        Name = resourceArea.AreaName,
        LocationUrl = str
      };
    }

    private static Guid NormalizeParentService(Guid parentService, Guid defaultParentService) => !(parentService == Guid.Empty) ? parentService : defaultParentService;
  }
}
