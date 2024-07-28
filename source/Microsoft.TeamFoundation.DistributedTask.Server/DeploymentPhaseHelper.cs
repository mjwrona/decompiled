// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DeploymentPhaseHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class DeploymentPhaseHelper
  {
    internal static string GetStrategyName(DeploymentStrategyType strategyType)
    {
      switch (strategyType)
      {
        case DeploymentStrategyType.RunOnce:
          return "runOnce";
        case DeploymentStrategyType.Rolling:
          return "rolling";
        case DeploymentStrategyType.Canary:
          return "canary";
        default:
          return "unknown";
      }
    }

    internal static bool ShouldUseOrchestrationV2(
      IVssRequestContext requestContext,
      DeploymentStrategyBase2 strategy,
      EnvironmentDeploymentTarget envTarget)
    {
      EnvironmentResourceType environmentResourceType = DeploymentPhaseHelper.GetEnvironmentResourceType(envTarget);
      bool flag = strategy.Type == DeploymentStrategyType.RunOnce && (environmentResourceType == EnvironmentResourceType.Kubernetes || environmentResourceType == EnvironmentResourceType.Undefined) && strategy.Hooks.Count == 1 && strategy.Hooks[0].Type == DeploymentLifeCycleHookType.Deploy;
      return requestContext.IsFeatureEnabled("DistributedTask.LifeCycleHooks") && !flag;
    }

    internal static EnvironmentResourceType GetEnvironmentResourceType(
      EnvironmentDeploymentTarget envTarget)
    {
      EnvironmentResourceFilter resourceFilter = envTarget.ResourceFilter;
      if ((resourceFilter != null ? (resourceFilter.Type.HasValue ? 1 : 0) : 0) != 0)
        return envTarget.ResourceFilter.Type.Value;
      return envTarget.Resource != null ? envTarget.Resource.Type : EnvironmentResourceType.Undefined;
    }
  }
}
