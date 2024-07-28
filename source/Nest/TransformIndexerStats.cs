// Decompiled with JetBrains decompiler
// Type: Nest.TransformIndexerStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class TransformIndexerStats
  {
    [DataMember(Name = "exponential_avg_checkpoint_duration_ms")]
    public double ExponentialAverageCheckpointDurationMs { get; internal set; }

    [DataMember(Name = "exponential_avg_documents_indexed")]
    public double ExponentialAverageDocumentsIndexed { get; internal set; }

    [DataMember(Name = "exponential_avg_documents_processed")]
    public double ExponentialAverageDocumentsProcessed { get; internal set; }

    [DataMember(Name = "pages_processed")]
    public long PagesProcessed { get; internal set; }

    [DataMember(Name = "documents_processed")]
    public long DocumentsProcessed { get; internal set; }

    [DataMember(Name = "documents_indexed")]
    public long DocumentsIndexed { get; internal set; }

    [DataMember(Name = "trigger_count")]
    public long TriggerCount { get; internal set; }

    [DataMember(Name = "index_time_in_ms")]
    public long IndexTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "search_time_in_ms")]
    public long SearchTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "processing_time_in_ms")]
    public long ProcessingTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "index_total")]
    public long IndexTotal { get; internal set; }

    [DataMember(Name = "search_total")]
    public long SearchTotal { get; internal set; }

    [DataMember(Name = "processing_total")]
    public long ProcessingTotal { get; internal set; }

    [DataMember(Name = "search_failures")]
    public long SearchFailures { get; internal set; }

    [DataMember(Name = "index_failures")]
    public long IndexFailures { get; internal set; }
  }
}
