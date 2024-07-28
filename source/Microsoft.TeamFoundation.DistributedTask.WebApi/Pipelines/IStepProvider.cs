// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.IStepProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  public interface IStepProvider
  {
    IList<TaskStep> GetPreSteps(IPipelineContext context, IReadOnlyList<JobStep> steps);

    Dictionary<Guid, List<TaskStep>> GetPostTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps);

    Dictionary<Guid, List<TaskStep>> GetPostTargetTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps);

    Dictionary<Guid, List<TaskStep>> GetPreTargetTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps);

    IList<TaskStep> GetPostSteps(IPipelineContext context, IReadOnlyList<JobStep> steps);

    bool ResolveStep(IPipelineContext context, JobStep step, out IList<TaskStep> resolvedSteps);

    Dictionary<Guid, List<string>> GetInputsToProvide(IPipelineContext context);
  }
}
