// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V2PackageBlobHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageDownload;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Utils;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class V2PackageBlobHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<RawV2PackageRequest, V2FeedPackage, INuGetMetadataService, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public V2PackageBlobHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<RawV2PackageRequest, V2FeedPackage> Bootstrap(
      INuGetMetadataService packageMetadataService,
      IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService)
    {
      this.requestContext.GetFeatureFlagFacade();
      ILocationFacade locationFacade = this.requestContext.GetLocationFacade();
      NuGetExtractInnerFileFromNupkgUriCalculator innerFileUriCalculator = new NuGetExtractInnerFileFromNupkgUriCalculator(locationFacade);
      V2PackageRequestConverter converter = new V2PackageRequestConverter(new NuGetRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap());
      FullMetadataEntryToV2FeedPackageConverter metadataEntryConverter = new FullMetadataEntryToV2FeedPackageConverter((INuGetLicenseUriCalculator) new NuGetLicenseUriCalculator((INuGetExtractInnerFileFromNupkgUriCalculator) innerFileUriCalculator), (INuGetIconUriCalculator) new NuGetIconUriCalculator((INuGetExtractInnerFileFromNupkgUriCalculator) innerFileUriCalculator));
      V2ODataFilterConverter oDataFilter = new V2ODataFilterConverter(FeatureEnabledConstants.RespectV2ODataQuerySkipParameter.Bootstrap(this.requestContext).Get());
      V2BatchDownloadUrlPopulator downloadUrlPopulator = new V2BatchDownloadUrlPopulator((IConverter<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>>) new NuGetDownloadV2UriCalculator(locationFacade));
      V2PackageBlobHandler handler = new V2PackageBlobHandler((IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>) new ThrowIfNotFoundOrDeletedDelegatingMetadataStore((IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>) new NuGetUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) packageMetadataService, upstreamVersionListService, true).Bootstrap()), (IConverter<IPackageRequest<VssNuGetPackageIdentity, INuGetMetadataEntry>, ServerV2FeedPackage>) metadataEntryConverter, (IConverter<V2FilterRequest, IEnumerable<ServerV2FeedPackage>>) oDataFilter, (IConverter<V2GetDownloadUrlBatchRequest, IEnumerable<V2FeedPackage>>) downloadUrlPopulator);
      return converter.ThenDelegateTo<RawV2PackageRequest, V2PackageRequest, V2FeedPackage>((IAsyncHandler<V2PackageRequest, V2FeedPackage>) handler);
    }
  }
}
