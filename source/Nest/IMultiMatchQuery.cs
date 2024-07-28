// Decompiled with JetBrains decompiler
// Type: Nest.IMultiMatchQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (MultiMatchQuery))]
  public interface IMultiMatchQuery : IQuery
  {
    [DataMember(Name = "analyzer")]
    string Analyzer { get; set; }

    [DataMember(Name = "auto_generate_synonyms_phrase_query")]
    bool? AutoGenerateSynonymsPhraseQuery { get; set; }

    [DataMember(Name = "cutoff_frequency")]
    double? CutoffFrequency { get; set; }

    [DataMember(Name = "fields")]
    Fields Fields { get; set; }

    [DataMember(Name = "fuzziness")]
    Fuzziness Fuzziness { get; set; }

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

    [DataMember(Name = "slop")]
    int? Slop { get; set; }

    [DataMember(Name = "tie_breaker")]
    double? TieBreaker { get; set; }

    [DataMember(Name = "type")]
    TextQueryType? Type { get; set; }

    [DataMember(Name = "use_dis_max")]
    bool? UseDisMax { get; set; }

    [DataMember(Name = "zero_terms_query")]
    Nest.ZeroTermsQuery? ZeroTermsQuery { get; set; }
  }
}
