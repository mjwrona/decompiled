// Decompiled with JetBrains decompiler
// Type: Nest.ShardMerges
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ShardMerges
  {
    [DataMember(Name = "current")]
    public long Current { get; internal set; }

    [DataMember(Name = "current_docs")]
    public long CurrentDocuments { get; internal set; }

    [DataMember(Name = "current_size_in_bytes")]
    public long CurrentSizeInBytes { get; internal set; }

    [DataMember(Name = "total")]
    public long Total { get; internal set; }

    [DataMember(Name = "total_auto_throttle_in_bytes")]
    public long TotalAutoThrottleInBytes { get; internal set; }

    [DataMember(Name = "total_docs")]
    public long TotalDocuments { get; internal set; }

    [DataMember(Name = "total_size_in_bytes")]
    public long TotalSizeInBytes { get; internal set; }

    [DataMember(Name = "total_stopped_time_in_millis")]
    public long TotalStoppedTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "total_throttled_time_in_millis")]
    public long TotalThrottledTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "total_time_in_millis")]
    public long TotalTimeInMilliseconds { get; internal set; }
  }
}
