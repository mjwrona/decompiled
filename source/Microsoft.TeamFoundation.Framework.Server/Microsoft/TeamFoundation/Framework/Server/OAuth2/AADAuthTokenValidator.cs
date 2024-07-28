// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.AADAuthTokenValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.Authentication;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  internal class AADAuthTokenValidator : OAuth2TokenValidator
  {
    private const string TraceLayer = "AADAuthTokenValidator";
    private const string TraceArea = "Authorization";
    private static readonly IReadOnlyCollection<string> RemoveClaimsMap = (IReadOnlyCollection<string>) new List<string>()
    {
      "iat",
      "aud",
      "iss",
      "nbf",
      "exp",
      "ver",
      "nonce",
      "idp",
      "c_hash",
      "sub",
      "altsecid",
      "appid",
      "appidacr",
      "http://schemas.microsoft.com/claims/authnclassreference",
      "http://schemas.microsoft.com/identity/claims/scope",
      "http://schemas.microsoft.com/claims/authnmethodsreferences",
      "name",
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn",
      "wids"
    };
    private readonly IAadAuthenticationSessionTokenConfiguration aadAuthenticationSessionTokenConfiguration;
    private readonly IAadScopesConfigurationHelper aadScopesConfigHelper;
    private bool _enabled;
    private string _aadIssuer;
    private IEnumerable<string> _validIssuers;
    private string _s2sTenantId;
    private IEnumerable<string> _blockedAADAppIds;
    private IEnumerable<string> _msaPassthroughBlockedAADAppIds;
    private bool _createAADTenant;
    private readonly IJwtTracer _jwtTracer;

    public AADAuthTokenValidator()
      : this(AadScopesConfigurationHelper.Instance, (IAadAuthenticationSessionTokenConfiguration) new AadAuthenticationSessionTokenConfiguration(), JwtTracer.Instance)
    {
    }

    public AADAuthTokenValidator(
      IAadScopesConfigurationHelper aadScopesConfigHelper,
      IAadAuthenticationSessionTokenConfiguration aadAuthenticationSessionTokenConfiguration,
      IJwtTracer jwtTracer)
    {
      this.aadScopesConfigHelper = aadScopesConfigHelper;
      this.aadAuthenticationSessionTokenConfiguration = aadAuthenticationSessionTokenConfiguration;
      this._jwtTracer = jwtTracer;
    }

    public override IEnumerable<string> ValidIssuers => this._validIssuers;

    public override OAuth2TokenValidators ValidatorType => OAuth2TokenValidators.AAD;

    public override void Initialize(
      IVssRequestContext requestContext,
      IOAuth2SettingsService settings)
    {
      IAADAuthSettings aadAuthSettings = settings.GetAADAuthSettings(requestContext);
      IS2SAuthSettings s2SauthSettings = settings.GetS2SAuthSettings(requestContext);
      this._enabled = aadAuthSettings.Enabled;
      if (this._enabled)
      {
        this._aadIssuer = aadAuthSettings.Issuer;
        string[] strArray;
        if (!string.IsNullOrEmpty(this._aadIssuer))
          strArray = new string[1]{ this._aadIssuer };
        else
          strArray = Array.Empty<string>();
        this._validIssuers = (IEnumerable<string>) strArray;
        List<string> stringList = new List<string>(aadAuthSettings.BlockedAADAppIds);
        this._msaPassthroughBlockedAADAppIds = (IEnumerable<string>) new List<string>(aadAuthSettings.MsaPassthroughBlockedAADAppIds);
        this._createAADTenant = aadAuthSettings.CreateAADTenant;
        this._s2sTenantId = s2SauthSettings.TenantId;
        stringList.AddRange(s2SauthSettings.FirstPartyServicePrincipals);
        this._blockedAADAppIds = (IEnumerable<string>) stringList;
      }
      else
        this._validIssuers = (IEnumerable<string>) Array.Empty<string>();
    }

    public override bool CanValidateToken(IVssRequestContext requestContext, JwtSecurityToken token) => this._enabled && this.IsValidIssuer(token) && this.HasValidClaims(token) && this.HasValidAppId(token);

    internal override bool ValidateIdentity(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
      if (!token.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type == "tid")))
      {
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.IdentityValidationFailure, TraceLevel.Warning, "Authorization", nameof (AADAuthTokenValidator), "Failed to validate token for issuer {0}. Expected tid claim not found.", (object) token.Issuer);
        return false;
      }
      Claim appIdClaim = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "appid"));
      if (appIdClaim != null && this._blockedAADAppIds.Any<string>((Func<string, bool>) (x => x.Equals(appIdClaim.Value, StringComparison.OrdinalIgnoreCase))))
      {
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.IdentityValidationFailure, TraceLevel.Warning, "Authorization", nameof (AADAuthTokenValidator), "Failed to validate token for issuer {0}. appid is blocked.", (object) token.Issuer);
        return false;
      }
      if (!token.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type == "upn")) && !token.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type == "unique_name")))
      {
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.IdentityValidationFailure, TraceLevel.Warning, "Authorization", nameof (AADAuthTokenValidator), "Failed to validate token for issuer {0}. Expected a upn or unique_name claim.", (object) token.Issuer);
        return false;
      }
      if (!AADAuthTokenValidator.TokenContainsOidOrWidsClaim(token))
      {
        string identityPuid = AADAuthTokenValidator.GetIdentityPuid(requestContext, token, identity);
        if (string.IsNullOrEmpty(identityPuid) || identityPuid == "aad:")
        {
          requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.IdentityValidationFailure, TraceLevel.Warning, "Authorization", nameof (AADAuthTokenValidator), "Failed to validate token for issuer {0} with Claims {1}. Token represents a MSA pass through token but expected altsecid or a puid claim was not found", (object) token.Issuer, (object) token.RawPayload);
          requestContext.Items.Add(RequestContextItemsKeys.ValidatorIdentityError, (object) "The user is not a member of the AAD directory that the target Organization is connected to.");
          return false;
        }
        if (appIdClaim == null || this._msaPassthroughBlockedAADAppIds.Any<string>((Func<string, bool>) (x => x.Equals(appIdClaim.Value, StringComparison.OrdinalIgnoreCase))))
        {
          if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.OrgIdDisambiguation"))
          {
            requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.IdentityValidationFailure, TraceLevel.Warning, "Authorization", nameof (AADAuthTokenValidator), "Failed to validate token for issuer {0}. Expected a oid claim. Disambiguation feature is not enabled.", (object) token.Issuer);
            return false;
          }
          if (token.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type == "scp" && x.Value.Equals("user_impersonation", StringComparison.OrdinalIgnoreCase))))
          {
            requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.IdentityValidationFailure, TraceLevel.Warning, "Authorization", nameof (AADAuthTokenValidator), "Failed to validate token for issuer {0}. Token without an OID claim was found with disambiguation flag enabled, but token is a user_impersonation token.", (object) token.Issuer);
            return false;
          }
        }
      }
      if (token.Payload.TryGetValue<string>("nonce", out string _))
      {
        string str;
        if (!requestContext.Items.TryGetValue<string>("nonce", out str))
          str = AadAuthUrlUtility.ParseState(requestContext)["nonce"];
        if (!nonce.Equals(str, StringComparison.OrdinalIgnoreCase))
        {
          requestContext.Trace(5510102, TraceLevel.Warning, "Authorization", nameof (AADAuthTokenValidator), "Failed to validate token for issuer {0}. Found mismatch between token nonce and request nonce", (object) token.Issuer);
          return false;
        }
        Guid result;
        SessionTrackingState sessionState;
        if (requestContext.GetSessionTrackingState(out sessionState) && sessionState != null && Guid.TryParse(nonce, out result) && (sessionState.PendingAuthenticationSessionId == Guid.Empty || result != sessionState.PendingAuthenticationSessionId))
        {
          requestContext.TraceDataConditionally(5510103, TraceLevel.Warning, "Authorization", nameof (AADAuthTokenValidator), "Failed to validate token for the issuer. Found mismatch between token nonce and session cookie nonce", (Func<object>) (() => (object) new
          {
            TokenIssuer = token.Issuer,
            SessionTrackingState = sessionState,
            IdTokenNonce = nonce
          }), nameof (ValidateIdentity));
          if (requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.AADAuthTokenValidator.NonceCheck"))
            return false;
        }
      }
      this.SetAuthenticationMechanism(requestContext, token);
      AuthenticationHelpers.SetOAuthAppId(requestContext, token);
      AuthenticationHelpers.SetCanIssueUserAuthenticationToken(requestContext, true);
      AuthenticationHelpers.SetCanIssueFedAuthToken(requestContext, true);
      return true;
    }

    protected virtual void SetAuthenticationMechanism(
      IVssRequestContext requestContext,
      JwtSecurityToken token)
    {
      if (this.IsAadWebAuthentication(requestContext, token.Claims))
        AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.AAD_Web);
      else if (token.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type == "appid" && x.Value.Equals("c44b4083-3bb0-49c1-b47d-974e53cbdf3c", StringComparison.OrdinalIgnoreCase))))
        AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.AAD_Ibiza);
      else if (token.Audiences != null && token.Audiences.Any<string>((Func<string, bool>) (x => x.Contains("management.core.windows.net"))))
        AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.AAD_ARM);
      else if (token.Audiences != null && token.Audiences.Contains<string>(requestContext.RequestUri().Host, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.AAD_Scoped);
      else
        AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.AAD);
    }

    internal override bool ValidateScopes(IVssRequestContext requestContext, JwtSecurityToken token)
    {
      if (!this.aadScopesConfigHelper.IsAadScopesEnabled(requestContext) || !requestContext.GetAuthenticationMechanism().Equals("AAD"))
        return base.ValidateScopes(requestContext, token);
      if (token.Claims.Any<Claim>((Func<Claim, bool>) (a => a.Type.Equals("scp") && a.Value.Contains("user_impersonation"))))
        return true;
      int num = requestContext.GetService<IScopesValidationService>().ValidateScopes(requestContext, token) ? 1 : 0;
      if (num == 0)
        return num != 0;
      requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.AadTokenScopesValidation, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "AAD token validation succeded for issuer {0}. Scopes found: {1}", (object) token.Issuer, (object) token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (f => f.Type.Equals("scp")))?.Value);
      return num != 0;
    }

    private bool IsAadWebAuthentication(
      IVssRequestContext requestContext,
      IEnumerable<Claim> claims)
    {
      return this.aadScopesConfigHelper.IsAadScopesEnabled(requestContext) ? !claims.Any<Claim>((Func<Claim, bool>) (x => x.Type.Equals("scp") || x.Type.Equals("roles"))) : !claims.Any<Claim>((Func<Claim, bool>) (x => x.Type.Equals("scp") && x.Value.Equals("user_impersonation", StringComparison.OrdinalIgnoreCase)));
    }

    private bool IsValidIssuer(JwtSecurityToken token)
    {
      string issuer = token.Issuer;
      Uri result;
      return !string.IsNullOrEmpty(issuer) && issuer.IndexOf(this._s2sTenantId, StringComparison.OrdinalIgnoreCase) == -1 && Uri.TryCreate(issuer, UriKind.Absolute, out result) && string.Equals(result.Host, this._aadIssuer, StringComparison.OrdinalIgnoreCase);
    }

    private bool HasValidClaims(JwtSecurityToken token) => token.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type == "tid")) && (token.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type == "upn")) || token.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type == "unique_name")));

    private bool HasValidAppId(JwtSecurityToken token)
    {
      Claim appIdClaim = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "appid"));
      return appIdClaim == null || !this._blockedAADAppIds.Any<string>((Func<string, bool>) (x => x.Equals(appIdClaim.Value, StringComparison.OrdinalIgnoreCase)));
    }

    internal override void TransformIdentityClaims(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity,
      out bool impersonating)
    {
      impersonating = false;
      AADAuthTokenValidator.PreProcessIdentityClaims(identity);
      bool result;
      bool.TryParse(HttpContextFactory.Current.Request.Headers.Get("X-VSS-ForceMsaPassThrough"), out result);
      if (!AADAuthTokenValidator.TokenContainsOidOrWidsClaim(token))
        AADAuthTokenValidator.ProcessMsaClaims(requestContext, token, identity);
      else if (result && !string.IsNullOrEmpty(AADAuthTokenValidator.GetAltSecId(token)))
      {
        AADAuthTokenValidator.ProcessMsaClaims(requestContext, token, identity);
        AADAuthTokenValidator.RemoveClaimsFromIdentity(identity, (IEnumerable<string>) new List<string>()
        {
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname",
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"
        });
      }
      else
      {
        AADAuthTokenValidator.ProcessAadClaims(requestContext, token, identity);
        this.EnsureTenantExists(requestContext, token, identity);
      }
      this.ProcessAadTokens(requestContext, token, identity);
      AADAuthTokenValidator.RemoveClaimsFromIdentity(identity, (IEnumerable<string>) AADAuthTokenValidator.RemoveClaimsMap);
      NameValueCollection state = AadAuthUrlUtility.ParseState(requestContext);
      if (state != null && state.Count > 0 && requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.OrgIdDisambiguation"))
        requestContext.To(TeamFoundationHostType.Deployment).Items.Add("IsAadAuthFlow", (object) true);
      requestContext.RootContext.Items["AuthenticationByIdentityProvider"] = (object) true;
      requestContext.RootContext.Items["CredentialValidFrom"] = (object) token.ValidFrom;
      this.SetAadAuthFlow(requestContext);
    }

    protected virtual void SetAadAuthFlow(IVssRequestContext requestContext)
    {
      if (requestContext.To(TeamFoundationHostType.Deployment).Items.Keys.Contains("IsAadAuthFlow"))
        return;
      requestContext.To(TeamFoundationHostType.Deployment).Items.Add("IsAadAuthFlow", (object) true);
    }

    private static void PreProcessIdentityClaims(ClaimsIdentity identity)
    {
      Claim claim1 = identity.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"));
      if (claim1 != null)
        identity.RemoveClaim(claim1);
      Claim claim2 = identity.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"));
      if (claim2 != null)
        identity.RemoveClaim(claim2);
      Claim claim3 = identity.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type.Equals("PUID", StringComparison.OrdinalIgnoreCase)));
      if (claim3 == null)
        return;
      identity.RemoveClaim(claim3);
    }

    private static void ProcessAadClaims(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
      string str1 = token.Claims.First<Claim>((Func<Claim, bool>) (x => x.Type == "tid")).Value;
      string uniqueName = AADAuthTokenValidator.GetUniqueName(requestContext, token, identity);
      string str2 = string.Format("{0}\\{1}", (object) str1, (object) uniqueName);
      string str3 = AADAuthTokenValidator.GetIdentityPuid(requestContext, token, identity);
      string identityProvider = AADAuthTokenValidator.GetIdentityProvider(requestContext, token, identity);
      string errorMsg = string.Empty;
      IdentityMetaType cspPartnerUserType;
      string cspPartnerUserPuid;
      if (CspTokenHelper.TryParseTokenAsCspPartner(requestContext, token, out cspPartnerUserType, out cspPartnerUserPuid, out errorMsg))
      {
        str3 = cspPartnerUserPuid;
        str2 = str1 + "\\" + str3;
        identity.AddClaim(new Claim("CspPartner", cspPartnerUserType.ToString()));
      }
      if (!string.IsNullOrEmpty(errorMsg))
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.AadTokenError, TraceLevel.Warning, "Authorization", nameof (AADAuthTokenValidator), "Could not parse token as a CSP partner. Error message: " + errorMsg);
      identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", str2));
      identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", uniqueName));
      identity.AddClaim(new Claim("http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider", str1));
      identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", identityProvider));
      if (!string.IsNullOrEmpty(str3))
        identity.AddClaim(new Claim("PUID", str3));
      AADAuthTokenValidator.EnsureEmailClaim(requestContext, identity, uniqueName);
    }

    protected virtual void ProcessAadTokens(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
      Claim claim = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "scp"));
      if (claim != null && claim.Value.Equals("user_impersonation", StringComparison.OrdinalIgnoreCase))
      {
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.OrgIdDisambiguation"))
        {
          requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.TenantDisambiguationDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Setting tenant disambiguation claim for AAD identity {0} to false", (object) identity.Name);
          identity.AddClaim(new Claim("tenant_disambiguate", "false"));
        }
        if (!this.aadAuthenticationSessionTokenConfiguration.IssueAadAuthenticationCookieEnabled(requestContext, token))
        {
          requestContext.To(TeamFoundationHostType.Deployment).Items[RequestContextItemsKeys.AlternateAuthCredentialsContextKey] = (object) true;
          requestContext.To(TeamFoundationHostType.Deployment).Items[RequestContextItemsKeys.AlternateAuthCredentialsIdentityCreatorContextKey] = (object) true;
          requestContext.To(TeamFoundationHostType.Deployment).Items[RequestContextItemsKeys.AadAccessTokenUsedAsAlternateAuthCredentialsContextKey] = (object) true;
        }
        string rawData = token.RawData;
        if (string.IsNullOrEmpty(rawData))
          return;
        requestContext.RootContext.Items.Add(RequestContextItemsKeys.AadAuthorizationToken, (object) rawData);
      }
      else
      {
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.OrgIdDisambiguation"))
        {
          requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.TenantDisambiguationDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Setting tenant disambiguation claim for AAD identity {0} to true", (object) identity.Name);
          identity.AddClaim(new Claim("tenant_disambiguate", "true"));
        }
        string str = HttpContextFactory.Current?.Request?.Params?["code"];
        if (string.IsNullOrEmpty(str))
          return;
        IVssRequestContext rootContext = requestContext.RootContext;
        rootContext.Items.Add(RequestContextItemsKeys.AadAuthorizationCode, (object) str);
        rootContext.Items.Add(RequestContextItemsKeys.AadAuthorizationToken, (object) token.RawData);
        if (!AADAuthTokenValidator.IsSilentAadProfileRequest(rootContext))
          return;
        rootContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.FoundSilentProfileCreationRequest, TraceLevel.Info, nameof (AADAuthTokenValidator), "Authorization", "Found query {0}, adding {1} to rootContext.Items User (ClaimsIdentity.Name) {2}", (object) "request_silent_aad_profile", (object) "PreAuthSilentAadProfileCreationRequested", (object) identity.Name);
        rootContext.Items.Add("PreAuthSilentAadProfileCreationRequested", (object) bool.TrueString);
      }
    }

    internal static bool IsSilentAadProfileRequest(IVssRequestContext rootContext)
    {
      rootContext.TraceEnter(TracePoints.AADAuthTokenValidatorTracePoints.IsSilentAadProfileRequestEnter, nameof (AADAuthTokenValidator), "Authorization", nameof (IsSilentAadProfileRequest));
      string str = string.Empty;
      try
      {
        str = AadAuthUrlUtility.ParseState(rootContext)["reply_to"];
        if (string.IsNullOrWhiteSpace(str) || !str.Contains("request_silent_aad_profile") || string.IsNullOrWhiteSpace(str))
          return false;
        str = HttpUtility.UrlDecode(str);
        Uri uri = new Uri(str);
        if (uri != (Uri) null)
        {
          string query = uri.Query;
          if (!string.IsNullOrWhiteSpace(query))
          {
            NameValueCollection queryString = HttpUtility.ParseQueryString(query);
            if (queryString != null && !string.IsNullOrWhiteSpace(queryString["request_silent_aad_profile"]) && queryString["request_silent_aad_profile"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase))
              return true;
          }
        }
        return false;
      }
      catch (Exception ex)
      {
        rootContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.IsSilentAadProfileRequestException, TraceLevel.Error, "Authorization", nameof (AADAuthTokenValidator), "Error Examining reply_to.  reply_to {1}", (object) str);
        rootContext.TraceException(TracePoints.AADAuthTokenValidatorTracePoints.IsSilentAadProfileRequestException, "Authorization", nameof (AADAuthTokenValidator), ex);
        return false;
      }
      finally
      {
        rootContext.TraceLeave(TracePoints.AADAuthTokenValidatorTracePoints.IsSilentAadProfileRequestLeave, nameof (AADAuthTokenValidator), "Authorization", nameof (IsSilentAadProfileRequest));
      }
    }

    private static void ProcessMsaClaims(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
      string identityPuid = AADAuthTokenValidator.GetIdentityPuid(requestContext, token, identity);
      string uniqueName = AADAuthTokenValidator.GetUniqueName(requestContext, token, identity);
      string accountName = AADAuthTokenValidator.GetAccountName(requestContext, identityPuid, uniqueName);
      identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", identityPuid + "@Live.com"));
      identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", accountName));
      identity.AddClaim(new Claim("http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider", "Windows Live ID"));
      identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "Windows Live ID"));
      identity.AddClaim(new Claim("PUID", identityPuid));
      identity.AddClaim(new Claim("msapt", "true"));
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.OrgIdDisambiguation"))
      {
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.TenantDisambiguationDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Setting tenant disambiguation claim for MSA identity {0}", (object) identity.Name);
        identity.AddClaim(new Claim("tenant_disambiguate", "true"));
      }
      AADAuthTokenValidator.EnsureEmailClaim(requestContext, identity, uniqueName);
      AADAuthTokenValidator.EnsureDisplayNameClaims(requestContext, identity, uniqueName, accountName);
      List<string> claimsToRemove = new List<string>()
      {
        "http://schemas.microsoft.com/identity/claims/objectidentifier",
        "http://schemas.microsoft.com/identity/claims/tenantid"
      };
      AADAuthTokenValidator.RemoveClaimsFromIdentity(identity, (IEnumerable<string>) claimsToRemove);
    }

    private static void EnsureEmailClaim(
      IVssRequestContext requestContext,
      ClaimsIdentity identity,
      string uniqueName)
    {
      bool canAddEmailClaim = !string.IsNullOrEmpty(uniqueName) && !identity.HasClaim((Predicate<Claim>) (x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"));
      bool shouldAddEmailClaim;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisableCustomProcessingForEmaillessClaims"))
      {
        shouldAddEmailClaim = canAddEmailClaim;
        requestContext.TraceDataConditionally(TracePoints.AADAuthTokenValidatorTracePoints.EmailClaimDecision, TraceLevel.Verbose, "Authorization", nameof (AADAuthTokenValidator), "Not enforcing formatting of email claim since feature is disabled", (Func<object>) (() => (object) new
        {
          canAddEmailClaim = canAddEmailClaim,
          shouldAddEmailClaim = shouldAddEmailClaim,
          uniqueName = uniqueName,
          feature = "VisualStudio.Services.Identity.DisableCustomProcessingForEmaillessClaims"
        }), nameof (EnsureEmailClaim));
      }
      else
      {
        bool isValidEmailAddress = ArgumentUtility.IsValidEmailAddress(uniqueName);
        shouldAddEmailClaim = canAddEmailClaim & isValidEmailAddress;
        requestContext.TraceDataConditionally(TracePoints.AADAuthTokenValidatorTracePoints.EmailClaimDecision, TraceLevel.Verbose, "Authorization", nameof (AADAuthTokenValidator), "Enforcing formatting of email claim", (Func<object>) (() => (object) new
        {
          isValidEmailAddress = isValidEmailAddress,
          canAddEmailClaim = canAddEmailClaim,
          shouldAddEmailClaim = shouldAddEmailClaim,
          uniqueName = uniqueName
        }), nameof (EnsureEmailClaim));
      }
      if (!shouldAddEmailClaim)
        return;
      identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", uniqueName));
      requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.EmailClaimDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Email {0} was not found for identity {1}. Defaulting email claim to {2}", (object) "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", (object) identity.Name, (object) uniqueName);
    }

    private static void EnsureDisplayNameClaims(
      IVssRequestContext requestContext,
      ClaimsIdentity identity,
      string uniqueName,
      string accountName)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisableCustomProcessingForEmaillessClaims"))
        requestContext.TraceDataConditionally(TracePoints.AADAuthTokenValidatorTracePoints.DisplayNameClaimDecision, TraceLevel.Verbose, "Authorization", nameof (AADAuthTokenValidator), "No action due since feature is disabled", (Func<object>) (() => (object) new
        {
          uniqueName = uniqueName,
          accountName = accountName,
          feature = "VisualStudio.Services.Identity.DisableCustomProcessingForEmaillessClaims"
        }), nameof (EnsureDisplayNameClaims));
      else if (identity.HasClaim((Predicate<Claim>) (x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")))
        requestContext.TraceDataConditionally(TracePoints.AADAuthTokenValidatorTracePoints.DisplayNameClaimDecision, TraceLevel.Verbose, "Authorization", nameof (AADAuthTokenValidator), "Given name claim is already present; no action required", (Func<object>) (() => (object) new
        {
          uniqueName = uniqueName,
          accountName = accountName
        }), nameof (EnsureDisplayNameClaims));
      else if (string.Equals(uniqueName, accountName, StringComparison.OrdinalIgnoreCase))
      {
        requestContext.TraceDataConditionally(TracePoints.AADAuthTokenValidatorTracePoints.DisplayNameClaimDecision, TraceLevel.Verbose, "Authorization", nameof (AADAuthTokenValidator), "Account name is equal to unique name; no action required", (Func<object>) (() => (object) new
        {
          uniqueName = uniqueName,
          accountName = accountName
        }), nameof (EnsureDisplayNameClaims));
      }
      else
      {
        requestContext.TraceDataConditionally(TracePoints.AADAuthTokenValidatorTracePoints.DisplayNameClaimDecision, TraceLevel.Verbose, "Authorization", nameof (AADAuthTokenValidator), "Filling in missing display name from unique name which is different than account name", (Func<object>) (() => (object) new
        {
          uniqueName = uniqueName,
          accountName = accountName
        }), nameof (EnsureDisplayNameClaims));
        identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", uniqueName));
        identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", string.Empty));
      }
    }

    private static string GetUniqueName(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
      Claim claim = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "upn"));
      if (claim != null)
      {
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.UniqueNameClaimDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Found claim {0} in token. Using claim value {1} as identity unique name for identity {2}", (object) "upn", (object) claim, (object) identity.Name);
        return claim.Value;
      }
      string uniqueName = token.Claims.First<Claim>((Func<Claim, bool>) (x => x.Type == "unique_name")).Value;
      if (uniqueName.Contains("#EXT#"))
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.UniqueNameClaimDecision, TraceLevel.Error, "Authorization", nameof (AADAuthTokenValidator), "Found #EXT# in unique name {0}. Raw token payload: {1}", (object) uniqueName, (object) token.RawPayload);
      if (uniqueName.StartsWith("live.com#", StringComparison.OrdinalIgnoreCase))
      {
        uniqueName = uniqueName.Substring("live.com#".Length);
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.UniqueNameClaimDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Found claim {0} in token with live id prefix {1} for identity {2}.", (object) "unique_name", (object) uniqueName, (object) identity.Name);
      }
      else if (uniqueName.StartsWith("google.com#", StringComparison.OrdinalIgnoreCase))
      {
        uniqueName = uniqueName.Substring("google.com#".Length);
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.UniqueNameClaimDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Found claim {0} in token with google id prefix {1} for identity {2}.", (object) "unique_name", (object) uniqueName, (object) identity.Name);
      }
      else if (uniqueName.StartsWith("mail#", StringComparison.OrdinalIgnoreCase))
      {
        uniqueName = uniqueName.Substring("mail#".Length);
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.UniqueNameClaimDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Found claim {0} in token with one time password prefix {1} for identity {2}.", (object) "unique_name", (object) uniqueName, (object) identity.Name);
      }
      requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.UniqueNameClaimDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Unique name resolved to {0} for identity {1}", (object) uniqueName, (object) identity.Name);
      return uniqueName;
    }

    private static string GetAccountName(
      IVssRequestContext requestContext,
      string puid,
      string uniqueName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(puid, nameof (puid));
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisableCustomProcessingForEmaillessClaims"))
      {
        requestContext.TraceDataConditionally(TracePoints.AADAuthTokenValidatorTracePoints.UniqueNameClaimDecision, TraceLevel.Verbose, "Authorization", nameof (AADAuthTokenValidator), "Using unique name with fallback to puid since feature is disabled", (Func<object>) (() => (object) new
        {
          puid = puid,
          uniqueName = uniqueName,
          feature = "VisualStudio.Services.Identity.DisableCustomProcessingForEmaillessClaims"
        }), nameof (GetAccountName));
        return !string.IsNullOrEmpty(uniqueName) ? uniqueName : puid;
      }
      if (!string.IsNullOrEmpty(uniqueName) && ArgumentUtility.IsValidEmailAddress(uniqueName))
      {
        requestContext.TraceDataConditionally(TracePoints.AADAuthTokenValidatorTracePoints.UniqueNameClaimDecision, TraceLevel.Verbose, "Authorization", nameof (AADAuthTokenValidator), "Using unique name as account name since it matches the format of an email address", (Func<object>) (() => (object) new
        {
          puid = puid,
          uniqueName = uniqueName
        }), nameof (GetAccountName));
        return uniqueName;
      }
      string accountName = puid + "@Live";
      requestContext.TraceDataConditionally(TracePoints.AADAuthTokenValidatorTracePoints.UniqueNameClaimDecision, TraceLevel.Verbose, "Authorization", nameof (AADAuthTokenValidator), "Using PUID-based pseudo-email as account name since unique name does not match the format of an email address", (Func<object>) (() => (object) new
      {
        puid = puid,
        uniqueName = uniqueName,
        accountName = accountName
      }), nameof (GetAccountName));
      return accountName;
    }

    private static string GetAltSecId(JwtSecurityToken token)
    {
      Claim claim = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type.Equals("altsecid", StringComparison.InvariantCultureIgnoreCase)));
      if (claim == null)
        return string.Empty;
      string str = claim.Value;
      return string.IsNullOrEmpty(str) || !str.StartsWith("1:live.com:", StringComparison.OrdinalIgnoreCase) ? string.Empty : str.Substring("1:live.com:".Length);
    }

    private static string GetIdentityPuid(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
      string altSecId = AADAuthTokenValidator.GetAltSecId(token);
      if (!string.IsNullOrEmpty(altSecId))
      {
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.PuidClaimDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Found altsecid claim in token issued by {0}. Using altsecid claim as Identity PUID for identity {1}", (object) token.Issuer, (object) identity.Name);
        return altSecId;
      }
      Claim claim1 = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "idp"));
      Claim claim2 = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "puid"));
      if (claim2 != null)
      {
        if (claim1 != null && AuthenticationHelpers.ShouldTreatIdentityProviderAsMsa(requestContext, claim1.Value))
        {
          requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.PuidClaimDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Found Live.com or google.com or one time password word puid claim in token issued by {0}. Using puid claim as Identity PUID for identity {1}", (object) token.Issuer, (object) identity.Name);
          return claim2.Value;
        }
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.PuidClaimDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Found AAD puid claim in token issued by {0}. Using puid claim as Identity PUID for identity {1}", (object) token.Issuer, (object) identity.Name);
        return "aad:" + claim2.Value;
      }
      requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.PuidClaimDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "No applicable PUID claim found for identity {1}", (object) token.Issuer, (object) identity.Name);
      return (string) null;
    }

    private static string GetIdentityProvider(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
      Claim claim = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "idp"));
      if (claim != null && AuthenticationHelpers.ShouldTreatIdentityProviderAsMsa(requestContext, claim.Value))
      {
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.IdentityProviderClaimDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Setting identity provider claim to {0} for token issued by {1} and identity {2}", (object) "uri:WindowsLiveId", (object) token.Issuer, (object) identity.Name);
        return "uri:WindowsLiveId";
      }
      requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.IdentityProviderClaimDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Setting identity provider claim to {0} for token issued by {1} and identity {2}", (object) "uri:MicrosoftOnline", (object) token.Issuer, (object) identity.Name);
      return "uri:MicrosoftOnline";
    }

    private void EnsureTenantExists(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
      if (!this._createAADTenant)
      {
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.TenantCreationDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Skipping tenant check for identity {0}. CreateAADTenant is disabled in settings.", (object) identity.Name);
      }
      else
      {
        Guid tenantId = Guid.Parse(token.Claims.First<Claim>((Func<Claim, bool>) (x => x.Type == "tid")).Value);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        IAuthTenantService service = vssRequestContext.GetService<IAuthTenantService>();
        if (service.GetTenant(vssRequestContext, tenantId) != null)
          return;
        requestContext.Trace(TracePoints.AADAuthTokenValidatorTracePoints.TenantCreationDecision, TraceLevel.Info, "Authorization", nameof (AADAuthTokenValidator), "Tenant {0} for identity {1} not found. Creating new tenant.", (object) tenantId, (object) identity.Name);
        service.CreateTenant(vssRequestContext, new Tenant()
        {
          TenantId = tenantId,
          Issuer = token.Issuer
        });
      }
    }

    internal override void TraceTokenDetails(
      IVssRequestContext requestContext,
      JwtSecurityToken token)
    {
      this._jwtTracer.Trace(requestContext, token);
    }

    private static void RemoveClaimsFromIdentity(
      ClaimsIdentity identity,
      IEnumerable<string> claimsToRemove)
    {
      foreach (string type in claimsToRemove)
      {
        foreach (Claim claim in identity.FindAll(type))
          identity.TryRemoveClaim(claim);
      }
    }

    private static bool TokenContainsOidOrWidsClaim(JwtSecurityToken token)
    {
      foreach (Claim claim in token.Claims)
      {
        if (StringComparer.OrdinalIgnoreCase.Equals(claim.Type, "oid") || StringComparer.OrdinalIgnoreCase.Equals(claim.Type, "wids"))
          return true;
      }
      return false;
    }

    private static class Features
    {
      public const string DisableCustomProcessingForEmaillessClaims = "VisualStudio.Services.Identity.DisableCustomProcessingForEmaillessClaims";
    }
  }
}
