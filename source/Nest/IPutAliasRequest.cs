// Decompiled with JetBrains decompiler
// Type: Nest.IPutAliasRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [MapsApi("indices.put_alias.json")]
  [InterfaceDataContract]
  public interface IPutAliasRequest : IRequest<PutAliasRequestParameters>, IRequest
  {
    [DataMember(Name = "filter")]
    QueryContainer Filter { get; set; }

    [DataMember(Name = "index_routing")]
    Routing IndexRouting { get; set; }

    [DataMember(Name = "is_write_index")]
    bool? IsWriteIndex { get; set; }

    [DataMember(Name = "routing")]
    Routing Routing { get; set; }

    [DataMember(Name = "search_routing")]
    Routing SearchRouting { get; set; }

    [IgnoreDataMember]
    Indices Index { get; }

    [IgnoreDataMember]
    Name Name { get; }
  }
}
