// Decompiled with JetBrains decompiler
// Type: Nest.ITermsAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (TermsAggregation))]
  public interface ITermsAggregation : IBucketAggregation, IAggregation
  {
    [DataMember(Name = "collect_mode")]
    TermsAggregationCollectMode? CollectMode { get; set; }

    [DataMember(Name = "exclude")]
    TermsExclude Exclude { get; set; }

    [DataMember(Name = "execution_hint")]
    TermsAggregationExecutionHint? ExecutionHint { get; set; }

    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "include")]
    TermsInclude Include { get; set; }

    [DataMember(Name = "min_doc_count")]
    int? MinimumDocumentCount { get; set; }

    [DataMember(Name = "missing")]
    object Missing { get; set; }

    [DataMember(Name = "order")]
    IList<TermsOrder> Order { get; set; }

    [DataMember(Name = "script")]
    IScript Script { get; set; }

    [DataMember(Name = "shard_size")]
    int? ShardSize { get; set; }

    [DataMember(Name = "show_term_doc_count_error")]
    bool? ShowTermDocCountError { get; set; }

    [DataMember(Name = "size")]
    int? Size { get; set; }
  }
}
