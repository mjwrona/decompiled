// Decompiled with JetBrains decompiler
// Type: Nest.GetUserRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetUserRequest : 
    PlainRequestBase<GetUserRequestParameters>,
    IGetUserRequest,
    IRequest<GetUserRequestParameters>,
    IRequest
  {
    protected IGetUserRequest Self => (IGetUserRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityGetUser;

    public GetUserRequest(Names username)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (username), (IUrlParameter) username)))
    {
    }

    public GetUserRequest()
    {
    }

    [IgnoreDataMember]
    Names IGetUserRequest.Username => this.Self.RouteValues.Get<Names>("username");
  }
}
