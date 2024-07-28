// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildOrchestratorExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class BuildOrchestratorExtensions
  {
    public static void RunPlan(
      this IBuildOrchestrator orchestrator,
      IVssRequestContext requestContext,
      Guid projectId,
      BuildData build,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPoolReference pool,
      PlanTemplateType templateType,
      IOrchestrationEnvironment environment,
      IOrchestrationProcess process,
      BuildOptions buildOptions,
      string pipelineInitializationLog = null,
      string pipelineExpandedYaml = null)
    {
      Guid requestedFor = build.RequestedFor;
      if (build.QueueOptions.HasFlag((Enum) QueueOptions.DoNotRun) || build.Reason == BuildReason.CheckInShelveset)
        orchestrator.CreatePlan(requestContext, projectId, build.OrchestrationPlan.PlanId, templateType, build, build.Uri, environment, process, buildOptions, requestedFor, pipelineInitializationLog, pipelineExpandedYaml);
      else
        orchestrator.RunPlan(requestContext, build, pool, projectId, build.OrchestrationPlan.PlanId, templateType, build.Uri, environment, process, buildOptions, requestedFor, pipelineInitializationLog: pipelineInitializationLog, pipelineExpandedYaml: pipelineExpandedYaml);
    }
  }
}
