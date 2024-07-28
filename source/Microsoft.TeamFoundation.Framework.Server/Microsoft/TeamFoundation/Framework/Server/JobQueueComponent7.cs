// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent7
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobQueueComponent7 : JobQueueComponent6
  {
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
      this.BindInt("@notificationRequiredSeconds", notificationRequiredSeconds);
      this.BindReleaseJobsTable("@jobsToRelease", (IEnumerable<ReleaseJobInfo>) jobsToRelease);
      this.BindJobScheduleUpdateTableForReleaseJobs("@jobsToReleaseSchedules", (IEnumerable<TeamFoundationJobSchedule>) jobsToReleaseSchedules);
      this.BindDaylightTransitionInfoTable("@daylightTransitions", (IEnumerable<DaylightTransitionInfo>) JobComponentBase.GetDaylightTransitions((IEnumerable<TeamFoundationJobSchedule>) jobsToReleaseSchedules));
      int num = (int) this.ExecuteScalar();
      if (num == jobsToRelease.Count)
        return;
      this.Trace(1700, TraceLevel.Error, "Released {0} jobs instead of {1}", (object) num, (object) jobsToRelease.Count);
    }
  }
}
