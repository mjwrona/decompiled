// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetV2DelistRequestHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.ListUnlistPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetV2DelistRequestHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest<ListingOperationRequestAdditionalData>, HttpResponseMessage>>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetV2DelistRequestHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest<ListingOperationRequestAdditionalData>, HttpResponseMessage> Bootstrap() => new RawPackageRequestWithDataConverter<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>(new NuGetRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap()).ThenDelegateTo<RawPackageRequest<ListingOperationRequestAdditionalData>, PackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>, HttpResponseMessage>(new NuGetWriteBootstrapper<PackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData, HttpResponseMessage>(this.requestContext, (IRequireAggHandlerBootstrapper<PackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData>) new NuGetDelistValidatingOpGeneratingHandlerBootstrapper(), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(PackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData), ICiData>) new UpdatePackageVersionRawRequestToCiDataFacadeHandler<PackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData>(this.requestContext), false).Bootstrap());
  }
}
