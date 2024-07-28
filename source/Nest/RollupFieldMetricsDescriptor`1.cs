// Decompiled with JetBrains decompiler
// Type: Nest.RollupFieldMetricsDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class RollupFieldMetricsDescriptor<T> : 
    DescriptorPromiseBase<RollupFieldMetricsDescriptor<T>, IList<IRollupFieldMetric>>
    where T : class
  {
    public RollupFieldMetricsDescriptor()
      : base((IList<IRollupFieldMetric>) new List<IRollupFieldMetric>())
    {
    }

    public RollupFieldMetricsDescriptor<T> Field<TValue>(
      Expression<Func<T, TValue>> field,
      params RollupMetric[] metrics)
    {
      return this.Assign<RollupFieldMetric>(new RollupFieldMetric()
      {
        Field = (Nest.Field) (Expression) field,
        Metrics = (IEnumerable<RollupMetric>) metrics
      }, (Action<IList<IRollupFieldMetric>, RollupFieldMetric>) ((a, v) => a.Add((IRollupFieldMetric) v)));
    }

    public RollupFieldMetricsDescriptor<T> Field(Nest.Field field, params RollupMetric[] metrics) => this.Assign<RollupFieldMetric>(new RollupFieldMetric()
    {
      Field = field,
      Metrics = (IEnumerable<RollupMetric>) metrics
    }, (Action<IList<IRollupFieldMetric>, RollupFieldMetric>) ((a, v) => a.Add((IRollupFieldMetric) v)));

    public RollupFieldMetricsDescriptor<T> Field<TValue>(
      Expression<Func<T, TValue>> field,
      IEnumerable<RollupMetric> metrics)
    {
      return this.Assign<RollupFieldMetric>(new RollupFieldMetric()
      {
        Field = (Nest.Field) (Expression) field,
        Metrics = metrics
      }, (Action<IList<IRollupFieldMetric>, RollupFieldMetric>) ((a, v) => a.Add((IRollupFieldMetric) v)));
    }

    public RollupFieldMetricsDescriptor<T> Field(Nest.Field field, IEnumerable<RollupMetric> metrics) => this.Assign<RollupFieldMetric>(new RollupFieldMetric()
    {
      Field = field,
      Metrics = metrics
    }, (Action<IList<IRollupFieldMetric>, RollupFieldMetric>) ((a, v) => a.Add((IRollupFieldMetric) v)));
  }
}
