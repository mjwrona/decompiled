// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.DisposingJobQueuer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class DisposingJobQueuer : IDisposingJobQueuer, IDisposable, IJobQueuer
  {
    private readonly IJobQueuer backingJobQueuer;
    private readonly IVssRequestContext requestContext;

    public DisposingJobQueuer(IJobQueuer backingJobQueuer, IVssRequestContext requestContext)
    {
      this.backingJobQueuer = backingJobQueuer;
      this.requestContext = requestContext;
    }

    public void Dispose() => this.requestContext.Dispose();

    public void QueueJob(JobId jobId, JobPriorityLevel jobPriority) => throw new NotImplementedException();

    public void QueueJob(JobId jobId, JobPriorityLevel jobPriority, TimeSpan maxDelay) => throw new NotImplementedException();

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

    public TeamFoundationJobHistoryEntry QueryLatestJobHistory(JobId jobId) => throw new NotImplementedException();

    public void CreateOrUpdateJobDefinition(TeamFoundationJobDefinition jobDefinition) => throw new NotImplementedException();
  }
}
