// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetPermanentDeleteRequestHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.UPack.Server.Controllers.Api;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetPermanentDeleteRequestHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest, HttpResponseMessage>>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetPermanentDeleteRequestHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest, HttpResponseMessage> Bootstrap() => (IAsyncHandler<RawPackageRequest, HttpResponseMessage>) new RawPackageRequestConverter<VssNuGetPackageIdentity>(new NuGetRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap()).ThenDelegateTo<IRawPackageRequest, PackageRequest<VssNuGetPackageIdentity>, HttpResponseMessage>(new NuGetWriteBootstrapper<PackageRequest<VssNuGetPackageIdentity>, IPermanentDeleteOperationData, HttpResponseMessage>(this.requestContext, (IRequireAggHandlerBootstrapper<PackageRequest<VssNuGetPackageIdentity>, IPermanentDeleteOperationData>) new PermanentDeleteValidatingOpGeneratingHandlerBootstrapper<VssNuGetPackageIdentity, INuGetMetadataEntry, INuGetMetadataService>(), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(PackageRequest<VssNuGetPackageIdentity>, IPermanentDeleteOperationData), ICiData>) new GetNuGetPermanentDeleteCiDataFacadeHandler(this.requestContext), false).Bootstrap());
  }
}
