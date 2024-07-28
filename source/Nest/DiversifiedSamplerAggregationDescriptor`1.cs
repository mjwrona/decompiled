// Decompiled with JetBrains decompiler
// Type: Nest.DiversifiedSamplerAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class DiversifiedSamplerAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<DiversifiedSamplerAggregationDescriptor<T>, IDiversifiedSamplerAggregation, T>,
    IDiversifiedSamplerAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    DiversifiedSamplerAggregationExecutionHint? IDiversifiedSamplerAggregation.ExecutionHint { get; set; }

    Nest.Field IDiversifiedSamplerAggregation.Field { get; set; }

    int? IDiversifiedSamplerAggregation.MaxDocsPerValue { get; set; }

    IScript IDiversifiedSamplerAggregation.Script { get; set; }

    int? IDiversifiedSamplerAggregation.ShardSize { get; set; }

    public DiversifiedSamplerAggregationDescriptor<T> ExecutionHint(
      DiversifiedSamplerAggregationExecutionHint? executionHint)
    {
      return this.Assign<DiversifiedSamplerAggregationExecutionHint?>(executionHint, (Action<IDiversifiedSamplerAggregation, DiversifiedSamplerAggregationExecutionHint?>) ((a, v) => a.ExecutionHint = v));
    }

    public DiversifiedSamplerAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IDiversifiedSamplerAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public DiversifiedSamplerAggregationDescriptor<T> Field<TValue>(
      Expression<Func<T, TValue>> field)
    {
      return this.Assign<Expression<Func<T, TValue>>>(field, (Action<IDiversifiedSamplerAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));
    }

    public DiversifiedSamplerAggregationDescriptor<T> MaxDocsPerValue(int? maxDocs) => this.Assign<int?>(maxDocs, (Action<IDiversifiedSamplerAggregation, int?>) ((a, v) => a.MaxDocsPerValue = v));

    public DiversifiedSamplerAggregationDescriptor<T> Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IDiversifiedSamplerAggregation, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public DiversifiedSamplerAggregationDescriptor<T> Script(
      Func<ScriptDescriptor, IScript> scriptSelector)
    {
      return this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IDiversifiedSamplerAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
    }

    public DiversifiedSamplerAggregationDescriptor<T> ShardSize(int? shardSize) => this.Assign<int?>(shardSize, (Action<IDiversifiedSamplerAggregation, int?>) ((a, v) => a.ShardSize = v));
  }
}
