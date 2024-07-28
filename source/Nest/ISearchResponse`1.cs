// Decompiled with JetBrains decompiler
// Type: Nest.ISearchResponse`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;

namespace Nest
{
  public interface ISearchResponse<out TDocument> : IResponse, IElasticsearchResponse where TDocument : class
  {
    AggregateDictionary Aggregations { get; }

    ClusterStatistics Clusters { get; }

    IReadOnlyCollection<TDocument> Documents { get; }

    IReadOnlyCollection<FieldValues> Fields { get; }

    IReadOnlyCollection<IHit<TDocument>> Hits { get; }

    IHitsMetadata<TDocument> HitsMetadata { get; }

    double MaxScore { get; }

    long NumberOfReducePhases { get; }

    string PointInTimeId { get; }

    Profile Profile { get; }

    string ScrollId { get; }

    ShardStatistics Shards { get; }

    ISuggestDictionary<TDocument> Suggest { get; }

    bool TerminatedEarly { get; }

    bool TimedOut { get; }

    long Took { get; }

    long Total { get; }
  }
}
