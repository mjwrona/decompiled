// Decompiled with JetBrains decompiler
// Type: Nest.ISignificantTextAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (SignificantTextAggregation))]
  public interface ISignificantTextAggregation : IBucketAggregation, IAggregation
  {
    [DataMember(Name = "background_filter")]
    QueryContainer BackgroundFilter { get; set; }

    [DataMember(Name = "chi_square")]
    IChiSquareHeuristic ChiSquare { get; set; }

    [DataMember(Name = "exclude")]
    IncludeExclude Exclude { get; set; }

    [DataMember(Name = "execution_hint")]
    TermsAggregationExecutionHint? ExecutionHint { get; set; }

    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "filter_duplicate_text")]
    bool? FilterDuplicateText { get; set; }

    [DataMember(Name = "gnd")]
    IGoogleNormalizedDistanceHeuristic GoogleNormalizedDistance { get; set; }

    [DataMember(Name = "include")]
    IncludeExclude Include { get; set; }

    [DataMember(Name = "min_doc_count")]
    long? MinimumDocumentCount { get; set; }

    [DataMember(Name = "mutual_information")]
    IMutualInformationHeuristic MutualInformation { get; set; }

    [DataMember(Name = "percentage")]
    IPercentageScoreHeuristic PercentageScore { get; set; }

    [DataMember(Name = "script_heuristic")]
    IScriptedHeuristic Script { get; set; }

    [DataMember(Name = "shard_min_doc_count")]
    long? ShardMinimumDocumentCount { get; set; }

    [DataMember(Name = "shard_size")]
    int? ShardSize { get; set; }

    [DataMember(Name = "size")]
    int? Size { get; set; }

    [DataMember(Name = "source_fields")]
    Fields SourceFields { get; set; }
  }
}
