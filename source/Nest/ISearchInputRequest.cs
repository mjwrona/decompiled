// Decompiled with JetBrains decompiler
// Type: Nest.ISearchInputRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (SearchInputRequest))]
  public interface ISearchInputRequest
  {
    [DataMember(Name = "body")]
    [ReadAs(typeof (SearchRequest))]
    ISearchRequest Body { get; set; }

    [DataMember(Name = "indices")]
    IEnumerable<IndexName> Indices { get; set; }

    [DataMember(Name = "indices_options")]
    IIndicesOptions IndicesOptions { get; set; }

    [DataMember(Name = "search_type")]
    Elasticsearch.Net.SearchType? SearchType { get; set; }

    [DataMember(Name = "template")]
    [ReadAs(typeof (SearchTemplateRequest))]
    ISearchTemplateRequest Template { get; set; }
  }
}
