// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.BlobStore2HttpClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [ResourceArea("5294EF93-12A1-4D13-8671-9D9D014072C8")]
  public class BlobStore2HttpClient : 
    ArtifactHttpClient,
    IBlobStoreHttpClient,
    IArtifactHttpClient,
    IDisposable
  {
    private readonly VssHttpRequestSettings settings;
    public bool ParallelHttpCallsSupported = true;
    protected const string GZip = "gzip";
    protected const int DefaultMaxParallelBlockUpload = 64;
    protected const int DefaultDownloadUriPageSize = 5000;
    protected static readonly int MaxParallelGetDownloadUri = int.Parse(Environment.GetEnvironmentVariable(nameof (MaxParallelGetDownloadUri)) ?? Environment.ProcessorCount.ToString());
    protected static readonly int MaxParallelFileUpload = int.Parse(Environment.GetEnvironmentVariable(nameof (MaxParallelFileUpload)) ?? Environment.ProcessorCount.ToString());
    protected static readonly int MaxParallelBlockUpload = int.Parse(Environment.GetEnvironmentVariable(nameof (MaxParallelBlockUpload)) ?? 64.ToString());
    internal static readonly SemaphoreSlim BlockUploadSemaphore = new SemaphoreSlim(BlobStore2HttpClient.MaxParallelBlockUpload, BlobStore2HttpClient.MaxParallelBlockUpload);
    protected static readonly TimeSpan ServiceToServiceTimeout = ConfigurationManager.AppSettings["BlobStoreHttpClientServiceToServiceTimeout"] == null ? TimeSpan.FromMinutes(5.0) : TimeSpan.Parse(ConfigurationManager.AppSettings["BlobStoreHttpClientServiceToServiceTimeout"]);

    public BlobStore2HttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
      this.settings = (VssHttpRequestSettings) null;
    }

    public BlobStore2HttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
      this.settings = settings;
    }

    public BlobStore2HttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
      this.settings = (VssHttpRequestSettings) null;
    }

    public BlobStore2HttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
      this.settings = settings;
    }

    public BlobStore2HttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
      HttpMessageHandler httpMessageHandler1 = pipeline;
      while (!(httpMessageHandler1 is VssHttpMessageHandler httpMessageHandler2))
      {
        httpMessageHandler1 = !(httpMessageHandler1 is DelegatingHandler delegatingHandler) ? (HttpMessageHandler) null : delegatingHandler.InnerHandler;
        if (httpMessageHandler1 == null)
          return;
      }
      httpMessageHandler2.Settings.SendTimeout = BlobStore2HttpClient.ServiceToServiceTimeout;
    }

    protected override ServicePointExtensions.ServicePointConfig GetServicePointSettings() => base.GetServicePointSettings() with
    {
      ConnectionLeaseTimeout = new TimeSpan?(TimeSpan.FromMinutes(3.0))
    };

    protected string TargetVersion => "2.1-preview.1";

    public virtual async Task<PreauthenticatedUri> GetDownloadUriAsync(
      BlobIdWithHeaders blobId,
      CancellationToken cancellationToken)
    {
      BlobStore2HttpClient store2HttpClient1 = this;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (blobId.FileName != null)
        dictionary["filename"] = blobId.FileName;
      if (blobId.ContentType != null)
        dictionary["contentType"] = blobId.ContentType;
      if (blobId.ExpiryTime.HasValue)
      {
        DateTimeOffset dateTimeOffset = blobId.ExpiryTime.Value + TimeSpan.FromSeconds(1.0);
        dictionary["expiryTime"] = dateTimeOffset.ToString(KeepUntilBlobReference.KeepUntilFormat, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      if (blobId.EdgeCache == EdgeCache.Allowed)
        dictionary["allowEdge"] = "true";
      VssHttpRequestSettings settings = store2HttpClient1.settings;
      TimeSpan delay = TimeSpan.FromTicks(2L * (settings != null ? settings.SendTimeout : TimeSpan.Zero).Ticks);
      string json;
      using (delay.Ticks <= 0L ? new CancellationTokenSource() : new CancellationTokenSource(delay))
      {
        BlobStore2HttpClient store2HttpClient2 = store2HttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid blobUrlResourceId = ResourceIds.BlobUrlResourceId;
        Dictionary<string, object> routeValues = new Dictionary<string, object>();
        routeValues.Add(nameof (blobId), (object) blobId.BlobId.ValueString);
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) dictionary;
        ApiResourceVersion version = new ApiResourceVersion(store2HttpClient1.TargetVersion);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpResponseMessage httpResponseMessage = await store2HttpClient2.SendAsync(get, blobUrlResourceId, (object) routeValues, version, queryParameters: queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
          if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            throw new BlobNotFoundException(string.Format("Blob {0} not found", (object) blobId.BlobId));
          throw new BlobServiceException(string.Format("Something wrong happens when retrieving {0}, http status {1}", (object) blobId.BlobId, (object) httpResponseMessage.StatusCode));
        }
        json = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
      }
      return new PreauthenticatedUri(new Uri(JsonSerializer.Deserialize<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob>(json).Url), EdgeType.Unknown);
    }

    public virtual async Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      IEnumerable<BlobIdentifier> blobIds,
      EdgeCache edgeCache,
      CancellationToken cancellationToken,
      DateTimeOffset? expiryTime = null)
    {
      ArgumentUtility.CheckForNull<IEnumerable<BlobIdentifier>>(blobIds, "blobId");
      IEnumerable<BlobIdentifier> source = blobIds.Take<BlobIdentifier>(2);
      switch (source.Count<BlobIdentifier>())
      {
        case 0:
          return (IDictionary<BlobIdentifier, PreauthenticatedUri>) new Dictionary<BlobIdentifier, PreauthenticatedUri>();
        case 1:
          BlobIdentifier blobId = source.First<BlobIdentifier>();
          PreauthenticatedUri preauthenticatedUri = await this.GetDownloadUriAsync(new BlobIdWithHeaders(blobId, edgeCache, expiryTime: expiryTime), cancellationToken).ConfigureAwait(false);
          return (IDictionary<BlobIdentifier, PreauthenticatedUri>) new Dictionary<BlobIdentifier, PreauthenticatedUri>()
          {
            {
              blobId,
              preauthenticatedUri
            }
          };
        default:
          Dictionary<string, string> args = new Dictionary<string, string>();
          if (expiryTime.HasValue)
            args[nameof (expiryTime)] = expiryTime.Value.ToString(KeepUntilBlobReference.KeepUntilFormat, (IFormatProvider) CultureInfo.InvariantCulture);
          if (edgeCache == EdgeCache.Allowed)
            args["allowEdge"] = "true";
          ConcurrentDictionary<BlobIdentifier, PreauthenticatedUri> blobsToUris = new ConcurrentDictionary<BlobIdentifier, PreauthenticatedUri>();
          IEnumerable<List<BlobIdentifier>> pages = blobIds.GetPages<BlobIdentifier>(5000);
          if (this.ParallelHttpCallsSupported)
          {
            Func<IEnumerable<BlobIdentifier>, Task> action = (Func<IEnumerable<BlobIdentifier>, Task>) (pageOfBlobIds => this.GetDownloadUrisPageAsync(blobsToUris, pageOfBlobIds, args, cancellationToken));
            ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
            dataflowBlockOptions.MaxDegreeOfParallelism = BlobStore2HttpClient.MaxParallelGetDownloadUri;
            dataflowBlockOptions.CancellationToken = cancellationToken;
            await NonSwallowingActionBlock.Create<IEnumerable<BlobIdentifier>>(action, dataflowBlockOptions).PostAllToUnboundedAndCompleteAsync<IEnumerable<BlobIdentifier>>((IEnumerable<IEnumerable<BlobIdentifier>>) pages, cancellationToken).ConfigureAwait(false);
          }
          else
          {
            foreach (List<BlobIdentifier> pageOfBlobIds in pages)
              await this.GetDownloadUrisPageAsync(blobsToUris, (IEnumerable<BlobIdentifier>) pageOfBlobIds, args, cancellationToken).ConfigureAwait(false);
          }
          return (IDictionary<BlobIdentifier, PreauthenticatedUri>) blobsToUris;
      }
    }

    private async Task GetDownloadUrisPageAsync(
      ConcurrentDictionary<BlobIdentifier, PreauthenticatedUri> blobsToUris,
      IEnumerable<BlobIdentifier> pageOfBlobIds,
      Dictionary<string, string> args,
      CancellationToken cancellationToken)
    {
      BlobStore2HttpClient store2HttpClient1 = this;
      BlobStore2HttpClient store2HttpClient2 = store2HttpClient1;
      HttpMethod post = HttpMethod.Post;
      Guid blobBatchResourceId = ResourceIds.BlobBatchResourceId;
      ApiResourceVersion version = new ApiResourceVersion(store2HttpClient1.TargetVersion);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) args;
      HttpContent content = JsonSerializer.SerializeToContent<BlobBatch>(new BlobBatch(pageOfBlobIds));
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      foreach (Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob blob in JsonSerializer.Deserialize<BlobBatch>(await (await store2HttpClient2.SendAsync(post, blobBatchResourceId, version: version, content: content, queryParameters: queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false)).Blobs)
        blobsToUris.TryAdd(BlobIdentifier.Deserialize(blob.Id), new PreauthenticatedUri(new Uri(blob.Url), EdgeType.Unknown));
    }

    public Task RemoveReferencesAsync(
      IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>> referencesGroupedByBlobIds)
    {
      IEnumerable<KeyValuePair<BlobReference, BlobIdentifier>> referenceToBlobMap = referencesGroupedByBlobIds.SelectMany<KeyValuePair<BlobIdentifier, IEnumerable<IdBlobReference>>, KeyValuePair<BlobReference, BlobIdentifier>>((Func<KeyValuePair<BlobIdentifier, IEnumerable<IdBlobReference>>, IEnumerable<KeyValuePair<BlobReference, BlobIdentifier>>>) (kvp => kvp.Value.Select<IdBlobReference, KeyValuePair<BlobReference, BlobIdentifier>>((Func<IdBlobReference, KeyValuePair<BlobReference, BlobIdentifier>>) (refId => new KeyValuePair<BlobReference, BlobIdentifier>(new BlobReference(refId), kvp.Key)))));
      HttpMethod delete = HttpMethod.Delete;
      Guid referenceBatchResourceId = ResourceIds.ReferenceBatchResourceId;
      ApiResourceVersion apiResourceVersion = new ApiResourceVersion(this.TargetVersion);
      var routeValues = new{  };
      ApiResourceVersion version = apiResourceVersion;
      HttpContent content = JsonSerializer.SerializeToContent<ReferenceBatch>(new ReferenceBatch(referenceToBlobMap));
      CancellationToken cancellationToken = new CancellationToken();
      return (Task) this.SendAsync(delete, referenceBatchResourceId, (object) routeValues, version, content, cancellationToken: cancellationToken);
    }

    public Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceAsync(
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> referencesGroupedByBlobIds,
      CancellationToken cancellationToken)
    {
      return this.TryReferenceInternalAsync(new ReferenceBatch(referencesGroupedByBlobIds.SelectMany<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>, KeyValuePair<BlobReference, BlobIdentifier>>((Func<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>, IEnumerable<KeyValuePair<BlobReference, BlobIdentifier>>>) (kvp => kvp.Value.Select<BlobReference, KeyValuePair<BlobReference, BlobIdentifier>>((Func<BlobReference, KeyValuePair<BlobReference, BlobIdentifier>>) (refId => new KeyValuePair<BlobReference, BlobIdentifier>(refId, kvp.Key)))))), cancellationToken);
    }

    public Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceWithBlocksAsync(
      IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> referencesGroupedByBlobIds,
      CancellationToken cancellationToken)
    {
      return this.TryReferenceInternalAsync(new ReferenceBatch(referencesGroupedByBlobIds.SelectMany<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>, KeyValuePair<BlobReference, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>>((Func<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>, IEnumerable<KeyValuePair<BlobReference, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>>>) (kvp => kvp.Value.Select<BlobReference, KeyValuePair<BlobReference, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>>((Func<BlobReference, KeyValuePair<BlobReference, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>>) (refId => new KeyValuePair<BlobReference, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>(refId, kvp.Key)))))), cancellationToken);
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) BlobExceptionMapping.ClientTranslatedExceptions;

    public async Task<Stream> GetBlobAsync(
      BlobIdentifier blobId,
      CancellationToken cancellationToken)
    {
      BlobStore2HttpClient store2HttpClient = this;
      ArgumentUtility.CheckForNull<BlobIdentifier>(blobId, nameof (blobId));
      if (blobId.IsOfNothing())
        return (Stream) new MemoryStream(0);
      HttpResponseMessage responseMessage = (HttpResponseMessage) null;
      try
      {
        responseMessage = await store2HttpClient.SendAsync(HttpMethod.Get, ResourceIds.BlobResourceId, HttpCompletionOption.ResponseHeadersRead, (object) new
        {
          blobId = blobId.ValueString
        }, new ApiResourceVersion(store2HttpClient.TargetVersion), cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      catch (BlobNotFoundException ex)
      {
        responseMessage?.Dispose();
        return (Stream) null;
      }
      Stream stream = await responseMessage.Content.ReadAsStreamAsync().EnforceCancellation<Stream>(cancellationToken, file: "D:\\a\\_work\\1\\s\\BlobStore\\Client\\WebApi\\BlobStore2HttpClient.cs", member: nameof (GetBlobAsync), line: 331).ConfigureAwait(false);
      if (responseMessage.Content.Headers.ContainsContentEncoding("gzip"))
        stream = (Stream) new GZipStream(stream, CompressionMode.Decompress);
      return new DisposingStreamWrapper(stream, new IDisposable[1]
      {
        (IDisposable) responseMessage
      }).WrapWithCancellationEnforcement(blobId.ValueString);
    }

    public override Guid ResourceId => ResourceIds.BlobResourceId;

    public virtual Task PutBlobBlockAsync(
      BlobIdentifier blobId,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blockHash,
      byte[] blockBuffer,
      int blockLength,
      CancellationToken cancellationToken)
    {
      return this.UploadBlockAsync(blockBuffer, blockLength, blobId, blockHash, false, (BlobReference) null, cancellationToken);
    }

    public virtual Task PutBlobBlockAsync(
      BlobIdentifier blobId,
      byte[] blockBuffer,
      int blockLength,
      CancellationToken cancellationToken)
    {
      return this.PutBlobBlockAsync(blobId, Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.HashBlock(blockBuffer, blockLength), blockBuffer, blockLength, cancellationToken);
    }

    public virtual Task PutSingleBlockBlobAndReferenceAsync(
      BlobIdentifier blobId,
      byte[] blockBuffer,
      int blockLength,
      BlobReference reference,
      CancellationToken cancellationToken)
    {
      return this.UploadBlockAsync(blockBuffer, blockLength, blobId, (Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash) null, true, reference, cancellationToken);
    }

    public async Task<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks> UploadBlocksForBlobAsync(
      BlobIdentifier blobId,
      Stream blobStream,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<Stream>(blobStream, "stream");
      RunOnce<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash> blockUploads = new RunOnce<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>(true);
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks identifierWithBlocks = await Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.WalkAllBlobBlocksAsync(blobStream, BlobStore2HttpClient.BlockUploadSemaphore, true, (MultiBlockBlobCallbackAsync) ((block, blockLength, blockHash, isFinalBlock) => blockUploads.RunOnceAsync(blockHash, (Func<Task>) (() => this.UploadBlockAsync(block, blockLength, blobId, blockHash, false, (BlobReference) null, cancellationToken))))).ConfigureAwait(false);
      if (blobId != identifierWithBlocks.BlobId)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "BlobId '{0}' does not match BlobIdentifierWithBlocks '{1}' '{2}' computed from blobStream.", (object) blobId, (object) identifierWithBlocks, (object) identifierWithBlocks.BlobId));
      return identifierWithBlocks;
    }

    public virtual async Task<IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>> UploadBlocksForBlobsAsync(
      IEnumerable<BlobToUriMapping> pathToUriMappings,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      ConcurrentBag<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks> blobIdsWithBlocks = new ConcurrentBag<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>();
      Func<BlobToUriMapping, Task> action = (Func<BlobToUriMapping, Task>) (async pathToUriMapping =>
      {
        Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks identifierWithBlocks = await this.UploadBlocksForSingleBlobTaskAsync(pathToUriMapping, cancellationToken).ConfigureAwait(false);
        if (!(identifierWithBlocks != (Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks) null))
          return;
        blobIdsWithBlocks.Add(identifierWithBlocks);
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.MaxDegreeOfParallelism = BlobStore2HttpClient.MaxParallelFileUpload;
      dataflowBlockOptions.CancellationToken = cancellationToken;
      await NonSwallowingActionBlock.Create<BlobToUriMapping>(action, dataflowBlockOptions).PostAllToUnboundedAndCompleteAsync<BlobToUriMapping>(pathToUriMappings, cancellationToken).ConfigureAwait(false);
      return (IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>) blobIdsWithBlocks;
    }

    protected virtual async Task UploadBlockAsync(
      byte[] block,
      int blockLength,
      BlobIdentifier blobId,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blockHash,
      bool isOnlyBlock,
      BlobReference reference,
      CancellationToken cancellationToken)
    {
      BlobStore2HttpClient store2HttpClient = this;
      await AsyncHttpRetryHelper.InvokeVoidAsync((Func<Task>) (() =>
      {
        HttpContent httpContent = (HttpContent) new ByteArrayContent(block, 0, blockLength);
        httpContent.Headers.ContentLength = new long?((long) blockLength);
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        httpContent.Headers.ContentRange = new ContentRangeHeaderValue((long) blockLength);
        Dictionary<string, string> queryParameters = new Dictionary<string, string>();
        if (!isOnlyBlock)
          queryParameters.Add(nameof (blockHash), blockHash.HashString);
        if (reference != (BlobReference) null)
          reference.Match((Action<IdBlobReference>) (idReference =>
          {
            if (idReference.Scope != null)
              queryParameters.Add("referenceScope", idReference.Scope);
            queryParameters.Add("referenceId", idReference.Name);
          }), (Action<KeepUntilBlobReference>) (keepUntilReference => queryParameters.Add("keepUntil", keepUntilReference.KeepUntilString)));
        HttpMethod method = isOnlyBlock ? HttpMethod.Put : HttpMethod.Post;
        Guid blobResourceId = ResourceIds.BlobResourceId;
        var routeValues = new{ blobId = blobId.ValueString };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) queryParameters;
        ApiResourceVersion version = new ApiResourceVersion(this.TargetVersion);
        HttpContent content = httpContent;
        IEnumerable<KeyValuePair<string, string>> queryParameters1 = keyValuePairs;
        CancellationToken cancellationToken1 = cancellationToken;
        return (Task) this.SendAsync(method, blobResourceId, (object) routeValues, version, content, queryParameters1, cancellationToken: cancellationToken1);
      }), int.Parse(Environment.GetEnvironmentVariable("BLOBSTORE_PUTBLOCK_RETRY_COUNT") ?? "3"), store2HttpClient.tracer, cancellationToken, false, "UploadBlockAsync " + blobId.ValueString + " " + (blockHash?.HashString ?? "[single block blob]")).ConfigureAwait(false);
    }

    protected async Task<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks> UploadBlocksForSingleBlobTaskAsync(
      BlobToUriMapping mapping,
      CancellationToken cancellationToken)
    {
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks identifierWithBlocks;
      try
      {
        using (Stream stream = mapping.StreamFactory.Value)
        {
          stream.Position = 0L;
          identifierWithBlocks = await this.UploadBlocksForBlobAsync(mapping.BlobId, stream, cancellationToken).ConfigureAwait(false);
        }
      }
      catch (Exception ex)
      {
        ex.Data[(object) "ContentSpec"] = (object) mapping.ContentSpec;
        ex.ReThrow();
        throw new InvalidOperationException("unreachable.");
      }
      return identifierWithBlocks;
    }

    private async Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceInternalAsync(
      ReferenceBatch batch,
      CancellationToken cancellationToken)
    {
      BlobStore2HttpClient store2HttpClient = this;
      return (IDictionary<BlobIdentifier, IEnumerable<BlobReference>>) JsonSerializer.Deserialize<ReferenceBatch>(await (await store2HttpClient.SendAsync(HttpMethod.Post, ResourceIds.ReferenceBatchResourceId, version: new ApiResourceVersion(store2HttpClient.TargetVersion), content: JsonSerializer.SerializeToContent<ReferenceBatch>(batch), cancellationToken: cancellationToken).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false)).References.Where<Reference>((Func<Reference, bool>) (r => r.Status.Equals("Missing", StringComparison.OrdinalIgnoreCase))).GroupBy<Reference, Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob>((Func<Reference, Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob>) (r => r.Blob)).ToDictionary<IGrouping<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob, Reference>, BlobIdentifier, IEnumerable<BlobReference>>((Func<IGrouping<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob, Reference>, BlobIdentifier>) (k => k.Key.ToBlobIdentifier()), (Func<IGrouping<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob, Reference>, IEnumerable<BlobReference>>) (v => v.Select<Reference, BlobReference>((Func<Reference, BlobReference>) (i => i.BlobReference))));
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    private struct Void
    {
    }

    public struct BlobUrisResponse
    {
      [DataMember]
      public List<BlobIdentifier> BlobIds;
      [DataMember]
      public List<Uri> Uris;
    }
  }
}
