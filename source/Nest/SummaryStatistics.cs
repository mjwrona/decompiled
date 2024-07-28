// Decompiled with JetBrains decompiler
// Type: Nest.SummaryStatistics
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class SummaryStatistics
  {
    [DataMember(Name = "count")]
    public int Count { get; internal set; }

    [DataMember(Name = "total_elapsed")]
    public string TotalElapsed { get; internal set; }

    [DataMember(Name = "total_elapsed_nanos")]
    public long TotalElapsedNanos { get; internal set; }

    [DataMember(Name = "total_size")]
    public string TotalSize { get; internal set; }

    [DataMember(Name = "total_size_bytes")]
    public long TotalSizeBytes { get; internal set; }

    [DataMember(Name = "total_throttled")]
    public string TotalThrottled { get; internal set; }

    [DataMember(Name = "total_throttled_nanos")]
    public long TotalThrottledNanos { get; internal set; }
  }
}
