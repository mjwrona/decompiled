// Decompiled with JetBrains decompiler
// Type: Nest.GetIndexSettingsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetIndexSettingsRequest : 
    PlainRequestBase<GetIndexSettingsRequestParameters>,
    IGetIndexSettingsRequest,
    IRequest<GetIndexSettingsRequestParameters>,
    IRequest
  {
    protected IGetIndexSettingsRequest Self => (IGetIndexSettingsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesGetSettings;

    public GetIndexSettingsRequest()
    {
    }

    public GetIndexSettingsRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    public GetIndexSettingsRequest(Indices index, Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index).Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    public GetIndexSettingsRequest(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    [IgnoreDataMember]
    Indices IGetIndexSettingsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    [IgnoreDataMember]
    Names IGetIndexSettingsRequest.Name => this.Self.RouteValues.Get<Names>("name");

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

    public bool? IncludeDefaults
    {
      get => this.Q<bool?>("include_defaults");
      set => this.Q("include_defaults", (object) value);
    }

    public bool? Local
    {
      get => this.Q<bool?>("local");
      set => this.Q("local", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }
  }
}
