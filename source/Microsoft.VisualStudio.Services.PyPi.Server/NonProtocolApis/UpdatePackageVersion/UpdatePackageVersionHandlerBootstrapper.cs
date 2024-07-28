// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.UpdatePackageVersion.UpdatePackageVersionHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.PyPi.Server.Converters.Identity;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.Telemetry;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API;
using System;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.UpdatePackageVersion
{
  public class UpdatePackageVersionHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest<PackageVersionDetails>, NullResult>>
  {
    private readonly IVssRequestContext requestContext;

    public UpdatePackageVersionHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest<PackageVersionDetails>, NullResult> Bootstrap() => new RawPackageRequestWithDataConverter<PyPiPackageIdentity, PackageVersionDetails>(new PyPiRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap()).ThenDelegateTo<RawPackageRequest<PackageVersionDetails>, PackageRequest<PyPiPackageIdentity, PackageVersionDetails>, NullResult>(new PyPiWriteBootstrapper<PackageRequest<PyPiPackageIdentity, PackageVersionDetails>, ICommitOperationData, NullResult>(this.requestContext, NoAggregationRequiredReturnSameInstanceBootstrapper.Create<PackageRequest<PyPiPackageIdentity, PackageVersionDetails>, ICommitOperationData>(new UpdatePackageVersionValidatingHandlerBootstrapper(this.requestContext).Bootstrap()), (IAsyncHandler<ICommitLogEntry, NullResult>) new ByFuncAsyncHandler<ICommitLogEntry, NullResult>((Func<ICommitLogEntry, NullResult>) (c => (NullResult) null)), (IAsyncHandler<(PackageRequest<PyPiPackageIdentity, PackageVersionDetails>, ICommitOperationData), ICiData>) new PyPiUpdatePackageVersionCiDataHandler<PackageRequest<PyPiPackageIdentity, PackageVersionDetails>, ICommitOperationData>(this.requestContext), false).Bootstrap());
  }
}
