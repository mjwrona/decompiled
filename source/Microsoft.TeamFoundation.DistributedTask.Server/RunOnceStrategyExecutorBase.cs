// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.RunOnceStrategyExecutorBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal abstract class RunOnceStrategyExecutorBase : IStrategyExecutor
  {
    private TaskResult? m_phaseResult;
    private RunOnceDeploymentStrategy m_strategy;
    private ProviderPhase m_providerPhase;
    private bool m_jobQueued;
    private const string c_defaultDisplayName = "Deploy";

    public RunOnceStrategyExecutorBase(RunOnceDeploymentStrategy strategy, ProviderPhase phase)
    {
      this.m_strategy = strategy;
      this.m_providerPhase = phase;
      this.m_jobQueued = false;
    }

    public void Initialize()
    {
    }

    public IList<JobInstance> GetNextJobs()
    {
      List<JobInstance> nextJobs = new List<JobInstance>();
      if (!this.m_jobQueued)
      {
        JobInstance job = this.CreateJob();
        nextJobs.Add(job);
        this.m_jobQueued = true;
      }
      return (IList<JobInstance>) nextJobs;
    }

    public void OnJobStarted(JobStartedEventData eventData)
    {
    }

    public void OnJobCompleted(Guid jobId, TaskResult result) => this.m_phaseResult = new TaskResult?(result);

    public bool IsDeploymentComplete() => this.m_phaseResult.HasValue;

    public TaskResult? GetDeploymentResult(out IList<TimelineRecord> timelineRecords)
    {
      timelineRecords = (IList<TimelineRecord>) null;
      return this.m_phaseResult;
    }

    private JobInstance CreateJob()
    {
      Job job = new Job()
      {
        Name = this.m_providerPhase.Name,
        DisplayName = string.IsNullOrWhiteSpace(this.m_providerPhase.DisplayName) ? "Deploy" : this.m_providerPhase.DisplayName
      };
      DeploymentStrategyBaseAction strategyBaseAction = this.m_strategy.Actions.First<DeploymentStrategyBaseAction>((Func<DeploymentStrategyBaseAction, bool>) (a => a.Type == DeploymentStrategyActionType.Deploy));
      if (strategyBaseAction != null)
      {
        foreach (Step step in (IEnumerable<Step>) strategyBaseAction.Steps)
        {
          if (step.Type == StepType.Task)
            job.Steps.Add((JobStep) (step as TaskStep));
        }
      }
      job.Target = this.m_providerPhase.Target;
      return new JobInstance(job);
    }
  }
}
