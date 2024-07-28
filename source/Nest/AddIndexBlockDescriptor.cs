// Decompiled with JetBrains decompiler
// Type: Nest.AddIndexBlockDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class AddIndexBlockDescriptor : 
    RequestDescriptorBase<AddIndexBlockDescriptor, AddIndexBlockRequestParameters, IAddIndexBlockRequest>,
    IAddIndexBlockRequest,
    IRequest<AddIndexBlockRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesAddBlock;

    public AddIndexBlockDescriptor(Indices index, IndexBlock block)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (block), (IUrlParameter) block)))
    {
    }

    [SerializationConstructor]
    protected AddIndexBlockDescriptor()
    {
    }

    Indices IAddIndexBlockRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    IndexBlock IAddIndexBlockRequest.Block => this.Self.RouteValues.Get<IndexBlock>("block");

    public AddIndexBlockDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IAddIndexBlockRequest, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public AddIndexBlockDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IAddIndexBlockRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public AddIndexBlockDescriptor AllIndices() => this.Index(Indices.All);

    public AddIndexBlockDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public AddIndexBlockDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public AddIndexBlockDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public AddIndexBlockDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public AddIndexBlockDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
