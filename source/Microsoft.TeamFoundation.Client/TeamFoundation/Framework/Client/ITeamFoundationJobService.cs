// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ITeamFoundationJobService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public interface ITeamFoundationJobService
  {
    void DeleteJob(TeamFoundationJobDefinition job);

    void DeleteJob(Guid jobId);

    void DeleteJobs(IEnumerable<Guid> jobIds);

    void DeleteJobs(IEnumerable<TeamFoundationJobDefinition> jobs);

    void UpdateJob(TeamFoundationJobDefinition job);

    void UpdateJobs(
      IEnumerable<Guid> jobsToDelete,
      IEnumerable<TeamFoundationJobDefinition> jobsToUpdate);

    void UpdateJobs(
      IEnumerable<TeamFoundationJobDefinition> jobsToDelete,
      IEnumerable<TeamFoundationJobDefinition> jobsToUpdate);

    void UpdateJobs(IEnumerable<TeamFoundationJobDefinition> jobs);

    bool StopJob(Guid jobId);

    bool StopJob(TeamFoundationJobDefinition job);

    bool PauseJob(Guid jobId);

    bool PauseJob(TeamFoundationJobDefinition job);

    bool ResumeJob(Guid jobId);

    bool ResumeJob(TeamFoundationJobDefinition job);

    int QueueJobNow(TeamFoundationJobDefinition job, bool highPriority);

    int QueueJobNow(Guid jobId, bool highPriority);

    int QueueJobsNow(IEnumerable<TeamFoundationJobDefinition> jobs, bool highPriority);

    int QueueJobsNow(IEnumerable<Guid> jobIds, bool highPriority);

    int QueueDelayedJob(TeamFoundationJobDefinition job);

    int QueueDelayedJob(Guid jobId);

    int QueueDelayedJob(TeamFoundationJobDefinition job, int maxDelaySeconds);

    int QueueDelayedJob(Guid jobId, int maxDelaySeconds);

    int QueueDelayedJobs(IEnumerable<TeamFoundationJobDefinition> jobs);

    int QueueDelayedJobs(IEnumerable<Guid> jobIds);

    int QueueDelayedJobs(IEnumerable<TeamFoundationJobDefinition> jobs, int maxDelaySeconds);

    int QueueDelayedJobs(IEnumerable<Guid> jobIds, int maxDelaySeconds);

    Guid QueueOneTimeJob(string jobName, string extensionName, XmlNode jobData, bool highPriority);

    TeamFoundationJobDefinition[] QueryJobs(IEnumerable<Guid> jobIds);

    TeamFoundationJobDefinition[] QueryJobs();

    IList<TeamFoundationJobHistoryEntry> QueryJobHistory(IEnumerable<Guid> jobIds);

    IList<TeamFoundationJobHistoryEntry> QueryJobHistory(
      IEnumerable<TeamFoundationJobDefinition> jobs);

    IList<TeamFoundationJobHistoryEntry> QueryLatestJobHistory(IEnumerable<Guid> jobIds);

    IList<TeamFoundationJobHistoryEntry> QueryLatestJobHistory(
      IEnumerable<TeamFoundationJobDefinition> jobs);

    TeamFoundationJobHistoryEntry QueryLatestJobHistory(Guid jobId);

    TeamFoundationJobHistoryEntry QueryLatestJobHistory(TeamFoundationJobDefinition job);
  }
}
