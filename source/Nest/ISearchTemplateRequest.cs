// Decompiled with JetBrains decompiler
// Type: Nest.ISearchTemplateRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (SearchTemplateRequest))]
  [MapsApi("search_template.json")]
  public interface ISearchTemplateRequest : 
    IRequest<SearchTemplateRequestParameters>,
    IRequest,
    ITypedSearchRequest
  {
    [IgnoreDataMember]
    Indices Index { get; }

    [DataMember(Name = "id")]
    string Id { get; set; }

    [DataMember(Name = "params")]
    IDictionary<string, object> Params { get; set; }

    [DataMember(Name = "source")]
    string Source { get; set; }

    [DataMember(Name = "explain")]
    bool? Explain { get; set; }
  }
}
