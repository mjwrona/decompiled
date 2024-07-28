// Decompiled with JetBrains decompiler
// Type: Nest.DeleteAliasDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteAliasDescriptor : 
    RequestDescriptorBase<DeleteAliasDescriptor, DeleteAliasRequestParameters, IDeleteAliasRequest>,
    IDeleteAliasRequest,
    IRequest<DeleteAliasRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesDeleteAlias;

    public DeleteAliasDescriptor(Indices index, Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected DeleteAliasDescriptor()
    {
    }

    Indices IDeleteAliasRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    Names IDeleteAliasRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public DeleteAliasDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IDeleteAliasRequest, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public DeleteAliasDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IDeleteAliasRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public DeleteAliasDescriptor AllIndices() => this.Index(Indices.All);

    public DeleteAliasDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public DeleteAliasDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
