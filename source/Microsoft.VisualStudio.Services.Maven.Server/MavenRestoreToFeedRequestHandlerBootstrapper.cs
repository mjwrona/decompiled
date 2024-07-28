// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenRestoreToFeedRequestHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenRestoreToFeedRequestHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest<IRecycleBinPackageVersionDetails>, ICommitLogEntry>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenRestoreToFeedRequestHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest<IRecycleBinPackageVersionDetails>, ICommitLogEntry> Bootstrap() => new MavenRawPackageRequestWithDataConverterBootstrapper<IRecycleBinPackageVersionDetails>(this.requestContext).Bootstrap().ThenDelegateTo<RawPackageRequest<IRecycleBinPackageVersionDetails>, PackageRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>, ICommitLogEntry>(new MavenWriteBootstrapper<PackageRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>(this.requestContext, (IRequireAggHandlerBootstrapper<PackageRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) new MavenRestoreToFeedValidatingOpGeneratingHandlerBootstrapper(), (IAsyncHandler<(PackageRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData), ICiData>) new RestoreToFeedCiDataFacadeHandler<MavenPackageIdentity>(this.requestContext), false).Bootstrap());
  }
}
