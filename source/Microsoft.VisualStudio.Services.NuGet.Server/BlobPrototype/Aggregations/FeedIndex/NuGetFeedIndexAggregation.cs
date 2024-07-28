// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.FeedIndex.NuGetFeedIndexAggregation
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.FeedIndex
{
  public class NuGetFeedIndexAggregation : FeedIndexAggregation
  {
    public static readonly NuGetFeedIndexAggregation V1 = new NuGetFeedIndexAggregation(nameof (V1));

    private NuGetFeedIndexAggregation(string name)
      : base((IProtocol) Protocol.NuGet, name, NuGetAggregationDefinitions.NuGetFeedIndexAggregationDefinition)
    {
    }

    public override IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      IFeedIndexClient feedIndexClient = this.GetFeedIndexClient(requestContext);
      IFeedService feedService1 = (IFeedService) new FeedServiceFacade(requestContext);
      IFeedIndexCommitHandlerFactory commitHandlerFactory = (IFeedIndexCommitHandlerFactory) new NuGetFeedIndexCommitHandlerFactory(requestContext);
      ITracerService tracerFacade = requestContext.GetTracerFacade();
      IFeedService feedService2 = feedService1;
      IProtocol protocol = this.protocol;
      IFeedIndexCommitHandlerFactory commitFactory = commitHandlerFactory;
      ITracerService tracerService = tracerFacade;
      return (IAggregationAccessor) new FeedIndexAggregationAccessor(feedIndexClient, feedService2, protocol, commitFactory, tracerService, (FeedIndexAggregation) this);
    }
  }
}
