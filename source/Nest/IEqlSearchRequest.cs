// Decompiled with JetBrains decompiler
// Type: Nest.IEqlSearchRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.EqlApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("eql.search.json")]
  [ReadAs(typeof (EqlSearchRequest))]
  public interface IEqlSearchRequest : 
    IRequest<EqlSearchRequestParameters>,
    IRequest,
    ITypedSearchRequest
  {
    [IgnoreDataMember]
    Indices Index { get; }

    [DataMember(Name = "event_category_field")]
    Field EventCategoryField { get; set; }

    [DataMember(Name = "fetch_size")]
    int? FetchSize { get; set; }

    [DataMember(Name = "fields")]
    Fields Fields { get; set; }

    [DataMember(Name = "filter")]
    QueryContainer Filter { get; set; }

    [DataMember(Name = "query")]
    string Query { get; set; }

    [DataMember(Name = "result_position")]
    EqlResultPosition? ResultPosition { get; set; }

    [DataMember(Name = "runtime_mappings")]
    IRuntimeFields RuntimeFields { get; set; }

    [DataMember(Name = "size")]
    float? Size { get; set; }

    [DataMember(Name = "tiebreaker_field")]
    Field TiebreakerField { get; set; }

    [DataMember(Name = "timestamp_field")]
    Field TimestampField { get; set; }
  }
}
