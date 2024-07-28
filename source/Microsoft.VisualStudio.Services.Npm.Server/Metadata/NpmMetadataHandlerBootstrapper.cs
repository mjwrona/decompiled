// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Metadata.NpmMetadataHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Upstreams;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Metadata
{
  public class NpmMetadataHandlerBootstrapper : 
    IBootstrapper<
    #nullable disable
    IAsyncHandler<IPackageRequest<NpmPackageIdentity>, INpmMetadataEntry>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmMetadataHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<IPackageRequest<NpmPackageIdentity>, INpmMetadataEntry> Bootstrap() => NpmAggregationResolver.Bootstrap(this.requestContext).HandlerFor<IPackageRequest<NpmPackageIdentity>, INpmMetadataEntry, INpmPackageMetadataAggregationAccessor, IUpstreamVersionListService<NpmPackageName, SemanticVersion>>((Func<INpmPackageMetadataAggregationAccessor, IUpstreamVersionListService<NpmPackageName, SemanticVersion>, IAsyncHandler<IPackageRequest<NpmPackageIdentity>, INpmMetadataEntry>>) ((metadataAccessor, upstreamVersionListService) =>
    {
      IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry> upstreamFetchingMetadata = new NpmUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>) metadataAccessor, upstreamVersionListService).Bootstrap();
      return ByAsyncFuncAsyncHandler.For<IPackageRequest<NpmPackageIdentity>, INpmMetadataEntry>((Func<IPackageRequest<NpmPackageIdentity>, Task<INpmMetadataEntry>>) (async request => await upstreamFetchingMetadata.GetPackageVersionStateAsync(request)));
    }));
  }
}
