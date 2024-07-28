// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.JobServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class JobServiceFacade : IJobQueuer
  {
    private readonly ITeamFoundationJobService jobService;
    private readonly IVssRequestContext requestContext;

    public JobServiceFacade(IVssRequestContext requestContext, ITeamFoundationJobService jobService)
    {
      this.jobService = jobService;
      this.requestContext = requestContext;
    }

    public void QueueJob(JobId jobId, JobPriorityLevel jobPriority) => this.QueueJob(jobId, jobPriority, TimeSpan.Zero);

    public void QueueJob(JobId jobId, JobPriorityLevel jobPriority, TimeSpan maxDelay)
    {
      ITeamFoundationJobService jobService = this.jobService;
      IVssRequestContext requestContext = this.requestContext;
      List<Guid> jobIds = new List<Guid>();
      jobIds.Add(jobId.Guid);
      int int32 = Convert.ToInt32(maxDelay.TotalSeconds);
      int priorityLevel = (int) jobPriority;
      jobService.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) jobIds, int32, (JobPriorityLevel) priorityLevel);
    }

    public Guid QueueOneTimeJob(
      string jobName,
      string extensionName,
      XmlNode jobData,
      JobPriorityLevel priorityLevel = JobPriorityLevel.Normal,
      JobPriorityClass priorityClass = JobPriorityClass.Normal,
      TimeSpan maxDelay = default (TimeSpan))
    {
      return this.jobService.QueueOneTimeJob(this.requestContext, jobName, extensionName, jobData, priorityLevel, priorityClass, maxDelay);
    }

    public TeamFoundationJobHistoryEntry QueryLatestJobHistory(JobId jobId) => this.jobService.QueryLatestJobHistory(this.requestContext, jobId.Guid);

    public void CreateOrUpdateJobDefinition(TeamFoundationJobDefinition jobDefinition) => this.jobService.UpdateJobDefinitions(this.requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new List<TeamFoundationJobDefinition>()
    {
      jobDefinition
    });
  }
}
