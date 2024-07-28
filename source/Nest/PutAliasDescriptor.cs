// Decompiled with JetBrains decompiler
// Type: Nest.PutAliasDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class PutAliasDescriptor : 
    RequestDescriptorBase<PutAliasDescriptor, PutAliasRequestParameters, IPutAliasRequest>,
    IPutAliasRequest,
    IRequest<PutAliasRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesPutAlias;

    public PutAliasDescriptor(Indices index, Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected PutAliasDescriptor()
    {
    }

    Indices IPutAliasRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    Name IPutAliasRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public PutAliasDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IPutAliasRequest, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public PutAliasDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IPutAliasRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public PutAliasDescriptor AllIndices() => this.Index(Indices.All);

    public PutAliasDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public PutAliasDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    QueryContainer IPutAliasRequest.Filter { get; set; }

    Nest.Routing IPutAliasRequest.IndexRouting { get; set; }

    bool? IPutAliasRequest.IsWriteIndex { get; set; }

    Nest.Routing IPutAliasRequest.Routing { get; set; }

    Nest.Routing IPutAliasRequest.SearchRouting { get; set; }

    public PutAliasDescriptor Routing(Nest.Routing routing) => this.Assign<Nest.Routing>(routing, (Action<IPutAliasRequest, Nest.Routing>) ((a, v) => a.Routing = v));

    public PutAliasDescriptor IndexRouting(Nest.Routing routing) => this.Assign<Nest.Routing>(routing, (Action<IPutAliasRequest, Nest.Routing>) ((a, v) => a.IndexRouting = v));

    public PutAliasDescriptor SearchRouting(Nest.Routing routing) => this.Assign<Nest.Routing>(routing, (Action<IPutAliasRequest, Nest.Routing>) ((a, v) => a.SearchRouting = v));

    public PutAliasDescriptor IsWriteIndex(bool? isWriteIndex = true) => this.Assign<bool?>(isWriteIndex, (Action<IPutAliasRequest, bool?>) ((a, v) => a.IsWriteIndex = v));

    public PutAliasDescriptor Filter<T>(
      Func<QueryContainerDescriptor<T>, QueryContainer> filterSelector)
      where T : class
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(filterSelector, (Action<IPutAliasRequest, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Filter = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }
  }
}
