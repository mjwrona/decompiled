// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common.BlobStitcher
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E75C933D-C085-4E42-931C-50E8D8D54917
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common
{
  public static class BlobStitcher
  {
    public static NodeStream GetTransitiveContent(
      long totalLength,
      IEnumerable<Uri> uris,
      HttpClient httpClient,
      IAppTraceSource tracer,
      CancellationToken cancellationToken)
    {
      ConcurrentIterator<DedupCompressedBuffer> producer = new ConcurrentIterator<DedupCompressedBuffer>(new int?(5), cancellationToken, (Func<TryAddValueAsyncFunc<DedupCompressedBuffer>, CancellationToken, Task>) (async (valueAdderAsync, cancelToken) =>
      {
        using (CancellationTokenSource cts = new CancellationTokenSource())
        {
          using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancelToken))
          {
            Func<Uri, Task<DedupCompressedBuffer>> transform = (Func<Uri, Task<DedupCompressedBuffer>>) (async uri =>
            {
              HttpResponseMessage responseMessage = await AsyncHttpRetryHelper.InvokeAsync<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (async () =>
              {
                HttpResponseMessage transitiveContent = await httpClient.GetAsync(uri, cancelToken).ConfigureAwait(false);
                if (transitiveContent.StatusCode == HttpStatusCode.ServiceUnavailable)
                  throw new AsyncHttpRetryHelper.RetryableException("HTTP 503 throttling.");
                transitiveContent.EnsureSuccessStatusCode();
                return transitiveContent;
              }), 5, tracer, cancelToken, false, uri.AbsoluteUri).ConfigureAwait(false);
              int rawLength = (int) responseMessage.Content.Headers.ContentLength.Value;
              IPoolHandle<byte[]> chunkBuffer = ChunkerHelper.BorrowChunkBuffer(rawLength);
              await (await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false)).ReadToEntireBufferAsync(new ArraySegment<byte>(chunkBuffer.Value, 0, rawLength), cancelToken).ConfigureAwait(false);
              DedupCompressedBuffer transitiveContent = !responseMessage.Content.Headers.HasXpressContentEncoding() ? DedupCompressedBuffer.FromUncompressed(chunkBuffer, 0, rawLength) : DedupCompressedBuffer.FromCompressed(chunkBuffer, 0, rawLength);
              responseMessage = (HttpResponseMessage) null;
              chunkBuffer = (IPoolHandle<byte[]>) null;
              return transitiveContent;
            });
            TransformBlock<Uri, DedupCompressedBuffer> targetBlock = NonSwallowingTransformBlock.Create<Uri, DedupCompressedBuffer>(transform, new ExecutionDataflowBlockOptions()
            {
              MaxDegreeOfParallelism = 5,
              BoundedCapacity = 10,
              CancellationToken = linkedCts.Token,
              EnsureOrdered = true
            });
            Func<DedupCompressedBuffer, Task> action = (Func<DedupCompressedBuffer, Task>) (async buffer =>
            {
              if (await valueAdderAsync(buffer).ConfigureAwait(false))
                return;
              cts.Cancel();
            });
            ActionBlock<DedupCompressedBuffer> actionBlock = NonSwallowingActionBlock.Create<DedupCompressedBuffer>(action, new ExecutionDataflowBlockOptions()
            {
              MaxDegreeOfParallelism = 1,
              BoundedCapacity = 5,
              CancellationToken = linkedCts.Token,
              EnsureOrdered = true
            });
            targetBlock.LinkTo((ITargetBlock<DedupCompressedBuffer>) actionBlock, new DataflowLinkOptions()
            {
              PropagateCompletion = true
            });
            await targetBlock.SendAllAndCompleteAsync<Uri, DedupCompressedBuffer>(uris, (ITargetBlock<DedupCompressedBuffer>) actionBlock, linkedCts.Token).ConfigureAwait(false);
          }
        }
      }));
      return new NodeStream(totalLength, (IConcurrentIterator<DedupCompressedBuffer>) producer, cancellationToken);
    }
  }
}
