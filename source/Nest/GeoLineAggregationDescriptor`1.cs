// Decompiled with JetBrains decompiler
// Type: Nest.GeoLineAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class GeoLineAggregationDescriptor<T> : 
    DescriptorBase<GeoLineAggregationDescriptor<T>, IGeoLineAggregation>,
    IGeoLineAggregation,
    IAggregation
    where T : class
  {
    IDictionary<string, object> IAggregation.Meta { get; set; }

    string IAggregation.Name { get; set; }

    GeoLinePoint IGeoLineAggregation.Point { get; set; }

    GeoLineSort IGeoLineAggregation.Sort { get; set; }

    bool? IGeoLineAggregation.IncludeSort { get; set; }

    string IGeoLineAggregation.SortOrder { get; set; }

    int? IGeoLineAggregation.Size { get; set; }

    public GeoLineAggregationDescriptor<T> Point(Field field) => this.Assign<Field>(field, (Action<IGeoLineAggregation, Field>) ((a, v) => a.Point = new GeoLinePoint()
    {
      Field = v
    }));

    public GeoLineAggregationDescriptor<T> Point<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IGeoLineAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Point = new GeoLinePoint()
    {
      Field = (Field) (Expression) v
    }));

    public GeoLineAggregationDescriptor<T> Sort(Field field) => this.Assign<Field>(field, (Action<IGeoLineAggregation, Field>) ((a, v) => a.Sort = new GeoLineSort()
    {
      Field = v
    }));

    public GeoLineAggregationDescriptor<T> Sort<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IGeoLineAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Sort = new GeoLineSort()
    {
      Field = (Field) (Expression) v
    }));

    public GeoLineAggregationDescriptor<T> IncludeSort(bool? includeSort = true) => this.Assign<bool?>(includeSort, (Action<IGeoLineAggregation, bool?>) ((a, v) => a.IncludeSort = v));

    public GeoLineAggregationDescriptor<T> SortOrder(string sortOrder) => this.Assign<string>(sortOrder, (Action<IGeoLineAggregation, string>) ((a, v) => a.SortOrder = v));

    public GeoLineAggregationDescriptor<T> Size(int? size) => this.Assign<int?>(size, (Action<IGeoLineAggregation, int?>) ((a, v) => a.Size = v));
  }
}
