// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Extensions.RunVMDeploymentPhase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Extensions
{
  internal sealed class RunVMDeploymentPhase : RunDeploymentPhaseBase
  {
    private IStrategyExecutor m_strategyExecutor;

    public IVMProviderManager VMProviderManager { get; set; }

    protected override IStrategyExecutor StrategyExecutor => this.m_strategyExecutor;

    protected override void EnsureExtensions(OrchestrationContext context)
    {
      base.EnsureExtensions(context);
      this.VMProviderManager = context.CreateClient<IVMProviderManager>(true);
    }

    protected override async Task InitializeStrategyExecutor(
      OrchestrationContext context,
      RunDeploymentPhaseInput input)
    {
      RunVMDeploymentPhase vmDeploymentPhase = this;
      VirtualMachineGroupState virtualMachineGroupState = new VirtualMachineGroupState();
      List<VirtualMachine> vms = await vmDeploymentPhase.ExecuteAsync<List<VirtualMachine>>(context, input, (Func<Task<List<VirtualMachine>>>) (() => this.VMProviderManager.GetVirtualMachines(input.ScopeId, input.ProviderPhase.EnvironmentTarget.EnvironmentId, input.ProviderPhase.EnvironmentTarget.Resource.Id)));
      VirtualMachineGroup virtualMachineGroup = await vmDeploymentPhase.ExecuteAsync<VirtualMachineGroup>(context, input, (Func<Task<VirtualMachineGroup>>) (() => this.VMProviderManager.GetVirtualMachineGroup(input.ScopeId, input.ProviderPhase.EnvironmentTarget.EnvironmentId, input.ProviderPhase.EnvironmentTarget.Resource.Id)));
      virtualMachineGroupState.EnvironmentId = input.ProviderPhase.EnvironmentTarget.EnvironmentId;
      virtualMachineGroupState.VirtualMachineGroup = virtualMachineGroup;
      foreach (VirtualMachine virtualMachine in vms)
      {
        VirtualMachineState virtualMachineState = new VirtualMachineState()
        {
          Id = virtualMachine.Agent.Id,
          Name = virtualMachine.Agent.Name,
          AgentStatus = virtualMachine.Agent.Status,
          DemandsMatched = true,
          DeploymentInQueue = false,
          DeploymentAttempted = false
        };
        virtualMachineGroupState.VirtualMachines.Add(virtualMachine.Agent.Id, virtualMachineState);
      }
      vmDeploymentPhase.m_strategyExecutor = VMStrategyExecutorFactory.GetDeploymentStrategyExecutor(virtualMachineGroupState, input.Strategy, input.Stage, input.Phase);
      vmDeploymentPhase.m_strategyExecutor.Initialize();
      virtualMachineGroupState = (VirtualMachineGroupState) null;
      vms = (List<VirtualMachine>) null;
    }
  }
}
