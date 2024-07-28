// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Extensions.RunDeploymentLifeCycleHook
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Extensions
{
  internal class RunDeploymentLifeCycleHook : 
    TaskOrchestration<TaskResult, RunDeploymentLifeCycleHookInput, object, string>
  {
    private Guid m_jobId;
    private TaskCompletionSource<JobInstance> m_inProgressJobTask;
    private readonly TaskCompletionSource<CanceledEvent> m_cancellationSource;

    public RunDeploymentLifeCycleHook()
    {
      this.m_cancellationSource = new TaskCompletionSource<CanceledEvent>();
      this.m_inProgressJobTask = new TaskCompletionSource<JobInstance>();
    }

    public Func<Exception, bool> RetryException { get; set; }

    public IJobSchedulerManager JobSchedulerManager { get; set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      context.Trace(0, TraceLevel.Info, "RunDeploymentLifeCycleHook: Received event {0}", (object) name);
      switch (name)
      {
        case "JobCompleted":
          JobInstance result = (JobInstance) input;
          Guid jobId = this.m_jobId;
          if (!result.Definition.Id.Equals(this.m_jobId))
            break;
          this.m_inProgressJobTask.TrySetResult(result);
          break;
        case "LifeCycleHookCanceled":
          this.m_cancellationSource.TrySetResult((CanceledEvent) input);
          break;
      }
    }

    public override async Task<TaskResult> RunTask(
      OrchestrationContext context,
      RunDeploymentLifeCycleHookInput input)
    {
      this.EnsureExtensions(context);
      context.Trace(0, TraceLevel.Info, "Started deployment life cycle hook orchestration for phase " + input.Phase.Name + " and life cycle hook job " + input.HookInstaceName + ".");
      TaskResult jobResult = TaskResult.Succeeded;
      bool jobCompleted = false;
      try
      {
        JobInstance job = LifeCycleHookHelper.CreateJob(input);
        this.m_jobId = PipelineUtilities.GetJobInstanceId(input.Stage.Name, input.Phase.Name, job.Definition.Name, job.Attempt);
        JobInstance queuedJob;
        if (input.Version >= 2)
        {
          Dictionary<string, string> jobMetaData = this.GetJobMetaData(input);
          queuedJob = await this.ExecuteAsync<JobInstance>(context, input, (Func<Task<JobInstance>>) (() => this.JobSchedulerManager.QueueJobAsync2(input.ScopeId, input.PlanType, input.PlanId, input.Stage.Name, input.Stage.Attempt, input.Phase.Name, input.Phase.Attempt, job, jobMetaData)));
        }
        else
          queuedJob = await this.ExecuteAsync<JobInstance>(context, input, (Func<Task<JobInstance>>) (() => this.JobSchedulerManager.QueueJobWithJobOrderAsync(input.ScopeId, input.PlanType, input.PlanId, input.Stage.Name, input.Stage.Attempt, input.Phase.Name, input.Phase.Attempt, job, input.Order)));
        Task[] tasks = new Task[2]
        {
          (Task) this.m_inProgressJobTask.Task,
          (Task) this.m_cancellationSource.Task
        };
        while (!jobCompleted)
        {
          Task completedTask = await Task.WhenAny(tasks);
          if (completedTask is Task<CanceledEvent>)
          {
            context.Trace(0, TraceLevel.Info, string.Format("Processing cancellation event. Attempting to cancel job: {0}", (object) this.m_jobId));
            queuedJob.State = PipelineState.Canceling;
            await this.ExecuteAsync(context, input, (Func<Task>) (() => this.JobSchedulerManager.CancelJobAsync(input.ScopeId, input.PlanType, input.PlanId, input.Stage.Name, input.Stage.Attempt, input.Phase.Name, input.Phase.Attempt, queuedJob)));
          }
          if (completedTask is Task<JobInstance> task)
          {
            JobInstance result1 = task.Result;
            TaskResult? result2 = result1.Result;
            int num;
            if (!result2.HasValue)
            {
              num = 2;
            }
            else
            {
              result2 = result1.Result;
              num = (int) result2.Value;
            }
            jobResult = (TaskResult) num;
            context.Trace(0, TraceLevel.Info, string.Format("Processing job completion event. Setting jobResult to: {0}", (object) jobResult));
            jobCompleted = true;
          }
          completedTask = (Task) null;
        }
        tasks = (Task[]) null;
      }
      catch (Exception ex)
      {
        context.Trace(0, TraceLevel.Error, "RunDeploymentLifeCycleHook failed with exception: " + ex.ToString());
        jobResult = TaskResult.Failed;
        if (input.Version >= 3)
          await this.UpdateErrorsInTimeline(context, input, ex.Message);
      }
      finally
      {
        context.Trace(0, TraceLevel.Info, string.Format("Completed deployment life cycle hook orchestration for phase {0} and life cycle hook job {1} with result {2}.", (object) input.Phase.Name, (object) input.HookInstaceName, (object) jobResult));
      }
      return jobResult;
    }

    protected virtual void EnsureExtensions(OrchestrationContext context) => this.JobSchedulerManager = context.CreateClient<IJobSchedulerManager>(true);

    protected Task<TResult> ExecuteAsync<TResult>(
      OrchestrationContext context,
      RunDeploymentLifeCycleHookInput input,
      Func<Task<TResult>> operation)
    {
      return context.ExecuteAsync<TResult>(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => RetryHelper.TracePhaseException(context, ex)));
    }

    private async Task UpdateErrorsInTimeline(
      OrchestrationContext context,
      RunDeploymentLifeCycleHookInput input,
      string errorMessage)
    {
      List<TimelineRecord> timelineRecords = new List<TimelineRecord>()
      {
        this.CreateErrorTimelineRecord(input, errorMessage)
      };
      await this.ExecuteAsync(context, input, (Func<Task>) (() => this.JobSchedulerManager.UpdateTimeline(input.ScopeId, input.PlanType, input.PlanId, (IList<TimelineRecord>) timelineRecords)));
    }

    private TimelineRecord CreateErrorTimelineRecord(
      RunDeploymentLifeCycleHookInput input,
      string errorMessage)
    {
      DateTime utcNow = DateTime.UtcNow;
      string hookInstaceName = input.HookInstaceName;
      PipelineIdGenerator pipelineIdGenerator = new PipelineIdGenerator();
      Guid phaseInstanceId = PipelineUtilities.GetPhaseInstanceId(input.Stage.Name, input.Phase.Name, input.Phase.Attempt);
      Guid jobInstanceId = PipelineUtilities.GetJobInstanceId(input.Stage.Name, input.Phase.Name, hookInstaceName, input.Phase.Attempt);
      return new TimelineRecord()
      {
        Attempt = input.Phase.Attempt,
        Id = jobInstanceId,
        Identifier = pipelineIdGenerator.GetJobIdentifier(input.Stage.Name, input.Phase.Name, hookInstaceName, 1),
        Name = input.HookInstaceDisplayName,
        ParentId = new Guid?(phaseInstanceId),
        RefName = input.HookInstaceName,
        RecordType = "Job",
        Result = new TaskResult?(TaskResult.Failed),
        State = new TimelineRecordState?(TimelineRecordState.Completed),
        StartTime = new DateTime?(utcNow),
        FinishTime = new DateTime?(utcNow),
        ErrorCount = new int?(1),
        Issues = {
          new Issue()
          {
            Type = IssueType.Error,
            Message = errorMessage
          }
        }
      };
    }

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunDeploymentLifeCycleHookInput input,
      Func<Task> operation)
    {
      Func<Task<object>> operation1 = (Func<Task<object>>) (async () =>
      {
        await operation();
        object obj;
        return obj;
      });
      return (Task) this.ExecuteAsync<object>(context, input, operation1);
    }

    private Dictionary<string, string> GetJobMetaData(RunDeploymentLifeCycleHookInput input) => new Dictionary<string, string>()
    {
      {
        DeploymentStrategyRunTimeConstants.JobOrder,
        input.Order.ToString()
      },
      {
        DeploymentStrategyRunTimeConstants.LifeCycleHookType,
        input.LifeCycleHook.GetType().ToString()
      }
    };
  }
}
