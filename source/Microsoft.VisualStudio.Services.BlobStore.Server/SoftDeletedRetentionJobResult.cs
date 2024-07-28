// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.SoftDeletedRetentionJobResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [Serializable]
  public class SoftDeletedRetentionJobResult : ISoftDeleteRetentionResult
  {
    public long TotalFileDedupSoftDeletedBlobs { get; set; }

    public long TotalFileDedupSoftDeletedBytes { get; set; }

    public long TotalChunkDedupSoftDeletedBlobs { get; set; }

    public long TotalChunkDedupSoftDeletedBytes { get; set; }

    public int TotalShards { get; set; }

    public int TotalShardsScanned { get; set; }

    public ConcurrentBag<string> ShardNames { get; set; } = new ConcurrentBag<string>();

    public string ErrorDetails { get; set; }

    public TimeSpan TotalThrottleDuration { get; set; }

    public LogHistogram FileSoftDeletedExpirationInDays { get; private set; } = new LogHistogram(2.0, 365.0);

    public LogHistogram ChunkSoftDeletedExpirationInDays { get; private set; } = new LogHistogram(2.0, 365.0);
  }
}
