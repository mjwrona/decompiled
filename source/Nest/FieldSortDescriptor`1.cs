// Decompiled with JetBrains decompiler
// Type: Nest.FieldSortDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class FieldSortDescriptor<T> : 
    SortDescriptorBase<FieldSortDescriptor<T>, IFieldSort, T>,
    IFieldSort,
    ISort
    where T : class
  {
    private Nest.Field _field;

    protected override Nest.Field SortKey => this._field;

    bool? IFieldSort.IgnoreUnmappedFields { get; set; }

    FieldType? IFieldSort.UnmappedType { get; set; }

    public virtual FieldSortDescriptor<T> Field(Nest.Field field)
    {
      this._field = field;
      return this;
    }

    public virtual FieldSortDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath)
    {
      this._field = (Nest.Field) (Expression) objectPath;
      return this;
    }

    public virtual FieldSortDescriptor<T> UnmappedType(FieldType? type) => this.Assign<FieldType?>(type, (Action<IFieldSort, FieldType?>) ((a, v) => a.UnmappedType = v));

    public virtual FieldSortDescriptor<T> IgnoreUnmappedFields(bool? ignore = true) => this.Assign<bool?>(ignore, (Action<IFieldSort, bool?>) ((a, v) => a.IgnoreUnmappedFields = v));
  }
}
