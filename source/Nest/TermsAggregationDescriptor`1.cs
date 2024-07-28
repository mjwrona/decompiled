// Decompiled with JetBrains decompiler
// Type: Nest.TermsAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class TermsAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<TermsAggregationDescriptor<T>, ITermsAggregation, T>,
    ITermsAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    TermsAggregationCollectMode? ITermsAggregation.CollectMode { get; set; }

    TermsExclude ITermsAggregation.Exclude { get; set; }

    TermsAggregationExecutionHint? ITermsAggregation.ExecutionHint { get; set; }

    Nest.Field ITermsAggregation.Field { get; set; }

    TermsInclude ITermsAggregation.Include { get; set; }

    int? ITermsAggregation.MinimumDocumentCount { get; set; }

    object ITermsAggregation.Missing { get; set; }

    IList<TermsOrder> ITermsAggregation.Order { get; set; }

    IScript ITermsAggregation.Script { get; set; }

    int? ITermsAggregation.ShardSize { get; set; }

    bool? ITermsAggregation.ShowTermDocCountError { get; set; }

    int? ITermsAggregation.Size { get; set; }

    public TermsAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ITermsAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public TermsAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<ITermsAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public TermsAggregationDescriptor<T> Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<ITermsAggregation, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public TermsAggregationDescriptor<T> Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<ITermsAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));

    public TermsAggregationDescriptor<T> Size(int? size) => this.Assign<int?>(size, (Action<ITermsAggregation, int?>) ((a, v) => a.Size = v));

    public TermsAggregationDescriptor<T> ShardSize(int? shardSize) => this.Assign<int?>(shardSize, (Action<ITermsAggregation, int?>) ((a, v) => a.ShardSize = v));

    public TermsAggregationDescriptor<T> MinimumDocumentCount(int? minimumDocumentCount) => this.Assign<int?>(minimumDocumentCount, (Action<ITermsAggregation, int?>) ((a, v) => a.MinimumDocumentCount = v));

    public TermsAggregationDescriptor<T> ExecutionHint(TermsAggregationExecutionHint? executionHint) => this.Assign<TermsAggregationExecutionHint?>(executionHint, (Action<ITermsAggregation, TermsAggregationExecutionHint?>) ((a, v) => a.ExecutionHint = v));

    public TermsAggregationDescriptor<T> Order(
      Func<TermsOrderDescriptor<T>, IPromise<IList<TermsOrder>>> selector)
    {
      return this.Assign<Func<TermsOrderDescriptor<T>, IPromise<IList<TermsOrder>>>>(selector, (Action<ITermsAggregation, Func<TermsOrderDescriptor<T>, IPromise<IList<TermsOrder>>>>) ((a, v) => a.Order = v != null ? v(new TermsOrderDescriptor<T>())?.Value : (IList<TermsOrder>) null));
    }

    public TermsAggregationDescriptor<T> Include(long partition, long numberOfPartitions) => this.Assign<TermsInclude>(new TermsInclude(partition, numberOfPartitions), (Action<ITermsAggregation, TermsInclude>) ((a, v) => a.Include = v));

    public TermsAggregationDescriptor<T> Include(string includePattern) => this.Assign<TermsInclude>(new TermsInclude(includePattern), (Action<ITermsAggregation, TermsInclude>) ((a, v) => a.Include = v));

    public TermsAggregationDescriptor<T> Include(IEnumerable<string> values) => this.Assign<TermsInclude>(new TermsInclude(values), (Action<ITermsAggregation, TermsInclude>) ((a, v) => a.Include = v));

    public TermsAggregationDescriptor<T> Exclude(string excludePattern) => this.Assign<TermsExclude>(new TermsExclude(excludePattern), (Action<ITermsAggregation, TermsExclude>) ((a, v) => a.Exclude = v));

    public TermsAggregationDescriptor<T> Exclude(IEnumerable<string> values) => this.Assign<TermsExclude>(new TermsExclude(values), (Action<ITermsAggregation, TermsExclude>) ((a, v) => a.Exclude = v));

    public TermsAggregationDescriptor<T> CollectMode(TermsAggregationCollectMode? collectMode) => this.Assign<TermsAggregationCollectMode?>(collectMode, (Action<ITermsAggregation, TermsAggregationCollectMode?>) ((a, v) => a.CollectMode = v));

    public TermsAggregationDescriptor<T> Missing(object missing) => this.Assign<object>(missing, (Action<ITermsAggregation, object>) ((a, v) => a.Missing = v));

    public TermsAggregationDescriptor<T> ShowTermDocCountError(bool? showTermDocCountError = true) => this.Assign<bool?>(showTermDocCountError, (Action<ITermsAggregation, bool?>) ((a, v) => a.ShowTermDocCountError = v));
  }
}
