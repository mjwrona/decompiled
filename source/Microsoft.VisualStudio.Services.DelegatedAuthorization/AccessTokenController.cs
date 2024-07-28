// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AccessTokenController
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Token", ResourceName = "AccessTokens")]
  public class AccessTokenController : DelegatedAuthorizationControllerBase
  {
    private const string Area = "DelegatedAuthorization";
    private const string Layer = "AccessTokenController";

    [HttpGet]
    [Obsolete("Use ExchangeAccessTokenKey instead.  This endpoint should be removed after all services are updated to M123.")]
    [ClientResponseType(typeof (AccessToken), null, null)]
    public HttpResponseMessage GetAccessToken(string key = null, bool isPublic = false) => this.ExchangeAccessTokenKey(key, isPublic);

    [HttpPost]
    [ClientResponseType(typeof (AccessToken), null, null)]
    public HttpResponseMessage ExchangeAccessTokenKey([FromBody] string accessTokenKey, bool isPublic = false)
    {
      try
      {
        AccessTokenResult accessTokenResult = this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().Exchange(this.TfsRequestContext, accessTokenKey, isPublic);
        if (accessTokenResult.HasError)
        {
          switch (accessTokenResult.AccessTokenError)
          {
            case TokenError.InvalidAccessToken:
            case TokenError.AccessTokenKeyRequired:
            case TokenError.InvalidAccessTokenKey:
              throw new InvalidPersonalAccessTokenException(accessTokenResult.AccessTokenError.ToString());
            case TokenError.AccessDenied:
              throw new AccessCheckException(accessTokenResult.AccessTokenError.ToString());
            case TokenError.FailedToGetAccessToken:
              throw new DelegatedAuthorizationControllerBase.InternalServerErrorException(accessTokenResult.AccessTokenError.ToString());
            case TokenError.InvalidPublicAccessTokenKey:
            case TokenError.InvalidPublicAccessToken:
              throw new InvalidPublicKeyException(accessTokenResult.AccessTokenError.ToString());
            default:
              throw new ExchangeAccessTokenKeyException(accessTokenResult.AccessTokenError.ToString());
          }
        }
        else
        {
          AccessToken accessToken = new AccessToken()
          {
            AuthorizationId = accessTokenResult.AuthorizationId,
            Token = accessTokenResult.AccessToken,
            TokenType = accessTokenResult.TokenType
          };
          if (accessTokenResult.RefreshToken != null)
            accessToken.RefreshToken = accessTokenResult.RefreshToken.Jwt;
          return this.Request.CreateResponse<AccessToken>(HttpStatusCode.OK, accessToken);
        }
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }
  }
}
