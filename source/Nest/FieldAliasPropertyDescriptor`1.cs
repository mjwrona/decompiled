// Decompiled with JetBrains decompiler
// Type: Nest.FieldAliasPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class FieldAliasPropertyDescriptor<T> : 
    PropertyDescriptorBase<FieldAliasPropertyDescriptor<T>, IFieldAliasProperty, T>,
    IFieldAliasProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public FieldAliasPropertyDescriptor()
      : base(FieldType.Alias)
    {
    }

    Field IFieldAliasProperty.Path { get; set; }

    public FieldAliasPropertyDescriptor<T> Path<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IFieldAliasProperty, Expression<Func<T, TValue>>>) ((a, v) => a.Path = (Field) (Expression) v));

    public FieldAliasPropertyDescriptor<T> Path(Field field) => this.Assign<Field>(field, (Action<IFieldAliasProperty, Field>) ((a, v) => a.Path = v));
  }
}
