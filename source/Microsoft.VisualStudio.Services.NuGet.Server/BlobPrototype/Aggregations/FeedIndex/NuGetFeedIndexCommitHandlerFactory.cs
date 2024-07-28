// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.FeedIndex.NuGetFeedIndexCommitHandlerFactory
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.FeedIndex;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.FeedIndex
{
  public class NuGetFeedIndexCommitHandlerFactory : IFeedIndexCommitHandlerFactory
  {
    private readonly IVssRequestContext requestContext;

    public NuGetFeedIndexCommitHandlerFactory(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public FeedIndexCommitHandler GetCommitHandler(
      IFeedIndexAggregationAccessor feedIndexAggregationAccessor)
    {
      ScheduledPermanentDeleteDateResolvingHandler scheduledPermanentDeleteDateResolvingHandler = new ScheduledPermanentDeleteDateResolvingHandler(new ScheduledPermanentDeleteDateCalculatingHandlerBootstrapper(this.requestContext).Bootstrap());
      return (FeedIndexCommitHandler) new BatchOperationCommitHandler(feedIndexAggregationAccessor, (FeedIndexCommitHandler) new AddCommitHandler((FeedIndexCommitHandler) new UpdateUpstreamMetadataCommitHandler((FeedIndexCommitHandler) new UpdateUpstreamMetadataVersionCommitHandler((FeedIndexCommitHandler) new DeleteCommitHandler((IAsyncHandler<IDeleteOperationData, DateTime>) scheduledPermanentDeleteDateResolvingHandler, (FeedIndexCommitHandler) new PermanentDeleteCommitHandler((FeedIndexCommitHandler) new ViewCommitHandler((FeedIndexCommitHandler) new DelistCommitHandler((FeedIndexCommitHandler) new RelistCommitHandler((FeedIndexCommitHandler) new RestoreToFeedCommitHandler((FeedIndexCommitHandler) new NoOperationCommitHandler((FeedIndexCommitHandler) new NoChangeCommitHandler<AddProblemPackageOperationData>((FeedIndexCommitHandler) new ThrowInvalidDataCommitHandler()))))))))))));
    }
  }
}
