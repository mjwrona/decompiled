// Decompiled with JetBrains decompiler
// Type: Nest.InvalidateUserAccessTokenDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;
using System;

namespace Nest
{
  public class InvalidateUserAccessTokenDescriptor : 
    RequestDescriptorBase<InvalidateUserAccessTokenDescriptor, InvalidateUserAccessTokenRequestParameters, IInvalidateUserAccessTokenRequest>,
    IInvalidateUserAccessTokenRequest,
    IRequest<InvalidateUserAccessTokenRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityInvalidateUserAccessToken;

    public InvalidateUserAccessTokenDescriptor(string token) => ((IInvalidateUserAccessTokenRequest) this).Token = token;

    public InvalidateUserAccessTokenDescriptor RefreshToken(string refreshToken) => this.Assign<string>(refreshToken, (Action<IInvalidateUserAccessTokenRequest, string>) ((a, v) => a.RefreshToken = v));

    public InvalidateUserAccessTokenDescriptor RealmName(string realmName) => this.Assign<string>(realmName, (Action<IInvalidateUserAccessTokenRequest, string>) ((a, v) => a.RealmName = v));

    public InvalidateUserAccessTokenDescriptor Username(string username) => this.Assign<string>(username, (Action<IInvalidateUserAccessTokenRequest, string>) ((a, v) => a.Username = v));

    string IInvalidateUserAccessTokenRequest.Token { get; set; }

    string IInvalidateUserAccessTokenRequest.RefreshToken { get; set; }

    string IInvalidateUserAccessTokenRequest.RealmName { get; set; }

    string IInvalidateUserAccessTokenRequest.Username { get; set; }
  }
}
