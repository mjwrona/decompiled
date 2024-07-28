// Decompiled with JetBrains decompiler
// Type: Nest.ReindexRequest`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ReindexRequest<TSource, TTarget> : IReindexRequest<TSource, TTarget>
    where TSource : class
    where TTarget : class
  {
    public ReindexRequest(
      IScrollAllRequest scrollSource,
      Func<TSource, TTarget> map,
      Func<IEnumerable<IHitMetadata<TTarget>>, IBulkAllRequest<IHitMetadata<TTarget>>> bulkAllTarget)
    {
      scrollSource.ThrowIfNull<IScrollAllRequest>(nameof (scrollSource), "scrollSource must be set in order to get the source of a Reindex operation");
      bulkAllTarget.ThrowIfNull<Func<IEnumerable<IHitMetadata<TTarget>>, IBulkAllRequest<IHitMetadata<TTarget>>>>(nameof (bulkAllTarget), "bulkAllTarget must set in order to get the target of a Reindex operation");
      map.ThrowIfNull<Func<TSource, TTarget>>(nameof (map), "map must be set to know how to take TSource and transform it into TTarget");
      IReindexRequest<TSource, TTarget> reindexRequest = (IReindexRequest<TSource, TTarget>) this;
      reindexRequest.ScrollAll = scrollSource;
      reindexRequest.BulkAll = bulkAllTarget;
      reindexRequest.Map = map;
    }

    public int? BackPressureFactor { get; set; }

    public ICreateIndexRequest CreateIndexRequest { get; set; }

    public bool OmitIndexCreation { get; set; }

    Func<IEnumerable<IHitMetadata<TTarget>>, IBulkAllRequest<IHitMetadata<TTarget>>> IReindexRequest<TSource, TTarget>.BulkAll { get; set; }

    Func<TSource, TTarget> IReindexRequest<TSource, TTarget>.Map { get; set; }

    IScrollAllRequest IReindexRequest<TSource, TTarget>.ScrollAll { get; set; }
  }
}
