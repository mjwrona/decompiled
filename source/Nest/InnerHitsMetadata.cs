// Decompiled with JetBrains decompiler
// Type: Nest.InnerHitsMetadata
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Nest
{
  public class InnerHitsMetadata
  {
    [DataMember(Name = "hits")]
    public List<IHit<ILazyDocument>> Hits { get; internal set; }

    [DataMember(Name = "max_score")]
    public double? MaxScore { get; internal set; }

    [DataMember(Name = "total")]
    public TotalHits Total { get; internal set; }

    public IEnumerable<T> Documents<T>() where T : class => this.Hits == null || this.Hits.Count == 0 ? Enumerable.Empty<T>() : (IEnumerable<T>) this.Hits.Select<IHit<ILazyDocument>, T>((Func<IHit<ILazyDocument>, T>) (hit => hit.Source.As<T>())).ToList<T>();
  }
}
