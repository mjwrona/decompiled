// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostProxyDataProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class HostProxyDataProvider : IHostProxyDataProvider, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual HostProxyData LoadHostProxyData(
      IVssRequestContext deploymentContext,
      Guid hostId,
      RouteFlags routeFlags)
    {
      ServiceInstance serviceInstance = this.GetServiceInstance(deploymentContext, hostId, routeFlags);
      return serviceInstance == null ? (HostProxyData) null : this.ProxyDataFromServiceInstance(deploymentContext, serviceInstance, hostId, routeFlags);
    }

    protected ServiceInstance GetServiceInstance(
      IVssRequestContext deploymentContext,
      Guid hostId,
      RouteFlags routeFlags)
    {
      routeFlags &= RouteFlags.AnyHost;
      switch (routeFlags)
      {
        case RouteFlags.None:
          return (ServiceInstance) null;
        case RouteFlags.DeploymentHost:
          return this.GetDeploymentServiceInstance(deploymentContext, hostId);
        case RouteFlags.OrganizationHost:
          return this.GetOrganizationServiceInstance(deploymentContext, hostId);
        case RouteFlags.CollectionHost:
          return this.GetCollectionServiceInstance(deploymentContext, hostId);
        default:
          return this.GetAmbiguousServiceInstance(deploymentContext, hostId);
      }
    }

    protected virtual ServiceInstance GetCollectionServiceInstance(
      IVssRequestContext deploymentContext,
      Guid hostId)
    {
      return this.GetServiceInstance(deploymentContext, hostId);
    }

    protected virtual ServiceInstance GetOrganizationServiceInstance(
      IVssRequestContext deploymentContext,
      Guid hostId)
    {
      return this.GetServiceInstance(deploymentContext, hostId);
    }

    protected virtual ServiceInstance GetAmbiguousServiceInstance(
      IVssRequestContext deploymentContext,
      Guid hostId)
    {
      return this.GetDeploymentServiceInstance(deploymentContext, hostId) ?? this.GetServiceInstance(deploymentContext, hostId);
    }

    protected virtual ServiceInstance GetDeploymentServiceInstance(
      IVssRequestContext deploymentContext,
      Guid hostId)
    {
      return deploymentContext.GetService<IInstanceManagementService>().GetServiceInstance(deploymentContext, hostId);
    }

    private ServiceInstance GetServiceInstance(IVssRequestContext deploymentContext, Guid hostId) => deploymentContext.GetService<IInstanceManagementService>().GetHostInstanceMapping(deploymentContext, hostId)?.ServiceInstance;

    private HostProxyData ProxyDataFromServiceInstance(
      IVssRequestContext deploymentContext,
      ServiceInstance serviceInstance,
      Guid hostId,
      RouteFlags routeFlags)
    {
      return new HostProxyData()
      {
        HostId = hostId,
        TargetInstanceId = serviceInstance.InstanceId,
        TargetInstanceUrl = serviceInstance.AzureUri.AbsoluteUri,
        TargetPublicUrl = serviceInstance.PublicUri.AbsoluteUri
      };
    }
  }
}
