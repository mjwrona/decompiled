// Decompiled with JetBrains decompiler
// Type: Nest.EnrichStatsResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class EnrichStatsResponse : ResponseBase
  {
    [DataMember(Name = "executing_policies")]
    public IReadOnlyCollection<ExecutingPolicy> ExecutingPolicies { get; internal set; } = EmptyReadOnly<ExecutingPolicy>.Collection;

    [DataMember(Name = "coordinator_stats")]
    public IReadOnlyCollection<Nest.CoordinatorStats> CoordinatorStats { get; internal set; } = EmptyReadOnly<Nest.CoordinatorStats>.Collection;
  }
}
