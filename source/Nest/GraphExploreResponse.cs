// Decompiled with JetBrains decompiler
// Type: Nest.GraphExploreResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class GraphExploreResponse : ResponseBase
  {
    [DataMember(Name = "connections")]
    public IReadOnlyCollection<GraphConnection> Connections { get; internal set; } = EmptyReadOnly<GraphConnection>.Collection;

    [DataMember(Name = "failures")]
    public IReadOnlyCollection<ShardFailure> Failures { get; internal set; } = EmptyReadOnly<ShardFailure>.Collection;

    [DataMember(Name = "timed_out")]
    public bool TimedOut { get; internal set; }

    [DataMember(Name = "took")]
    public long Took { get; internal set; }

    [DataMember(Name = "vertices")]
    public IReadOnlyCollection<GraphVertex> Vertices { get; internal set; } = EmptyReadOnly<GraphVertex>.Collection;
  }
}
