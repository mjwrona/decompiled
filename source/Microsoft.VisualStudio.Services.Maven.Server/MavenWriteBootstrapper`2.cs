// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenWriteBootstrapper`2
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Constants;
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

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenWriteBootstrapper<TReq, TOp> : 
    IBootstrapper<IAsyncHandler<TReq, ICommitLogEntry>>
    where TReq : class, IFeedRequest
    where TOp : class, ICommitOperationData
  {
    private readonly IVssRequestContext requestContext;
    private readonly IRequireAggHandlerBootstrapper<TReq, TOp> requestToOpHandler;
    private readonly IAsyncHandler<(TReq, TOp), ICiData> requestToCiHandler;
    private readonly bool applyToAggregations;
    private readonly IAsyncHandler<TReq, NullResult> finallyHandler;

    public MavenWriteBootstrapper(
      IVssRequestContext requestContext,
      IRequireAggHandlerBootstrapper<TReq, TOp> requestToOpHandler,
      IAsyncHandler<(TReq, TOp), ICiData> requestToCiHandler,
      bool applyToAggregations,
      IAsyncHandler<TReq, NullResult> finallyHandler = null)
    {
      this.requestContext = requestContext;
      this.requestToOpHandler = requestToOpHandler;
      this.requestToCiHandler = requestToCiHandler;
      this.applyToAggregations = applyToAggregations;
      this.finallyHandler = finallyHandler ?? (IAsyncHandler<TReq, NullResult>) new DoNothingHandler<TReq>();
    }

    public IAsyncHandler<TReq, ICommitLogEntry> Bootstrap()
    {
      IMigrationDefinitionsProvider migrationsProvider = new MavenMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap();
      IFactory<IAggregation, IAggregationAccessor> factory = new AggregationAccessorFactoryBootstrapper(this.requestContext).Bootstrap();
      ICommitLog commitLog = new MavenCommitLogFacadeBootstrapper(this.requestContext).Bootstrap();
      GdprDataWriterBootstrapper<TReq> writerBootstrapper = new GdprDataWriterBootstrapper<TReq>(this.requestContext);
      IFactory<IAggregation, IAggregationAccessor> aggregationsProvider = factory;
      JobCreationInfo processingJobCreationInfo = MavenServerConstants.ChangeProcessingJobConstants.MavenChangeProcessingJobCreationInfo;
      ICommitLog commitLogFacade = commitLog;
      IVssRequestContext requestContext = this.requestContext;
      IRequireAggHandlerBootstrapper<TReq, TOp> requestToOpHandler = this.requestToOpHandler;
      ByFuncAsyncHandler<ICommitLogEntry, ICommitLogEntry> commitEntryToResponseHandler = ByFuncAsyncHandler.Identity<ICommitLogEntry>();
      IAsyncHandler<(TReq, TOp), ICiData> requestToCiHandler = this.requestToCiHandler;
      int num = this.applyToAggregations ? 1 : 0;
      GdprDataWriterBootstrapper<TReq> gdprDataHandlerBootstrapper = writerBootstrapper;
      IAsyncHandler<TReq, NullResult> finallyHandler = this.finallyHandler;
      return new WriteBootstrapper<TReq, TOp, ICommitLogEntry>(migrationsProvider, aggregationsProvider, processingJobCreationInfo, commitLogFacade, requestContext, requestToOpHandler, (IAsyncHandler<ICommitLogEntry, ICommitLogEntry>) commitEntryToResponseHandler, requestToCiHandler, num != 0, (IBootstrapper<IAsyncHandler<(TReq, ICommitLogEntry), bool>>) gdprDataHandlerBootstrapper, finallyHandler).Bootstrap();
    }
  }
}
