// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentStrategyConstants
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal static class DeploymentStrategyConstants
  {
    internal const string CanaryIncrements = "increments";
    internal const string Completed = "Completed";
    internal const string Demands = "demands";
    internal const string DeploymentStrategyCanary = "canary";
    internal const string DeploymentStrategyRunOnce = "runOnce";
    internal const string DeploymentStrategyRolling = "rolling";
    internal const string Iteration = "Iteration";
    internal const string Name = "name";
    internal const string PreIteration = "PreIteration";
    internal const string Pool = "pool";
    internal const string PostIteration = "PostIteration";
    internal const string RollingDeploymentMaxParallel = "maxParallel";
    internal const string Server = "server";
    internal const string StepsPropertyName = "steps";
    internal const string StrategyDeployHookName = "deploy";
    internal const string StrategyPreDeployHookName = "preDeploy";
    internal const string StrategyRouteTrafficHookName = "routeTraffic";
    internal const string StrategyPostRouteTrafficHookName = "postRouteTraffic";
    internal const string StrategyOnSuccessOrFailureHookName = "on";
    internal const string StrategyOnSuccessHookName = "success";
    internal const string StrategyOnFailureHookName = "failure";
    internal const string VMImage = "vmImage";
    internal const string DeployAction = "deploy";
    internal const string PromoteAction = "promote";
    internal const string RejectAction = "reject";
  }
}
