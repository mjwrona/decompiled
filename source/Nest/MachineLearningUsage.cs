// Decompiled with JetBrains decompiler
// Type: Nest.MachineLearningUsage
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class MachineLearningUsage : XPackUsage
  {
    [DataMember(Name = "node_count")]
    public int NodeCount { get; internal set; }

    [DataMember(Name = "datafeeds")]
    public IReadOnlyDictionary<string, MachineLearningUsage.DataFeed> Datafeeds { get; set; } = EmptyReadOnly<string, MachineLearningUsage.DataFeed>.Dictionary;

    [DataMember(Name = "jobs")]
    public IReadOnlyDictionary<string, MachineLearningUsage.Job> Jobs { get; set; } = EmptyReadOnly<string, MachineLearningUsage.Job>.Dictionary;

    public class DataFeed
    {
      [DataMember(Name = "count")]
      public long Count { get; internal set; }
    }

    public class Job
    {
      [DataMember(Name = "count")]
      public long Count { get; internal set; }

      [DataMember(Name = "detectors")]
      public MachineLearningUsage.JobStatistics Detectors { get; internal set; }

      [DataMember(Name = "forecasts")]
      public MachineLearningUsage.ForecastStatistics Forecasts { get; internal set; }

      [DataMember(Name = "created_by")]
      public IReadOnlyDictionary<string, long> CreatedBy { get; internal set; }

      [DataMember(Name = "model_size")]
      public MachineLearningUsage.JobStatistics ModelSize { get; internal set; }
    }

    public class JobStatistics
    {
      [DataMember(Name = "avg")]
      public double Average { get; internal set; }

      [DataMember(Name = "max")]
      public double Maximum { get; internal set; }

      [DataMember(Name = "min")]
      public double Minimum { get; internal set; }

      [DataMember(Name = "total")]
      public double Total { get; internal set; }
    }

    public class ForecastStatistics
    {
      [DataMember(Name = "forecasted_jobs")]
      public long Jobs { get; internal set; }

      [DataMember(Name = "memory_bytes")]
      public MachineLearningUsage.JobStatistics MemoryBytes { get; internal set; }

      [DataMember(Name = "processing_time_ms")]
      public MachineLearningUsage.JobStatistics ProcessingTimeMilliseconds { get; internal set; }

      [DataMember(Name = "records")]
      public MachineLearningUsage.JobStatistics Records { get; internal set; }

      [DataMember(Name = "status")]
      public IReadOnlyDictionary<string, long> Status { get; internal set; } = EmptyReadOnly<string, long>.Dictionary;

      [DataMember(Name = "total")]
      public long Total { get; internal set; }
    }
  }
}
