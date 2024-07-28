// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupStitcher.DedupContentDownloader
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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupStitcher
{
  public class DedupContentDownloader : IDedupContentProvider
  {
    private readonly HttpClient httpClient;
    private readonly IVssRequestContext requestContext;

    public DedupContentDownloader(IVssRequestContext requestContext, HttpClient httpClient)
    {
      this.httpClient = httpClient;
      this.requestContext = requestContext;
    }

    public async Task<DedupCompressedBuffer> GetContentAsync(DedupDownloadInfoBase info)
    {
      HttpResponseMessage responseMessage = await AsyncHttpRetryHelper.InvokeAsync<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (async () =>
      {
        HttpResponseMessage contentAsync = await this.httpClient.GetAsync(info.Url, this.requestContext.CancellationToken).ConfigureAwait(false);
        if (contentAsync.StatusCode == HttpStatusCode.ServiceUnavailable)
          throw new AsyncHttpRetryHelper.RetryableException("HTTP 503 throttling.");
        contentAsync.EnsureSuccessStatusCode();
        return contentAsync;
      }), 5, (IAppTraceSource) NoopAppTraceSource.Instance, this.requestContext.CancellationToken, false, info.Url.GetLeftPart(UriPartial.Path)).ConfigureAwait(false);
      int rawLength = (int) responseMessage.Content.Headers.ContentLength.Value;
      IPoolHandle<byte[]> chunkBuffer = ChunkerHelper.BorrowChunkBuffer(rawLength);
      await (await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false)).ReadToEntireBufferAsync(new ArraySegment<byte>(chunkBuffer.Value, 0, rawLength), this.requestContext.CancellationToken).ConfigureAwait(false);
      DedupCompressedBuffer contentAsync1 = !responseMessage.Content.Headers.HasXpressContentEncoding() ? DedupCompressedBuffer.FromUncompressed(chunkBuffer, 0, rawLength) : DedupCompressedBuffer.FromCompressed(chunkBuffer, 0, rawLength);
      responseMessage = (HttpResponseMessage) null;
      chunkBuffer = (IPoolHandle<byte[]>) null;
      return contentAsync1;
    }

    public Task<DedupDownloadInfo> GetDownloadInfoAsync(IDomainId domainId, DedupIdentifier dedupId) => this.requestContext.GetService<IDedupStore>().GetDownloadInfoAsync(this.requestContext, domainId, dedupId, true);
  }
}
