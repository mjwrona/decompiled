// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent6
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobQueueComponent6 : JobQueueComponent5
  {
    internal override ResultCollection AcquireJobs(
      Guid agentId,
      int maxJobsToAcquire,
      bool allowDeferJobs,
      int dormancyInterval)
    {
      this.PrepareStoredProcedure("prc_AcquireJobs");
      this.BindGuid("@agentId", agentId);
      this.BindInt("@maxJobsToAcquire", maxJobsToAcquire);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns3());
      resultCollection.AddBinder<int>((ObjectBinder<int>) new NextScheduledJobColumns());
      return resultCollection;
    }

    internal override void ReleaseJobs(
      Guid agentId,
      int assumeHostActiveSeconds,
      int failureIgnoreDormancySeconds,
      int notificationRequiredSeconds,
      List<ReleaseJobInfo> jobsToRelease,
      List<TeamFoundationJobSchedule> jobsToReleaseSchedules,
      bool logSuccessfulJobs = true)
    {
      this.PrepareStoredProcedure("prc_ReleaseJobs");
      this.BindGuid("@agentId", agentId);
      this.BindInt("@assumeHostActiveSeconds", assumeHostActiveSeconds);
      this.BindInt("@failureIgnoreDormancySeconds", failureIgnoreDormancySeconds);
      this.BindReleaseJobsTable("@jobsToRelease", (IEnumerable<ReleaseJobInfo>) jobsToRelease);
      this.BindJobScheduleUpdateTableForReleaseJobs("@jobsToReleaseSchedules", (IEnumerable<TeamFoundationJobSchedule>) jobsToReleaseSchedules);
      this.BindDaylightTransitionInfoTable("@daylightTransitions", (IEnumerable<DaylightTransitionInfo>) JobComponentBase.GetDaylightTransitions((IEnumerable<TeamFoundationJobSchedule>) jobsToReleaseSchedules));
      int num = (int) this.ExecuteScalar();
      if (num == jobsToRelease.Count)
        return;
      this.Trace(1700, TraceLevel.Error, "Released {0} jobs instead of {1}", (object) num, (object) jobsToRelease.Count);
    }

    internal override void ReenableJobs(IEnumerable<Guid> jobSources)
    {
      this.PrepareStoredProcedure("prc_ReenableJobs");
      this.BindGuidTable("@jobSources", jobSources);
      this.ExecuteNonQuery();
    }

    internal override ResultCollection QueryJobQueue()
    {
      this.PrepareStoredProcedure("prc_QueryJobQueue");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns3());
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns3());
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns3());
      return resultCollection;
    }

    public override List<TeamFoundationJobQueueEntry> QueryJobQueueForOneHost(
      Guid jobSource,
      IEnumerable<Guid> jobIds)
    {
      this.PrepareStoredProcedure("prc_QueryJobQueueForOneHost");
      this.BindGuid("@jobSource", jobSource);
      this.BindQueryJobsTable("@jobIds", jobIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns3());
      return resultCollection.GetCurrent<TeamFoundationJobQueueEntry>().Items;
    }

    public override List<TeamFoundationJobHistoryEntry> QueryJobHistory(
      Guid jobSource,
      IEnumerable<Guid> jobIds)
    {
      this.PrepareStoredProcedure("prc_QueryJobHistory");
      this.BindGuid("@jobSource", jobSource);
      this.BindGuidTable("@jobIds", jobIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobHistoryEntry>((ObjectBinder<TeamFoundationJobHistoryEntry>) new TeamFoundationJobHistoryEntryColumns3());
      return resultCollection.GetCurrent<TeamFoundationJobHistoryEntry>().Items;
    }

    public override List<TeamFoundationJobHistoryEntry> QueryLatestJobHistory(
      Guid jobSource,
      IEnumerable<Guid> jobIds)
    {
      this.PrepareStoredProcedure("prc_QueryLatestJobHistory");
      this.BindGuid("@jobSource", jobSource);
      this.BindQueryJobsTable("@jobIds", jobIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobHistoryEntry>((ObjectBinder<TeamFoundationJobHistoryEntry>) new TeamFoundationJobHistoryEntryColumns3());
      return resultCollection.GetCurrent<TeamFoundationJobHistoryEntry>().Items;
    }
  }
}
