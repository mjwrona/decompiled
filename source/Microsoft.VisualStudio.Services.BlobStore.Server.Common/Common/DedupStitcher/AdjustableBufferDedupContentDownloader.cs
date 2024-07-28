// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupStitcher.AdjustableBufferDedupContentDownloader
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupStitcher
{
  public class AdjustableBufferDedupContentDownloader : IDedupContentProvider
  {
    public const int MinimumSizeToUseBufferPool = 80000;
    private readonly HttpClient httpClient;
    private readonly IVssRequestContext requestContext;
    private readonly bool onlyUseBufferPoolAboveThreshold;
    private static readonly Microsoft.VisualStudio.Services.BlobStore.Common.ByteArrayPool chunkBufferPool = new Microsoft.VisualStudio.Services.BlobStore.Common.ByteArrayPool(ChunkerHelper.GetMaxNodeContentSize(), 100);

    public static void ResizeBufferPool(int maxToKeep) => AdjustableBufferDedupContentDownloader.chunkBufferPool.Resize(maxToKeep);

    public AdjustableBufferDedupContentDownloader(
      IVssRequestContext requestContext,
      HttpClient httpClient,
      bool onlyUseBufferPoolAboveThreshold)
    {
      this.httpClient = httpClient;
      this.onlyUseBufferPoolAboveThreshold = onlyUseBufferPoolAboveThreshold;
      this.requestContext = requestContext;
    }

    public async Task<DedupCompressedBuffer> GetContentAsync(DedupDownloadInfoBase info)
    {
      HttpResponseMessage httpResponseMessage = await AsyncHttpRetryHelper.InvokeAsync<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (async () =>
      {
        HttpResponseMessage contentAsync = await this.httpClient.GetAsync(info.Url, this.requestContext.CancellationToken).ConfigureAwait(false);
        if (contentAsync.StatusCode == HttpStatusCode.ServiceUnavailable)
          throw new AsyncHttpRetryHelper.RetryableException("HTTP 503 throttling.");
        contentAsync.EnsureSuccessStatusCode();
        return contentAsync;
      }), 5, (IAppTraceSource) NoopAppTraceSource.Instance, this.requestContext.CancellationToken, false, info.Url.GetLeftPart(UriPartial.Path)).ConfigureAwait(false);
      int rawLength = (int) httpResponseMessage.Content.Headers.ContentLength.Value;
      bool isCompressed = httpResponseMessage.Content.Headers.HasXpressContentEncoding();
      Stream content = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
      if (this.onlyUseBufferPoolAboveThreshold && rawLength < 80000)
      {
        byte[] chunkBuffer = new byte[rawLength];
        await content.ReadToEntireBufferAsync(new ArraySegment<byte>(chunkBuffer, 0, rawLength), this.requestContext.CancellationToken).ConfigureAwait(false);
        return !isCompressed ? DedupCompressedBuffer.FromUncompressed(chunkBuffer) : DedupCompressedBuffer.FromCompressed(chunkBuffer);
      }
      IPoolHandle<byte[]> chunkBuffer1 = ChunkerHelper.BorrowChunkBuffer(rawLength);
      await content.ReadToEntireBufferAsync(new ArraySegment<byte>(chunkBuffer1.Value, 0, rawLength), this.requestContext.CancellationToken).ConfigureAwait(false);
      return !isCompressed ? DedupCompressedBuffer.FromUncompressed(chunkBuffer1, 0, rawLength) : DedupCompressedBuffer.FromCompressed(chunkBuffer1, 0, rawLength);
    }

    public Task<DedupDownloadInfo> GetDownloadInfoAsync(IDomainId domainId, DedupIdentifier dedupId) => this.requestContext.GetService<IDedupStore>().GetDownloadInfoAsync(this.requestContext, domainId, dedupId, true);
  }
}
