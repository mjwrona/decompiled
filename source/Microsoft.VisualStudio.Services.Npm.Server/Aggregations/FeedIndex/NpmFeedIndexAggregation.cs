// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Aggregations.FeedIndex.NpmFeedIndexAggregation
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;

namespace Microsoft.VisualStudio.Services.Npm.Server.Aggregations.FeedIndex
{
  public class NpmFeedIndexAggregation : FeedIndexAggregation
  {
    public static readonly NpmFeedIndexAggregation V1 = new NpmFeedIndexAggregation(nameof (V1));

    private NpmFeedIndexAggregation(string name)
      : base((IProtocol) Protocol.npm, name, NpmAggregationDefinitions.NpmFeedIndexAggregationDefinition)
    {
    }

    public override IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      IFeedIndexClient feedIndexClient = this.GetFeedIndexClient(requestContext);
      IFeedService feedService1 = (IFeedService) new FeedServiceFacade(requestContext);
      IFeedIndexCommitHandlerFactory commitHandlerFactory = (IFeedIndexCommitHandlerFactory) new NpmFeedIndexCommitHandlerFactory(requestContext);
      ITracerService tracerFacade = requestContext.GetTracerFacade();
      IFeedService feedService2 = feedService1;
      IProtocol protocol = this.protocol;
      IFeedIndexCommitHandlerFactory commitFactory = commitHandlerFactory;
      ITracerService tracerService = tracerFacade;
      return (IAggregationAccessor) new FeedIndexAggregationAccessor(feedIndexClient, feedService2, protocol, commitFactory, tracerService, (FeedIndexAggregation) this);
    }
  }
}
