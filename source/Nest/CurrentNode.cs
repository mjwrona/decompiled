// Decompiled with JetBrains decompiler
// Type: Nest.CurrentNode
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CurrentNode
  {
    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    [DataMember(Name = "attributes")]
    public IReadOnlyDictionary<string, string> NodeAttributes { get; set; } = EmptyReadOnly<string, string>.Dictionary;

    [DataMember(Name = "transport_address")]
    public string TransportAddress { get; internal set; }

    [DataMember(Name = "weight_ranking")]
    public int WeightRanking { get; internal set; }
  }
}
