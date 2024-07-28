// Decompiled with JetBrains decompiler
// Type: Nest.EnableUserRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class EnableUserRequest : 
    PlainRequestBase<EnableUserRequestParameters>,
    IEnableUserRequest,
    IRequest<EnableUserRequestParameters>,
    IRequest
  {
    protected IEnableUserRequest Self => (IEnableUserRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityEnableUser;

    public EnableUserRequest(Name username)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (username), (IUrlParameter) username)))
    {
    }

    [SerializationConstructor]
    protected EnableUserRequest()
    {
    }

    [IgnoreDataMember]
    Name IEnableUserRequest.Username => this.Self.RouteValues.Get<Name>("username");

    public Elasticsearch.Net.Refresh? Refresh
    {
      get => this.Q<Elasticsearch.Net.Refresh?>("refresh");
      set => this.Q("refresh", (object) value);
    }
  }
}
