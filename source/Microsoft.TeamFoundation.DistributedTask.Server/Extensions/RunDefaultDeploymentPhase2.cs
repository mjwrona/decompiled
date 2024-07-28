// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Extensions.RunDefaultDeploymentPhase2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.VisualStudio.Services.Orchestration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Extensions
{
  internal sealed class RunDefaultDeploymentPhase2 : RunDeploymentPhaseBase2
  {
    private IStrategyExecutor2 m_strategyExecutor;

    protected override IStrategyExecutor2 StrategyExecutor => this.m_strategyExecutor;

    protected override Task InitializeStrategyExecutor(
      OrchestrationContext context,
      RunDeploymentPhaseInput2 input)
    {
      string strategyName = DeploymentPhaseHelper.GetStrategyName(input.Strategy.Type);
      context.Trace(0, TraceLevel.Info, "Started deployment phase orchestration for target type Environment and strategy " + strategyName + ".");
      this.m_strategyExecutor = DefaultStrategyExecutorFactory2.GetDeploymentStrategyExecutor(input.Strategy, input.ProviderPhase);
      this.m_strategyExecutor.Initialize();
      return Task.CompletedTask;
    }
  }
}
