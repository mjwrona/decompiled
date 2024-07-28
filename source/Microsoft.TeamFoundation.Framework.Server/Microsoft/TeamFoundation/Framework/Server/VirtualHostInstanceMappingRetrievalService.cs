// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VirtualHostInstanceMappingRetrievalService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VirtualHostInstanceMappingRetrievalService : 
    IVirtualHostInstanceMappingRetrievalService,
    IVssFrameworkService
  {
    private const string s_area = "InstanceManagement";
    private const string s_layer = "VirtualHostInstanceMappingRetrievalService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Dictionary<Guid, List<VirtualHostInstanceMapping>> GetVirtualHostInstanceMappings(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(14444082, "InstanceManagement", nameof (VirtualHostInstanceMappingRetrievalService), nameof (GetVirtualHostInstanceMappings));
      try
      {
        Dictionary<Guid, List<VirtualHostInstanceMapping>> instanceMappings = new Dictionary<Guid, List<VirtualHostInstanceMapping>>();
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          return instanceMappings;
        IEnumerable<ServiceDefinition> serviceDefinitions = requestContext.GetService<ILocationService>().GetLocationData(requestContext, ServiceInstanceTypes.SPS).FindServiceDefinitions(requestContext, "VirtualLocation");
        requestContext.To(TeamFoundationHostType.Deployment).GetService<IInstanceManagementService>();
        foreach (ServiceDefinition instanceMapping in serviceDefinitions)
        {
          VirtualHostInstanceMapping hostInstanceMapping = VirtualHostInstanceMapping.FromServiceDefinition(requestContext.ServiceHost.InstanceId, instanceMapping);
          if (!instanceMappings.ContainsKey(hostInstanceMapping.ServiceInstanceType))
            instanceMappings[hostInstanceMapping.ServiceInstanceType] = new List<VirtualHostInstanceMapping>();
          instanceMappings[hostInstanceMapping.ServiceInstanceType].Add(hostInstanceMapping);
        }
        return instanceMappings;
      }
      finally
      {
        requestContext.TraceLeave(14160473, "InstanceManagement", nameof (VirtualHostInstanceMappingRetrievalService), nameof (GetVirtualHostInstanceMappings));
      }
    }
  }
}
