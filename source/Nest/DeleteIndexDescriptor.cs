// Decompiled with JetBrains decompiler
// Type: Nest.DeleteIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteIndexDescriptor : 
    RequestDescriptorBase<DeleteIndexDescriptor, DeleteIndexRequestParameters, IDeleteIndexRequest>,
    IDeleteIndexRequest,
    IRequest<DeleteIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesDelete;

    public DeleteIndexDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected DeleteIndexDescriptor()
    {
    }

    Indices IDeleteIndexRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public DeleteIndexDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IDeleteIndexRequest, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public DeleteIndexDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IDeleteIndexRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public DeleteIndexDescriptor AllIndices() => this.Index(Indices.All);

    public DeleteIndexDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public DeleteIndexDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public DeleteIndexDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public DeleteIndexDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public DeleteIndexDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
