// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.IDelegatedAuthorizationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [DefaultServiceImplementation(typeof (FrameworkDelegatedAuthorizationService))]
  public interface IDelegatedAuthorizationService : IVssFrameworkService
  {
    AuthorizationDescription InitiateAuthorization(
      IVssRequestContext requestContext,
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes);

    AuthorizationDescription InitiateAuthorization(
      IVssRequestContext requestContext,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes);

    AuthorizationDecision Authorize(
      IVssRequestContext requestContext,
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      Guid? authorizationId = null);

    AuthorizationDecision Authorize(
      IVssRequestContext requestContext,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      Guid? authorizationId = null);

    void Revoke(IVssRequestContext requestContext, Guid userId, Guid authorizationId);

    void Revoke(IVssRequestContext requestContext, Guid authorizationId);

    AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      GrantType grantType,
      JsonWebToken grant,
      JsonWebToken clientSecret,
      Uri redirectUri = null,
      Guid? accessId = null);

    AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false);

    AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      Guid registrationId,
      string clientSecret,
      Guid hostId,
      string requestedScopes = null);

    IEnumerable<AuthorizationDetails> GetAuthorizations(
      IVssRequestContext requestContext,
      Guid userId);

    IEnumerable<AuthorizationDetails> GetAuthorizations(IVssRequestContext requestContext);

    JsonWebToken GenerateImplicitGrantTokenForCurrentUser(
      IVssRequestContext requestContext,
      ResponseType responseType);

    SessionTokenResult IssueSessionToken(
      IVssRequestContext requestContext,
      Guid? clientId = null,
      Guid? userId = null,
      string name = null,
      DateTime? validTo = null,
      string scope = null,
      IList<Guid> targetAccounts = null,
      SessionTokenType tokenType = SessionTokenType.SelfDescribing,
      bool isPublic = false,
      string publicData = null,
      string source = null,
      bool isRequestedByTfsPatWebUI = false,
      Guid? authorizationId = null,
      Guid? accessId = null,
      IDictionary<string, string> customClaims = null);

    void RemovePublicKey(IVssRequestContext requestContext, string publicKey);

    void RevokeSessionToken(IVssRequestContext requestContext, Guid authorizationId, bool isPublic = false);

    void RevokeAllSessionTokensOfUser(IVssRequestContext requestContext);

    IList<SessionToken> ListSessionTokens(
      IVssRequestContext requestContext,
      bool isPublic = false,
      bool includePublicData = false);

    PagedSessionTokens ListSessionTokensByPage(
      IVssRequestContext requestContext,
      TokenPageRequest tokenPageRequest,
      bool isPublic = false,
      bool includePublicData = false);

    SessionToken GetSessionToken(
      IVssRequestContext requestContext,
      Guid authorizationId,
      bool isPublic = false);

    AppSessionTokenResult IssueAppSessionToken(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? userId,
      Guid? authorizationId = null);

    HostAuthorizationDecision AuthorizeHost(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? newId = null);

    void RevokeHostAuthorization(IVssRequestContext requestContext, Guid clientId, Guid? hostId);

    SessionToken UpdateSessionToken(
      IVssRequestContext requestContext,
      Guid authorizationId,
      string displayName = null,
      string scope = null,
      DateTime? validTo = null,
      IList<Guid> targetAccounts = null,
      bool isPublic = false);

    AccessTokenResult ExchangeAppToken(
      IVssRequestContext requestContext,
      JsonWebToken appToken,
      JsonWebToken clientSecret,
      Guid? accessId = null);

    IList<HostAuthorization> GetHostAuthorizations(IVssRequestContext requestContext, Guid hostId);
  }
}
