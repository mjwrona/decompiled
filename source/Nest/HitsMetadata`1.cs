// Decompiled with JetBrains decompiler
// Type: Nest.HitsMetadata`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class HitsMetadata<T> : IHitsMetadata<T> where T : class
  {
    [DataMember(Name = "hits")]
    public IReadOnlyCollection<IHit<T>> Hits { get; internal set; } = EmptyReadOnly<IHit<T>>.Collection;

    [DataMember(Name = "max_score")]
    public double? MaxScore { get; internal set; }

    [DataMember(Name = "total")]
    public TotalHits Total { get; internal set; }
  }
}
