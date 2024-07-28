// Decompiled with JetBrains decompiler
// Type: Nest.TermDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class TermDescriptor<T> : DescriptorBase<TermDescriptor<T>, ITerm>, ITerm where T : class
  {
    Nest.Field ITerm.Field { get; set; }

    object ITerm.Missing { get; set; }

    public TermDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ITerm, Nest.Field>) ((a, v) => a.Field = v));

    public TermDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<ITerm, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public TermDescriptor<T> Missing(object missing) => this.Assign<object>(missing, (Action<ITerm, object>) ((a, v) => a.Missing = v));
  }
}
