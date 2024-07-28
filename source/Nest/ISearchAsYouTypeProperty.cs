// Decompiled with JetBrains decompiler
// Type: Nest.ISearchAsYouTypeProperty
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface ISearchAsYouTypeProperty : ICoreProperty, IProperty, IFieldMapping
  {
    [DataMember(Name = "analyzer")]
    string Analyzer { get; set; }

    [DataMember(Name = "index")]
    bool? Index { get; set; }

    [DataMember(Name = "index_options")]
    Nest.IndexOptions? IndexOptions { get; set; }

    [DataMember(Name = "max_shingle_size")]
    int? MaxShingleSize { get; set; }

    [DataMember(Name = "norms")]
    bool? Norms { get; set; }

    [DataMember(Name = "search_analyzer")]
    string SearchAnalyzer { get; set; }

    [DataMember(Name = "search_quote_analyzer")]
    string SearchQuoteAnalyzer { get; set; }

    [DataMember(Name = "term_vector")]
    TermVectorOption? TermVector { get; set; }
  }
}
