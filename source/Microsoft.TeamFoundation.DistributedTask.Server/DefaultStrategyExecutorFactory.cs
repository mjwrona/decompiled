// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DefaultStrategyExecutorFactory
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class DefaultStrategyExecutorFactory
  {
    public static IStrategyExecutor GetDeploymentStrategyExecutor(
      DeploymentStrategyBase strategy,
      ProviderPhase providerPhase)
    {
      return strategy.Type == DeploymentStrategyType.RunOnce ? (IStrategyExecutor) new DefaultRunOnceStrategyExecutor((RunOnceDeploymentStrategy) strategy, providerPhase) : throw new NotSupportedException(TaskResources.InvalidDeploymentStrategy((object) strategy.Type, (object) "Environment"));
    }
  }
}
