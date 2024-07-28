// Decompiled with JetBrains decompiler
// Type: Nest.OpenPointInTimeRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class OpenPointInTimeRequest : 
    PlainRequestBase<OpenPointInTimeRequestParameters>,
    IOpenPointInTimeRequest,
    IRequest<OpenPointInTimeRequestParameters>,
    IRequest
  {
    protected IOpenPointInTimeRequest Self => (IOpenPointInTimeRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceOpenPointInTime;

    public OpenPointInTimeRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected OpenPointInTimeRequest()
    {
    }

    [IgnoreDataMember]
    Indices IOpenPointInTimeRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }

    public bool? IgnoreUnavailable
    {
      get => this.Q<bool?>("ignore_unavailable");
      set => this.Q("ignore_unavailable", (object) value);
    }

    public string KeepAlive
    {
      get => this.Q<string>("keep_alive");
      set => this.Q("keep_alive", (object) value);
    }

    public string Preference
    {
      get => this.Q<string>("preference");
      set => this.Q("preference", (object) value);
    }

    public Routing Routing
    {
      get => this.Q<Routing>("routing");
      set => this.Q("routing", (object) value);
    }
  }
}
