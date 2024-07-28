// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetWriteBootstrapper`3
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.Server.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetWriteBootstrapper<TReq, TOp, TResp> : IBootstrapper<IAsyncHandler<TReq, TResp>>
    where TReq : class, IFeedRequest
    where TOp : class, ICommitOperationData
  {
    private readonly IVssRequestContext requestContext;
    private readonly IRequireAggHandlerBootstrapper<TReq, TOp> requestToOpHandler;
    private readonly IAsyncHandler<ICommitLogEntry, TResp> commitEntryToResponseHandler;
    private readonly IAsyncHandler<(TReq, TOp), ICiData> requestToCiHandler;
    private readonly bool applyToAggregations;
    private readonly IAsyncHandler<TReq> finallyHandler;

    public NuGetWriteBootstrapper(
      IVssRequestContext requestContext,
      IRequireAggHandlerBootstrapper<TReq, TOp> requestToOpHandler,
      IAsyncHandler<ICommitLogEntry, TResp> commitEntryToResponseHandler,
      IAsyncHandler<(TReq, TOp), ICiData> requestToCiHandler,
      bool applyToAggregations,
      IAsyncHandler<TReq> finallyHandler = null)
    {
      this.requestContext = requestContext;
      this.requestToOpHandler = requestToOpHandler;
      this.commitEntryToResponseHandler = commitEntryToResponseHandler;
      this.requestToCiHandler = requestToCiHandler;
      this.applyToAggregations = applyToAggregations;
      this.finallyHandler = finallyHandler ?? (IAsyncHandler<TReq>) new DoNothingHandler<TReq>();
    }

    public IAsyncHandler<TReq, TResp> Bootstrap()
    {
      IMigrationDefinitionsProvider migrationsProvider = new NuGetMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap();
      IFactory<IAggregation, IAggregationAccessor> factory = new AggregationAccessorFactoryBootstrapper(this.requestContext).Bootstrap();
      ICommitLog commitLog = new NuGetCommitLogFacadeBootstrapper(this.requestContext).Bootstrap();
      IFactory<IAggregation, IAggregationAccessor> aggregationsProvider = factory;
      JobCreationInfo processingJobCreationInfo = NuGetServerConstants.ChangeProcessingJobConstants.NuGetChangeProcessingJobCreationInfo;
      ICommitLog commitLogFacade = commitLog;
      IVssRequestContext requestContext = this.requestContext;
      IRequireAggHandlerBootstrapper<TReq, TOp> requestToOpHandler = this.requestToOpHandler;
      IAsyncHandler<ICommitLogEntry, TResp> toResponseHandler = this.commitEntryToResponseHandler;
      IAsyncHandler<(TReq, TOp), ICiData> requestToCiHandler = this.requestToCiHandler;
      int num = this.applyToAggregations ? 1 : 0;
      IAsyncHandler<TReq> finallyHandler = this.finallyHandler;
      return new WriteBootstrapper<TReq, TOp, TResp>(migrationsProvider, aggregationsProvider, processingJobCreationInfo, commitLogFacade, requestContext, requestToOpHandler, toResponseHandler, requestToCiHandler, num != 0, (IBootstrapper<IAsyncHandler<(TReq, ICommitLogEntry), bool>>) null, (IAsyncHandler<TReq, NullResult>) finallyHandler).Bootstrap();
    }
  }
}
