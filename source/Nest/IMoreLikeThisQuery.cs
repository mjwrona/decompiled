// Decompiled with JetBrains decompiler
// Type: Nest.IMoreLikeThisQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (MoreLikeThisQuery))]
  public interface IMoreLikeThisQuery : IQuery
  {
    [DataMember(Name = "analyzer")]
    string Analyzer { get; set; }

    [DataMember(Name = "boost_terms")]
    double? BoostTerms { get; set; }

    [DataMember(Name = "fields")]
    Fields Fields { get; set; }

    [DataMember(Name = "include")]
    bool? Include { get; set; }

    [DataMember(Name = "like")]
    IEnumerable<Nest.Like> Like { get; set; }

    [DataMember(Name = "max_doc_freq")]
    int? MaxDocumentFrequency { get; set; }

    [DataMember(Name = "max_query_terms")]
    int? MaxQueryTerms { get; set; }

    [DataMember(Name = "max_word_length")]
    int? MaxWordLength { get; set; }

    [DataMember(Name = "min_doc_freq")]
    int? MinDocumentFrequency { get; set; }

    [DataMember(Name = "minimum_should_match")]
    MinimumShouldMatch MinimumShouldMatch { get; set; }

    [DataMember(Name = "min_term_freq")]
    int? MinTermFrequency { get; set; }

    [DataMember(Name = "min_word_length")]
    int? MinWordLength { get; set; }

    [DataMember(Name = "per_field_analyzer")]
    IPerFieldAnalyzer PerFieldAnalyzer { get; set; }

    [DataMember(Name = "routing")]
    Routing Routing { get; set; }

    [DataMember(Name = "stop_words")]
    StopWords StopWords { get; set; }

    [DataMember(Name = "unlike")]
    IEnumerable<Nest.Like> Unlike { get; set; }

    [DataMember(Name = "version")]
    long? Version { get; set; }

    [DataMember(Name = "version_type")]
    Elasticsearch.Net.VersionType? VersionType { get; set; }
  }
}
