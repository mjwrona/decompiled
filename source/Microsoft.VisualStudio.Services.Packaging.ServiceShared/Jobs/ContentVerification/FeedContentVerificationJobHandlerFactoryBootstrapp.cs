// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ContentVerification.FeedContentVerificationJobHandlerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ContentVerification
{
  public class FeedContentVerificationJobHandlerFactoryBootstrapper : 
    IBootstrapper<IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly ICommitLogReader<CommitLogEntry> commitLogFacade;
    private readonly BookmarkTokenKey contentVerificationScanningBookmarkTokenKey;
    private readonly IAsyncHandler<IContentVerificationRequest, NullResult> contentVerificationHandler;

    public FeedContentVerificationJobHandlerFactoryBootstrapper(
      IVssRequestContext requestContext,
      ICommitLogReader<CommitLogEntry> commitLogFacade,
      BookmarkTokenKey contentVerificationScanningBookmarkTokenKey,
      IAsyncHandler<IContentVerificationRequest, NullResult> contentVerificationHandler)
    {
      this.requestContext = requestContext;
      this.commitLogFacade = commitLogFacade;
      this.contentVerificationScanningBookmarkTokenKey = contentVerificationScanningBookmarkTokenKey;
      this.contentVerificationHandler = contentVerificationHandler;
    }

    public IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>> Bootstrap()
    {
      IExecutionEnvironment environmentFacade = this.requestContext.GetExecutionEnvironmentFacade();
      FlattenAndBatchCommitEnumeratingStrategy commitEnumerator = new FlattenAndBatchCommitEnumeratingStrategy(this.commitLogFacade, (IFactory<int>) new ByFuncFactory<int>((Func<int>) (() => 100)));
      PackageGDPRDataStoreBootstrapper storeBootstrapper = new PackageGDPRDataStoreBootstrapper(this.requestContext.To(TeamFoundationHostType.Deployment));
      ContentVerificationScanner contentVerificationScanner = new ContentVerificationScanner(this.contentVerificationHandler);
      FeedContentVerificationJobHandler feedContentVerificationJobHandler = new FeedContentVerificationJobHandler((ICommitEnumeratingStrategy) commitEnumerator, new CommitLogBookmarkTokenProviderBootstrapper(this.requestContext, this.contentVerificationScanningBookmarkTokenKey).Bootstrap(), environmentFacade, (IContentVerificationScanner) contentVerificationScanner, this.requestContext.GetTracerFacade());
      return (IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>) new ByFuncInputFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>((Func<JobId, IAsyncHandler<IFeedRequest, JobResult>>) (jobId => !((GuidBasedId) null == (GuidBasedId) jobId) ? new JobErrorHandlerBootstrapper<IFeedRequest>(this.requestContext, (IAsyncHandler<IFeedRequest, JobResult>) feedContentVerificationJobHandler, jobId).Bootstrap() : (IAsyncHandler<IFeedRequest, JobResult>) feedContentVerificationJobHandler));
    }
  }
}
