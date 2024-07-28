// Decompiled with JetBrains decompiler
// Type: Nest.InvalidateUserAccessTokenRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;

namespace Nest
{
  public class InvalidateUserAccessTokenRequest : 
    PlainRequestBase<InvalidateUserAccessTokenRequestParameters>,
    IInvalidateUserAccessTokenRequest,
    IRequest<InvalidateUserAccessTokenRequestParameters>,
    IRequest
  {
    protected IInvalidateUserAccessTokenRequest Self => (IInvalidateUserAccessTokenRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityInvalidateUserAccessToken;

    public InvalidateUserAccessTokenRequest(string token) => ((IInvalidateUserAccessTokenRequest) this).Token = token;

    string IInvalidateUserAccessTokenRequest.Token { get; set; }

    string IInvalidateUserAccessTokenRequest.RefreshToken { get; set; }

    string IInvalidateUserAccessTokenRequest.RealmName { get; set; }

    string IInvalidateUserAccessTokenRequest.Username { get; set; }
  }
}
