// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Aggregations.FeedIndex.PyPiFeedIndexAggregation
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.PyPi.Server.Migration;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Aggregations.FeedIndex
{
  public class PyPiFeedIndexAggregation : FeedIndexAggregation
  {
    public static readonly PyPiFeedIndexAggregation V1 = new PyPiFeedIndexAggregation(nameof (V1));

    private PyPiFeedIndexAggregation(string name)
      : base((IProtocol) Protocol.PyPi, name, AggregationDefinitions.PyPiFeedIndexAggregationDefinition)
    {
    }

    public override IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      IFeedIndexClient feedIndexClient = this.GetFeedIndexClient(requestContext);
      IFeedService feedService1 = (IFeedService) new FeedServiceFacade(requestContext);
      IFeedIndexCommitHandlerFactory commitHandlerFactory = (IFeedIndexCommitHandlerFactory) new PyPiCommitHandlerFactory(requestContext);
      ITracerService tracerFacade = requestContext.GetTracerFacade();
      IFeedService feedService2 = feedService1;
      IProtocol protocol = this.protocol;
      IFeedIndexCommitHandlerFactory commitFactory = commitHandlerFactory;
      ITracerService tracerService = tracerFacade;
      return (IAggregationAccessor) new FeedIndexAggregationAccessor(feedIndexClient, feedService2, protocol, commitFactory, tracerService, (FeedIndexAggregation) this);
    }
  }
}
