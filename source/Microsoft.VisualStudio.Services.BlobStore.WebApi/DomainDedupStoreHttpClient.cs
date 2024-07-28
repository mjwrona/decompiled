// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DomainDedupStoreHttpClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [ResourceArea("D7C52D59-024A-4376-A82D-AB6F81122D14")]
  public class DomainDedupStoreHttpClient : 
    ArtifactHttpClient,
    IDomainDedupStoreHttpClient,
    IArtifactHttpClient
  {
    private const string EnvironmentVariablePrefix = "VSO_DEDUP_";
    public static readonly StringWithQualityHeaderValue XpressCompressionHeader = new StringWithQualityHeaderValue("xpress");
    private static readonly HttpClient basicClient;
    private HttpClient proxyHttpClient;
    private RequestHandler requestHandler;

    static DomainDedupStoreHttpClient()
    {
      if (VssHttpMessageHandler.DefaultWebProxy != null)
        DomainDedupStoreHttpClient.basicClient = new HttpClient((HttpMessageHandler) new HttpClientHandler()
        {
          Proxy = VssHttpMessageHandler.DefaultWebProxy,
          UseProxy = true
        });
      else
        DomainDedupStoreHttpClient.basicClient = new HttpClient();
      if (!("1" == Environment.GetEnvironmentVariable("VSO_DEDUP_DISABLE_TIMEOUT")))
        return;
      DomainDedupStoreHttpClient.basicClient.Timeout = TimeSpan.FromMilliseconds(-1.0);
    }

    public long Calls => this.requestHandler.calls;

    public long ThrottledCalls => this.requestHandler.throttledCalls;

    public long XCacheHits => this.requestHandler.xCacheHits;

    public long XCacheMisses => this.requestHandler.xCacheMisses;

    public DomainDedupStoreHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
      this.Initialize(credentials);
    }

    public DomainDedupStoreHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
      this.Initialize(credentials);
    }

    public DomainDedupStoreHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
      this.Initialize(credentials);
    }

    public DomainDedupStoreHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
      this.Initialize(credentials);
    }

    public DomainDedupStoreHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
      this.Initialize(pipeline);
      this.requestHandler = new RequestHandler(this.tracer, DomainDedupStoreHttpClient.basicClient);
    }

    public bool RouteViaQueryString { get; set; }

    public int TotalPublishRetryCount { get; set; } = 1;

    private void Initialize(VssCredentials credentials)
    {
      if (credentials == null)
        return;
      this.Initialize((HttpMessageHandler) new VssHttpMessageHandler(credentials, new VssHttpRequestSettings()));
    }

    private void Initialize(HttpMessageHandler messageHandler)
    {
      this.proxyHttpClient = new HttpClient(messageHandler, true);
      this.requestHandler = new RequestHandler(this.tracer, DomainDedupStoreHttpClient.basicClient);
    }

    public override void SetTracer(IAppTraceSource tracer)
    {
      base.SetTracer(tracer);
      this.requestHandler.SetTracer(tracer);
    }

    public void SetRedirectTimeout(int? redirectTimeOutSeconds) => this.requestHandler.SetRedirectTimeout(redirectTimeOutSeconds);

    public override Guid ResourceId => ResourceIds.MultiDomainChunkResourceId;

    public async Task<MaybeCached<DedupCompressedBuffer>> GetChunkAsync(
      IDomainId domainId,
      ChunkDedupIdentifier dedupId,
      bool canRedirect,
      CancellationToken cancellationToken)
    {
      return (DedupIdentifier) dedupId == DedupStoreHttpClient.EmptyChunk ? MaybeCached.FromCached<DedupCompressedBuffer>(DedupCompressedBuffer.FromUncompressed(Array.Empty<byte>())) : MaybeCached.FromUncached<DedupCompressedBuffer>(await this.GetDedupAsync(domainId, ResourceIds.MultiDomainChunkResourceId, (DedupIdentifier) dedupId, canRedirect, cancellationToken).ConfigureAwait(false));
    }

    public async Task<MaybeCached<DedupCompressedBuffer>> GetNodeAsync(
      IDomainId domainId,
      NodeDedupIdentifier dedupId,
      bool canRedirect,
      CancellationToken cancellationToken)
    {
      return (DedupIdentifier) dedupId == DedupStoreHttpClient.EmptyNode ? MaybeCached.FromCached<DedupCompressedBuffer>(DedupCompressedBuffer.FromUncompressed(Array.Empty<byte>())) : MaybeCached.FromUncached<DedupCompressedBuffer>(await this.GetDedupAsync(domainId, ResourceIds.MultiDomainNodeResourceId, (DedupIdentifier) dedupId, canRedirect, cancellationToken).ConfigureAwait(false));
    }

    public async Task<Dictionary<DedupIdentifier, PreauthenticatedUri>> GetDedupUrlsAsync(
      IDomainId domainId,
      ISet<DedupIdentifier> dedupIds,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      return (await this.GetDedupUrlsRawAsync(domainId, dedupIds, edgeCache, cancellationToken).ConfigureAwait(false)).ToDictionary<KeyValuePair<string, string>, DedupIdentifier, PreauthenticatedUri>((Func<KeyValuePair<string, string>, DedupIdentifier>) (kvp => DedupIdentifier.Deserialize(kvp.Key)), (Func<KeyValuePair<string, string>, PreauthenticatedUri>) (kvp => new PreauthenticatedUri(new Uri(kvp.Value), EdgeType.Unknown)));
    }

    public async Task<Dictionary<DedupIdentifier, GetDedupAsyncFunc>> GetDedupGettersAsync(
      IDomainId domainId,
      ISet<DedupIdentifier> dedupIds,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken,
      bool haveProxyRetrieveSasUris = false)
    {
      return proxyUri != (Uri) null & haveProxyRetrieveSasUris ? dedupIds.ToDictionary<DedupIdentifier, DedupIdentifier, GetDedupAsyncFunc>((Func<DedupIdentifier, DedupIdentifier>) (dedupId => dedupId), (Func<DedupIdentifier, GetDedupAsyncFunc>) (dedupId => (GetDedupAsyncFunc) (ct =>
      {
        if (dedupId == DedupStoreHttpClient.EmptyChunk || dedupId == DedupStoreHttpClient.EmptyNode)
          return MaybeCached.FromCached<DedupCompressedBuffer>(DedupCompressedBuffer.FromUncompressed(Array.Empty<byte>()));
        return MaybeCached.FromUncached<DedupCompressedBuffer>(await this.requestHandler.HandleRedirectAsync(false, ProxyUriHelper.GetProxyDownloadUri(domainId, dedupId.ToBlobIdentifier(), proxyUri, this.BaseAddress), this.proxyHttpClient, ct).ConfigureAwait(false));
      }))) : (await this.GetDedupUrlsRawAsync(domainId, dedupIds, edgeCache, cancellationToken).ConfigureAwait(false)).ToDictionary<KeyValuePair<string, string>, DedupIdentifier, GetDedupAsyncFunc>((Func<KeyValuePair<string, string>, DedupIdentifier>) (kvp => DedupIdentifier.Create(kvp.Key)), (Func<KeyValuePair<string, string>, GetDedupAsyncFunc>) (kvp => (GetDedupAsyncFunc) (ct =>
      {
        if (kvp.Key == DedupStoreHttpClient.EmptyChunk.ValueString || kvp.Key == DedupStoreHttpClient.EmptyNode.ValueString)
          return MaybeCached.FromCached<DedupCompressedBuffer>(DedupCompressedBuffer.FromUncompressed(Array.Empty<byte>()));
        DedupCompressedBuffer compressedBuffer;
        if (proxyUri != (Uri) null)
          compressedBuffer = await this.requestHandler.HandleRedirectAsync(false, ProxyUriHelper.GetProxyDownloadUri(BlobIdentifier.Deserialize(kvp.Key), new Uri(kvp.Value), proxyUri, this.BaseAddress), this.proxyHttpClient, ct).ConfigureAwait(false);
        else
          compressedBuffer = await this.requestHandler.HandleRedirectAsync(false, new Uri(kvp.Value), (HttpClient) null, ct).ConfigureAwait(false);
        return MaybeCached.FromUncached<DedupCompressedBuffer>(compressedBuffer);
      })));
    }

    private async Task<Dictionary<string, string>> GetDedupUrlsRawAsync(
      IDomainId domainId,
      ISet<DedupIdentifier> dedupIds,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      DomainDedupStoreHttpClient dedupStoreHttpClient1 = this;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (edgeCache == EdgeCache.Allowed)
        dictionary["allowEdge"] = "true";
      DomainDedupStoreHttpClient dedupStoreHttpClient2 = dedupStoreHttpClient1;
      HttpMethod post = HttpMethod.Post;
      Guid dedupUrlsResourceId = ResourceIds.MultiDomainDedupUrlsResourceId;
      ApiResourceVersion apiResourceVersion = new ApiResourceVersion(new Version(5, 1));
      var routeValues = new
      {
        domainId = domainId.Serialize()
      };
      ApiResourceVersion version = apiResourceVersion;
      HttpContent content = JsonSerializer.SerializeToContent<string[]>(dedupIds.Select<DedupIdentifier, string>((Func<DedupIdentifier, string>) (d => d.ValueString)).ToArray<string>());
      Dictionary<string, string> queryParameters = dictionary;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpResponseMessage httpResponseMessage = await dedupStoreHttpClient2.SendAsync(post, dedupUrlsResourceId, (object) routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return JsonSerializer.Deserialize<Dictionary<string, string>>(await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false));
    }

    public async Task<KeepUntilReceipt> PutChunkAndKeepUntilReferenceAsync(
      IDomainId domainId,
      ChunkDedupIdentifier dedupId,
      DedupCompressedBuffer chunkBuffer,
      KeepUntilBlobReference keepUntil,
      CancellationToken cancellationToken)
    {
      KeepUntilReceipt keepUntilReceipt;
      try
      {
        keepUntilReceipt = JsonSerializer.Deserialize<KeepUntilReceipt>(await (await this.PutDedupAsync(domainId, ResourceIds.MultiDomainChunkResourceId, (DedupIdentifier) dedupId, chunkBuffer, keepUntil, (SummaryKeepUntilReceipt) null, cancellationToken).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false));
      }
      catch (VssServiceResponseException ex) when (ex.InnerException is ArgumentException)
      {
        throw ex.InnerException;
      }
      return keepUntilReceipt;
    }

    public async Task<PutNodeResponse> PutNodeAndKeepUntilReferenceAsync(
      IDomainId domainId,
      NodeDedupIdentifier dedupId,
      DedupCompressedBuffer chunkBuffer,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipt,
      CancellationToken cancellationToken)
    {
      try
      {
        HttpResponseMessage response = await this.PutDedupAsync(domainId, ResourceIds.MultiDomainNodeResourceId, (DedupIdentifier) dedupId, chunkBuffer, keepUntil, receipt, cancellationToken).ConfigureAwait(false);
        string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.OK)
          return new PutNodeResponse(new DedupNodeUpdated(JsonSerializer.Deserialize<Dictionary<string, KeepUntilReceipt>>(json).ToDictionary<KeyValuePair<string, KeepUntilReceipt>, DedupIdentifier, KeepUntilReceipt>((Func<KeyValuePair<string, KeepUntilReceipt>, DedupIdentifier>) (r => DedupIdentifier.Create(r.Key)), (Func<KeyValuePair<string, KeepUntilReceipt>, KeepUntilReceipt>) (r => r.Value))));
        if (response.StatusCode == HttpStatusCode.Conflict)
          return new PutNodeResponse(JsonSerializer.Deserialize<DedupNodeChildrenNeedAction>(json));
        throw new InvalidOperationException();
      }
      catch (VssServiceResponseException ex) when (ex.InnerException is ArgumentException)
      {
        throw ex.InnerException;
      }
    }

    public async Task<KeepUntilReceipt> TryKeepUntilReferenceChunkAsync(
      IDomainId domainId,
      ChunkDedupIdentifier dedupId,
      KeepUntilBlobReference keepUntil,
      CancellationToken cancellationToken)
    {
      try
      {
        HttpResponseMessage httpResponseMessage = await this.TryReferenceDedupAsync(domainId, ResourceIds.MultiDomainChunkResourceId, (DedupIdentifier) dedupId, keepUntil, (SummaryKeepUntilReceipt) null, cancellationToken).ConfigureAwait(false);
        return httpResponseMessage.StatusCode == HttpStatusCode.OK ? JsonSerializer.Deserialize<KeepUntilReceipt>(await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false)) : throw new InvalidOperationException();
      }
      catch (DedupNotFoundException ex)
      {
        return (KeepUntilReceipt) null;
      }
    }

    public async Task<TryReferenceNodeResponse> TryKeepUntilReferenceNodeAsync(
      IDomainId domainId,
      NodeDedupIdentifier dedupId,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipt,
      CancellationToken cancellationToken)
    {
      try
      {
        HttpResponseMessage httpResponseMessage = await this.TryReferenceDedupAsync(domainId, ResourceIds.MultiDomainNodeResourceId, (DedupIdentifier) dedupId, keepUntil, receipt, cancellationToken).ConfigureAwait(false);
        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
          return new TryReferenceNodeResponse(new DedupNodeUpdated(JsonSerializer.Deserialize<Dictionary<string, KeepUntilReceipt>>(await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false)).ToDictionary<KeyValuePair<string, KeepUntilReceipt>, DedupIdentifier, KeepUntilReceipt>((Func<KeyValuePair<string, KeepUntilReceipt>, DedupIdentifier>) (r => DedupIdentifier.Create(r.Key)), (Func<KeyValuePair<string, KeepUntilReceipt>, KeepUntilReceipt>) (r => r.Value))));
        return httpResponseMessage.StatusCode == HttpStatusCode.Conflict ? new TryReferenceNodeResponse(JsonSerializer.Deserialize<DedupNodeChildrenNotEnoughKeepUntil>(await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false))) : throw new InvalidOperationException();
      }
      catch (DedupNotFoundException ex)
      {
        return new TryReferenceNodeResponse(new DedupNodeNotFound());
      }
    }

    private async Task<HttpResponseMessage> TryReferenceDedupAsync(
      IDomainId domainId,
      Guid locationId,
      DedupIdentifier dedupId,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipt,
      CancellationToken cancellationToken)
    {
      Dictionary<string, string> queryParameters = new Dictionary<string, string>();
      queryParameters.Add(nameof (keepUntil), keepUntil.KeepUntilString);
      object routeValues;
      if (this.RouteViaQueryString)
      {
        queryParameters.Add(nameof (dedupId), dedupId.ValueString);
        routeValues = (object) new
        {
          domainId = domainId.Serialize()
        };
      }
      else
        routeValues = (object) new
        {
          domainId = domainId.Serialize(),
          dedupId = dedupId.ValueString
        };
      Func<Dictionary<string, string>, Task<HttpResponseMessage>> createAndSendAsync = (Func<Dictionary<string, string>, Task<HttpResponseMessage>>) (async headers => await this.SendAsync(await this.CreateRequestMessageAsync(HttpMethod.Post, (IEnumerable<KeyValuePair<string, string>>) headers, locationId, routeValues, new ApiResourceVersion(new Version(5, 1)), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false));
      int num;
      try
      {
        return await createAndSendAsync(this.requestHandler.CreateHeadersFromReceipts(receipt)).ConfigureAwait(false);
      }
      catch (VssServiceResponseException ex) when (ex.HttpStatusCode == HttpStatusCode.BadRequest && ex.Message.Equals("Bad Request", StringComparison.InvariantCultureIgnoreCase))
      {
        num = 1;
      }
      if (num == 1)
        return await createAndSendAsync((Dictionary<string, string>) null).ConfigureAwait(false);
      createAndSendAsync = (Func<Dictionary<string, string>, Task<HttpResponseMessage>>) null;
      HttpResponseMessage httpResponseMessage;
      return httpResponseMessage;
    }

    public int RecommendedChunkCountPerCall { get; set; } = 64;

    public async Task<Dictionary<ChunkDedupIdentifier, KeepUntilReceipt>> PutChunksAsync(
      IDomainId domainId,
      Dictionary<ChunkDedupIdentifier, DedupCompressedBuffer> allChunks,
      KeepUntilBlobReference keepUntil,
      CancellationToken cancellationToken)
    {
      DomainDedupStoreHttpClient dedupStoreHttpClient = this;
      Dictionary<ChunkDedupIdentifier, KeepUntilReceipt> results = new Dictionary<ChunkDedupIdentifier, KeepUntilReceipt>(allChunks.Count);
      // ISSUE: explicit non-virtual call
      foreach (List<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>> page in allChunks.GetPages<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>>(__nonvirtual (dedupStoreHttpClient.RecommendedChunkCountPerCall)))
      {
        List<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>> chunkPage = page;
        HttpResponseMessage httpResponseMessage = await AsyncHttpRetryHelper.InvokeAsync<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (() =>
        {
          List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
          byte[] numArray = new byte[chunkPage.Sum<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>>((Func<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>, int>) (chunk =>
          {
            ArraySegment<byte> buffer;
            chunk.Value.GetBytes(out bool _, out buffer);
            return buffer.Count;
          }))];
          int dstOffset = 0;
          foreach (KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer> keyValuePair in chunkPage)
          {
            bool isCompressed;
            ArraySegment<byte> buffer;
            keyValuePair.Value.GetBytes(out isCompressed, out buffer);
            System.Buffer.BlockCopy((Array) buffer.Array, buffer.Offset, (Array) numArray, dstOffset, buffer.Count);
            string str = isCompressed ? "true" : "false";
            collection.Add("X-ms-chunk-" + keyValuePair.Key.ValueString, string.Format("{0}/{1}", (object) buffer.Count, (object) str));
            dstOffset += buffer.Count;
          }
          HttpContent httpContent = (HttpContent) new ByteArrayContent(numArray);
          httpContent.Headers.ContentLength = new long?((long) numArray.Length);
          httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
          httpContent.Headers.ContentRange = new ContentRangeHeaderValue((long) numArray.Length);
          Dictionary<string, string> dictionary = new Dictionary<string, string>();
          dictionary.Add(nameof (keepUntil), keepUntil.KeepUntilString);
          HttpMethod put = HttpMethod.Put;
          List<KeyValuePair<string, string>> additionalHeaders = collection;
          Guid domainChunkResourceId = ResourceIds.MultiDomainChunkResourceId;
          ApiResourceVersion apiResourceVersion = new ApiResourceVersion(new Version(5, 1));
          var routeValues = new
          {
            domainId = domainId.Serialize()
          };
          ApiResourceVersion version = apiResourceVersion;
          IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) dictionary;
          HttpContent content = httpContent;
          IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
          CancellationToken cancellationToken1 = cancellationToken;
          return this.SendAsync(put, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, domainChunkResourceId, (object) routeValues, version, content, queryParameters, cancellationToken: cancellationToken1);
        }), dedupStoreHttpClient.TotalPublishRetryCount, dedupStoreHttpClient.tracer, cancellationToken, false, nameof (PutChunksAsync)).ConfigureAwait(false);
        httpResponseMessage.EnsureSuccessStatusCode();
        foreach (KeyValuePair<string, KeepUntilReceipt> keyValuePair in JsonSerializer.Deserialize<Dictionary<string, KeepUntilReceipt>>(await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false)))
          results.Add(DedupIdentifier.Create(keyValuePair.Key).CastToChunkDedupIdentifier(), keyValuePair.Value);
      }
      Dictionary<ChunkDedupIdentifier, KeepUntilReceipt> dictionary1 = results;
      results = (Dictionary<ChunkDedupIdentifier, KeepUntilReceipt>) null;
      return dictionary1;
    }

    public async Task PutRootAsync(
      IDomainId domainId,
      DedupIdentifier dedupId,
      IdBlobReference rootRef,
      CancellationToken cancellationToken)
    {
      HttpResponseMessage httpResponseMessage = await this.PutRootAsync(domainId, ResourceIds.MultiDomainRootResourceId, dedupId, rootRef, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteRootAsync(
      IDomainId domainId,
      DedupIdentifier dedupId,
      IdBlobReference rootRef,
      CancellationToken cancellationToken)
    {
      HttpResponseMessage httpResponseMessage = await this.DeleteRootAsync(domainId, ResourceIds.MultiDomainRootResourceId, dedupId, rootRef, cancellationToken).ConfigureAwait(false);
    }

    public async Task<DedupDownloadInfo> GetDownloadInfoAsync(
      IDomainId domainId,
      DedupIdentifier dedupId,
      bool includeChunks,
      CancellationToken cancellationToken)
    {
      DomainDedupStoreHttpClient dedupStoreHttpClient = this;
      Dictionary<string, string> queryParameters = new Dictionary<string, string>();
      queryParameters.Add(nameof (includeChunks), includeChunks.ToString());
      object routeValues;
      if (dedupStoreHttpClient.RouteViaQueryString)
      {
        queryParameters.Add(nameof (dedupId), dedupId.ValueString);
        routeValues = (object) new
        {
          domainId = domainId.Serialize()
        };
      }
      else
        routeValues = (object) new
        {
          domainId = domainId.Serialize(),
          dedupId = dedupId.ValueString
        };
      HttpRequestMessage message = await dedupStoreHttpClient.CreateRequestMessageAsync(HttpMethod.Get, ResourceIds.MultiDomainDedupUrlsResourceId, routeValues, new ApiResourceVersion(new Version(5, 1)), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters).ConfigureAwait(false);
      return JsonSerializer.Deserialize<DedupDownloadInfo>(await (await dedupStoreHttpClient.SendAsync(message, cancellationToken: cancellationToken).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false));
    }

    public async Task<IList<DedupDownloadInfo>> GetBatchDownloadInfoAsync(
      IDomainId domainId,
      ISet<DedupIdentifier> dedupIds,
      bool includeChunks,
      CancellationToken cancellationToken)
    {
      DomainDedupStoreHttpClient dedupStoreHttpClient1 = this;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add(nameof (includeChunks), includeChunks.ToString());
      ISet<string> hashSet = (ISet<string>) dedupIds.Select<DedupIdentifier, string>((Func<DedupIdentifier, string>) (d => d.ValueString)).ToHashSet<string, string>((Func<string, string>) (x => x));
      HttpContent content1 = JsonSerializer.SerializeToContent<DedupIdBatch>(new DedupIdBatch()
      {
        DedupIds = hashSet
      });
      DomainDedupStoreHttpClient dedupStoreHttpClient2 = dedupStoreHttpClient1;
      HttpMethod post = HttpMethod.Post;
      Guid urlsBatchResourceId = ResourceIds.MultiDomainDedupUrlsBatchResourceId;
      var routeValues = new
      {
        domainId = domainId.Serialize()
      };
      HttpContent httpContent = content1;
      ApiResourceVersion version = new ApiResourceVersion(new Version(5, 1));
      HttpContent content2 = httpContent;
      Dictionary<string, string> queryParameters = dictionary;
      CancellationToken cancellationToken1 = new CancellationToken();
      HttpRequestMessage message = await dedupStoreHttpClient2.CreateRequestMessageAsync(post, urlsBatchResourceId, (object) routeValues, version, content2, (IEnumerable<KeyValuePair<string, string>>) queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false);
      return (IList<DedupDownloadInfo>) JsonSerializer.Deserialize<List<DedupDownloadInfo>>(await (await dedupStoreHttpClient1.SendAsync(message, cancellationToken: cancellationToken).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false));
    }

    public override Task GetOptionsAsync(CancellationToken cancellationToken) => (Task) this.SendAsync(HttpMethod.Options, this.ResourceId, version: ResourceIds.MultiDomainApiVersion, cancellationToken: cancellationToken);

    private Task<HttpResponseMessage> PutRootAsync(
      IDomainId domainId,
      Guid locationId,
      DedupIdentifier dedupId,
      IdBlobReference rootRef,
      CancellationToken cancellationToken)
    {
      return AsyncHttpRetryHelper.InvokeAsync<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (async () =>
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        object obj;
        if (this.RouteViaQueryString)
        {
          dictionary.Add(nameof (dedupId), dedupId.ValueString);
          dictionary.Add("name", rootRef.Name);
          obj = (object) new
          {
            domainId = domainId.Serialize()
          };
        }
        else
          obj = (object) new
          {
            domainId = domainId.Serialize(),
            dedupId = dedupId.ValueString,
            name = rootRef.Name
          };
        dictionary.Add("scope", rootRef.Scope);
        HttpMethod put = HttpMethod.Put;
        Guid locationId1 = locationId;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) dictionary;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(new Version(5, 1));
        object routeValues = obj;
        ApiResourceVersion version = apiResourceVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        CancellationToken cancellationToken1 = new CancellationToken();
        return await this.SendAsync(await this.CreateRequestMessageAsync(put, locationId1, routeValues, version, queryParameters: queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
      }), this.TotalPublishRetryCount, this.tracer, cancellationToken, false, nameof (PutRootAsync));
    }

    private async Task<HttpResponseMessage> DeleteRootAsync(
      IDomainId domainId,
      Guid locationId,
      DedupIdentifier dedupId,
      IdBlobReference rootRef,
      CancellationToken cancellationToken)
    {
      DomainDedupStoreHttpClient dedupStoreHttpClient1 = this;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      object obj;
      if (dedupStoreHttpClient1.RouteViaQueryString)
      {
        dictionary.Add(nameof (dedupId), dedupId.ValueString);
        dictionary.Add("name", rootRef.Name);
        obj = (object) new
        {
          domainId = domainId.Serialize()
        };
      }
      else
        obj = (object) new
        {
          domainId = domainId.Serialize(),
          dedupId = dedupId.ValueString,
          name = rootRef.Name
        };
      dictionary.Add("scope", rootRef.Scope);
      DomainDedupStoreHttpClient dedupStoreHttpClient2 = dedupStoreHttpClient1;
      HttpMethod delete = HttpMethod.Delete;
      Guid locationId1 = locationId;
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) dictionary;
      ApiResourceVersion apiResourceVersion = new ApiResourceVersion(new Version(5, 1));
      object routeValues = obj;
      ApiResourceVersion version = apiResourceVersion;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = new CancellationToken();
      HttpRequestMessage message = await dedupStoreHttpClient2.CreateRequestMessageAsync(delete, locationId1, routeValues, version, queryParameters: queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false);
      return await dedupStoreHttpClient1.SendAsync(message, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private async Task<HttpResponseMessage> PutDedupAsync(
      IDomainId domainId,
      Guid locationId,
      DedupIdentifier dedupId,
      DedupCompressedBuffer chunkBuffer,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipt,
      CancellationToken cancellationToken)
    {
      ArraySegment<byte> wireBytes;
      bool isCompressed;
      chunkBuffer.GetBytesTryCompress(out isCompressed, out wireBytes);
      Dictionary<string, string> queryParameters = new Dictionary<string, string>();
      queryParameters.Add(nameof (keepUntil), keepUntil.KeepUntilString);
      object routeValues;
      if (this.RouteViaQueryString)
      {
        queryParameters.Add(nameof (dedupId), dedupId.ValueString);
        routeValues = (object) new
        {
          domainId = domainId.Serialize()
        };
      }
      else
        routeValues = (object) new
        {
          domainId = domainId.Serialize(),
          dedupId = dedupId.ValueString
        };
      Func<Dictionary<string, string>, Task<HttpResponseMessage>> createAndSendAsync = (Func<Dictionary<string, string>, Task<HttpResponseMessage>>) (async headers =>
      {
        HttpContent httpContent = (HttpContent) new ByteArrayContent(wireBytes.Array, wireBytes.Offset, wireBytes.Count);
        httpContent.Headers.ContentLength = new long?((long) wireBytes.Count);
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        httpContent.Headers.ContentRange = new ContentRangeHeaderValue((long) wireBytes.Count);
        if (isCompressed)
          httpContent.Headers.ContentEncoding.Add("xpress");
        HttpMethod put = HttpMethod.Put;
        Dictionary<string, string> additionalHeaders = headers;
        Guid locationId1 = locationId;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) queryParameters;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(new Version(5, 1));
        object routeValues1 = routeValues;
        ApiResourceVersion version = apiResourceVersion;
        HttpContent content = httpContent;
        IEnumerable<KeyValuePair<string, string>> queryParameters1 = keyValuePairs;
        CancellationToken cancellationToken1 = new CancellationToken();
        HttpRequestMessage message = await this.CreateRequestMessageAsync(put, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId1, routeValues1, version, content, queryParameters1, cancellationToken: cancellationToken1).ConfigureAwait(false);
        message.Properties.Add("ExpectedStatus", (object) new HttpStatusCode[2]
        {
          HttpStatusCode.Conflict,
          HttpStatusCode.SeeOther
        });
        return await this.SendAsync(message, cancellationToken: cancellationToken).ConfigureAwait(false);
      });
      int num;
      try
      {
        return await createAndSendAsync(this.requestHandler.CreateHeadersFromReceipts(receipt)).ConfigureAwait(false);
      }
      catch (VssServiceResponseException ex) when (ex.HttpStatusCode == HttpStatusCode.BadRequest && ex.Message.Equals("Bad Request", StringComparison.InvariantCultureIgnoreCase))
      {
        num = 1;
      }
      if (num == 1)
        return await createAndSendAsync((Dictionary<string, string>) null).ConfigureAwait(false);
      createAndSendAsync = (Func<Dictionary<string, string>, Task<HttpResponseMessage>>) null;
      HttpResponseMessage httpResponseMessage;
      return httpResponseMessage;
    }

    private async Task<DedupCompressedBuffer> GetDedupAsync(
      IDomainId domainId,
      Guid locationId,
      DedupIdentifier dedupId,
      bool canRedirect,
      CancellationToken cancellationToken)
    {
      DomainDedupStoreHttpClient dedupStoreHttpClient = this;
      ArgumentUtility.CheckForNull<DedupIdentifier>(dedupId, nameof (dedupId));
      Dictionary<string, string> queryParameters = new Dictionary<string, string>();
      queryParameters.Add("redirect", canRedirect.ToString().ToLowerInvariant());
      object routeValues;
      if (dedupStoreHttpClient.RouteViaQueryString)
      {
        queryParameters.Add(nameof (dedupId), dedupId.ValueString);
        routeValues = (object) new
        {
          domainId = domainId.Serialize()
        };
      }
      else
        routeValues = (object) new
        {
          domainId = domainId.Serialize(),
          dedupId = dedupId.ValueString
        };
      HttpResponseMessage responseMessage;
      try
      {
        responseMessage = await dedupStoreHttpClient.GetAsync(locationId, routeValues, new ApiResourceVersion(new Version(5, 1)), (IEnumerable<KeyValuePair<string, string>>) queryParameters, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      catch (DedupNotFoundException ex)
      {
        return (DedupCompressedBuffer) null;
      }
      return await dedupStoreHttpClient.requestHandler.GetDedupAsync(responseMessage, dedupId, cancellationToken);
    }

    protected override bool ShouldThrowError(HttpResponseMessage response) => response.StatusCode != HttpStatusCode.SeeOther && response.StatusCode != HttpStatusCode.Conflict && base.ShouldThrowError(response);
  }
}
