// Decompiled with JetBrains decompiler
// Type: Nest.TopHitsAggregate
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class TopHitsAggregate : MetricAggregateBase
  {
    private readonly IJsonFormatterResolver _formatterResolver;
    private readonly IList<LazyDocument> _hits;

    public TopHitsAggregate()
    {
    }

    internal TopHitsAggregate(IList<LazyDocument> hits, IJsonFormatterResolver formatterResolver)
    {
      this._hits = hits;
      this._formatterResolver = formatterResolver;
    }

    public double? MaxScore { get; set; }

    public TotalHits Total { get; set; }

    private IEnumerable<IHit<TDocument>> ConvertHits<TDocument>() where TDocument : class
    {
      IJsonFormatter<IHit<TDocument>> formatter = this._formatterResolver.GetFormatter<IHit<TDocument>>();
      return this._hits.Select<LazyDocument, IHit<TDocument>>((Func<LazyDocument, IHit<TDocument>>) (h =>
      {
        JsonReader reader = new JsonReader(h.Bytes);
        return formatter.Deserialize(ref reader, this._formatterResolver);
      }));
    }

    public IReadOnlyCollection<IHit<TDocument>> Hits<TDocument>() where TDocument : class => (IReadOnlyCollection<IHit<TDocument>>) this.ConvertHits<TDocument>().ToList<IHit<TDocument>>().AsReadOnly();

    public IReadOnlyCollection<TDocument> Documents<TDocument>() where TDocument : class => (IReadOnlyCollection<TDocument>) this.ConvertHits<TDocument>().Select<IHit<TDocument>, TDocument>((Func<IHit<TDocument>, TDocument>) (h => h.Source)).ToList<TDocument>().AsReadOnly();
  }
}
