// Decompiled with JetBrains decompiler
// Type: Nest.TopHitsAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class TopHitsAggregationDescriptor<T> : 
    MetricAggregationDescriptorBase<TopHitsAggregationDescriptor<T>, ITopHitsAggregation, T>,
    ITopHitsAggregation,
    IMetricAggregation,
    IAggregation
    where T : class
  {
    Fields ITopHitsAggregation.DocValueFields { get; set; }

    bool? ITopHitsAggregation.Explain { get; set; }

    int? ITopHitsAggregation.From { get; set; }

    IHighlight ITopHitsAggregation.Highlight { get; set; }

    IScriptFields ITopHitsAggregation.ScriptFields { get; set; }

    int? ITopHitsAggregation.Size { get; set; }

    IList<ISort> ITopHitsAggregation.Sort { get; set; }

    Union<bool, ISourceFilter> ITopHitsAggregation.Source { get; set; }

    Fields ITopHitsAggregation.StoredFields { get; set; }

    bool? ITopHitsAggregation.TrackScores { get; set; }

    bool? ITopHitsAggregation.Version { get; set; }

    public TopHitsAggregationDescriptor<T> From(int? from) => this.Assign<int?>(from, (Action<ITopHitsAggregation, int?>) ((a, v) => a.From = v));

    public TopHitsAggregationDescriptor<T> Size(int? size) => this.Assign<int?>(size, (Action<ITopHitsAggregation, int?>) ((a, v) => a.Size = v));

    public TopHitsAggregationDescriptor<T> Sort(
      Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortSelector)
    {
      return this.Assign<Func<SortDescriptor<T>, IPromise<IList<ISort>>>>(sortSelector, (Action<ITopHitsAggregation, Func<SortDescriptor<T>, IPromise<IList<ISort>>>>) ((a, v) => a.Sort = v != null ? v(new SortDescriptor<T>())?.Value : (IList<ISort>) null));
    }

    public TopHitsAggregationDescriptor<T> Source(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<ITopHitsAggregation, bool?>) ((a, v) =>
    {
      ITopHitsAggregation topHitsAggregation = a;
      bool? nullable = v;
      Union<bool, ISourceFilter> valueOrDefault = nullable.HasValue ? (Union<bool, ISourceFilter>) nullable.GetValueOrDefault() : (Union<bool, ISourceFilter>) null;
      topHitsAggregation.Source = valueOrDefault;
    }));

    public TopHitsAggregationDescriptor<T> Source(
      Func<SourceFilterDescriptor<T>, ISourceFilter> selector)
    {
      return this.Assign<Func<SourceFilterDescriptor<T>, ISourceFilter>>(selector, (Action<ITopHitsAggregation, Func<SourceFilterDescriptor<T>, ISourceFilter>>) ((a, v) => a.Source = new Union<bool, ISourceFilter>(v != null ? v(new SourceFilterDescriptor<T>()) : (ISourceFilter) null)));
    }

    public TopHitsAggregationDescriptor<T> Highlight(
      Func<HighlightDescriptor<T>, IHighlight> highlightSelector)
    {
      return this.Assign<Func<HighlightDescriptor<T>, IHighlight>>(highlightSelector, (Action<ITopHitsAggregation, Func<HighlightDescriptor<T>, IHighlight>>) ((a, v) => a.Highlight = v != null ? v(new HighlightDescriptor<T>()) : (IHighlight) null));
    }

    public TopHitsAggregationDescriptor<T> Explain(bool? explain = true) => this.Assign<bool?>(explain, (Action<ITopHitsAggregation, bool?>) ((a, v) => a.Explain = v));

    public TopHitsAggregationDescriptor<T> ScriptFields(
      Func<ScriptFieldsDescriptor, IPromise<IScriptFields>> scriptFieldsSelector)
    {
      return this.Assign<Func<ScriptFieldsDescriptor, IPromise<IScriptFields>>>(scriptFieldsSelector, (Action<ITopHitsAggregation, Func<ScriptFieldsDescriptor, IPromise<IScriptFields>>>) ((a, v) => a.ScriptFields = v != null ? v(new ScriptFieldsDescriptor())?.Value : (IScriptFields) null));
    }

    public TopHitsAggregationDescriptor<T> StoredFields(
      Func<FieldsDescriptor<T>, IPromise<Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(fields, (Action<ITopHitsAggregation, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.StoredFields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));
    }

    public TopHitsAggregationDescriptor<T> Version(bool? version = true) => this.Assign<bool?>(version, (Action<ITopHitsAggregation, bool?>) ((a, v) => a.Version = v));

    public TopHitsAggregationDescriptor<T> TrackScores(bool? trackScores = true) => this.Assign<bool?>(trackScores, (Action<ITopHitsAggregation, bool?>) ((a, v) => a.TrackScores = v));

    public TopHitsAggregationDescriptor<T> DocValueFields(
      Func<FieldsDescriptor<T>, IPromise<Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(fields, (Action<ITopHitsAggregation, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.DocValueFields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));
    }

    public TopHitsAggregationDescriptor<T> DocValueFields(Fields fields) => this.Assign<Fields>(fields, (Action<ITopHitsAggregation, Fields>) ((a, v) => a.DocValueFields = v));
  }
}
