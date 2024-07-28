// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupStitcher.DedupContentStreamFactory
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E75C933D-C085-4E42-931C-50E8D8D54917
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupStitcher
{
  public class DedupContentStreamFactory
  {
    public const int DefaultBoundedCapacity = 20;
    public const int DefaultParallelism = 5;
    private readonly IDedupContentProvider provider;
    private readonly int boundedCapacity;
    private readonly int parallelism;

    public DedupContentStreamFactory(
      IDedupContentProvider provider,
      int boundedCapacity = 20,
      int parallelism = 5)
    {
      this.provider = provider;
      this.boundedCapacity = boundedCapacity;
      this.parallelism = parallelism;
    }

    public async Task<Stream> GetStreamAsync(
      IDomainId domainId,
      DedupIdentifier dedupId,
      CancellationToken cancellationToken)
    {
      DedupDownloadInfo downloadInfoAsync = await this.provider.GetDownloadInfoAsync(domainId, dedupId);
      DedupDownloadInfoBase[] downloadInfoBaseArray = downloadInfoAsync != null ? (DedupDownloadInfoBase[]) downloadInfoAsync.Chunks : throw new DedupNotFoundException(string.Format("The requested ID {0} does not exist.", (object) dedupId));
      if (downloadInfoBaseArray == null)
        downloadInfoBaseArray = new DedupDownloadInfoBase[1]
        {
          (DedupDownloadInfoBase) downloadInfoAsync
        };
      IEnumerable<DedupDownloadInfoBase> chunks = (IEnumerable<DedupDownloadInfoBase>) downloadInfoBaseArray;
      return (Stream) this.CreateNodeStream(downloadInfoAsync.Size, chunks, cancellationToken);
    }

    public NodeStream CreateNodeStream(
      long totalSize,
      IEnumerable<DedupDownloadInfoBase> chunks,
      CancellationToken cancellationToken)
    {
      ConcurrentIterator<DedupCompressedBuffer> producer = new ConcurrentIterator<DedupCompressedBuffer>(new int?(this.boundedCapacity), cancellationToken, (Func<TryAddValueAsyncFunc<DedupCompressedBuffer>, CancellationToken, Task>) (async (sendChunkAsync, producerCancellationToken) => await this.DownloadAndSendChunks(chunks, sendChunkAsync, producerCancellationToken)));
      return new NodeStream(totalSize, (IConcurrentIterator<DedupCompressedBuffer>) producer, cancellationToken);
    }

    private async Task DownloadAndSendChunks(
      IEnumerable<DedupDownloadInfoBase> chunks,
      TryAddValueAsyncFunc<DedupCompressedBuffer> sendChunkAsync,
      CancellationToken cancelToken)
    {
      Func<DedupDownloadInfoBase, Task<DedupCompressedBuffer>> transform = (Func<DedupDownloadInfoBase, Task<DedupCompressedBuffer>>) (async chunk => await this.provider.GetContentAsync(chunk).ConfigureAwait(false));
      ExecutionDataflowBlockOptions dataflowBlockOptions1 = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions1.MaxDegreeOfParallelism = this.parallelism;
      dataflowBlockOptions1.BoundedCapacity = this.boundedCapacity;
      dataflowBlockOptions1.CancellationToken = cancelToken;
      dataflowBlockOptions1.EnsureOrdered = true;
      TransformBlock<DedupDownloadInfoBase, DedupCompressedBuffer> targetBlock = NonSwallowingTransformBlock.Create<DedupDownloadInfoBase, DedupCompressedBuffer>(transform, dataflowBlockOptions1);
      int num;
      Func<DedupCompressedBuffer, Task> action = (Func<DedupCompressedBuffer, Task>) (async buffer => num = await sendChunkAsync(buffer).ConfigureAwait(false) ? 1 : 0);
      ExecutionDataflowBlockOptions dataflowBlockOptions2 = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions2.MaxDegreeOfParallelism = 1;
      dataflowBlockOptions2.BoundedCapacity = this.boundedCapacity;
      dataflowBlockOptions2.CancellationToken = cancelToken;
      dataflowBlockOptions2.EnsureOrdered = true;
      ActionBlock<DedupCompressedBuffer> actionBlock = NonSwallowingActionBlock.Create<DedupCompressedBuffer>(action, dataflowBlockOptions2);
      targetBlock.LinkTo((ITargetBlock<DedupCompressedBuffer>) actionBlock, new DataflowLinkOptions()
      {
        PropagateCompletion = true
      });
      await targetBlock.SendAllAndCompleteAsync<DedupDownloadInfoBase, DedupCompressedBuffer>(chunks, (ITargetBlock<DedupCompressedBuffer>) actionBlock, cancelToken).ConfigureAwait(false);
    }
  }
}
