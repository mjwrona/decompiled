// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamFoundationJobService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public class TeamFoundationJobService : ITeamFoundationJobService
  {
    private JobWebService m_service;

    public TeamFoundationJobService(TfsConnection server) => this.m_service = new JobWebService(server);

    public void DeleteJobs(IEnumerable<Guid> jobIds) => this.m_service.UpdateJobDefinitions(jobIds, (IEnumerable<TeamFoundationJobDefinition>) null);

    public void DeleteJobs(IEnumerable<TeamFoundationJobDefinition> jobs) => this.m_service.UpdateJobDefinitions(jobs.Select<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (job => job.JobId)), (IEnumerable<TeamFoundationJobDefinition>) null);

    public void DeleteJob(TeamFoundationJobDefinition job) => this.m_service.UpdateJobDefinitions((IEnumerable<Guid>) new Guid[1]
    {
      job.JobId
    }, (IEnumerable<TeamFoundationJobDefinition>) null);

    public void DeleteJob(Guid jobId) => this.m_service.UpdateJobDefinitions((IEnumerable<Guid>) new Guid[1]
    {
      jobId
    }, (IEnumerable<TeamFoundationJobDefinition>) null);

    public void UpdateJob(TeamFoundationJobDefinition job) => this.m_service.UpdateJobDefinitions((IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
    {
      job
    });

    public void UpdateJobs(
      IEnumerable<Guid> jobsToDelete,
      IEnumerable<TeamFoundationJobDefinition> jobsToUpdate)
    {
      this.m_service.UpdateJobDefinitions(jobsToDelete, jobsToUpdate);
    }

    public void UpdateJobs(IEnumerable<TeamFoundationJobDefinition> jobs) => this.m_service.UpdateJobDefinitions((IEnumerable<Guid>) null, jobs);

    public void UpdateJobs(
      IEnumerable<TeamFoundationJobDefinition> jobsToDelete,
      IEnumerable<TeamFoundationJobDefinition> jobsToUpdate)
    {
      this.m_service.UpdateJobDefinitions(jobsToDelete.Select<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (job => job.JobId)), jobsToUpdate);
    }

    public bool StopJob(Guid jobId) => this.m_service.StopJob(jobId);

    public bool StopJob(TeamFoundationJobDefinition job) => this.m_service.StopJob(job.JobId);

    public bool PauseJob(Guid jobId) => this.m_service.PauseJob(jobId);

    public bool PauseJob(TeamFoundationJobDefinition job) => this.m_service.PauseJob(job.JobId);

    public int QueueDelayedJob(TeamFoundationJobDefinition job) => this.m_service.QueueJobs((IEnumerable<Guid>) new Guid[1]
    {
      job.JobId
    }, false, -1);

    public int QueueDelayedJob(Guid jobId) => this.m_service.QueueJobs((IEnumerable<Guid>) new Guid[1]
    {
      jobId
    }, false, -1);

    public int QueueDelayedJob(TeamFoundationJobDefinition job, int maxDelaySeconds) => this.m_service.QueueJobs((IEnumerable<Guid>) new Guid[1]
    {
      job.JobId
    }, false, maxDelaySeconds);

    public int QueueDelayedJob(Guid jobId, int maxDelaySeconds) => this.m_service.QueueJobs((IEnumerable<Guid>) new Guid[1]
    {
      jobId
    }, false, maxDelaySeconds);

    public int QueueDelayedJobs(IEnumerable<TeamFoundationJobDefinition> jobs) => this.m_service.QueueJobs(jobs.Select<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (job => job.JobId)), false, -1);

    public int QueueDelayedJobs(IEnumerable<Guid> jobIds) => this.m_service.QueueJobs(jobIds, false, -1);

    public int QueueDelayedJobs(IEnumerable<TeamFoundationJobDefinition> jobs, int maxDelaySeconds) => this.m_service.QueueJobs(jobs.Select<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (job => job.JobId)), false, maxDelaySeconds);

    public int QueueDelayedJobs(IEnumerable<Guid> jobIds, int maxDelaySeconds) => this.m_service.QueueJobs(jobIds, false, maxDelaySeconds);

    public bool ResumeJob(Guid jobId) => this.m_service.ResumeJob(jobId);

    public bool ResumeJob(TeamFoundationJobDefinition job) => this.m_service.ResumeJob(job.JobId);

    public int QueueJobsNow(IEnumerable<TeamFoundationJobDefinition> jobs, bool highPriority) => this.m_service.QueueJobs(jobs.Select<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (job => job.JobId)), highPriority, 0);

    public int QueueJobsNow(IEnumerable<Guid> jobIds, bool highPriority) => this.m_service.QueueJobs(jobIds, highPriority, 0);

    public int QueueJobNow(TeamFoundationJobDefinition job, bool highPriority) => this.m_service.QueueJobs((IEnumerable<Guid>) new Guid[1]
    {
      job.JobId
    }, (highPriority ? 1 : 0) != 0, 0);

    public int QueueJobNow(Guid jobId, bool highPriority) => this.m_service.QueueJobs((IEnumerable<Guid>) new Guid[1]
    {
      jobId
    }, (highPriority ? 1 : 0) != 0, 0);

    [Obsolete("This method is no longer supported.")]
    public Guid QueueOneTimeJob(
      string jobName,
      string extensionName,
      XmlNode data,
      bool highPriority)
    {
      return this.m_service.QueueOneTimeJob(jobName, extensionName, data, highPriority);
    }

    public TeamFoundationJobDefinition[] QueryJobs() => this.m_service.QueryJobDefinitions((IEnumerable<Guid>) null);

    public TeamFoundationJobDefinition[] QueryJobs(IEnumerable<Guid> jobIds) => this.m_service.QueryJobDefinitions(jobIds);

    public IList<TeamFoundationJobHistoryEntry> QueryJobHistory(IEnumerable<Guid> jobIds) => (IList<TeamFoundationJobHistoryEntry>) this.m_service.QueryJobHistory(jobIds);

    public IList<TeamFoundationJobHistoryEntry> QueryJobHistory(
      IEnumerable<TeamFoundationJobDefinition> jobs)
    {
      return this.QueryJobHistory(jobs != null ? jobs.Select<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (job => job.JobId)) : (IEnumerable<Guid>) null);
    }

    public IList<TeamFoundationJobHistoryEntry> QueryLatestJobHistory(IEnumerable<Guid> jobIds) => (IList<TeamFoundationJobHistoryEntry>) this.m_service.QueryLatestJobHistory(jobIds);

    public IList<TeamFoundationJobHistoryEntry> QueryLatestJobHistory(
      IEnumerable<TeamFoundationJobDefinition> jobs)
    {
      return (IList<TeamFoundationJobHistoryEntry>) this.m_service.QueryLatestJobHistory(jobs != null ? jobs.Select<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (job => job.JobId)) : (IEnumerable<Guid>) null);
    }

    public TeamFoundationJobHistoryEntry QueryLatestJobHistory(Guid jobId) => this.m_service.QueryLatestJobHistory((IEnumerable<Guid>) new Guid[1]
    {
      jobId
    })[0];

    public TeamFoundationJobHistoryEntry QueryLatestJobHistory(TeamFoundationJobDefinition job)
    {
      ArgumentUtility.CheckForNull<TeamFoundationJobDefinition>(job, nameof (job));
      return this.m_service.QueryLatestJobHistory((IEnumerable<Guid>) new Guid[1]
      {
        job.JobId
      })[0];
    }
  }
}
