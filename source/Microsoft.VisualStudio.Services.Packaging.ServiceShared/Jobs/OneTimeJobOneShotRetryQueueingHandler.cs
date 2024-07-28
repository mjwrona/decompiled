// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.OneTimeJobOneShotRetryQueueingHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs
{
  public class OneTimeJobOneShotRetryQueueingHandler : IHandler<JobQueueRequest>
  {
    private readonly IJobQueuer jobQueuer;
    private readonly ITracerService tracerService;
    private readonly JobRunSettings jobRunSettings;

    public OneTimeJobOneShotRetryQueueingHandler(
      IJobQueuer jobQueuer,
      ITracerService tracerService,
      JobRunSettings jobRunSettings)
    {
      this.jobQueuer = jobQueuer;
      this.tracerService = tracerService;
      this.jobRunSettings = jobRunSettings;
    }

    public void Handle(JobQueueRequest jobQueueRequest)
    {
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (Handle)))
      {
        try
        {
          JobId jobId = jobQueueRequest.JobId;
          if (this.jobQueuer.QueryLatestJobHistory(jobId) != null)
            return;
          TimeSpan maxDelay = jobQueueRequest.Options == JobQueuingOptions.FromHistory ? this.jobRunSettings.FirstRequeueMaxDelay : jobQueueRequest.QueueDelay;
          this.jobQueuer.QueueJob(jobId, JobPriorityLevel.Normal, maxDelay);
        }
        catch (Exception ex)
        {
          tracerBlock.TraceException(ex);
        }
      }
    }
  }
}
