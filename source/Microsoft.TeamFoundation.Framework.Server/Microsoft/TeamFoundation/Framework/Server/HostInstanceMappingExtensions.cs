// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostInstanceMappingExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HostInstanceMappingExtensions
  {
    public static ServiceDefinition ToServiceDefinition(this HostInstanceMapping hostInstanceMapping)
    {
      ServiceDefinition serviceDefinition = new ServiceDefinition("LocationService2", hostInstanceMapping.ServiceInstance.InstanceType, TFCommonResources.LocationService(), (string) null, RelativeToSetting.FullyQualified, TFCommonResources.LocationService(), "Framework")
      {
        ParentServiceType = "LocationService2",
        ParentIdentifier = hostInstanceMapping.ServiceInstance.InstanceId,
        Status = hostInstanceMapping.Status
      };
      if (hostInstanceMapping.Uri != (Uri) null)
        serviceDefinition.LocationMappings.Add(new LocationMapping(AccessMappingConstants.ServerAccessMappingMoniker, hostInstanceMapping.Uri.AbsoluteUri));
      return serviceDefinition;
    }

    public static ServiceDefinition ToServiceDefinition(this ServiceInstanceType serviceInstanceType)
    {
      ServiceDefinition serviceDefinition = new ServiceDefinition("VsService", serviceInstanceType.InstanceType, "VS Service", (string) null, RelativeToSetting.FullyQualified, "VS Service", "Framework");
      serviceDefinition.Properties = serviceInstanceType.Properties;
      if (serviceInstanceType.Name != null)
        serviceDefinition.SetProperty("Microsoft.TeamFoundation.Location.InstanceTypeName", (object) serviceInstanceType.Name);
      serviceDefinition.LocationMappings.AddRange((IEnumerable<LocationMapping>) serviceInstanceType.LocationMappings);
      return serviceDefinition;
    }

    public static ServiceDefinition ToServiceDefinition(this ServiceInstance serviceInstance)
    {
      ServiceDefinition serviceDefinition = new ServiceDefinition("LocationService2", serviceInstance.InstanceId, TFCommonResources.LocationService(), (string) null, RelativeToSetting.FullyQualified, TFCommonResources.LocationService(), "Framework")
      {
        ParentServiceType = "VsService",
        ParentIdentifier = serviceInstance.InstanceType
      };
      serviceDefinition.Properties = serviceInstance.Properties;
      if (serviceInstance.Name != null)
        serviceDefinition.SetProperty(InstanceManagementPropertyConstants.HostedServiceName, (object) serviceInstance.Name);
      if (serviceInstance.PublicUri != (Uri) null)
      {
        LocationMapping locationMapping = new LocationMapping()
        {
          AccessMappingMoniker = AccessMappingConstants.PublicAccessMappingMoniker,
          Location = serviceInstance.PublicUri.AbsoluteUri
        };
        serviceDefinition.LocationMappings.Add(locationMapping);
      }
      if (serviceInstance.AzureUri != (Uri) null)
      {
        LocationMapping locationMapping = new LocationMapping()
        {
          AccessMappingMoniker = AccessMappingConstants.AzureInstanceMappingMoniker,
          Location = serviceInstance.AzureUri.AbsoluteUri
        };
        serviceDefinition.LocationMappings.Add(locationMapping);
      }
      serviceDefinition.LocationMappings.AddRange((IEnumerable<LocationMapping>) serviceInstance.AdditionalMappings);
      string regions;
      string weights;
      RegionHelper.GetRegionsAndWeights(serviceInstance.Regions, out regions, out weights);
      serviceDefinition.SetProperty(InstanceManagementPropertyConstants.RegionsPropertyName, (object) regions);
      serviceDefinition.SetProperty(InstanceManagementPropertyConstants.WeightsPropertyName, (object) weights);
      return serviceDefinition;
    }

    public static bool HasPhysicalOrganizationHosts(this ServiceInstance serviceInstance) => serviceInstance.GetProperty<TeamFoundationHostType>("Microsoft.TeamFoundation.Location.PhysicalHostTypesSupported", TeamFoundationHostType.Unknown).HasFlag((Enum) TeamFoundationHostType.Application);

    public static bool HasLightweightCollectionHosts(this ServiceInstance serviceInstance) => serviceInstance.GetProperty<TeamFoundationHostType>("Microsoft.TeamFoundation.Location.LightweightHostTypes", TeamFoundationHostType.Unknown).HasFlag((Enum) TeamFoundationHostType.ProjectCollection);
  }
}
