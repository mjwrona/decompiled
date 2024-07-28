// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.CargoWriteBootstrapper`3
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations;
using Microsoft.VisualStudio.Services.Cargo.Server.CommitLog;
using Microsoft.VisualStudio.Services.Cargo.Server.Constants;
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
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server
{
  [ExcludeFromCodeCoverage]
  public class CargoWriteBootstrapper<TReq, TOp, TResp> : IBootstrapper<IAsyncHandler<TReq, TResp>>
    where TReq : class, IFeedRequest
    where TOp : class, ICommitOperationData
  {
    private readonly IVssRequestContext requestContext;
    private readonly IRequireAggHandlerBootstrapper<TReq, TOp> requestToOpHandler;
    private readonly IAsyncHandler<ICommitLogEntry, TResp> commitEntryToResponseHandler;
    private readonly IAsyncHandler<(TReq, TOp), ICiData> requestToCiHandler;
    private readonly bool applyToAggregations;
    private readonly IAsyncHandler<TReq, NullResult> finallyHandler;

    public CargoWriteBootstrapper(
      IVssRequestContext requestContext,
      IRequireAggHandlerBootstrapper<TReq, TOp> requestToOpHandler,
      IAsyncHandler<ICommitLogEntry, TResp> commitEntryToResponseHandler,
      IAsyncHandler<(TReq, TOp), ICiData> requestToCiHandler,
      bool applyToAggregations,
      IAsyncHandler<TReq, NullResult>? finallyHandler = null)
    {
      this.requestContext = requestContext;
      this.requestToOpHandler = requestToOpHandler;
      this.commitEntryToResponseHandler = commitEntryToResponseHandler;
      this.requestToCiHandler = requestToCiHandler;
      this.applyToAggregations = applyToAggregations;
      this.finallyHandler = finallyHandler ?? (IAsyncHandler<TReq, NullResult>) new DoNothingHandler<TReq>();
    }

    public IAsyncHandler<TReq, TResp> Bootstrap() => new WriteBootstrapper<TReq, TOp, TResp>(new CargoMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), new AggregationAccessorFactoryBootstrapper(this.requestContext).Bootstrap(), CargoJobConstants.ChangeProcessingJobConstants.CargoChangeProcessingJobCreationInfo, new CargoCommitLogFacadeBootstrapper(this.requestContext).Bootstrap(), this.requestContext, this.requestToOpHandler, this.commitEntryToResponseHandler, this.requestToCiHandler, this.applyToAggregations, (IBootstrapper<IAsyncHandler<(TReq, ICommitLogEntry), bool>>) null, this.finallyHandler).Bootstrap();
  }
}
