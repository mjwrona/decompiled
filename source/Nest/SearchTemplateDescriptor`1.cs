// Decompiled with JetBrains decompiler
// Type: Nest.SearchTemplateDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class SearchTemplateDescriptor<TDocument> : 
    RequestDescriptorBase<SearchTemplateDescriptor<TDocument>, SearchTemplateRequestParameters, ISearchTemplateRequest>,
    ISearchTemplateRequest,
    IRequest<SearchTemplateRequestParameters>,
    IRequest,
    ITypedSearchRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceSearchTemplate;

    public SearchTemplateDescriptor()
      : this((Indices) typeof (TDocument))
    {
    }

    public SearchTemplateDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices ISearchTemplateRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public SearchTemplateDescriptor<TDocument> Index(Indices index) => this.Assign<Indices>(index, (Action<ISearchTemplateRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public SearchTemplateDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ISearchTemplateRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public SearchTemplateDescriptor<TDocument> AllIndices() => this.Index(Indices.All);

    public SearchTemplateDescriptor<TDocument> AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public SearchTemplateDescriptor<TDocument> CcsMinimizeRoundtrips(bool? ccsminimizeroundtrips = true) => this.Qs("ccs_minimize_roundtrips", (object) ccsminimizeroundtrips);

    public SearchTemplateDescriptor<TDocument> ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public SearchTemplateDescriptor<TDocument> IgnoreThrottled(bool? ignorethrottled = true) => this.Qs("ignore_throttled", (object) ignorethrottled);

    public SearchTemplateDescriptor<TDocument> IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public SearchTemplateDescriptor<TDocument> Preference(string preference) => this.Qs(nameof (preference), (object) preference);

    public SearchTemplateDescriptor<TDocument> Profile(bool? profile = true) => this.Qs(nameof (profile), (object) profile);

    public SearchTemplateDescriptor<TDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public SearchTemplateDescriptor<TDocument> Scroll(Time scroll) => this.Qs(nameof (scroll), (object) scroll);

    public SearchTemplateDescriptor<TDocument> SearchType(Elasticsearch.Net.SearchType? searchtype) => this.Qs("search_type", (object) searchtype);

    public SearchTemplateDescriptor<TDocument> TotalHitsAsInteger(bool? totalhitsasinteger = true) => this.Qs("rest_total_hits_as_int", (object) totalhitsasinteger);

    public SearchTemplateDescriptor<TDocument> TypedKeys(bool? typedkeys = true) => this.Qs("typed_keys", (object) typedkeys);

    Type ITypedSearchRequest.ClrType => typeof (TDocument);

    string ISearchTemplateRequest.Id { get; set; }

    IDictionary<string, object> ISearchTemplateRequest.Params { get; set; }

    string ISearchTemplateRequest.Source { get; set; }

    bool? ISearchTemplateRequest.Explain { get; set; }

    protected override sealed void RequestDefaults(SearchTemplateRequestParameters parameters) => this.TypedKeys();

    public SearchTemplateDescriptor<TDocument> Source(string template) => this.Assign<string>(template, (Action<ISearchTemplateRequest, string>) ((a, v) => a.Source = v));

    public SearchTemplateDescriptor<TDocument> Id(string id) => this.Assign<string>(id, (Action<ISearchTemplateRequest, string>) ((a, v) => a.Id = v));

    public SearchTemplateDescriptor<TDocument> Params(Dictionary<string, object> paramDictionary) => this.Assign<Dictionary<string, object>>(paramDictionary, (Action<ISearchTemplateRequest, Dictionary<string, object>>) ((a, v) => a.Params = (IDictionary<string, object>) v));

    public SearchTemplateDescriptor<TDocument> Params(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> paramDictionary)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(paramDictionary, (Action<ISearchTemplateRequest, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Params = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }

    public SearchTemplateDescriptor<TDocument> Explain(bool? explain = true) => this.Assign<bool?>(explain, (Action<ISearchTemplateRequest, bool?>) ((a, v) => a.Explain = v));
  }
}
