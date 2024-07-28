// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.ClientAuthTokenValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.IdentityModel.Tokens;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.TeamFoundation.Framework.Server.OAuthWhitelist;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.TokenRevocation;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  internal class ClientAuthTokenValidator : OAuth2TokenValidator
  {
    private const string TraceLayer = "ClientAuthTokenValidator";
    private const string TraceArea = "Authorization";
    private const string separator = "|";
    private static readonly Guid userService = new Guid("00000038-0000-8888-8000-000000000000");
    private static readonly IConfigPrototype<bool> identityNegativeCacheEnabledPrototype = ConfigPrototype.Create<bool>("VisualStudio.Services.Authentication.NegativeIdentityCache", false);
    private readonly IConfigQueryable<bool> identityNegativeCacheEnabledConfig;
    private readonly IAadScopesConfigurationHelper aadScopesConfigHelper;
    private IEnumerable<string> _trustedIssuers;
    private string _userAuthAudience;
    private bool _enabled;
    private bool _isDeploymentLevelOnlyService;
    internal const string AudiencePrefix = "vso:";
    private const string CheckTokenHostMappingRules = "VisualStudio.Services.Authentication.CheckTokenHostMappingRules";
    private const string OAuthWhitelistFeatureFlag = "VisualStudio.Services.OAuthWhitelist.AllowOAuthWhitelist";
    private const string AceScopeMitigationFeatureFlag = "VisualStudio.Services.Authentication.EnableAceScopeMitigation";
    private const string NegativeIdentityCacheFeatureFlag = "VisualStudio.Services.Authentication.NegativeIdentityCache";

    public ClientAuthTokenValidator()
      : this(AadScopesConfigurationHelper.Instance, ConfigProxy.Create<bool>(ClientAuthTokenValidator.identityNegativeCacheEnabledPrototype))
    {
    }

    public ClientAuthTokenValidator(
      IAadScopesConfigurationHelper aadScopesConfigHelper,
      IConfigQueryable<bool> identityNegativeCacheEnabledConfig)
      : base("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
    {
      this.aadScopesConfigHelper = aadScopesConfigHelper;
      this.identityNegativeCacheEnabledConfig = identityNegativeCacheEnabledConfig;
    }

    public override IEnumerable<string> ValidIssuers => this._trustedIssuers;

    public override OAuth2TokenValidators ValidatorType => OAuth2TokenValidators.DelegatedAuth;

    public override void Initialize(
      IVssRequestContext requestContext,
      IOAuth2SettingsService settings)
    {
      IDelegatedAuthSettings delegatedAuthSettings = settings.GetDelegatedAuthSettings(requestContext);
      this._enabled = delegatedAuthSettings.Enabled;
      this._trustedIssuers = delegatedAuthSettings.TrustedIssuers;
      this._isDeploymentLevelOnlyService = delegatedAuthSettings.IsDeploymentLevelOnlyService;
      this._userAuthAudience = settings.GetUserAuthSettings(requestContext).Audience;
    }

    public override bool CanValidateToken(IVssRequestContext requestContext, JwtSecurityToken token)
    {
      if (this._enabled && token != null && token.Payload != null && !token.Payload.ContainsKey("host_auth") && (token.Audiences == null || !token.Audiences.Contains<string>(this._userAuthAudience)))
      {
        string issuer = token.Issuer;
        if (!string.IsNullOrEmpty(issuer))
        {
          Uri result;
          return Uri.TryCreate(issuer, UriKind.Absolute, out result) ? this._trustedIssuers.Contains<string>(result.Host, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : this._trustedIssuers.Contains<string>(issuer, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        }
      }
      return false;
    }

    public override IEnumerable<IdentityDescriptor> ProcessScopes(
      IVssRequestContext requestContext,
      ClaimsPrincipal claimsPrincipal)
    {
      List<IdentityDescriptor> identityDescriptorList = (List<IdentityDescriptor>) null;
      Claim claim1;
      if (claimsPrincipal == null)
      {
        claim1 = (Claim) null;
      }
      else
      {
        IEnumerable<Claim> claims = claimsPrincipal.Claims;
        claim1 = claims != null ? claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "scp")) : (Claim) null;
      }
      Claim claim2 = claim1;
      if (claim2 != null && claim2.Value != null)
      {
        identityDescriptorList = new List<IdentityDescriptor>();
        AuthorizationScopeConfiguration configuration = requestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetConfiguration(requestContext);
        IScopeTemplateService service = requestContext.GetService<IScopeTemplateService>();
        string scopes = claim2.Value;
        bool flag;
        ref bool local = ref flag;
        string[] strArray = configuration.SplitScopes(scopes, false, out local);
        if (flag)
          return (IEnumerable<IdentityDescriptor>) null;
        foreach (string scope in strArray)
        {
          try
          {
            string scopeIdentifier;
            string[] variables;
            ScopeHelpers.DeconstructScope(scope, out scopeIdentifier, out variables);
            ScopeTemplateEntry scopeTemplateEntry = service.GetScopeTemplateEntry(requestContext, scopeIdentifier);
            if (scopeTemplateEntry != null)
              identityDescriptorList.AddRange((IEnumerable<IdentityDescriptor>) scopeTemplateEntry.GenerateScopeSubjects(variables));
          }
          catch (ArgumentException ex)
          {
            requestContext.TraceException(5510323, TraceLevel.Warning, "Authorization", nameof (ClientAuthTokenValidator), (Exception) ex);
          }
        }
      }
      return (IEnumerable<IdentityDescriptor>) identityDescriptorList;
    }

    internal override bool ValidateIdentity(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
      if (!token.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type.Equals(DelegatedAuthorizationTokenClaims.AccessId))) && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && !requestContext.Items.ContainsKey(RequestContextItemsKeys.IgnoreOAuthEnabledChecks) && requestContext.ExecutionEnvironment.IsHostedDeployment && this.IsOAuthAuthenticationDisallowed(requestContext, token))
      {
        string claimAsString = token.GetClaimAsString("appid");
        requestContext.Trace(5510300, TraceLevel.Error, "Authorization", nameof (ClientAuthTokenValidator), "Token validation failed for issuer {0}. Application ID '{1}'. OAuth authentication for host {2} is disabled", (object) token.Issuer, (object) claimAsString, (object) requestContext.ServiceHost.InstanceId);
        return false;
      }
      if (string.IsNullOrEmpty(identity.Name))
      {
        requestContext.Trace(5510300, TraceLevel.Error, "Authorization", nameof (ClientAuthTokenValidator), "Token validation failed for issuer {0}. Expected nameid claim was not found", (object) token.Issuer);
        return false;
      }
      if (!Guid.TryParse(identity.Name, out Guid _))
      {
        requestContext.Trace(5510300, TraceLevel.Error, "Authorization", nameof (ClientAuthTokenValidator), "Token validation failed for issuer {0}. Token did not contain a valid nameid claim", (object) token.Issuer);
        return false;
      }
      if (token.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type.Equals(DelegatedAuthorizationTokenClaims.AccessTokenId))))
      {
        requestContext.Trace(5510300, TraceLevel.Error, "Authorization", nameof (ClientAuthTokenValidator), "Token validation failed for issuer {0}. Token appears to be a refresh token", (object) token.Issuer);
        return false;
      }
      if (!this.aadScopesConfigHelper.IsAadScopesEnabled(requestContext))
      {
        AuthorizationScopeConfiguration configuration = requestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetConfiguration(requestContext);
        Claim claim = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type.Equals("scp")));
        if (string.IsNullOrEmpty(claim?.Value))
        {
          requestContext.Trace(5510300, TraceLevel.Error, "Authorization", nameof (ClientAuthTokenValidator), "Token validation failed for issuer {0}. Expected claim scp was not found", (object) token.Issuer);
          return false;
        }
        bool usingUriScopes = true;
        try
        {
          string normalizedScopes = configuration.NormalizeScopes(claim.Value, out usingUriScopes, true);
          if (usingUriScopes)
          {
            string uri = (string) null;
            TeamFoundationExecutionEnvironment executionEnvironment = requestContext.ExecutionEnvironment;
            if (executionEnvironment.IsOnPremisesDeployment && requestContext.RootContext is IVssWebRequestContext rootContext)
              uri = rootContext.RelativePath;
            if (string.IsNullOrEmpty(uri))
              uri = requestContext.RequestUri().OriginalString;
            executionEnvironment = requestContext.ExecutionEnvironment;
            if (executionEnvironment.IsHostedDeployment)
              uri = new UriBuilder(uri)
              {
                Path = requestContext.RelativePath()
              }.Uri.ToString();
            string requestHttpMethod = requestContext.HttpMethod();
            HttpMethod method = new HttpMethod(HttpContextFactory.Current.Request.GetHttpMethodFromMethodOverrideHeader(requestHttpMethod) ?? requestHttpMethod);
            if (!configuration.HasScopePatternMatch(normalizedScopes, uri, method, requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.EnableAceScopeMitigation")))
            {
              requestContext.Trace(5510300, TraceLevel.Error, "Authorization", nameof (ClientAuthTokenValidator), "Token validation failed for issuer {0}. Token scope claim '{1}' not valid for request", (object) token.Issuer, (object) normalizedScopes);
              return false;
            }
          }
          if (normalizedScopes.Contains("vso.authorization_grant"))
          {
            requestContext.Trace(5510322, TraceLevel.Error, "Authorization", nameof (ClientAuthTokenValidator), "Token validation failed for issuer {0}. Token scopes contains vso.authorization_grant scope, which is not valid for use as an access token", (object) token.Issuer);
            return false;
          }
          requestContext.Items.Add(RequestContextItemsKeys.Scope, (object) claim.Value);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(5510300, TraceLevel.Error, "Authorization", nameof (ClientAuthTokenValidator), ex);
          return false;
        }
      }
      if (!this.ValidateSourceClaims(requestContext, token))
      {
        requestContext.Trace(5510316, TraceLevel.Warning, "Authorization", nameof (ClientAuthTokenValidator), "Token validation failed for client IP address.");
        return false;
      }
      if (token.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type.Equals(DelegatedAuthorizationTokenClaims.AccessId))))
      {
        AuthenticationMechanism authenticationMechanism = AuthenticationMechanism.SessionToken;
        bool flag1 = AuthenticationHelpers.IsGlobalToken(token);
        bool flag2 = AuthenticationHelpers.IsUnscopedToken(token);
        if (flag1 & flag2)
          authenticationMechanism = AuthenticationMechanism.SessionToken_UnscopedGlobal;
        else if (flag1)
          authenticationMechanism = AuthenticationMechanism.SessionToken_Global;
        else if (flag2)
          authenticationMechanism = AuthenticationMechanism.SessionToken_Unscoped;
        AuthenticationHelpers.SetAuthenticationMechanism(requestContext, authenticationMechanism);
      }
      else
      {
        AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.Oauth);
        AuthenticationHelpers.SetOAuthAppId(requestContext, token);
      }
      if (!this.aadScopesConfigHelper.IsAadScopesEnabled(requestContext))
        this.SetRequestContextKeysForUserImpersonation(requestContext, token.Claims);
      return true;
    }

    internal override bool ValidateScopes(IVssRequestContext requestContext, JwtSecurityToken token)
    {
      if (!this.aadScopesConfigHelper.IsAadScopesEnabled(requestContext))
        return base.ValidateScopes(requestContext, token);
      this.SetRequestContextKeysForUserImpersonation(requestContext, token.Claims);
      return requestContext.GetService<IScopesValidationService>().ValidateScopes(requestContext, token);
    }

    internal bool ValidateSourceClaims(IVssRequestContext requestContext, JwtSecurityToken token)
    {
      Claim claim = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type.Equals("src")));
      if (string.IsNullOrEmpty(claim?.Value))
        return true;
      requestContext.Trace(5510315, TraceLevel.Info, "Authorization", nameof (ClientAuthTokenValidator), "Entering in ip token validation - token claims - {0}.", (object) claim.Value);
      IPAddress address1;
      if (!IPAddress.TryParse(requestContext.RemoteIPAddress(), out address1))
      {
        requestContext.Trace(5510317, TraceLevel.Error, "Authorization", nameof (ClientAuthTokenValidator), "Token validation failed. Application not able to get the client address {0} ", (object) requestContext.RemoteIPAddress());
        return false;
      }
      string str1 = claim.Value;
      string[] separator = new string[1]{ "|" };
      foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        if (str2.StartsWith("ipa:", StringComparison.OrdinalIgnoreCase))
        {
          IPAddress address2;
          if (IPAddress.TryParse(str2.Substring("ipa:".Length), out address2) && address2.Equals((object) address1))
          {
            requestContext.Trace(5510318, TraceLevel.Info, "Authorization", nameof (ClientAuthTokenValidator), "Token IP validation passed with clientIP {0} and claim IP {1}", (object) address1, (object) address2);
            return true;
          }
        }
        else if (str2.StartsWith("ipr:", StringComparison.OrdinalIgnoreCase))
        {
          string range = str2.Substring("ipr:".Length);
          if (IPAddressRange.IsAddressInRange(range, address1))
          {
            requestContext.Trace(5510319, TraceLevel.Info, "Authorization", nameof (ClientAuthTokenValidator), "Token IP validation passed with clientIP {0} and claim IPA {1}", (object) address1, (object) range);
            return true;
          }
        }
      }
      requestContext.Trace(5510316, TraceLevel.Error, "Authorization", nameof (ClientAuthTokenValidator), "Token validation failed for client IP address {0},  given supported source IP {1} - ", (object) address1, (object) claim.Value);
      return false;
    }

    internal override void TransformIdentityClaims(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity,
      out bool impersonating)
    {
      impersonating = false;
      Guid guid = Guid.Parse(identity.Name);
      requestContext.Trace(5510310, TraceLevel.Verbose, "Authorization", nameof (ClientAuthTokenValidator), "Looking up identity {0} from nameid claim", (object) guid);
      IVssRequestContext vssRequestContext1 = requestContext.Elevate();
      IdentityDescriptor identityDescriptor1 = (IdentityDescriptor) null;
      IReadOnlyVssIdentity matchedIdentity;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        if (requestContext.ServiceInstanceType() == ClientAuthTokenValidator.userService)
        {
          identityDescriptor1 = vssRequestContext1.GetService<IIdentityIdentifierConversionService>().GetDescriptorByMasterId(vssRequestContext1, guid);
          if (identityDescriptor1 == (IdentityDescriptor) null)
          {
            requestContext.Trace(5510311, TraceLevel.Error, "Authorization", nameof (ClientAuthTokenValidator), "Failed to find an identity descriptor {0} from nameid claim in User service", (object) guid);
            throw new JsonWebTokenValidationException("Invalid nameid claim. Identity was not found.");
          }
          matchedIdentity = (IReadOnlyVssIdentity) vssRequestContext1.GetService<IVssIdentityRetrievalService>().ResolveEligibleActor(vssRequestContext1, identityDescriptor1);
        }
        else
          matchedIdentity = (IReadOnlyVssIdentity) vssRequestContext1.GetService<IdentityService>().ReadIdentities(vssRequestContext1, (IList<Guid>) new Guid[1]
          {
            guid
          }, QueryMembership.None, (IEnumerable<string>) null).First<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      else
      {
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        IdentityInEnterpriseNegativeCache service = requestContext.GetService<IdentityInEnterpriseNegativeCache>();
        bool flag = this.identityNegativeCacheEnabledConfig.QueryByIds<bool>(requestContext, guid, instanceId);
        if ((!flag ? 0 : (service.IsIdentityAbsent(requestContext, guid) ? 1 : 0)) == 0)
        {
          identityDescriptor1 = vssRequestContext1.GetService<IdentityService>().ReadIdentities(vssRequestContext1, (IList<Guid>) new Guid[1]
          {
            guid
          }, QueryMembership.None, (IEnumerable<string>) null).First<Microsoft.VisualStudio.Services.Identity.Identity>()?.Descriptor;
          if (flag && identityDescriptor1 == (IdentityDescriptor) null)
            service.SetIdentityAbsent(requestContext, guid);
        }
        if (identityDescriptor1 == (IdentityDescriptor) null || !identityDescriptor1.IsServiceIdentityType())
        {
          IdentityDescriptor identityDescriptor2 = identityDescriptor1;
          IVssRequestContext vssRequestContext2 = vssRequestContext1.To(TeamFoundationHostType.Deployment);
          identityDescriptor1 = vssRequestContext2.GetService<IdentityService>().ReadIdentities(vssRequestContext2, (IList<Guid>) new Guid[1]
          {
            guid
          }, QueryMembership.None, (IEnumerable<string>) null).First<Microsoft.VisualStudio.Services.Identity.Identity>()?.Descriptor;
          requestContext.Trace(5510313, TraceLevel.Info, "Authorization", nameof (ClientAuthTokenValidator), "Resolve {0} as {1} in deployment level, originally resolved as {2} in current level", (object) guid, (object) identityDescriptor1, (object) identityDescriptor2);
        }
        if (identityDescriptor1 == (IdentityDescriptor) null)
        {
          requestContext.Trace(5510311, TraceLevel.Error, "Authorization", nameof (ClientAuthTokenValidator), "Failed to find an identity descriptor {0} from nameid claim", (object) guid);
          throw new JsonWebTokenValidationException("Invalid nameid claim. Identity was not found.");
        }
        matchedIdentity = (IReadOnlyVssIdentity) vssRequestContext1.GetService<IVssIdentityRetrievalService>().ResolveEligibleActor(vssRequestContext1, identityDescriptor1);
      }
      if (matchedIdentity == null)
      {
        requestContext.Trace(5510311, TraceLevel.Error, "Authorization", nameof (ClientAuthTokenValidator), "Failed to find an identity (ID: {0}, DescriptorHash: {1}) from nameid claim", (object) guid, (object) identityDescriptor1?.GetHashCode());
        throw new JsonWebTokenValidationException("Invalid nameid claim. Identity was not found.");
      }
      if (matchedIdentity.Descriptor.IsBindPendingType())
      {
        requestContext.Trace(5510311, TraceLevel.Error, "Authorization", nameof (ClientAuthTokenValidator), "Found a bind pending identity (ID: {0}, DescriptorHash: {1}) from nameid claim", (object) guid, (object) matchedIdentity.Descriptor.GetHashCode());
        throw new JsonWebTokenValidationException("Invalid nameid claim. Identity is bind pending.");
      }
      string identifier = matchedIdentity.Descriptor.Identifier;
      string property = matchedIdentity.GetProperty<string>("Domain", string.Empty);
      Claim claim1 = new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", identifier);
      Claim first = identity.FindFirst((Predicate<Claim>) (x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"));
      if (first != null)
        identity.RemoveClaim(first);
      identity.AddClaim(claim1);
      identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", property));
      identity.AddClaim(new Claim("http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider", property));
      identity.AddClaim(new Claim("IdentityTypeClaim", matchedIdentity.Descriptor.IdentityType));
      AuthorizationScopeConfiguration configuration = requestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetConfiguration(requestContext);
      bool usingUriScopes;
      string normalizedScopes = configuration.NormalizeScopes(token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type.Equals("scp"))).Value, out usingUriScopes, true);
      if (!usingUriScopes)
        identity.AddClaim(new Claim("scp", normalizedScopes));
      Claim claim2 = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type.Equals(DelegatedAuthorizationTokenClaims.ClientId)));
      if (claim2 != null)
        requestContext.RootContext.Items.Add(RequestContextItemsKeys.ClientId, (object) claim2.Value);
      Claim claim3 = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type.Equals(DelegatedAuthorizationTokenClaims.AuthorizationId)));
      if (claim3 == null && requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AuthorizationIdClaim"))
        claim3 = identity.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "https://schemas.microsoft.com/teamfoundationserver/2010/12/claims/authorizationid"));
      if (claim3 != null)
        requestContext.RootContext.Items.Add(RequestContextItemsKeys.AuthorizationId, (object) claim3.Value);
      Claim claim4 = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type.Equals(DelegatedAuthorizationTokenClaims.HostAuthorizationId)));
      if (claim4 != null)
        requestContext.RootContext.Items.Add(RequestContextItemsKeys.HostAuthorizationId, (object) claim4.Value);
      Claim claim5 = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type.Equals(DelegatedAuthorizationTokenClaims.OrchestrationId)));
      if (claim5 != null && requestContext.RootContext.OrchestrationId == null && requestContext.RootContext is IRequestContextInternal rootContext)
        rootContext.SetOrchestrationId(claim5.Value);
      if (!configuration.HasImpersonationScope(normalizedScopes, requestContext.RequestUri().OriginalString, new HttpMethod(requestContext.HttpMethod())))
        requestContext.To(TeamFoundationHostType.Deployment).Items.Add(RequestContextItemsKeys.AlternateAuthCredentialsContextKey, (object) true);
      foreach (IClientAuthenticationExtension extension in (IEnumerable<IClientAuthenticationExtension>) requestContext.GetExtensions<IClientAuthenticationExtension>(ExtensionLifetime.Service))
        extension.TransformAdditionalClaims(requestContext, identity, matchedIdentity);
    }

    private bool IsOAuthAuthenticationDisallowed(
      IVssRequestContext requestContext,
      JwtSecurityToken token)
    {
      IOrganizationPolicyService service = requestContext.GetService<IOrganizationPolicyService>();
      IVssRequestContext requestContext1 = requestContext.Elevate();
      IVssRequestContext context = requestContext1;
      bool effectiveValue = service.GetPolicy<bool>(context, "Policy.DisallowOAuthAuthentication", false).EffectiveValue;
      Guid result;
      return effectiveValue && (requestContext1.IsVirtualServiceHost() ? requestContext1.To(TeamFoundationHostType.Parent) : requestContext1).IsFeatureEnabled("VisualStudio.Services.OAuthWhitelist.AllowOAuthWhitelist") && Guid.TryParse(token.GetClaimAsString("appid"), out result) && result != Guid.Empty ? !requestContext.GetService<IOAuthWhitelistService>().IsWhitelisted(requestContext, result) : effectiveValue;
    }

    private void SetRequestContextKeysForUserImpersonation(
      IVssRequestContext requestContext,
      IEnumerable<Claim> tokenClaims)
    {
      bool flag = tokenClaims.Any<Claim>((Func<Claim, bool>) (a => a.Type.Equals("scp") && a.Value.Contains("user_impersonation")));
      AuthenticationHelpers.SetCanIssueFedAuthToken(requestContext, flag);
      AuthenticationHelpers.SetCanIssueUserAuthenticationToken(requestContext, flag);
    }

    public override bool ValidateAudience(
      IVssRequestContext requestContext,
      IEnumerable<string> audiences,
      Microsoft.IdentityModel.Tokens.SecurityToken securityToken,
      TokenValidationParameters validationParameters)
    {
      if (audiences == null || !audiences.Any<string>())
        return false;
      if ((securityToken is JwtSecurityToken jwtSecurityToken ? jwtSecurityToken.Audiences : (IEnumerable<string>) null) == null || !jwtSecurityToken.Audiences.Any<string>() || validationParameters?.ValidAudiences == null || !validationParameters.ValidAudiences.Any<string>() || !validationParameters.ValidateAudience)
        return true;
      if (requestContext == null)
        return false;
      IVssRequestContext elevatedRequestContext = requestContext.Elevate();
      bool flag = (requestContext.IsVirtualServiceHost() ? requestContext.To(TeamFoundationHostType.Parent) : requestContext).IsFeatureEnabled("VisualStudio.Services.Authentication.CheckTokenHostMappingRules");
      IList<Guid> allowedHosts = this.GetAllowedHosts(requestContext);
      IList<Guid> translatedHosts = (IList<Guid>) new List<Guid>();
      Lazy<IEnumerable<TokenRevocationRule>> lazy = new Lazy<IEnumerable<TokenRevocationRule>>((Func<IEnumerable<TokenRevocationRule>>) (() => (IEnumerable<TokenRevocationRule>) elevatedRequestContext.GetService<ITokenRevocationService>().GetAllRules(elevatedRequestContext)));
      foreach (string audience in audiences)
      {
        if (!string.IsNullOrWhiteSpace(audience))
        {
          if (audience.Contains("vso:"))
          {
            string[] source1 = audience.Split('|');
            List<string> list1 = ((IEnumerable<string>) source1).Where<string>((Func<string, bool>) (x => x.StartsWith("vso:"))).Select<string, string>((Func<string, string>) (x => x.Substring("vso:".Length))).ToList<string>();
            List<string> list2 = ((IEnumerable<string>) source1).Where<string>((Func<string, bool>) (x => !x.StartsWith("vso:"))).Select<string, string>((Func<string, string>) (x => x)).ToList<string>();
            if (!this._isDeploymentLevelOnlyService)
            {
              List<Guid> source2 = new List<Guid>();
              foreach (string input in list1)
              {
                Guid result;
                if (Guid.TryParse(input, out result) && result != Guid.Empty)
                  source2.Add(result);
              }
              if (source2.Any<Guid>() && !source2.Any<Guid>((Func<Guid, bool>) (x => allowedHosts.Any<Guid>((Func<Guid, bool>) (y => x == y)))))
              {
                if (flag)
                  translatedHosts = this.GetTranslatedAllowedHosts(jwtSecurityToken.ValidFrom, lazy.Value);
                if (!source2.Any<Guid>((Func<Guid, bool>) (x => translatedHosts.Any<Guid>((Func<Guid, bool>) (y => x == y)))))
                  return false;
              }
            }
            if (list2 != null && list2.Any<string>() && !list2.Any<string>((Func<string, bool>) (x => validationParameters.ValidAudiences.Any<string>((Func<string, bool>) (y => string.Equals(x, y))))))
              return false;
          }
          else if (!this.ValidateServiceAudience(requestContext, audience, validationParameters))
            return false;
        }
      }
      return true;
    }

    private IList<Guid> GetTranslatedAllowedHosts(
      DateTime validFrom,
      IEnumerable<TokenRevocationRule> hostTranslationRules)
    {
      List<Guid> translatedAllowedHosts = new List<Guid>();
      if (hostTranslationRules != null)
        translatedAllowedHosts = new List<Guid>(hostTranslationRules.Where<TokenRevocationRule>((Func<TokenRevocationRule, bool>) (rule => validFrom <= rule.CreatedBefore.Value && rule.RuleType == TokenRevocationRuleType.ActivatedParentHost)).Select<TokenRevocationRule, Guid>((Func<TokenRevocationRule, Guid>) (rule => rule.HostId.Value)));
      return (IList<Guid>) translatedAllowedHosts;
    }

    protected virtual IList<Guid> GetAllowedHosts(IVssRequestContext requestContext)
    {
      List<Guid> allowedHosts = new List<Guid>();
      if (requestContext != null)
      {
        for (IVssServiceHost vssServiceHost = requestContext.ServiceHost; vssServiceHost != null; vssServiceHost = vssServiceHost.ParentServiceHost)
          allowedHosts.Add(vssServiceHost.InstanceId);
      }
      return (IList<Guid>) allowedHosts;
    }
  }
}
