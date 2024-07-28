// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.NaivePackageMetadataSearchHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PublicClient;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3
{
  public class NaivePackageMetadataSearchHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<FeedRequest<NuGetSearchQuery>, NuGetSearchResultsInfo, INuGetPackageVersionCountsService, INuGetMetadataService, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly bool supportSearchingUpstreams;

    public NaivePackageMetadataSearchHandlerBootstrapper(
      IVssRequestContext requestContext,
      bool supportSearchingUpstreams)
    {
      this.requestContext = requestContext;
      this.supportSearchingUpstreams = supportSearchingUpstreams;
    }

    protected override IAsyncHandler<FeedRequest<NuGetSearchQuery>, NuGetSearchResultsInfo> Bootstrap(
      INuGetPackageVersionCountsService namesAccessor,
      INuGetMetadataService metadataAccessor,
      IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService)
    {
      return (IAsyncHandler<FeedRequest<NuGetSearchQuery>, NuGetSearchResultsInfo>) new NaivePackageMetadataSearchHandler(namesAccessor, this.supportSearchingUpstreams ? new NuGetUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataAccessor, upstreamVersionListService, false).Bootstrap() : (IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataAccessor, this.requestContext.GetTracerFacade(), (INuGetPackageMetadataSearchVersionFilteringStrategy) new NuGetPackageMetadataSearchVersionFilteringStrategy(), (INuGetLatestVersionsFinder) new NuGetLatestVersionsFinder(), (IConverter<IFeedRequest, Guid>) new LocalViewFoldingRequestToViewConverter(), this.requestContext.GetFeatureFlagFacade(), new UpstreamNuGetClientFactoryBootstrapper(this.requestContext).Bootstrap());
    }
  }
}
