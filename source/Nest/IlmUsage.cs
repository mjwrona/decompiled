// Decompiled with JetBrains decompiler
// Type: Nest.IlmUsage
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class IlmUsage
  {
    [DataMember(Name = "policy_count")]
    public int PolicyCount { get; internal set; }

    [DataMember(Name = "policy_stats")]
    public IReadOnlyCollection<IlmUsage.IlmPolicyStatistics> PolicyStatistics { get; internal set; } = EmptyReadOnly<IlmUsage.IlmPolicyStatistics>.Collection;

    public class IlmPolicyStatistics
    {
      [DataMember(Name = "phases")]
      public IPhases Phases { get; internal set; }

      [DataMember(Name = "indices_managed")]
      public int IndicesManaged { get; internal set; }
    }
  }
}
