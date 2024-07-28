// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.PermanentDelete.PyPiPermanentDeleteRequestHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.PyPi.Server.Converters.Identity;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.PermanentDelete
{
  internal class PyPiPermanentDeleteRequestHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest, HttpResponseMessage>>
  {
    private IVssRequestContext requestContext;

    public PyPiPermanentDeleteRequestHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest, HttpResponseMessage> Bootstrap()
    {
      IAsyncHandler<PackageRequest<PyPiPackageIdentity>, HttpResponseMessage> handler = new PyPiWriteBootstrapper<PackageRequest<PyPiPackageIdentity>, IPermanentDeleteOperationData, HttpResponseMessage>(this.requestContext, (IRequireAggHandlerBootstrapper<PackageRequest<PyPiPackageIdentity>, IPermanentDeleteOperationData>) new PyPiPermanentDeleteValidatingOpGeneratingHandlerBootstrapper(), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(PackageRequest<PyPiPackageIdentity>, IPermanentDeleteOperationData), ICiData>) new PermanentDeleteCiDataFacadeHandler<PyPiPackageIdentity>(this.requestContext), false).Bootstrap();
      return (IAsyncHandler<RawPackageRequest, HttpResponseMessage>) new PyPiRawPackageRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap().ThenDelegateTo<IRawPackageRequest, PackageRequest<PyPiPackageIdentity>, HttpResponseMessage>(handler);
    }
  }
}
