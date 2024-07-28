// Decompiled with JetBrains decompiler
// Type: Nest.CatTransformsRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CatTransformsRecord : ICatRecord
  {
    [DataMember(Name = "changes_last_detection_time")]
    public string ChangesLastDetectionTime { get; internal set; }

    [DataMember(Name = "checkpoint_duration_time_exp_avg")]
    public long? CheckpointDurationTimeExpAvg { get; internal set; }

    [DataMember(Name = "create_time")]
    public DateTimeOffset CreateTime { get; internal set; }

    [DataMember(Name = "description")]
    public string Description { get; internal set; }

    [DataMember(Name = "dest_index")]
    public string DestinationIndex { get; internal set; }

    [DataMember(Name = "documents_indexed")]
    public long? DocumentsIndexed { get; internal set; }

    [DataMember(Name = "documents_processed")]
    public long? DocumentsProcessed { get; internal set; }

    [DataMember(Name = "frequency")]
    public Time Frequency { get; internal set; }

    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "index_failure")]
    public long? IndexFailure { get; internal set; }

    [DataMember(Name = "index_time")]
    public long? IndexTime { get; internal set; }

    [DataMember(Name = "index_total")]
    public long? IndexTotal { get; internal set; }

    [DataMember(Name = "indexed_documents_exp_avg")]
    public long? IndexedDocumentsExpAvg { get; internal set; }

    [DataMember(Name = "max_page_search_size")]
    public long? MaxPageSearchSize { get; internal set; }

    [DataMember(Name = "pages_processed")]
    public long? PagesProcessed { get; internal set; }

    [DataMember(Name = "pipeline")]
    public string Pipeline { get; internal set; }

    [DataMember(Name = "processed_documents_exp_avg")]
    public long? ProcessedDocumentsExpAvg { get; internal set; }

    [DataMember(Name = "processing_time")]
    public long? ProcessingTime { get; internal set; }

    [DataMember(Name = "reason")]
    public string Reason { get; internal set; }

    [DataMember(Name = "search_failure")]
    public long? SearchFailure { get; internal set; }

    [DataMember(Name = "search_time")]
    public long? SearchTime { get; internal set; }

    [DataMember(Name = "search_total")]
    public long? SearchTotal { get; internal set; }

    [DataMember(Name = "source_index")]
    [JsonFormatter(typeof (IndicesFormatter))]
    public Indices SourceIndices { get; internal set; }

    [DataMember(Name = "state")]
    public TransformState State { get; internal set; }

    [DataMember(Name = "transform_type")]
    public TransformType TransformType { get; internal set; }

    [DataMember(Name = "trigger_count")]
    public long? TriggerCount { get; internal set; }

    [DataMember(Name = "version")]
    public string Version { get; internal set; }
  }
}
