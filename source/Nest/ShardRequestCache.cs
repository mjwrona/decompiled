// Decompiled with JetBrains decompiler
// Type: Nest.ShardRequestCache
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ShardRequestCache
  {
    [DataMember(Name = "evictions")]
    public long Evictions { get; internal set; }

    [DataMember(Name = "hit_count")]
    public long HitCount { get; internal set; }

    [DataMember(Name = "memory_size_in_bytes")]
    public long MemorySizeInBytes { get; internal set; }

    [DataMember(Name = "miss_count")]
    public long MissCount { get; internal set; }
  }
}
