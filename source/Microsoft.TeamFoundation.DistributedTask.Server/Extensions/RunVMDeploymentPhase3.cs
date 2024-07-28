// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Extensions.RunVMDeploymentPhase3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Extensions
{
  internal sealed class RunVMDeploymentPhase3 : RunDeploymentPhaseBase2
  {
    private IStrategyExecutor2 m_strategyExecutor;

    public IVMProviderManager2 VMProviderManager { get; set; }

    protected override IStrategyExecutor2 StrategyExecutor => this.m_strategyExecutor;

    protected override void EnsureExtensions(OrchestrationContext context)
    {
      base.EnsureExtensions(context);
      this.VMProviderManager = context.CreateClient<IVMProviderManager2>(true);
    }

    protected override async Task InitializeStrategyExecutor(
      OrchestrationContext context,
      RunDeploymentPhaseInput2 input)
    {
      RunVMDeploymentPhase3 deploymentPhase3 = this;
      context.Trace(0, TraceLevel.Info, "Started deployment phase orchestration for target type VirtualMachine and strategy " + DeploymentPhaseHelper.GetStrategyName(input.Strategy.Type) + ".");
      EnvironmentDeploymentTarget environmentTarget = input.ProviderPhase.EnvironmentTarget;
      EnvironmentResourceFilter resourceFilter = environmentTarget.ResourceFilter;
      VMDeploymentExecutionState vmDeploymentExecutionState = new VMDeploymentExecutionState();
      IList<VirtualMachineResource> vms = await deploymentPhase3.ExecuteAsync<IList<VirtualMachineResource>>(context, input, (Func<Task<IList<VirtualMachineResource>>>) (() => this.VMProviderManager.GetVirtualMachines(input.ScopeId, environmentTarget.EnvironmentId, resourceFilter)));
      if (!vms.Any<VirtualMachineResource>())
      {
        // ISSUE: variable of a boxed type
        __Boxed<int> environmentId = (ValueType) environmentTarget.EnvironmentId;
        int? id = resourceFilter.Id;
        string empty;
        if (!id.HasValue)
        {
          empty = string.Empty;
        }
        else
        {
          id = resourceFilter.Id;
          empty = id.ToString();
        }
        string str1 = resourceFilter.Name ?? string.Empty;
        string str2 = string.Join(",", (IEnumerable<string>) resourceFilter.Tags);
        throw new ResourceNotFoundWithSpecifiedCriteria(TaskResources.NoResourceFoundWithSpecifiedFilter((object) environmentId, (object) empty, (object) str1, (object) "VirtualMachine", (object) str2));
      }
      vmDeploymentExecutionState.DeploymentPool = await deploymentPhase3.ExecuteAsync<TaskAgentPoolReference>(context, input, (Func<Task<TaskAgentPoolReference>>) (() => this.VMProviderManager.GetEnvironmentDeploymentPool(input.ScopeId, environmentTarget.EnvironmentId)));
      foreach (VirtualMachineResource virtualMachineResource in (IEnumerable<VirtualMachineResource>) vms)
        vmDeploymentExecutionState.VMResources.Add(new VMResourceState()
        {
          ResourceId = virtualMachineResource.Id,
          ResourceName = virtualMachineResource.Name,
          AgentId = virtualMachineResource.Agent.Id,
          AgentName = virtualMachineResource.Agent.Name,
          AgentStatus = virtualMachineResource.Agent.Status,
          DeploymentAttempted = false
        });
      deploymentPhase3.m_strategyExecutor = VMStrategyExecutorFactory2.GetDeploymentStrategyExecutor(input, vmDeploymentExecutionState);
      deploymentPhase3.m_strategyExecutor.Initialize();
      vmDeploymentExecutionState = (VMDeploymentExecutionState) null;
      vms = (IList<VirtualMachineResource>) null;
    }
  }
}
