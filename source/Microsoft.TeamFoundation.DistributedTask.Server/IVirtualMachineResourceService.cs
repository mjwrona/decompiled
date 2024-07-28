// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IVirtualMachineResourceService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (VirtualMachineResourceService))]
  public interface IVirtualMachineResourceService : 
    IEnvironmentResourceService<VirtualMachineResource>,
    IVssFrameworkService
  {
    Task<IPagedList<VirtualMachineResource>> GetVirtualMachinesPagedAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      string continuationToken = null,
      int top = 1000,
      string name = null,
      IList<string> tagFilters = null);

    void DeleteVirtualMachineResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      int resourceId);

    VirtualMachineResource UpdateVirtualMachineResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      VirtualMachineResource resource,
      TaskAgentCapabilityType capabilityUpdate = TaskAgentCapabilityType.System);
  }
}
