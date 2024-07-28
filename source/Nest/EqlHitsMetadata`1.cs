// Decompiled with JetBrains decompiler
// Type: Nest.EqlHitsMetadata`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class EqlHitsMetadata<TEvent> where TEvent : class
  {
    [DataMember(Name = "events")]
    public IReadOnlyCollection<Event<TEvent>> Events { get; internal set; } = EmptyReadOnly<Event<TEvent>>.Collection;

    [DataMember(Name = "sequences")]
    public IReadOnlyCollection<Sequence<TEvent>> Sequences { get; internal set; } = EmptyReadOnly<Sequence<TEvent>>.Collection;

    [DataMember(Name = "total")]
    public TotalHits Total { get; internal set; }
  }
}
