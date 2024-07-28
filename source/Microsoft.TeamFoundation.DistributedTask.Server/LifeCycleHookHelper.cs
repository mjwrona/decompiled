// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.LifeCycleHookHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class LifeCycleHookHelper
  {
    internal static JobInstance CreateJob(RunDeploymentLifeCycleHookInput input)
    {
      Job job = new Job()
      {
        Name = input.HookInstaceName,
        DisplayName = input.HookInstaceDisplayName,
        TimeoutInMinutes = LifeCycleHookHelper.GetJobTimeOutInMinutes(input.LifeCycleHook),
        CancelTimeoutInMinutes = LifeCycleHookHelper.GetJobCancelTimeOutInMinutes(input.LifeCycleHook)
      };
      foreach (Step step in input.LifeCycleHook.Steps)
      {
        if (step.Type == StepType.Task)
          job.Steps.Add((JobStep) (step as TaskStep));
      }
      if (job.Variables is List<IVariable> variables)
        variables.AddRange((IEnumerable<IVariable>) input.Variables);
      job.Target = input.LifeCycleHook.Target;
      return new JobInstance(job, input.Attempt);
    }

    internal static string GetLifeCycleHookOrchestrationId(RunDeploymentLifeCycleHookInput input)
    {
      string phaseInstanceName = new PipelineIdGenerator().GetPhaseInstanceName(input.Stage.Name, input.Phase.Name, input.Phase.Attempt);
      return string.Format("{0}.{1}.{2}", (object) input.PlanId, (object) phaseInstanceName.ToLowerInvariant(), (object) input.HookInstaceName.ToLowerInvariant());
    }

    internal static string GetLifeCycleOrchestrationId(RunDeploymentLifeCycleInput input)
    {
      string phaseInstanceName = new PipelineIdGenerator().GetPhaseInstanceName(input.Stage.Name, input.Phase.Name, input.Phase.Attempt);
      return string.Format("{0}.{1}.{2}", (object) input.PlanId, (object) phaseInstanceName.ToLowerInvariant(), (object) input.LifeCycleInstanceName.ToLowerInvariant());
    }

    internal static string GetLifeCycleHookOrchestrationId(
      string phaseOrchestrationId,
      JobInstance job)
    {
      return phaseOrchestrationId + "." + job.Name.ToLowerInvariant();
    }

    internal static RunDeploymentLifeCycleHookInput GetRunDeploymentLifeCycleHookInput(
      RunDeploymentLifeCycleInput runCycleInput,
      DeploymentLifeCycleHookBase hook)
    {
      string str1 = runCycleInput.Version < 4 ? runCycleInput.LifeCycleInstanceName + "_" + LifeCycleHookHelper.GetDeploymentHookName(hook.Type) : string.Format(runCycleInput.LifeCycleInstanceNameFormat, (object) LifeCycleHookHelper.GetDeploymentHookName(hook.Type));
      string str2 = string.Format(runCycleInput.HookInstanceDisplayNameFormat, (object) hook.Type);
      RunDeploymentLifeCycleHookInput lifeCycleHookInput = new RunDeploymentLifeCycleHookInput();
      lifeCycleHookInput.ScopeId = runCycleInput.ScopeId;
      lifeCycleHookInput.PlanId = runCycleInput.PlanId;
      lifeCycleHookInput.PlanType = runCycleInput.PlanType;
      lifeCycleHookInput.Stage = runCycleInput.Stage;
      lifeCycleHookInput.Phase = runCycleInput.Phase;
      lifeCycleHookInput.HookInstaceName = str1;
      lifeCycleHookInput.Attempt = runCycleInput.Attempt;
      lifeCycleHookInput.HookInstaceDisplayName = str2;
      lifeCycleHookInput.LifeCycleHook = hook;
      lifeCycleHookInput.Version = runCycleInput.Version;
      if (!(lifeCycleHookInput.Variables is List<IVariable> variables))
        return lifeCycleHookInput;
      variables.AddRange((IEnumerable<IVariable>) runCycleInput.Variables);
      return lifeCycleHookInput;
    }

    internal static string GetDeploymentHookName(DeploymentLifeCycleHookType hookType)
    {
      switch (hookType)
      {
        case DeploymentLifeCycleHookType.Deploy:
          return "Deploy";
        case DeploymentLifeCycleHookType.PreDeploy:
          return "PreDeploy";
        case DeploymentLifeCycleHookType.RouteTraffic:
          return "RouteTraffic";
        case DeploymentLifeCycleHookType.PostRouteTraffic:
          return "PostRouteTraffic";
        case DeploymentLifeCycleHookType.OnSuccess:
          return "OnSuccess";
        case DeploymentLifeCycleHookType.OnFailure:
          return "OnFailure";
        default:
          return "Deploy";
      }
    }

    private static int GetJobTimeOutInMinutes(DeploymentLifeCycleHookBase lifeCycleHook)
    {
      int? nullable = lifeCycleHook.Target?.TimeoutInMinutes?.GetValue()?.Value;
      return !nullable.HasValue || nullable.Value < 0 || nullable.Value >= int.MaxValue ? PipelineConstants.DefaultJobTimeoutInMinutes : nullable.Value;
    }

    private static int GetJobCancelTimeOutInMinutes(DeploymentLifeCycleHookBase lifeCycleHook)
    {
      int? nullable = lifeCycleHook.Target?.CancelTimeoutInMinutes?.GetValue()?.Value;
      return !nullable.HasValue || nullable.Value < 0 || nullable.Value >= int.MaxValue ? PipelineConstants.DefaultJobCancelTimeoutInMinutes : nullable.Value;
    }
  }
}
