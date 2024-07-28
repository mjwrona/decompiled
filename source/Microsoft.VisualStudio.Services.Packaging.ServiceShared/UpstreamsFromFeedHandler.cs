// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamsFromFeedHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class UpstreamsFromFeedHandler : IHandler<IFeedRequest, IEnumerable<UpstreamSource>>
  {
    private readonly IFeatureFlagService featureFlagService;
    private readonly UpstreamSourceType? sourceTypeFilter;

    public UpstreamsFromFeedHandler(
      IFeatureFlagService featureFlagService,
      UpstreamSourceType? sourceTypeFilter = null)
    {
      this.featureFlagService = featureFlagService;
      this.sourceTypeFilter = sourceTypeFilter;
    }

    public IEnumerable<UpstreamSource> Handle(IFeedRequest feedRequest)
    {
      FeedCore feed = feedRequest.Feed;
      if (!feed.UpstreamEnabled)
        return Enumerable.Empty<UpstreamSource>();
      if (feed.View != null)
        return Enumerable.Empty<UpstreamSource>();
      IEnumerable<UpstreamSource> source1 = feed.GetSourcesForProtocol(feedRequest.Protocol).Where<UpstreamSource>((Func<UpstreamSource, bool>) (u => this.featureFlagService.IsEnabled("Packaging.InternalUpstreams") || u.UpstreamSourceType != UpstreamSourceType.Internal));
      if (this.sourceTypeFilter.HasValue)
        source1 = source1.Where<UpstreamSource>((Func<UpstreamSource, bool>) (source =>
        {
          int upstreamSourceType = (int) source.UpstreamSourceType;
          UpstreamSourceType? sourceTypeFilter = this.sourceTypeFilter;
          int valueOrDefault = (int) sourceTypeFilter.GetValueOrDefault();
          return upstreamSourceType == valueOrDefault & sourceTypeFilter.HasValue;
        }));
      return source1;
    }
  }
}
