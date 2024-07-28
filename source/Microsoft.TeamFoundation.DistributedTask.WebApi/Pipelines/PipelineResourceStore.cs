// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineResourceStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  public class PipelineResourceStore : 
    InMemoryResourceStore<PipelineResource>,
    IPipelineStore,
    IStepProvider
  {
    private IArtifactResolver m_artifactResolver;
    private bool m_isEnabled;
    private bool m_useSystemStepsDecorator;

    public PipelineResourceStore(
      IEnumerable<PipelineResource> pipelines,
      IArtifactResolver artifactResolver = null,
      bool isEnabled = false,
      bool useSystemStepsDecorator = false)
      : base(pipelines)
    {
      this.m_artifactResolver = artifactResolver;
      this.m_isEnabled = isEnabled;
      this.m_useSystemStepsDecorator = useSystemStepsDecorator;
    }

    public IList<TaskStep> GetPreSteps(IPipelineContext context, IReadOnlyList<JobStep> steps) => (IList<TaskStep>) new List<TaskStep>();

    public Dictionary<Guid, List<TaskStep>> GetPostTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public Dictionary<Guid, List<TaskStep>> GetPostTargetTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public Dictionary<Guid, List<TaskStep>> GetPreTargetTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public Dictionary<Guid, List<string>> GetInputsToProvide(IPipelineContext context) => new Dictionary<Guid, List<string>>();

    public IList<TaskStep> GetPostSteps(IPipelineContext context, IReadOnlyList<JobStep> steps) => (IList<TaskStep>) new List<TaskStep>();

    public bool ResolveStep(
      IPipelineContext context,
      JobStep step,
      out IList<TaskStep> resolvedSteps)
    {
      resolvedSteps = (IList<TaskStep>) new List<TaskStep>();
      if (!step.IsDownloadTask())
        return false;
      if (!this.m_isEnabled)
        return true;
      IArtifactResolver artifactResolver = this.m_artifactResolver;
      return artifactResolver != null && artifactResolver.ResolveStep(context, step, out resolvedSteps);
    }
  }
}
