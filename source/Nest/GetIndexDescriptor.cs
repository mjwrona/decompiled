// Decompiled with JetBrains decompiler
// Type: Nest.GetIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class GetIndexDescriptor : 
    RequestDescriptorBase<GetIndexDescriptor, GetIndexRequestParameters, IGetIndexRequest>,
    IGetIndexRequest,
    IRequest<GetIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesGet;

    public GetIndexDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected GetIndexDescriptor()
    {
    }

    Indices IGetIndexRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public GetIndexDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IGetIndexRequest, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public GetIndexDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IGetIndexRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public GetIndexDescriptor AllIndices() => this.Index(Indices.All);

    public GetIndexDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public GetIndexDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public GetIndexDescriptor FlatSettings(bool? flatsettings = true) => this.Qs("flat_settings", (object) flatsettings);

    public GetIndexDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public GetIndexDescriptor IncludeDefaults(bool? includedefaults = true) => this.Qs("include_defaults", (object) includedefaults);

    public GetIndexDescriptor IncludeTypeName(bool? includetypename = true) => this.Qs("include_type_name", (object) includetypename);

    public GetIndexDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public GetIndexDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);
  }
}
