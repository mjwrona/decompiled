// Decompiled with JetBrains decompiler
// Type: Nest.SortDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class SortDescriptor<T> : DescriptorPromiseBase<SortDescriptor<T>, IList<ISort>> where T : class
  {
    public SortDescriptor()
      : base((IList<ISort>) new List<ISort>())
    {
    }

    public SortDescriptor<T> Ascending<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IList<ISort>, Expression<Func<T, TValue>>>) ((a, v) =>
    {
      IList<ISort> sortList = a;
      sortList.Add((ISort) new FieldSort()
      {
        Field = (Nest.Field) (Expression) v,
        Order = new SortOrder?(SortOrder.Ascending)
      });
    }));

    public SortDescriptor<T> Descending<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IList<ISort>, Expression<Func<T, TValue>>>) ((a, v) =>
    {
      IList<ISort> sortList = a;
      sortList.Add((ISort) new FieldSort()
      {
        Field = (Nest.Field) (Expression) v,
        Order = new SortOrder?(SortOrder.Descending)
      });
    }));

    public SortDescriptor<T> Ascending(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IList<ISort>, Nest.Field>) ((a, v) =>
    {
      IList<ISort> sortList = a;
      sortList.Add((ISort) new FieldSort()
      {
        Field = v,
        Order = new SortOrder?(SortOrder.Ascending)
      });
    }));

    public SortDescriptor<T> Descending(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IList<ISort>, Nest.Field>) ((a, v) =>
    {
      IList<ISort> sortList = a;
      sortList.Add((ISort) new FieldSort()
      {
        Field = v,
        Order = new SortOrder?(SortOrder.Descending)
      });
    }));

    public SortDescriptor<T> Ascending(SortSpecialField field) => this.Assign<string>(field.GetStringValue(), (Action<IList<ISort>, string>) ((a, v) =>
    {
      IList<ISort> sortList = a;
      sortList.Add((ISort) new FieldSort()
      {
        Field = (Nest.Field) v,
        Order = new SortOrder?(SortOrder.Ascending)
      });
    }));

    public SortDescriptor<T> Descending(SortSpecialField field) => this.Assign<string>(field.GetStringValue(), (Action<IList<ISort>, string>) ((a, v) =>
    {
      IList<ISort> sortList = a;
      sortList.Add((ISort) new FieldSort()
      {
        Field = (Nest.Field) v,
        Order = new SortOrder?(SortOrder.Descending)
      });
    }));

    public SortDescriptor<T> Field(
      Func<FieldSortDescriptor<T>, IFieldSort> sortSelector)
    {
      return this.AddSort(sortSelector != null ? (ISort) sortSelector(new FieldSortDescriptor<T>()) : (ISort) null);
    }

    public SortDescriptor<T> Field(Nest.Field field, SortOrder order)
    {
      FieldSort fieldSort = new FieldSort();
      fieldSort.Field = field;
      fieldSort.Order = new SortOrder?(order);
      return this.AddSort((ISort) fieldSort);
    }

    public SortDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field, SortOrder order)
    {
      FieldSort fieldSort = new FieldSort();
      fieldSort.Field = (Nest.Field) (Expression) field;
      fieldSort.Order = new SortOrder?(order);
      return this.AddSort((ISort) fieldSort);
    }

    public SortDescriptor<T> GeoDistance(
      Func<GeoDistanceSortDescriptor<T>, IGeoDistanceSort> sortSelector)
    {
      return this.AddSort(sortSelector != null ? (ISort) sortSelector(new GeoDistanceSortDescriptor<T>()) : (ISort) null);
    }

    public SortDescriptor<T> Script(
      Func<ScriptSortDescriptor<T>, IScriptSort> sortSelector)
    {
      return this.AddSort(sortSelector != null ? (ISort) sortSelector(new ScriptSortDescriptor<T>()) : (ISort) null);
    }

    private SortDescriptor<T> AddSort(ISort sort) => sort != null ? this.Assign<ISort>(sort, (Action<IList<ISort>, ISort>) ((a, v) => a.Add(v))) : this;
  }
}
