// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.IDedupStoreClientWithDataport
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.DataDeduplication.Interop;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [CLSCompliant(false)]
  public interface IDedupStoreClientWithDataport : IDedupStoreClient
  {
    int MaxParallelismCount { get; }

    Task<ChunkedFileDownloadResult> DownloadToFileAsync(
      IDedupDataPort dataport,
      DedupIdentifier dedupId,
      string fullPath,
      ulong fileSize,
      GetDedupAsyncFunc dedupFetcher,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken);

    Task<DedupNode> DownloadToFileAsync(
      IDedupDataPort dataport,
      DedupNode node,
      string fullPath,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken);

    Task EnsureChunksAreLocalAsync(
      IDedupDataPort dataPort,
      IEnumerable<DedupNode> chunks,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken);
  }
}
