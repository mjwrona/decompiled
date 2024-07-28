// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenFeedIndexAggregation
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenFeedIndexAggregation : FeedIndexAggregation
  {
    public static readonly MavenFeedIndexAggregation V1 = new MavenFeedIndexAggregation(nameof (V1));

    private MavenFeedIndexAggregation(string name)
      : base((IProtocol) Protocol.Maven, name, AggregationDefinitions.MavenFeedIndexAggregationDefinition)
    {
    }

    public override IAggregationAccessor Bootstrap(IVssRequestContext requestContext)
    {
      IFeedIndexClient feedIndexClient = this.GetFeedIndexClient(requestContext);
      IFeedService feedService1 = (IFeedService) new FeedServiceFacade(requestContext);
      IFeedIndexCommitHandlerFactory commitHandlerFactory = (IFeedIndexCommitHandlerFactory) new MavenCommitHandlerFactory(requestContext);
      ITracerService tracerFacade = requestContext.GetTracerFacade();
      IFeedService feedService2 = feedService1;
      IProtocol protocol = this.protocol;
      IFeedIndexCommitHandlerFactory commitFactory = commitHandlerFactory;
      ITracerService tracerService = tracerFacade;
      return (IAggregationAccessor) new FeedIndexAggregationAccessor(feedIndexClient, feedService2, protocol, commitFactory, tracerService, (FeedIndexAggregation) this);
    }
  }
}
