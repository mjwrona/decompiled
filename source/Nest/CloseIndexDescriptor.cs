// Decompiled with JetBrains decompiler
// Type: Nest.CloseIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class CloseIndexDescriptor : 
    RequestDescriptorBase<CloseIndexDescriptor, CloseIndexRequestParameters, ICloseIndexRequest>,
    ICloseIndexRequest,
    IRequest<CloseIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesClose;

    public CloseIndexDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected CloseIndexDescriptor()
    {
    }

    Indices ICloseIndexRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public CloseIndexDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<ICloseIndexRequest, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public CloseIndexDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ICloseIndexRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public CloseIndexDescriptor AllIndices() => this.Index(Indices.All);

    public CloseIndexDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public CloseIndexDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public CloseIndexDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public CloseIndexDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CloseIndexDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public CloseIndexDescriptor WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);
  }
}
