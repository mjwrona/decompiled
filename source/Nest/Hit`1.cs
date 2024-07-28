// Decompiled with JetBrains decompiler
// Type: Nest.Hit`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;

namespace Nest
{
  public class Hit<TDocument> : IHit<TDocument>, IHitMetadata<TDocument> where TDocument : class
  {
    public Explanation Explanation { get; internal set; }

    public FieldValues Fields { get; internal set; }

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Highlight { get; internal set; } = EmptyReadOnly<string, IReadOnlyCollection<string>>.Dictionary;

    public string Id { get; internal set; }

    public string Index { get; internal set; }

    public IReadOnlyDictionary<string, InnerHitsResult> InnerHits { get; internal set; } = EmptyReadOnly<string, InnerHitsResult>.Dictionary;

    public IReadOnlyCollection<string> MatchedQueries { get; internal set; } = EmptyReadOnly<string>.Collection;

    public NestedIdentity Nested { get; internal set; }

    public long? PrimaryTerm { get; internal set; }

    public string Routing { get; internal set; }

    public double? Score { get; set; }

    public long? SequenceNumber { get; internal set; }

    public IReadOnlyCollection<object> Sorts { get; internal set; } = EmptyReadOnly<object>.Collection;

    public TDocument Source { get; internal set; }

    public string Type { get; internal set; }

    public long Version { get; internal set; }
  }
}
