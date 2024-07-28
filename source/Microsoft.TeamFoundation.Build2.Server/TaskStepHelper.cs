// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TaskStepHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class TaskStepHelper
  {
    private const string TraceLayer = "TaskStepHelper";

    public static IEnumerable<TaskStep> GetTaskSteps(
      IVssRequestContext requestContext,
      IOrchestrationProcess orchestration,
      Func<Guid, bool> taskIdFilter,
      string nameofProcess)
    {
      if (orchestration is PipelineProcess pipelineProcess)
        return pipelineProcess.Stages.SelectMany<Stage, PhaseNode>((Func<Stage, IEnumerable<PhaseNode>>) (s => (IEnumerable<PhaseNode>) s.Phases)).OfType<Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase>().SelectMany<Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase, Step>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase, IEnumerable<Step>>) (p => (IEnumerable<Step>) p.Steps)).OfType<TaskStep>().Where<TaskStep>((Func<TaskStep, bool>) (x => taskIdFilter(x.Reference.Id)));
      requestContext.TraceError(nameof (TaskStepHelper), string.Format("{0} unknown orchestration type found: {1}", (object) nameofProcess, (object) orchestration.ProcessType));
      return Enumerable.Empty<TaskStep>();
    }
  }
}
