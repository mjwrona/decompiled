// Decompiled with JetBrains decompiler
// Type: Nest.PutUserRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class PutUserRequest : 
    PlainRequestBase<PutUserRequestParameters>,
    IPutUserRequest,
    IRequest<PutUserRequestParameters>,
    IRequest
  {
    protected IPutUserRequest Self => (IPutUserRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityPutUser;

    public PutUserRequest(Name username)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (username), (IUrlParameter) username)))
    {
    }

    [SerializationConstructor]
    protected PutUserRequest()
    {
    }

    [IgnoreDataMember]
    Name IPutUserRequest.Username => this.Self.RouteValues.Get<Name>("username");

    public Elasticsearch.Net.Refresh? Refresh
    {
      get => this.Q<Elasticsearch.Net.Refresh?>("refresh");
      set => this.Q("refresh", (object) value);
    }

    public string Email { get; set; }

    public string FullName { get; set; }

    public IDictionary<string, object> Metadata { get; set; }

    public string Password { get; set; }

    public string PasswordHash { get; set; }

    public IEnumerable<string> Roles { get; set; }
  }
}
