// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.CollectionPublicFeedQueuingJobHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs
{
  public class CollectionPublicFeedQueuingJobHandler : 
    IAsyncHandler<TeamFoundationJobDefinition, JobResult>,
    IHaveInputType<TeamFoundationJobDefinition>,
    IHaveOutputType<JobResult>
  {
    private readonly IProtocol protocol;
    private readonly IFeedJobQueuer jobQueuer;
    private readonly IFeedService feedService;
    private readonly ITracerService tracerService;

    public CollectionPublicFeedQueuingJobHandler(
      IFeedJobQueuer jobQueuer,
      IProtocol protocol,
      IFeedService feedService,
      ITracerService tracerService)
    {
      this.jobQueuer = jobQueuer;
      this.protocol = protocol;
      this.feedService = feedService;
      this.tracerService = tracerService;
    }

    public async Task<JobResult> Handle(TeamFoundationJobDefinition jobDefinition)
    {
      CollectionPublicFeedQueuingJobHandler sendInTheThisObject = this;
      FeedJobQueuerTelemetry telemetry = new FeedJobQueuerTelemetry();
      try
      {
        using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
        {
          IEnumerable<FeedCore> source = sendInTheThisObject.feedService.GetFeeds().Where<FeedCore>((Func<FeedCore, bool>) (feed => feed.IsPublicFeed()));
          telemetry.FeedCount = source.Count<FeedCore>();
          foreach (FeedCore feed in source)
          {
            Guid guid = await sendInTheThisObject.jobQueuer.QueueJob(feed, sendInTheThisObject.protocol, JobPriorityLevel.Normal);
            ++telemetry.FeedsJobsQueued;
          }
        }
      }
      catch (Exception ex)
      {
        FeedJobQueuerTelemetry telemetry1 = telemetry;
        throw new JobFailedException(ex, (JobTelemetry) telemetry1);
      }
      JobResult jobResult = JobResult.Succeeded((JobTelemetry) telemetry);
      telemetry = (FeedJobQueuerTelemetry) null;
      return jobResult;
    }
  }
}
