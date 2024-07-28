// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DeleteDanglingPipelineArtifactRootsJobResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [Serializable]
  public class DeleteDanglingPipelineArtifactRootsJobResult
  {
    private long totalRootsDeletedSize;
    private long totalRootsDeleted;

    public TimeSpan TotalThrottleDuration { get; set; }

    public ulong TotalRootsDiscovered { get; set; }

    public ulong TotalRootsEvaluated { get; set; }

    public long TotalRootsDeleted
    {
      get => this.totalRootsDeleted;
      set => this.totalRootsDeleted = value;
    }

    public long TotalRootsDeletedSize
    {
      get => this.totalRootsDeletedSize;
      set => this.totalRootsDeletedSize = value;
    }

    public long TotalRootsFailedToDelete { get; set; }

    public LogHistogram DeletionSizeLog2Histogram { get; set; } = new LogHistogram(2.0, Math.Pow(2.0, 64.0));

    public void LogDeletionDetails(ulong rootSize)
    {
      Interlocked.Increment(ref this.totalRootsDeleted);
      Interlocked.Add(ref this.totalRootsDeletedSize, (long) rootSize);
      this.DeletionSizeLog2Histogram.AddToCounts((double) Math.Max(rootSize, 0UL), 1L);
    }
  }
}
