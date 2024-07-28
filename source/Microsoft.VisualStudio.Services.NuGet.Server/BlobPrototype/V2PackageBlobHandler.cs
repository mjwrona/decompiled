// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V2PackageBlobHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.OData.Query;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class V2PackageBlobHandler : 
    IAsyncHandler<
    #nullable disable
    V2PackageRequest, V2FeedPackage>,
    IHaveInputType<V2PackageRequest>,
    IHaveOutputType<V2FeedPackage>
  {
    private readonly IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> packageMetadataService;
    private readonly IConverter<IPackageRequest<VssNuGetPackageIdentity, INuGetMetadataEntry>, ServerV2FeedPackage> metadataEntryConverter;
    private readonly IConverter<V2FilterRequest, IEnumerable<ServerV2FeedPackage>> oDataFilter;
    private readonly IConverter<V2GetDownloadUrlBatchRequest, IEnumerable<V2FeedPackage>> downloadUrlPopulator;

    public V2PackageBlobHandler(
      IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> packageMetadataService,
      IConverter<IPackageRequest<VssNuGetPackageIdentity, INuGetMetadataEntry>, ServerV2FeedPackage> metadataEntryConverter,
      IConverter<V2FilterRequest, IEnumerable<ServerV2FeedPackage>> oDataFilter,
      IConverter<V2GetDownloadUrlBatchRequest, IEnumerable<V2FeedPackage>> downloadUrlPopulator)
    {
      this.packageMetadataService = packageMetadataService;
      this.metadataEntryConverter = metadataEntryConverter;
      this.oDataFilter = oDataFilter;
      this.downloadUrlPopulator = downloadUrlPopulator;
    }

    public async Task<V2FeedPackage> Handle(V2PackageRequest request)
    {
      INuGetMetadataEntry versionStateAsync = await this.packageMetadataService.GetPackageVersionStateAsync((IPackageRequest<VssNuGetPackageIdentity>) request);
      List<ServerV2FeedPackage> list = this.oDataFilter.Convert(new V2FilterRequest((IEnumerable<ServerV2FeedPackage>) new ServerV2FeedPackage[1]
      {
        this.metadataEntryConverter.Convert((IPackageRequest<VssNuGetPackageIdentity, INuGetMetadataEntry>) request.WithData<VssNuGetPackageIdentity, INuGetMetadataEntry>(versionStateAsync))
      }, (ODataQueryOptions) request.ODataQueryOptions)).ToList<ServerV2FeedPackage>();
      if (!list.Any<ServerV2FeedPackage>())
        throw ControllerExceptionHelper.PackageNotFound_LegacyNuGetSpecificType((IPackageIdentity) request.PackageId, request.Feed);
      V2FeedPackage v2FeedPackage = this.downloadUrlPopulator.Convert(new V2GetDownloadUrlBatchRequest((IFeedRequest) request, (IEnumerable<ServerV2FeedPackage>) list)).FirstOrDefault<V2FeedPackage>();
      v2FeedPackage.SetSecuredObject(FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(request.Feed));
      return v2FeedPackage;
    }
  }
}
