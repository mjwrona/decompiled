// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.NpmFindPackageVersionHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Upstreams;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.FindPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly
{
  public class NpmFindPackageVersionHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<RawPackageRequest, NullResult, INpmMetadataService, IUpstreamVersionListService<NpmPackageName, SemanticVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmFindPackageVersionHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<RawPackageRequest, NullResult> Bootstrap(
      INpmMetadataService metadataService,
      IUpstreamVersionListService<NpmPackageName, SemanticVersion> upstreamVersionListService)
    {
      ByFuncInputFactory<IFeedRequest, Task<IUpstreamMetadataManager>> upstreamMetadataManagerFactory = new ByFuncInputFactory<IFeedRequest, Task<IUpstreamMetadataManager>>((Func<IFeedRequest, Task<IUpstreamMetadataManager>>) (feed => Task.FromResult<IUpstreamMetadataManager>(new NpmUpstreamMetadataManagerExistingLocalMetadataBootstrapper(this.requestContext, (IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>) metadataService, upstreamVersionListService).Bootstrap())));
      return (IAsyncHandler<RawPackageRequest, NullResult>) new RawPackageRequestConverter<NpmPackageIdentity>(new NpmPackageIdentityParsingConverterBootstrapper(this.requestContext).Bootstrap()).ThenDelegateTo<IRawPackageRequest, PackageRequest<NpmPackageIdentity>, NullResult>((IAsyncHandler<PackageRequest<NpmPackageIdentity>, NullResult>) new FindPackageVersionHandler<NpmPackageIdentity, INpmMetadataEntry>((IAsyncHandler<PackageRequest<NpmPackageIdentity>, INpmMetadataEntry>) new NpmMetadataHandlerBootstrapper(this.requestContext).Bootstrap(), (IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>) upstreamMetadataManagerFactory, (IPackagingTraces) new PackagingTracesFacade(this.requestContext)));
    }
  }
}
