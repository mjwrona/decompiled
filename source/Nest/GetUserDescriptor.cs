// Decompiled with JetBrains decompiler
// Type: Nest.GetUserDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using System;

namespace Nest
{
  public class GetUserDescriptor : 
    RequestDescriptorBase<GetUserDescriptor, GetUserRequestParameters, IGetUserRequest>,
    IGetUserRequest,
    IRequest<GetUserRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityGetUser;

    public GetUserDescriptor(Names username)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (username), (IUrlParameter) username)))
    {
    }

    public GetUserDescriptor()
    {
    }

    Names IGetUserRequest.Username => this.Self.RouteValues.Get<Names>("username");

    public GetUserDescriptor Username(Names username) => this.Assign<Names>(username, (Action<IGetUserRequest, Names>) ((a, v) => a.RouteValues.Optional(nameof (username), (IUrlParameter) v)));
  }
}
