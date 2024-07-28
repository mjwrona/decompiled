// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceInstance
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Partitioning;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DataContract]
  public class ServiceInstance : PropertyContainerObject
  {
    private static readonly string[] c_includedAccessMappings = new string[2]
    {
      AccessMappingConstants.PublicAccessMappingMoniker,
      AccessMappingConstants.AzureInstanceMappingMoniker
    };
    private static readonly string[] c_excludedAdditionalMappings = new string[2]
    {
      AccessMappingConstants.ServiceDomainMappingMoniker,
      AccessMappingConstants.ServicePathMappingMoniker
    };

    public ServiceInstance()
    {
      this.Regions = new RegionCollection();
      this.AdditionalMappings = new List<LocationMapping>();
    }

    public ServiceInstance(PartitionContainer container)
    {
      this.InstanceId = container.ContainerId;
      this.InstanceType = container.ContainerType;
      this.Name = container.Name;
      this.PublicUri = container.Address != null ? new Uri(container.Address) : (Uri) null;
      this.AzureUri = container.InternalAddress != null ? new Uri(container.InternalAddress) : (Uri) null;
      foreach (string tag in container.Tags)
      {
        string[] strArray = tag.Split(new char[1]{ '=' }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length == 1)
          this.Properties[tag] = (object) "";
        else if (strArray.Length == 2)
          this.Properties[strArray[0]] = (object) strArray[1];
        else if (strArray.Length > 2)
          this.Properties[strArray[0]] = (object) string.Join("=", strArray, 1, strArray.Length - 1);
      }
    }

    internal ServiceInstance(ServiceDefinition serviceDefinition)
    {
      Guid identifier = serviceDefinition.Identifier;
      Guid parentIdentifier = serviceDefinition.ParentIdentifier;
      LocationMapping locationMapping1 = serviceDefinition.GetLocationMapping(AccessMappingConstants.PublicAccessMappingMoniker);
      LocationMapping locationMapping2 = serviceDefinition.GetLocationMapping(AccessMappingConstants.AzureInstanceMappingMoniker);
      string str = serviceDefinition.GetProperty<string>(InstanceManagementPropertyConstants.HostedServiceName, (string) null) ?? serviceDefinition.GetProperty<string>(InstanceManagementPropertyConstants.DevFabricMachineName, (string) null);
      RegionCollection regionsAndWeights = RegionHelper.ParseRegionsAndWeights(serviceDefinition.GetProperty<string>(InstanceManagementPropertyConstants.RegionsPropertyName, ""), serviceDefinition.GetProperty<string>(InstanceManagementPropertyConstants.WeightsPropertyName, "0"), false);
      this.InstanceId = identifier;
      this.InstanceType = parentIdentifier;
      this.Name = str;
      this.PublicUri = locationMapping1 != null ? new Uri(locationMapping1.Location) : (Uri) null;
      this.AzureUri = locationMapping2 != null ? new Uri(locationMapping2.Location) : (Uri) null;
      this.Regions = regionsAndWeights;
      this.Properties = new PropertiesCollection((IDictionary<string, object>) serviceDefinition.Properties);
      this.AdditionalMappings = ServiceInstance.FilterServiceDefinitionMappings(serviceDefinition.LocationMappings);
    }

    [DataMember]
    public Guid InstanceId { get; set; }

    [DataMember]
    public Guid InstanceType { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public Uri PublicUri { get; set; }

    [DataMember]
    public Uri AzureUri { get; set; }

    [DataMember]
    internal RegionCollection Regions { get; set; }

    [DataMember]
    public List<LocationMapping> AdditionalMappings { get; set; }

    public string[] GetSupportedRegions() => this.Regions.GetSupportedRegions();

    public bool IsRegionSupported(string region) => this.Regions.IsRegionSupported(region);

    public int GetRegionWeight(string region) => this.Regions.GetRegionWeight(region);

    public void SetRegionWeight(string region, int weight) => this.Regions.SetRegionWeight(region, weight);

    public bool SupportsPhysicalHostsForHostType(TeamFoundationHostType hostType) => this.GetProperty<TeamFoundationHostType>("Microsoft.TeamFoundation.Location.PhysicalHostTypesSupported", TeamFoundationHostType.All).HasFlag((Enum) hostType);

    public ServiceInstance Clone()
    {
      ServiceInstance serviceInstance = new ServiceInstance();
      serviceInstance.InstanceId = this.InstanceId;
      serviceInstance.InstanceType = this.InstanceType;
      serviceInstance.Name = this.Name;
      serviceInstance.PublicUri = this.PublicUri;
      serviceInstance.AzureUri = this.AzureUri;
      serviceInstance.Regions = new RegionCollection(this.Regions);
      serviceInstance.Properties = new PropertiesCollection((IDictionary<string, object>) this.Properties);
      serviceInstance.AdditionalMappings = this.AdditionalMappings.Select<LocationMapping, LocationMapping>((Func<LocationMapping, LocationMapping>) (m => m.Clone())).ToList<LocationMapping>();
      return serviceInstance;
    }

    internal static TeamFoundationHostType GetPhysicalServiceHostTypeSupported(
      bool supportOrganizationLevelExperiences,
      bool supportCollectionLevelExperiences)
    {
      return ServiceInstance.GetHostTypeMask(supportOrganizationLevelExperiences, supportCollectionLevelExperiences, TeamFoundationHostType.Deployment);
    }

    internal static TeamFoundationHostType GetLightweightServiceHostTypeSupported(
      bool lightweightOrganizationHosts,
      bool lightweightCollectionHosts)
    {
      return ServiceInstance.GetHostTypeMask(lightweightOrganizationHosts, lightweightCollectionHosts, TeamFoundationHostType.Unknown);
    }

    private static TeamFoundationHostType GetHostTypeMask(
      bool organizationValue,
      bool collectionValue,
      TeamFoundationHostType defaultValue)
    {
      TeamFoundationHostType hostTypeMask = defaultValue;
      if (organizationValue)
        hostTypeMask |= TeamFoundationHostType.Application;
      if (collectionValue)
        hostTypeMask |= TeamFoundationHostType.ProjectCollection;
      return hostTypeMask;
    }

    public static List<LocationMapping> FilterServiceDefinitionMappings(
      List<LocationMapping> mappings)
    {
      return ServiceInstance.FilterIncludedMappings(ServiceInstance.FilterAdditionalMappings((IEnumerable<LocationMapping>) mappings)).ToList<LocationMapping>();
    }

    internal static IEnumerable<LocationMapping> FilterIncludedMappings(
      IEnumerable<LocationMapping> mappings)
    {
      return mappings.Where<LocationMapping>((Func<LocationMapping, bool>) (m => !((IEnumerable<string>) ServiceInstance.c_includedAccessMappings).Contains<string>(m.AccessMappingMoniker, (IEqualityComparer<string>) VssStringComparer.AccessMappingMoniker)));
    }

    internal static IEnumerable<LocationMapping> FilterAdditionalMappings(
      IEnumerable<LocationMapping> mappings)
    {
      return mappings.Where<LocationMapping>((Func<LocationMapping, bool>) (m => !((IEnumerable<string>) ServiceInstance.c_excludedAdditionalMappings).Contains<string>(m.AccessMappingMoniker, (IEqualityComparer<string>) VssStringComparer.AccessMappingMoniker)));
    }
  }
}
