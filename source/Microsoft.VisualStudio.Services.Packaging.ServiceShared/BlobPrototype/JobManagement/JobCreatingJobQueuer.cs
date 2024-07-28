// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.JobCreatingJobQueuer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class JobCreatingJobQueuer : IJobQueuer
  {
    private readonly IVssRequestContext requestContext;
    private readonly IJobQueuer backingJobQueuer;
    private readonly JobCreationInfo jobCreationInfo;

    public JobCreatingJobQueuer(
      IVssRequestContext requestContext,
      IJobQueuer backingJobQueuer,
      JobCreationInfo jobCreationInfo)
    {
      this.jobCreationInfo = jobCreationInfo;
      this.requestContext = requestContext;
      this.backingJobQueuer = backingJobQueuer;
    }

    public void QueueJob(JobId jobId, JobPriorityLevel jobPriority) => this.QueueJob(jobId, jobPriority, TimeSpan.Zero);

    public void QueueJob(JobId jobId, JobPriorityLevel jobPriority, TimeSpan maxDelay)
    {
      this.requestContext.CheckServiceHostType(this.jobCreationInfo.HostType, "class: " + this.GetType().FullName + "; jobCreationInfo : " + JsonConvert.SerializeObject((object) this.jobCreationInfo));
      try
      {
        this.backingJobQueuer.QueueJob(jobId, jobPriority);
      }
      catch (JobDefinitionNotFoundException ex)
      {
        this.CreateJobDefinition(jobId);
        this.backingJobQueuer.QueueJob(jobId, jobPriority);
      }
    }

    public Guid QueueOneTimeJob(
      string jobName,
      string extensionName,
      XmlNode jobData,
      JobPriorityLevel priorityLevel = JobPriorityLevel.Normal,
      JobPriorityClass priorityClass = JobPriorityClass.Normal,
      TimeSpan maxDelay = default (TimeSpan))
    {
      return this.backingJobQueuer.QueueOneTimeJob(jobName, extensionName, jobData, priorityLevel, priorityClass, maxDelay);
    }

    public TeamFoundationJobHistoryEntry QueryLatestJobHistory(JobId jobId) => this.backingJobQueuer.QueryLatestJobHistory(jobId);

    public void CreateOrUpdateJobDefinition(TeamFoundationJobDefinition jobDefinition) => this.backingJobQueuer.CreateOrUpdateJobDefinition(jobDefinition);

    private void CreateJobDefinition(JobId jobId) => this.CreateOrUpdateJobDefinition(new TeamFoundationJobDefinition(jobId.Guid, this.jobCreationInfo.JobName, this.jobCreationInfo.JobExtensionName, (XmlNode) null, TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.AboveNormal));
  }
}
