// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.FileStorageSizeJobResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [Serializable]
  public sealed class FileStorageSizeJobResult
  {
    public ConcurrentDictionary<ArtifactScopeType, long> LogicalSizeByScope = new ConcurrentDictionary<ArtifactScopeType, long>();
    public ConcurrentDictionary<ArtifactScopeType, long> PhysicalSizeByScope = new ConcurrentDictionary<ArtifactScopeType, long>();
    [IgnoreDataMember]
    public ConcurrentDictionary<string, ulong> LogicalSizeByFeed = new ConcurrentDictionary<string, ulong>();
    public LogHistogram ageSizeHistogram = new LogHistogram(2.0, 3650.0);
    public LogHistogram ageLogicalSizeHistogram = new LogHistogram(2.0, 3650.0);
    public LogHistogram idRefCountHistogram = new LogHistogram(2.0, 2000000.0);
    public static readonly TimeSpan KeepUntilDurationToCountAsSeparateReference = TimeSpan.FromDays(7.0);

    public long TotalBlobs { get; set; }

    public long TotalExpiredBlobs { get; set; }

    public long TotalSingleRefBlobs { get; set; }

    public long TotalBlobsWithoutRef { get; set; }

    public long TotalBlobsWithoutSize { get; set; }

    public long TotalBlobsNotInAddedState { get; set; }

    public long TotalSizeUpdates { get; set; }

    public long TotalSizeUpdateFailures { get; set; }

    public ulong TotalBytes { get; set; }

    public ulong TotalLogicalBytes { get; set; }

    public ulong TotalExpiredBytes { get; set; }

    public ulong TotalSingleRefBytes { get; set; }

    public ulong TotalKeepUntilReferenceBytes { get; set; }

    public ulong TotalBytesWithoutRef { get; set; }

    public TimeSpan TotalThrottleDuration { get; set; }

    public int NumJobRetried { get; set; }

    public void AddSizeInfo(IBlobMetadataSizeInfo blobInfo)
    {
      if (blobInfo.StoredReferenceState != BlobReferenceState.AddedBlob)
      {
        ++this.TotalBlobsNotInAddedState;
      }
      else
      {
        ++this.TotalBlobs;
        long idReferenceCount = blobInfo.IdReferenceCount;
        bool flag = false;
        DateTimeOffset? nullable = blobInfo.KeepUntilTime;
        if (nullable.HasValue)
        {
          nullable = blobInfo.KeepUntilTime;
          DateTimeOffset dateTimeOffset = nullable.Value;
          if (dateTimeOffset > DateTimeOffset.UtcNow && idReferenceCount == 0L || dateTimeOffset > DateTimeOffset.UtcNow.Add(FileStorageSizeJobResult.KeepUntilDurationToCountAsSeparateReference))
          {
            flag = true;
            ++idReferenceCount;
          }
        }
        if (blobInfo.BlobLength.HasValue && blobInfo.BlobLength.Value > 0L)
        {
          long blobLength = blobInfo.BlobLength.Value;
          this.TotalBytes += (ulong) blobLength;
          this.TotalLogicalBytes += (ulong) (blobLength * idReferenceCount);
          this.TotalExpiredBytes += idReferenceCount == 0L ? (ulong) blobLength : 0UL;
          this.TotalSingleRefBytes += blobInfo.IdReferenceCount == 1L ? (ulong) blobLength : 0UL;
          this.TotalKeepUntilReferenceBytes += flag ? (ulong) blobLength : 0UL;
          blobInfo.IdReferenceCountByScope.ForEach<KeyValuePair<ArtifactScopeType, long>>((Action<KeyValuePair<ArtifactScopeType, long>>) (kvp =>
          {
            long logicalSize = blobLength * kvp.Value;
            this.LogicalSizeByScope.AddOrUpdate(kvp.Key, logicalSize, (Func<ArtifactScopeType, long, long>) ((key, cur) => cur + logicalSize));
          }));
          if (flag)
            this.LogicalSizeByScope.AddOrUpdate(ArtifactScopeType.Others, blobLength, (Func<ArtifactScopeType, long, long>) ((key, cur) => cur + blobLength));
          if (idReferenceCount == 0L)
          {
            nullable = blobInfo.KeepUntilTime;
            if (!nullable.HasValue)
            {
              ++this.TotalBlobsWithoutRef;
              this.TotalBytesWithoutRef += (ulong) blobLength;
            }
          }
          List<ArtifactScopeType> list = blobInfo.IdReferenceCountByScope.Select<KeyValuePair<ArtifactScopeType, long>, ArtifactScopeType>((Func<KeyValuePair<ArtifactScopeType, long>, ArtifactScopeType>) (kvp => kvp.Key)).ToList<ArtifactScopeType>();
          if (flag && !list.Contains(ArtifactScopeType.Others))
            list.Add(ArtifactScopeType.Others);
          if (list.Count > 0)
          {
            long perScopePhisicalSize = blobLength / (long) list.Count;
            long remainder = blobLength % (long) list.Count;
            foreach (ArtifactScopeType key1 in list)
              this.PhysicalSizeByScope.AddOrUpdate(key1, perScopePhisicalSize, (Func<ArtifactScopeType, long, long>) ((key, cur) => cur + perScopePhisicalSize));
            this.PhysicalSizeByScope.AddOrUpdate(list[0], remainder, (Func<ArtifactScopeType, long, long>) ((key, cur) => cur + remainder));
          }
          blobInfo.IdReferenceCountByFeed.ForEach<KeyValuePair<string, ulong>>((Action<KeyValuePair<string, ulong>>) (kvp =>
          {
            ulong logicalSize = (ulong) blobLength * kvp.Value;
            long num = (long) this.LogicalSizeByFeed.AddOrUpdate(kvp.Key, logicalSize, (Func<string, ulong, ulong>) ((key, cur) => cur + logicalSize));
          }));
          nullable = blobInfo.BlobAddedTime;
          if (nullable.HasValue)
          {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            ref DateTimeOffset local = ref utcNow;
            nullable = blobInfo.BlobAddedTime;
            DateTimeOffset dateTimeOffset = nullable.Value;
            double totalDays = local.Subtract(dateTimeOffset).TotalDays;
            this.ageSizeHistogram.AddToCounts(totalDays, blobLength);
            this.ageLogicalSizeHistogram.AddToCounts(totalDays, blobLength * idReferenceCount);
          }
        }
        else
          ++this.TotalBlobsWithoutSize;
        if (blobInfo.IdReferenceCount > 1L)
          this.idRefCountHistogram.IncrementCount((double) blobInfo.IdReferenceCount);
        this.TotalExpiredBlobs += (long) (idReferenceCount == 0L);
        this.TotalSingleRefBlobs += (long) (idReferenceCount == 1L);
      }
    }

    public void AddStorageSizeJobResult(IEnumerable<FileStorageSizeJobResult> results)
    {
      foreach (FileStorageSizeJobResult result in results)
      {
        this.TotalBlobs += result.TotalBlobs;
        this.TotalExpiredBlobs += result.TotalExpiredBlobs;
        this.TotalSingleRefBlobs += result.TotalSingleRefBlobs;
        this.TotalBlobsWithoutSize += result.TotalBlobsWithoutSize;
        this.TotalBlobsNotInAddedState += result.TotalBlobsNotInAddedState;
        this.TotalSizeUpdates += result.TotalSizeUpdates;
        this.TotalSizeUpdateFailures += result.TotalSizeUpdateFailures;
        this.TotalBytes += result.TotalBytes;
        this.TotalLogicalBytes += result.TotalLogicalBytes;
        this.TotalExpiredBytes += result.TotalExpiredBytes;
        this.TotalSingleRefBytes += result.TotalSingleRefBytes;
        foreach (KeyValuePair<ArtifactScopeType, long> keyValuePair in result.LogicalSizeByScope)
        {
          KeyValuePair<ArtifactScopeType, long> kvp = keyValuePair;
          this.LogicalSizeByScope.AddOrUpdate(kvp.Key, kvp.Value, (Func<ArtifactScopeType, long, long>) ((k, v) => v + kvp.Value));
        }
        foreach (KeyValuePair<ArtifactScopeType, long> keyValuePair in result.PhysicalSizeByScope)
        {
          KeyValuePair<ArtifactScopeType, long> kvp = keyValuePair;
          this.PhysicalSizeByScope.AddOrUpdate(kvp.Key, kvp.Value, (Func<ArtifactScopeType, long, long>) ((k, v) => v + kvp.Value));
        }
        this.TotalThrottleDuration += result.TotalThrottleDuration;
        this.NumJobRetried += result.NumJobRetried;
        this.ageSizeHistogram.MergeFrom((Histogram) result.ageSizeHistogram);
        this.ageLogicalSizeHistogram.MergeFrom((Histogram) result.ageLogicalSizeHistogram);
        this.idRefCountHistogram.MergeFrom((Histogram) result.idRefCountHistogram);
      }
    }

    public void Scale(double factor)
    {
      if (factor < 0.9999)
        throw new ArgumentException("Factor needs to be greater than 1.0");
      this.TotalBlobs = (long) ((double) this.TotalBlobs * factor);
      this.TotalExpiredBlobs = (long) ((double) this.TotalExpiredBlobs * factor);
      this.TotalSingleRefBlobs = (long) ((double) this.TotalSingleRefBlobs * factor);
      this.TotalBlobsWithoutSize = (long) ((double) this.TotalBlobsWithoutSize * factor);
      this.TotalBlobsNotInAddedState = (long) ((double) this.TotalBlobsNotInAddedState * factor);
      this.TotalSizeUpdates = (long) ((double) this.TotalSizeUpdates * factor);
      this.TotalSizeUpdateFailures = (long) ((double) this.TotalSizeUpdateFailures * factor);
      this.TotalBytes = (ulong) ((double) this.TotalBytes * factor);
      this.TotalLogicalBytes = (ulong) ((double) this.TotalLogicalBytes * factor);
      this.TotalExpiredBytes = (ulong) ((double) this.TotalExpiredBytes * factor);
      this.TotalSingleRefBytes = (ulong) ((double) this.TotalSingleRefBytes * factor);
      foreach (KeyValuePair<ArtifactScopeType, long> keyValuePair in this.LogicalSizeByScope)
        this.LogicalSizeByScope.AddOrUpdate(keyValuePair.Key, keyValuePair.Value, (Func<ArtifactScopeType, long, long>) ((k, v) => (long) ((double) v * factor)));
      foreach (KeyValuePair<ArtifactScopeType, long> keyValuePair in this.PhysicalSizeByScope)
        this.PhysicalSizeByScope.AddOrUpdate(keyValuePair.Key, keyValuePair.Value, (Func<ArtifactScopeType, long, long>) ((k, v) => (long) ((double) v * factor)));
      this.ageSizeHistogram.Scale(factor);
      this.ageLogicalSizeHistogram.Scale(factor);
      this.idRefCountHistogram.Scale(factor);
    }
  }
}
