// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.FeedIndex.CargoFeedIndexAggregation
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.FeedIndex
{
  public class CargoFeedIndexAggregation : FeedIndexAggregation
  {
    public static readonly CargoFeedIndexAggregation V1 = new CargoFeedIndexAggregation(nameof (V1));

    private CargoFeedIndexAggregation(string name)
      : base((IProtocol) Protocol.Cargo, name, AggregationDefinitions.CargoFeedIndexAggregationDefinition)
    {
    }

    public override IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      IFeedIndexClient feedIndexClient = this.GetFeedIndexClient(requestContext);
      IFeedService feedService1 = (IFeedService) new FeedServiceFacade(requestContext);
      IFeedIndexCommitHandlerFactory commitHandlerFactory = (IFeedIndexCommitHandlerFactory) new CargoCommitHandlerFactory(requestContext);
      ITracerService tracerFacade = requestContext.GetTracerFacade();
      IFeedService feedService2 = feedService1;
      IProtocol protocol = this.protocol;
      IFeedIndexCommitHandlerFactory commitFactory = commitHandlerFactory;
      ITracerService tracerService = tracerFacade;
      return (IAggregationAccessor) new FeedIndexAggregationAccessor(feedIndexClient, feedService2, protocol, commitFactory, tracerService, (FeedIndexAggregation) this);
    }
  }
}
