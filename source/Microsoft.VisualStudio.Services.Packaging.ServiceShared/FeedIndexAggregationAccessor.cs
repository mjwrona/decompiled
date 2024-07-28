// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.FeedIndexAggregationAccessor
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class FeedIndexAggregationAccessor : 
    IFeedIndexAggregationAccessor,
    IAggregationAccessor<FeedIndexAggregation>,
    IAggregationAccessor,
    IFeedIndexAggregationContext
  {
    private readonly FeedIndexCommitHandler commitHandler;

    public FeedIndexAggregationAccessor(
      IFeedIndexClient feedIndexClient,
      IFeedService feedService,
      IProtocol protocol,
      IFeedIndexCommitHandlerFactory commitFactory,
      ITracerService tracerService,
      FeedIndexAggregation aggregation)
    {
      this.FeedIndexClient = feedIndexClient;
      this.FeedService = feedService;
      this.Protocol = protocol;
      this.TracerService = tracerService;
      this.Aggregation = (IAggregation) aggregation;
      this.commitHandler = commitFactory.GetCommitHandler((IFeedIndexAggregationAccessor) this);
    }

    public IFeedIndexClient FeedIndexClient { get; }

    public IFeedService FeedService { get; }

    public IProtocol Protocol { get; }

    public ITracerService TracerService { get; }

    public IAggregation Aggregation { get; }

    public async Task ApplyCommitAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      foreach (ICommitLogEntry commitLogEntry in (IEnumerable<ICommitLogEntry>) commitLogEntries)
        await this.ApplyCommitAsync(feedRequest, commitLogEntry);
    }

    public async Task ApplyCommitAsync(IFeedRequest feedRequest, ICommitLogEntry commitLogEntry)
    {
      FeedIndexAggregationAccessor context = this;
      try
      {
        await context.commitHandler.ApplyCommitAsync((IFeedIndexAggregationContext) context, feedRequest.Feed, commitLogEntry);
      }
      catch (PackageVersionDeletedException ex)
      {
      }
    }
  }
}
