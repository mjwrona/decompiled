// Decompiled with JetBrains decompiler
// Type: Nest.TransformSourceDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TransformSourceDescriptor<T> : 
    DescriptorBase<TransformSourceDescriptor<T>, ITransformSource>,
    ITransformSource
    where T : class
  {
    Indices ITransformSource.Index { get; set; }

    QueryContainer ITransformSource.Query { get; set; }

    IRuntimeFields ITransformSource.RuntimeFields { get; set; }

    public TransformSourceDescriptor<T> Index(Indices indices) => this.Assign<Indices>(indices, (Action<ITransformSource, Indices>) ((a, v) => a.Index = v));

    public TransformSourceDescriptor<T> Index<TOther>() => this.Assign<Type>(typeof (TOther), (Action<ITransformSource, Type>) ((a, v) => a.Index = (Indices) v));

    public TransformSourceDescriptor<T> Query(
      Func<QueryContainerDescriptor<T>, QueryContainer> selector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(selector, (Action<ITransformSource, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public TransformSourceDescriptor<T> RuntimeFields(
      Func<RuntimeFieldsDescriptor<T>, IPromise<IRuntimeFields>> runtimeFieldsSelector)
    {
      return this.Assign<Func<RuntimeFieldsDescriptor<T>, IPromise<IRuntimeFields>>>(runtimeFieldsSelector, (Action<ITransformSource, Func<RuntimeFieldsDescriptor<T>, IPromise<IRuntimeFields>>>) ((a, v) => a.RuntimeFields = v != null ? v(new RuntimeFieldsDescriptor<T>())?.Value : (IRuntimeFields) null));
    }

    public TransformSourceDescriptor<T> RuntimeFields<TSource>(
      Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>> runtimeFieldsSelector)
      where TSource : class
    {
      return this.Assign<Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>>>(runtimeFieldsSelector, (Action<ITransformSource, Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>>>) ((a, v) => a.RuntimeFields = v != null ? v(new RuntimeFieldsDescriptor<TSource>())?.Value : (IRuntimeFields) null));
    }
  }
}
