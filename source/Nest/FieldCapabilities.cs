// Decompiled with JetBrains decompiler
// Type: Nest.FieldCapabilities
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class FieldCapabilities
  {
    [DataMember(Name = "aggregatable")]
    public bool Aggregatable { get; internal set; }

    [DataMember(Name = "indices")]
    [JsonFormatter(typeof (IndicesFormatter))]
    public Indices Indices { get; internal set; }

    [DataMember(Name = "meta")]
    public IReadOnlyDictionary<string, string[]> Meta { get; internal set; } = EmptyReadOnly<string, string[]>.Dictionary;

    [DataMember(Name = "non_aggregatable_indices")]
    [JsonFormatter(typeof (IndicesFormatter))]
    public Indices NonAggregatableIndices { get; internal set; }

    [DataMember(Name = "non_searchable_indices")]
    [JsonFormatter(typeof (IndicesFormatter))]
    public Indices NonSearchableIndices { get; internal set; }

    [DataMember(Name = "searchable")]
    public bool Searchable { get; internal set; }
  }
}
