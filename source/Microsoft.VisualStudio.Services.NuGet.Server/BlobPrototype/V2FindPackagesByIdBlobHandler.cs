// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V2FindPackagesByIdBlobHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.OData.Query;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class V2FindPackagesByIdBlobHandler : 
    IAsyncHandler<
    #nullable disable
    V2PackageNameRequest, IEnumerable<V2FeedPackage>>,
    IHaveInputType<V2PackageNameRequest>,
    IHaveOutputType<IEnumerable<V2FeedPackage>>
  {
    private readonly IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> packageMetadataService;
    private readonly IConverter<IPackageRequest<VssNuGetPackageIdentity, INuGetMetadataEntry>, ServerV2FeedPackage> metadataEntryConverter;
    private readonly IConverter<V2FilterRequest, IEnumerable<ServerV2FeedPackage>> oDataFilter;
    private readonly IConverter<V2GetDownloadUrlBatchRequest, IEnumerable<V2FeedPackage>> downloadUrlPopulator;

    public V2FindPackagesByIdBlobHandler(
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

    public async Task<IEnumerable<V2FeedPackage>> Handle(V2PackageNameRequest request)
    {
      QueryOptions<INuGetMetadataEntry> queryOptions = new QueryOptions<INuGetMetadataEntry>().WithFilter((Func<INuGetMetadataEntry, bool>) (x => !x.IsDeleted()));
      if (request.ODataQueryOptions != null && V2FindPackagesByIdBlobHandler.ShouldFilterOutVersionsWithBuildMetadata((ODataQueryOptions) request.ODataQueryOptions))
        queryOptions = queryOptions.WithFilter((Func<INuGetMetadataEntry, bool>) (x => !x.PackageIdentity.Version.NuGetVersion.HasMetadata));
      IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> packageMetadataService = this.packageMetadataService;
      NuGetPackageNameQuery packageNameQueryRequest = new NuGetPackageNameQuery((IPackageNameRequest<IPackageName>) request);
      packageNameQueryRequest.Options = queryOptions;
      IList<ServerV2FeedPackage> list1 = (IList<ServerV2FeedPackage>) this.metadataEntryConverter.ConvertMultiple<IPackageRequest<VssNuGetPackageIdentity, INuGetMetadataEntry>, ServerV2FeedPackage>((IEnumerable<IPackageRequest<VssNuGetPackageIdentity, INuGetMetadataEntry>>) (await packageMetadataService.GetPackageVersionStatesAsync((PackageNameQuery<INuGetMetadataEntry>) packageNameQueryRequest)).Select<INuGetMetadataEntry, PackageRequest<VssNuGetPackageIdentity, INuGetMetadataEntry>>((Func<INuGetMetadataEntry, PackageRequest<VssNuGetPackageIdentity, INuGetMetadataEntry>>) (x => request.WithPackage<VssNuGetPackageIdentity>(x.PackageIdentity).WithData<VssNuGetPackageIdentity, INuGetMetadataEntry>(x)))).OrderBy<ServerV2FeedPackage, VssNuGetPackageVersion>((Func<ServerV2FeedPackage, VssNuGetPackageVersion>) (p => p.PackageIdentity.Version)).ToList<ServerV2FeedPackage>();
      NuGetV2LatestPackageVersionUtils.PopulateLatestVersion(list1);
      IList<ServerV2FeedPackage> list2 = (IList<ServerV2FeedPackage>) this.oDataFilter.Convert(new V2FilterRequest((IEnumerable<ServerV2FeedPackage>) list1, (ODataQueryOptions) request.ODataQueryOptions)).OrderBy<ServerV2FeedPackage, VssNuGetPackageVersion>((Func<ServerV2FeedPackage, VssNuGetPackageVersion>) (package => package.PackageIdentity.Version)).ToList<ServerV2FeedPackage>();
      return this.downloadUrlPopulator.Convert(new V2GetDownloadUrlBatchRequest((IFeedRequest) request, (IEnumerable<ServerV2FeedPackage>) list2)).ToSecuredObject<V2FeedPackage>(FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(request.Feed));
    }

    private static bool ShouldFilterOutVersionsWithBuildMetadata(ODataQueryOptions options)
    {
      Dictionary<string, string> dictionary = options.Request.GetQueryNameValuePairs().ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (o => o.Key), (Func<KeyValuePair<string, string>, string>) (o => o.Value));
      return !dictionary.ContainsKey("semVerLevel") || dictionary["semVerLevel"].StartsWith("1");
    }
  }
}
