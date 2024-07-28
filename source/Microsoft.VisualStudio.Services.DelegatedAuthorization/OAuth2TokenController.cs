// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.OAuth2TokenController
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "oauth2", ResourceName = "token")]
  [RestrictAadServicePrincipals]
  public class OAuth2TokenController : DelegatedAuthorizationControllerBase
  {
    private const string Area = "DelegatedAuthorization";
    private const string Layer = "OAuth2TokenController";
    private const int TracePointStart = 1048600;
    private const int TracePointEnd = 1048700;
    private const int TracePointGrantTokenStart = 1048650;
    private const int TracePointGrantTokenEnd = 1048659;
    private const int TracePointGrantTokenUnknownError = 1048651;

    [HttpPost]
    [RequestContentTypeRestriction(AllowForm = true, AllowJson = false, AllowJsonPatch = false)]
    [ClientResponseType(typeof (VssOAuthTokenResponse), null, null)]
    public HttpResponseMessage IssueToken(FormDataCollection form)
    {
      VssOAuthTokenRequest oauthTokenRequest;
      try
      {
        oauthTokenRequest = VssOAuthTokenRequest.FromFormInput(form);
      }
      catch (VssOAuthTokenRequestException ex)
      {
        VssOAuthTokenResponse oauthTokenResponse = new VssOAuthTokenResponse()
        {
          Error = ex.Error,
          ErrorDescription = ex.Message
        };
        this.TfsRequestContext.TraceException(1048652, "DelegatedAuthorization", nameof (OAuth2TokenController), (Exception) ex);
        return this.Request.CreateResponse<VssOAuthTokenResponse>(HttpStatusCode.BadRequest, oauthTokenResponse);
      }
      if (oauthTokenRequest.ClientCredential == null || oauthTokenRequest.ClientCredential.CredentialType != VssOAuthClientCredentialType.JwtBearer)
      {
        VssOAuthTokenResponse oauthTokenResponse = new VssOAuthTokenResponse()
        {
          Error = VssOAuthErrorCodes.InvalidClient,
          ErrorDescription = "Invalid client auth token"
        };
        this.TfsRequestContext.Trace(1048652, TraceLevel.Verbose, "DelegatedAuthorization", nameof (OAuth2TokenController), "Client auth token was invalid.");
        return this.Request.CreateResponse<VssOAuthTokenResponse>(HttpStatusCode.BadRequest, oauthTokenResponse);
      }
      if (oauthTokenRequest.Grant == null)
      {
        VssOAuthTokenResponse oauthTokenResponse = new VssOAuthTokenResponse()
        {
          Error = VssOAuthErrorCodes.InvalidRequest,
          ErrorDescription = "grant_type must be provided"
        };
        this.TfsRequestContext.Trace(1048653, TraceLevel.Verbose, "DelegatedAuthorization", nameof (OAuth2TokenController), "Grant type was not provided.");
        return this.Request.CreateResponse<VssOAuthTokenResponse>(HttpStatusCode.BadRequest, oauthTokenResponse);
      }
      if (oauthTokenRequest.Grant.GrantType != VssOAuthGrantType.ClientCredentials)
      {
        VssOAuthTokenResponse oauthTokenResponse = new VssOAuthTokenResponse()
        {
          Error = VssOAuthErrorCodes.UnsupportedGrantType,
          ErrorDescription = "grant_type must be client_credentials"
        };
        this.TfsRequestContext.Trace(1048653, TraceLevel.Verbose, "DelegatedAuthorization", nameof (OAuth2TokenController), "An unsupported grant type was provided: {0}.", (object) form["grant_type"]);
        return this.Request.CreateResponse<VssOAuthTokenResponse>(HttpStatusCode.BadRequest, oauthTokenResponse);
      }
      try
      {
        VssOAuthJwtBearerClientCredential clientCredential = (VssOAuthJwtBearerClientCredential) oauthTokenRequest.ClientCredential;
        AccessTokenResult accessTokenResult = this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().Exchange(this.TfsRequestContext, GrantType.ClientCredentials, (JsonWebToken) null, clientCredential.Assertion.GetBearerToken());
        if (accessTokenResult.HasError)
          return this.CreateErrorResponseFromTokenError(accessTokenResult.AccessTokenError, accessTokenResult.ErrorDescription);
        return this.Request.CreateResponse<VssOAuthTokenResponse>(HttpStatusCode.OK, new VssOAuthTokenResponse()
        {
          AccessToken = accessTokenResult.AccessToken.EncodedToken,
          ExpiresIn = (int) (accessTokenResult.AccessToken.ValidTo - DateTime.UtcNow).TotalSeconds,
          TokenType = accessTokenResult.AccessToken.TokenType
        });
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpPost]
    [ClientResponseType(typeof (AccessTokenResult), null, null)]
    public HttpResponseMessage IssueApplicationToken(
      [FromBody] string clientSecret,
      Guid registrationId,
      Guid hostId,
      string requestedScopes = null)
    {
      this.TfsRequestContext.CheckServiceHostType(TeamFoundationHostType.Deployment);
      AccessTokenResult accessTokenResult = this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().Exchange(this.TfsRequestContext, registrationId, clientSecret, hostId, requestedScopes);
      return accessTokenResult.HasError && this.GetErrorCode(accessTokenResult.AccessTokenError) == VssOAuthErrorCodes.ServerError ? this.Request.CreateResponse<FailedToIssueAccessTokenException>(HttpStatusCode.InternalServerError, new FailedToIssueAccessTokenException(accessTokenResult.AccessTokenError.ToString())) : this.Request.CreateResponse<AccessTokenResult>(HttpStatusCode.OK, accessTokenResult);
    }

    private HttpResponseMessage CreateErrorResponseFromTokenError(
      TokenError error,
      string errorDescription)
    {
      VssOAuthTokenResponse oauthTokenResponse = new VssOAuthTokenResponse()
      {
        Error = this.GetErrorCode(error),
        ErrorDescription = string.IsNullOrEmpty(errorDescription) ? this.GetErrorDescription(error) : errorDescription
      };
      HttpStatusCode statusCode = HttpStatusCode.BadRequest;
      if (oauthTokenResponse.Error == VssOAuthErrorCodes.ServerError)
        statusCode = HttpStatusCode.InternalServerError;
      return this.Request.CreateResponse<VssOAuthTokenResponse>(statusCode, oauthTokenResponse);
    }

    private string GetErrorCode(TokenError error)
    {
      string invalidRequest = VssOAuthErrorCodes.InvalidRequest;
      string errorCode;
      switch (error)
      {
        case TokenError.GrantTypeRequired:
        case TokenError.AuthorizationGrantRequired:
        case TokenError.RedirectUriRequired:
        case TokenError.InvalidRefreshToken:
        case TokenError.AuthorizationGrantExpired:
        case TokenError.AccessAlreadyIssued:
        case TokenError.InvalidRedirectUri:
        case TokenError.AccessTokenNotFound:
        case TokenError.InvalidAccessToken:
        case TokenError.AccessTokenAlreadyRefreshed:
        case TokenError.ClientSecretExpired:
          errorCode = VssOAuthErrorCodes.InvalidRequest;
          break;
        case TokenError.ClientSecretRequired:
        case TokenError.InvalidClientId:
        case TokenError.InvalidClient:
          errorCode = VssOAuthErrorCodes.InvalidClient;
          break;
        case TokenError.InvalidAuthorizationGrant:
        case TokenError.AuthorizationNotFound:
          errorCode = VssOAuthErrorCodes.InvalidGrant;
          break;
        case TokenError.InvalidClientSecret:
        case TokenError.HostAuthorizationNotFound:
        case TokenError.HostAuthorizationIsNotValid:
          errorCode = VssOAuthErrorCodes.UnauthorizedClient;
          break;
        case TokenError.InvalidScope:
          errorCode = VssOAuthErrorCodes.InvalidScope;
          break;
        default:
          errorCode = VssOAuthErrorCodes.ServerError;
          break;
      }
      return errorCode;
    }

    private string GetErrorDescription(TokenError error)
    {
      string errorDescription;
      switch (error)
      {
        case TokenError.GrantTypeRequired:
          errorDescription = "grant_type must be provided";
          break;
        case TokenError.AuthorizationGrantRequired:
          errorDescription = "A valid authorization grant must be provided";
          break;
        case TokenError.ClientSecretRequired:
          errorDescription = "A client secret must be provided";
          break;
        case TokenError.RedirectUriRequired:
          errorDescription = "A redirect URI must be provided";
          break;
        case TokenError.InvalidAuthorizationGrant:
          errorDescription = "The provided authorization grant failed verification";
          break;
        case TokenError.InvalidRefreshToken:
          errorDescription = "The given refresh token is invalid";
          break;
        case TokenError.AuthorizationNotFound:
          errorDescription = "No authorization grant was found";
          break;
        case TokenError.AuthorizationGrantExpired:
          errorDescription = "The authorization grant has expired.";
          break;
        case TokenError.AccessAlreadyIssued:
          errorDescription = "An access token has already been issued";
          break;
        case TokenError.InvalidRedirectUri:
          errorDescription = "The given redirect_url was not valid";
          break;
        case TokenError.AccessTokenNotFound:
          errorDescription = "The access token was not found";
          break;
        case TokenError.InvalidAccessToken:
          errorDescription = "The access token is not valid";
          break;
        case TokenError.AccessTokenAlreadyRefreshed:
          errorDescription = "The access token has already been refreshed";
          break;
        case TokenError.InvalidClientSecret:
          errorDescription = "Client failed validation";
          break;
        case TokenError.ClientSecretExpired:
          errorDescription = "Client secret is expired.";
          break;
        case TokenError.InvalidClientId:
        case TokenError.InvalidClient:
          errorDescription = "The client is not valid";
          break;
        default:
          this.TfsRequestContext.Trace(1048651, TraceLevel.Warning, "DelegatedAuthorization", nameof (OAuth2TokenController), "Unknown error received from PlatformDelegatedAuthorizationService.");
          errorDescription = "An internal server error occurred";
          break;
      }
      return errorDescription;
    }
  }
}
