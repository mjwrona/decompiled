// Decompiled with JetBrains decompiler
// Type: Nest.MultiTermsAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class MultiTermsAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<MultiTermsAggregationDescriptor<T>, IMultiTermsAggregation, T>,
    IMultiTermsAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    TermsAggregationCollectMode? IMultiTermsAggregation.CollectMode { get; set; }

    int? IMultiTermsAggregation.MinimumDocumentCount { get; set; }

    IList<TermsOrder> IMultiTermsAggregation.Order { get; set; }

    IScript IMultiTermsAggregation.Script { get; set; }

    int? IMultiTermsAggregation.ShardMinimumDocumentCount { get; set; }

    int? IMultiTermsAggregation.ShardSize { get; set; }

    bool? IMultiTermsAggregation.ShowTermDocCountError { get; set; }

    int? IMultiTermsAggregation.Size { get; set; }

    IEnumerable<ITerm> IMultiTermsAggregation.Terms { get; set; }

    public MultiTermsAggregationDescriptor<T> CollectMode(TermsAggregationCollectMode? collectMode) => this.Assign<TermsAggregationCollectMode?>(collectMode, (Action<IMultiTermsAggregation, TermsAggregationCollectMode?>) ((a, v) => a.CollectMode = v));

    public MultiTermsAggregationDescriptor<T> MinimumDocumentCount(int? minimumDocumentCount) => this.Assign<int?>(minimumDocumentCount, (Action<IMultiTermsAggregation, int?>) ((a, v) => a.MinimumDocumentCount = v));

    public MultiTermsAggregationDescriptor<T> Order(
      Func<TermsOrderDescriptor<T>, IPromise<IList<TermsOrder>>> selector)
    {
      return this.Assign<Func<TermsOrderDescriptor<T>, IPromise<IList<TermsOrder>>>>(selector, (Action<IMultiTermsAggregation, Func<TermsOrderDescriptor<T>, IPromise<IList<TermsOrder>>>>) ((a, v) => a.Order = v != null ? v(new TermsOrderDescriptor<T>())?.Value : (IList<TermsOrder>) null));
    }

    public MultiTermsAggregationDescriptor<T> Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IMultiTermsAggregation, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public MultiTermsAggregationDescriptor<T> Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IMultiTermsAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));

    public MultiTermsAggregationDescriptor<T> ShardMinimumDocumentCount(
      int? shardMinimumDocumentCount)
    {
      return this.Assign<int?>(shardMinimumDocumentCount, (Action<IMultiTermsAggregation, int?>) ((a, v) => a.ShardMinimumDocumentCount = v));
    }

    public MultiTermsAggregationDescriptor<T> ShardSize(int? shardSize) => this.Assign<int?>(shardSize, (Action<IMultiTermsAggregation, int?>) ((a, v) => a.ShardSize = v));

    public MultiTermsAggregationDescriptor<T> ShowTermDocCountError(bool? showTermDocCountError = true) => this.Assign<bool?>(showTermDocCountError, (Action<IMultiTermsAggregation, bool?>) ((a, v) => a.ShowTermDocCountError = v));

    public MultiTermsAggregationDescriptor<T> Size(int? size) => this.Assign<int?>(size, (Action<IMultiTermsAggregation, int?>) ((a, v) => a.Size = v));

    public MultiTermsAggregationDescriptor<T> Terms(params ITerm[] ranges) => this.Assign<List<ITerm>>(((IEnumerable<ITerm>) ranges).ToListOrNullIfEmpty<ITerm>(), (Action<IMultiTermsAggregation, List<ITerm>>) ((a, v) => a.Terms = (IEnumerable<ITerm>) v));

    public MultiTermsAggregationDescriptor<T> Terms(params Func<TermDescriptor<T>, ITerm>[] ranges) => this.Assign<List<ITerm>>(ranges != null ? ((IEnumerable<Func<TermDescriptor<T>, ITerm>>) ranges).Select<Func<TermDescriptor<T>, ITerm>, ITerm>((Func<Func<TermDescriptor<T>, ITerm>, ITerm>) (r => r(new TermDescriptor<T>()))).ToListOrNullIfEmpty<ITerm>() : (List<ITerm>) null, (Action<IMultiTermsAggregation, List<ITerm>>) ((a, v) => a.Terms = (IEnumerable<ITerm>) v));

    public MultiTermsAggregationDescriptor<T> Terms(
      IEnumerable<Func<TermDescriptor<T>, ITerm>> ranges)
    {
      return this.Assign<List<ITerm>>(ranges != null ? ranges.Select<Func<TermDescriptor<T>, ITerm>, ITerm>((Func<Func<TermDescriptor<T>, ITerm>, ITerm>) (r => r(new TermDescriptor<T>()))).ToListOrNullIfEmpty<ITerm>() : (List<ITerm>) null, (Action<IMultiTermsAggregation, List<ITerm>>) ((a, v) => a.Terms = (IEnumerable<ITerm>) v));
    }
  }
}
