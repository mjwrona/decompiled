// Decompiled with JetBrains decompiler
// Type: Nest.ChangePasswordRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ChangePasswordRequest : 
    PlainRequestBase<ChangePasswordRequestParameters>,
    IChangePasswordRequest,
    IRequest<ChangePasswordRequestParameters>,
    IRequest
  {
    protected IChangePasswordRequest Self => (IChangePasswordRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityChangePassword;

    public ChangePasswordRequest(Name username)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (username), (IUrlParameter) username)))
    {
    }

    public ChangePasswordRequest()
    {
    }

    [IgnoreDataMember]
    Name IChangePasswordRequest.Username => this.Self.RouteValues.Get<Name>("username");

    public Elasticsearch.Net.Refresh? Refresh
    {
      get => this.Q<Elasticsearch.Net.Refresh?>("refresh");
      set => this.Q("refresh", (object) value);
    }

    public string Password { get; set; }
  }
}
