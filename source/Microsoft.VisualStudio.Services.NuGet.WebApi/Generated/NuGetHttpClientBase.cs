// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.Generated.NuGetHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi.Generated
{
  [ResourceArea("{B3BE7473-68EA-4A81-BFC7-9530BAAA19AD}")]
  public abstract class NuGetHttpClientBase : VssHttpClientBase
  {
    public NuGetHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public NuGetHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public NuGetHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public NuGetHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public NuGetHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<Stream> DownloadPackageAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      string sourceProtocolVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6ea81b8c-7386-490b-a71f-6cf23c80b388");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (sourceProtocolVersion != null)
        keyValuePairList.Add(nameof (sourceProtocolVersion), sourceProtocolVersion);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await getHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await getHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> DownloadPackageAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      string sourceProtocolVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6ea81b8c-7386-490b-a71f-6cf23c80b388");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (sourceProtocolVersion != null)
        keyValuePairList.Add(nameof (sourceProtocolVersion), sourceProtocolVersion);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await getHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await getHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> DownloadPackageAsync(
      string feedId,
      string packageName,
      string packageVersion,
      string sourceProtocolVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6ea81b8c-7386-490b-a71f-6cf23c80b388");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (sourceProtocolVersion != null)
        keyValuePairList.Add(nameof (sourceProtocolVersion), sourceProtocolVersion);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await getHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await getHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task UpdatePackageVersionsAsync(
      NuGetPackagesBatchRequest batchRequest,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("00c58ea7-d55f-49de-b59f-983533ae11dc");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<NuGetPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePackageVersionsAsync(
      NuGetPackagesBatchRequest batchRequest,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("00c58ea7-d55f-49de-b59f-983533ae11dc");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NuGetPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePackageVersionsAsync(
      NuGetPackagesBatchRequest batchRequest,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("00c58ea7-d55f-49de-b59f-983533ae11dc");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NuGetPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdateRecycleBinPackageVersionsAsync(
      NuGetPackagesBatchRequest batchRequest,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6479ac16-32f4-40f7-aa96-9414de861352");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<NuGetPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdateRecycleBinPackageVersionsAsync(
      NuGetPackagesBatchRequest batchRequest,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6479ac16-32f4-40f7-aa96-9414de861352");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NuGetPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdateRecycleBinPackageVersionsAsync(
      NuGetPackagesBatchRequest batchRequest,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6479ac16-32f4-40f7-aa96-9414de861352");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NuGetPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePackageVersionFromRecycleBinAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("07e88775-e3cb-4408-bbe1-628e036fac8c"), (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePackageVersionFromRecycleBinAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("07e88775-e3cb-4408-bbe1-628e036fac8c"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePackageVersionFromRecycleBinAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("07e88775-e3cb-4408-bbe1-628e036fac8c"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<NuGetPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NuGetPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("07e88775-e3cb-4408-bbe1-628e036fac8c"), (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<NuGetPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NuGetPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("07e88775-e3cb-4408-bbe1-628e036fac8c"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<NuGetPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NuGetPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("07e88775-e3cb-4408-bbe1-628e036fac8c"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task RestorePackageVersionFromRecycleBinAsync(
      NuGetRecycleBinPackageVersionDetails packageVersionDetails,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("07e88775-e3cb-4408-bbe1-628e036fac8c");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NuGetRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task RestorePackageVersionFromRecycleBinAsync(
      NuGetRecycleBinPackageVersionDetails packageVersionDetails,
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("07e88775-e3cb-4408-bbe1-628e036fac8c");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NuGetRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task RestorePackageVersionFromRecycleBinAsync(
      NuGetRecycleBinPackageVersionDetails packageVersionDetails,
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("07e88775-e3cb-4408-bbe1-628e036fac8c");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NuGetRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<UpstreamingBehavior> GetUpstreamingBehaviorAsync(
      string feedId,
      string packageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("b41eec47-6472-4efa-bcd5-a2c5607b66ec"), (object) new
      {
        feedId = feedId,
        packageName = packageName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<UpstreamingBehavior> GetUpstreamingBehaviorAsync(
      string project,
      string feedId,
      string packageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("b41eec47-6472-4efa-bcd5-a2c5607b66ec"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<UpstreamingBehavior> GetUpstreamingBehaviorAsync(
      Guid project,
      string feedId,
      string packageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("b41eec47-6472-4efa-bcd5-a2c5607b66ec"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task SetUpstreamingBehaviorAsync(
      string feedId,
      string packageName,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b41eec47-6472-4efa-bcd5-a2c5607b66ec");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageName = packageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SetUpstreamingBehaviorAsync(
      string project,
      string feedId,
      string packageName,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b41eec47-6472-4efa-bcd5-a2c5607b66ec");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SetUpstreamingBehaviorAsync(
      Guid project,
      string feedId,
      string packageName,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b41eec47-6472-4efa-bcd5-a2c5607b66ec");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<Package> DeletePackageVersionAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("36c9353b-e250-4c57-b040-513c186c3905"), (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Package> DeletePackageVersionAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("36c9353b-e250-4c57-b040-513c186c3905"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Package> DeletePackageVersionAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("36c9353b-e250-4c57-b040-513c186c3905"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Package> GetPackageVersionAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      bool? showDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("36c9353b-e250-4c57-b040-513c186c3905");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (showDeleted.HasValue)
        keyValuePairList.Add(nameof (showDeleted), showDeleted.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Package> GetPackageVersionAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      bool? showDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("36c9353b-e250-4c57-b040-513c186c3905");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (showDeleted.HasValue)
        keyValuePairList.Add(nameof (showDeleted), showDeleted.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Package> GetPackageVersionAsync(
      string feedId,
      string packageName,
      string packageVersion,
      bool? showDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("36c9353b-e250-4c57-b040-513c186c3905");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (showDeleted.HasValue)
        keyValuePairList.Add(nameof (showDeleted), showDeleted.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdatePackageVersionAsync(
      PackageVersionDetails packageVersionDetails,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("36c9353b-e250-4c57-b040-513c186c3905");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePackageVersionAsync(
      PackageVersionDetails packageVersionDetails,
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("36c9353b-e250-4c57-b040-513c186c3905");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePackageVersionAsync(
      PackageVersionDetails packageVersionDetails,
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClientBase getHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("36c9353b-e250-4c57-b040-513c186c3905");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NuGetHttpClientBase getHttpClientBase2 = getHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await getHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
