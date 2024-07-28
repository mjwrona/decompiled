// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicNuGetHttpClient
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;
using Microsoft.VisualStudio.Services.NuGet.Server.UpstreamClient;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  public class PublicNuGetHttpClient : 
    PublicNuGetClientBase,
    IUpstreamNuGetClient,
    IUpstreamPackageNamesClient,
    INuGetV3Client
  {
    private readonly Uri packageSourceUri;
    private readonly Lazy<Task<ServiceIndex>> lazyServiceIndex;
    private readonly IPublicServiceIndexService publicServiceIndex;
    private readonly IVersionCountsFromFileProvider versionCountsFromFileProvider;
    private readonly ITracerService tracerService;
    private readonly ICancellationFacade cancellation;

    public PublicNuGetHttpClient(
      Uri packageSourceUri,
      IHttpClient httpClient,
      IPublicServiceIndexService publicServiceIndex,
      IVersionCountsFromFileProvider versionCountsFromFileProvider,
      ITracerService tracerService,
      ICancellationFacade cancellation)
      : base(packageSourceUri, httpClient)
    {
      this.packageSourceUri = packageSourceUri;
      this.lazyServiceIndex = new Lazy<Task<ServiceIndex>>(new Func<Task<ServiceIndex>>(this.FetchServiceIndex), LazyThreadSafetyMode.PublicationOnly);
      this.publicServiceIndex = publicServiceIndex;
      this.versionCountsFromFileProvider = versionCountsFromFileProvider;
      this.tracerService = tracerService;
      this.cancellation = cancellation;
    }

    public async Task<IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>> GetNuspecs(
      IFeedRequest feedRequest,
      VssNuGetPackageName packageId,
      IEnumerable<VssNuGetPackageVersion> packageVersions)
    {
      Uri packageBaseAddressUri = await this.GetPackageBaseAddressUri();
      ConcurrencyLimitingInvoker invoker = new ConcurrencyLimitingInvoker(50);
      return (IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>) ((IEnumerable<KeyValuePair<VssNuGetPackageVersion, ContentBytes>>) await Task.WhenAll<KeyValuePair<VssNuGetPackageVersion, ContentBytes>>(packageVersions.Select<VssNuGetPackageVersion, Task<KeyValuePair<VssNuGetPackageVersion, ContentBytes>>>((Func<VssNuGetPackageVersion, Task<KeyValuePair<VssNuGetPackageVersion, ContentBytes>>>) (version => invoker.Invoke<KeyValuePair<VssNuGetPackageVersion, ContentBytes>>((Func<Task<KeyValuePair<VssNuGetPackageVersion, ContentBytes>>>) (async () =>
      {
        VssNuGetPackageVersion getPackageVersion = version;
        return Kvp.Create<VssNuGetPackageVersion, ContentBytes>(getPackageVersion, new ContentBytes(await this.GetNuspecInternal(packageBaseAddressUri, packageId, version)));
      })))))).ToDictionaryAnyWins<VssNuGetPackageVersion, ContentBytes>();
    }

    Task<IReadOnlyList<RawPackageNameEntry>> IUpstreamPackageNamesClient.GetPackageNames() => throw new NotImplementedException("should not call this api for public upstreams");

    Task<IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>>> IUpstreamNuGetClient.GetPackageNames() => throw new NotImplementedException("should not call this api for public upstreams");

    public override async Task<NuGetPackageRegistrationState> GetRegistrationState(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageName,
      IEnumerable<VssNuGetPackageVersion> versions)
    {
      PublicNuGetHttpClient sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetRegistrationState)))
      {
        SortedSet<VssNuGetPackageVersion> versionsSet = new SortedSet<VssNuGetPackageVersion>(versions);
        if (!versionsSet.Any<VssNuGetPackageVersion>())
          return new NuGetPackageRegistrationState((IImmutableDictionary<VssNuGetPackageVersion, NuGetRegistrationState>) ImmutableDictionary<VssNuGetPackageVersion, NuGetRegistrationState>.Empty, (NuGetCatalogCursor) null);
        Uri packageUri = PublicNuGetClientBase.ConstructEscapedRelativeUri(await sendInTheThisObject.GetRegistrationsBaseUrl(), "{0}/index.json", packageName.NormalizedName);
        PublicNuGetHttpClient.RegistrationIndex registrationIndex = await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<PublicNuGetHttpClient.RegistrationIndex>(sendInTheThisObject.HttpClient, sendInTheThisObject.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageName) packageName), packageUri, HttpCompletionOption.ResponseContentRead, new Func<HttpResponseMessage, Task<PublicNuGetHttpClient.RegistrationIndex>>(sendInTheThisObject.DeserializeJson<PublicNuGetHttpClient.RegistrationIndex>)).EnforceCancellation<PublicNuGetHttpClient.RegistrationIndex>(sendInTheThisObject.cancellation.Token, file: "D:\\a\\_work\\1\\s\\Packaging\\Service\\NuGet\\Server\\UpstreamClient\\PublicNuGetHttpClient.cs", member: nameof (GetRegistrationState), line: 149);
        ImmutableDictionary<VssNuGetPackageVersion, NuGetRegistrationState>.Builder result = ImmutableDictionary.CreateBuilder<VssNuGetPackageVersion, NuGetRegistrationState>();
        foreach (PublicNuGetHttpClient.RegistrationPage registrationPage in (IEnumerable<PublicNuGetHttpClient.RegistrationPage>) registrationIndex.Items)
        {
          VssNuGetPackageVersion orDefault1 = VssNuGetPackageVersion.ParseOrDefault(registrationPage.Lower);
          if (orDefault1 == null)
          {
            tracer.TraceError(string.Format("Detected unparseable lower-bound version '{0}' for a registration page in {1}", (object) registrationPage.Lower, (object) packageUri));
          }
          else
          {
            VssNuGetPackageVersion orDefault2 = VssNuGetPackageVersion.ParseOrDefault(registrationPage.Upper);
            if (orDefault2 == null)
            {
              tracer.TraceError(string.Format("Detected unparseable upper-bound version '{0}' for a registration page in {1}", (object) registrationPage.Upper, (object) packageUri));
            }
            else
            {
              SortedSet<VssNuGetPackageVersion> pageVersionsSubset = versionsSet.GetViewBetween(orDefault1, orDefault2);
              if (pageVersionsSubset.Any<VssNuGetPackageVersion>())
              {
                IReadOnlyList<PublicNuGetHttpClient.RegistrationItem> items = registrationPage.Items;
                if (items == null)
                {
                  items = (await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<PublicNuGetHttpClient.RegistrationPage>(sendInTheThisObject.HttpClient, sendInTheThisObject.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.None, registrationPage.Id, HttpCompletionOption.ResponseContentRead, new Func<HttpResponseMessage, Task<PublicNuGetHttpClient.RegistrationPage>>(sendInTheThisObject.DeserializeJson<PublicNuGetHttpClient.RegistrationPage>)).EnforceCancellation<PublicNuGetHttpClient.RegistrationPage>(sendInTheThisObject.cancellation.Token, file: "D:\\a\\_work\\1\\s\\Packaging\\Service\\NuGet\\Server\\UpstreamClient\\PublicNuGetHttpClient.cs", member: nameof (GetRegistrationState), line: 187)).Items;
                  if (items == null)
                    throw new PublicUpstreamFailureException(Resources.Error_UpstreamFailure((object) sendInTheThisObject.PackageSourceUri.AbsoluteUri, (object) Resources.Error_UpstreamRegistrationInvalid()), (Exception) null, sendInTheThisObject.PackageSourceUri);
                }
                foreach (PublicNuGetHttpClient.RegistrationItem registrationItem in (IEnumerable<PublicNuGetHttpClient.RegistrationItem>) items)
                {
                  PublicNuGetHttpClient.RegistrationCatalogEntry catalogEntry = registrationItem.CatalogEntry;
                  VssNuGetPackageVersion orDefault3 = VssNuGetPackageVersion.ParseOrDefault(catalogEntry.Version);
                  if (orDefault3 != null && pageVersionsSubset.Contains(orDefault3))
                  {
                    VssNuGetPackageName name = new VssNuGetPackageName(catalogEntry.Id);
                    result.Add(orDefault3, new NuGetRegistrationState(new VssNuGetPackageIdentity(name, orDefault3), (object) catalogEntry.Deprecation != null ? ConvertDeprecation(catalogEntry.Deprecation) : (NuGetDeprecation) null, ((int) catalogEntry.Listed ?? 1) != 0, catalogEntry.Published, (IImmutableList<NuGetVulnerability>) (catalogEntry.Vulnerabilities != null ? ConvertVulnerabilities((IEnumerable<PublicNuGetHttpClient.RegistrationCatalogEntryVulnerability>) catalogEntry.Vulnerabilities) : ImmutableArray<NuGetVulnerability>.Empty), registrationItem.CatalogCursorPosition));
                  }
                }
                pageVersionsSubset = (SortedSet<VssNuGetPackageVersion>) null;
              }
            }
          }
        }
        return new NuGetPackageRegistrationState((IImmutableDictionary<VssNuGetPackageVersion, NuGetRegistrationState>) result.ToImmutable(), registrationIndex.CatalogCursorPosition);
      }

      static NuGetDeprecation ConvertDeprecation(
        PublicNuGetHttpClient.RegistrationCatalogEntryDeprecation deprecation)
      {
        return new NuGetDeprecation((IImmutableList<string>) deprecation.Reasons.ToImmutableArray<string>(), deprecation.Message, deprecation.AlternatePackage?.Id, deprecation.AlternatePackage?.Range);
      }

      static ImmutableArray<NuGetVulnerability> ConvertVulnerabilities(
        IEnumerable<PublicNuGetHttpClient.RegistrationCatalogEntryVulnerability> vulnerabilities)
      {
        return vulnerabilities.Select<PublicNuGetHttpClient.RegistrationCatalogEntryVulnerability, NuGetVulnerability>((Func<PublicNuGetHttpClient.RegistrationCatalogEntryVulnerability, NuGetVulnerability>) (x => new NuGetVulnerability(x.AdvisoryUrl, x.Severity))).ToImmutableArray<NuGetVulnerability>();
      }
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<VssNuGetPackageVersion>>> GetPackageVersions(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageName)
    {
      PublicNuGetHttpClient publicNuGetHttpClient = this;
      Uri uri = PublicNuGetClientBase.ConstructEscapedRelativeUri(await publicNuGetHttpClient.GetPackageBaseAddressUri(), "{0}/index.json", packageName.NormalizedName);
      // ISSUE: reference to a compiler-generated method
      return (IReadOnlyList<VersionWithSourceChain<VssNuGetPackageVersion>>) await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<List<VersionWithSourceChain<VssNuGetPackageVersion>>>(publicNuGetHttpClient.HttpClient, publicNuGetHttpClient.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageName) packageName), uri, HttpCompletionOption.ResponseContentRead, new Func<HttpResponseMessage, Task<List<VersionWithSourceChain<VssNuGetPackageVersion>>>>(publicNuGetHttpClient.\u003CGetPackageVersions\u003Eb__11_0)).EnforceCancellation<List<VersionWithSourceChain<VssNuGetPackageVersion>>>(publicNuGetHttpClient.cancellation.Token, file: "D:\\a\\_work\\1\\s\\Packaging\\Service\\NuGet\\Server\\UpstreamClient\\PublicNuGetHttpClient.cs", member: nameof (GetPackageVersions), line: 285);
    }

    public async Task<GetVersionCountsResult> GetVersionCounts(
      NuGetSearchCategoryToggles queryCategories,
      string queryHint)
    {
      PublicNuGetHttpClient publicNuGetHttpClient = this;
      Uri uri = PublicNuGetClientBase.ConstructEscapedRelativeUri(await publicNuGetHttpClient.GetSearchQueryServiceUri(), "?q={0}&prerelease={1}&semVerLevel={2}", queryHint, queryCategories.IncludePrereleaseVersions ? "true" : "false", queryCategories.VersionsWithBuildMetadataAppearToExist ? "2.0.0" : "1.0.0");
      QueryResult queryResult = await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<QueryResult>(publicNuGetHttpClient.HttpClient, publicNuGetHttpClient.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.None, uri, HttpCompletionOption.ResponseContentRead, new Func<HttpResponseMessage, Task<QueryResult>>(publicNuGetHttpClient.DeserializeJson<QueryResult>)).EnforceCancellation<QueryResult>(publicNuGetHttpClient.cancellation.Token, file: "D:\\a\\_work\\1\\s\\Packaging\\Service\\NuGet\\Server\\UpstreamClient\\PublicNuGetHttpClient.cs", member: nameof (GetVersionCounts), line: 306);
      return new GetVersionCountsResult((IEnumerable<ILazyNuGetPackageVersionCounts>) queryResult.Data.Select(queryResultPackage => new
      {
        Package = new VssNuGetPackageName(queryResultPackage.Id),
        Versions = queryResultPackage.Versions.Select<QueryResultPackageVersion, VssNuGetPackageVersion>((Func<QueryResultPackageVersion, VssNuGetPackageVersion>) (queryResultVersion => VssNuGetPackageVersion.ParseOrDefault(queryResultVersion.Version))).Where<VssNuGetPackageVersion>((Func<VssNuGetPackageVersion, bool>) (version => version != null))
      }).Select(element => new
      {
        Package = element.Package,
        Versions = element.Versions.Select<VssNuGetPackageVersion, NuGetSearchResultVersionSummary>((Func<VssNuGetPackageVersion, NuGetSearchResultVersionSummary>) (version => new NuGetSearchResultVersionSummary(new VssNuGetPackageIdentity(element.Package, version), true, (IEnumerable<Guid>) null, false, false)))
      }).Where(element => element.Versions.Any<NuGetSearchResultVersionSummary>()).Select(element => this.versionCountsFromFileProvider.CalculateCounts(queryCategories, Guid.Empty, element.Versions, element.Package, DateTime.MaxValue)).ToImmutableList<NuGetPackageVersionCounts>(), (IVersionCountsImplementationMetrics) new PublicNuGetClientBase.DummyVersionCountsImplementationMetrics(queryResult.Data.Count<QueryResultPackage>(), queryResult.Data.Sum<QueryResultPackage>((Func<QueryResultPackage, int>) (x => x.Versions.Count<QueryResultPackageVersion>()))));
    }

    private async Task<byte[]> GetNuspecInternal(
      Uri packageBaseAddressUri,
      VssNuGetPackageName packageId,
      VssNuGetPackageVersion packageVersion)
    {
      PublicNuGetHttpClient publicNuGetHttpClient = this;
      Uri uri = PublicNuGetClientBase.ConstructEscapedRelativeUri(packageBaseAddressUri, "{0}/{1}/{0}.nuspec", packageId.NormalizedName, packageVersion.NormalizedVersion);
      // ISSUE: reference to a compiler-generated method
      return await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<byte[]>(publicNuGetHttpClient.HttpClient, publicNuGetHttpClient.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageIdentity) new VssNuGetPackageIdentity(packageId, packageVersion)), uri, HttpCompletionOption.ResponseContentRead, new Func<HttpResponseMessage, Task<byte[]>>(publicNuGetHttpClient.\u003CGetNuspecInternal\u003Eb__13_0)).EnforceCancellation<byte[]>(publicNuGetHttpClient.cancellation.Token, file: "D:\\a\\_work\\1\\s\\Packaging\\Service\\NuGet\\Server\\UpstreamClient\\PublicNuGetHttpClient.cs", member: nameof (GetNuspecInternal), line: 364);
    }

    public async Task<Stream> GetNupkg(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageIdentity packageIdentity)
    {
      PublicNuGetHttpClient publicNuGetHttpClient = this;
      Uri uri = PublicNuGetClientBase.ConstructEscapedRelativeUri(await publicNuGetHttpClient.GetPackageBaseAddressUri(), "{0}/{1}/{0}.{1}.nupkg", packageIdentity.Name.NormalizedName, packageIdentity.Version.NormalizedVersion);
      return await PublicUpstreamHttpClientHelper.GetStreamWithErrorHandlingAsync(publicNuGetHttpClient.HttpClient, publicNuGetHttpClient.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageIdentity) packageIdentity), uri, HttpCompletionOption.ResponseHeadersRead).EnforceCancellation<Stream>(publicNuGetHttpClient.cancellation.Token, file: "D:\\a\\_work\\1\\s\\Packaging\\Service\\NuGet\\Server\\UpstreamClient\\PublicNuGetHttpClient.cs", member: nameof (GetNupkg), line: 382);
    }

    public async Task<NuGetCatalogIndex> GetCatalogIndexAsync()
    {
      PublicNuGetHttpClient sendInTheThisObject = this;
      NuGetCatalogIndex catalogIndexAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetCatalogIndexAsync)))
      {
        Uri serviceAddress = await sendInTheThisObject.GetServiceAddress("Catalog/3.0.0", false);
        catalogIndexAsync = await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<NuGetCatalogIndex>(sendInTheThisObject.HttpClient, sendInTheThisObject.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.None, serviceAddress, HttpCompletionOption.ResponseContentRead, new Func<HttpResponseMessage, Task<NuGetCatalogIndex>>(sendInTheThisObject.DeserializeJson<NuGetCatalogIndex>)).EnforceCancellation<NuGetCatalogIndex>(sendInTheThisObject.cancellation.Token, file: "D:\\a\\_work\\1\\s\\Packaging\\Service\\NuGet\\Server\\UpstreamClient\\PublicNuGetHttpClient.cs", member: nameof (GetCatalogIndexAsync), line: 398);
      }
      return catalogIndexAsync;
    }

    public async Task<NuGetCatalogPage> GetCatalogPageAsync(NuGetCatalogIndexPageReference pageRef)
    {
      PublicNuGetHttpClient sendInTheThisObject = this;
      NuGetCatalogPage catalogPageAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetCatalogPageAsync)))
        catalogPageAsync = await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<NuGetCatalogPage>(sendInTheThisObject.HttpClient, sendInTheThisObject.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.None, pageRef.PageUri, HttpCompletionOption.ResponseContentRead, new Func<HttpResponseMessage, Task<NuGetCatalogPage>>(sendInTheThisObject.DeserializeJson<NuGetCatalogPage>)).EnforceCancellation<NuGetCatalogPage>(sendInTheThisObject.cancellation.Token, file: "D:\\a\\_work\\1\\s\\Packaging\\Service\\NuGet\\Server\\UpstreamClient\\PublicNuGetHttpClient.cs", member: nameof (GetCatalogPageAsync), line: 411);
      return catalogPageAsync;
    }

    private async Task<Uri> GetRegistrationsBaseUrl()
    {
      Uri registrationsBaseUrl = await this.GetServiceAddressOrDefault("RegistrationsBaseUrl/3.6.0", true);
      if ((object) registrationsBaseUrl == null)
      {
        Uri uri1 = await this.GetServiceAddressOrDefault("RegistrationsBaseUrl/3.4.0", true);
        if ((object) uri1 == null)
        {
          Uri uri2 = await this.GetServiceAddressOrDefault("RegistrationsBaseUrl/3.0.0-rc", true);
          if ((object) uri2 == null)
          {
            Uri uri3 = await this.GetServiceAddressOrDefault("RegistrationsBaseUrl/3.0.0-beta", true);
            if ((object) uri3 == null)
              uri3 = await this.GetServiceAddress("RegistrationsBaseUrl", true);
            uri2 = uri3;
          }
          uri1 = uri2;
        }
        registrationsBaseUrl = uri1;
      }
      return registrationsBaseUrl;
    }

    private async Task<Uri> GetPackageBaseAddressUri() => await this.GetServiceAddress("PackageBaseAddress", true);

    private async Task<Uri> GetSearchQueryServiceUri() => await this.GetServiceAddress("SearchQueryService", false);

    private async Task<Uri> GetServiceAddress(string serviceName, bool mustEndInSlash)
    {
      Uri addressOrDefault = await this.GetServiceAddressOrDefault(serviceName, mustEndInSlash);
      return !(addressOrDefault == (Uri) null) ? addressOrDefault : throw new InvalidDataException(Resources.Error_UpstreamInvalidServiceIndex());
    }

    private async Task<Uri?> GetServiceAddressOrDefault(string serviceName, bool mustEndInSlash)
    {
      Uri addressOrDefault;
      try
      {
        Uri serviceOrDefault = (await this.lazyServiceIndex.Value).GetServiceOrDefault(serviceName);
        addressOrDefault = !(serviceOrDefault == (Uri) null) ? (mustEndInSlash ? serviceOrDefault.EnsurePathEndsInSlash() : serviceOrDefault) : (Uri) null;
      }
      catch (Exception ex) when (!(ex is PublicUpstreamFailureException))
      {
        throw new PublicUpstreamFailureException(Resources.Error_UpstreamFailure((object) this.packageSourceUri.AbsoluteUri, (object) ex.Message), ex, this.packageSourceUri);
      }
      return addressOrDefault;
    }

    private async Task<ServiceIndex> FetchServiceIndex()
    {
      PublicNuGetHttpClient publicNuGetHttpClient = this;
      return await publicNuGetHttpClient.publicServiceIndex.GetServiceIndex(publicNuGetHttpClient.packageSourceUri, new Func<Uri, Task<ServiceIndex>>(publicNuGetHttpClient.FetchServiceIndexFromNetworkAsync));
    }

    private async Task<ServiceIndex> FetchServiceIndexFromNetworkAsync(Uri packageSourceUri)
    {
      PublicNuGetHttpClient publicNuGetHttpClient = this;
      return await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<ServiceIndex>(publicNuGetHttpClient.HttpClient, packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.None, packageSourceUri, HttpCompletionOption.ResponseContentRead, new Func<HttpResponseMessage, Task<ServiceIndex>>(publicNuGetHttpClient.DeserializeJson<ServiceIndex>)).EnforceCancellation<ServiceIndex>(publicNuGetHttpClient.cancellation.Token, file: "D:\\a\\_work\\1\\s\\Packaging\\Service\\NuGet\\Server\\UpstreamClient\\PublicNuGetHttpClient.cs", member: nameof (FetchServiceIndexFromNetworkAsync), line: 492);
    }

    private async Task<T> DeserializeJson<T>(HttpResponseMessage response)
    {
      Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
      T obj;
      using (Stream stream = await response.Content.ReadAsStreamAsync().EnforceCancellation<Stream>(this.cancellation.Token, file: "D:\\a\\_work\\1\\s\\Packaging\\Service\\NuGet\\Server\\UpstreamClient\\PublicNuGetHttpClient.cs", member: nameof (DeserializeJson), line: 499))
      {
        using (StreamReader reader1 = new StreamReader(stream))
        {
          using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
            obj = serializer.Deserialize<T>((JsonReader) reader2) ?? throw new InvalidOperationException("deserialize returned null");
        }
      }
      serializer = (Newtonsoft.Json.JsonSerializer) null;
      return obj;
    }

    private record RegistrationCatalogEntry(
      PublicNuGetHttpClient.RegistrationCatalogEntryDeprecation? Deprecation,
      string Id,
      bool? Listed,
      DateTime? Published,
      string Version,
      IReadOnlyList<PublicNuGetHttpClient.RegistrationCatalogEntryVulnerability>? Vulnerabilities)
    ;

    private record RegistrationCatalogEntryAlternatePackage(string Id, string? Range);

    private record RegistrationCatalogEntryDeprecation(
      IReadOnlyList<string> Reasons,
      string? Message,
      PublicNuGetHttpClient.RegistrationCatalogEntryAlternatePackage? AlternatePackage)
    ;

    private record RegistrationCatalogEntryVulnerability(string AdvisoryUrl, string Severity);

    private record RegistrationItem(
      PublicNuGetHttpClient.RegistrationCatalogEntry CatalogEntry,
      NuGetCatalogCursor? CatalogCursorPosition)
    ;

    private record RegistrationPage(
      Uri Id,
      IReadOnlyList<PublicNuGetHttpClient.RegistrationItem>? Items,
      string Lower,
      string Upper,
      NuGetCatalogCursor? CatalogCursorPosition)
    ;

    private record RegistrationIndex(
      IReadOnlyList<PublicNuGetHttpClient.RegistrationPage> Items,
      NuGetCatalogCursor? CatalogCursorPosition)
    ;
  }
}
