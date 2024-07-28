// Decompiled with JetBrains decompiler
// Type: Nest.InnerHitsDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class InnerHitsDescriptor<T> : 
    DescriptorBase<InnerHitsDescriptor<T>, IInnerHits>,
    IInnerHits
    where T : class
  {
    IFieldCollapse IInnerHits.Collapse { get; set; }

    Fields IInnerHits.DocValueFields { get; set; }

    bool? IInnerHits.Explain { get; set; }

    int? IInnerHits.From { get; set; }

    IHighlight IInnerHits.Highlight { get; set; }

    bool? IInnerHits.IgnoreUnmapped { get; set; }

    string IInnerHits.Name { get; set; }

    IScriptFields IInnerHits.ScriptFields { get; set; }

    int? IInnerHits.Size { get; set; }

    IList<ISort> IInnerHits.Sort { get; set; }

    Union<bool, ISourceFilter> IInnerHits.Source { get; set; }

    bool? IInnerHits.Version { get; set; }

    public InnerHitsDescriptor<T> From(int? from) => this.Assign<int?>(from, (Action<IInnerHits, int?>) ((a, v) => a.From = v));

    public InnerHitsDescriptor<T> Size(int? size) => this.Assign<int?>(size, (Action<IInnerHits, int?>) ((a, v) => a.Size = v));

    public InnerHitsDescriptor<T> Name(string name) => this.Assign<string>(name, (Action<IInnerHits, string>) ((a, v) => a.Name = v));

    public InnerHitsDescriptor<T> Explain(bool? explain = true) => this.Assign<bool?>(explain, (Action<IInnerHits, bool?>) ((a, v) => a.Explain = v));

    public InnerHitsDescriptor<T> Version(bool? version = true) => this.Assign<bool?>(version, (Action<IInnerHits, bool?>) ((a, v) => a.Version = v));

    public InnerHitsDescriptor<T> Sort(
      Func<SortDescriptor<T>, IPromise<IList<ISort>>> selector)
    {
      return this.Assign<Func<SortDescriptor<T>, IPromise<IList<ISort>>>>(selector, (Action<IInnerHits, Func<SortDescriptor<T>, IPromise<IList<ISort>>>>) ((a, v) => a.Sort = v != null ? v(new SortDescriptor<T>())?.Value : (IList<ISort>) null));
    }

    public InnerHitsDescriptor<T> Highlight(Func<HighlightDescriptor<T>, IHighlight> selector) => this.Assign<Func<HighlightDescriptor<T>, IHighlight>>(selector, (Action<IInnerHits, Func<HighlightDescriptor<T>, IHighlight>>) ((a, v) => a.Highlight = v != null ? v(new HighlightDescriptor<T>()) : (IHighlight) null));

    public InnerHitsDescriptor<T> Source(bool enabled = true) => this.Assign<bool>(enabled, (Action<IInnerHits, bool>) ((a, v) => a.Source = (Union<bool, ISourceFilter>) v));

    public InnerHitsDescriptor<T> Source(
      Func<SourceFilterDescriptor<T>, ISourceFilter> selector)
    {
      return this.Assign<Func<SourceFilterDescriptor<T>, ISourceFilter>>(selector, (Action<IInnerHits, Func<SourceFilterDescriptor<T>, ISourceFilter>>) ((a, v) => a.Source = new Union<bool, ISourceFilter>(v != null ? v(new SourceFilterDescriptor<T>()) : (ISourceFilter) null)));
    }

    public InnerHitsDescriptor<T> ScriptFields(
      Func<ScriptFieldsDescriptor, IPromise<IScriptFields>> selector)
    {
      return this.Assign<Func<ScriptFieldsDescriptor, IPromise<IScriptFields>>>(selector, (Action<IInnerHits, Func<ScriptFieldsDescriptor, IPromise<IScriptFields>>>) ((a, v) => a.ScriptFields = v != null ? v(new ScriptFieldsDescriptor())?.Value : (IScriptFields) null));
    }

    public InnerHitsDescriptor<T> DocValueFields(Func<FieldsDescriptor<T>, IPromise<Fields>> fields) => this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(fields, (Action<IInnerHits, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.DocValueFields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));

    public InnerHitsDescriptor<T> DocValueFields(Fields fields) => this.Assign<Fields>(fields, (Action<IInnerHits, Fields>) ((a, v) => a.DocValueFields = v));

    public InnerHitsDescriptor<T> IgnoreUnmapped(bool? ignoreUnmapped = true) => this.Assign<bool?>(ignoreUnmapped, (Action<IInnerHits, bool?>) ((a, v) => a.IgnoreUnmapped = v));

    public InnerHitsDescriptor<T> Collapse(
      Func<FieldCollapseDescriptor<T>, IFieldCollapse> collapseSelector)
    {
      return this.Assign<Func<FieldCollapseDescriptor<T>, IFieldCollapse>>(collapseSelector, (Action<IInnerHits, Func<FieldCollapseDescriptor<T>, IFieldCollapse>>) ((a, v) => a.Collapse = v != null ? v(new FieldCollapseDescriptor<T>()) : (IFieldCollapse) null));
    }
  }
}
