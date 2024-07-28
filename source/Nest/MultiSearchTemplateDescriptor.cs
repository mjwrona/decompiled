// Decompiled with JetBrains decompiler
// Type: Nest.MultiSearchTemplateDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class MultiSearchTemplateDescriptor : 
    RequestDescriptorBase<MultiSearchTemplateDescriptor, MultiSearchTemplateRequestParameters, IMultiSearchTemplateRequest>,
    IMultiSearchTemplateRequest,
    IRequest<MultiSearchTemplateRequestParameters>,
    IRequest
  {
    private IDictionary<string, ISearchTemplateRequest> _operations = (IDictionary<string, ISearchTemplateRequest>) new Dictionary<string, ISearchTemplateRequest>();

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceMultiSearchTemplate;

    public MultiSearchTemplateDescriptor()
    {
    }

    public MultiSearchTemplateDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices IMultiSearchTemplateRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public MultiSearchTemplateDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IMultiSearchTemplateRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public MultiSearchTemplateDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IMultiSearchTemplateRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public MultiSearchTemplateDescriptor AllIndices() => this.Index(Indices.All);

    public MultiSearchTemplateDescriptor CcsMinimizeRoundtrips(bool? ccsminimizeroundtrips = true) => this.Qs("ccs_minimize_roundtrips", (object) ccsminimizeroundtrips);

    public MultiSearchTemplateDescriptor MaxConcurrentSearches(long? maxconcurrentsearches) => this.Qs("max_concurrent_searches", (object) maxconcurrentsearches);

    public MultiSearchTemplateDescriptor SearchType(Elasticsearch.Net.SearchType? searchtype) => this.Qs("search_type", (object) searchtype);

    public MultiSearchTemplateDescriptor TotalHitsAsInteger(bool? totalhitsasinteger = true) => this.Qs("rest_total_hits_as_int", (object) totalhitsasinteger);

    public MultiSearchTemplateDescriptor TypedKeys(bool? typedkeys = true) => this.Qs("typed_keys", (object) typedkeys);

    protected override sealed void RequestDefaults(MultiSearchTemplateRequestParameters parameters)
    {
      this.TypedKeys();
      parameters.CustomResponseBuilder = (CustomResponseBuilderBase) new MultiSearchResponseBuilder((IRequest) this);
    }

    IDictionary<string, ISearchTemplateRequest> IMultiSearchTemplateRequest.Operations
    {
      get => this._operations;
      set => this._operations = value;
    }

    public MultiSearchTemplateDescriptor Template<T>(
      string name,
      Func<SearchTemplateDescriptor<T>, ISearchTemplateRequest> selector)
      where T : class
    {
      name.ThrowIfNull<string>(nameof (name));
      selector.ThrowIfNull<Func<SearchTemplateDescriptor<T>, ISearchTemplateRequest>>(nameof (selector));
      ISearchTemplateRequest searchTemplateRequest = selector(new SearchTemplateDescriptor<T>());
      if (searchTemplateRequest == null)
        return this;
      this._operations.Add(name, searchTemplateRequest);
      return this;
    }

    public MultiSearchTemplateDescriptor Template<T>(
      Func<SearchTemplateDescriptor<T>, ISearchTemplateRequest> selector)
      where T : class
    {
      return this.Template<T>(Guid.NewGuid().ToString(), selector);
    }
  }
}
