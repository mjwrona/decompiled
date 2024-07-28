// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.PipelineRuntimeService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Orchestration.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal sealed class PipelineRuntimeService : IPipelineRuntimeService, IVssFrameworkService
  {
    public async Task<IList<StageAttempt>> RetryPipelineAsync(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeIdentifier,
      Guid planId)
    {
      return await this.RetryStagesAsync(requestContext, hubName, scopeIdentifier, planId, (IList<string>) null);
    }

    public async Task<IList<StageAttempt>> RetryStagesAsync(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeIdentifier,
      Guid planId,
      IList<string> stageNames,
      bool forceRetryAllJobs = false,
      bool retryDependencies = true)
    {
      TaskHub hub = this.GetHub(requestContext, hubName);
      TaskOrchestrationPlan plan = await hub.GetPlanAsync(requestContext, scopeIdentifier, planId);
      if (plan == null)
        throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
      IList<StageAttempt> stageAttemptList;
      using (requestContext.CreateOrchestrationIdScope(plan.GetOrchestrationId()))
      {
        if (stageNames == null || stageNames.Count == 0)
        {
          TaskResult? nullable = plan.State == TaskOrchestrationPlanState.Completed ? plan.Result : throw new InvalidPipelineOperationException(TaskResources.InvalidPipelineRetryNotCompleted((object) (plan.Owner?.Name ?? plan.PlanId.ToString())));
          TaskResult taskResult1 = TaskResult.Succeeded;
          if (!(nullable.GetValueOrDefault() == taskResult1 & nullable.HasValue))
          {
            TaskResult? result = plan.Result;
            TaskResult taskResult2 = TaskResult.SucceededWithIssues;
            if (!(result.GetValueOrDefault() == taskResult2 & result.HasValue))
              goto label_10;
          }
          throw new InvalidPipelineOperationException(TaskResources.InvalidPipelineRetryDidNotFail((object) (plan.Owner?.Name ?? plan.PlanId.ToString())));
        }
label_10:
        Timeline timelineAsync = await hub.GetTimelineAsync(requestContext, scopeIdentifier, planId, Guid.Empty, includeRecords: true);
        PipelineProcess process = plan.GetProcess<PipelineProcess>();
        IList<string> stringList = stageNames;
        if ((stringList != null ? (stringList.Count > 0 ? 1 : 0) : 0) != 0 && plan.State != TaskOrchestrationPlanState.Completed)
        {
          HashSet<string> stagesToRetry = process.Stages.Where<Stage>((Func<Stage, bool>) (x => stageNames.Contains<string>(x.Name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))).Select<Stage, string>((Func<Stage, string>) (x => x.Name)).ToHashSet<string>();
          bool flag;
          do
          {
            flag = false;
            foreach (Stage stage in process.Stages.Where<Stage>((Func<Stage, bool>) (x => !stagesToRetry.Contains<string>(x.Name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))))
            {
              if (stage.DependsOn.Any<string>((Func<string, bool>) (x => stagesToRetry.Contains<string>(x, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))))
              {
                flag = true;
                stagesToRetry.Add(stage.Name);
              }
            }
          }
          while (flag);
          List<string> values = new List<string>();
          foreach (string str in stagesToRetry)
          {
            string stageName = str;
            TimelineRecord timelineRecord = timelineAsync.Records.SingleOrDefault<TimelineRecord>((Func<TimelineRecord, bool>) (x => string.Equals(stageName, x.RefName, StringComparison.OrdinalIgnoreCase) && x.RecordType == "Stage"));
            TimelineRecordState? state = timelineRecord.State;
            TimelineRecordState timelineRecordState = TimelineRecordState.InProgress;
            if (state.GetValueOrDefault() == timelineRecordState & state.HasValue)
              values.Add(string.IsNullOrWhiteSpace(timelineRecord.Name) ? timelineRecord.RefName : timelineRecord.Name);
          }
          if (values.Count > 0)
            throw new InvalidPipelineOperationException("Unable to retry stage(s): " + string.Join(", ", (IEnumerable<string>) stageNames) + ", because previous attempts are still running: " + string.Join(", ", (IEnumerable<string>) values));
        }
        Tuple<IList<StageAttempt>, IList<StageAttempt>> attemptTuple = this.PrepareStageAttempts(requestContext, hubName, scopeIdentifier, plan, stageNames, timelineAsync, forceRetryAllJobs, retryDependencies);
        IList<StageAttempt> allAttempts = attemptTuple.Item1;
        IList<StageAttempt> newStageAttempts = attemptTuple.Item2;
        if (newStageAttempts.Count > 0)
        {
          Guid requestedBy = requestContext.GetUserId(true);
          OrchestrationService service = requestContext.GetService<OrchestrationService>();
          OrchestrationSession orchestrationSession = (OrchestrationSession) null;
          OrchestrationSessionNotFoundException innerException = (OrchestrationSessionNotFoundException) null;
          try
          {
            orchestrationSession = service.GetOrchestrationSession(requestContext, hubName, planId.ToString());
          }
          catch (OrchestrationSessionNotFoundException ex)
          {
            innerException = ex;
          }
          if (plan.State == TaskOrchestrationPlanState.Completed && orchestrationSession != null)
            throw new TaskOrchestrationPlanAlreadyStartedException(TaskResources.PlanAlreadyStarted((object) plan.PlanId, (object) plan.Version, (object) plan.PlanType), (Exception) innerException);
          if (plan.State == TaskOrchestrationPlanState.Completed)
          {
            await this.CreateStageAttempts(requestContext, hubName, scopeIdentifier, plan, attemptTuple, requestedBy);
            hub.StartQueuedPlan(requestContext, plan, new int?(), allAttempts);
          }
          else
          {
            if (plan.Version < 17)
              await this.CreateStageAttempts(requestContext, hubName, scopeIdentifier, plan, attemptTuple, requestedBy);
            TaskHub taskHub = hub;
            IVssRequestContext requestContext1 = requestContext;
            string orchestrationId = plan.GetOrchestrationId();
            RetryEvent eventData = new RetryEvent();
            eventData.Children = newStageAttempts.Select<StageAttempt, StageExecutionState>((Func<StageAttempt, StageExecutionState>) (x => new StageExecutionState(x))).Cast<Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.IGraphNode>();
            eventData.TargetedChildNames = stageNames;
            eventData.ForceRetrySubNodes = forceRetryAllJobs;
            eventData.RequestedBy = requestedBy;
            DateTime? fireAt = new DateTime?();
            await taskHub.RaiseOrchestrationEventAsync(requestContext1, orchestrationId, "Retry", (object) eventData, fireAt: fireAt);
          }
          requestedBy = new Guid();
        }
        stageAttemptList = allAttempts;
      }
      hub = (TaskHub) null;
      plan = (TaskOrchestrationPlan) null;
      return stageAttemptList;
    }

    private Tuple<IList<StageAttempt>, IList<StageAttempt>> PrepareStageAttempts(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeIdentifier,
      TaskOrchestrationPlan plan,
      IList<string> stageNames,
      Timeline timelineWithRecords,
      bool forceRetryAllJobs = false,
      bool retryDependencies = true)
    {
      using (requestContext.CreateOrchestrationIdScope(plan.GetOrchestrationId()))
      {
        PipelineProcess process = plan.GetProcess<PipelineProcess>();
        return new PipelineAttemptBuilder((IPipelineIdGenerator) new PipelineIdGenerator(plan.Version < 4), process, new Timeline[1]
        {
          timelineWithRecords
        }).Retry(stageNames, forceRetryAllJobs, retryDependencies);
      }
    }

    public async Task<IList<StageAttempt>> CreateStageAttempts(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeIdentifier,
      TaskOrchestrationPlan plan,
      IList<string> stageNames,
      Guid requestedBy,
      bool forceRetryAllJobs = false,
      bool retryDependencies = true)
    {
      TaskHub hub = this.GetHub(requestContext, hubName);
      IList<StageAttempt> stageAttempts;
      using (requestContext.CreateOrchestrationIdScope(plan.GetOrchestrationId()))
      {
        Timeline timelineAsync = await hub.GetTimelineAsync(requestContext, scopeIdentifier, plan.PlanId, Guid.Empty, includeRecords: true);
        Tuple<IList<StageAttempt>, IList<StageAttempt>> attemptTuple = this.PrepareStageAttempts(requestContext, hubName, scopeIdentifier, plan, stageNames, timelineAsync, forceRetryAllJobs, retryDependencies);
        await this.CreateStageAttempts(requestContext, hubName, scopeIdentifier, plan, attemptTuple, requestedBy);
        stageAttempts = attemptTuple.Item2;
      }
      return stageAttempts;
    }

    private async Task CreateStageAttempts(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeIdentifier,
      TaskOrchestrationPlan plan,
      Tuple<IList<StageAttempt>, IList<StageAttempt>> attemptTuple,
      Guid requestedBy)
    {
      using (requestContext.CreateOrchestrationIdScope(plan.GetOrchestrationId()))
      {
        TaskHub hub = this.GetHub(requestContext, hubName);
        PipelineIdGenerator idGenerator = new PipelineIdGenerator(plan.Version < 4);
        IList<StageAttempt> stageAttemptList = attemptTuple.Item1;
        IList<StageAttempt> newStageAttempts = attemptTuple.Item2;
        if (newStageAttempts.Count == 0)
          throw new ArgumentException("Retry timeline cannot be created with no new stage attempts");
        Timeline oldTimeline = (Timeline) null;
        if (!requestContext.IsFeatureEnabled("DistributedTask.UseNewPipelineAttemptForTimelineRecordsUpdated"))
          oldTimeline = await hub.GetTimelineAsync(requestContext, scopeIdentifier, plan.PlanId, Guid.Empty, includeRecords: true);
        Timeline newAttempt = new Timeline();
        newAttempt.Records.AddRange((IEnumerable<TimelineRecord>) newStageAttempts.SelectMany<StageAttempt, TimelineRecord>((Func<StageAttempt, IEnumerable<TimelineRecord>>) (x => (IEnumerable<TimelineRecord>) x.Timeline.Records)).ToList<TimelineRecord>());
        List<TimelineAttempt> list = newStageAttempts.Select<StageAttempt, TimelineAttempt>((Func<StageAttempt, TimelineAttempt>) (x => new TimelineAttempt()
        {
          Identifier = x.Stage.Identifier,
          Attempt = x.Stage.Attempt,
          RecordId = idGenerator.GetStageInstanceId(x.Stage.Name, x.Stage.Attempt)
        })).ToList<TimelineAttempt>();
        Timeline createdTimeline = (Timeline) null;
        using (TaskTrackingComponent component = hub.CreateComponent<TaskTrackingComponent>(requestContext))
          createdTimeline = await component.CreatePipelineAttemptAsync(scopeIdentifier, plan.PlanId, (IList<TimelineAttempt>) list, newAttempt, requestedBy);
        await hub.Extension.NewAttemptCreatedAsync(requestContext, plan, newStageAttempts);
        if (requestContext.IsFeatureEnabled("DistributedTask.UseNewPipelineAttemptForTimelineRecordsUpdated"))
          hub.Extension.TimelineRecordsUpdated(requestContext, plan.AsReference(), createdTimeline.AsReference(), (IEnumerable<TimelineRecord>) createdTimeline.Records);
        else
          hub.Extension.TimelineRecordsUpdated(requestContext, plan.AsReference(), oldTimeline.AsReference(), (IEnumerable<TimelineRecord>) oldTimeline.Records);
        hub = (TaskHub) null;
        newStageAttempts = (IList<StageAttempt>) null;
        oldTimeline = (Timeline) null;
        createdTimeline = (Timeline) null;
      }
    }

    public async Task<StageAttempt> GetStageAttemptAsync(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeId,
      Guid planId,
      string name,
      int attempt)
    {
      TaskHub hub = this.GetHub(requestContext, hubName);
      TaskOrchestrationPlan plan = await hub.GetPlanAsync(requestContext, scopeId, planId);
      PipelineProcess pipeline = plan != null ? plan.GetProcess<PipelineProcess>() : throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
      string stageName = name ?? PipelineConstants.DefaultJobName;
      if (pipeline.Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase))) == null)
        return (StageAttempt) null;
      Timeline timelineAttemptAsync;
      using (TaskTrackingComponent trackingComponent = hub.CreateComponent<TaskTrackingComponent>(requestContext))
        timelineAttemptAsync = await trackingComponent.GetTimelineAttemptAsync(scopeId, planId, name, attempt);
      return new PipelineAttemptBuilder((IPipelineIdGenerator) new PipelineIdGenerator(plan.Version < 4), pipeline, new Timeline[1]
      {
        timelineAttemptAsync
      }).GetStageAttempt(name, attempt);
    }

    public async Task<IList<StageAttempt>> GetAllStageAttemptsAsync(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeId,
      Guid planId,
      string name,
      IList<string> includedPhases = null)
    {
      TaskHub hub = this.GetHub(requestContext, hubName);
      TaskOrchestrationPlan plan = await hub.GetPlanAsync(requestContext, scopeId, planId);
      PipelineProcess pipeline = plan != null ? plan.GetProcess<PipelineProcess>() : throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
      string stageName = name ?? PipelineConstants.DefaultJobName;
      if (pipeline.Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase))) == null)
        return (IList<StageAttempt>) null;
      IEnumerable<Timeline> timelineAttemptsAsync;
      using (TaskTrackingComponent trackingComponent = hub.CreateComponent<TaskTrackingComponent>(requestContext))
        timelineAttemptsAsync = (IEnumerable<Timeline>) await trackingComponent.GetAllTimelineAttemptsAsync(scopeId, planId, name, includedPhases);
      return new PipelineAttemptBuilder((IPipelineIdGenerator) new PipelineIdGenerator(plan.Version < 4), pipeline, timelineAttemptsAsync.ToArray<Timeline>()).GetStageAttempts(name);
    }

    private TaskHub GetHub(IVssRequestContext requestContext, string hubName) => requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, hubName);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
