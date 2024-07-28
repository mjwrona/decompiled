// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetMetadataHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetMetadataHandlerBootstrapper : 
    IBootstrapper<
    #nullable disable
    IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry>>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetMetadataHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry> Bootstrap() => NuGetAggregationResolver.Bootstrap(this.requestContext).HandlerFor<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry, INuGetMetadataService, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>((Func<INuGetMetadataService, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>, IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry>>) ((metadataAccessor, upstreamVersionListService) =>
    {
      IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry> upstreamFetchingMetadata = new NuGetUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataAccessor, upstreamVersionListService, true).Bootstrap();
      return ByAsyncFuncAsyncHandler.For<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry>((Func<PackageRequest<VssNuGetPackageIdentity>, Task<INuGetMetadataEntry>>) (async request => await upstreamFetchingMetadata.GetPackageVersionStateAsync((IPackageRequest<VssNuGetPackageIdentity>) request)));
    }));
  }
}
