// Decompiled with JetBrains decompiler
// Type: Nest.NodesInfoResponse
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
  public class NodesInfoResponse : NodesResponseBase
  {
    [DataMember(Name = "cluster_name")]
    public string ClusterName { get; internal set; }

    [DataMember(Name = "nodes")]
    [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, NodeInfo>))]
    public IReadOnlyDictionary<string, NodeInfo> Nodes { get; internal set; } = EmptyReadOnly<string, NodeInfo>.Dictionary;
  }
}
