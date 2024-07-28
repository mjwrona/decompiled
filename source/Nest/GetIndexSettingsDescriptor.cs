// Decompiled with JetBrains decompiler
// Type: Nest.GetIndexSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class GetIndexSettingsDescriptor : 
    RequestDescriptorBase<GetIndexSettingsDescriptor, GetIndexSettingsRequestParameters, IGetIndexSettingsRequest>,
    IGetIndexSettingsRequest,
    IRequest<GetIndexSettingsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesGetSettings;

    public GetIndexSettingsDescriptor()
    {
    }

    public GetIndexSettingsDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    public GetIndexSettingsDescriptor(Indices index, Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index).Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    public GetIndexSettingsDescriptor(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    Indices IGetIndexSettingsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    Names IGetIndexSettingsRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public GetIndexSettingsDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IGetIndexSettingsRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public GetIndexSettingsDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IGetIndexSettingsRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public GetIndexSettingsDescriptor AllIndices() => this.Index(Indices.All);

    public GetIndexSettingsDescriptor Name(Names name) => this.Assign<Names>(name, (Action<IGetIndexSettingsRequest, Names>) ((a, v) => a.RouteValues.Optional(nameof (name), (IUrlParameter) v)));

    public GetIndexSettingsDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public GetIndexSettingsDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public GetIndexSettingsDescriptor FlatSettings(bool? flatsettings = true) => this.Qs("flat_settings", (object) flatsettings);

    public GetIndexSettingsDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public GetIndexSettingsDescriptor IncludeDefaults(bool? includedefaults = true) => this.Qs("include_defaults", (object) includedefaults);

    public GetIndexSettingsDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public GetIndexSettingsDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);
  }
}
