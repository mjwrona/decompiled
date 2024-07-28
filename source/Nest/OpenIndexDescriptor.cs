// Decompiled with JetBrains decompiler
// Type: Nest.OpenIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class OpenIndexDescriptor : 
    RequestDescriptorBase<OpenIndexDescriptor, OpenIndexRequestParameters, IOpenIndexRequest>,
    IOpenIndexRequest,
    IRequest<OpenIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesOpen;

    public OpenIndexDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected OpenIndexDescriptor()
    {
    }

    Indices IOpenIndexRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public OpenIndexDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IOpenIndexRequest, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public OpenIndexDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IOpenIndexRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public OpenIndexDescriptor AllIndices() => this.Index(Indices.All);

    public OpenIndexDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public OpenIndexDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public OpenIndexDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public OpenIndexDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public OpenIndexDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public OpenIndexDescriptor WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);
  }
}
