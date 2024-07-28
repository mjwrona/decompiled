// Decompiled with JetBrains decompiler
// Type: Nest.FieldLookupDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class FieldLookupDescriptor<T> : 
    DescriptorBase<FieldLookupDescriptor<T>, IFieldLookup>,
    IFieldLookup
    where T : class
  {
    public FieldLookupDescriptor() => this.Self.Index = (IndexName) FieldLookupDescriptor<T>.ClrType;

    private static Type ClrType => typeof (T);

    Nest.Id IFieldLookup.Id { get; set; }

    IndexName IFieldLookup.Index { get; set; }

    Field IFieldLookup.Path { get; set; }

    Nest.Routing IFieldLookup.Routing { get; set; }

    public FieldLookupDescriptor<T> Index(IndexName index) => this.Assign<IndexName>(index, (Action<IFieldLookup, IndexName>) ((a, v) => a.Index = v));

    public FieldLookupDescriptor<T> Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<IFieldLookup, Nest.Id>) ((a, v) => a.Id = v));

    public FieldLookupDescriptor<T> Path(Field path) => this.Assign<Field>(path, (Action<IFieldLookup, Field>) ((a, v) => a.Path = v));

    public FieldLookupDescriptor<T> Path<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IFieldLookup, Expression<Func<T, TValue>>>) ((a, v) => a.Path = (Field) (Expression) v));

    public FieldLookupDescriptor<T> Routing(Nest.Routing routing) => this.Assign<Nest.Routing>(routing, (Action<IFieldLookup, Nest.Routing>) ((a, v) => a.Routing = v));
  }
}
