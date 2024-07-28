// Decompiled with JetBrains decompiler
// Type: Nest.GetUserAccessTokenDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;
using System;

namespace Nest
{
  public class GetUserAccessTokenDescriptor : 
    RequestDescriptorBase<GetUserAccessTokenDescriptor, GetUserAccessTokenRequestParameters, IGetUserAccessTokenRequest>,
    IGetUserAccessTokenRequest,
    IRequest<GetUserAccessTokenRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityGetUserAccessToken;

    public GetUserAccessTokenDescriptor(string username, string password)
    {
      IGetUserAccessTokenRequest accessTokenRequest = (IGetUserAccessTokenRequest) this;
      accessTokenRequest.Username = username;
      accessTokenRequest.Password = password;
    }

    AccessTokenGrantType? IGetUserAccessTokenRequest.GrantType { get; set; } = new AccessTokenGrantType?(AccessTokenGrantType.Password);

    string IGetUserAccessTokenRequest.Password { get; set; }

    string IGetUserAccessTokenRequest.Scope { get; set; }

    string IGetUserAccessTokenRequest.Username { get; set; }

    public GetUserAccessTokenDescriptor GrantType(AccessTokenGrantType? type) => this.Assign<AccessTokenGrantType?>(type, (Action<IGetUserAccessTokenRequest, AccessTokenGrantType?>) ((a, v) => a.GrantType = v));

    public GetUserAccessTokenDescriptor Scope(string scope) => this.Assign<string>(scope, (Action<IGetUserAccessTokenRequest, string>) ((a, v) => a.Scope = v));
  }
}
