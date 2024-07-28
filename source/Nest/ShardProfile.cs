// Decompiled with JetBrains decompiler
// Type: Nest.ShardProfile
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class ShardProfile
  {
    [DataMember(Name = "aggregations")]
    public IReadOnlyCollection<AggregationProfile> Aggregations { get; internal set; } = EmptyReadOnly<AggregationProfile>.Collection;

    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "searches")]
    public IReadOnlyCollection<SearchProfile> Searches { get; internal set; } = EmptyReadOnly<SearchProfile>.Collection;
  }
}
