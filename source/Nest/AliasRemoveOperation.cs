// Decompiled with JetBrains decompiler
// Type: Nest.AliasRemoveOperation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class AliasRemoveOperation
  {
    [DataMember(Name = "alias")]
    public string Alias { get; set; }

    [DataMember(Name = "aliases")]
    public IEnumerable<string> Aliases { get; set; }

    [DataMember(Name = "index")]
    public IndexName Index { get; set; }

    [DataMember(Name = "indices")]
    [JsonFormatter(typeof (IndicesFormatter))]
    public Indices Indices { get; set; }

    [DataMember(Name = "must_exist")]
    public bool? MustExist { get; set; }
  }
}
