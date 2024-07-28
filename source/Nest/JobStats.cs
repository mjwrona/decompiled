// Decompiled with JetBrains decompiler
// Type: Nest.JobStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class JobStats
  {
    [DataMember(Name = "assignment_explanation")]
    public string AssignmentExplanation { get; internal set; }

    [DataMember(Name = "data_counts")]
    public DataCounts DataCounts { get; internal set; }

    [DataMember(Name = "deleting")]
    public bool? Deleting { get; internal set; }

    [DataMember(Name = "forecasts_stats")]
    public JobForecastStatistics Forecasts { get; internal set; }

    [DataMember(Name = "job_id")]
    public string JobId { get; internal set; }

    [DataMember(Name = "model_size_stats")]
    public ModelSizeStats ModelSizeStats { get; internal set; }

    [DataMember(Name = "node")]
    public DiscoveryNode Node { get; internal set; }

    [DataMember(Name = "open_time")]
    public Time OpenTime { get; internal set; }

    [DataMember(Name = "state")]
    public JobState State { get; internal set; }

    [DataMember(Name = "timing_stats")]
    public TimingStats TimingStats { get; internal set; }
  }
}
