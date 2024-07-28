// Decompiled with JetBrains decompiler
// Type: Nest.ITermSuggester
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (TermSuggester))]
  public interface ITermSuggester : ISuggester
  {
    [DataMember(Name = "lowercase_terms")]
    bool? LowercaseTerms { get; set; }

    [DataMember(Name = "max_edits")]
    int? MaxEdits { get; set; }

    [DataMember(Name = "max_inspections")]
    int? MaxInspections { get; set; }

    [DataMember(Name = "max_term_freq")]
    float? MaxTermFrequency { get; set; }

    [DataMember(Name = "min_doc_freq")]
    float? MinDocFrequency { get; set; }

    [DataMember(Name = "min_word_length")]
    int? MinWordLength { get; set; }

    [DataMember(Name = "prefix_length")]
    int? PrefixLength { get; set; }

    [DataMember(Name = "shard_size")]
    int? ShardSize { get; set; }

    [DataMember(Name = "sort")]
    SuggestSort? Sort { get; set; }

    [DataMember(Name = "string_distance")]
    Nest.StringDistance? StringDistance { get; set; }

    [DataMember(Name = "suggest_mode")]
    Elasticsearch.Net.SuggestMode? SuggestMode { get; set; }

    [IgnoreDataMember]
    string Text { get; set; }
  }
}
