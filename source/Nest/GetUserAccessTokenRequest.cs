// Decompiled with JetBrains decompiler
// Type: Nest.GetUserAccessTokenRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetUserAccessTokenRequest : 
    PlainRequestBase<GetUserAccessTokenRequestParameters>,
    IGetUserAccessTokenRequest,
    IRequest<GetUserAccessTokenRequestParameters>,
    IRequest
  {
    protected IGetUserAccessTokenRequest Self => (IGetUserAccessTokenRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityGetUserAccessToken;

    public GetUserAccessTokenRequest(string username, string password)
    {
      IGetUserAccessTokenRequest accessTokenRequest = (IGetUserAccessTokenRequest) this;
      accessTokenRequest.Username = username;
      accessTokenRequest.Password = password;
    }

    public AccessTokenGrantType? GrantType { get; set; } = new AccessTokenGrantType?(AccessTokenGrantType.Password);

    public string Scope { get; set; }

    [DataMember(Name = "password")]
    string IGetUserAccessTokenRequest.Password { get; set; }

    [DataMember(Name = "username")]
    string IGetUserAccessTokenRequest.Username { get; set; }
  }
}
