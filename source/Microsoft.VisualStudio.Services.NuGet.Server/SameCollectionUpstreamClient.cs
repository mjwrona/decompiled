// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.SameCollectionUpstreamClient
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Client.Internal;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.GetPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  public class SameCollectionUpstreamClient : 
    InternalUpstreamNuGetClientBase,
    IUpstreamNuGetClient,
    IUpstreamPackageNamesClient
  {
    private readonly 
    #nullable disable
    IVssRequestContext collectionContext;
    private readonly IFeedService feedService;
    private readonly UpstreamSource upstreamSource;

    public SameCollectionUpstreamClient(
      IVssRequestContext collectionContext,
      IFeedService feedService,
      UpstreamSource upstreamSource)
    {
      this.collectionContext = collectionContext;
      this.feedService = feedService;
      this.upstreamSource = upstreamSource;
    }

    public async Task<Stream> GetNupkg(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageIdentity packageIdentity)
    {
      IFeedRequest upstreamFeedRequest = this.GetUpstreamFeedRequest();
      HttpResponseMessage httpResponseMessage = await new GetFileAsyncBootstrapper(this.collectionContext).Bootstrap().TaskYieldOnException<IRawPackageInnerFileRequest<NuGetGetFileData>, HttpResponseMessage>().Handle((IRawPackageInnerFileRequest<NuGetGetFileData>) new RawPackageInnerFileRequest<NuGetGetFileData>(upstreamFeedRequest, packageIdentity.Name.NormalizedName, packageIdentity.Version.NormalizedVersion, packageIdentity.Name.NormalizedName + "." + packageIdentity.Version.NormalizedVersion + ".nupkg", (string) null, new NuGetGetFileData((string) null)));
      Uri location = httpResponseMessage.Headers.Location;
      if (location != (Uri) null)
        return await new HttpClient().GetStreamAsync(location);
      httpResponseMessage.EnsureSuccessStatusCode();
      return await httpResponseMessage.Content.ReadAsStreamAsync();
    }

    public async Task<IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>> GetNuspecs(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageId,
      IEnumerable<VssNuGetPackageVersion> packageVersions)
    {
      IFeedRequest upstreamFeedRequest = this.GetUpstreamFeedRequest();
      return await new GetNuspecsInternalBootstrapper(this.collectionContext).Bootstrap().TaskYieldOnException<NuGetGetNuspecsRequest, IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>>().Handle(new NuGetGetNuspecsRequest(upstreamFeedRequest.WithPackageName<VssNuGetPackageName>(packageId), packageVersions));
    }

    async Task<IReadOnlyList<RawPackageNameEntry>> IUpstreamPackageNamesClient.GetPackageNames() => (IReadOnlyList<RawPackageNameEntry>) (await this.GetPackageNames()).Select<IPackageNameEntry<VssNuGetPackageName>, RawPackageNameEntry>((Func<IPackageNameEntry<VssNuGetPackageName>, RawPackageNameEntry>) (entry => new RawPackageNameEntry()
    {
      Name = entry.Name.NormalizedName,
      LastUpdatedDateTime = entry.LastUpdatedDateTime
    })).ToList<RawPackageNameEntry>();

    public async Task<IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>>> GetPackageNames()
    {
      IFactory<IFeedRequest, Task<INuGetNamesService>> factory = NuGetAggregationResolver.Bootstrap(this.collectionContext).FactoryFor<INuGetNamesService>();
      IFeedRequest feed = this.GetUpstreamFeedRequest();
      IFeedRequest input = feed;
      IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>> list = (IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>>) (await (await factory.Get(input)).GetPackageNamesAsync(feed)).ToList<IPackageNameEntry<VssNuGetPackageName>>();
      feed = (IFeedRequest) null;
      return list;
    }

    public async Task<GetVersionCountsResult> GetVersionCounts(
      NuGetSearchCategoryToggles queryCategories,
      string queryHint)
    {
      IFactory<IFeedRequest, Task<INuGetPackageVersionCountsService>> factory = NuGetAggregationResolver.Bootstrap(this.collectionContext).FactoryFor<INuGetPackageVersionCountsService>();
      IFeedRequest feed = this.GetUpstreamFeedRequest();
      IFeedRequest input = feed;
      GetVersionCountsResult versionCounts = await (await factory.Get(input)).GetVersionCounts(feed, queryCategories);
      feed = (IFeedRequest) null;
      return versionCounts;
    }

    protected override async Task<IReadOnlyList<NuGetRawVersionWithSourceChainAndListed>> GetRawVersionInfoFromUpstreamAsync(
      VssNuGetPackageName packageId)
    {
      IFeedRequest feed = this.GetUpstreamFeedRequest();
      IReadOnlyList<NuGetRawVersionWithSourceChainAndListed> versionInfo = (await (await NuGetAggregationResolver.Bootstrap(this.collectionContext).FactoryFor<IAsyncHandler<RawPackageNameRequest, NuGetVersionsExposedToDownstreamsResponse>>((IRequireAggBootstrapper<IAsyncHandler<RawPackageNameRequest, NuGetVersionsExposedToDownstreamsResponse>>) new InternalPackageVersionsHandlerBootstrapper(this.collectionContext)).Get(feed)).Handle(new RawPackageNameRequest(feed, packageId.NormalizedName))).VersionInfo;
      feed = (IFeedRequest) null;
      return versionInfo;
    }

    public async Task<NuGetUpstreamMetadata> GetUpstreamMetadata(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageIdentity packageIdentity)
    {
      IFeedRequest upstreamFeedRequest = this.GetUpstreamFeedRequest();
      INuGetMetadataEntry getMetadataEntry = await new GetPackageVersionHandler<VssNuGetPackageIdentity, INuGetMetadataEntry, INuGetMetadataEntry>((IConverter<INuGetMetadataEntry, INuGetMetadataEntry>) new NoOpConverter<INuGetMetadataEntry>(), new NuGetMetadataHandlerBootstrapper(this.collectionContext).Bootstrap()).TaskYieldOnException<PackageRequest<VssNuGetPackageIdentity, ShowDeletedBool>, INuGetMetadataEntry>().Handle(upstreamFeedRequest.WithPackage<VssNuGetPackageIdentity>(packageIdentity).WithData<VssNuGetPackageIdentity, ShowDeletedBool>(new ShowDeletedBool(false)));
      return new NuGetUpstreamMetadata()
      {
        SourceChain = (IReadOnlyCollection<UpstreamSourceInfo>) getMetadataEntry.SourceChain.ToList<UpstreamSourceInfo>(),
        StorageId = getMetadataEntry.PackageStorageId,
        Listed = getMetadataEntry.Listed
      };
    }

    private IFeedRequest GetUpstreamFeedRequest() => (IFeedRequest) new FeedRequest(this.feedService.GetFeed(this.upstreamSource.GetProjectId() ?? Guid.Empty, this.upstreamSource.GetFullyQualifiedFeedId()), (IProtocol) Protocol.NuGet);
  }
}
