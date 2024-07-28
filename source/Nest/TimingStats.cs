// Decompiled with JetBrains decompiler
// Type: Nest.TimingStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class TimingStats
  {
    [DataMember(Name = "job_id")]
    public string JobId { get; internal set; }

    [DataMember(Name = "bucket_count")]
    public long BucketCount { get; internal set; }

    [DataMember(Name = "minimum_bucket_processing_time_ms")]
    public double MinimumBucketProcessingTimeMilliseconds { get; internal set; }

    [DataMember(Name = "maximum_bucket_processing_time_ms")]
    public double MaximumBucketProcessingTimeMilliseconds { get; internal set; }

    [DataMember(Name = "average_bucket_processing_time_ms")]
    public double AverageBucketProcessingTimeMilliseconds { get; internal set; }

    [DataMember(Name = "exponential_average_bucket_processing_time_ms")]
    public double ExponentialAverageBucketProcessingTimeMilliseconds { get; internal set; }

    [DataMember(Name = "exponential_average_bucket_processing_time_per_hour_ms")]
    public double ExponentialAverageBucketProcessingTimePerHourMilliseconds { get; internal set; }

    [DataMember(Name = "total_bucket_processing_time_ms")]
    public double TotalBucketProcessingTimeMilliseconds { get; internal set; }
  }
}
