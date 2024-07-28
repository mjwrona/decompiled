// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.JobResult
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class JobResult
  {
    public TeamFoundationJobExecutionResult Result { get; set; }

    public JobTelemetry Telemetry { get; set; }

    public TimeSpan RequeueAfter { get; private set; }

    public static JobResult Failed(JobTelemetry telemetry) => new JobResult()
    {
      Result = TeamFoundationJobExecutionResult.Failed,
      Telemetry = telemetry
    };

    public static JobResult Succeeded(JobTelemetry telemetry) => new JobResult()
    {
      Result = TeamFoundationJobExecutionResult.Succeeded,
      Telemetry = telemetry
    };

    public static JobResult SuccessButMoreWorkToDo(JobTelemetry telemetry, TimeSpan requeueAfter) => new JobResult()
    {
      Result = TeamFoundationJobExecutionResult.PartiallySucceeded,
      Telemetry = telemetry,
      RequeueAfter = requeueAfter
    };

    public static JobResult StoppedAndMoreWorkToDo(JobTelemetry telemetry, TimeSpan requeueAfter) => new JobResult()
    {
      Result = TeamFoundationJobExecutionResult.Stopped,
      Telemetry = telemetry,
      RequeueAfter = requeueAfter
    };

    public static JobResult Blocked(JobTelemetry telemetry) => new JobResult()
    {
      Result = TeamFoundationJobExecutionResult.Blocked,
      Telemetry = telemetry
    };

    public VssJobResult ToVssJobResult() => new VssJobResult(this.Result, this.Telemetry.Serialize<JobTelemetry>(true));

    public static JobResult Coalesce(IEnumerable<JobResult> results)
    {
      List<JobResult> list = results.ToList<JobResult>();
      Dictionary<TeamFoundationJobExecutionResult, int> dictionary = list.GroupBy<JobResult, TeamFoundationJobExecutionResult>((Func<JobResult, TeamFoundationJobExecutionResult>) (x => x.Result)).ToDictionary<IGrouping<TeamFoundationJobExecutionResult, JobResult>, TeamFoundationJobExecutionResult, int>((Func<IGrouping<TeamFoundationJobExecutionResult, JobResult>, TeamFoundationJobExecutionResult>) (x => x.Key), (Func<IGrouping<TeamFoundationJobExecutionResult, JobResult>, int>) (x => x.Count<JobResult>()));
      MultiJobTelemetry telemetry = new MultiJobTelemetry((IReadOnlyDictionary<TeamFoundationJobExecutionResult, int>) dictionary, (IReadOnlyList<JobResult>) list);
      if (!list.Any<JobResult>())
        return JobResult.Succeeded((JobTelemetry) telemetry);
      if (dictionary.ContainsKey(TeamFoundationJobExecutionResult.Failed))
        return JobResult.Failed((JobTelemetry) telemetry);
      if (dictionary.ContainsKey(TeamFoundationJobExecutionResult.Blocked))
        return JobResult.Blocked((JobTelemetry) telemetry);
      TimeSpan requeueAfter = list.Max<JobResult, TimeSpan>((Func<JobResult, TimeSpan>) (x => x.RequeueAfter));
      if (dictionary.ContainsKey(TeamFoundationJobExecutionResult.Stopped))
        return JobResult.StoppedAndMoreWorkToDo((JobTelemetry) telemetry, requeueAfter);
      if (dictionary.ContainsKey(TeamFoundationJobExecutionResult.PartiallySucceeded))
        return JobResult.SuccessButMoreWorkToDo((JobTelemetry) telemetry, requeueAfter);
      KeyValuePair<TeamFoundationJobExecutionResult, int> keyValuePair = dictionary.OrderByDescending<KeyValuePair<TeamFoundationJobExecutionResult, int>, int>((Func<KeyValuePair<TeamFoundationJobExecutionResult, int>, int>) (x => x.Value)).FirstOrDefault<KeyValuePair<TeamFoundationJobExecutionResult, int>>((Func<KeyValuePair<TeamFoundationJobExecutionResult, int>, bool>) (x => x.Key != 0));
      return new JobResult()
      {
        Result = keyValuePair.Value > 0 ? keyValuePair.Key : TeamFoundationJobExecutionResult.Succeeded,
        Telemetry = (JobTelemetry) telemetry,
        RequeueAfter = requeueAfter
      };
    }
  }
}
