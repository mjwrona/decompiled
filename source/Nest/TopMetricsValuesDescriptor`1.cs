// Decompiled with JetBrains decompiler
// Type: Nest.TopMetricsValuesDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class TopMetricsValuesDescriptor<T> : 
    DescriptorPromiseBase<TopMetricsValuesDescriptor<T>, IList<ITopMetricsValue>>
    where T : class
  {
    public TopMetricsValuesDescriptor()
      : base((IList<ITopMetricsValue>) new List<ITopMetricsValue>())
    {
    }

    public TopMetricsValuesDescriptor<T> Field(Nest.Field field) => this.AddTopMetrics((ITopMetricsValue) new TopMetricsValue()
    {
      Field = field
    });

    public TopMetricsValuesDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.AddTopMetrics((ITopMetricsValue) new TopMetricsValue()
    {
      Field = (Nest.Field) (Expression) field
    });

    private TopMetricsValuesDescriptor<T> AddTopMetrics(ITopMetricsValue TopMetrics) => TopMetrics != null ? this.Assign<ITopMetricsValue>(TopMetrics, (Action<IList<ITopMetricsValue>, ITopMetricsValue>) ((a, v) => a.Add(v))) : this;
  }
}
