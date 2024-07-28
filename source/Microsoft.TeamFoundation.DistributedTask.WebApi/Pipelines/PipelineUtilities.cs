// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineUtilities
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class PipelineUtilities
  {
    public static Guid GetInstanceId(string identifier, bool preserveCase = false) => preserveCase ? TimelineRecordIdGenerator.GetId(identifier) : TimelineRecordIdGenerator.GetId(identifier?.ToLowerInvariant());

    public static string GetInstanceName(params string[] segments) => string.Join(".", ((IEnumerable<string>) segments).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))).Select<string, string>((Func<string, string>) (x => x.Trim('.'))));

    public static string GetName(string identifier)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(identifier, nameof (identifier));
      int num = identifier.LastIndexOf('.');
      return num < 0 ? identifier : identifier.Substring(num + 1);
    }

    public static Guid GetStageInstanceId(StageInstance stage, bool preserveCase = false) => PipelineUtilities.GetStageInstanceId(stage.Name, stage.Attempt, preserveCase);

    public static string GetStageIdentifier(StageInstance stage) => PipelineUtilities.GetStageIdentifier(stage.Name);

    public static string GetStageIdentifier(string stageName) => PipelineUtilities.GetStageInstanceName(stageName, 1);

    public static string GetStageInstanceName(StageInstance stage) => PipelineUtilities.GetStageInstanceName(stage.Name, stage.Attempt);

    public static Guid GetStageInstanceId(string stageName, int stageAttempt, bool preserveCase = false) => PipelineUtilities.GetInstanceId(PipelineUtilities.GetStageInstanceName(stageName, stageAttempt, true), preserveCase);

    public static string GetStageInstanceName(string stageName, int stageAttempt) => PipelineUtilities.GetStageInstanceName(stageName, stageAttempt, true);

    public static string GetStageInstanceName(
      string stageName,
      int stageAttempt,
      bool includeDefault)
    {
      if (string.IsNullOrEmpty(stageName) || !includeDefault && stageName.Equals(PipelineConstants.DefaultJobName, StringComparison.OrdinalIgnoreCase))
        return string.Empty;
      string stageInstanceName = stageName;
      if (stageAttempt > 1)
        stageInstanceName = string.Format("{0}.{1}", (object) stageName, (object) stageAttempt);
      return stageInstanceName;
    }

    public static string GetPhaseIdentifier(StageInstance stage, PhaseInstance phase) => PipelineUtilities.GetPhaseIdentifier(stage?.Name, phase.Name);

    public static string GetPhaseIdentifier(string stageName, string phaseName) => PipelineUtilities.GetPhaseInstanceName(stageName, phaseName, 1);

    public static Guid GetPhaseInstanceId(
      StageInstance stage,
      PhaseInstance phase,
      bool preserveCase = false)
    {
      return PipelineUtilities.GetPhaseInstanceId(stage?.Name, phase.Name, phase.Attempt, preserveCase);
    }

    public static Guid GetPhaseInstanceId(
      string stageName,
      string phaseName,
      int phaseAttempt,
      bool preserveCase = false)
    {
      return PipelineUtilities.GetInstanceId(PipelineUtilities.GetPhaseInstanceName(stageName, phaseName, phaseAttempt), preserveCase);
    }

    public static string GetPhaseInstanceName(StageInstance stage, PhaseInstance phase)
    {
      StringBuilder stringBuilder = new StringBuilder(PipelineUtilities.GetStageInstanceName(stage?.Name, 1, false));
      if (stringBuilder.Length > 0)
        stringBuilder.Append(".");
      stringBuilder.Append(phase.Name ?? "");
      if (phase.Attempt > 1)
        stringBuilder.Append(string.Format(".{0}", (object) phase.Attempt));
      return stringBuilder.ToString();
    }

    public static string GetPhaseInstanceName(string stageName, string phaseName, int phaseAttempt)
    {
      StringBuilder stringBuilder = new StringBuilder(PipelineUtilities.GetStageInstanceName(stageName, 1, false));
      if (stringBuilder.Length > 0)
        stringBuilder.Append(".");
      stringBuilder.Append(phaseName ?? "");
      if (phaseAttempt > 1)
        stringBuilder.Append(string.Format(".{0}", (object) phaseAttempt));
      return stringBuilder.ToString();
    }

    public static string GetJobIdentifier(
      StageInstance stage,
      PhaseInstance phase,
      JobInstance job,
      int attempt = 1,
      int checkRerunAttempt = 1)
    {
      return PipelineUtilities.GetJobIdentifier(stage?.Name, phase.Name, job.Name, attempt, checkRerunAttempt);
    }

    public static string GetJobIdentifier(
      string stageName,
      string phaseName,
      string jobName,
      int attempt = 1,
      int checkRerunAttempt = 1)
    {
      return PipelineUtilities.GetJobInstanceName(stageName, phaseName, jobName, attempt, checkRerunAttempt);
    }

    public static Guid GetJobInstanceId(
      StageInstance stage,
      PhaseInstance phase,
      JobInstance job,
      bool preserveCase = false,
      int checkRerunAttempt = 1)
    {
      return PipelineUtilities.GetJobInstanceId(stage?.Name, phase.Name, job.Name, job.Attempt, preserveCase, checkRerunAttempt);
    }

    public static Guid GetJobInstanceId(
      string stageName,
      string phaseName,
      string jobName,
      int jobAttempt,
      bool preserveCase = false,
      int checkRerunAttempt = 1)
    {
      return PipelineUtilities.GetInstanceId(PipelineUtilities.GetJobInstanceName(stageName, phaseName, jobName, jobAttempt, checkRerunAttempt), preserveCase);
    }

    public static string GetJobInstanceName(
      StageInstance stage,
      PhaseInstance phase,
      JobInstance job)
    {
      return PipelineUtilities.GetJobInstanceName(stage?.Name, phase.Name, job.Name, job.Attempt, job.CheckRerunAttempt);
    }

    public static string GetJobInstanceName(string jobIdentifier, int jobAttempt)
    {
      StringBuilder stringBuilder = new StringBuilder(jobIdentifier);
      if (jobAttempt > 1)
        stringBuilder.Append(string.Format(".{0}", (object) jobAttempt));
      return stringBuilder.ToString();
    }

    public static string GetJobInstanceName(TimelineRecord job) => job.Attempt <= 1 ? job.Identifier : string.Format("{0}.{1}", (object) job.Identifier, (object) job.Attempt);

    public static string GetJobInstanceName(
      string stageName,
      string phaseName,
      string jobName,
      int jobAttempt = 1,
      int checkRerunAttempt = 1)
    {
      StringBuilder stringBuilder = new StringBuilder(PipelineUtilities.GetPhaseInstanceName(stageName, phaseName, 1));
      stringBuilder.Append("." + jobName);
      if (checkRerunAttempt > 1)
        stringBuilder.Append(string.Format(".rerun.{0}", (object) checkRerunAttempt));
      if (jobAttempt > 1)
        stringBuilder.Append(string.Format(".{0}", (object) jobAttempt));
      return stringBuilder.ToString();
    }

    public static Guid GetTaskInstanceId(
      string stageName,
      string phaseName,
      string jobName,
      int jobAttempt,
      string taskName,
      bool preserveCase = false,
      int checkRerunAttempt = 1)
    {
      return PipelineUtilities.GetInstanceId(PipelineUtilities.GetTaskInstanceName(stageName, phaseName, jobName, jobAttempt, taskName, checkRerunAttempt), preserveCase);
    }

    public static string GetTaskInstanceName(
      string stageName,
      string phaseName,
      string jobName,
      int jobAttempt,
      string taskName,
      int checkRerunAttempt = 1)
    {
      return PipelineUtilities.GetJobInstanceName(stageName, phaseName, jobName, jobAttempt, checkRerunAttempt) + "." + taskName;
    }

    public static string GetTaskInstanceName(TimelineRecord jobRecord, TimelineRecord taskRecord) => PipelineUtilities.GetJobInstanceName(jobRecord) + "." + taskRecord.RefName;

    public static TaskResult MergeResult(TaskResult result, TaskResult childResult)
    {
      if (result == TaskResult.Canceled || result == TaskResult.Failed)
        return result;
      switch (childResult)
      {
        case TaskResult.SucceededWithIssues:
          if (result == TaskResult.Succeeded)
          {
            result = TaskResult.SucceededWithIssues;
            break;
          }
          break;
        case TaskResult.Failed:
        case TaskResult.Abandoned:
          result = TaskResult.Failed;
          break;
        case TaskResult.Canceled:
          result = TaskResult.Canceled;
          break;
      }
      return result;
    }

    public static TaskResult AggregateResult(IEnumerable<TaskResult> results)
    {
      TaskResult result1 = TaskResult.Succeeded;
      if (results == null)
        return result1;
      foreach (TaskResult result2 in results)
        result1 = PipelineUtilities.MergeResult(result1, result2);
      return result1;
    }

    public static IList<string> GetPathComponents(string instanceName)
    {
      List<string> list = new List<string>();
      if (!string.IsNullOrEmpty(instanceName))
      {
        string[] strArray = instanceName.Split('.');
        int index = 0;
        if (Guid.TryParse(strArray[index], out Guid _))
          index = 1;
        for (int length = strArray.Length; index < length; ++index)
        {
          string str = strArray[index];
          list.AddIf<string>(!int.TryParse(str, out int _), str);
        }
      }
      return (IList<string>) list;
    }

    public static bool IsLegalNodeName(string name, bool allowWcharLetters = false)
    {
      if (string.IsNullOrWhiteSpace(name) || name.Length > PipelineConstants.MaxNodeNameLength)
        return false;
      if (allowWcharLetters)
      {
        if (!char.IsLetter(name[0]))
          return false;
        foreach (char c in name)
        {
          if (!char.IsLetterOrDigit(c) && c != '_')
            return false;
        }
        return true;
      }
      return NameValidation.IsValid(name, true) && name[0] != '_';
    }

    public static string GetOrchestrationInstanceId(Guid planId, string nodeIdentifier) => PipelineUtilities.GetInstanceName(planId.ToString("D"), nodeIdentifier);

    public static string GetPhaseOrchestrationId(
      PhaseExecutionContext phaseExecutionContext,
      Guid planId)
    {
      return string.Format("{0}.{1}", (object) planId, (object) phaseExecutionContext?.GetInstanceName()?.ToLowerInvariant());
    }
  }
}
