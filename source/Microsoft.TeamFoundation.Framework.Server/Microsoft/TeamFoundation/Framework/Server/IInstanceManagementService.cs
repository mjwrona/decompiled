// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IInstanceManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (IInternalInstanceManagementService))]
  public interface IInstanceManagementService : IVssFrameworkService
  {
    void RegisterServiceInstanceType(
      IVssRequestContext requestContext,
      ServiceInstanceType instanceTypeToRegister);

    void UnregisterServiceInstanceType(
      IVssRequestContext requestContext,
      ServiceInstanceType instanceTypeToUnregister);

    void UpdateServiceInstanceType(
      IVssRequestContext requestContext,
      ServiceInstanceType instanceTypeToUpdate);

    ServiceInstanceType GetServiceInstanceType(IVssRequestContext requestContext, Guid instanceType);

    IList<ServiceInstanceType> GetServiceInstanceTypes(IVssRequestContext requestContext);

    void RegisterServiceInstance(
      IVssRequestContext requestContext,
      ServiceInstance instanceToRegister);

    void UnregisterServiceInstance(
      IVssRequestContext requestContext,
      ServiceInstance instanceToUnregister);

    void UpdateServiceInstance(IVssRequestContext requestContext, ServiceInstance instanceToUpdate);

    ServiceInstance GetServiceInstance(IVssRequestContext requestContext, Guid instanceId);

    IList<ServiceInstance> GetServiceInstances(IVssRequestContext requestContext, Guid instanceType = default (Guid));

    IList<ServiceInstance> GetServiceInstancesForRegion(
      IVssRequestContext requestContext,
      string region,
      Guid instanceType = default (Guid));

    void SetHostInstanceMapping(
      IVssRequestContext requestContext,
      Guid hostId,
      ServiceInstance serviceInstance = null);

    void RemoveHostInstanceMapping(
      IVssRequestContext requestContext,
      Guid hostId,
      ServiceInstance serviceInstance = null,
      bool overrideInstanceCheck = false);

    HostInstanceMapping GetHostInstanceMapping(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid instanceType = default (Guid),
      bool previewFaultIn = false);

    IList<HostInstanceMapping> GetHostInstanceMappings(
      IVssRequestContext requestContext,
      Guid hostId);

    void UpdateHostInstanceMappingStatus(
      IVssRequestContext requestContext,
      Guid hostId,
      ServiceStatus status,
      ServiceInstance serviceInstance = null);

    IList<string> GetRegisteredServiceDomains(IVssRequestContext requestContext);
  }
}
