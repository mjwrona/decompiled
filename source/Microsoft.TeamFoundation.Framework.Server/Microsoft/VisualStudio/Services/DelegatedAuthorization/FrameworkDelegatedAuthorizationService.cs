// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.FrameworkDelegatedAuthorizationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkDelegatedAuthorizationService : 
    IDelegatedAuthorizationService,
    IVssFrameworkService
  {
    private const string Area = "DelegatedAuthorizationService";
    private const string Layer = "FrameworkDelegatedAuthorizationService";

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckHostedDeployment();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public SessionTokenResult IssueSessionToken(
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
      IDictionary<string, string> customClaims = null)
    {
      requestContext.TraceEnter(1048032, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (IssueSessionToken));
      try
      {
        if (userId.HasValue && !requestContext.IsSystemContext)
          throw new InvalidOperationException(FrameworkResources.DelegatedAuthorizationUserIdSpecifiedWithoutSystemContext());
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        bool flag1;
        bool flag2;
        if (vssRequestContext.Items.TryGetValue<bool>(RequestContextItemsKeys.AlternateAuthCredentialsContextKey, out flag1) & flag1 && (!vssRequestContext.Items.TryGetValue<bool>("ExemptFromAuthenticationTypeCheckInDelegatedAuthorization", out flag2) || !flag2))
        {
          requestContext.Trace(1048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), "Alternate credentials cannot be used to request session token.");
          throw new SessionTokenCreateException("AccessDenied");
        }
        if (userId.HasValue && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          requestContext.Trace(1048045, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), string.Format("user ID {0} specified at deployment level", (object) userId));
        userId = this.TryResolveMasterId(requestContext, userId);
        FrameworkTokenHttpClient spsHttpClient = this.GetSpsHttpClient<FrameworkTokenHttpClient>(requestContext);
        SessionToken sessionToken1 = new SessionToken();
        sessionToken1.Source = source;
        Guid? nullable1 = clientId;
        sessionToken1.ClientId = nullable1 ?? Guid.Empty;
        nullable1 = userId;
        sessionToken1.UserId = nullable1 ?? Guid.Empty;
        sessionToken1.DisplayName = name;
        sessionToken1.ValidTo = validTo.GetValueOrDefault();
        sessionToken1.Scope = scope;
        sessionToken1.TargetAccounts = targetAccounts;
        sessionToken1.PublicData = publicData;
        sessionToken1.Claims = customClaims;
        bool? nullable2 = new bool?(isPublic);
        SessionTokenType? tokenType1 = new SessionTokenType?(tokenType);
        bool? isPublic1 = nullable2;
        bool? isRequestedByTfsPatWebUI1 = new bool?(isRequestedByTfsPatWebUI);
        CancellationToken cancellationToken = new CancellationToken();
        SessionToken sessionToken2 = spsHttpClient.CreateSessionTokenAsync(sessionToken1, tokenType1, isPublic1, isRequestedByTfsPatWebUI1, cancellationToken: cancellationToken).SyncResult<SessionToken>();
        return new SessionTokenResult()
        {
          SessionToken = sessionToken2
        };
      }
      finally
      {
        requestContext.TraceLeave(1048039, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (IssueSessionToken));
      }
    }

    public SessionToken UpdateSessionToken(
      IVssRequestContext requestContext,
      Guid authorizationId,
      string name = null,
      string scope = null,
      DateTime? validTo = null,
      IList<Guid> targetAccounts = null,
      bool isPublic = false)
    {
      requestContext.TraceEnter(1049030, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (UpdateSessionToken));
      try
      {
        FrameworkTokenHttpClient spsHttpClient = this.GetSpsHttpClient<FrameworkTokenHttpClient>(requestContext);
        SessionToken sessionToken = new SessionToken();
        sessionToken.AuthorizationId = authorizationId;
        sessionToken.DisplayName = name;
        sessionToken.ValidTo = validTo.GetValueOrDefault();
        sessionToken.Scope = scope;
        sessionToken.TargetAccounts = targetAccounts;
        bool? nullable = new bool?(isPublic);
        SessionTokenType? tokenType = new SessionTokenType?();
        bool? isPublic1 = nullable;
        bool? isRequestedByTfsPatWebUI = new bool?();
        CancellationToken cancellationToken = new CancellationToken();
        return spsHttpClient.CreateSessionTokenAsync(sessionToken, tokenType, isPublic1, isRequestedByTfsPatWebUI, cancellationToken: cancellationToken).SyncResult<SessionToken>();
      }
      finally
      {
        requestContext.TraceLeave(1049031, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (UpdateSessionToken));
      }
    }

    public AccessTokenResult ExchangeAppToken(
      IVssRequestContext requestContext,
      JsonWebToken appToken,
      JsonWebToken clientSecret,
      Guid? accessId = null)
    {
      requestContext.TraceEnter(1048160, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (ExchangeAppToken));
      try
      {
        throw new NotSupportedException();
      }
      finally
      {
        requestContext.TraceLeave(1048169, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (ExchangeAppToken));
      }
    }

    public JsonWebToken GenerateImplicitGrantTokenForCurrentUser(
      IVssRequestContext requestContext,
      ResponseType responseType)
    {
      requestContext.TraceEnter(1048150, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (GenerateImplicitGrantTokenForCurrentUser));
      try
      {
        throw new NotSupportedException();
      }
      finally
      {
        requestContext.TraceLeave(1048159, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (GenerateImplicitGrantTokenForCurrentUser));
      }
    }

    public AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      GrantType grantType,
      JsonWebToken grant,
      JsonWebToken clientSecret,
      Uri redirectUri = null,
      Guid? accessId = null)
    {
      requestContext.TraceEnter(1048140, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (Exchange));
      try
      {
        throw new NotSupportedException();
      }
      finally
      {
        requestContext.TraceLeave(1048149, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (Exchange));
      }
    }

    public AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      Guid registrationId,
      string clientSecret,
      Guid hostId,
      string requestedScopes = null)
    {
      try
      {
        requestContext.TraceEnter(1048340, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (Exchange));
        throw new NotSupportedException();
      }
      finally
      {
        requestContext.TraceLeave(1048349, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (Exchange));
      }
    }

    public AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false)
    {
      requestContext.TraceEnter(1048040, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (Exchange));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IFrameworkDelegatedAuthorizationCacheService service = vssRequestContext.GetService<IFrameworkDelegatedAuthorizationCacheService>();
        AccessToken accessTokenResult1;
        if (!service.TryGetAccessToken(vssRequestContext, accessTokenKey, out accessTokenResult1))
        {
          requestContext.Trace(1048041, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), "Access token cache - cache miss");
          accessTokenResult1 = this.GetAndSetAccessToken(requestContext, service, accessTokenKey, isPublic);
        }
        else
        {
          requestContext.Trace(1048042, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), "Access token cache - cache hit.");
          if (requestContext.IsFeatureEnabled("AzureDevOps.Services.TokenService.ExchangeLongLastingTokens.M167") && accessTokenResult1.ValidTo != new DateTimeOffset() && accessTokenResult1.ValidTo <= (DateTimeOffset) DateTime.UtcNow)
          {
            requestContext.Trace(1048041, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), "Access token cache - expired");
            accessTokenResult1 = this.GetAndSetAccessToken(requestContext, service, accessTokenKey, isPublic);
          }
        }
        AccessTokenResult accessTokenResult2 = new AccessTokenResult()
        {
          AccessToken = accessTokenResult1.Token,
          TokenType = accessTokenResult1.TokenType,
          ValidTo = accessTokenResult1.ValidTo.UtcDateTime
        };
        if (accessTokenResult1.RefreshToken != null)
          accessTokenResult2.RefreshToken = new RefreshTokenGrant(accessTokenResult1.RefreshToken);
        return accessTokenResult2;
      }
      finally
      {
        requestContext.TraceLeave(1048049, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (Exchange));
      }
    }

    internal virtual AccessToken GetAndSetAccessToken(
      IVssRequestContext requestContext,
      IFrameworkDelegatedAuthorizationCacheService frameworkDelegatedAuthorizationCacheService,
      string accessTokenKey,
      bool isPublic)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      AccessToken accessTokenResult;
      try
      {
        accessTokenResult = this.GetAccessToken(requestContext, accessTokenKey, isPublic);
        frameworkDelegatedAuthorizationCacheService.SetAccessToken(requestContext1, accessTokenKey, accessTokenResult);
      }
      catch (AccessCheckException ex)
      {
        requestContext.TraceException(1048043, TraceLevel.Info, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), (Exception) ex);
        frameworkDelegatedAuthorizationCacheService.AddNoAccessTokenEntryToLocalCache(requestContext1, accessTokenKey);
        accessTokenResult = frameworkDelegatedAuthorizationCacheService.NoAccessTokenEntry;
      }
      catch (ExchangeAccessTokenKeyException ex)
      {
        requestContext.TraceException(1048043, TraceLevel.Info, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), (Exception) ex);
        frameworkDelegatedAuthorizationCacheService.AddNoAccessTokenEntryToLocalCache(requestContext1, accessTokenKey, isPublic);
        accessTokenResult = frameworkDelegatedAuthorizationCacheService.NoAccessTokenEntry;
      }
      catch (InvalidPersonalAccessTokenException ex)
      {
        requestContext.TraceException(1048043, TraceLevel.Info, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), (Exception) ex);
        frameworkDelegatedAuthorizationCacheService.AddNoAccessTokenEntryToLocalCache(requestContext1, accessTokenKey, isPublic);
        accessTokenResult = frameworkDelegatedAuthorizationCacheService.NoAccessTokenEntry;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1048043, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), ex);
        throw;
      }
      return accessTokenResult;
    }

    internal virtual AccessToken GetAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false)
    {
      return this.GetSpsHttpClient<FrameworkTokenHttpClient>(requestContext).ExchangeAccessTokenKeyAsync(accessTokenKey, new bool?(isPublic)).SyncResult<AccessToken>();
    }

    public AuthorizationDescription InitiateAuthorization(
      IVssRequestContext requestContext,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes)
    {
      return this.InitiateAuthorization(requestContext, Guid.Empty, responseType, clientId, redirectUri, scopes);
    }

    public AuthorizationDescription InitiateAuthorization(
      IVssRequestContext requestContext,
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes)
    {
      requestContext.TraceEnter(1048180, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (InitiateAuthorization));
      try
      {
        userId = this.TryResolveMasterId(requestContext, userId);
        return this.GetSpsHttpClient<DelegatedAuthorizationHttpClient>(requestContext).InitiateAuthorizationAsync(userId, responseType, clientId, redirectUri, scopes).SyncResult<AuthorizationDescription>();
      }
      finally
      {
        requestContext.TraceLeave(1048189, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (InitiateAuthorization));
      }
    }

    public AuthorizationDecision Authorize(
      IVssRequestContext requestContext,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      Guid? authorizationId = null)
    {
      return this.Authorize(requestContext, Guid.Empty, responseType, clientId, redirectUri, scopes, authorizationId);
    }

    public AuthorizationDecision Authorize(
      IVssRequestContext requestContext,
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      Guid? authorizationId = null)
    {
      requestContext.TraceEnter(1048170, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (Authorize));
      try
      {
        userId = this.TryResolveMasterId(requestContext, userId);
        return this.GetSpsHttpClient<DelegatedAuthorizationHttpClient>(requestContext).AuthorizeAsync(userId, responseType, clientId, redirectUri, scopes).SyncResult<AuthorizationDecision>();
      }
      finally
      {
        requestContext.TraceLeave(1048179, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (Authorize));
      }
    }

    public void Revoke(IVssRequestContext requestContext, Guid authorizationId) => this.Revoke(requestContext, Guid.Empty, authorizationId);

    public void Revoke(IVssRequestContext requestContext, Guid userId, Guid authorizationId)
    {
      requestContext.TraceEnter(1048120, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (Revoke));
      try
      {
        userId = this.TryResolveMasterId(requestContext, userId);
        this.GetSpsHttpClient<DelegatedAuthorizationHttpClient>(requestContext).RevokeAuthorizationAsync(authorizationId, new Guid?(userId)).SyncResult();
      }
      finally
      {
        requestContext.TraceLeave(1048129, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (Revoke));
      }
    }

    public IEnumerable<AuthorizationDetails> GetAuthorizations(IVssRequestContext requestContext) => this.GetAuthorizations(requestContext, Guid.Empty);

    public IEnumerable<AuthorizationDetails> GetAuthorizations(
      IVssRequestContext requestContext,
      Guid userId)
    {
      requestContext.TraceEnter(1048130, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (GetAuthorizations));
      try
      {
        userId = this.TryResolveMasterId(requestContext, userId);
        return (IEnumerable<AuthorizationDetails>) this.GetSpsHttpClient<DelegatedAuthorizationHttpClient>(requestContext).GetAuthorizationsAsync(new Guid?(userId)).SyncResult<List<AuthorizationDetails>>();
      }
      finally
      {
        requestContext.TraceLeave(1048139, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (GetAuthorizations));
      }
    }

    public IList<SessionToken> ListSessionTokens(
      IVssRequestContext requestContext,
      bool isPublic = false,
      bool includePublicData = false)
    {
      requestContext.TraceEnter(1048090, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (ListSessionTokens));
      try
      {
        return (IList<SessionToken>) this.GetSpsHttpClient<FrameworkTokenHttpClient>(requestContext).GetSessionTokensAsync(new bool?(includePublicData), new bool?(isPublic)).SyncResult<List<SessionToken>>();
      }
      finally
      {
        requestContext.TraceLeave(1048099, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (ListSessionTokens));
      }
    }

    public PagedSessionTokens ListSessionTokensByPage(
      IVssRequestContext requestContext,
      TokenPageRequest tokenPageRequest,
      bool isPublic = false,
      bool includePublicData = false)
    {
      requestContext.TraceEnter(1048191, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (ListSessionTokensByPage));
      try
      {
        ArgumentUtility.CheckForNull<TokenPageRequest>(tokenPageRequest, nameof (tokenPageRequest));
        return this.GetSpsHttpClient<FrameworkTokenHttpClient>(requestContext).GetSessionTokensPageAsync(tokenPageRequest.DisplayFilterOption, tokenPageRequest.CreatedByOption, tokenPageRequest.SortByOption, tokenPageRequest.IsSortAscending, tokenPageRequest.StartRowNumber, tokenPageRequest.PageSize, tokenPageRequest.PageRequestTimeStamp, new bool?(isPublic), new bool?(includePublicData)).SyncResult<PagedSessionTokens>();
      }
      finally
      {
        requestContext.TraceLeave(1048192, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (ListSessionTokensByPage));
      }
    }

    public SessionToken GetSessionToken(
      IVssRequestContext requestContext,
      Guid authorizationId,
      bool isPublic = false)
    {
      requestContext.TraceEnter(1048100, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (GetSessionToken));
      try
      {
        return this.GetSpsHttpClient<FrameworkTokenHttpClient>(requestContext).GetSessionTokenAsync(authorizationId, new bool?(isPublic)).SyncResult<SessionToken>();
      }
      finally
      {
        requestContext.TraceLeave(1048109, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (GetSessionToken));
      }
    }

    public void RemovePublicKey(IVssRequestContext requestContext, string publicKey)
    {
      requestContext.TraceEnter(1048211, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (RemovePublicKey));
      try
      {
        FrameworkTokenHttpClient spsHttpClient = this.GetSpsHttpClient<FrameworkTokenHttpClient>(requestContext);
        SshPublicKey publicData = new SshPublicKey();
        publicData.Value = publicKey;
        CancellationToken cancellationToken = new CancellationToken();
        spsHttpClient.RemovePublicKeyAsync(publicData, true, cancellationToken: cancellationToken).SyncResult();
      }
      finally
      {
        requestContext.TraceLeave(1048208, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (RemovePublicKey));
      }
    }

    public void RevokeSessionToken(
      IVssRequestContext requestContext,
      Guid authorizationId,
      bool isPublic = false)
    {
      requestContext.TraceEnter(1048212, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (RevokeSessionToken));
      try
      {
        this.GetSpsHttpClient<FrameworkTokenHttpClient>(requestContext).RevokeSessionTokenAsync(authorizationId, new bool?(isPublic)).SyncResult();
      }
      finally
      {
        requestContext.TraceLeave(1048208, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (RevokeSessionToken));
      }
    }

    public void RevokeAllSessionTokensOfUser(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1048240, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (RevokeAllSessionTokensOfUser));
      try
      {
        this.GetSpsHttpClient<FrameworkTokenHttpClient>(requestContext).RevokeAllSessionTokensOfUserAsync().SyncResult();
      }
      finally
      {
        requestContext.TraceLeave(1048241, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (RevokeAllSessionTokensOfUser));
      }
    }

    public AppSessionTokenResult IssueAppSessionToken(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? userId,
      Guid? authorizationId = null)
    {
      requestContext.TraceEnter(1048209, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (IssueAppSessionToken));
      try
      {
        userId = this.TryResolveMasterId(requestContext, userId);
        return this.GetSpsHttpClient<FrameworkTokenHttpClient>(requestContext).IssueAppSessionTokenAsync(clientId, userId).SyncResult<AppSessionTokenResult>();
      }
      finally
      {
        requestContext.TraceLeave(1048210, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (IssueAppSessionToken));
      }
    }

    public HostAuthorizationDecision AuthorizeHost(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? newId = null)
    {
      requestContext.TraceEnter(1048091, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (AuthorizeHost));
      try
      {
        return this.GetSpsHttpClient<DelegatedAuthorizationHttpClient>(requestContext).AuthorizeHostAsync(clientId).SyncResult<HostAuthorizationDecision>();
      }
      finally
      {
        requestContext.TraceLeave(1048092, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (AuthorizeHost));
      }
    }

    public void RevokeHostAuthorization(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? hostId)
    {
      requestContext.TraceEnter(1048093, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (RevokeHostAuthorization));
      try
      {
        this.GetSpsHttpClient<DelegatedAuthorizationHttpClient>(requestContext).RevokeHostAuthorizationAsync(clientId, hostId).SyncResult();
      }
      finally
      {
        requestContext.TraceLeave(1048094, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationService), nameof (RevokeHostAuthorization));
      }
    }

    public IList<HostAuthorization> GetHostAuthorizations(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      throw new NotImplementedException();
    }

    internal virtual TClient GetSpsHttpClient<TClient>(IVssRequestContext requestContext) where TClient : class, IVssHttpClient
    {
      if (!requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return PartitionedClientHelper.GetSpsClientForHostId<TClient>(requestContext, requestContext.RootContext.ServiceHost.InstanceId);
      HostProxyData hostProxyData;
      return requestContext.TryGetItem<HostProxyData>(RequestContextItemsKeys.HostProxyData, out hostProxyData) ? PartitionedClientHelper.GetSpsClientForHostId<TClient>(requestContext, hostProxyData.HostId) : requestContext.GetClient<TClient>();
    }

    private Guid? TryResolveMasterId(IVssRequestContext requestContext, Guid? identityId)
    {
      Guid masterId;
      return identityId.HasValue && IdentityHelper.TryResolveMasterId(requestContext, identityId.Value, out masterId) ? new Guid?(masterId) : identityId;
    }

    private Guid TryResolveMasterId(IVssRequestContext requestContext, Guid identityId)
    {
      Guid masterId;
      return IdentityHelper.TryResolveMasterId(requestContext, identityId, out masterId) ? masterId : identityId;
    }
  }
}
