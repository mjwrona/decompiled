// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.WriteBootstrapper`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class WriteBootstrapper<TReq, TOp, TResp> : IBootstrapper<IAsyncHandler<TReq, TResp>>
    where TReq : class, IFeedRequest
    where TOp : class, ICommitOperationData
  {
    private readonly IVssRequestContext requestContext;
    private readonly IRequireAggHandlerBootstrapper<TReq, TOp> requestToOpHandler;
    private readonly IAsyncHandler<ICommitLogEntry, TResp> commitEntryToResponseHandler;
    private readonly IAsyncHandler<(TReq, TOp), ICiData> requestToCiHandler;
    private readonly bool applyToAggregations;
    private readonly IAsyncHandler<TReq, NullResult> finallyHandler;
    private readonly JobCreationInfo feedChangeProcessingJobCreationInfo;
    private readonly IMigrationDefinitionsProvider migrationsProvider;
    private readonly IFactory<IAggregation, IAggregationAccessor> aggregationsProvider;
    private readonly ICommitLog commitLogFacade;
    private IBootstrapper<IAsyncHandler<(TReq, ICommitLogEntry), bool>> gdprDataHandlerBootstrapper;

    public WriteBootstrapper(
      IMigrationDefinitionsProvider migrationsProvider,
      IFactory<IAggregation, IAggregationAccessor> aggregationsProvider,
      JobCreationInfo feedChangeProcessingJobCreationInfo,
      ICommitLog commitLogFacade,
      IVssRequestContext requestContext,
      IRequireAggHandlerBootstrapper<TReq, TOp> requestToOpHandler,
      IAsyncHandler<ICommitLogEntry, TResp> commitEntryToResponseHandler,
      IAsyncHandler<(TReq, TOp), ICiData> requestToCiHandler,
      bool applyToAggregations,
      IBootstrapper<IAsyncHandler<(TReq, ICommitLogEntry), bool>> gdprDataHandlerBootstrapper,
      IAsyncHandler<TReq, NullResult> finallyHandler = null)
    {
      this.migrationsProvider = migrationsProvider;
      this.aggregationsProvider = aggregationsProvider;
      this.feedChangeProcessingJobCreationInfo = feedChangeProcessingJobCreationInfo;
      this.requestContext = requestContext;
      this.requestToOpHandler = requestToOpHandler;
      this.commitEntryToResponseHandler = commitEntryToResponseHandler;
      this.requestToCiHandler = requestToCiHandler;
      this.applyToAggregations = applyToAggregations;
      this.finallyHandler = finallyHandler ?? (IAsyncHandler<TReq, NullResult>) new DoNothingHandler<TReq>();
      this.commitLogFacade = commitLogFacade;
      this.gdprDataHandlerBootstrapper = gdprDataHandlerBootstrapper;
    }

    public IAsyncHandler<TReq, TResp> Bootstrap()
    {
      IFeedPerms permsFacade = (IFeedPerms) new FeedPermsFacade(this.requestContext);
      IAggregationAccessorFactory readAggregationAccessorFactory = new ReadAggregationAccessorFactoryBootstrapper(this.requestContext, this.migrationsProvider, this.aggregationsProvider).Bootstrap();
      IAggregationAccessorFactory writeAggregationAccessorFactory = new WriteAggregationAccessorFactoryBootstrapper(this.requestContext, this.migrationsProvider, this.aggregationsProvider).Bootstrap();
      DependencyResolvingAggregationCommitApplier aggregationCommitApplier = new DependencyResolvingAggregationCommitApplierBootstrapper(this.requestContext, true).Bootstrap();
      IFeedJobQueuer feedChangeProcessingJobQueuer = new ChangeProcessingFeedJobQueuerBootstrapper(this.requestContext, this.feedChangeProcessingJobCreationInfo, (ICommitLogEndpointReader) this.commitLogFacade).Bootstrap();
      IAsyncHandler<(TReq, ICommitLogEntry)> forwardingToThisHandler = new RequestAndCommitToOpHandler<TReq, TOp>().ThenDelegateTo<(TReq, ICommitLogEntry), (TReq, TOp), ICiData>(this.requestToCiHandler).ThenDelegateTo<(TReq, ICommitLogEntry), ICiData>((IAsyncHandler<ICiData>) new TelemetryPublisherUsingTracerFacadeBootstrapper(this.requestContext).Bootstrap());
      return new ChangeApplier<TReq, TOp>((ICommitLogWriter<ICommitLogEntry>) this.commitLogFacade, readAggregationAccessorFactory, writeAggregationAccessorFactory, feedChangeProcessingJobQueuer, this.requestToOpHandler, permsFacade, this.applyToAggregations, this.requestContext.GetTracerFacade(), (IAggregationCommitApplier) aggregationCommitApplier, this.requestContext.GetFeatureFlagFacade(), this.finallyHandler, (ICache<string, object>) new RequestContextItemsAsCacheFacade(this.requestContext)).ThenForwardOriginalRequestAndResultTo<TReq, ICommitLogEntry>(new GdprDataHandlerLazyBootstrapper<TReq>(this.requestContext.GetExecutionEnvironmentFacade(), this.gdprDataHandlerBootstrapper).Bootstrap()).ThenForwardOriginalRequestAndResultTo<TReq, ICommitLogEntry>(forwardingToThisHandler).ThenDelegateTo<TReq, ICommitLogEntry, TResp>(this.commitEntryToResponseHandler);
    }
  }
}
