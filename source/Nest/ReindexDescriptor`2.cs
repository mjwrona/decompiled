// Decompiled with JetBrains decompiler
// Type: Nest.ReindexDescriptor`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ReindexDescriptor<TSource, TTarget> : 
    DescriptorBase<ReindexDescriptor<TSource, TTarget>, IReindexRequest<TSource, TTarget>>,
    IReindexRequest<TSource, TTarget>
    where TSource : class
    where TTarget : class
  {
    private Func<BulkAllDescriptor<IHitMetadata<TTarget>>, IBulkAllRequest<IHitMetadata<TTarget>>> _createBulkAll;

    public ReindexDescriptor(Func<TSource, TTarget> mapper)
    {
      IReindexRequest<TSource, TTarget> reindexRequest = (IReindexRequest<TSource, TTarget>) this;
      reindexRequest.BulkAll = (Func<IEnumerable<IHitMetadata<TTarget>>, IBulkAllRequest<IHitMetadata<TTarget>>>) (d => this._createBulkAll.InvokeOrDefault<BulkAllDescriptor<IHitMetadata<TTarget>>, IBulkAllRequest<IHitMetadata<TTarget>>>(new BulkAllDescriptor<IHitMetadata<TTarget>>(d)));
      reindexRequest.Map = mapper;
    }

    int? IReindexRequest<TSource, TTarget>.BackPressureFactor { get; set; }

    Func<IEnumerable<IHitMetadata<TTarget>>, IBulkAllRequest<IHitMetadata<TTarget>>> IReindexRequest<TSource, TTarget>.BulkAll { get; set; }

    ICreateIndexRequest IReindexRequest<TSource, TTarget>.CreateIndexRequest { get; set; }

    Func<TSource, TTarget> IReindexRequest<TSource, TTarget>.Map { get; set; }

    bool IReindexRequest<TSource, TTarget>.OmitIndexCreation { get; set; }

    IScrollAllRequest IReindexRequest<TSource, TTarget>.ScrollAll { get; set; }

    public ReindexDescriptor<TSource, TTarget> ScrollAll(
      Time scrollTime,
      int slices,
      Func<ScrollAllDescriptor<TSource>, IScrollAllRequest> selector = null)
    {
      return this.Assign<Func<ScrollAllDescriptor<TSource>, IScrollAllRequest>>(selector, (Action<IReindexRequest<TSource, TTarget>, Func<ScrollAllDescriptor<TSource>, IScrollAllRequest>>) ((a, v) => a.ScrollAll = v.InvokeOrDefault<ScrollAllDescriptor<TSource>, IScrollAllRequest>(new ScrollAllDescriptor<TSource>(scrollTime, slices))));
    }

    public ReindexDescriptor<TSource, TTarget> BackPressureFactor(int? maximum) => this.Assign<int?>(maximum, (Action<IReindexRequest<TSource, TTarget>, int?>) ((a, v) => a.BackPressureFactor = v));

    public ReindexDescriptor<TSource, TTarget> BulkAll(
      Func<BulkAllDescriptor<IHitMetadata<TTarget>>, IBulkAllRequest<IHitMetadata<TTarget>>> selector)
    {
      this._createBulkAll = selector;
      return this;
    }

    public ReindexDescriptor<TSource, TTarget> OmitIndexCreation(bool omit = true) => this.Assign<bool>(omit, (Action<IReindexRequest<TSource, TTarget>, bool>) ((a, v) => a.OmitIndexCreation = v));

    public ReindexDescriptor<TSource, TTarget> CreateIndex(
      Func<CreateIndexDescriptor, ICreateIndexRequest> createIndexSelector)
    {
      return this.Assign<ICreateIndexRequest>(createIndexSelector.InvokeOrDefault<CreateIndexDescriptor, ICreateIndexRequest>(new CreateIndexDescriptor((IndexName) "ignored")), (Action<IReindexRequest<TSource, TTarget>, ICreateIndexRequest>) ((a, v) => a.CreateIndexRequest = v));
    }
  }
}
