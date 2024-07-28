// Decompiled with JetBrains decompiler
// Type: Nest.CompositeAggregationSourceDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public abstract class CompositeAggregationSourceDescriptorBase<TDescriptor, TInterface, T> : 
    DescriptorBase<TDescriptor, TInterface>,
    ICompositeAggregationSource
    where TDescriptor : CompositeAggregationSourceDescriptorBase<TDescriptor, TInterface, T>, TInterface
    where TInterface : class, ICompositeAggregationSource
  {
    private readonly string _sourceType;

    protected CompositeAggregationSourceDescriptorBase(string name, string sourceType)
    {
      this._sourceType = sourceType;
      this.Self.Name = name;
    }

    Nest.Field ICompositeAggregationSource.Field { get; set; }

    bool? ICompositeAggregationSource.MissingBucket { get; set; }

    string ICompositeAggregationSource.Name { get; set; }

    SortOrder? ICompositeAggregationSource.Order { get; set; }

    string ICompositeAggregationSource.SourceType => this._sourceType;

    public TDescriptor Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<TInterface, Nest.Field>) ((a, v) => a.Field = v));

    public TDescriptor Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<TInterface, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public TDescriptor Order(SortOrder? order) => this.Assign<SortOrder?>(order, (Action<TInterface, SortOrder?>) ((a, v) => a.Order = v));

    public TDescriptor MissingBucket(bool? includeMissing = true) => this.Assign<bool?>(includeMissing, (Action<TInterface, bool?>) ((a, v) => a.MissingBucket = v));
  }
}
