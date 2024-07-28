// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.PhysicalDataJobResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [JsonObject]
  [Serializable]
  public sealed class PhysicalDataJobResult
  {
    private LogHistogram sizeHistogram = new LogHistogram(2.0, 1.8446744073709552E+19);
    private LogHistogram ageLastModifiedHistograph = new LogHistogram(2.0, 365000.0);

    [JsonProperty]
    public ulong TotalBlobs { get; set; }

    [JsonProperty]
    public ulong TotalBytes { get; set; }

    [JsonProperty]
    public TimeSpan ThrottledTime { get; set; }

    [JsonProperty]
    public int NumRetried { get; set; }

    [JsonProperty]
    public long[] SizeHistograph => this.sizeHistogram.GetCounts().Values.ToArray<long>();

    [JsonProperty]
    public long[] AgeLastModifiedHistograph => this.ageLastModifiedHistograph.GetCounts().Values.ToArray<long>();

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

    public void AddStorageDataJobResult(IEnumerable<PhysicalDataJobResult> results)
    {
      foreach (PhysicalDataJobResult result in results)
      {
        this.TotalBlobs += result.TotalBlobs;
        this.TotalBytes += result.TotalBytes;
        this.ThrottledTime += result.ThrottledTime;
        this.NumRetried += result.NumRetried;
        this.sizeHistogram.MergeFrom((Histogram) result.sizeHistogram);
        this.ageLastModifiedHistograph.MergeFrom((Histogram) result.ageLastModifiedHistograph);
      }
    }
  }
}
