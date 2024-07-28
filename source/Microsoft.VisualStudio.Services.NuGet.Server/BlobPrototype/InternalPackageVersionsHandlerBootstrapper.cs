// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.InternalPackageVersionsHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Client.Internal;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class InternalPackageVersionsHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<RawPackageNameRequest, NuGetVersionsExposedToDownstreamsResponse, INuGetMetadataService, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public InternalPackageVersionsHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<RawPackageNameRequest, NuGetVersionsExposedToDownstreamsResponse> Bootstrap(
      INuGetMetadataService metadataService,
      IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService)
    {
      IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService1 = new NuGetUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataService, upstreamVersionListService, true).Bootstrap();
      return new PackageNameRequestConverter(new NuGetPackageNameParsingConverterBootstrapper(this.requestContext).Bootstrap()).ThenDelegateTo<RawPackageNameRequest, PackageNameRequest<VssNuGetPackageName>, NuGetVersionsExposedToDownstreamsResponse>((IAsyncHandler<PackageNameRequest<VssNuGetPackageName>, NuGetVersionsExposedToDownstreamsResponse>) new PackageVersionsFromPackageNameHandler((IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataService1, this.requestContext.GetTracerFacade()));
    }
  }
}
