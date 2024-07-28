// Decompiled with JetBrains decompiler
// Type: Nest.DatafeedTimingStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class DatafeedTimingStats
  {
    [DataMember(Name = "bucket_count")]
    public long BucketCount { get; internal set; }

    [DataMember(Name = "exponential_average_search_time_per_hour_ms")]
    public double ExponentialAverageSearchTimePerHourMilliseconds { get; internal set; }

    [DataMember(Name = "job_id")]
    public string JobId { get; internal set; }

    [DataMember(Name = "search_count")]
    public long SearchCount { get; internal set; }

    [DataMember(Name = "total_search_time_ms")]
    public double TotalSearchTimeMilliseconds { get; internal set; }
  }
}
