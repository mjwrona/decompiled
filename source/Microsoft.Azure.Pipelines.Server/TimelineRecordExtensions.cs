// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.TimelineRecordExtensions
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.Azure.Pipelines.Server
{
  public static class TimelineRecordExtensions
  {
    public static JobState GetJobState(this TimelineRecord jobTimelineRecord)
    {
      TimelineRecordState? state = jobTimelineRecord.State;
      if (state.HasValue)
      {
        switch (state.GetValueOrDefault())
        {
          case TimelineRecordState.Pending:
            return JobState.Waiting;
          case TimelineRecordState.InProgress:
            return JobState.Running;
          case TimelineRecordState.Completed:
            return JobState.Completed;
        }
      }
      return JobState.Running;
    }

    public static JobResult? GetJobResult(this TimelineRecord jobTimelineRecord)
    {
      TimelineRecordState? state = jobTimelineRecord.State;
      TimelineRecordState timelineRecordState = TimelineRecordState.Completed;
      if (!(state.GetValueOrDefault() == timelineRecordState & state.HasValue))
        return new JobResult?();
      TaskResult? result = jobTimelineRecord.Result;
      if (result.HasValue)
      {
        switch (result.GetValueOrDefault())
        {
          case TaskResult.Succeeded:
          case TaskResult.SucceededWithIssues:
            return new JobResult?(JobResult.Succeeded);
          case TaskResult.Failed:
            return new JobResult?(JobResult.Rejected);
          case TaskResult.Canceled:
            return new JobResult?(JobResult.Canceled);
          case TaskResult.Skipped:
            return new JobResult?(JobResult.Skipped);
          case TaskResult.Abandoned:
            return new JobResult?(JobResult.Failed);
        }
      }
      return new JobResult?(JobResult.Failed);
    }

    public static StageState GetStageState(
      this TimelineRecord stageTimelineRecord,
      TimelineRecord checkpointTimelineRecord)
    {
      stageTimelineRecord.CheckTypeIsStage();
      TimelineRecordExtensions.CheckCheckpointTimeRecord(checkpointTimelineRecord, stageTimelineRecord.Id);
      if (checkpointTimelineRecord != null)
      {
        Guid? nullable = !(checkpointTimelineRecord.RecordType != "Checkpoint") ? checkpointTimelineRecord.ParentId : throw new ArgumentException(PipelinesServerResources.InvalidTimelineRecordType((object) checkpointTimelineRecord.RecordType), nameof (checkpointTimelineRecord));
        Guid id = stageTimelineRecord.Id;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != id ? 1 : 0) : 0) : 1) != 0)
          throw new ArgumentException(PipelinesServerResources.TimelineRecordIsntChildOfStageRecord(), nameof (checkpointTimelineRecord));
      }
      TimelineRecordState? state1 = stageTimelineRecord.State;
      if (state1.HasValue)
      {
        switch (state1.GetValueOrDefault())
        {
          case TimelineRecordState.Pending:
            if (checkpointTimelineRecord != null)
            {
              TimelineRecordState? state2 = checkpointTimelineRecord.State;
              TimelineRecordState timelineRecordState = TimelineRecordState.InProgress;
              if (state2.GetValueOrDefault() == timelineRecordState & state2.HasValue)
                return StageState.Waiting;
            }
            return StageState.NotStarted;
          case TimelineRecordState.InProgress:
            return StageState.Running;
          case TimelineRecordState.Completed:
            return StageState.Completed;
        }
      }
      return StageState.Running;
    }

    public static StageResult? GetStageResult(
      this TimelineRecord stageTimelineRecord,
      TimelineRecord checkpointTimelineRecord)
    {
      stageTimelineRecord.CheckTypeIsStage();
      TimelineRecordExtensions.CheckCheckpointTimeRecord(checkpointTimelineRecord, stageTimelineRecord.Id);
      TimelineRecordState? state = stageTimelineRecord.State;
      TimelineRecordState timelineRecordState = TimelineRecordState.Completed;
      if (!(state.GetValueOrDefault() == timelineRecordState & state.HasValue))
        return new StageResult?();
      TaskResult? result1 = stageTimelineRecord.Result;
      if (result1.HasValue)
      {
        switch (result1.GetValueOrDefault())
        {
          case TaskResult.Succeeded:
          case TaskResult.SucceededWithIssues:
            return new StageResult?(StageResult.Succeeded);
          case TaskResult.Failed:
            if (checkpointTimelineRecord != null)
            {
              TaskResult? result2 = checkpointTimelineRecord.Result;
              TaskResult taskResult = TaskResult.Failed;
              if (result2.GetValueOrDefault() == taskResult & result2.HasValue)
                return new StageResult?(StageResult.Rejected);
            }
            return new StageResult?(StageResult.Failed);
          case TaskResult.Canceled:
            return new StageResult?(StageResult.Canceled);
          case TaskResult.Skipped:
            return new StageResult?(StageResult.Skipped);
          case TaskResult.Abandoned:
            return new StageResult?(StageResult.Failed);
        }
      }
      return new StageResult?(StageResult.Failed);
    }

    public static void CheckTypeIsStage(this TimelineRecord stageTimelineRecord)
    {
      ArgumentUtility.CheckForNull<TimelineRecord>(stageTimelineRecord, nameof (stageTimelineRecord));
      if (stageTimelineRecord.RecordType != "Stage")
        throw new ArgumentException(PipelinesServerResources.InvalidTimelineRecordType((object) stageTimelineRecord.RecordType), nameof (stageTimelineRecord));
    }

    private static void CheckCheckpointTimeRecord(
      TimelineRecord checkpointTimelineRecord,
      Guid parentId)
    {
      if (checkpointTimelineRecord == null)
        return;
      Guid? nullable = !(checkpointTimelineRecord.RecordType != "Checkpoint") ? checkpointTimelineRecord.ParentId : throw new ArgumentException(PipelinesServerResources.InvalidTimelineRecordType((object) checkpointTimelineRecord.RecordType), nameof (checkpointTimelineRecord));
      Guid guid = parentId;
      if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != guid ? 1 : 0) : 0) : 1) != 0)
        throw new ArgumentException(PipelinesServerResources.TimelineRecordIsntChildOfStageRecord(), nameof (checkpointTimelineRecord));
    }
  }
}
