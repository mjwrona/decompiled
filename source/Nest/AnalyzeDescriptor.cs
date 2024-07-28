// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzeDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class AnalyzeDescriptor : 
    RequestDescriptorBase<AnalyzeDescriptor, AnalyzeRequestParameters, IAnalyzeRequest>,
    IAnalyzeRequest,
    IRequest<AnalyzeRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesAnalyze;

    public AnalyzeDescriptor()
    {
    }

    public AnalyzeDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    IndexName IAnalyzeRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public AnalyzeDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IAnalyzeRequest, IndexName>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public AnalyzeDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IAnalyzeRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (IndexName) v)));

    string IAnalyzeRequest.Analyzer { get; set; }

    IEnumerable<string> IAnalyzeRequest.Attributes { get; set; }

    AnalyzeCharFilters IAnalyzeRequest.CharFilter { get; set; }

    bool? IAnalyzeRequest.Explain { get; set; }

    Nest.Field IAnalyzeRequest.Field { get; set; }

    AnalyzeTokenFilters IAnalyzeRequest.Filter { get; set; }

    string IAnalyzeRequest.Normalizer { get; set; }

    IEnumerable<string> IAnalyzeRequest.Text { get; set; }

    Union<string, ITokenizer> IAnalyzeRequest.Tokenizer { get; set; }

    public AnalyzeDescriptor Tokenizer(string tokenizer) => this.Assign<string>(tokenizer, (Action<IAnalyzeRequest, string>) ((a, v) => a.Tokenizer = (Union<string, ITokenizer>) v));

    public AnalyzeDescriptor Tokenizer(
      Func<AnalyzeTokenizersSelector, ITokenizer> tokenizer)
    {
      return this.Assign<Func<AnalyzeTokenizersSelector, ITokenizer>>(tokenizer, (Action<IAnalyzeRequest, Func<AnalyzeTokenizersSelector, ITokenizer>>) ((a, v) =>
      {
        ITokenizer tokenizer1 = v != null ? v(new AnalyzeTokenizersSelector()) : (ITokenizer) null;
        if (tokenizer1 == null)
          return;
        a.Tokenizer = new Union<string, ITokenizer>(tokenizer1);
      }));
    }

    public AnalyzeDescriptor Analyzer(string analyser) => this.Assign<string>(analyser, (Action<IAnalyzeRequest, string>) ((a, v) => a.Analyzer = v));

    public AnalyzeDescriptor CharFilter(params string[] charFilter) => this.Assign<string[]>(charFilter, (Action<IAnalyzeRequest, string[]>) ((a, v) => a.CharFilter = (AnalyzeCharFilters) v));

    public AnalyzeDescriptor CharFilter(IEnumerable<string> charFilter) => this.Assign<string[]>(charFilter.ToArray<string>(), (Action<IAnalyzeRequest, string[]>) ((a, v) => a.CharFilter = (AnalyzeCharFilters) v));

    public AnalyzeDescriptor CharFilter(
      Func<AnalyzeCharFiltersDescriptor, IPromise<AnalyzeCharFilters>> charFilters)
    {
      return this.Assign<Func<AnalyzeCharFiltersDescriptor, IPromise<AnalyzeCharFilters>>>(charFilters, (Action<IAnalyzeRequest, Func<AnalyzeCharFiltersDescriptor, IPromise<AnalyzeCharFilters>>>) ((a, v) => a.CharFilter = v != null ? v(new AnalyzeCharFiltersDescriptor())?.Value : (AnalyzeCharFilters) null));
    }

    public AnalyzeDescriptor Filter(params string[] filter) => this.Assign<string[]>(filter, (Action<IAnalyzeRequest, string[]>) ((a, v) => a.Filter = (AnalyzeTokenFilters) v));

    public AnalyzeDescriptor Filter(IEnumerable<string> filter) => this.Assign<string[]>(filter.ToArray<string>(), (Action<IAnalyzeRequest, string[]>) ((a, v) => a.Filter = (AnalyzeTokenFilters) v));

    public AnalyzeDescriptor Filter(
      Func<AnalyzeTokenFiltersDescriptor, IPromise<AnalyzeTokenFilters>> tokenFilters)
    {
      return this.Assign<Func<AnalyzeTokenFiltersDescriptor, IPromise<AnalyzeTokenFilters>>>(tokenFilters, (Action<IAnalyzeRequest, Func<AnalyzeTokenFiltersDescriptor, IPromise<AnalyzeTokenFilters>>>) ((a, v) => a.Filter = v != null ? v(new AnalyzeTokenFiltersDescriptor())?.Value : (AnalyzeTokenFilters) null));
    }

    public AnalyzeDescriptor Normalizer(string normalizer) => this.Assign<string>(normalizer, (Action<IAnalyzeRequest, string>) ((a, v) => a.Normalizer = v));

    public AnalyzeDescriptor Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IAnalyzeRequest, Nest.Field>) ((a, v) => a.Field = v));

    public AnalyzeDescriptor Field<T, TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IAnalyzeRequest, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public AnalyzeDescriptor Field<T>(Expression<Func<T, object>> field) => this.Assign<Expression<Func<T, object>>>(field, (Action<IAnalyzeRequest, Expression<Func<T, object>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public AnalyzeDescriptor Text(params string[] text) => this.Assign<string[]>(text, (Action<IAnalyzeRequest, string[]>) ((a, v) => a.Text = (IEnumerable<string>) v));

    public AnalyzeDescriptor Text(IEnumerable<string> text) => this.Assign<IEnumerable<string>>(text, (Action<IAnalyzeRequest, IEnumerable<string>>) ((a, v) => a.Text = v));

    public AnalyzeDescriptor Explain(bool? explain = true) => this.Assign<bool?>(explain, (Action<IAnalyzeRequest, bool?>) ((a, v) => a.Explain = v));

    public AnalyzeDescriptor Attributes(params string[] attributes) => this.Assign<string[]>(attributes, (Action<IAnalyzeRequest, string[]>) ((a, v) => a.Attributes = (IEnumerable<string>) v));

    public AnalyzeDescriptor Attributes(IEnumerable<string> attributes) => this.Assign<string[]>(attributes.ToArray<string>(), (Action<IAnalyzeRequest, string[]>) ((a, v) => a.Attributes = (IEnumerable<string>) v));
  }
}
