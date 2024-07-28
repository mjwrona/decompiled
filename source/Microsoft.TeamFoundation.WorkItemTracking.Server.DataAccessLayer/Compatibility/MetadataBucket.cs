// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility.MetadataBucket
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility
{
  internal class MetadataBucket
  {
    private const int c_minMaskBits = 10;
    private const int c_maxMaskBits = 24;
    private int m_maskBits;
    private long m_id;

    internal MetadataBucket()
    {
      this.m_maskBits = 0;
      this.m_id = 0L;
    }

    public MetadataBucket(long id, int maskBits)
    {
      ArgumentUtility.CheckForOutOfRange(maskBits, nameof (maskBits), 10, 24);
      this.m_maskBits = maskBits;
      ArgumentUtility.CheckForOutOfRange(id, nameof (id), 0L, this.MaxBucketId);
      this.m_id = id;
    }

    public long Id => this.m_id;

    public int MaskBits => this.m_maskBits;

    public int Capacity => (1 << this.MaskBits) - 1;

    public long MaxBucketId => (1L << 64 - this.MaskBits) - 1L;

    public void Merge(MetadataBucket other)
    {
      if (this.MaskBits < other.MaskBits)
      {
        this.m_maskBits = other.MaskBits;
        this.m_id = other.Id;
      }
      else
      {
        if (this.MaskBits != other.MaskBits || this.Id >= other.Id)
          return;
        this.m_id = other.Id;
      }
    }

    public long ParseBucketId(long cacheStamp) => cacheStamp >> this.MaskBits;

    public int ParseLocalId(long cacheStamp) => (int) cacheStamp & int.MaxValue >> 31 - this.MaskBits;

    public ulong GenerateCacheStamp(int localId)
    {
      ArgumentUtility.CheckForOutOfRange(localId, nameof (localId), 1, this.Capacity);
      return this.GetBaseCachestamp() + (ulong) localId;
    }

    public ulong GetBaseCachestamp() => (ulong) (this.Id << this.MaskBits);

    public static int ComputeMaskBits(int capacity)
    {
      int var = 0;
      while (capacity > 0)
      {
        capacity >>= 1;
        ++var;
      }
      ArgumentUtility.CheckForOutOfRange(var, "maskBits", 10, 24);
      return var;
    }

    internal static void EnsureBucketCapacity(
      int capacityNeeded,
      ref MetadataBucket bucket,
      ref int newMaskBits)
    {
      if (capacityNeeded <= bucket.Capacity)
        return;
      newMaskBits = MetadataBucket.ComputeMaskBits(capacityNeeded);
      bucket = new MetadataBucket(bucket.Id, newMaskBits);
    }
  }
}
