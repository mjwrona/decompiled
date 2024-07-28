// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine.FeedMigrationAndChangeProcessingInlinePerformerAsQueuer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine
{
  public class FeedMigrationAndChangeProcessingInlinePerformerAsQueuer : IFeedJobQueuer
  {
    private readonly IFeedService feedService;
    private readonly IAsyncHandler<IFeedRequest, JobResult> migrationJobHandler;
    private readonly IAsyncHandler<IFeedRequest, JobResult> changeProcessingJobHandler;

    public FeedMigrationAndChangeProcessingInlinePerformerAsQueuer(
      IFeedService feedService,
      IAsyncHandler<IFeedRequest, JobResult> migrationJobHandler,
      IAsyncHandler<IFeedRequest, JobResult> changeProcessingJobHandler)
    {
      this.feedService = feedService;
      this.migrationJobHandler = migrationJobHandler;
      this.changeProcessingJobHandler = changeProcessingJobHandler;
    }

    public async Task<Guid> QueueJob(
      FeedCore feed,
      IProtocol protocol,
      JobPriorityLevel jobPriority,
      int maxDelaySeconds = 0)
    {
      FeedRequest feedRequest = new FeedRequest(feed, protocol);
      JobResult jobResult1 = await this.migrationJobHandler.Handle((IFeedRequest) feedRequest);
      if (jobResult1.Result != TeamFoundationJobExecutionResult.Succeeded)
        throw new Exception(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FailedToMigrateDataForFeed((object) feed.Id, (object) jobResult1.Result, (object) JsonConvert.SerializeObject((object) jobResult1.Telemetry)));
      JobResult jobResult2 = await this.changeProcessingJobHandler.Handle((IFeedRequest) feedRequest);
      if (jobResult2.Result != TeamFoundationJobExecutionResult.Succeeded)
        throw new Exception(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FailedToCompleteChangeProcessing((object) feed.Id, (object) jobResult2.Result, (object) JsonConvert.SerializeObject((object) jobResult2.Telemetry)));
      Guid empty = Guid.Empty;
      feedRequest = (FeedRequest) null;
      return empty;
    }
  }
}
