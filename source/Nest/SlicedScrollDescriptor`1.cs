// Decompiled with JetBrains decompiler
// Type: Nest.SlicedScrollDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class SlicedScrollDescriptor<T> : 
    DescriptorBase<SlicedScrollDescriptor<T>, ISlicedScroll>,
    ISlicedScroll
    where T : class
  {
    Nest.Field ISlicedScroll.Field { get; set; }

    int? ISlicedScroll.Id { get; set; }

    int? ISlicedScroll.Max { get; set; }

    public SlicedScrollDescriptor<T> Id(int? id) => this.Assign<int?>(id, (Action<ISlicedScroll, int?>) ((a, v) => a.Id = v));

    public SlicedScrollDescriptor<T> Max(int? max) => this.Assign<int?>(max, (Action<ISlicedScroll, int?>) ((a, v) => a.Max = v));

    public SlicedScrollDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ISlicedScroll, Nest.Field>) ((a, v) => a.Field = v));

    public SlicedScrollDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ISlicedScroll, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));
  }
}
