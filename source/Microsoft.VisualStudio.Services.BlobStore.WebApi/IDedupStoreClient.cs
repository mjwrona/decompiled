// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.IDedupStoreClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Common.Telemetry;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [CLSCompliant(false)]
  public interface IDedupStoreClient
  {
    IDedupStoreHttpClient Client { get; }

    DedupDownloadStatistics DownloadStatistics { get; }

    HashType HashType { get; }

    Task<IDisposable> AcquireParallelismTokenAsync();

    IDedupUploadSession CreateUploadSession(
      KeepUntilBlobReference keepUntilReference,
      IAppTraceSource tracer,
      IFileSystem fileSystem);

    Task<ChunkedFileDownloadResult> DownloadToFileAsync(
      DedupIdentifier dedupId,
      string fullPath,
      GetDedupAsyncFunc dedupFetcher,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken);

    Task DownloadToStreamAsync(
      DedupIdentifier dedupId,
      Stream stream,
      Uri proxyUri,
      EdgeCache edgeCache,
      Action<ulong> traceDownloadProgressFunc,
      Action<ulong> traceFinalizeDownloadProgressFunc,
      CancellationToken cancellationToken);

    Task<DedupNode> DownloadToFileAsync(
      DedupNode node,
      string fullPath,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken);

    Task<MaybeCached<DedupCompressedBuffer>> GetChunkAsync(
      ChunkDedupIdentifier chunkId,
      CancellationToken cancellationToken);

    Task<MaybeCached<DedupCompressedBuffer>> GetDedupAsync(
      DedupIdentifier dedupId,
      CancellationToken cancellationToken);

    Task<DedupNode> GetFilledNodesAsync(
      DedupNode node,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken);

    Task<MaybeCached<DedupCompressedBuffer>> GetNodeAsync(
      NodeDedupIdentifier nodeId,
      CancellationToken cancellationToken);

    Task<Dictionary<DedupIdentifier, GetDedupAsyncFunc>> GetDedupGettersAsync(
      ISet<DedupIdentifier> dedupIds,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken);

    void ResetDownloadStatistics();
  }
}
