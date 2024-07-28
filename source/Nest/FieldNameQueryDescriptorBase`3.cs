// Decompiled with JetBrains decompiler
// Type: Nest.FieldNameQueryDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public abstract class FieldNameQueryDescriptorBase<TDescriptor, TInterface, T> : 
    QueryDescriptorBase<TDescriptor, TInterface>,
    IFieldNameQuery,
    IQuery
    where TDescriptor : FieldNameQueryDescriptorBase<TDescriptor, TInterface, T>, TInterface
    where TInterface : class, IFieldNameQuery
    where T : class
  {
    Nest.Field IFieldNameQuery.Field { get; set; }

    bool IQuery.IsStrict { get; set; }

    bool IQuery.IsVerbatim { get; set; }

    public TDescriptor Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<TInterface, Nest.Field>) ((a, v) => a.Field = v));

    public TDescriptor Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<TInterface, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));
  }
}
