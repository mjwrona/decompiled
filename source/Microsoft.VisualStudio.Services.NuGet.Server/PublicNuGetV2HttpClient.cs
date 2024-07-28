// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicNuGetV2HttpClient
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using BuildXL.Cache.ContentStore.UtilitiesCore.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.HttpStreams;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;
using Microsoft.VisualStudio.Services.NuGet.Server.UpstreamClient;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  public class PublicNuGetV2HttpClient : 
    PublicNuGetClientBase,
    IUpstreamNuGetClient,
    IUpstreamPackageNamesClient
  {
    private const int DefaultTopValueForPagination = 100;
    private const int DefaultSkipValueForPagination = 0;
    private readonly 
    #nullable disable
    Uri packageSourceUri;
    private readonly IVersionCountsFromFileProvider versionCountsFromFileProvider;
    private readonly IHttpClient httpNonForwardingClient;
    private readonly IFactory<VssNuGetPackageName, Task<ImmutableDictionary<VssNuGetPackageVersion, NugetV2PackageFeedResponse>>> packageVersionsFactory;

    public PublicNuGetV2HttpClient(
      Uri packageSourceUri,
      IHttpClient httpClient,
      IVersionCountsFromFileProvider versionCountsFromFileProvider,
      IHttpClient httpNonForwardingClient)
      : base(packageSourceUri, httpClient)
    {
      this.packageSourceUri = packageSourceUri;
      this.versionCountsFromFileProvider = versionCountsFromFileProvider;
      this.httpNonForwardingClient = httpNonForwardingClient;
      this.packageVersionsFactory = ByFuncInputFactory.For<VssNuGetPackageName, Task<ImmutableDictionary<VssNuGetPackageVersion, NugetV2PackageFeedResponse>>>((Func<VssNuGetPackageName, Task<ImmutableDictionary<VssNuGetPackageVersion, NugetV2PackageFeedResponse>>>) (async name => await this.GetPackageVersionsInternal(name))).SingleElementCache<VssNuGetPackageName, Task<ImmutableDictionary<VssNuGetPackageVersion, NugetV2PackageFeedResponse>>>();
    }

    public async Task<Stream> GetNupkg(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageIdentity packageIdentity)
    {
      PublicNuGetV2HttpClient nuGetV2HttpClient = this;
      Uri nupkgDownloadUri = await nuGetV2HttpClient.GetNupkgDownloadUri(packageIdentity);
      return await PublicUpstreamHttpClientHelper.GetStreamWithErrorHandlingAsync(nuGetV2HttpClient.HttpClient, nuGetV2HttpClient.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageIdentity) packageIdentity), nupkgDownloadUri, HttpCompletionOption.ResponseHeadersRead);
    }

    public async Task<IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>> GetNuspecs(
      IFeedRequest feedRequest,
      VssNuGetPackageName packageId,
      IEnumerable<VssNuGetPackageVersion> packageVersions)
    {
      HashSet<VssNuGetPackageVersion> versionsToDownload = new HashSet<VssNuGetPackageVersion>(packageVersions);
      Dictionary<VssNuGetPackageVersion, ContentBytes> nuspecs = new Dictionary<VssNuGetPackageVersion, ContentBytes>(versionsToDownload.Count);
      foreach (KeyValuePair<VssNuGetPackageVersion, NugetV2PackageFeedResponse> source in await this.packageVersionsFactory.Get(packageId))
      {
        VssNuGetPackageVersion key1;
        NugetV2PackageFeedResponse packageFeedResponse1;
        source.Deconstruct<VssNuGetPackageVersion, NugetV2PackageFeedResponse>(out key1, out packageFeedResponse1);
        VssNuGetPackageVersion key2 = key1;
        NugetV2PackageFeedResponse packageFeedResponse2 = packageFeedResponse1;
        if (versionsToDownload.Contains(key2))
          nuspecs.Add(key2, this.CreateNuspecContent(packageFeedResponse2.Properties));
      }
      IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes> nuspecs1 = (IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>) nuspecs;
      versionsToDownload = (HashSet<VssNuGetPackageVersion>) null;
      nuspecs = (Dictionary<VssNuGetPackageVersion, ContentBytes>) null;
      return nuspecs1;
    }

    private ContentBytes CreateNuspecContent(Properties properties)
    {
      using (MemoryStream outStream = new MemoryStream())
      {
        XmlDocument xmlDocument = new XmlDocument();
        XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", (string) null);
        xmlDocument.AppendChild((XmlNode) xmlDeclaration);
        XmlElement element1 = xmlDocument.CreateElement("package", "http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd");
        XmlElement element2 = xmlDocument.CreateElement("metadata");
        element2.AppendChild((XmlNode) this.CreateElement("id", properties.Id, xmlDocument));
        element2.AppendChild((XmlNode) this.CreateElement("version", properties.Version, xmlDocument));
        element2.AppendChild((XmlNode) this.CreateElement("description", properties.Description, xmlDocument));
        element2.AppendChild((XmlNode) this.CreateElement("authors", properties.Authors, xmlDocument));
        element2.AppendChild((XmlNode) this.CreateElement("copyright", properties.Copyright, xmlDocument));
        element2.AppendChild((XmlNode) this.CreateElement("dependencies", properties.Dependencies, xmlDocument));
        element2.AppendChild((XmlNode) this.CreateElement("created", properties.Created.ToString(), xmlDocument));
        element2.AppendChild((XmlNode) this.CreateElement("iconUrl", properties.IconUrl, xmlDocument));
        element2.AppendChild((XmlNode) this.CreateElement("language", properties.Language, xmlDocument));
        element2.AppendChild((XmlNode) this.CreateElement("projectUrl", properties.ProjectUrl, xmlDocument));
        element2.AppendChild((XmlNode) this.CreateElement("releaseNotes", properties.ReleaseNotes, xmlDocument));
        element2.AppendChild((XmlNode) this.CreateElement("requireLicenseAcceptance", properties.RequireLicenseAcceptance.ToString(), xmlDocument));
        element2.AppendChild((XmlNode) this.CreateElement("tags", properties.Tags, xmlDocument));
        element2.AppendChild((XmlNode) this.CreateElement("title", properties.Title, xmlDocument));
        element2.AppendChild((XmlNode) this.CreateElement("licenseUrl", properties.LicenseUrl, xmlDocument));
        element1.AppendChild((XmlNode) element2);
        xmlDocument.AppendChild((XmlNode) element1);
        xmlDocument.Save((Stream) outStream);
        return new ContentBytes(outStream.ToArray());
      }
    }

    private XmlElement CreateElement(string name, string value, XmlDocument xmlDocument)
    {
      XmlElement element = xmlDocument.CreateElement(name);
      element.AppendChild((XmlNode) xmlDocument.CreateTextNode(value));
      return element;
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<VssNuGetPackageVersion>>> GetPackageVersions(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageName)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IReadOnlyList<VersionWithSourceChain<VssNuGetPackageVersion>>) (await this.packageVersionsFactory.Get(packageName)).Keys.Select<VssNuGetPackageVersion, VersionWithSourceChain<VssNuGetPackageVersion>>(PublicNuGetV2HttpClient.\u003C\u003EO.\u003C0\u003E__FromExternalSource ?? (PublicNuGetV2HttpClient.\u003C\u003EO.\u003C0\u003E__FromExternalSource = new Func<VssNuGetPackageVersion, VersionWithSourceChain<VssNuGetPackageVersion>>(VersionWithSourceChain.FromExternalSource<VssNuGetPackageVersion>))).ToList<VersionWithSourceChain<VssNuGetPackageVersion>>();
    }

    private async Task<ImmutableDictionary<VssNuGetPackageVersion, NugetV2PackageFeedResponse>> GetPackageVersionsInternal(
      VssNuGetPackageName packageId)
    {
      PublicNuGetV2HttpClient nuGetV2HttpClient = this;
      List<NugetV2PackageFeedResponse> resultList = new List<NugetV2PackageFeedResponse>();
      int skip = 0;
      NugetV2PackageEntryResponse errorHandlingAsync;
      do
      {
        Uri uri = PublicNuGetClientBase.ConstructEscapedRelativeUri(nuGetV2HttpClient.packageSourceUri, "FindPackagesById()?Id='{0}'&$skip={1}&$top={2}", packageId.NormalizedName, skip.ToString(), 100.ToString());
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        errorHandlingAsync = await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<NugetV2PackageEntryResponse>(nuGetV2HttpClient.HttpClient, nuGetV2HttpClient.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageName) packageId), uri, HttpCompletionOption.ResponseContentRead, PublicNuGetV2HttpClient.\u003C\u003EO.\u003C1\u003E__DeserializeXml ?? (PublicNuGetV2HttpClient.\u003C\u003EO.\u003C1\u003E__DeserializeXml = new Func<HttpResponseMessage, Task<NugetV2PackageEntryResponse>>(PublicNuGetV2HttpClient.DeserializeXml<NugetV2PackageEntryResponse>)));
        resultList.AddRange((IEnumerable<NugetV2PackageFeedResponse>) errorHandlingAsync.Entries);
        skip += errorHandlingAsync.Entries.Count;
      }
      while (errorHandlingAsync.Entries.Count == 100);
      ImmutableDictionary<VssNuGetPackageVersion, NugetV2PackageFeedResponse> immutableDictionary = resultList.Where<NugetV2PackageFeedResponse>((Func<NugetV2PackageFeedResponse, bool>) (entry => entry.Properties != null)).Select(x => new
      {
        Version = VssNuGetPackageVersion.ParseOrDefault(x.Properties.Version),
        Info = x
      }).Where(x => x.Version != null).ToImmutableDictionary(x => x.Version, x => x.Info);
      resultList = (List<NugetV2PackageFeedResponse>) null;
      return immutableDictionary;
    }

    public async Task<GetVersionCountsResult> GetVersionCounts(
      NuGetSearchCategoryToggles queryCategories,
      string queryHint)
    {
      PublicNuGetV2HttpClient nuGetV2HttpClient = this;
      Uri uri = PublicNuGetClientBase.ConstructEscapedRelativeUri(nuGetV2HttpClient.packageSourceUri, "Search()?searchTerm='{0}'&includePrerelease={1}&semVerLevel={2}", queryHint, queryCategories.IncludePrereleaseVersions ? "true" : "false", queryCategories.VersionsWithBuildMetadataAppearToExist ? "2.0.0" : "1.0.0");
      return await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<GetVersionCountsResult>(nuGetV2HttpClient.HttpClient, nuGetV2HttpClient.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.None, uri, HttpCompletionOption.ResponseContentRead, (Func<HttpResponseMessage, Task<GetVersionCountsResult>>) (async response =>
      {
        NugetV2PackageEntryResponse queryResults = await PublicNuGetV2HttpClient.DeserializeXml<NugetV2PackageEntryResponse>(response);
        return new GetVersionCountsResult((IEnumerable<ILazyNuGetPackageVersionCounts>) this.MapQueryResultsToNugetPackageVersionCounts(queryResults, queryCategories), (IVersionCountsImplementationMetrics) new PublicNuGetClientBase.DummyVersionCountsImplementationMetrics(queryResults.Entries.GroupBy<NugetV2PackageFeedResponse, string>((Func<NugetV2PackageFeedResponse, string>) (entry => entry.Properties.Id)).Distinct<IGrouping<string, NugetV2PackageFeedResponse>>().Count<IGrouping<string, NugetV2PackageFeedResponse>>(), queryResults.Entries.Count<NugetV2PackageFeedResponse>()));
      }));
    }

    public override async Task<NuGetPackageRegistrationState> GetRegistrationState(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageName,
      IEnumerable<VssNuGetPackageVersion> versions)
    {
      ImmutableDictionary<VssNuGetPackageVersion, NugetV2PackageFeedResponse> immutableDictionary = await this.packageVersionsFactory.Get(packageName);
      ImmutableDictionary<VssNuGetPackageVersion, NuGetRegistrationState>.Builder builder = ImmutableDictionary.CreateBuilder<VssNuGetPackageVersion, NuGetRegistrationState>();
      foreach (VssNuGetPackageVersion version in versions)
      {
        NugetV2PackageFeedResponse packageFeedResponse;
        if (immutableDictionary.TryGetValue(version, out packageFeedResponse))
          builder.Add(version, new NuGetRegistrationState(new VssNuGetPackageIdentity(packageFeedResponse.Properties.Id, packageFeedResponse.Properties.Version), (NuGetDeprecation) null, packageFeedResponse.Properties.IsListed, new DateTime?(packageFeedResponse.Properties.Published), (IImmutableList<NuGetVulnerability>) ImmutableArray<NuGetVulnerability>.Empty, (NuGetCatalogCursor) null));
      }
      return new NuGetPackageRegistrationState((IImmutableDictionary<VssNuGetPackageVersion, NuGetRegistrationState>) builder.ToImmutable(), (NuGetCatalogCursor) null);
    }

    Task<IReadOnlyList<RawPackageNameEntry>> IUpstreamPackageNamesClient.GetPackageNames() => throw new NotImplementedException("should not call this api for public upstreams");

    Task<IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>>> IUpstreamNuGetClient.GetPackageNames() => throw new NotImplementedException("should not call this api for public upstreams");

    private ImmutableList<NuGetPackageVersionCounts> MapQueryResultsToNugetPackageVersionCounts(
      NugetV2PackageEntryResponse queryResults,
      NuGetSearchCategoryToggles queryCategories)
    {
      return queryResults.Entries.Where<NugetV2PackageFeedResponse>((Func<NugetV2PackageFeedResponse, bool>) (entry => entry.Properties.Id != null)).GroupBy<NugetV2PackageFeedResponse, string>((Func<NugetV2PackageFeedResponse, string>) (entry => entry.Properties.Id.ToLowerInvariant())).Select(groupEntry => new
      {
        Package = new VssNuGetPackageName(groupEntry.Key),
        Versions = groupEntry.Select<NugetV2PackageFeedResponse, VssNuGetPackageVersion>((Func<NugetV2PackageFeedResponse, VssNuGetPackageVersion>) (entry => VssNuGetPackageVersion.ParseOrDefault(entry.Properties.Version))).Where<VssNuGetPackageVersion>((Func<VssNuGetPackageVersion, bool>) (version => version != null)).ToList<VssNuGetPackageVersion>()
      }).Select(element => new
      {
        Package = element.Package,
        Versions = element.Versions.Select<VssNuGetPackageVersion, NuGetSearchResultVersionSummary>((Func<VssNuGetPackageVersion, NuGetSearchResultVersionSummary>) (version => new NuGetSearchResultVersionSummary(new VssNuGetPackageIdentity(element.Package, version), true, (IEnumerable<Guid>) null, false, false))).ToList<NuGetSearchResultVersionSummary>()
      }).Where(element => element.Versions.Any<NuGetSearchResultVersionSummary>()).Select(element => this.versionCountsFromFileProvider.CalculateCounts(queryCategories, Guid.Empty, (IEnumerable<NuGetSearchResultVersionSummary>) element.Versions, element.Package, DateTime.MaxValue)).ToImmutableList<NuGetPackageVersionCounts>();
    }

    private async Task<byte[]> GetNuspecInternal(VssNuGetPackageIdentity packageIdentity)
    {
      byte[] nuspecBytes;
      using (HttpSeekableStream httpSeekableStream = new HttpSeekableStream(await this.GetNupkgDownloadUri(packageIdentity)))
      {
        ZipUtils.ValidateZipEndOfCentralDirectoryIsInRange((Stream) httpSeekableStream);
        nuspecBytes = NuGetNuspecUtils.GetNuspecBytes((Stream) httpSeekableStream);
      }
      return nuspecBytes;
    }

    private async Task<Uri> GetNupkgDownloadUri(VssNuGetPackageIdentity packageIdentity)
    {
      PublicNuGetV2HttpClient nuGetV2HttpClient = this;
      Uri uri1 = PublicNuGetClientBase.ConstructEscapedRelativeUri(nuGetV2HttpClient.packageSourceUri, "Packages(Id='{0}',Version='{1}')", packageIdentity.Name.NormalizedName, packageIdentity.Version.NormalizedVersion);
      return await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<Uri>(nuGetV2HttpClient.HttpClient, nuGetV2HttpClient.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageIdentity) packageIdentity), uri1, HttpCompletionOption.ResponseHeadersRead, (Func<HttpResponseMessage, Task<Uri>>) (async response =>
      {
        NugetV2PackageFeedResponse packageV2EntryResponse = await PublicNuGetV2HttpClient.DeserializeXml<NugetV2PackageFeedResponse>(response);
        HttpResponseMessage async = await this.httpNonForwardingClient.GetAsync(new Uri(packageV2EntryResponse.Content.Src), HttpCompletionOption.ResponseHeadersRead);
        Uri uri2;
        if (async.StatusCode == HttpStatusCode.Found)
          uri2 = async?.Headers?.Location;
        else if (async.StatusCode == HttpStatusCode.OK)
          uri2 = new Uri(packageV2EntryResponse.Content.Src);
        Uri nupkgDownloadUri = !(uri2 == (Uri) null) ? uri2 : throw new PublicUpstreamFailureException("Failed to find nupkg download url for pakage: " + packageIdentity.DisplayStringForMessages, new Uri(packageV2EntryResponse.Content.Src));
        packageV2EntryResponse = (NugetV2PackageFeedResponse) null;
        return nupkgDownloadUri;
      }));
    }

    private static async Task<T> DeserializeXml<T>(HttpResponseMessage response)
    {
      XmlSerializer serializer = TeamFoundationSerializationUtility.CreateSerializer(typeof (T), (XmlRootAttribute) null);
      T obj;
      using (Stream input = await response.Content.ReadAsStreamAsync())
      {
        using (XmlReader xmlReader = XmlReader.Create(input))
          obj = (T) serializer.Deserialize(xmlReader);
      }
      serializer = (XmlSerializer) null;
      return obj;
    }
  }
}
