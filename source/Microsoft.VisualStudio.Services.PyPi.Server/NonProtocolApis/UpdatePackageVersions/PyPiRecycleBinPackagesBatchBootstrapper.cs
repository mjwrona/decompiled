// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.UpdatePackageVersions.PyPiRecycleBinPackagesBatchBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.PyPi.Server.Converters.Identity;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.UPack.Server;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.UpdatePackageVersions
{
  public class PyPiRecycleBinPackagesBatchBootstrapper : 
    IBootstrapper<IAsyncHandler<BatchRawRequest, HttpResponseMessage>>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiRecycleBinPackagesBatchBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<BatchRawRequest, HttpResponseMessage> Bootstrap() => (IAsyncHandler<BatchRawRequest, HttpResponseMessage>) new RawBatchPackageRequestValidatingConverter<PyPiPackageIdentity>(new PyPiRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap(), (IRegistryService) new RegistryServiceFacade(this.requestContext)).ThenDelegateTo<IBatchRawRequest, PackagesBatchRequest<PyPiPackageIdentity>, HttpResponseMessage>(new PyPiWriteBootstrapper<PackagesBatchRequest<PyPiPackageIdentity>, BatchCommitOperationData, HttpResponseMessage>(this.requestContext, (IRequireAggHandlerBootstrapper<PackagesBatchRequest<PyPiPackageIdentity>, BatchCommitOperationData>) new PyPiRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper(this.requestContext), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(PackagesBatchRequest<PyPiPackageIdentity>, BatchCommitOperationData), ICiData>) new BatchCiDataFacadeHandler<PyPiPackageIdentity>(this.requestContext), false).Bootstrap());
  }
}
