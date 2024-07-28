// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.CrossCollectionUpstreamNuGetClient
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Client.Internal;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  public class CrossCollectionUpstreamNuGetClient : 
    InternalUpstreamNuGetClientBase,
    IUpstreamNuGetClient,
    IUpstreamPackageNamesClient
  {
    private readonly string qualifiedFeedId;
    private readonly Guid viewId;
    private readonly Guid aadTenantId;
    private readonly string projectId;
    private readonly InternalNuGetHttpClient nugetHttpClient;
    private readonly InternalNuGetHttpClient elevatedNuGetHttpClient;
    private readonly FeedHttpClient feedHttpClient;
    private readonly IVersionCountsFromFileProvider versionCountsFromFileProvider;

    public CrossCollectionUpstreamNuGetClient(
      Guid? projectId,
      string qualifiedFeedId,
      Guid viewId,
      Guid aadTenantId,
      InternalNuGetHttpClient nugetHttpClient,
      InternalNuGetHttpClient elevatedNuGetHttpClient,
      FeedHttpClient feedHttpClient,
      IVersionCountsFromFileProvider versionCountsFromFileProvider)
    {
      this.projectId = projectId?.ToString();
      this.qualifiedFeedId = qualifiedFeedId;
      this.viewId = viewId;
      this.aadTenantId = aadTenantId;
      this.nugetHttpClient = nugetHttpClient;
      this.elevatedNuGetHttpClient = elevatedNuGetHttpClient;
      this.feedHttpClient = feedHttpClient;
      this.versionCountsFromFileProvider = versionCountsFromFileProvider;
    }

    public Task<Stream> GetNupkg(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageIdentity packageIdentity)
    {
      string file = packageIdentity.Name.NormalizedName + "." + packageIdentity.Version.NormalizedVersion + ".nupkg";
      return this.nugetHttpClient.GetNupkgInternalAsync(this.projectId, this.qualifiedFeedId, packageIdentity.Name.NormalizedName, packageIdentity.Version.NormalizedVersion, file, this.aadTenantId, (string) null, (object) null, new CancellationToken());
    }

    public async Task<IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>> GetNuspecs(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageId,
      IEnumerable<VssNuGetPackageVersion> packageVersions)
    {
      List<GetNuspecsInternalResponseNuspec> nuspecsInternalAsync = await this.nugetHttpClient.GetNuspecsInternalAsync((IEnumerable<string>) packageVersions.Select<VssNuGetPackageVersion, string>((Func<VssNuGetPackageVersion, string>) (x => x.NormalizedVersion)).ToList<string>(), this.projectId, this.qualifiedFeedId, packageId.NormalizedName, this.aadTenantId);
      Dictionary<VssNuGetPackageVersion, ContentBytes> nuspecs = new Dictionary<VssNuGetPackageVersion, ContentBytes>();
      foreach (GetNuspecsInternalResponseNuspec internalResponseNuspec in nuspecsInternalAsync)
      {
        if (!string.IsNullOrWhiteSpace(internalResponseNuspec.Content))
        {
          ContentBytes contentBytes = new ContentBytes(Convert.FromBase64String(internalResponseNuspec.Content), internalResponseNuspec.AreBytesCompressed);
          VssNuGetPackageVersion key = !string.IsNullOrWhiteSpace(internalResponseNuspec.DisplayVersion) ? new VssNuGetPackageVersion(internalResponseNuspec.DisplayVersion) : ExtractVersionFromNuspec(contentBytes);
          nuspecs.Add(key, contentBytes);
        }
      }
      return (IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>) nuspecs;

      static VssNuGetPackageVersion ExtractVersionFromNuspec(ContentBytes contentBytes) => new NuGetPackageMetadata(contentBytes.Content, contentBytes.AreBytesCompressed, MetadataReadOptions.IgnoreNonCriticalErrors).Identity.Version;
    }

    async Task<IReadOnlyList<RawPackageNameEntry>> IUpstreamPackageNamesClient.GetPackageNames() => (IReadOnlyList<RawPackageNameEntry>) (await this.nugetHttpClient.GetNamesInternalAsync(this.projectId, this.qualifiedFeedId, this.aadTenantId)).ToList<RawPackageNameEntry>();

    public async Task<GetVersionCountsResult> GetVersionCounts(
      NuGetSearchCategoryToggles queryCategories,
      string queryHint)
    {
      GetVersionCountsResult versionCounts;
      using (Stream docStream = await this.elevatedNuGetHttpClient.GetAllPackageVersionsInternalAsync(this.projectId, this.qualifiedFeedId, this.aadTenantId))
      {
        VersionListsFile versionListsFile = VersionListsFile.Parser.ParseFrom(docStream);
        VersionListsFileUnpacker.Unpack(versionListsFile);
        Guid viewIdForGetVersionCounts = this.viewId;
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedAsync = await this.feedHttpClient.GetFeedAsync(this.projectId, this.qualifiedFeedId);
        if (feedAsync.View != null && feedAsync.View.Name == "Local" && feedAsync.View.Type == FeedViewType.Implicit)
          viewIdForGetVersionCounts = Guid.Empty;
        versionCounts = new GetVersionCountsResult(this.versionCountsFromFileProvider.GetVersionCountsFromVersionListFile((ILazyVersionListsFile) new VersionListsFileWrapper(versionListsFile), queryCategories, viewIdForGetVersionCounts, false), (IVersionCountsImplementationMetrics) versionListsFile);
      }
      return versionCounts;
    }

    public async Task<IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>>> GetPackageNames() => (IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>>) (await this.nugetHttpClient.GetNamesInternalAsync(this.projectId, this.qualifiedFeedId, this.aadTenantId)).Select<RawPackageNameEntry, PackageNameEntry<VssNuGetPackageName>>((Func<RawPackageNameEntry, PackageNameEntry<VssNuGetPackageName>>) (x => new PackageNameEntry<VssNuGetPackageName>()
    {
      LastUpdatedDateTime = x.LastUpdatedDateTime,
      Name = new VssNuGetPackageName(x.Name)
    })).ToList<PackageNameEntry<VssNuGetPackageName>>();

    protected override async Task<IReadOnlyList<NuGetRawVersionWithSourceChainAndListed>> GetRawVersionInfoFromUpstreamAsync(
      VssNuGetPackageName packageId)
    {
      return (await this.nugetHttpClient.GetPackageVersionsExposedToDownstreamsAsync(this.projectId, this.qualifiedFeedId, packageId.NormalizedName, this.aadTenantId)).VersionInfo;
    }

    public async Task<NuGetUpstreamMetadata> GetUpstreamMetadata(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageIdentity packageIdentity)
    {
      Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package packageVersionAsync = await this.nugetHttpClient.GetPackageVersionAsync(this.projectId, this.qualifiedFeedId, packageIdentity.Name.NormalizedName, packageIdentity.Version.NormalizedVersion);
      return new NuGetUpstreamMetadata()
      {
        SourceChain = (IReadOnlyCollection<UpstreamSourceInfo>) packageVersionAsync.SourceChain.ToList<UpstreamSourceInfo>(),
        StorageId = (IStorageId) null,
        Listed = ((int) packageVersionAsync.Listed ?? 1) != 0
      };
    }
  }
}
