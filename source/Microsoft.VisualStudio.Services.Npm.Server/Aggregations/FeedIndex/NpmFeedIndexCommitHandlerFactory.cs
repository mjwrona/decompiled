// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Aggregations.FeedIndex.NpmFeedIndexCommitHandlerFactory
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.Aggregations.FeedIndex
{
  public class NpmFeedIndexCommitHandlerFactory : IFeedIndexCommitHandlerFactory
  {
    private readonly IVssRequestContext requestContext;

    public NpmFeedIndexCommitHandlerFactory(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public FeedIndexCommitHandler GetCommitHandler(
      IFeedIndexAggregationAccessor feedIndexAggregationAccessor)
    {
      ScheduledPermanentDeleteDateResolvingHandler scheduledPermanentDeleteDateResolvingHandler = new ScheduledPermanentDeleteDateResolvingHandler(new ScheduledPermanentDeleteDateCalculatingHandlerBootstrapper(this.requestContext).Bootstrap());
      return (FeedIndexCommitHandler) new BatchOperationCommitHandler(feedIndexAggregationAccessor, (FeedIndexCommitHandler) new AddCommitHandler((FeedIndexCommitHandler) new UpdateUpstreamMetadataCommitHandler((FeedIndexCommitHandler) new UpdateUpstreamMetadataVersionCommitHandler((FeedIndexCommitHandler) new NpmDistTagRemoveCommitHandler((FeedIndexCommitHandler) new NpmDistTagSetCommitHandler((FeedIndexCommitHandler) new NpmDeprecateCommitHandler((FeedIndexCommitHandler) new DeleteCommitHandler((IAsyncHandler<IDeleteOperationData, DateTime>) scheduledPermanentDeleteDateResolvingHandler, (FeedIndexCommitHandler) new PermanentDeleteCommitHandler((FeedIndexCommitHandler) new ViewCommitHandler((FeedIndexCommitHandler) new NpmMetadataDiffCommitHandler((FeedIndexCommitHandler) new ClearUpstreamCacheCommitHandler((FeedIndexCommitHandler) new NpmUpgradeCommitHandler((FeedIndexCommitHandler) new RestoreToFeedCommitHandler((FeedIndexCommitHandler) new NoOperationCommitHandler((FeedIndexCommitHandler) new NoChangeCommitHandler<AddProblemPackageOperationData>((FeedIndexCommitHandler) new ThrowInvalidDataCommitHandler()))))))))))))))));
    }
  }
}
