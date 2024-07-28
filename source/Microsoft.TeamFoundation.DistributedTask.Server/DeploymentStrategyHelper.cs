// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DeploymentStrategyHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class DeploymentStrategyHelper
  {
    internal static DeploymentStrategyBase2 ProcessAndGetStrategy(
      IVssRequestContext requestContext,
      Guid projectId,
      EnvironmentDeploymentTarget environmentTarget,
      PhaseTarget providerPhaseTarget,
      Dictionary<string, JToken> strategy,
      string phaseName,
      IResourceStore resourceStore,
      ValidationResult result)
    {
      DeploymentStrategyBase2 strategy1 = DeploymentStrategyBuilder.Build(resourceStore, strategy, result, requestContext.IsFeatureEnabled("DistributedTask.DeploymentJobAllowTaskMinorVersion"), requestContext.IsFeatureEnabled("DistributedTask.FixForDownloadBuildTaskNotResolved"));
      if (strategy1.Hooks.Any<DeploymentLifeCycleHookBase>())
      {
        foreach (DeploymentLifeCycleHookBase hook in (IEnumerable<DeploymentLifeCycleHookBase>) strategy1.Hooks)
        {
          if (hook.Target == null)
            hook.Target = providerPhaseTarget;
          if (hook.Steps.Any<Step>())
            result.Errors.AddRange<PipelineValidationError, IList<PipelineValidationError>>((IEnumerable<PipelineValidationError>) TaskStepProcessor.ResolveStepsAndAutoFillEnvironmentInputs(requestContext, projectId, (IList<Step>) hook.Steps, environmentTarget, phaseName));
        }
      }
      return strategy1;
    }
  }
}
