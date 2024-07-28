// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.UpdatePackageVersionHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class UpdatePackageVersionHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest<PackageVersionDetails>, NullResult>>
  {
    private readonly IVssRequestContext requestContext;

    public UpdatePackageVersionHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest<PackageVersionDetails>, NullResult> Bootstrap() => new RawPackageRequestWithDataConverter<VssNuGetPackageIdentity, PackageVersionDetails>(new NuGetRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap()).ThenDelegateTo<RawPackageRequest<PackageVersionDetails>, PackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, NullResult>(new NuGetWriteBootstrapper<PackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, ICommitOperationData, NullResult>(this.requestContext, (IRequireAggHandlerBootstrapper<PackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, ICommitOperationData>) new NuGetUpdatePackageVersionValidatingOpGeneratingHandlerBootstrapper(this.requestContext), (IAsyncHandler<ICommitLogEntry, NullResult>) new ByFuncAsyncHandler<ICommitLogEntry, NullResult>((Func<ICommitLogEntry, NullResult>) (c => (NullResult) null)), (IAsyncHandler<(PackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, ICommitOperationData), ICiData>) new UpdatePackageVersionRawRequestToCiDataFacadeHandler<PackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, ICommitOperationData>(this.requestContext), false).Bootstrap());
  }
}
