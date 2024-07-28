// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamMetadataCacheRefreshFeedJobHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class UpstreamMetadataCacheRefreshFeedJobHandler : 
    IAsyncHandler<IFeedRequest<FeedUpstreamRequestJobData>, JobResult>,
    IHaveInputType<IFeedRequest<FeedUpstreamRequestJobData>>,
    IHaveOutputType<JobResult>
  {
    private readonly IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> upstreamMetadataManagerFactory;
    private readonly IDoer retryJobQueuer;
    private readonly IFeatureFlagService featureFlagService;

    public UpstreamMetadataCacheRefreshFeedJobHandler(
      IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> upstreamMetadataManagerFactory,
      IDoer retryJobQueuer,
      IFeatureFlagService featureFlagService)
    {
      this.upstreamMetadataManagerFactory = upstreamMetadataManagerFactory;
      this.retryJobQueuer = retryJobQueuer;
      this.featureFlagService = featureFlagService;
    }

    public async Task<JobResult> Handle(
      IFeedRequest<FeedUpstreamRequestJobData> feedRequest)
    {
      if (this.featureFlagService.IsEnabled("Packaging.SkipFeedLevelRefreshes"))
      {
        FeedJobTelemetry telemetry = new FeedJobTelemetry();
        telemetry.FeedId = feedRequest.Feed.Id;
        telemetry.Message = "Disabled by feature flag Packaging.SkipFeedLevelRefreshes";
        return JobResult.Blocked((JobTelemetry) telemetry);
      }
      FeedCore feed = feedRequest.Feed;
      try
      {
        TeamFoundationJobExecutionResult jobExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
        IUpstreamMetadataManager upstreamMetadataManager = await this.upstreamMetadataManagerFactory.Get((IFeedRequest) feedRequest);
        FeedUpstreamRequestJobData feedUpstreamRequestJobData = feedRequest.AdditionalData;
        if (feedUpstreamRequestJobData == null || feedUpstreamRequestJobData.UpstreamPackagesToRefresh == null)
          throw new ArgumentException("Job data is required", "feedRequest.AdditionalData");
        if (feedRequest.Feed.Id != feedUpstreamRequestJobData.UpstreamPackagesToRefresh.FeedId)
        {
          Guid guid = feedRequest.Feed.Id;
          string str1 = guid.ToString();
          guid = feedUpstreamRequestJobData.UpstreamPackagesToRefresh.FeedId;
          string str2 = guid.ToString();
          throw new UpstreamJobFeedDoesNotMatchRequest(Resources.Error_UpstreamJobFeedRequestDifferentFromRefreshInformationFeed((object) str1, (object) str2));
        }
        IFeedRequest<FeedUpstreamRequestJobData> feedRequest1 = feedRequest;
        ImmutableHashSet<Guid> empty = ImmutableHashSet<Guid>.Empty;
        UpstreamPackagesToRefreshInformation packagesToRefresh = feedUpstreamRequestJobData.UpstreamPackagesToRefresh;
        RefreshPackagesResult refreshPackagesResult = await upstreamMetadataManager.RefreshPackagesAsync((IFeedRequest) feedRequest1, true, (ISet<Guid>) empty, packagesToRefresh);
        FeedUpstreamRefreshJobTelemetry refreshJobTelemetry = new FeedUpstreamRefreshJobTelemetry()
        {
          CorrelationId = feedUpstreamRequestJobData.CorrelationId,
          NumSplitJobs = feedUpstreamRequestJobData.NumSplitJobs,
          Result = refreshPackagesResult
        };
        if (refreshPackagesResult.Failures.Any<RefreshPackageFailure>())
        {
          this.retryJobQueuer.Do();
          jobExecutionResult = refreshPackagesResult.Successes.Any<RefreshPackageResult>((Func<RefreshPackageResult, bool>) (x => x.RefreshNeeded)) ? TeamFoundationJobExecutionResult.PartiallySucceeded : TeamFoundationJobExecutionResult.Failed;
        }
        return new JobResult()
        {
          Result = jobExecutionResult,
          Telemetry = (JobTelemetry) refreshJobTelemetry
        };
      }
      catch (FeedIdNotFoundException ex)
      {
        return JobResult.Succeeded(new JobTelemetry()
        {
          Message = string.Format("Feed '{0}' not found", (object) feed.Id)
        });
      }
      catch (UpstreamJobFeedDoesNotMatchRequest ex)
      {
        JobTelemetry telemetry = new JobTelemetry();
        telemetry.LogException((Exception) ex);
        return JobResult.Failed(telemetry);
      }
      catch (Exception ex)
      {
        this.retryJobQueuer.Do();
        JobTelemetry telemetry = new JobTelemetry();
        telemetry.LogException(ex);
        return JobResult.Failed(telemetry);
      }
    }
  }
}
