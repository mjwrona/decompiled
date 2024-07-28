// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupPhysicalStorageData
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [Serializable]
  public sealed class DedupPhysicalStorageData
  {
    public LogHistogram sizeHistogram = new LogHistogram(2.0, 1.8446744073709552E+19);
    public LogHistogram ageLastModifiedHistograph = new LogHistogram(2.0, 365000.0);

    public ulong TotalBlobs { get; set; }

    public ulong TotalBytes { get; set; }

    public long NumRetried { get; set; }

    public void AddBlockBlobMetadata(BasicBlobMetadata blockBlob, IClock clock)
    {
      ++this.TotalBlobs;
      this.TotalBytes += blockBlob.Size;
      this.sizeHistogram.IncrementCount((double) blockBlob.Size);
      DateTimeOffset? lastModified = blockBlob.LastModified;
      if (!lastModified.HasValue)
        return;
      this.ageLastModifiedHistograph.IncrementCount((double) (long) Math.Ceiling(clock.Now.Subtract(lastModified.Value).TotalDays));
    }

    public void AccumulateDedupStorageData(
      IEnumerable<DedupPhysicalStorageData> dedupStorageData)
    {
      foreach (DedupPhysicalStorageData physicalStorageData in dedupStorageData)
      {
        this.TotalBlobs += physicalStorageData.TotalBlobs;
        this.TotalBytes += physicalStorageData.TotalBytes;
        this.NumRetried += physicalStorageData.NumRetried;
        this.sizeHistogram.MergeFrom((Histogram) physicalStorageData.sizeHistogram);
        this.ageLastModifiedHistograph.MergeFrom((Histogram) physicalStorageData.ageLastModifiedHistograph);
      }
    }

    public void AggregateDedupStorageData(
      IEnumerable<DedupPhysicalStorageData> dedupPhysicalStorageData,
      int totalPartitions,
      bool extrapolate = false)
    {
      this.TotalBlobs = extrapolate ? (ulong) dedupPhysicalStorageData.Sum<DedupPhysicalStorageData>((Func<DedupPhysicalStorageData, double>) (r => (double) r.TotalBlobs)) : (ulong) (dedupPhysicalStorageData.Average<DedupPhysicalStorageData>((Func<DedupPhysicalStorageData, double>) (r => (double) r.TotalBlobs)) * (double) totalPartitions);
      this.TotalBytes = extrapolate ? (ulong) dedupPhysicalStorageData.Sum<DedupPhysicalStorageData>((Func<DedupPhysicalStorageData, double>) (r => (double) r.TotalBytes)) : (ulong) (dedupPhysicalStorageData.Average<DedupPhysicalStorageData>((Func<DedupPhysicalStorageData, double>) (r => (double) r.TotalBytes)) * (double) totalPartitions);
      this.NumRetried = (long) (int) (dedupPhysicalStorageData.Average<DedupPhysicalStorageData>((Func<DedupPhysicalStorageData, long>) (r => r.NumRetried)) * (double) totalPartitions);
      foreach (DedupPhysicalStorageData physicalStorageData in dedupPhysicalStorageData)
      {
        this.sizeHistogram.MergeFrom((Histogram) physicalStorageData.sizeHistogram);
        this.ageLastModifiedHistograph.MergeFrom((Histogram) physicalStorageData.ageLastModifiedHistograph);
      }
    }
  }
}
