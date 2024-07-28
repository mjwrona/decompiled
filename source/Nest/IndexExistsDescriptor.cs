// Decompiled with JetBrains decompiler
// Type: Nest.IndexExistsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class IndexExistsDescriptor : 
    RequestDescriptorBase<IndexExistsDescriptor, IndexExistsRequestParameters, IIndexExistsRequest>,
    IIndexExistsRequest,
    IRequest<IndexExistsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesExists;

    public IndexExistsDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected IndexExistsDescriptor()
    {
    }

    Indices IIndexExistsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public IndexExistsDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IIndexExistsRequest, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public IndexExistsDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IIndexExistsRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public IndexExistsDescriptor AllIndices() => this.Index(Indices.All);

    public IndexExistsDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public IndexExistsDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public IndexExistsDescriptor FlatSettings(bool? flatsettings = true) => this.Qs("flat_settings", (object) flatsettings);

    public IndexExistsDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public IndexExistsDescriptor IncludeDefaults(bool? includedefaults = true) => this.Qs("include_defaults", (object) includedefaults);

    public IndexExistsDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);
  }
}
