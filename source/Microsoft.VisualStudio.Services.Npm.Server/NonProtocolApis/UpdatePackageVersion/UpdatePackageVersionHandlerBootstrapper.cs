// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersion.UpdatePackageVersionHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Telemetry;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types.API;
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

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersion
{
  public class UpdatePackageVersionHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest<PackageVersionDetails>, NullResult>>
  {
    private readonly IVssRequestContext requestContext;

    public UpdatePackageVersionHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest<PackageVersionDetails>, NullResult> Bootstrap() => new RawPackageRequestWithDataConverter<NpmPackageIdentity, PackageVersionDetails>(new NpmRawPackageRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap().Select<IRawPackageRequest, PackageRequest<NpmPackageIdentity>, NpmPackageIdentity>((Func<PackageRequest<NpmPackageIdentity>, NpmPackageIdentity>) (r => r.PackageId))).ThenDelegateTo<RawPackageRequest<PackageVersionDetails>, PackageRequest<NpmPackageIdentity, PackageVersionDetails>, NullResult>(new NpmWriteBootstrapper<PackageRequest<NpmPackageIdentity, PackageVersionDetails>, ICommitOperationData, NullResult>(this.requestContext, NoAggregationRequiredReturnSameInstanceBootstrapper.Create<PackageRequest<NpmPackageIdentity, PackageVersionDetails>, ICommitOperationData>(new UpdatePackageVersionValidatingHandlerBootstrapper(this.requestContext).Bootstrap()), (IAsyncHandler<ICommitLogEntry, NullResult>) new ByFuncAsyncHandler<ICommitLogEntry, NullResult>((Func<ICommitLogEntry, NullResult>) (c => (NullResult) null)), (IAsyncHandler<(PackageRequest<NpmPackageIdentity, PackageVersionDetails>, ICommitOperationData), ICiData>) new NpmUpdatePackageVersionCiDataHandler(this.requestContext), false).Bootstrap());
  }
}
