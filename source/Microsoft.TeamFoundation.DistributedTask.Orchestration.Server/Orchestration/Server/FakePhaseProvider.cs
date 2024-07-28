// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.FakePhaseProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public class FakePhaseProvider : IPhaseProviderExtension, IPhaseProvider
  {
    private Dictionary<string, Tuple<ProviderPhaseRequest, Dictionary<string, JobInstance>>> m_runningPhases = new Dictionary<string, Tuple<ProviderPhaseRequest, Dictionary<string, JobInstance>>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public string Provider => "fake";

    public Task StartPhaseAsync(
      IVssRequestContext requestContext,
      ProviderPhaseRequest request,
      PhaseExecutionContext phaseExecutionContext)
    {
      this.m_runningPhases[request.PhaseOrchestrationId] = new Tuple<ProviderPhaseRequest, Dictionary<string, JobInstance>>(request, new Dictionary<string, JobInstance>());
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.QueueJob), (object) this.m_runningPhases[request.PhaseOrchestrationId], DateTime.UtcNow.AddSeconds(2.0), 0));
      return Task.CompletedTask;
    }

    public Task CancelPhaseAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      string reason)
    {
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.CancelJobs), (object) this.m_runningPhases[phaseOrchestrationId], DateTime.UtcNow.AddSeconds(2.0), 0));
      return Task.CompletedTask;
    }

    public Task JobCompletedAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      JobInstance job)
    {
      Tuple<ProviderPhaseRequest, Dictionary<string, JobInstance>> runningPhase = this.m_runningPhases[phaseOrchestrationId];
      foreach (KeyValuePair<string, JobInstance> keyValuePair in runningPhase.Item2)
      {
        if (string.Equals(keyValuePair.Key, job.Identifier, StringComparison.OrdinalIgnoreCase))
        {
          keyValuePair.Value.State = job.State;
          keyValuePair.Value.FinishTime = job.FinishTime;
          keyValuePair.Value.Result = job.Result;
        }
      }
      if (runningPhase.Item2.Any<KeyValuePair<string, JobInstance>>((Func<KeyValuePair<string, JobInstance>, bool>) (x => x.Value.State != PipelineState.Completed)))
        requestContext.TraceAlways(0, "DistributedTask", "Running....");
      else
        requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.CompletePhase), (object) this.m_runningPhases[phaseOrchestrationId], DateTime.UtcNow.AddSeconds(2.0), 0));
      return Task.CompletedTask;
    }

    public Task JobStartedAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      JobInstance job,
      JobStartedEventData data)
    {
      requestContext.TraceAlways(0, "DistributedTask", phaseOrchestrationId + ": Job " + job.Identifier + " started.");
      return Task.CompletedTask;
    }

    private void QueueJob(IVssRequestContext requestContext, object taskArgs)
    {
      Tuple<ProviderPhaseRequest, Dictionary<string, JobInstance>> tuple = taskArgs as Tuple<ProviderPhaseRequest, Dictionary<string, JobInstance>>;
      ProviderPhaseRequest request = tuple.Item1;
      TaskHub taskhub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "build");
      int num = 0;
      while (num++ < 10)
      {
        Job job = new Job();
        job.Name = string.Format("ProviderJob_{0}", (object) num);
        job.DisplayName = string.Format("Server Job {0}", (object) num);
        job.Target = (PhaseTarget) new ServerTarget();
        job.Variables.Add((IVariable) new Variable()
        {
          Name = "hello",
          Value = "world"
        });
        job.Variables.Add((IVariable) new Variable()
        {
          Name = "secret_hello",
          Value = "secret_world",
          Secret = true
        });
        IList<JobStep> steps = job.Steps;
        TaskStep taskStep = new TaskStep();
        taskStep.DisplayName = "sleep 1 min";
        taskStep.Name = "delay";
        taskStep.Reference = new TaskStepDefinitionReference()
        {
          Id = new Guid("28782B92-5E8E-4458-9751-A71CD1492BAE"),
          Version = "1.1.6"
        };
        taskStep.Inputs.Add("delayForMinutes", "1");
        steps.Add((JobStep) taskStep);
        Job serverJob = job;
        JobInstance jobInstance = requestContext.RunSynchronously<JobInstance>((Func<Task<JobInstance>>) (() => taskhub.QueueProviderJobAsync(requestContext, request.Project.Id, request.PlanId, request.Stage.Name, request.Phase.Name, request.Phase.Attempt, new JobInstance(serverJob, 1))));
        tuple.Item2[jobInstance.Identifier] = jobInstance;
      }
    }

    private void CancelJobs(IVssRequestContext requestContext, object taskArgs)
    {
      Tuple<ProviderPhaseRequest, Dictionary<string, JobInstance>> tuple = taskArgs as Tuple<ProviderPhaseRequest, Dictionary<string, JobInstance>>;
      ProviderPhaseRequest request = tuple.Item1;
      foreach (KeyValuePair<string, JobInstance> keyValuePair in tuple.Item2)
      {
        KeyValuePair<string, JobInstance> job = keyValuePair;
        if (job.Value.State != PipelineState.Completed)
        {
          job.Value.State = PipelineState.Canceling;
          TaskHub taskhub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "build");
          requestContext.RunSynchronously<JobInstance>((Func<Task<JobInstance>>) (() => taskhub.CancelProviderJobAsync(requestContext, request.Project.Id, request.PlanId, request.Stage.Name, request.Phase.Name, request.Phase.Attempt, job.Value)));
        }
      }
    }

    private void CompletePhase(IVssRequestContext requestContext, object taskArgs)
    {
      Tuple<ProviderPhaseRequest, Dictionary<string, JobInstance>> tuple = taskArgs as Tuple<ProviderPhaseRequest, Dictionary<string, JobInstance>>;
      ProviderPhaseRequest request = tuple.Item1;
      TaskResult result = TaskResult.Succeeded;
      foreach (KeyValuePair<string, JobInstance> keyValuePair in tuple.Item2)
        result = PipelineUtilities.MergeResult(result, keyValuePair.Value.Result.GetValueOrDefault());
      TaskHub taskhub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "build");
      requestContext.RunSynchronously((Func<Task>) (() => taskhub.CompleteProviderPhaseAsync(requestContext, request.Project.Id, request.PlanId, request.Stage.Name, request.Phase.Name, request.Phase.Attempt, result)));
    }

    public ValidationResult Validate(PipelineBuildContext context, ProviderPhase phase) => new ValidationResult();
  }
}
