// Decompiled with JetBrains decompiler
// Type: Nest.ExistsQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class ExistsQueryDescriptor<T> : 
    QueryDescriptorBase<ExistsQueryDescriptor<T>, IExistsQuery>,
    IExistsQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => ExistsQuery.IsConditionless((IExistsQuery) this);

    Nest.Field IExistsQuery.Field { get; set; }

    public ExistsQueryDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IExistsQuery, Nest.Field>) ((a, v) => a.Field = v));

    public ExistsQueryDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IExistsQuery, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));
  }
}
