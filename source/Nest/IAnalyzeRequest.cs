// Decompiled with JetBrains decompiler
// Type: Nest.IAnalyzeRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [MapsApi("indices.analyze.json")]
  [ReadAs(typeof (AnalyzeRequest))]
  [InterfaceDataContract]
  public interface IAnalyzeRequest : IRequest<AnalyzeRequestParameters>, IRequest
  {
    [DataMember(Name = "analyzer")]
    string Analyzer { get; set; }

    [DataMember(Name = "attributes")]
    IEnumerable<string> Attributes { get; set; }

    [DataMember(Name = "char_filter")]
    AnalyzeCharFilters CharFilter { get; set; }

    [DataMember(Name = "explain")]
    bool? Explain { get; set; }

    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "filter")]
    AnalyzeTokenFilters Filter { get; set; }

    [DataMember(Name = "normalizer")]
    string Normalizer { get; set; }

    [DataMember(Name = "text")]
    IEnumerable<string> Text { get; set; }

    [DataMember(Name = "tokenizer")]
    Union<string, ITokenizer> Tokenizer { get; set; }

    [IgnoreDataMember]
    IndexName Index { get; }
  }
}
