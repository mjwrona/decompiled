// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Generated.MavenHttpClient
// Assembly: Microsoft.VisualStudio.Services.Maven.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62CDE373-A3CE-478E-B824-A307191D9BE2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types.API;
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

namespace Microsoft.VisualStudio.Services.Maven.WebApi.Generated
{
  [ResourceArea("6F7F8C07-FF36-473C-BCF3-BD6CC9B6C066")]
  public class MavenHttpClient : VssHttpClientBase
  {
    public MavenHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public MavenHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public MavenHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public MavenHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public MavenHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<Stream> DownloadPackageAsync(
      string feedId,
      string groupId,
      string artifactId,
      string version,
      string fileName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c338d4b5-d30a-47e2-95b7-f157ef558833");
      object routeValues = (object) new
      {
        feedId = feedId,
        groupId = groupId,
        artifactId = artifactId,
        version = version,
        fileName = fileName
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await mavenHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await mavenHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> DownloadPackageAsync(
      string project,
      string feedId,
      string groupId,
      string artifactId,
      string version,
      string fileName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c338d4b5-d30a-47e2-95b7-f157ef558833");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        groupId = groupId,
        artifactId = artifactId,
        version = version,
        fileName = fileName
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await mavenHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await mavenHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> DownloadPackageAsync(
      Guid project,
      string feedId,
      string groupId,
      string artifactId,
      string version,
      string fileName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c338d4b5-d30a-47e2-95b7-f157ef558833");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        groupId = groupId,
        artifactId = artifactId,
        version = version,
        fileName = fileName
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await mavenHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await mavenHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task UpdatePackageVersionsAsync(
      MavenPackagesBatchRequest batchRequest,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b7c586b0-d947-4d35-811a-f1161de80e6c");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<MavenPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackageVersionsAsync(
      MavenPackagesBatchRequest batchRequest,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b7c586b0-d947-4d35-811a-f1161de80e6c");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MavenPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackageVersionsAsync(
      MavenPackagesBatchRequest batchRequest,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b7c586b0-d947-4d35-811a-f1161de80e6c");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MavenPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdateRecycleBinPackagesAsync(
      MavenPackagesBatchRequest batchRequest,
      string feed,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5dd6f547-c76f-4d9d-b2ec-4720feda641f");
      object obj1 = (object) new{ feed = feed };
      HttpContent httpContent = (HttpContent) new ObjectContent<MavenPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdateRecycleBinPackagesAsync(
      MavenPackagesBatchRequest batchRequest,
      string project,
      string feed,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5dd6f547-c76f-4d9d-b2ec-4720feda641f");
      object obj1 = (object) new
      {
        project = project,
        feed = feed
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MavenPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdateRecycleBinPackagesAsync(
      MavenPackagesBatchRequest batchRequest,
      Guid project,
      string feed,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5dd6f547-c76f-4d9d-b2ec-4720feda641f");
      object obj1 = (object) new
      {
        project = project,
        feed = feed
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MavenPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task DeletePackageVersionFromRecycleBinAsync(
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("f67e10eb-1254-4953-add7-d49b83a16c9f"), (object) new
      {
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task DeletePackageVersionFromRecycleBinAsync(
      string project,
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("f67e10eb-1254-4953-add7-d49b83a16c9f"), (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task DeletePackageVersionFromRecycleBinAsync(
      Guid project,
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("f67e10eb-1254-4953-add7-d49b83a16c9f"), (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<MavenPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<MavenPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("f67e10eb-1254-4953-add7-d49b83a16c9f"), (object) new
      {
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<MavenPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      string project,
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<MavenPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("f67e10eb-1254-4953-add7-d49b83a16c9f"), (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<MavenPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      Guid project,
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<MavenPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("f67e10eb-1254-4953-add7-d49b83a16c9f"), (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RestorePackageVersionFromRecycleBinAsync(
      MavenRecycleBinPackageVersionDetails packageVersionDetails,
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("f67e10eb-1254-4953-add7-d49b83a16c9f");
      object obj1 = (object) new
      {
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MavenRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version1 = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version1, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task RestorePackageVersionFromRecycleBinAsync(
      MavenRecycleBinPackageVersionDetails packageVersionDetails,
      string project,
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("f67e10eb-1254-4953-add7-d49b83a16c9f");
      object obj1 = (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MavenRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version1 = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version1, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task RestorePackageVersionFromRecycleBinAsync(
      MavenRecycleBinPackageVersionDetails packageVersionDetails,
      Guid project,
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("f67e10eb-1254-4953-add7-d49b83a16c9f");
      object obj1 = (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MavenRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version1 = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version1, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<UpstreamingBehavior> GetUpstreamingBehaviorAsync(
      string feed,
      string groupId,
      string artifactId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("fba7ba8c-d1f5-4aeb-8f5d-f017a7d5e719"), (object) new
      {
        feed = feed,
        groupId = groupId,
        artifactId = artifactId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<UpstreamingBehavior> GetUpstreamingBehaviorAsync(
      string project,
      string feed,
      string groupId,
      string artifactId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("fba7ba8c-d1f5-4aeb-8f5d-f017a7d5e719"), (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<UpstreamingBehavior> GetUpstreamingBehaviorAsync(
      Guid project,
      string feed,
      string groupId,
      string artifactId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("fba7ba8c-d1f5-4aeb-8f5d-f017a7d5e719"), (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task SetUpstreamingBehaviorAsync(
      string feed,
      string groupId,
      string artifactId,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("fba7ba8c-d1f5-4aeb-8f5d-f017a7d5e719");
      object obj1 = (object) new
      {
        feed = feed,
        groupId = groupId,
        artifactId = artifactId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task SetUpstreamingBehaviorAsync(
      string project,
      string feed,
      string groupId,
      string artifactId,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("fba7ba8c-d1f5-4aeb-8f5d-f017a7d5e719");
      object obj1 = (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task SetUpstreamingBehaviorAsync(
      Guid project,
      string feed,
      string groupId,
      string artifactId,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("fba7ba8c-d1f5-4aeb-8f5d-f017a7d5e719");
      object obj1 = (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<Package> GetPackageVersionAsync(
      string project,
      string feed,
      string groupId,
      string artifactId,
      string version,
      bool? showDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("180ed967-377a-4112-986b-607adb14ded4");
      object routeValues = (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (showDeleted.HasValue)
        keyValuePairList.Add(nameof (showDeleted), showDeleted.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> GetPackageVersionAsync(
      Guid project,
      string feed,
      string groupId,
      string artifactId,
      string version,
      bool? showDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("180ed967-377a-4112-986b-607adb14ded4");
      object routeValues = (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (showDeleted.HasValue)
        keyValuePairList.Add(nameof (showDeleted), showDeleted.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> GetPackageVersionAsync(
      string feed,
      string groupId,
      string artifactId,
      string version,
      bool? showDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("180ed967-377a-4112-986b-607adb14ded4");
      object routeValues = (object) new
      {
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (showDeleted.HasValue)
        keyValuePairList.Add(nameof (showDeleted), showDeleted.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task PackageDeleteAsync(
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("180ed967-377a-4112-986b-607adb14ded4"), (object) new
      {
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task PackageDeleteAsync(
      string project,
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("180ed967-377a-4112-986b-607adb14ded4"), (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task PackageDeleteAsync(
      Guid project,
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("180ed967-377a-4112-986b-607adb14ded4"), (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackageVersionAsync(
      PackageVersionDetails packageVersionDetails,
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("180ed967-377a-4112-986b-607adb14ded4");
      object obj1 = (object) new
      {
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version1 = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version1, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackageVersionAsync(
      PackageVersionDetails packageVersionDetails,
      string project,
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("180ed967-377a-4112-986b-607adb14ded4");
      object obj1 = (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version1 = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version1, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackageVersionAsync(
      PackageVersionDetails packageVersionDetails,
      Guid project,
      string feed,
      string groupId,
      string artifactId,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MavenHttpClient mavenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("180ed967-377a-4112-986b-607adb14ded4");
      object obj1 = (object) new
      {
        project = project,
        feed = feed,
        groupId = groupId,
        artifactId = artifactId,
        version = version
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MavenHttpClient mavenHttpClient2 = mavenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version1 = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mavenHttpClient2.SendAsync(method, locationId, routeValues, version1, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
