// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Client.Internal.NuGetHttpClientInternal
// Assembly: Microsoft.VisualStudio.Services.NuGet.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E63C245C-898F-41A7-9916-45B2DC75C1BE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Client.Internal.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Generated;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Client.Internal
{
  [ResourceArea("{B3BE7473-68EA-4A81-BFC7-9530BAAA19AD}")]
  public class NuGetHttpClientInternal : NuGetHttpClient
  {
    private static readonly ApiResourceVersion CurrentApiVersion = new ApiResourceVersion("3.0-preview.1");

    public NuGetHttpClientInternal(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public NuGetHttpClientInternal(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public NuGetHttpClientInternal(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public NuGetHttpClientInternal(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public NuGetHttpClientInternal(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task AddPackageFromBlobStoreAsync(
      string project,
      string feedId,
      BlobIdentifierWithBlocks blob,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      object obj = (object) new
      {
        project = project,
        feedId = feedId
      };
      AddPackageFromBlobRequestInternal blobRequestInternal = new AddPackageFromBlobRequestInternal()
      {
        Blob = new Blob(blob)
      };
      HttpMethod method = httpMethod;
      Guid packagesResourceId = ResourceIds.PackagesResourceId;
      object routeValues = obj;
      HttpContent httpContent = (HttpContent) new ObjectContent<AddPackageFromBlobRequestInternal>(blobRequestInternal, (MediaTypeFormatter) new VssJsonMediaTypeFormatter());
      ApiResourceVersion currentApiVersion = NuGetHttpClientInternal.CurrentApiVersion;
      HttpContent content = httpContent;
      object userState1 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      return (Task) this.SendAsync(method, packagesResourceId, routeValues, currentApiVersion, content, userState: userState1, cancellationToken: cancellationToken1);
    }

    public virtual Task AddPackageFromDropAsync(
      string project,
      string feedId,
      string dropName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      object obj = (object) new
      {
        project = project,
        feedId = feedId
      };
      AddPackageFromBlobRequestInternal blobRequestInternal = new AddPackageFromBlobRequestInternal()
      {
        DropName = dropName
      };
      HttpMethod method = httpMethod;
      Guid packagesResourceId = ResourceIds.PackagesResourceId;
      object routeValues = obj;
      HttpContent httpContent = (HttpContent) new ObjectContent<AddPackageFromBlobRequestInternal>(blobRequestInternal, (MediaTypeFormatter) new VssJsonMediaTypeFormatter());
      ApiResourceVersion currentApiVersion = NuGetHttpClientInternal.CurrentApiVersion;
      HttpContent content = httpContent;
      object userState1 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      return (Task) this.SendAsync(method, packagesResourceId, routeValues, currentApiVersion, content, userState: userState1, cancellationToken: cancellationToken1);
    }

    public virtual Task<QueryResult> ExecuteQueryAsync(
      string project,
      string feedId,
      string query = null,
      int? skip = null,
      int? take = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      Dictionary<string, string> queryParameters = new Dictionary<string, string>();
      if (!string.IsNullOrEmpty(query))
        queryParameters.Add("q", query);
      if (skip.HasValue)
        queryParameters.Add(nameof (skip), skip.Value.ToString());
      if (take.HasValue)
        queryParameters.Add(nameof (take), take.Value.ToString());
      return this.SendAsync<QueryResult>(method, ResourceIds.Query2ResourceId, routeValues, NuGetHttpClientInternal.CurrentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Uri> GetServiceIndexUriAsync(string project, string feedId)
    {
      NuGetHttpClientInternal httpClientInternal = this;
      ApiResourceLocation resourceLocation = await httpClientInternal.GetResourceLocationAsync(ResourceIds.ServiceIndexResourceId).ConfigureAwait(false);
      if (resourceLocation == null)
      {
        // ISSUE: explicit non-virtual call
        throw new VssResourceNotFoundException(ResourceIds.ServiceIndexResourceId, __nonvirtual (httpClientInternal.BaseAddress));
      }
      Dictionary<string, object> routeDictionary = VssHttpUriUtility.ToRouteDictionary((object) new
      {
        project = project,
        feedId = feedId
      }, resourceLocation.Area, resourceLocation.ResourceName);
      string relativeUri = VssHttpUriUtility.ReplaceRouteValues(resourceLocation.RouteTemplate, routeDictionary);
      // ISSUE: explicit non-virtual call
      return VssHttpUriUtility.ConcatUri(__nonvirtual (httpClientInternal.BaseAddress), relativeUri);
    }

    public virtual async Task<Uri> GetUriForApiDownloadAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion)
    {
      NuGetHttpClientInternal httpClientInternal = this;
      ApiResourceLocation resourceLocation = await httpClientInternal.GetResourceLocationAsync(ResourceIds.PackageVersionContentResourceId).ConfigureAwait(false);
      if (resourceLocation == null)
      {
        // ISSUE: explicit non-virtual call
        throw new VssResourceNotFoundException(ResourceIds.ServiceIndexResourceId, __nonvirtual (httpClientInternal.BaseAddress));
      }
      Dictionary<string, object> routeDictionary = VssHttpUriUtility.ToRouteDictionary((object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, resourceLocation.Area, resourceLocation.ResourceName);
      string relativeUri = VssHttpUriUtility.ReplaceRouteValues(resourceLocation.RouteTemplate, routeDictionary);
      // ISSUE: explicit non-virtual call
      return VssHttpUriUtility.ConcatUri(__nonvirtual (httpClientInternal.BaseAddress), relativeUri);
    }

    public virtual Task<DropInfo> CreatePackageDropAsync(
      string project,
      string feedId,
      string packageId = null,
      string packageVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c3f4b6de-af66-4511-a19e-388c9aaf86a2");
      object obj = (object) new
      {
        project = project,
        feedId = feedId
      };
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (packageId != null)
        dictionary.Add(nameof (packageId), packageId);
      if (packageVersion != null)
        dictionary.Add(nameof (packageVersion), packageVersion);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) dictionary;
      ApiResourceVersion currentApiVersion = NuGetHttpClientInternal.CurrentApiVersion;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      return this.SendAsync<DropInfo>(method, locationId, routeValues, currentApiVersion, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1);
    }

    public virtual Task<NuGetStorageInfo> GetNuGetPackageContentStorageInfoAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NuGetStorageInfo>(new HttpMethod("GET"), new Guid("aaa1d716-f23e-4783-81ca-d8230a9b3721"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, NuGetHttpClientInternal.CurrentApiVersion, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task ForcePackageUpstreamRefreshAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      string sourceProtocolVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientInternal httpClientInternal = this;
      HttpMethod method = new HttpMethod("HEAD");
      Guid locationId = new Guid("6ea81b8c-7386-490b-a71f-6cf23c80b388");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(sourceProtocolVersion))
        keyValuePairList.Add(nameof (sourceProtocolVersion), sourceProtocolVersion);
      (await httpClientInternal.SendAsync(method, locationId, routeValues, new ApiResourceVersion("3.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken)).EnsureSuccessStatusCode();
    }
  }
}
