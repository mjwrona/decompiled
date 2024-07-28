// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ServerTargetExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class ServerTargetExtensions
  {
    public static Microsoft.TeamFoundation.DistributedTask.Pipelines.PhaseTarget ToPipelinePhaseTarget(
      this ServerTarget target,
      IVssRequestContext requestContext,
      IDictionary<string, VariableValue> environment,
      int executionTimeout,
      int cancelTimeout)
    {
      ParallelExecutionOptions executionOptions = (ParallelExecutionOptions) null;
      IVariableMultiplierExecutionOptions multiplierOptions = target.GetMultiplierOptions();
      if (multiplierOptions != null)
        executionOptions = new ParallelExecutionOptions()
        {
          MaxConcurrency = (ExpressionValue<int>) multiplierOptions.MaxConcurrency,
          Matrix = (ExpressionValue<IDictionary<string, IDictionary<string, string>>>) (IDictionary<string, IDictionary<string, string>>) multiplierOptions.GetMatrix(environment)
        };
      Microsoft.TeamFoundation.DistributedTask.Pipelines.ServerTarget pipelinePhaseTarget = new Microsoft.TeamFoundation.DistributedTask.Pipelines.ServerTarget();
      pipelinePhaseTarget.CancelTimeoutInMinutes = (ExpressionValue<int>) cancelTimeout;
      pipelinePhaseTarget.TimeoutInMinutes = (ExpressionValue<int>) executionTimeout;
      pipelinePhaseTarget.Execution = executionOptions;
      return (Microsoft.TeamFoundation.DistributedTask.Pipelines.PhaseTarget) pipelinePhaseTarget;
    }
  }
}
