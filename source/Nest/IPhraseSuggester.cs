// Decompiled with JetBrains decompiler
// Type: Nest.IPhraseSuggester
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (PhraseSuggester))]
  public interface IPhraseSuggester : ISuggester
  {
    [DataMember(Name = "collate")]
    IPhraseSuggestCollate Collate { get; set; }

    [DataMember(Name = "confidence")]
    double? Confidence { get; set; }

    [DataMember(Name = "direct_generator")]
    IEnumerable<IDirectGenerator> DirectGenerator { get; set; }

    [DataMember(Name = "force_unigrams")]
    bool? ForceUnigrams { get; set; }

    [DataMember(Name = "gram_size")]
    int? GramSize { get; set; }

    [DataMember(Name = "highlight")]
    IPhraseSuggestHighlight Highlight { get; set; }

    [DataMember(Name = "max_errors")]
    double? MaxErrors { get; set; }

    [DataMember(Name = "real_word_error_likelihood")]
    double? RealWordErrorLikelihood { get; set; }

    [DataMember(Name = "separator")]
    char? Separator { get; set; }

    [DataMember(Name = "shard_size")]
    int? ShardSize { get; set; }

    [DataMember(Name = "smoothing")]
    SmoothingModelContainer Smoothing { get; set; }

    [IgnoreDataMember]
    string Text { get; set; }

    [DataMember(Name = "token_limit")]
    int? TokenLimit { get; set; }
  }
}
