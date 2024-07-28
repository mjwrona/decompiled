// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationJobService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationJobService))]
  public interface ITeamFoundationJobService : IVssFrameworkService
  {
    int StopJobTimeLimit { get; }

    bool IsIgnoreDormancyPermitted { get; }

    List<TeamFoundationJobDefinition> QueryJobDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds);

    void UpdateJobDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobsToDelete,
      IEnumerable<TeamFoundationJobDefinition> jobUpdates);

    int QueueJobs(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobReference> jobReferences,
      JobPriorityLevel priorityLevel,
      int maxDelaySeconds,
      bool queueAsDormant);

    bool StopJob(IVssRequestContext requestContext, Guid jobId);

    List<TeamFoundationJobHistoryEntry> QueryJobHistory(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds);

    List<TeamFoundationJobHistoryEntry> QueryLatestJobHistory(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds);

    void RepairQueue(IVssRequestContext requestContext, ITFLogger logger);

    List<TeamFoundationJobQueueEntry> QueryJobQueue(
      IVssRequestContext jobSourceRequestContext,
      IEnumerable<Guid> jobIds);

    List<TeamFoundationJobQueueEntry> QueryRunningJobs(
      IVssRequestContext requestContext,
      bool includePendingJobs = false);

    int DeleteOneTimeJobDefinitions(
      IVssRequestContext requestContext,
      DateTime? completedTo = null,
      int batchSize = 5000);
  }
}
