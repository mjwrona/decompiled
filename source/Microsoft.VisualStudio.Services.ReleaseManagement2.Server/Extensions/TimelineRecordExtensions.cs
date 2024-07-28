// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.TimelineRecordExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class TimelineRecordExtensions
  {
    private static readonly Dictionary<TimelineRecordState, Func<TaskResult?, TaskStatus>> TimelineRecordStateMap = new Dictionary<TimelineRecordState, Func<TaskResult?, TaskStatus>>()
    {
      {
        TimelineRecordState.Pending,
        (Func<TaskResult?, TaskStatus>) (r => TaskStatus.Pending)
      },
      {
        TimelineRecordState.InProgress,
        (Func<TaskResult?, TaskStatus>) (r => TaskStatus.InProgress)
      },
      {
        TimelineRecordState.Completed,
        new Func<TaskResult?, TaskStatus>(TimelineRecordExtensions.GetStatusFromTaskResult)
      }
    };
    private static readonly Dictionary<TaskResult, TaskStatus> TaskResultMap = new Dictionary<TaskResult, TaskStatus>()
    {
      {
        TaskResult.Abandoned,
        TaskStatus.Canceled
      },
      {
        TaskResult.Canceled,
        TaskStatus.Canceled
      },
      {
        TaskResult.Failed,
        TaskStatus.Failed
      },
      {
        TaskResult.Skipped,
        TaskStatus.Skipped
      },
      {
        TaskResult.Succeeded,
        TaskStatus.Succeeded
      },
      {
        TaskResult.SucceededWithIssues,
        TaskStatus.PartiallySucceeded
      }
    };

    public static ReleaseTask ToReleaseTask(this TimelineRecord timelineRecord)
    {
      if (timelineRecord == null)
        throw new ArgumentNullException(nameof (timelineRecord));
      int num = 0;
      if (timelineRecord.Log != null)
        num = timelineRecord.Log.Id;
      ReleaseTask releaseTask = new ReleaseTask()
      {
        Id = num,
        TimelineRecordId = timelineRecord.Id,
        Name = timelineRecord.Name,
        DateStarted = timelineRecord.StartTime,
        DateEnded = timelineRecord.FinishTime,
        StartTime = timelineRecord.StartTime,
        FinishTime = timelineRecord.FinishTime,
        Status = TimelineRecordExtensions.ConvertToTaskStatus(timelineRecord.State, timelineRecord.Result),
        Rank = timelineRecord.Order,
        PercentComplete = timelineRecord.PercentComplete,
        AgentName = timelineRecord.WorkerName,
        ResultCode = timelineRecord.ResultCode
      };
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.Issue issue1 in timelineRecord.Issues)
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.Issue issue = issue1;
        if (issue != null)
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Issue taskIssue = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Issue()
          {
            Message = issue.Message,
            IssueType = issue.Type.ToString()
          };
          IDictionary<string, string> data = issue.Data;
          if (data != null)
            data.Keys.ForEach<string>((Action<string>) (key => taskIssue.Data[key] = issue.Data[key]));
          releaseTask.Issues.Add(taskIssue);
        }
      }
      if (timelineRecord.Task != null)
        releaseTask.Task = new WorkflowTaskReference()
        {
          Id = timelineRecord.Task.Id,
          Name = timelineRecord.Task.Name,
          Version = timelineRecord.Task.Version
        };
      return releaseTask;
    }

    public static IList<ReleaseDeployPhase> ToReleaseDeployPhases(
      this IList<TimelineRecord> timelineRecords,
      Guid planId)
    {
      List<ReleaseDeployPhase> releaseDeployPhases = new List<ReleaseDeployPhase>();
      foreach (TimelineRecord timelineRecord in (IEnumerable<TimelineRecord>) timelineRecords.Where<TimelineRecord>((Func<TimelineRecord, bool>) (x => x.RecordType == "Phase")).OrderBy<TimelineRecord, int?>((Func<TimelineRecord, int?>) (x => x.Order)))
      {
        TimelineRecord phaseRecord = timelineRecord;
        ReleaseDeployPhase releaseDeployPhase = new ReleaseDeployPhase()
        {
          PhaseId = phaseRecord.Id.ToString(),
          Name = phaseRecord.Name,
          Status = TimelineRecordExtensions.ToReleaseDeployPhaseStatus(phaseRecord.State, phaseRecord.Result),
          RunPlanId = new Guid?(planId),
          Rank = phaseRecord.Order.GetValueOrDefault(),
          PhaseType = DeployPhaseTypes.AgentBasedDeployment
        };
        foreach (TimelineRecord job in timelineRecords.Where<TimelineRecord>((Func<TimelineRecord, bool>) (x =>
        {
          if (!(x.RecordType == "Job"))
            return false;
          Guid? parentId = x.ParentId;
          Guid id = phaseRecord.Id;
          if (!parentId.HasValue)
            return false;
          return !parentId.HasValue || parentId.GetValueOrDefault() == id;
        })))
          releaseDeployPhase.DeploymentJobs.Add(TimelineRecordExtensions.GetJobRecord((IEnumerable<TimelineRecord>) timelineRecords, job));
        releaseDeployPhase.ErrorLog = TimelineRecordExtensions.GetErrorLog((IEnumerable<TimelineRecord>) new TimelineRecord[1]
        {
          phaseRecord
        });
        releaseDeployPhases.Add(releaseDeployPhase);
      }
      return (IList<ReleaseDeployPhase>) releaseDeployPhases;
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Reviewed.")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Callers don't need to know")]
    public static IEnumerable<DeploymentJob> ToDeployStepRecords(
      this IEnumerable<TimelineRecord> timelineRecords,
      out string phaseErrorLog,
      int top = 0)
    {
      List<DeploymentJob> deployStepRecords = new List<DeploymentJob>();
      IEnumerable<TimelineRecord> source = timelineRecords.Where<TimelineRecord>((Func<TimelineRecord, bool>) (record => record.RecordType == "Job"));
      if (!source.Any<TimelineRecord>())
      {
        DeploymentJob deploymentJob = new DeploymentJob();
        deploymentJob.Tasks.AddRange<ReleaseTask, IList<ReleaseTask>>(timelineRecords.ToOrderedReleaseTasks());
        ReleaseTask jobTask = (ReleaseTask) null;
        if (TimelineRecordExtensions.TryGetJobTaskData(timelineRecords, out jobTask))
        {
          deploymentJob.Job = jobTask;
          deployStepRecords.Add(deploymentJob);
        }
      }
      else if (top > 0)
      {
        foreach (TimelineRecord job in source.OrderByDescending<TimelineRecord, int?>((Func<TimelineRecord, int?>) (j => j.Order)).Take<TimelineRecord>(top))
        {
          DeploymentJob jobRecord = TimelineRecordExtensions.GetJobRecord(timelineRecords, job);
          deployStepRecords.Add(jobRecord);
        }
      }
      else
      {
        foreach (TimelineRecord job in source)
        {
          DeploymentJob jobRecord = TimelineRecordExtensions.GetJobRecord(timelineRecords, job);
          deployStepRecords.Add(jobRecord);
        }
      }
      IEnumerable<TimelineRecord> timelineRecords1 = timelineRecords.Where<TimelineRecord>((Func<TimelineRecord, bool>) (record => record.RecordType == "Phase"));
      phaseErrorLog = TimelineRecordExtensions.GetErrorLog(timelineRecords1);
      return (IEnumerable<DeploymentJob>) deployStepRecords;
    }

    private static string GetErrorLog(IEnumerable<TimelineRecord> timelineRecords)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.Issue issue in timelineRecords.SelectMany<TimelineRecord, Microsoft.TeamFoundation.DistributedTask.WebApi.Issue>((Func<TimelineRecord, IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue>>) (x => (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue>) x.Issues)))
      {
        if (!string.IsNullOrEmpty(issue?.Message))
          stringBuilder.AppendLine(issue.Message);
      }
      return stringBuilder.ToString();
    }

    private static DeployPhaseStatus ToReleaseDeployPhaseStatus(
      TimelineRecordState? state,
      TaskResult? result)
    {
      if (state.HasValue)
      {
        switch (state.GetValueOrDefault())
        {
          case TimelineRecordState.Pending:
            return DeployPhaseStatus.NotStarted;
          case TimelineRecordState.InProgress:
            return DeployPhaseStatus.InProgress;
          case TimelineRecordState.Completed:
            return TimelineRecordExtensions.ToReleaseDeployPhaseStatus(result);
        }
      }
      return DeployPhaseStatus.Undefined;
    }

    private static DeployPhaseStatus ToReleaseDeployPhaseStatus(TaskResult? result)
    {
      if (result.HasValue)
      {
        switch (result.GetValueOrDefault())
        {
          case TaskResult.Succeeded:
            return DeployPhaseStatus.Succeeded;
          case TaskResult.SucceededWithIssues:
            return DeployPhaseStatus.PartiallySucceeded;
          case TaskResult.Failed:
            return DeployPhaseStatus.Failed;
          case TaskResult.Canceled:
            return DeployPhaseStatus.Canceled;
          case TaskResult.Skipped:
            return DeployPhaseStatus.Skipped;
          case TaskResult.Abandoned:
            return DeployPhaseStatus.Canceled;
        }
      }
      return DeployPhaseStatus.Undefined;
    }

    private static DeploymentJob GetJobRecord(
      IEnumerable<TimelineRecord> timelineRecords,
      TimelineRecord job)
    {
      IEnumerable<TimelineRecord> timelineRecords1 = timelineRecords.Where<TimelineRecord>((Func<TimelineRecord, bool>) (t =>
      {
        if (!(t.RecordType != "Job"))
          return false;
        Guid? parentId = t.ParentId;
        Guid id = job.Id;
        if (!parentId.HasValue)
          return false;
        return !parentId.HasValue || parentId.GetValueOrDefault() == id;
      }));
      DeploymentJob jobRecord = new DeploymentJob();
      jobRecord.Job = job.ToReleaseTask();
      jobRecord.Tasks.AddRange<ReleaseTask, IList<ReleaseTask>>(timelineRecords1.ToOrderedReleaseTasks());
      return jobRecord;
    }

    private static bool TryGetJobTaskData(
      IEnumerable<TimelineRecord> timelineRecords,
      out ReleaseTask jobTask)
    {
      jobTask = (ReleaseTask) null;
      TimelineRecord timelineRecord = timelineRecords.FirstOrDefault<TimelineRecord>((Func<TimelineRecord, bool>) (t => t.RecordType != "Job"));
      if (timelineRecord != null)
      {
        Guid? parentId = timelineRecord.ParentId;
        if (parentId.HasValue)
        {
          ref ReleaseTask local = ref jobTask;
          ReleaseTask releaseTask = new ReleaseTask();
          parentId = timelineRecord.ParentId;
          releaseTask.TimelineRecordId = parentId.Value;
          local = releaseTask;
          return true;
        }
      }
      return false;
    }

    private static IEnumerable<ReleaseTask> ToOrderedReleaseTasks(
      this IEnumerable<TimelineRecord> timelineRecords)
    {
      IEnumerable<ReleaseTask> source = timelineRecords.Select<TimelineRecord, ReleaseTask>((Func<TimelineRecord, ReleaseTask>) (record => record.ToReleaseTask()));
      return source.All<ReleaseTask>((Func<ReleaseTask, bool>) (task => task.Rank.HasValue)) ? (IEnumerable<ReleaseTask>) source.OrderBy<ReleaseTask, int>((Func<ReleaseTask, int>) (task => task.Rank.Value)) : (IEnumerable<ReleaseTask>) source.OrderBy<ReleaseTask, DateTime>((Func<ReleaseTask, DateTime>) (task => task.StartTime ?? DateTime.MinValue));
    }

    private static TaskStatus ConvertToTaskStatus(
      TimelineRecordState? timelineRecordState,
      TaskResult? taskResult)
    {
      return !timelineRecordState.HasValue ? TaskStatus.Unknown : TimelineRecordExtensions.TimelineRecordStateMap[timelineRecordState.Value](taskResult);
    }

    private static TaskStatus GetStatusFromTaskResult(TaskResult? result) => !result.HasValue ? TaskStatus.Unknown : TimelineRecordExtensions.TaskResultMap[result.Value];
  }
}
