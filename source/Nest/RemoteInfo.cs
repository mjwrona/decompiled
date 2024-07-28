// Decompiled with JetBrains decompiler
// Type: Nest.RemoteInfo
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class RemoteInfo
  {
    [DataMember(Name = "connected")]
    public bool Connected { get; internal set; }

    [DataMember(Name = "skip_unavailable")]
    public bool SkipUnavailable { get; internal set; }

    [DataMember(Name = "initial_connect_timeout")]
    public Time InitialConnectTimeout { get; internal set; }

    [DataMember(Name = "max_connections_per_cluster")]
    public int MaxConnectionsPerCluster { get; internal set; }

    [DataMember(Name = "num_nodes_connected")]
    public long NumNodesConnected { get; internal set; }

    [DataMember(Name = "seeds")]
    public IReadOnlyCollection<string> Seeds { get; internal set; } = EmptyReadOnly<string>.Collection;
  }
}
