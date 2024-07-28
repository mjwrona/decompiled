// Decompiled with JetBrains decompiler
// Type: Nest.FieldsDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class FieldsDescriptor<T> : DescriptorPromiseBase<FieldsDescriptor<T>, Nest.Fields> where T : class
  {
    public FieldsDescriptor()
      : base(new Nest.Fields())
    {
    }

    public FieldsDescriptor<T> Fields(params Expression<Func<T, object>>[] fields) => this.Assign<Expression<Func<T, object>>[]>(fields, (Action<Nest.Fields, Expression<Func<T, object>>[]>) ((f, v) => f.And<T>(v)));

    public FieldsDescriptor<T> Fields(params string[] fields) => this.Assign<string[]>(fields, (Action<Nest.Fields, string[]>) ((f, v) => f.And(v)));

    public FieldsDescriptor<T> Fields(IEnumerable<Nest.Field> fields) => this.Assign<IEnumerable<Nest.Field>>(fields, (Action<Nest.Fields, IEnumerable<Nest.Field>>) ((f, v) => f.ListOfFields.AddRange(v)));

    public FieldsDescriptor<T> Field<TValue>(
      Expression<Func<T, TValue>> field,
      double? boost = null,
      string format = null)
    {
      return this.Assign<Nest.Field>(new Nest.Field((Expression) field, boost, format), (Action<Nest.Fields, Nest.Field>) ((f, v) => f.And(v)));
    }

    public FieldsDescriptor<T> Field(string field, double? boost = null, string format = null) => this.Assign<Nest.Field>(new Nest.Field(field, boost, format), (Action<Nest.Fields, Nest.Field>) ((f, v) => f.And(v)));

    public FieldsDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<Nest.Fields, Nest.Field>) ((f, v) => f.And(v)));
  }
}
