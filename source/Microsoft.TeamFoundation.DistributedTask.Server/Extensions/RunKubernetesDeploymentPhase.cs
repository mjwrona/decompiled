// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Extensions.RunKubernetesDeploymentPhase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.VisualStudio.Services.Orchestration;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Extensions
{
  internal sealed class RunKubernetesDeploymentPhase : RunDeploymentPhaseBase
  {
    private IStrategyExecutor m_strategyExecutor;

    protected override IStrategyExecutor StrategyExecutor => this.m_strategyExecutor;

    protected override Task InitializeStrategyExecutor(
      OrchestrationContext context,
      RunDeploymentPhaseInput input)
    {
      this.m_strategyExecutor = KubernetesStrategyExecutorFactory.GetDeploymentStrategyExecutor(input.Strategy, input.ProviderPhase);
      this.m_strategyExecutor.Initialize();
      return Task.CompletedTask;
    }
  }
}
