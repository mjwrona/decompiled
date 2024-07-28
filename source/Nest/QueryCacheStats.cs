// Decompiled with JetBrains decompiler
// Type: Nest.QueryCacheStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class QueryCacheStats
  {
    [DataMember(Name = "cache_count")]
    public long CacheCount { get; set; }

    [DataMember(Name = "cache_size")]
    public long CacheSize { get; set; }

    [DataMember(Name = "evictions")]
    public long Evictions { get; set; }

    [DataMember(Name = "hit_count")]
    public long HitCount { get; set; }

    [DataMember(Name = "memory_size_in_bytes")]
    public long MemorySizeInBytes { get; set; }

    [DataMember(Name = "miss_count")]
    public long MissCount { get; set; }

    [DataMember(Name = "total_count")]
    public long TotalCount { get; set; }
  }
}
