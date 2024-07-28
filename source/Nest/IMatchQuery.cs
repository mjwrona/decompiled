// Decompiled with JetBrains decompiler
// Type: Nest.IMatchQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [JsonFormatter(typeof (FieldNameQueryFormatter<MatchQuery, IMatchQuery>))]
  public interface IMatchQuery : IFieldNameQuery, IQuery
  {
    [DataMember(Name = "analyzer")]
    string Analyzer { get; set; }

    [DataMember(Name = "auto_generate_synonyms_phrase_query")]
    bool? AutoGenerateSynonymsPhraseQuery { get; set; }

    [DataMember(Name = "cutoff_frequency")]
    [Obsolete("Deprecated in 7.3.0. This option can be omitted since MatchQuery can skip blocks of documents efficiently if the total number of hits is not tracked.")]
    double? CutoffFrequency { get; set; }

    [DataMember(Name = "fuzziness")]
    IFuzziness Fuzziness { get; set; }

    [DataMember(Name = "fuzzy_rewrite")]
    MultiTermQueryRewrite FuzzyRewrite { get; set; }

    [DataMember(Name = "fuzzy_transpositions")]
    bool? FuzzyTranspositions { get; set; }

    [DataMember(Name = "lenient")]
    bool? Lenient { get; set; }

    [DataMember(Name = "max_expansions")]
    int? MaxExpansions { get; set; }

    [DataMember(Name = "minimum_should_match")]
    MinimumShouldMatch MinimumShouldMatch { get; set; }

    [DataMember(Name = "operator")]
    Nest.Operator? Operator { get; set; }

    [DataMember(Name = "prefix_length")]
    int? PrefixLength { get; set; }

    [DataMember(Name = "query")]
    string Query { get; set; }

    [DataMember(Name = "zero_terms_query")]
    Nest.ZeroTermsQuery? ZeroTermsQuery { get; set; }
  }
}
