// Decompiled with JetBrains decompiler
// Type: Nest.ReloadSearchAnalyzersDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ReloadSearchAnalyzersDescriptor : 
    RequestDescriptorBase<ReloadSearchAnalyzersDescriptor, ReloadSearchAnalyzersRequestParameters, IReloadSearchAnalyzersRequest>,
    IReloadSearchAnalyzersRequest,
    IRequest<ReloadSearchAnalyzersRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesReloadSearchAnalyzers;

    public ReloadSearchAnalyzersDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected ReloadSearchAnalyzersDescriptor()
    {
    }

    Indices IReloadSearchAnalyzersRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public ReloadSearchAnalyzersDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IReloadSearchAnalyzersRequest, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public ReloadSearchAnalyzersDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IReloadSearchAnalyzersRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public ReloadSearchAnalyzersDescriptor AllIndices() => this.Index(Indices.All);

    public ReloadSearchAnalyzersDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public ReloadSearchAnalyzersDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public ReloadSearchAnalyzersDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);
  }
}
