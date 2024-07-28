// Decompiled with JetBrains decompiler
// Type: Nest.RollupSearchDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.RollupApi;
using System;

namespace Nest
{
  public class RollupSearchDescriptor<TDocument> : 
    RequestDescriptorBase<RollupSearchDescriptor<TDocument>, RollupSearchRequestParameters, IRollupSearchRequest>,
    IRollupSearchRequest,
    IRequest<RollupSearchRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.RollupSearch;

    public RollupSearchDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    public RollupSearchDescriptor()
      : this((Indices) typeof (TDocument))
    {
    }

    Indices IRollupSearchRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public RollupSearchDescriptor<TDocument> Index(Indices index) => this.Assign<Indices>(index, (Action<IRollupSearchRequest, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public RollupSearchDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IRollupSearchRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public RollupSearchDescriptor<TDocument> AllIndices() => this.Index(Indices.All);

    public RollupSearchDescriptor<TDocument> TotalHitsAsInteger(bool? totalhitsasinteger = true) => this.Qs("rest_total_hits_as_int", (object) totalhitsasinteger);

    public RollupSearchDescriptor<TDocument> TypedKeys(bool? typedkeys = true) => this.Qs("typed_keys", (object) typedkeys);

    AggregationDictionary IRollupSearchRequest.Aggregations { get; set; }

    QueryContainer IRollupSearchRequest.Query { get; set; }

    int? IRollupSearchRequest.Size { get; set; }

    public RollupSearchDescriptor<TDocument> Size(int? size) => this.Assign<int?>(size, (Action<IRollupSearchRequest, int?>) ((a, v) => a.Size = v));

    public RollupSearchDescriptor<TDocument> Aggregations(
      Func<AggregationContainerDescriptor<TDocument>, IAggregationContainer> aggregationsSelector)
    {
      return this.Assign<AggregationDictionary>(aggregationsSelector(new AggregationContainerDescriptor<TDocument>())?.Aggregations, (Action<IRollupSearchRequest, AggregationDictionary>) ((a, v) => a.Aggregations = v));
    }

    public RollupSearchDescriptor<TDocument> Aggregations(AggregationDictionary aggregations) => this.Assign<AggregationDictionary>(aggregations, (Action<IRollupSearchRequest, AggregationDictionary>) ((a, v) => a.Aggregations = v));

    public RollupSearchDescriptor<TDocument> Query(
      Func<QueryContainerDescriptor<TDocument>, QueryContainer> query)
    {
      return this.Assign<Func<QueryContainerDescriptor<TDocument>, QueryContainer>>(query, (Action<IRollupSearchRequest, Func<QueryContainerDescriptor<TDocument>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<TDocument>()) : (QueryContainer) null));
    }
  }
}
