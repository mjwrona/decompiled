// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.IAzureVirtualMachineScaleSetResourceServiceInternal
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  [DefaultServiceImplementation(typeof (AzureVirtualMachineScaleSetResourceService))]
  internal interface IAzureVirtualMachineScaleSetResourceServiceInternal : 
    IAzureVirtualMachineScaleSetResourceService,
    IVssFrameworkService
  {
    Task InstallAgentExtensionAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string poolName,
      string token);

    Task DeleteMachinesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      IEnumerable<string> instanceIds,
      bool force);

    Task SetCapacityAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      int newCapacity);

    Task<IReadOnlyList<VirtualMachineScaleSetVM>> ListVMInstancesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool);

    Task<IReadOnlyList<VirtualMachineScaleSetVM>> ListVMsAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool);

    Task ReimageAllMachinesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      HashSet<string> instanceIds);

    Task ReimageMachinesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      HashSet<string> instanceIds);

    Task<VirtualMachineScaleSetVMInstanceView> GetInstanceViewAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string instanceId);

    Task UpgradeMachinesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      HashSet<string> instanceIds);

    Task<VirtualMachineScaleSet> UpdateScaleSetAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      VirtualMachineScaleSet scaleSet);

    Task DeleteExtensionAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string extensionName);
  }
}
