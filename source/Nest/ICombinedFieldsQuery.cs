// Decompiled with JetBrains decompiler
// Type: Nest.ICombinedFieldsQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (CombinedFieldsQuery))]
  public interface ICombinedFieldsQuery : IQuery
  {
    [DataMember(Name = "query")]
    string Query { get; set; }

    [DataMember(Name = "fields")]
    Fields Fields { get; set; }

    [DataMember(Name = "minimum_should_match")]
    MinimumShouldMatch MinimumShouldMatch { get; set; }

    [DataMember(Name = "auto_generate_synonyms_phrase_query")]
    bool? AutoGenerateSynonymsPhraseQuery { get; set; }

    [DataMember(Name = "operator")]
    Nest.Operator? Operator { get; set; }

    [DataMember(Name = "zero_terms_query")]
    Nest.ZeroTermsQuery? ZeroTermsQuery { get; set; }
  }
}
