// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.FrameworkTokenService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Tokens
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkTokenService : 
    TokenServiceBase,
    ITokenService,
    IDelegatedAuthorizationService,
    IVssFrameworkService
  {
    private readonly ConfigProxy<bool> config = new ConfigProxy<bool>("DisableFrameworkTokenCache");
    private readonly IHostLevelConversionConfiguration hostLevelConversionConfiguration;
    private const string Area = "Token";
    private const string Layer = "FrameworkTokenService";
    private const string EnablePATCreateRoutingToCentralSU = "AzureDevOps.Token.EnablePATCreateRoutingToCentralSU";

    public FrameworkTokenService()
      : this(HostLevelConversionConfiguration.Instance)
    {
    }

    public FrameworkTokenService(
      IHostLevelConversionConfiguration hostLevelConversionConfiguration)
    {
      this.hostLevelConversionConfiguration = hostLevelConversionConfiguration;
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
      throw new NotImplementedException();
    }

    public SessionTokenResult IssueSessionToken(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid orgHostId,
      Guid deploymentHostId,
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
      bool isImpersonating = false,
      string token = null,
      IDictionary<string, string> customClaims = null,
      Guid? requestedById = null)
    {
      if (userId.HasValue && userId.Value != Guid.Empty && !requestContext.IsSystemContext)
        throw new InvalidOperationException(FrameworkResources.TokenUserIdSpecifiedWithoutSystemContext());
      return TokenServiceBase.ExecuteTokenServiceResultRequest<SessionTokenResult>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, SessionTokenResult>) ((context, impersonating) =>
      {
        context.TraceEnter(1048032, "Token", nameof (FrameworkTokenService), nameof (IssueSessionToken));
        if (userId.HasValue && context.ServiceHost.Is(TeamFoundationHostType.Deployment))
          context.Trace(1048045, TraceLevel.Verbose, "Token", nameof (FrameworkTokenService), string.Format("user ID {0} specified at deployment level", (object) userId));
        if (isPublic && context.ExecutionEnvironment.IsHostedDeployment && context.GetService<IOrganizationPolicyService>().GetPolicy<bool>(context, "Policy.DisallowSecureShell", false).EffectiveValue)
        {
          context.Trace(1430016, TraceLevel.Info, "Token", nameof (FrameworkTokenService), "Organization policy has SSH keys disabled.");
          return new SessionTokenResult()
          {
            SessionTokenError = SessionTokenError.SSHPolicyDisabled
          };
        }
        try
        {
          int num;
          if (requestContext.IsFeatureEnabled("AzureDevOps.Token.EnablePATCreateRoutingToCentralSU"))
          {
            IList<Guid> guidList = targetAccounts;
            num = guidList != null ? (guidList.Count == 1 ? 1 : 0) : 0;
          }
          else
            num = 0;
          bool flag = num != 0;
          bool projectCollectionLevelEnabled = this.hostLevelConversionConfiguration.IsEnabledForSessionTokenFlow(requestContext) & flag;
          FrameworkTokenIssueHttpClient httpClient = this.GetHttpClient<FrameworkTokenIssueHttpClient>(context, projectCollectionLevelEnabled);
          SessionToken sessionToken1 = new SessionToken();
          sessionToken1.Source = source;
          Guid? nullable1 = clientId;
          sessionToken1.ClientId = nullable1 ?? Guid.Empty;
          sessionToken1.UserId = Guid.Empty;
          sessionToken1.DisplayName = name;
          sessionToken1.ValidTo = validTo.GetValueOrDefault();
          sessionToken1.Scope = scope;
          sessionToken1.TargetAccounts = targetAccounts;
          sessionToken1.PublicData = publicData;
          nullable1 = authorizationId;
          sessionToken1.AuthorizationId = nullable1 ?? Guid.Empty;
          nullable1 = accessId;
          sessionToken1.AccessId = nullable1 ?? Guid.Empty;
          sessionToken1.Token = token;
          sessionToken1.Claims = customClaims;
          Guid hostId1 = hostId;
          Guid orgHostId1 = orgHostId;
          Guid deploymentHostId1 = deploymentHostId;
          bool? nullable2 = new bool?(isPublic);
          SessionTokenType? tokenType1 = new SessionTokenType?(tokenType);
          bool? isPublic1 = nullable2;
          bool? isRequestedByTfsPatWebUI1 = new bool?(isRequestedByTfsPatWebUI);
          bool? isImpersonating1 = new bool?(impersonating);
          Guid? requestedById1 = requestedById;
          CancellationToken cancellationToken = new CancellationToken();
          SessionToken sessionToken2 = httpClient.CreateSessionTokenAsync(sessionToken1, hostId1, orgHostId1, deploymentHostId1, tokenType1, isPublic1, isRequestedByTfsPatWebUI1, isImpersonating1, requestedById: requestedById1, cancellationToken: cancellationToken).SyncResult<SessionToken>();
          return new SessionTokenResult()
          {
            SessionToken = sessionToken2
          };
        }
        finally
        {
          this.Cleanup(context);
          context.TraceLeave(1048039, "Token", nameof (FrameworkTokenService), nameof (IssueSessionToken));
        }
      }), userId, addAuthorizationId: true);
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
      return TokenServiceBase.ExecuteTokenServiceResultRequest<SessionToken>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, SessionToken>) ((context, isImpersonating) =>
      {
        context.TraceEnter(1049030, "Token", nameof (FrameworkTokenService), nameof (UpdateSessionToken));
        try
        {
          FrameworkTokenIssueHttpClient httpClient = this.GetHttpClient<FrameworkTokenIssueHttpClient>(context, this.hostLevelConversionConfiguration.IsEnabledForSessionTokenFlow(requestContext));
          Guid authorizationId1 = authorizationId;
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
          return httpClient.UpdateSessionTokenAsync(authorizationId1, sessionToken, tokenType, isPublic1, isRequestedByTfsPatWebUI, cancellationToken: cancellationToken).SyncResult<SessionToken>();
        }
        finally
        {
          this.Cleanup(context);
          context.TraceLeave(1049031, "Token", nameof (FrameworkTokenService), nameof (UpdateSessionToken));
        }
      }));
    }

    public AccessTokenResult ExchangeAppToken(
      IVssRequestContext requestContext,
      JsonWebToken appToken,
      JsonWebToken clientSecret,
      Guid? accessId = null)
    {
      ArgumentUtility.CheckForNull<JsonWebToken>(appToken, nameof (appToken));
      ArgumentUtility.CheckForNull<JsonWebToken>(clientSecret, nameof (clientSecret));
      Guid? userId = new Guid?();
      Guid result = Guid.Empty;
      if (!Guid.TryParse(appToken.NameIdentifier, out result))
        userId = new Guid?(result);
      return TokenServiceBase.ExecuteTokenServiceResultRequest<AccessTokenResult>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, AccessTokenResult>) ((context, isImpersonating) =>
      {
        context.TraceEnter(1048160, "Token", nameof (FrameworkTokenService), nameof (ExchangeAppToken));
        try
        {
          AppTokenSecretPair appInfo = new AppTokenSecretPair()
          {
            AppToken = appToken.EncodedToken,
            ClientSecret = clientSecret.EncodedToken
          };
          return this.GetHttpClient<FrameworkTokenIssueHttpClient>(context).ExchangeAppTokenAsync(appInfo, accessId).SyncResult<AccessTokenResult>();
        }
        finally
        {
          this.Cleanup(context);
          context.TraceLeave(1048169, "Token", nameof (FrameworkTokenService), nameof (ExchangeAppToken));
        }
      }), userId, true);
    }

    public JsonWebToken GenerateImplicitGrantTokenForCurrentUser(
      IVssRequestContext requestContext,
      ResponseType responseType)
    {
      requestContext.TraceEnter(1048150, "Token", nameof (FrameworkTokenService), nameof (GenerateImplicitGrantTokenForCurrentUser));
      try
      {
        throw new NotSupportedException();
      }
      finally
      {
        requestContext.TraceLeave(1048159, "Token", nameof (FrameworkTokenService), nameof (GenerateImplicitGrantTokenForCurrentUser));
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
      Guid instanceId1 = requestContext.ServiceHost.InstanceId;
      Guid instanceId2 = requestContext.ServiceHost.InstanceId;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        instanceId2 = requestContext.To(TeamFoundationHostType.Application).ServiceHost.InstanceId;
      Uri audience = (Uri) null;
      if (grantType == GrantType.ClientCredentials)
      {
        IVssRequestContext rootContext = requestContext.RootContext;
        audience = rootContext.GetService<ILocationService>().GetResourceUri(rootContext, "oauth2", OAuth2ResourceIds.Token, (object) null);
      }
      return this.Exchange(requestContext, grantType, grant, clientSecret, instanceId1, instanceId2, audience, redirectUri, accessId);
    }

    public AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      GrantType grantType,
      JsonWebToken grant,
      JsonWebToken clientSecret,
      Guid hostId,
      Guid orgHostId,
      Uri audience = null,
      Uri redirectUri = null,
      Guid? accessId = null)
    {
      requestContext.TraceEnter(1048140, "Token", nameof (FrameworkTokenService), nameof (Exchange));
      GrantTokenSecretPair tokenSecretPair = new GrantTokenSecretPair()
      {
        ClientSecret = clientSecret == null ? (string) null : clientSecret.EncodedToken,
        GrantToken = grant == null ? (string) null : grant.EncodedToken
      };
      try
      {
        return this.GetHttpClient<FrameworkTokenOauth2HttpClient>(requestContext).IssueTokenAsync(tokenSecretPair, grantType, hostId, orgHostId, audience, redirectUri, accessId).SyncResult<AccessTokenResult>();
      }
      finally
      {
        this.Cleanup(requestContext);
        requestContext.TraceLeave(1048149, "Token", nameof (FrameworkTokenService), nameof (Exchange));
      }
    }

    public AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      Guid registrationId,
      string clientSecret,
      Guid hostId,
      string requestedScopes = null)
    {
      throw new NotImplementedException();
    }

    public AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      Guid registrationId,
      string clientSecret,
      Guid hostId,
      Guid tenantId,
      string requestedScopes = null)
    {
      requestContext.TraceEnter(1048340, "Token", nameof (FrameworkTokenService), nameof (Exchange));
      return TokenServiceBase.ExecuteTokenServiceResultRequest<AccessTokenResult>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, AccessTokenResult>) ((context, isImpersonating) =>
      {
        try
        {
          AccessToken accessToken = this.config.QueryByCtx<bool>(requestContext) ? this.GetAccessToken(context, registrationId, clientSecret, hostId, tenantId, requestedScopes) : this.GetAccessTokenFromCache(context, clientSecret, false, (Func<AccessToken>) (() => this.GetAccessToken(context, registrationId, clientSecret, hostId, tenantId, requestedScopes)), registrationId);
          AccessTokenResult accessTokenResult = new AccessTokenResult()
          {
            AccessToken = accessToken.Token,
            TokenType = accessToken.TokenType,
            ValidTo = accessToken.ValidTo.UtcDateTime,
            AuthorizationId = accessToken.AuthorizationId,
            IsFirstPartyClient = accessToken.IsFirstPartyClient,
            Scope = accessToken.Scope,
            AccessTokenError = accessToken.AccessTokenError
          };
          if (accessToken.RefreshToken != null)
            accessTokenResult.RefreshToken = new RefreshTokenGrant(accessToken.RefreshToken);
          return accessTokenResult;
        }
        finally
        {
          context.TraceLeave(1048049, "Token", nameof (FrameworkTokenService), nameof (Exchange));
        }
      }));
    }

    public AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false)
    {
      requestContext.TraceEnter(1048040, "Token", nameof (FrameworkTokenService), nameof (Exchange));
      try
      {
        AccessToken accessToken = this.GetAccessToken(requestContext, accessTokenKey, isPublic);
        AccessTokenResult accessTokenResult = new AccessTokenResult()
        {
          AccessToken = accessToken.Token,
          TokenType = accessToken.TokenType,
          ValidTo = accessToken.ValidTo.UtcDateTime
        };
        if (accessToken.RefreshToken != null)
          accessTokenResult.RefreshToken = new RefreshTokenGrant(accessToken.RefreshToken);
        return accessTokenResult;
      }
      finally
      {
        requestContext.TraceLeave(1048049, "Token", nameof (FrameworkTokenService), nameof (Exchange));
      }
    }

    internal AccessToken GetAccessTokenFromCache(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic,
      Func<AccessToken> getAccessToken,
      Guid registrationId = default (Guid))
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IFrameworkTokenCacheService service = vssRequestContext.GetService<IFrameworkTokenCacheService>();
      AccessToken accessTokenResult;
      if (!service.TryGetAccessToken(vssRequestContext, accessTokenKey, out accessTokenResult))
      {
        requestContext.Trace(1048041, TraceLevel.Verbose, "Token", nameof (FrameworkTokenService), "Access token cache - cache miss");
        try
        {
          accessTokenResult = getAccessToken();
          if (!(registrationId == Guid.Empty))
          {
            if (!(registrationId == accessTokenResult.RegistrationId))
              goto label_11;
          }
          if (accessTokenResult.Token != null)
            service.SetAccessToken(vssRequestContext, accessTokenKey, accessTokenResult);
        }
        catch (AccessCheckException ex)
        {
          requestContext.TraceException(1048043, TraceLevel.Info, "Token", nameof (FrameworkTokenService), (Exception) ex);
          service.AddNoAccessTokenEntryToLocalCache(vssRequestContext, accessTokenKey);
          accessTokenResult = service.NoAccessTokenEntry;
        }
        catch (ExchangeAccessTokenKeyException ex)
        {
          requestContext.TraceException(1048043, TraceLevel.Info, "Token", nameof (FrameworkTokenService), (Exception) ex);
          service.AddNoAccessTokenEntryToLocalCache(vssRequestContext, accessTokenKey, isPublic);
          accessTokenResult = service.NoAccessTokenEntry;
        }
        catch (InvalidPersonalAccessTokenException ex)
        {
          requestContext.TraceException(1048043, TraceLevel.Info, "Token", nameof (FrameworkTokenService), (Exception) ex);
          service.AddNoAccessTokenEntryToLocalCache(vssRequestContext, accessTokenKey, isPublic);
          accessTokenResult = service.NoAccessTokenEntry;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1048043, "Token", nameof (FrameworkTokenService), ex);
          throw;
        }
      }
      else
        requestContext.Trace(1048042, TraceLevel.Verbose, "Token", nameof (FrameworkTokenService), "Access token cache - cache hit.");
label_11:
      return accessTokenResult;
    }

    public virtual AccessToken GetAccessToken(
      IVssRequestContext requestContext,
      Guid registrationId,
      string clientSecret,
      Guid hostId,
      string requestedScopes = null)
    {
      return TokenServiceBase.ExecuteTokenServiceResultRequest<AccessToken>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, AccessToken>) ((context, isImpersonating) =>
      {
        try
        {
          FrameworkTokenOauth2HttpClient httpClient = this.GetHttpClient<FrameworkTokenOauth2HttpClient>(requestContext);
          string clientSecret1 = clientSecret;
          Guid registrationId1 = registrationId;
          Guid hostId1 = hostId;
          string str = requestedScopes;
          Guid? tenantId = new Guid?();
          string requestedScopes1 = str;
          CancellationToken cancellationToken = new CancellationToken();
          AccessTokenResult accessTokenResult = httpClient.IssueApplicationTokenAsync(clientSecret1, registrationId1, hostId1, tenantId, requestedScopes1, cancellationToken: cancellationToken).SyncResult<AccessTokenResult>();
          return new AccessToken()
          {
            AccessId = accessTokenResult.AuthorizationId,
            AuthorizationId = accessTokenResult.AuthorizationId,
            IsRefresh = false,
            IsValid = accessTokenResult.HasError,
            RegistrationId = accessTokenResult.AuthorizationId,
            Token = accessTokenResult.AccessToken,
            TokenType = accessTokenResult.TokenType,
            ValidTo = (DateTimeOffset) accessTokenResult.ValidTo,
            IsFirstPartyClient = accessTokenResult.IsFirstPartyClient,
            Scope = accessTokenResult.Scope,
            AccessTokenError = accessTokenResult.AccessTokenError
          };
        }
        finally
        {
          this.Cleanup(context);
        }
      }));
    }

    public virtual AccessToken GetAccessToken(
      IVssRequestContext requestContext,
      Guid registrationId,
      string clientSecret,
      Guid hostId,
      Guid tenantId = default (Guid),
      string requestedScopes = null)
    {
      return TokenServiceBase.ExecuteTokenServiceResultRequest<AccessToken>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, AccessToken>) ((context, isImpersonating) =>
      {
        try
        {
          AccessTokenResult accessTokenResult = this.GetHttpClient<FrameworkTokenOauth2HttpClient>(requestContext).IssueApplicationTokenAsync(clientSecret, registrationId, hostId, new Guid?(tenantId), requestedScopes).SyncResult<AccessTokenResult>();
          return new AccessToken()
          {
            AccessId = accessTokenResult.AuthorizationId,
            AuthorizationId = accessTokenResult.AuthorizationId,
            IsRefresh = false,
            IsValid = accessTokenResult.HasError,
            RegistrationId = accessTokenResult.AuthorizationId,
            Token = accessTokenResult.AccessToken,
            TokenType = accessTokenResult.TokenType,
            ValidTo = (DateTimeOffset) accessTokenResult.ValidTo,
            IsFirstPartyClient = accessTokenResult.IsFirstPartyClient,
            Scope = accessTokenResult.Scope,
            AccessTokenError = accessTokenResult.AccessTokenError
          };
        }
        finally
        {
          this.Cleanup(context);
        }
      }));
    }

    public virtual AccessToken GetAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false)
    {
      return TokenServiceBase.ExecuteTokenServiceResultRequest<AccessToken>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, AccessToken>) ((context, isImpersonating) =>
      {
        try
        {
          bool projectCollectionLevelEnabled = this.hostLevelConversionConfiguration.IsEnabledForExchangeTokenFlow(requestContext);
          return this.GetHttpClient<FrameworkTokenIssueHttpClient>(context, projectCollectionLevelEnabled).ExchangeAccessTokenKeyAsync(accessTokenKey, new bool?(isPublic)).SyncResult<AccessToken>();
        }
        finally
        {
          this.Cleanup(context);
        }
      }));
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
      return TokenServiceBase.ExecuteTokenServiceResultRequest<AuthorizationDescription>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, AuthorizationDescription>) ((context, isImpersonating) =>
      {
        context.TraceEnter(1048180, "Token", nameof (FrameworkTokenService), nameof (InitiateAuthorization));
        try
        {
          return this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).InitiateAuthorizationAsync(userId, responseType, clientId, redirectUri, scopes).SyncResult<AuthorizationDescription>();
        }
        finally
        {
          this.Cleanup(context);
          context.TraceLeave(1048189, "Token", nameof (FrameworkTokenService), nameof (InitiateAuthorization));
        }
      }), new Guid?(userId));
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
      return TokenServiceBase.ExecuteTokenServiceResultRequest<AuthorizationDecision>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, AuthorizationDecision>) ((context, isImpersonating) =>
      {
        context.TraceEnter(1048170, "Token", nameof (FrameworkTokenService), nameof (Authorize));
        try
        {
          return this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).AuthorizeAsync(userId, responseType, clientId, redirectUri, scopes, authorizationId).SyncResult<AuthorizationDecision>();
        }
        finally
        {
          this.Cleanup(context);
          context.TraceLeave(1048179, "Token", nameof (FrameworkTokenService), nameof (Authorize));
        }
      }), new Guid?(userId));
    }

    public void Revoke(IVssRequestContext requestContext, Guid authorizationId) => this.Revoke(requestContext, Guid.Empty, authorizationId);

    public void Revoke(IVssRequestContext requestContext, Guid userId, Guid authorizationId) => TokenServiceBase.ExecuteTokenServiceVoidRequest(requestContext, "Token", nameof (FrameworkTokenService), (Action<IVssRequestContext>) (context =>
    {
      context.TraceEnter(1048120, "Token", nameof (FrameworkTokenService), nameof (Revoke));
      try
      {
        this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).RevokeAuthorizationAsync(authorizationId, new Guid?(userId)).SyncResult();
      }
      finally
      {
        this.Cleanup(context);
        context.TraceLeave(1048129, "Token", nameof (FrameworkTokenService), nameof (Revoke));
      }
    }), new Guid?(userId));

    public IEnumerable<AuthorizationDetails> GetAuthorizations(IVssRequestContext requestContext) => this.GetAuthorizations(requestContext, Guid.Empty);

    public IEnumerable<AuthorizationDetails> GetAuthorizations(
      IVssRequestContext requestContext,
      Guid userId)
    {
      return (IEnumerable<AuthorizationDetails>) TokenServiceBase.ExecuteTokenServiceResultRequest<List<AuthorizationDetails>>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, List<AuthorizationDetails>>) ((context, isImpersonating) =>
      {
        context.TraceEnter(1048130, "Token", nameof (FrameworkTokenService), nameof (GetAuthorizations));
        try
        {
          return this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).GetAuthorizationsAsync(new Guid?(userId)).SyncResult<List<AuthorizationDetails>>();
        }
        finally
        {
          this.Cleanup(context);
          context.TraceLeave(1048139, "Token", nameof (FrameworkTokenService), nameof (GetAuthorizations));
        }
      }), new Guid?(userId));
    }

    public IList<SessionToken> ListSessionTokens(
      IVssRequestContext requestContext,
      bool isPublic = false,
      bool includePublicData = false)
    {
      return (IList<SessionToken>) TokenServiceBase.ExecuteTokenServiceResultRequest<List<SessionToken>>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, List<SessionToken>>) ((context, isImpersonating) =>
      {
        context.TraceEnter(1048090, "Token", nameof (FrameworkTokenService), nameof (ListSessionTokens));
        try
        {
          return this.GetHttpClient<FrameworkTokenIssueHttpClient>(context, this.hostLevelConversionConfiguration.IsEnabledForSessionTokenFlow(requestContext)).GetSessionTokensAsync(new bool?(isPublic), new bool?(includePublicData)).SyncResult<List<SessionToken>>();
        }
        finally
        {
          this.Cleanup(context);
          context.TraceLeave(1048099, "Token", nameof (FrameworkTokenService), nameof (ListSessionTokens));
        }
      }));
    }

    public PagedSessionTokens ListSessionTokensByPage(
      IVssRequestContext requestContext,
      TokenPageRequest tokenPageRequest,
      bool isPublic = false,
      bool includePublicData = false)
    {
      return TokenServiceBase.ExecuteTokenServiceResultRequest<PagedSessionTokens>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, PagedSessionTokens>) ((context, isImpersonating) =>
      {
        context.TraceEnter(1048191, "Token", nameof (FrameworkTokenService), nameof (ListSessionTokensByPage));
        ArgumentUtility.CheckForNull<TokenPageRequest>(tokenPageRequest, nameof (tokenPageRequest));
        try
        {
          return this.GetHttpClient<FrameworkTokenIssueHttpClient>(context, this.hostLevelConversionConfiguration.IsEnabledForSessionTokenFlow(requestContext)).GetSessionTokensPageAsync(tokenPageRequest.DisplayFilterOption, tokenPageRequest.CreatedByOption, tokenPageRequest.SortByOption, tokenPageRequest.IsSortAscending, tokenPageRequest.StartRowNumber, tokenPageRequest.PageSize, tokenPageRequest.PageRequestTimeStamp, new bool?(isPublic), new bool?(includePublicData)).SyncResult<PagedSessionTokens>();
        }
        finally
        {
          this.Cleanup(context);
          requestContext.TraceLeave(1048192, "Token", nameof (FrameworkTokenService), nameof (ListSessionTokensByPage));
        }
      }));
    }

    public SessionToken GetSessionToken(
      IVssRequestContext requestContext,
      Guid authorizationId,
      bool isPublic = false)
    {
      return TokenServiceBase.ExecuteTokenServiceResultRequest<SessionToken>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, SessionToken>) ((context, isImpersonating) =>
      {
        context.TraceEnter(1048100, "Token", nameof (FrameworkTokenService), nameof (GetSessionToken));
        try
        {
          return this.GetHttpClient<FrameworkTokenIssueHttpClient>(context, this.hostLevelConversionConfiguration.IsEnabledForSessionTokenFlow(requestContext)).GetSessionTokenAsync(authorizationId, new bool?(isPublic)).SyncResult<SessionToken>();
        }
        finally
        {
          this.Cleanup(context);
          context.TraceLeave(1048109, "Token", nameof (FrameworkTokenService), nameof (GetSessionToken));
        }
      }));
    }

    public void RemovePublicKey(IVssRequestContext requestContext, string publicKey) => TokenServiceBase.ExecuteTokenServiceVoidRequest(requestContext, "Token", nameof (FrameworkTokenService), (Action<IVssRequestContext>) (context =>
    {
      context.TraceEnter(1048211, "Token", nameof (FrameworkTokenService), nameof (RemovePublicKey));
      try
      {
        FrameworkTokenIssueHttpClient httpClient = this.GetHttpClient<FrameworkTokenIssueHttpClient>(context);
        SshPublicKey publicData = new SshPublicKey();
        publicData.Value = publicKey;
        CancellationToken cancellationToken = new CancellationToken();
        httpClient.RemovePublicKeyAsync(publicData, true, cancellationToken: cancellationToken).SyncResult();
      }
      finally
      {
        this.Cleanup(context);
        context.TraceLeave(1048208, "Token", nameof (FrameworkTokenService), nameof (RemovePublicKey));
      }
    }));

    public void RevokeSessionToken(
      IVssRequestContext requestContext,
      Guid authorizationId,
      bool isPublic = false)
    {
      TokenServiceBase.ExecuteTokenServiceVoidRequest(requestContext, "Token", nameof (FrameworkTokenService), (Action<IVssRequestContext>) (context =>
      {
        context.TraceEnter(1048212, "Token", nameof (FrameworkTokenService), nameof (RevokeSessionToken));
        try
        {
          this.GetHttpClient<FrameworkTokenIssueHttpClient>(context, this.hostLevelConversionConfiguration.IsEnabledForSessionTokenFlow(requestContext)).RevokeSessionTokenAsync(authorizationId, new bool?(isPublic)).SyncResult();
        }
        finally
        {
          this.Cleanup(context);
          context.TraceLeave(1048208, "Token", nameof (FrameworkTokenService), nameof (RevokeSessionToken));
        }
      }));
    }

    public void RevokeAllSessionTokensOfUser(IVssRequestContext requestContext) => TokenServiceBase.ExecuteTokenServiceVoidRequest(requestContext, "Token", nameof (FrameworkTokenService), (Action<IVssRequestContext>) (context =>
    {
      context.TraceEnter(1048240, "Token", nameof (FrameworkTokenService), nameof (RevokeAllSessionTokensOfUser));
      try
      {
        this.GetHttpClient<FrameworkTokenIssueHttpClient>(context).RevokeAllSessionTokensOfUserAsync().SyncResult();
      }
      finally
      {
        this.Cleanup(context);
        context.TraceLeave(1048241, "Token", nameof (FrameworkTokenService), nameof (RevokeAllSessionTokensOfUser));
      }
    }));

    public AppSessionTokenResult IssueAppSessionToken(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? userId,
      Guid? authorizationId = null)
    {
      throw new NotImplementedException();
    }

    public AppSessionTokenResult IssueAppSessionToken(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      Guid clientId,
      Guid? authorizationId = null)
    {
      requestContext.TraceEnter(1048209, "Token", nameof (FrameworkTokenService), nameof (IssueAppSessionToken));
      try
      {
        return this.GetHttpClient<FrameworkTokenIssueHttpClient>(requestContext).IssueAppSessionTokenAsync(subjectDescriptor, clientId, authorizationId).SyncResult<AppSessionTokenResult>();
      }
      finally
      {
        this.Cleanup(requestContext);
        requestContext.TraceLeave(1048210, "Token", nameof (FrameworkTokenService), nameof (IssueAppSessionToken));
      }
    }

    public HostAuthorizationDecision AuthorizeHost(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? newId = null)
    {
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      return this.AuthorizeHost(requestContext, clientId, instanceId, newId);
    }

    public HostAuthorizationDecision AuthorizeHost(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid hostId,
      Guid? newId = null)
    {
      hostId = this.ValidateHostIdForHostAuthorization(requestContext, hostId);
      return TokenServiceBase.ExecuteTokenServiceResultRequest<HostAuthorizationDecision>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, HostAuthorizationDecision>) ((context, isImpersonating) =>
      {
        context.TraceEnter(1048091, "Token", nameof (FrameworkTokenService), nameof (AuthorizeHost));
        try
        {
          return this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).AuthorizeHostAsync(clientId, hostId, newId).SyncResult<HostAuthorizationDecision>();
        }
        finally
        {
          this.Cleanup(context);
          context.TraceLeave(1048092, "Token", nameof (FrameworkTokenService), nameof (AuthorizeHost));
        }
      }), elevateCall: true);
    }

    public void RevokeHostAuthorization(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? hostId)
    {
      if (!hostId.HasValue || hostId.Value == Guid.Empty)
        hostId = new Guid?(requestContext.ServiceHost.InstanceId);
      hostId = new Guid?(this.ValidateHostIdForHostAuthorization(requestContext, hostId.Value));
      TokenServiceBase.ExecuteTokenServiceVoidRequest(requestContext, "Token", nameof (FrameworkTokenService), (Action<IVssRequestContext>) (context =>
      {
        context.TraceEnter(1048093, "Token", nameof (FrameworkTokenService), nameof (RevokeHostAuthorization));
        try
        {
          this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).RevokeHostAuthorizationAsync(clientId, hostId).SyncResult();
        }
        finally
        {
          this.Cleanup(context);
          context.TraceLeave(1048094, "Token", nameof (FrameworkTokenService), nameof (RevokeHostAuthorization));
        }
      }), elevateCall: true);
    }

    public IList<HostAuthorization> GetHostAuthorizations(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      hostId = this.ValidateHostIdForHostAuthorization(requestContext, hostId);
      return (IList<HostAuthorization>) TokenServiceBase.ExecuteTokenServiceResultRequest<List<HostAuthorization>>(requestContext, "Token", nameof (FrameworkTokenService), (Func<IVssRequestContext, bool, List<HostAuthorization>>) ((context, isImpersonating) =>
      {
        try
        {
          return this.GetHttpClient<FrameworkTokenAuthHttpClient>(context).GetHostAuthorizationsAsync(hostId).SyncResult<List<HostAuthorization>>();
        }
        finally
        {
          this.Cleanup(context);
        }
      }), elevateCall: true);
    }

    protected Guid ValidateHostIdForHostAuthorization(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (hostId == vssRequestContext.ServiceHost.InstanceId)
        hostId = TokenServiceBase.s_TokenServiceInstanceType;
      return hostId;
    }
  }
}
