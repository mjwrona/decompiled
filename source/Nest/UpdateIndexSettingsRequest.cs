// Decompiled with JetBrains decompiler
// Type: Nest.UpdateIndexSettingsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class UpdateIndexSettingsRequest : 
    PlainRequestBase<UpdateIndexSettingsRequestParameters>,
    IUpdateIndexSettingsRequest,
    IRequest<UpdateIndexSettingsRequestParameters>,
    IRequest
  {
    public IDynamicIndexSettings IndexSettings { get; set; }

    protected IUpdateIndexSettingsRequest Self => (IUpdateIndexSettingsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesUpdateSettings;

    public UpdateIndexSettingsRequest()
    {
    }

    public UpdateIndexSettingsRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices IUpdateIndexSettingsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public bool? AllowNoIndices
    {
      get => this.Q<bool?>("allow_no_indices");
      set => this.Q("allow_no_indices", (object) value);
    }

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }

    public bool? FlatSettings
    {
      get => this.Q<bool?>("flat_settings");
      set => this.Q("flat_settings", (object) value);
    }

    public bool? IgnoreUnavailable
    {
      get => this.Q<bool?>("ignore_unavailable");
      set => this.Q("ignore_unavailable", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public bool? PreserveExisting
    {
      get => this.Q<bool?>("preserve_existing");
      set => this.Q("preserve_existing", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
