// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Azure.IAzureVirtualMachineScaleSetClient
// Assembly: Microsoft.TeamFoundation.DistributedTask.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C98D823D-2608-4E2C-9060-C83C236BDAA8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Azure.dll

using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Azure
{
  public interface IAzureVirtualMachineScaleSetClient : ICanExpire
  {
    Task<VirtualMachineScaleSet> GetAsync(
      Guid activityId,
      string azureId,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IEnumerable<VirtualMachineScaleSet>> ListAsync(
      Guid activityId,
      Guid subscriptionId,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<VirtualMachineScaleSet> UpdateAsync(
      Guid activityId,
      string azureId,
      VirtualMachineScaleSet scaleSet,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IEnumerable<VirtualMachineScaleSetVM>> ListVMInstancesAsync(
      Guid activityId,
      string azureId,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IEnumerable<VirtualMachineScaleSetVM>> ListVMsAsync(
      Guid activityId,
      string azureId,
      CancellationToken cancellationToken = default (CancellationToken));

    Task DeleteInstancesAsync(
      Guid activityId,
      string azureId,
      MachineInstances instances,
      bool force,
      CancellationToken cancellationToken = default (CancellationToken));

    Task UpgradeInstancesAsync(
      Guid activityId,
      string azureId,
      MachineInstances instances,
      CancellationToken cancellationToken = default (CancellationToken));

    Task ReimageAllInstancesAsync(
      Guid activityId,
      string azureId,
      MachineInstances instances,
      CancellationToken cancellationToken = default (CancellationToken));

    Task ReimageInstancesAsync(
      Guid activityId,
      string azureId,
      MachineInstances instances,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<VirtualMachineScaleSetVMInstanceView> GetInstanceViewAsync(
      Guid activityId,
      string azureId,
      string instanceId,
      CancellationToken cancellationToken = default (CancellationToken));

    Task CreateExtensionAsync(
      Guid activityId,
      string azureId,
      VirtualMachineScaleSetExtension extension,
      CancellationToken cancellationToken = default (CancellationToken));

    Task DeleteExtensionAsync(
      Guid activityId,
      string azureId,
      string extensionName,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IEnumerable<VirtualMachineScaleSetExtension>> ListExtensionsAsync(
      Guid activityId,
      string azureId,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
