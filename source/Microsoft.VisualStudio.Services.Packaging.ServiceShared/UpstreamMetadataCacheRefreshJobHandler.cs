// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamMetadataCacheRefreshJobHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class UpstreamMetadataCacheRefreshJobHandler : 
    INoInputAsyncHandler<VssJobResult>,
    IAsyncHandler<NullRequest, VssJobResult>,
    IHaveInputType<NullRequest>,
    IHaveOutputType<VssJobResult>
  {
    private readonly IProtocol protocol;
    private readonly IFeedService feedService;
    private readonly IFeedUpstreamRefreshJobQueuer feedJobQueuer;
    private readonly IDoer retryJobQueuer;
    private readonly IRegistryService registryService;
    private readonly IRandomProvider rng;
    private JobTelemetry telemetry;
    private readonly IFactory<IFeedRequest, IUpstreamMetadataCacheInfoStore> upstreamMetadataCacheInfoStoreFactory;
    private readonly IFeatureFlagService featureFlagService;
    private readonly IUpstreamPackagesToRefreshInformationConverter refreshInformationConverter;
    internal const int DefaultFeedJobSpreadSeconds = 900;

    public UpstreamMetadataCacheRefreshJobHandler(
      IProtocol protocol,
      IFeedService feedService,
      IFeedUpstreamRefreshJobQueuer feedJobQueuer,
      IDoer retryJobQueuer,
      IRegistryService registryService,
      IRandomProvider rng,
      IFactory<IFeedRequest, IUpstreamMetadataCacheInfoStore> upstreamMetadataCacheInfoStoreFactory,
      IFeatureFlagService featureFlagService,
      IUpstreamPackagesToRefreshInformationConverter refreshInformationConverter)
    {
      this.protocol = protocol;
      this.feedService = feedService;
      this.feedJobQueuer = feedJobQueuer;
      this.retryJobQueuer = retryJobQueuer;
      this.registryService = registryService;
      this.rng = rng;
      this.upstreamMetadataCacheInfoStoreFactory = upstreamMetadataCacheInfoStoreFactory;
      this.featureFlagService = featureFlagService;
      this.refreshInformationConverter = refreshInformationConverter;
    }

    public async Task<VssJobResult> Handle(NullRequest request)
    {
      try
      {
        IList<FeedCore> feedsWithUpstreams = await this.GetFeedsWithUpstreams();
        int spreadPeriodSeconds = this.GetFeedJobSpreadSeconds();
        int index = 0;
        foreach (FeedCore feed in (IEnumerable<FeedCore>) feedsWithUpstreams)
        {
          await this.QueueMultipleRefreshJobsPerFeed(feed, spreadPeriodSeconds);
          ++index;
        }
        return JobResult.Succeeded(this.telemetry).ToVssJobResult();
      }
      catch (Exception ex)
      {
        this.retryJobQueuer.Do();
        JobTelemetry telemetry = new JobTelemetry();
        telemetry.LogException(ex);
        return JobResult.Failed(telemetry).ToVssJobResult();
      }
    }

    private async Task<IList<FeedCore>> GetFeedsWithUpstreams()
    {
      UpstreamMetadataCacheRefreshJobHandler refreshJobHandler = this;
      IEnumerable<FeedCore> feedsAsync = await refreshJobHandler.feedService.GetFeedsAsync();
      List<FeedCore> list = feedsAsync.Where<FeedCore>(new Func<FeedCore, bool>(refreshJobHandler.HasUpstreams)).OrderBy<FeedCore, string>((Func<FeedCore, string>) (x => x.FullyQualifiedId)).ToList<FeedCore>();
      string str = Resources.Job_UpstreamRefresh((object) refreshJobHandler.protocol.ToString(), (object) list.Count, (object) feedsAsync.Count<FeedCore>());
      refreshJobHandler.telemetry = new JobTelemetry()
      {
        Message = str
      };
      return (IList<FeedCore>) list;
    }

    private bool HasUpstreams(FeedCore feed) => feed.UpstreamEnabled && feed.GetSourcesForProtocol(this.protocol).Any<UpstreamSource>();

    private Task QueueRefreshJob(FeedCore feed, int delaySeconds)
    {
      UpstreamPackagesToRefreshInformation refreshInformation = new UpstreamPackagesToRefreshInformation(feed.Id, (IPackageName) null, (IPackageName) null);
      IFeedUpstreamRefreshJobQueuer feedJobQueuer = this.feedJobQueuer;
      FeedCore feed1 = feed;
      IProtocol protocol = this.protocol;
      UpstreamPackagesToRefreshInformation toRefreshInformation = refreshInformation;
      int num = delaySeconds;
      Guid? correlationId = new Guid?();
      int maxDelaySeconds = num;
      return (Task) feedJobQueuer.QueueJob(feed1, protocol, JobPriorityLevel.Normal, toRefreshInformation, correlationId, maxDelaySeconds: maxDelaySeconds);
    }

    private async Task QueueMultipleRefreshJobsPerFeed(FeedCore feed, int spreadPeriodSeconds)
    {
      UpstreamMetadataCacheInfo metadataCacheInfoAsync = await this.upstreamMetadataCacheInfoStoreFactory.Get((IFeedRequest) new FeedRequest(feed, this.protocol)).GetMetadataCacheInfoAsync(feed);
      List<UpstreamPackagesToRefreshInformation> batchesOfPackagesToRefresh;
      if (metadataCacheInfoAsync?.PackageNames == null)
        batchesOfPackagesToRefresh = (List<UpstreamPackagesToRefreshInformation>) null;
      else if (!metadataCacheInfoAsync.PackageNames.Any<IPackageName>())
      {
        batchesOfPackagesToRefresh = (List<UpstreamPackagesToRefreshInformation>) null;
      }
      else
      {
        batchesOfPackagesToRefresh = this.refreshInformationConverter.GetListOfUpstreamPackagesToRefreshInformation(feed, metadataCacheInfoAsync, this.protocol).ToList<UpstreamPackagesToRefreshInformation>();
        Guid correlationId = Guid.NewGuid();
        foreach (UpstreamPackagesToRefreshInformation toRefreshInformation in batchesOfPackagesToRefresh)
        {
          int maxDelaySeconds = this.rng.Next(0, spreadPeriodSeconds);
          Guid guid = await this.feedJobQueuer.QueueJob(feed, this.protocol, JobPriorityLevel.Normal, toRefreshInformation, new Guid?(correlationId), batchesOfPackagesToRefresh.Count, maxDelaySeconds);
        }
        batchesOfPackagesToRefresh = (List<UpstreamPackagesToRefreshInformation>) null;
      }
    }

    private int GetFeedJobSpreadSeconds() => this.registryService.GetValue<int>(new RegistryQuery(string.Format("/Configuration/Packaging/{0}/UpstreamMetadataCache/RefreshFeedJobSpreadSeconds", (object) this.protocol.CorrectlyCasedName)), 900);
  }
}
