// Decompiled with JetBrains decompiler
// Type: Nest.NodeAllocationExplanation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class NodeAllocationExplanation
  {
    [DataMember(Name = "deciders")]
    public IReadOnlyCollection<AllocationDecision> Deciders { get; set; } = EmptyReadOnly<AllocationDecision>.Collection;

    [DataMember(Name = "node_attributes")]
    public IReadOnlyDictionary<string, string> NodeAttributes { get; set; } = EmptyReadOnly<string, string>.Dictionary;

    [DataMember(Name = "node_decision")]
    public Decision? NodeDecision { get; set; }

    [DataMember(Name = "node_id")]
    public string NodeId { get; set; }

    [DataMember(Name = "node_name")]
    public string NodeName { get; set; }

    [DataMember(Name = "store")]
    public AllocationStore Store { get; set; }

    [DataMember(Name = "transport_address")]
    public string TransportAddress { get; set; }

    [DataMember(Name = "weight_ranking")]
    public int? WeightRanking { get; set; }
  }
}
