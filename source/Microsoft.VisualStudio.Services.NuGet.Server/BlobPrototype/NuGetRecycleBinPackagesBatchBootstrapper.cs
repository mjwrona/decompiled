// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetRecycleBinPackagesBatchBootstrapper
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
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.UPack.Server;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetRecycleBinPackagesBatchBootstrapper : 
    IBootstrapper<IAsyncHandler<BatchRawRequest, HttpResponseMessage>>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetRecycleBinPackagesBatchBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<BatchRawRequest, HttpResponseMessage> Bootstrap() => (IAsyncHandler<BatchRawRequest, HttpResponseMessage>) new RawBatchPackageRequestValidatingConverter<VssNuGetPackageIdentity>(new NuGetRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap(), (IRegistryService) new RegistryServiceFacade(this.requestContext)).ThenDelegateTo<IBatchRawRequest, PackagesBatchRequest<VssNuGetPackageIdentity>, HttpResponseMessage>(new NuGetWriteBootstrapper<PackagesBatchRequest<VssNuGetPackageIdentity>, BatchCommitOperationData, HttpResponseMessage>(this.requestContext, (IRequireAggHandlerBootstrapper<PackagesBatchRequest<VssNuGetPackageIdentity>, BatchCommitOperationData>) new NuGetRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper(this.requestContext), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(PackagesBatchRequest<VssNuGetPackageIdentity>, BatchCommitOperationData), ICiData>) new BatchCiDataFacadeHandler<VssNuGetPackageIdentity>(this.requestContext), false).Bootstrap());
  }
}
