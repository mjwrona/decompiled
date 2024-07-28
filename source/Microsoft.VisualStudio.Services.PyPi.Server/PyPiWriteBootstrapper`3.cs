// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PyPiWriteBootstrapper`3
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.PyPi.Server.CommitLog;
using Microsoft.VisualStudio.Services.PyPi.Server.Constants;
using Microsoft.VisualStudio.Services.PyPi.Server.Migration;

namespace Microsoft.VisualStudio.Services.PyPi.Server
{
  public class PyPiWriteBootstrapper<TReq, TOp, TResp> : IBootstrapper<IAsyncHandler<TReq, TResp>>
    where TReq : class, IFeedRequest
    where TOp : class, ICommitOperationData
  {
    private readonly IVssRequestContext requestContext;
    private readonly IRequireAggHandlerBootstrapper<TReq, TOp> requestToOpHandler;
    private readonly IAsyncHandler<ICommitLogEntry, TResp> commitEntryToResponseHandler;
    private readonly IAsyncHandler<(TReq, TOp), ICiData> requestToCiHandler;
    private readonly bool applyToAggregations;
    private readonly IAsyncHandler<TReq, NullResult> finallyHandler;

    public PyPiWriteBootstrapper(
      IVssRequestContext requestContext,
      IRequireAggHandlerBootstrapper<TReq, TOp> requestToOpHandler,
      IAsyncHandler<ICommitLogEntry, TResp> commitEntryToResponseHandler,
      IAsyncHandler<(TReq, TOp), ICiData> requestToCiHandler,
      bool applyToAggregations,
      IAsyncHandler<TReq, NullResult> finallyHandler = null)
    {
      this.requestContext = requestContext;
      this.requestToOpHandler = requestToOpHandler;
      this.commitEntryToResponseHandler = commitEntryToResponseHandler;
      this.requestToCiHandler = requestToCiHandler;
      this.applyToAggregations = applyToAggregations;
      this.finallyHandler = finallyHandler ?? (IAsyncHandler<TReq, NullResult>) new DoNothingHandler<TReq>();
    }

    public IAsyncHandler<TReq, TResp> Bootstrap()
    {
      IMigrationDefinitionsProvider migrationsProvider = new PyPiMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap();
      IFactory<IAggregation, IAggregationAccessor> factory = new AggregationAccessorFactoryBootstrapper(this.requestContext).Bootstrap();
      ICommitLog commitLog = new PyPiCommitLogFacadeBootstrapper(this.requestContext).Bootstrap();
      GdprDataWriterBootstrapper<TReq> writerBootstrapper = new GdprDataWriterBootstrapper<TReq>(this.requestContext);
      IFactory<IAggregation, IAggregationAccessor> aggregationsProvider = factory;
      JobCreationInfo processingJobCreationInfo = PyPiJobConstants.ChangeProcessingJobConstants.PyPiChangeProcessingJobCreationInfo;
      ICommitLog commitLogFacade = commitLog;
      IVssRequestContext requestContext = this.requestContext;
      IRequireAggHandlerBootstrapper<TReq, TOp> requestToOpHandler = this.requestToOpHandler;
      IAsyncHandler<ICommitLogEntry, TResp> toResponseHandler = this.commitEntryToResponseHandler;
      IAsyncHandler<(TReq, TOp), ICiData> requestToCiHandler = this.requestToCiHandler;
      int num = this.applyToAggregations ? 1 : 0;
      GdprDataWriterBootstrapper<TReq> gdprDataHandlerBootstrapper = writerBootstrapper;
      IAsyncHandler<TReq, NullResult> finallyHandler = this.finallyHandler;
      return new WriteBootstrapper<TReq, TOp, TResp>(migrationsProvider, aggregationsProvider, processingJobCreationInfo, commitLogFacade, requestContext, requestToOpHandler, toResponseHandler, requestToCiHandler, num != 0, (IBootstrapper<IAsyncHandler<(TReq, ICommitLogEntry), bool>>) gdprDataHandlerBootstrapper, finallyHandler).Bootstrap();
    }
  }
}
