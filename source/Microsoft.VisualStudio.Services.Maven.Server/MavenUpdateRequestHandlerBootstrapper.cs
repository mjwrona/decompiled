// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenUpdateRequestHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Telemetry;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenUpdateRequestHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<MavenRawPackageRequest<PackageVersionDetails>, ICommitLogEntry>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenUpdateRequestHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<MavenRawPackageRequest<PackageVersionDetails>, ICommitLogEntry> Bootstrap() => (IAsyncHandler<MavenRawPackageRequest<PackageVersionDetails>, ICommitLogEntry>) new RawPackageRequestWithDataConverter<MavenPackageIdentity, PackageVersionDetails>(new MavenRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap()).ThenDelegateTo<RawPackageRequest<PackageVersionDetails>, PackageRequest<MavenPackageIdentity, PackageVersionDetails>, ICommitLogEntry>(new MavenWriteBootstrapper<PackageRequest<MavenPackageIdentity, PackageVersionDetails>, ICommitOperationData>(this.requestContext, (IRequireAggHandlerBootstrapper<PackageRequest<MavenPackageIdentity, PackageVersionDetails>, ICommitOperationData>) new MavenUpdateValidatingOpGeneratingHandlerBootstrapper(this.requestContext), (IAsyncHandler<(PackageRequest<MavenPackageIdentity, PackageVersionDetails>, ICommitOperationData), ICiData>) new MavenUpdatePackageCiDataFacadeHandler(this.requestContext), false).Bootstrap());
  }
}
