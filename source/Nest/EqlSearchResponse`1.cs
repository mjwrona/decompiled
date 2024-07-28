// Decompiled with JetBrains decompiler
// Type: Nest.EqlSearchResponse`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class EqlSearchResponse<TDocument> : ResponseBase where TDocument : class
  {
    private IReadOnlyCollection<Event<TDocument>> _events;
    private IReadOnlyCollection<Sequence<TDocument>> _sequences;

    [IgnoreDataMember]
    public IReadOnlyCollection<Event<TDocument>> Events => this._events ?? (this._events = this.EqlHitsMetadata?.Events ?? EmptyReadOnly<Event<TDocument>>.Collection);

    [IgnoreDataMember]
    public IReadOnlyCollection<Sequence<TDocument>> Sequences => this._sequences ?? (this._sequences = this.EqlHitsMetadata?.Sequences ?? EmptyReadOnly<Sequence<TDocument>>.Collection);

    [DataMember(Name = "hits")]
    public Nest.EqlHitsMetadata<TDocument> EqlHitsMetadata { get; internal set; }

    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "is_partial")]
    public bool IsPartial { get; internal set; }

    [DataMember(Name = "is_running")]
    public bool IsRunning { get; internal set; }

    [DataMember(Name = "took")]
    public long Took { get; internal set; }

    [DataMember(Name = "timed_out")]
    public bool TimedOut { get; internal set; }

    [IgnoreDataMember]
    public long Total
    {
      get
      {
        Nest.EqlHitsMetadata<TDocument> eqlHitsMetadata = this.EqlHitsMetadata;
        return eqlHitsMetadata == null ? -1L : eqlHitsMetadata.Total.Value;
      }
    }
  }
}
