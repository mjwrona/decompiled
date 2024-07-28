// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.VirtualLocalLocationDataProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal class VirtualLocalLocationDataProvider : LocalLocationDataProvider
  {
    public VirtualLocalLocationDataProvider(
      IVssRequestContext requestContext,
      ILocationDataCache<string> locationCache)
      : base(requestContext, locationCache)
    {
    }

    protected internal override LocationData FetchLocationData(IVssRequestContext requestContext)
    {
      requestContext.CheckHostedDeployment();
      requestContext.CheckOrganizationOnlyRequestContext();
      Dictionary<string, AccessMapping> accessMappings = new Dictionary<string, AccessMapping>((IEqualityComparer<string>) VssStringComparer.AccessMappingMoniker);
      AccessMapping guidAccessMapping = LocationServiceHelper.GetHostGuidAccessMapping(requestContext, requestContext.ServiceHost.InstanceId);
      accessMappings[AccessMappingConstants.HostGuidAccessMappingMoniker] = guidAccessMapping;
      AccessMapping accessMapping = LocationServiceHelper.GetHostPublicAccessMapping(requestContext, requestContext.ServiceHost.InstanceId);
      if (accessMapping == null)
      {
        accessMapping = guidAccessMapping.Clone();
        accessMapping.Moniker = AccessMappingConstants.PublicAccessMappingMoniker;
        accessMapping.DisplayName = TFCommonResources.PublicAccessMappingDisplayName();
      }
      accessMappings[AccessMappingConstants.PublicAccessMappingMoniker] = accessMapping;
      accessMappings[AccessMappingConstants.ServerAccessMappingMoniker] = accessMapping;
      LocationData locationData = new LocationData(accessMappings, accessMapping, accessMapping, accessMapping, (IEnumerable<ServiceDefinition>) null, (string) null, DateTime.MaxValue, 1L, this.m_hostId, this.m_instanceType, requestContext.ServiceHost.DeploymentServiceHost.InstanceId);
      if (this.InstanceType != ServiceInstanceTypes.SPS)
      {
        object obj = (object) null;
        ServiceDefinition serviceDefinition = (ServiceDefinition) null;
        if (!requestContext.RootContext.Items.TryGetValue(RequestContextItemsKeys.SpsDefinition, out obj))
        {
          if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application) && requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
            serviceDefinition = LocationServiceHelper.GetSpsDefinitionForHost(requestContext, requestContext.RootContext.ServiceHost.InstanceId);
        }
        else
          serviceDefinition = (ServiceDefinition) obj;
        if (serviceDefinition == null)
          serviceDefinition = LocationServiceHelper.CreateSpsServiceDefinition(requestContext, requestContext.ServiceHost.InstanceId);
        serviceDefinition.LocationMappings.Clear();
        locationData.AddServiceDefinition(this.PostProcessAndCloneServiceDefinition(requestContext, serviceDefinition, locationData.WebAppRelativeDirectory, (Func<string, Guid, ServiceDefinition>) ((st, id) => (ServiceDefinition) null)));
      }
      using (this.AcquireSqlSemaphore(requestContext))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        foreach (TeamFoundationServiceHostProperties child in vssRequestContext.GetService<TeamFoundationHostManagementService>().QueryServiceHostProperties(vssRequestContext, requestContext.ServiceHost.InstanceId, ServiceHostFilterFlags.IncludeChildren).Children)
        {
          AccessMapping publicAccessMapping = LocationServiceHelper.GetHostPublicAccessMapping(requestContext, child.Id);
          LocationMapping locationMapping = publicAccessMapping != null ? publicAccessMapping.ToLocationMapping() : (LocationMapping) null;
          if (locationMapping == null)
          {
            locationMapping = LocationServiceHelper.GetHostGuidLocationMapping(requestContext, child.Id);
            locationMapping.AccessMappingMoniker = AccessMappingConstants.PublicAccessMappingMoniker;
          }
          Guid id1 = child.Id;
          string displayName1 = TFCommonResources.LocationService();
          string description1 = TFCommonResources.FrameworkLocationServiceDescription();
          List<LocationMapping> locationMappings1 = new List<LocationMapping>();
          locationMappings1.Add(locationMapping);
          locationMappings1.Add(LocationServiceHelper.GetHostGuidLocationMapping(requestContext, child.Id));
          Guid serviceOwner1 = new Guid();
          ServiceDefinition serviceDefinition1 = new ServiceDefinition("LocationService2", id1, displayName1, (string) null, Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified, description1, "Framework", locationMappings1, serviceOwner1);
          serviceDefinition1.SetProperty("Microsoft.TeamFoundation.Location.CollectionName", (object) child.Name);
          locationData.AddServiceDefinition(serviceDefinition1);
          if (this.InstanceType == Guid.Empty || this.InstanceType == ServiceInstanceTypes.TFS)
          {
            Guid id2 = child.Id;
            string displayName2 = TFCommonResources.LocationService();
            string description2 = TFCommonResources.FrameworkLocationServiceDescription();
            List<LocationMapping> locationMappings2 = new List<LocationMapping>();
            locationMappings2.Add(new LocationMapping()
            {
              AccessMappingMoniker = locationMapping.AccessMappingMoniker,
              Location = TFCommonUtil.CombinePaths(locationMapping.Location, Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.CollectionLocationServiceRelativePath)
            });
            Guid serviceOwner2 = new Guid();
            ServiceDefinition serviceDefinition2 = new ServiceDefinition("LocationService", id2, displayName2, (string) null, Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified, description2, "Framework", locationMappings2, serviceOwner2);
            locationData.AddServiceDefinition(serviceDefinition2);
          }
        }
      }
      return locationData;
    }

    protected override IDisposable AcquireLoadSemaphore(IVssRequestContext requestContext) => (IDisposable) null;
  }
}
