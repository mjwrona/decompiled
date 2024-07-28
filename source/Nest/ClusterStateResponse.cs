// Decompiled with JetBrains decompiler
// Type: Nest.ClusterStateResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [JsonFormatter(typeof (DynamicResponseFormatter<ClusterStateResponse>))]
  public class ClusterStateResponse : DynamicResponseBase
  {
    public DynamicDictionary State => this.Self.BackingDictionary;

    [DataMember(Name = "cluster_name")]
    public string ClusterName => this.State.Get<string>("cluster_name");

    [DataMember(Name = "cluster_uuid")]
    public string ClusterUUID => this.State.Get<string>("cluster_uuid");

    [DataMember(Name = "master_node")]
    public string MasterNode => this.State.Get<string>("master_node");

    [DataMember(Name = "state_uuid")]
    public string StateUUID => this.State.Get<string>("state_uuid");

    [DataMember(Name = "version")]
    public long? Version => this.State.Get<long?>("version");
  }
}
