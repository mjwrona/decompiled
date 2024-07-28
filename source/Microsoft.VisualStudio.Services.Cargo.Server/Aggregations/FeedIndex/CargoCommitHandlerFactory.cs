// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.FeedIndex.CargoCommitHandlerFactory
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.FeedIndex;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.FeedIndex
{
  public class CargoCommitHandlerFactory : IFeedIndexCommitHandlerFactory
  {
    private readonly IVssRequestContext requestContext;

    public CargoCommitHandlerFactory(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public FeedIndexCommitHandler GetCommitHandler(
      IFeedIndexAggregationAccessor feedIndexAggregationAccessor)
    {
      ScheduledPermanentDeleteDateResolvingHandler scheduledPermanentDeleteDateResolvingHandler = new ScheduledPermanentDeleteDateResolvingHandler(new ScheduledPermanentDeleteDateCalculatingHandlerBootstrapper(this.requestContext).Bootstrap());
      return (FeedIndexCommitHandler) new BatchOperationCommitHandler(feedIndexAggregationAccessor, (FeedIndexCommitHandler) new AddCommitHandler((FeedIndexCommitHandler) new UpdateUpstreamMetadataCommitHandler((FeedIndexCommitHandler) new DeleteCommitHandler((IAsyncHandler<IDeleteOperationData, DateTime>) scheduledPermanentDeleteDateResolvingHandler, (FeedIndexCommitHandler) new PermanentDeleteCommitHandler((FeedIndexCommitHandler) new ViewCommitHandler((FeedIndexCommitHandler) new DelistCommitHandler((FeedIndexCommitHandler) new RelistCommitHandler((FeedIndexCommitHandler) new RestoreToFeedCommitHandler((FeedIndexCommitHandler) new NoOperationCommitHandler((FeedIndexCommitHandler) new NoChangeCommitHandler<AddProblemPackageOperationData>((FeedIndexCommitHandler) new ThrowInvalidDataCommitHandler())))))))))));
    }
  }
}
