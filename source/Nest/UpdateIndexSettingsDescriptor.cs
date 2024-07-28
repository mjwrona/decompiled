// Decompiled with JetBrains decompiler
// Type: Nest.UpdateIndexSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class UpdateIndexSettingsDescriptor : 
    RequestDescriptorBase<UpdateIndexSettingsDescriptor, UpdateIndexSettingsRequestParameters, IUpdateIndexSettingsRequest>,
    IUpdateIndexSettingsRequest,
    IRequest<UpdateIndexSettingsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesUpdateSettings;

    public UpdateIndexSettingsDescriptor()
    {
    }

    public UpdateIndexSettingsDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices IUpdateIndexSettingsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public UpdateIndexSettingsDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IUpdateIndexSettingsRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public UpdateIndexSettingsDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IUpdateIndexSettingsRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public UpdateIndexSettingsDescriptor AllIndices() => this.Index(Indices.All);

    public UpdateIndexSettingsDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public UpdateIndexSettingsDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public UpdateIndexSettingsDescriptor FlatSettings(bool? flatsettings = true) => this.Qs("flat_settings", (object) flatsettings);

    public UpdateIndexSettingsDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public UpdateIndexSettingsDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public UpdateIndexSettingsDescriptor PreserveExisting(bool? preserveexisting = true) => this.Qs("preserve_existing", (object) preserveexisting);

    public UpdateIndexSettingsDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    IDynamicIndexSettings IUpdateIndexSettingsRequest.IndexSettings { get; set; }

    public UpdateIndexSettingsDescriptor IndexSettings(
      Func<DynamicIndexSettingsDescriptor, IPromise<IDynamicIndexSettings>> settings)
    {
      return this.Assign<Func<DynamicIndexSettingsDescriptor, IPromise<IDynamicIndexSettings>>>(settings, (Action<IUpdateIndexSettingsRequest, Func<DynamicIndexSettingsDescriptor, IPromise<IDynamicIndexSettings>>>) ((a, v) => a.IndexSettings = v != null ? v(new DynamicIndexSettingsDescriptor())?.Value : (IDynamicIndexSettings) null));
    }
  }
}
