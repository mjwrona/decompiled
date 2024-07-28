// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IJobSchedulerManager
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public interface IJobSchedulerManager
  {
    Task CompletePhaseAsync(
      Guid scopeId,
      string planType,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      TaskResult result);

    Task<JobInstance> QueueJobAsync(
      Guid scopeId,
      string planType,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      JobInstance job);

    Task<JobInstance> QueueJobWithJobOrderAsync(
      Guid scopeId,
      string planType,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      JobInstance job,
      int jobOrder);

    Task<JobInstance> QueueJobAsync2(
      Guid scopeId,
      string planType,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      JobInstance job,
      Dictionary<string, string> jobMetaData);

    Task CancelJobAsync(
      Guid scopeId,
      string planType,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      JobInstance job);

    Task UpdateTimeline(Guid scopeId, string planType, Guid planId, IList<TimelineRecord> record);
  }
}
