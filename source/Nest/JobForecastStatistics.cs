// Decompiled with JetBrains decompiler
// Type: Nest.JobForecastStatistics
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class JobForecastStatistics
  {
    [DataMember(Name = "forecasted_jobs")]
    public long ForecastedJobs { get; internal set; }

    [DataMember(Name = "memory_bytes")]
    public JobForecastStatistics.JobStatistics MemoryBytes { get; internal set; }

    [DataMember(Name = "processing_time_ms")]
    public JobForecastStatistics.JobStatistics ProcessingTimeMilliseconds { get; internal set; }

    [DataMember(Name = "records")]
    public JobForecastStatistics.JobStatistics Records { get; internal set; }

    [DataMember(Name = "status")]
    public IReadOnlyDictionary<string, long> Status { get; internal set; } = EmptyReadOnly<string, long>.Dictionary;

    [DataMember(Name = "total")]
    public long Total { get; internal set; }

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
  }
}
