// Decompiled with JetBrains decompiler
// Type: Nest.AliasAddOperation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class AliasAddOperation
  {
    [DataMember(Name = "alias")]
    public string Alias { get; set; }

    [DataMember(Name = "aliases")]
    public IEnumerable<string> Aliases { get; set; }

    [DataMember(Name = "filter")]
    public QueryContainer Filter { get; set; }

    [DataMember(Name = "index")]
    public IndexName Index { get; set; }

    [DataMember(Name = "indices")]
    [JsonFormatter(typeof (IndicesFormatter))]
    public Indices Indices { get; set; }

    [DataMember(Name = "index_routing")]
    public string IndexRouting { get; set; }

    [DataMember(Name = "is_write_index")]
    public bool? IsWriteIndex { get; set; }

    [DataMember(Name = "is_hidden")]
    public bool? IsHidden { get; set; }

    [DataMember(Name = "routing")]
    public string Routing { get; set; }

    [DataMember(Name = "search_routing")]
    public string SearchRouting { get; set; }
  }
}
