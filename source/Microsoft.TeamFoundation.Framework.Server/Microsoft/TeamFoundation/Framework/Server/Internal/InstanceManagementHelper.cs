// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Internal.InstanceManagementHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Client;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Partitioning;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class InstanceManagementHelper
  {
    private static readonly Guid SPSPrincipal = new Guid("00000001-0000-8888-8000-000000000000");
    private static readonly Guid TFSPrincipal = new Guid("00000002-0000-8888-8000-000000000000");
    private static readonly Guid ELSPrincipal = new Guid("0000000A-0000-8888-8000-000000000000");
    private static readonly Guid TFSOnPremisesPrincipal = new Guid("000007F5-0000-8888-8000-000000000000");
    internal static readonly Guid UserSvcPrincipal = new Guid("00000038-0000-8888-8000-000000000000");
    internal static readonly Guid TokenSvcPrincipal = new Guid("00000052-0000-8888-8000-000000000000");
    private static readonly Guid ELSInstanceType = new Guid("6C404D78-EF65-4E65-8B6A-DF19D6361EAE");

    public static bool IsValidServiceInstanceType(Guid instanceType) => ServicePrincipals.IsInternalServicePrincipalId(InstanceManagementHelper.ServicePrincipalFromServiceInstance(instanceType));

    public static Guid ServicePrincipalFromServiceInstance(Guid serviceInstanceType)
    {
      Guid guid = serviceInstanceType;
      if (ServiceInstanceTypes.TFSOnPremises == serviceInstanceType)
        guid = InstanceManagementHelper.TFSOnPremisesPrincipal;
      else if (ServiceInstanceTypes.SPS == serviceInstanceType)
        guid = InstanceManagementHelper.SPSPrincipal;
      else if (ServiceInstanceTypes.TFS == serviceInstanceType)
        guid = InstanceManagementHelper.TFSPrincipal;
      else if (InstanceManagementHelper.ELSInstanceType == serviceInstanceType)
        guid = InstanceManagementHelper.ELSPrincipal;
      return guid;
    }

    public static bool RequiresSharedService(Guid serviceInstanceType) => serviceInstanceType != ServiceInstanceTypes.SPS && serviceInstanceType != ServiceInstanceTypes.MPS && serviceInstanceType != InstanceManagementHelper.UserSvcPrincipal;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool RegistersWithPartitionService(Guid serviceInstanceType) => !InstanceManagementHelper.RequiresSharedService(serviceInstanceType) || serviceInstanceType == FrameworkServerConstants.UserExtensionPrincipal;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool RegistersWithInstanceManagement(Guid serviceInstanceType) => InstanceManagementHelper.RequiresSharedService(serviceInstanceType) || serviceInstanceType == InstanceManagementHelper.UserSvcPrincipal;

    internal static ServiceInstanceType GetServiceInstanceType(
      IVssRequestContext requestContext,
      LocationHttpClient locationClient,
      Guid instanceTypeId)
    {
      ServiceDefinition serviceDefinition = locationClient.GetServiceDefinitionAsync("VsService", instanceTypeId, requestContext.CancellationToken).SyncResult<ServiceDefinition>();
      return serviceDefinition != null ? new ServiceInstanceType(serviceDefinition) : (ServiceInstanceType) null;
    }

    internal static void SetServiceInstanceType(
      IVssRequestContext requestContext,
      LocationHttpClient locationClient,
      ServiceInstanceType instanceType,
      bool update)
    {
      ServiceDefinition serviceDefinition = locationClient.GetServiceDefinitionAsync("VsService", instanceType.InstanceType, requestContext.CancellationToken).SyncResult<ServiceDefinition>();
      if (!update && serviceDefinition != null)
        return;
      locationClient.UpdateServiceDefinitionsAsync((IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
      {
        instanceType.ToServiceDefinition()
      }, requestContext.CancellationToken).SyncResult();
    }

    internal static void RemoveServiceInstanceType(
      IVssRequestContext requestContext,
      LocationHttpClient locationClient,
      ServiceInstanceType instanceType)
    {
      TaskExtensions.SyncResult(locationClient.DeleteServiceDefinitionAsync("LocationService2", instanceType.InstanceType, requestContext.CancellationToken));
    }

    internal static void SetServiceInstance(
      IVssRequestContext requestContext,
      LocationHttpClient locationClient,
      ServiceInstance serviceInstance,
      bool update)
    {
      ServiceDefinition serviceDefinition1;
      if (!update)
      {
        LocationMapping locationMapping1 = (locationClient.GetServiceDefinitionAsync("VsService", serviceInstance.InstanceType, requestContext.CancellationToken).SyncResult<ServiceDefinition>() ?? throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "You must register a service instance type of {0}", (object) serviceInstance.InstanceType))).GetLocationMapping(AccessMappingConstants.RootDomainMappingMoniker);
        Uri uri1 = locationMapping1 != null ? new Uri(locationMapping1.Location) : (Uri) null;
        LocationMapping locationMapping2 = serviceInstance.AdditionalMappings.SingleOrDefault<LocationMapping>((Func<LocationMapping, bool>) (x => x.AccessMappingMoniker.Equals(AccessMappingConstants.RootDomainMappingMoniker, StringComparison.OrdinalIgnoreCase)));
        Uri uri2 = locationMapping2 != null ? new Uri(locationMapping2.Location) : (Uri) null;
        if (uri1 != (Uri) null && uri2 != (Uri) null && VssStringComparer.Url.Equals(uri1.AbsoluteUri, uri2.AbsoluteUri))
        {
          serviceInstance = serviceInstance.Clone();
          serviceInstance.AdditionalMappings.Remove(locationMapping2);
        }
        serviceDefinition1 = serviceInstance.ToServiceDefinition();
      }
      else
      {
        ServiceDefinition serviceDefinition2 = locationClient.GetServiceDefinitionAsync("LocationService2", serviceInstance.InstanceId, requestContext.CancellationToken).SyncResult<ServiceDefinition>();
        if (serviceDefinition2 == null)
          throw new ArgumentException(string.Format("Could not find registered service instance: {0}", (object) serviceInstance.InstanceId));
        if (serviceInstance.InstanceType != serviceDefinition2.ParentIdentifier)
          throw new ArgumentException("Cannot change the instance type of a registered service instance.");
        serviceDefinition1 = serviceInstance.ToServiceDefinition();
        serviceDefinition1.LocationMappings = serviceDefinition2.LocationMappings;
      }
      locationClient.UpdateServiceDefinitionsAsync((IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
      {
        serviceDefinition1
      }, requestContext.CancellationToken).SyncResult();
    }

    internal static void RemoveServiceInstance(
      IVssRequestContext requestContext,
      LocationHttpClient locationClient,
      ServiceInstance serviceInstance)
    {
      TaskExtensions.SyncResult(locationClient.DeleteServiceDefinitionAsync("LocationService2", serviceInstance.InstanceId, requestContext.CancellationToken));
    }

    internal static bool IsServiceInstanceDefinition(ServiceDefinition definition) => VssStringComparer.ServiceType.Equals(definition.ServiceType, "LocationService2") && VssStringComparer.ServiceType.Equals(definition.ParentServiceType, "VsService");

    internal static void ExecuteForAllSpsInstances(
      IVssRequestContext requestContext,
      Action<LocationHttpClient> action)
    {
      foreach (PartitionContainer partitionContainer in (IEnumerable<PartitionContainer>) requestContext.GetService<IPartitioningService>().QueryPartitionContainers(requestContext, ServiceInstanceTypes.SPS))
      {
        LocationHttpClient locationClient = LocationServiceHelper.CreateLocationClient(requestContext, partitionContainer.Address);
        action(locationClient);
      }
    }
  }
}
