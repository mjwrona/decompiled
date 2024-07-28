// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetPackagesBatchBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.UPack.Server;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetPackagesBatchBootstrapper : 
    IBootstrapper<IAsyncHandler<BatchRawRequest, NullResult>>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetPackagesBatchBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<BatchRawRequest, NullResult> Bootstrap() => (IAsyncHandler<BatchRawRequest, NullResult>) new RawBatchPackageRequestValidatingConverter<VssNuGetPackageIdentity>(new NuGetRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap(), (IRegistryService) new RegistryServiceFacade(this.requestContext)).ThenDelegateTo<IBatchRawRequest, PackagesBatchRequest<VssNuGetPackageIdentity>, NullResult>(new NuGetWriteBootstrapper<PackagesBatchRequest<VssNuGetPackageIdentity>, BatchCommitOperationData, NullResult>(this.requestContext, (IRequireAggHandlerBootstrapper<PackagesBatchRequest<VssNuGetPackageIdentity>, BatchCommitOperationData>) new NuGetBatchValidatingOpGeneratingHandlerBootstrapper(this.requestContext), (IAsyncHandler<ICommitLogEntry, NullResult>) new ByFuncAsyncHandler<ICommitLogEntry, NullResult>((Func<ICommitLogEntry, NullResult>) (c => (NullResult) null)), (IAsyncHandler<(PackagesBatchRequest<VssNuGetPackageIdentity>, BatchCommitOperationData), ICiData>) new BatchCiDataFacadeHandler<VssNuGetPackageIdentity>(this.requestContext), false).Bootstrap());
  }
}
