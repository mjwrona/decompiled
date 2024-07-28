// Decompiled with JetBrains decompiler
// Type: Nest.FieldCollapseDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class FieldCollapseDescriptor<T> : 
    DescriptorBase<FieldCollapseDescriptor<T>, IFieldCollapse>,
    IFieldCollapse
    where T : class
  {
    Nest.Field IFieldCollapse.Field { get; set; }

    IInnerHits IFieldCollapse.InnerHits { get; set; }

    int? IFieldCollapse.MaxConcurrentGroupSearches { get; set; }

    public FieldCollapseDescriptor<T> MaxConcurrentGroupSearches(int? maxConcurrentGroupSearches) => this.Assign<int?>(maxConcurrentGroupSearches, (Action<IFieldCollapse, int?>) ((a, v) => a.MaxConcurrentGroupSearches = v));

    public FieldCollapseDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IFieldCollapse, Nest.Field>) ((a, v) => a.Field = v));

    public FieldCollapseDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IFieldCollapse, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public FieldCollapseDescriptor<T> InnerHits(Func<InnerHitsDescriptor<T>, IInnerHits> selector = null) => this.Assign<IInnerHits>(selector.InvokeOrDefault<InnerHitsDescriptor<T>, IInnerHits>(new InnerHitsDescriptor<T>()), (Action<IFieldCollapse, IInnerHits>) ((a, v) => a.InnerHits = v));
  }
}
