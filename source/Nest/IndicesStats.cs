// Decompiled with JetBrains decompiler
// Type: Nest.IndicesStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class IndicesStats
  {
    [DataMember(Name = "primaries")]
    public IndexStats Primaries { get; internal set; }

    [DataMember(Name = "shards")]
    [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, ShardStats[]>))]
    public IReadOnlyDictionary<string, ShardStats[]> Shards { get; internal set; } = EmptyReadOnly<string, ShardStats[]>.Dictionary;

    [DataMember(Name = "total")]
    public IndexStats Total { get; internal set; }

    [DataMember(Name = "uuid")]
    public string UUID { get; }
  }
}
