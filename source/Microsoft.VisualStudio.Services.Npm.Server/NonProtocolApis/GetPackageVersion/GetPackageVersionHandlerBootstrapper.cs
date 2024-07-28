// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.GetPackageVersion.GetPackageVersionHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Constants;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Telemetry;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.GetPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.GetPackageVersion
{
  public class GetPackageVersionHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest, Package>>
  {
    private readonly IVssRequestContext requestContext;

    public GetPackageVersionHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest, Package> Bootstrap() => (IAsyncHandler<RawPackageRequest, Package>) new NpmRawPackageRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap().Select<IRawPackageRequest, PackageRequest<NpmPackageIdentity>, PackageRequest<NpmPackageIdentity, ShowDeletedBool>>((Func<PackageRequest<NpmPackageIdentity>, PackageRequest<NpmPackageIdentity, ShowDeletedBool>>) (r => new PackageRequest<NpmPackageIdentity, ShowDeletedBool>((IPackageRequest<NpmPackageIdentity>) r, new ShowDeletedBool(true)))).ThenDelegateTo<IRawPackageRequest, PackageRequest<NpmPackageIdentity, ShowDeletedBool>, Package>(new GetPackageVersionHandler<NpmPackageIdentity, INpmMetadataEntry, Package>((IConverter<INpmMetadataEntry, Package>) new NpmMetadataEntryToPackageConverter(), (IAsyncHandler<PackageRequest<NpmPackageIdentity>, INpmMetadataEntry>) new NpmMetadataHandlerBootstrapper(this.requestContext).Bootstrap()).ThenForwardOriginalRequestTo<PackageRequest<NpmPackageIdentity, ShowDeletedBool>, Package>((IAsyncHandler<PackageRequest<NpmPackageIdentity, ShowDeletedBool>, NullResult>) new GetPackageCiDataFacadeHandler(this.requestContext).ThenDelegateTo<IPackageRequest<NpmPackageIdentity>, ICiData>((IAsyncHandler<ICiData>) new TelemetryBootstrapper(this.requestContext, NpmTracePoints.Telemetry.TraceInfo).Bootstrap())));
  }
}
