// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.UpdatePackageVersions.PyPiPackagesBatchBootstrapper
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
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.PyPi.Server.Converters.Identity;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.UPack.Server;
using System;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.UpdatePackageVersions
{
  public class PyPiPackagesBatchBootstrapper : 
    IBootstrapper<IAsyncHandler<BatchRawRequest, NullResult>>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiPackagesBatchBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<BatchRawRequest, NullResult> Bootstrap() => (IAsyncHandler<BatchRawRequest, NullResult>) new RawBatchPackageRequestValidatingConverter<PyPiPackageIdentity>(new PyPiRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap(), (IRegistryService) new RegistryServiceFacade(this.requestContext)).ThenDelegateTo<IBatchRawRequest, PackagesBatchRequest<PyPiPackageIdentity>, NullResult>(new PyPiWriteBootstrapper<PackagesBatchRequest<PyPiPackageIdentity>, BatchCommitOperationData, NullResult>(this.requestContext, (IRequireAggHandlerBootstrapper<PackagesBatchRequest<PyPiPackageIdentity>, BatchCommitOperationData>) new PyPiBatchValidatingOpGeneratingHandlerBootstrapper(this.requestContext), (IAsyncHandler<ICommitLogEntry, NullResult>) new ByFuncAsyncHandler<ICommitLogEntry, NullResult>((Func<ICommitLogEntry, NullResult>) (c => (NullResult) null)), (IAsyncHandler<(PackagesBatchRequest<PyPiPackageIdentity>, BatchCommitOperationData), ICiData>) new BatchCiDataFacadeHandler<PyPiPackageIdentity>(this.requestContext), false).Bootstrap());
  }
}
