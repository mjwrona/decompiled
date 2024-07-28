// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VirtualHostInstanceMappingRegistrationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VirtualHostInstanceMappingRegistrationService : 
    IVirtualHostInstanceMappingRegistrationService,
    IVssFrameworkService
  {
    private const string s_area = "InstanceManagement";
    private const string s_layer = "VirtualHostInstanceMappingRegistrationService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void RegisterVirtualHostInstanceMapping(
      IVssRequestContext deploymentContext,
      Guid hostId)
    {
      this.RegisterVirtualHostInstanceMapping(deploymentContext, hostId, deploymentContext.ServiceInstanceType(), deploymentContext.ServiceHost.InstanceId);
    }

    public void RegisterVirtualHostInstanceMapping(
      IVssRequestContext deploymentContext,
      Guid hostId,
      Guid serviceInstanceType,
      Guid serviceInstanceId)
    {
      deploymentContext.TraceEnter(35556860, "InstanceManagement", nameof (VirtualHostInstanceMappingRegistrationService), nameof (RegisterVirtualHostInstanceMapping));
      try
      {
        deploymentContext.CheckDeploymentRequestContext();
        if (serviceInstanceType == ServiceInstanceTypes.SPS)
          return;
        if (deploymentContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
        {
          using (IVssRequestContext vssRequestContext = this.BeginLocalContext(deploymentContext, hostId))
            vssRequestContext.GetService<ILocationService>().SaveServiceDefinitions(vssRequestContext, (IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
            {
              new VirtualHostInstanceMapping()
              {
                HostId = hostId,
                ServiceInstanceId = serviceInstanceId,
                ServiceInstanceType = serviceInstanceType
              }.ToServiceDefinition()
            });
        }
        else
          LocationServiceHelper.GetSpsLocationClient(deploymentContext, hostId).UpdateServiceDefinitionsAsync((IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
          {
            new VirtualHostInstanceMapping()
            {
              HostId = hostId,
              ServiceInstanceId = serviceInstanceId,
              ServiceInstanceType = serviceInstanceType
            }.ToServiceDefinition()
          }, deploymentContext.CancellationToken).SyncResult();
      }
      finally
      {
        deploymentContext.TraceLeave(47338599, "InstanceManagement", nameof (VirtualHostInstanceMappingRegistrationService), nameof (RegisterVirtualHostInstanceMapping));
      }
    }

    public void UnRegisterVirtualHostInstanceMapping(
      IVssRequestContext deploymentContext,
      Guid hostId)
    {
      deploymentContext.TraceEnter(18907236, "InstanceManagement", nameof (VirtualHostInstanceMappingRegistrationService), nameof (UnRegisterVirtualHostInstanceMapping));
      try
      {
        deploymentContext.CheckDeploymentRequestContext();
        if (deploymentContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
          return;
        deploymentContext.TraceAlways(23438307, TraceLevel.Info, "InstanceManagement", nameof (VirtualHostInstanceMappingRegistrationService), (string[]) null, string.Format("Deleting virtual host instance mapping for host: {0}.", (object) hostId));
        TaskExtensions.SyncResult(LocationServiceHelper.GetSpsLocationClient(deploymentContext, hostId).DeleteServiceDefinitionAsync("VirtualLocation", deploymentContext.ServiceHost.InstanceId, deploymentContext.CancellationToken));
      }
      finally
      {
        deploymentContext.TraceLeave(36857927, "InstanceManagement", nameof (VirtualHostInstanceMappingRegistrationService), nameof (UnRegisterVirtualHostInstanceMapping));
      }
    }

    private IVssRequestContext BeginLocalContext(IVssRequestContext deploymentContext, Guid hostId) => deploymentContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(deploymentContext, hostId, RequestContextType.SystemContext);
  }
}
