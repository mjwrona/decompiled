// Decompiled with JetBrains decompiler
// Type: Nest.RangeAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class RangeAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<RangeAggregationDescriptor<T>, IRangeAggregation, T>,
    IRangeAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    Nest.Field IRangeAggregation.Field { get; set; }

    IEnumerable<IAggregationRange> IRangeAggregation.Ranges { get; set; }

    IScript IRangeAggregation.Script { get; set; }

    public RangeAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IRangeAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public RangeAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IRangeAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public RangeAggregationDescriptor<T> Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IRangeAggregation, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public RangeAggregationDescriptor<T> Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IRangeAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));

    public RangeAggregationDescriptor<T> Ranges(
      params Func<AggregationRangeDescriptor, IAggregationRange>[] ranges)
    {
      return this.Assign<IEnumerable<IAggregationRange>>(((IEnumerable<Func<AggregationRangeDescriptor, IAggregationRange>>) ranges).Select<Func<AggregationRangeDescriptor, IAggregationRange>, IAggregationRange>((Func<Func<AggregationRangeDescriptor, IAggregationRange>, IAggregationRange>) (r => r(new AggregationRangeDescriptor()))), (Action<IRangeAggregation, IEnumerable<IAggregationRange>>) ((a, v) => a.Ranges = v));
    }
  }
}
