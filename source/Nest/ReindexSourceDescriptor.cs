// Decompiled with JetBrains decompiler
// Type: Nest.ReindexSourceDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ReindexSourceDescriptor : 
    DescriptorBase<ReindexSourceDescriptor, IReindexSource>,
    IReindexSource
  {
    Indices IReindexSource.Index { get; set; }

    QueryContainer IReindexSource.Query { get; set; }

    IRemoteSource IReindexSource.Remote { get; set; }

    int? IReindexSource.Size { get; set; }

    ISlicedScroll IReindexSource.Slice { get; set; }

    IList<ISort> IReindexSource.Sort { get; set; }

    Fields IReindexSource.Source { get; set; }

    public ReindexSourceDescriptor Query<T>(
      Func<QueryContainerDescriptor<T>, QueryContainer> querySelector)
      where T : class
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(querySelector, (Action<IReindexSource, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    [Obsolete("Deprecated in 7.6.0. Instead consider using query filtering to find the desired subset of data.")]
    public ReindexSourceDescriptor Sort<T>(
      Func<SortDescriptor<T>, IPromise<IList<ISort>>> selector)
      where T : class
    {
      return this.Assign<Func<SortDescriptor<T>, IPromise<IList<ISort>>>>(selector, (Action<IReindexSource, Func<SortDescriptor<T>, IPromise<IList<ISort>>>>) ((a, v) => a.Sort = v != null ? v(new SortDescriptor<T>())?.Value : (IList<ISort>) null));
    }

    public ReindexSourceDescriptor Remote(
      Func<RemoteSourceDescriptor, IRemoteSource> selector)
    {
      return this.Assign<Func<RemoteSourceDescriptor, IRemoteSource>>(selector, (Action<IReindexSource, Func<RemoteSourceDescriptor, IRemoteSource>>) ((a, v) => a.Remote = v != null ? v(new RemoteSourceDescriptor()) : (IRemoteSource) null));
    }

    public ReindexSourceDescriptor Index(Indices indices) => this.Assign<Indices>(indices, (Action<IReindexSource, Indices>) ((a, v) => a.Index = v));

    public ReindexSourceDescriptor Size(int? size) => this.Assign<int?>(size, (Action<IReindexSource, int?>) ((a, v) => a.Size = v));

    public ReindexSourceDescriptor Slice<T>(
      Func<SlicedScrollDescriptor<T>, ISlicedScroll> selector)
      where T : class
    {
      return this.Assign<Func<SlicedScrollDescriptor<T>, ISlicedScroll>>(selector, (Action<IReindexSource, Func<SlicedScrollDescriptor<T>, ISlicedScroll>>) ((a, v) => a.Slice = v != null ? v(new SlicedScrollDescriptor<T>()) : (ISlicedScroll) null));
    }

    public ReindexSourceDescriptor Source<T>(Func<FieldsDescriptor<T>, IPromise<Fields>> fields) where T : class => this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(fields, (Action<IReindexSource, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.Source = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));
  }
}
