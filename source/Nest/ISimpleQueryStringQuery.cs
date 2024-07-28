// Decompiled with JetBrains decompiler
// Type: Nest.ISimpleQueryStringQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (SimpleQueryStringQueryDescriptor<object>))]
  public interface ISimpleQueryStringQuery : IQuery
  {
    [DataMember(Name = "analyzer")]
    string Analyzer { get; set; }

    [DataMember(Name = "analyze_wildcard")]
    bool? AnalyzeWildcard { get; set; }

    [DataMember(Name = "auto_generate_synonyms_phrase_query")]
    bool? AutoGenerateSynonymsPhraseQuery { get; set; }

    [DataMember(Name = "default_operator")]
    Operator? DefaultOperator { get; set; }

    [DataMember(Name = "fields")]
    Fields Fields { get; set; }

    [DataMember(Name = "flags")]
    SimpleQueryStringFlags? Flags { get; set; }

    [DataMember(Name = "fuzzy_max_expansions")]
    int? FuzzyMaxExpansions { get; set; }

    [DataMember(Name = "fuzzy_prefix_length")]
    int? FuzzyPrefixLength { get; set; }

    [DataMember(Name = "fuzzy_transpositions")]
    bool? FuzzyTranspositions { get; set; }

    [DataMember(Name = "lenient")]
    bool? Lenient { get; set; }

    [DataMember(Name = "minimum_should_match")]
    MinimumShouldMatch MinimumShouldMatch { get; set; }

    [DataMember(Name = "query")]
    string Query { get; set; }

    [DataMember(Name = "quote_field_suffix")]
    string QuoteFieldSuffix { get; set; }
  }
}
