// Decompiled with JetBrains decompiler
// Type: Nest.UnfreezeIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class UnfreezeIndexDescriptor : 
    RequestDescriptorBase<UnfreezeIndexDescriptor, UnfreezeIndexRequestParameters, IUnfreezeIndexRequest>,
    IUnfreezeIndexRequest,
    IRequest<UnfreezeIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesUnfreeze;

    public UnfreezeIndexDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected UnfreezeIndexDescriptor()
    {
    }

    IndexName IUnfreezeIndexRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public UnfreezeIndexDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IUnfreezeIndexRequest, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public UnfreezeIndexDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IUnfreezeIndexRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public UnfreezeIndexDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public UnfreezeIndexDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public UnfreezeIndexDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public UnfreezeIndexDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public UnfreezeIndexDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public UnfreezeIndexDescriptor WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);
  }
}
