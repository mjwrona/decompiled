// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenCommitHandlerFactory
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenCommitHandlerFactory : IFeedIndexCommitHandlerFactory
  {
    private readonly IVssRequestContext requestContext;

    public MavenCommitHandlerFactory(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public FeedIndexCommitHandler GetCommitHandler(
      IFeedIndexAggregationAccessor feedIndexAggregationAccessor)
    {
      ScheduledPermanentDeleteDateResolvingHandler scheduledPermanentDeleteDateResolvingHandler = new ScheduledPermanentDeleteDateResolvingHandler(new ScheduledPermanentDeleteDateCalculatingHandlerBootstrapper(this.requestContext).Bootstrap());
      return (FeedIndexCommitHandler) new BatchOperationCommitHandler(feedIndexAggregationAccessor, (FeedIndexCommitHandler) new CommitOperationCommitHandler((FeedIndexCommitHandler) new UpdateUpstreamMetadataCommitHandler((FeedIndexCommitHandler) new UpdateUpstreamMetadataVersionCommitHandler((FeedIndexCommitHandler) new SnapshotCleanupOperationCommitHandler((FeedIndexCommitHandler) new DeleteCommitHandler((IAsyncHandler<IDeleteOperationData, DateTime>) scheduledPermanentDeleteDateResolvingHandler, (FeedIndexCommitHandler) new PermanentDeleteCommitHandler((FeedIndexCommitHandler) new ViewCommitHandler((FeedIndexCommitHandler) new RestoreToFeedCommitHandler((FeedIndexCommitHandler) new NoOperationCommitHandler((FeedIndexCommitHandler) new NoChangeCommitHandler<AddProblemPackageOperationData>((FeedIndexCommitHandler) new ThrowInvalidDataCommitHandler())))))))))));
    }
  }
}
