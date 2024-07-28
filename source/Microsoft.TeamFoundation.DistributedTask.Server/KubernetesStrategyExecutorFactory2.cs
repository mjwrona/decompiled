// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.KubernetesStrategyExecutorFactory2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class KubernetesStrategyExecutorFactory2
  {
    public static IStrategyExecutor2 GetDeploymentStrategyExecutor(
      DeploymentStrategyBase2 strategy,
      ProviderPhase providerPhase)
    {
      switch (strategy.Type)
      {
        case DeploymentStrategyType.RunOnce:
          return (IStrategyExecutor2) new KubernetesRunOnceStrategyExecutor2((RunOnceDeploymentStrategy2) strategy, providerPhase);
        case DeploymentStrategyType.Canary:
          return (IStrategyExecutor2) new KubernetesCanaryStrategyExecutor((CanaryDeploymentStrategy) strategy, providerPhase);
        default:
          throw new NotSupportedException(TaskResources.InvalidDeploymentStrategy((object) strategy.Type, (object) "Kubernetes"));
      }
    }
  }
}
