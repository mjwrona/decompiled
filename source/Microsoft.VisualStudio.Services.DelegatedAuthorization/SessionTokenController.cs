// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.SessionTokenController
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Token", ResourceName = "SessionTokens")]
  [RestrictAadServicePrincipals(WhiteListCategoryName = "WhiteList.SessionToken")]
  public class SessionTokenController : DelegatedAuthorizationControllerBase
  {
    private const string Area = "DelegatedAuthorization";
    private const string Layer = "SessionTokenController";

    [HttpPost]
    [ClientResponseType(typeof (SessionToken), null, null)]
    public HttpResponseMessage CreateSessionToken(
      SessionToken sessionToken,
      SessionTokenType tokenType = SessionTokenType.SelfDescribing,
      bool isPublic = false,
      bool isRequestedByTfsPatWebUI = false)
    {
      try
      {
        if (sessionToken == null)
          sessionToken = new SessionToken();
        bool flag = true;
        if (sessionToken.AuthorizationId == Guid.Empty)
          flag = false;
        IDelegatedAuthorizationService service = this.TfsRequestContext.GetService<IDelegatedAuthorizationService>();
        if (flag)
        {
          SessionToken token = service.UpdateSessionToken(this.TfsRequestContext, sessionToken.AuthorizationId, sessionToken.DisplayName, sessionToken.Scope, new DateTime?(sessionToken.ValidTo), sessionToken.TargetAccounts, isPublic);
          SessionTokenTracing.TraceTokenUpdate(tokenType, token);
          return this.Request.CreateResponse<SessionToken>(HttpStatusCode.OK, token);
        }
        IDelegatedAuthorizationService authorizationService = service;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        Guid? clientId = new Guid?(sessionToken.ClientId);
        Guid? userId = new Guid?(sessionToken.UserId);
        string displayName = sessionToken.DisplayName;
        DateTime? validTo = new DateTime?(sessionToken.ValidTo);
        string scope = sessionToken.Scope;
        IList<Guid> targetAccounts = sessionToken.TargetAccounts;
        int tokenType1 = (int) tokenType;
        int num1 = isPublic ? 1 : 0;
        string publicData = sessionToken.PublicData;
        string source = sessionToken.Source;
        int num2 = isRequestedByTfsPatWebUI ? 1 : 0;
        IDictionary<string, string> claims = sessionToken.Claims;
        Guid? authorizationId = new Guid?();
        Guid? accessId = new Guid?();
        IDictionary<string, string> customClaims = claims;
        SessionTokenResult result = authorizationService.IssueSessionToken(tfsRequestContext, clientId, userId, displayName, validTo, scope, targetAccounts, (SessionTokenType) tokenType1, num1 != 0, publicData, source, num2 != 0, authorizationId, accessId, customClaims);
        SessionTokenTracing.TraceTokenIssuance(tokenType, result.SessionTokenError, result.SessionToken);
        return this.Request.CreateResponse<SessionToken>(HttpStatusCode.OK, this.CheckSessionTokenResultError(result.SessionToken, result));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    private SessionToken CheckSessionTokenResultError(
      SessionToken sessionToken,
      SessionTokenResult result)
    {
      if (!result.HasError)
        return result.SessionToken;
      this.TfsRequestContext.Trace(1048025, TraceLevel.Error, "DelegatedAuthorization", nameof (SessionTokenController), string.Format("{0} - error creating session token. User: {1}, Client: {2}, Scope: {3}, TargetAccounts: {4}", (object) result.SessionTokenError, (object) sessionToken?.UserId, (object) sessionToken?.ClientId, (object) sessionToken?.Scope, sessionToken?.TargetAccounts == null ? (object) string.Empty : (object) string.Join<Guid>(",", (IEnumerable<Guid>) sessionToken.TargetAccounts)));
      if (result.SessionTokenError == SessionTokenError.FailedToIssueAccessToken)
        throw new FailedToIssueAccessTokenException(result.SessionTokenError.ToString());
      if (result.SessionTokenError == SessionTokenError.FailedToReadTenantPolicy)
        throw new FailedToReadTenantPoliciesException(result.SessionTokenError.ToString());
      throw new SessionTokenCreateException(result.SessionTokenError.ToString());
    }

    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage RemovePublicKey(SshPublicKey publicData, bool remove)
    {
      try
      {
        this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().RemovePublicKey(this.TfsRequestContext.To(TeamFoundationHostType.Deployment), publicData.Value);
        SessionTokenTracing.TracePublicKeyRemoval(publicData.Value);
        return this.Request.CreateResponse(HttpStatusCode.OK);
      }
      catch (AuthorizationIdNotFoundException ex)
      {
        SessionTokenTracing.TracePublicKeyRemoval(publicData.Value, ex.Message);
        throw ex;
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage RevokeSessionToken(Guid authorizationId, bool isPublic = false)
    {
      try
      {
        this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().RevokeSessionToken(this.TfsRequestContext, authorizationId, isPublic);
        SessionTokenTracing.TraceTokenRevocation(authorizationId);
        return this.Request.CreateResponse(HttpStatusCode.OK);
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage RevokeAllSessionTokensOfUser()
    {
      try
      {
        this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().RevokeAllSessionTokensOfUser(this.TfsRequestContext);
        SessionTokenTracing.TraceAllTokensRevokeOrRemovalForUser(this.TfsRequestContext.GetUserIdentity().StorageKey(this.TfsRequestContext, TeamFoundationHostType.Deployment));
        return this.Request.CreateResponse(HttpStatusCode.OK);
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (List<SessionToken>), null, null)]
    public HttpResponseMessage GetSessionTokens(bool isPublic = false, bool includePublicData = false)
    {
      try
      {
        return this.Request.CreateResponse<IList<SessionToken>>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().ListSessionTokens(this.TfsRequestContext, isPublic, includePublicData));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (PagedSessionTokens), null, null)]
    public HttpResponseMessage GetSessionTokensPage(
      DisplayFilterOptions displayFilterOption,
      CreatedByOptions createdByOption,
      SortByOptions sortByOption,
      bool isSortAscending,
      int startRowNumber,
      int pageSize,
      string pageRequestTimeStamp,
      bool isPublic = false,
      bool includePublicData = false)
    {
      try
      {
        IDelegatedAuthorizationService service = this.TfsRequestContext.GetService<IDelegatedAuthorizationService>();
        TokenPageRequest tokenPageRequest1 = new TokenPageRequest()
        {
          DisplayFilterOption = displayFilterOption,
          CreatedByOption = createdByOption,
          SortByOption = sortByOption,
          IsSortAscending = isSortAscending,
          StartRowNumber = startRowNumber,
          PageSize = pageSize,
          PageRequestTimeStamp = pageRequestTimeStamp
        };
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        TokenPageRequest tokenPageRequest2 = tokenPageRequest1;
        int num1 = isPublic ? 1 : 0;
        int num2 = includePublicData ? 1 : 0;
        return this.Request.CreateResponse<PagedSessionTokens>(HttpStatusCode.OK, service.ListSessionTokensByPage(tfsRequestContext, tokenPageRequest2, num1 != 0, num2 != 0));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (SessionToken), null, null)]
    public HttpResponseMessage GetSessionToken(Guid authorizationId, bool isPublic = false)
    {
      ArgumentUtility.CheckForEmptyGuid(authorizationId, nameof (authorizationId));
      try
      {
        return this.Request.CreateResponse<SessionToken>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().GetSessionToken(this.TfsRequestContext, authorizationId, isPublic));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }
  }
}
