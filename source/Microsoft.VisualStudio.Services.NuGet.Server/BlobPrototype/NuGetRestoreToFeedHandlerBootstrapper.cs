// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetRestoreToFeedHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.OperationsData;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetRestoreToFeedHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest<IRecycleBinPackageVersionDetails>, HttpResponseMessage>>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetRestoreToFeedHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest<IRecycleBinPackageVersionDetails>, HttpResponseMessage> Bootstrap() => new RawPackageRequestWithDataConverter<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>(new NuGetRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap()).ThenDelegateTo<RawPackageRequest<IRecycleBinPackageVersionDetails>, PackageRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>, HttpResponseMessage>(new NuGetWriteBootstrapper<PackageRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>, NuGetRestoreToFeedOperationData, HttpResponseMessage>(this.requestContext, (IRequireAggHandlerBootstrapper<PackageRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>, NuGetRestoreToFeedOperationData>) new NuGetRestoreToFeedValidatingOpGeneratingHandlerBootstrapper(this.requestContext), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(PackageRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>, NuGetRestoreToFeedOperationData), ICiData>) new GetNuGetRestoreToFeedCiDataFacadeHandler(this.requestContext), false).Bootstrap());
  }
}
