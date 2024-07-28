// Decompiled with JetBrains decompiler
// Type: Nest.MultiSearchDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class MultiSearchDescriptor : 
    RequestDescriptorBase<MultiSearchDescriptor, MultiSearchRequestParameters, IMultiSearchRequest>,
    IMultiSearchRequest,
    IRequest<MultiSearchRequestParameters>,
    IRequest
  {
    private IDictionary<string, ISearchRequest> _operations = (IDictionary<string, ISearchRequest>) new Dictionary<string, ISearchRequest>();

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceMultiSearch;

    public MultiSearchDescriptor()
    {
    }

    public MultiSearchDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices IMultiSearchRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public MultiSearchDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IMultiSearchRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public MultiSearchDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IMultiSearchRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public MultiSearchDescriptor AllIndices() => this.Index(Indices.All);

    public MultiSearchDescriptor CcsMinimizeRoundtrips(bool? ccsminimizeroundtrips = true) => this.Qs("ccs_minimize_roundtrips", (object) ccsminimizeroundtrips);

    public MultiSearchDescriptor MaxConcurrentSearches(long? maxconcurrentsearches) => this.Qs("max_concurrent_searches", (object) maxconcurrentsearches);

    public MultiSearchDescriptor MaxConcurrentShardRequests(long? maxconcurrentshardrequests) => this.Qs("max_concurrent_shard_requests", (object) maxconcurrentshardrequests);

    public MultiSearchDescriptor PreFilterShardSize(long? prefiltershardsize) => this.Qs("pre_filter_shard_size", (object) prefiltershardsize);

    public MultiSearchDescriptor SearchType(Elasticsearch.Net.SearchType? searchtype) => this.Qs("search_type", (object) searchtype);

    public MultiSearchDescriptor TotalHitsAsInteger(bool? totalhitsasinteger = true) => this.Qs("rest_total_hits_as_int", (object) totalhitsasinteger);

    public MultiSearchDescriptor TypedKeys(bool? typedkeys = true) => this.Qs("typed_keys", (object) typedkeys);

    protected override sealed void RequestDefaults(MultiSearchRequestParameters parameters)
    {
      this.TypedKeys();
      parameters.CustomResponseBuilder = (CustomResponseBuilderBase) new MultiSearchResponseBuilder((IRequest) this);
    }

    IDictionary<string, ISearchRequest> IMultiSearchRequest.Operations
    {
      get => this._operations;
      set => this._operations = value;
    }

    public MultiSearchDescriptor Search<T>(
      string name,
      Func<SearchDescriptor<T>, ISearchRequest> searchSelector)
      where T : class
    {
      name.ThrowIfNull<string>(nameof (name));
      searchSelector.ThrowIfNull<Func<SearchDescriptor<T>, ISearchRequest>>(nameof (searchSelector));
      ISearchRequest searchRequest = searchSelector(new SearchDescriptor<T>());
      if (searchRequest == null)
        return this;
      this._operations.Add(name, searchRequest);
      return this;
    }

    public MultiSearchDescriptor Search<T>(
      Func<SearchDescriptor<T>, ISearchRequest> searchSelector)
      where T : class
    {
      return this.Search<T>(Guid.NewGuid().ToString(), searchSelector);
    }
  }
}
