// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.OAuth2AuthenticationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.Settings;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  internal class OAuth2AuthenticationService : IOAuth2AuthenticationService, IVssFrameworkService
  {
    private const string c_DelegatedAuthorizationFeatureFlag = "VisualStudio.DelegatedAuthorizationService.L2TestOnly.DelegatedAuthorizationService";
    private IOAuth2SettingsService _settings;
    private TokenValidationParameters _tokenValidationParameters;
    private ILockName _initLock;
    private long _oauthSettingsVersion;
    private IDisposableReadOnlyList<IOAuth2TokenValidator> m_validators;
    private IDictionary<string, IList<IOAuth2TokenValidator>> m_validatorMap;
    public const string TraceArea = "Authentication";
    public const string TraceLayer = "Service";
    public const string ValidatorClaimType = "OAuth2Validator";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      this._settings = requestContext.To(TeamFoundationHostType.Deployment).GetService<IOAuth2SettingsService>();
      this._oauthSettingsVersion = 0L;
      this._initLock = requestContext.ServiceHost.CreateUniqueLockName(nameof (OAuth2AuthenticationService));
      this.VerifyParameters(requestContext);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      if (this.m_validators == null)
        return;
      this.m_validators.Dispose();
      this.m_validators = (IDisposableReadOnlyList<IOAuth2TokenValidator>) null;
    }

    internal IDisposableReadOnlyList<IOAuth2TokenValidator> Validators => this.m_validators;

    private static bool CallTokenService(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return false;
      return !requestContext.IsFeatureEnabled("VisualStudio.DelegatedAuthorizationService.L2TestOnly.DelegatedAuthorizationService") || !requestContext.ExecutionEnvironment.IsDevFabricDeployment;
    }

    private JwtSecurityToken ExchangeToken(IVssRequestContext requestContext, string encodedToken)
    {
      JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(encodedToken);
      if (OAuth2AuthenticationService.CallTokenService(requestContext) && requestContext.IsFeatureEnabled("AzureDevOps.Services.TokenService.ExchangeLongLastingTokens.M167"))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        DelegatedAuthorizationSettings settings = vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetSettings(vssRequestContext);
        TimeSpan timeSpan = jwtSecurityToken.ValidTo - jwtSecurityToken.ValidFrom;
        requestContext.Trace(5510503, TraceLevel.Info, "Authentication", "Service", string.Format("PAT token hours: {0}", (object) timeSpan.TotalHours));
        if (Math.Floor(timeSpan.TotalHours) > settings.PATAccessTokenMaxLifetime.TotalHours && jwtSecurityToken.Claims.Where<Claim>((Func<Claim, bool>) (c => c.Type == DelegatedAuthorizationTokenClaims.AuthorizationId && c.Value != Guid.Empty.ToString())).Any<Claim>())
        {
          string acccessHash = CryptoStringSecretGeneratorHelper.GenerateAcccessHash(vssRequestContext, encodedToken, "Framework drawer does not exist");
          requestContext.Trace(5510503, TraceLevel.Info, "Authentication", "Service", "Exchanging PAT token access key " + acccessHash + " for a new shorter token");
          AccessTokenResult accessTokenResult = vssRequestContext.GetService<IDelegatedAuthorizationService>().Exchange(vssRequestContext, acccessHash);
          if (accessTokenResult.HasError)
          {
            requestContext.TraceAlways(5510504, TraceLevel.Info, "Authentication", "Service", accessTokenResult.ErrorDescription);
            return (JwtSecurityToken) null;
          }
          jwtSecurityToken = new JwtSecurityToken(accessTokenResult.AccessToken.EncodedToken);
        }
      }
      return jwtSecurityToken;
    }

    public ClaimsPrincipal ValidateToken(
      IVssRequestContext requestContext,
      string encodedToken,
      OAuth2TokenValidators allowedValidators,
      out JwtSecurityToken jwtToken,
      out bool impersonating,
      out bool validIdentity,
      out OAuth2TokenValidators selectedValidator)
    {
      requestContext.TraceEnter(5510500, "Authentication", "Service", nameof (ValidateToken));
      impersonating = false;
      validIdentity = false;
      jwtToken = (JwtSecurityToken) null;
      selectedValidator = OAuth2TokenValidators.None;
      if (encodedToken == null)
      {
        requestContext.Trace(5510502, TraceLevel.Info, "Authentication", "Service", "EncodedToken is null.");
        requestContext.TraceLeave(5510500, "Authentication", "Service", nameof (ValidateToken));
        return (ClaimsPrincipal) null;
      }
      try
      {
        jwtToken = this.ExchangeToken(requestContext, encodedToken);
        if (jwtToken == null)
          return (ClaimsPrincipal) null;
        bool flag = this.CheckTokenCache(requestContext, jwtToken, encodedToken);
        TokenValidationParameters innerParameters;
        TokenValidationParameters tokenForTokenCaching = this.GetValidationParametersForToken_ForTokenCaching(requestContext, jwtToken, !flag, out innerParameters);
        IOAuth2TokenValidator tokenValidator = this.GetValidator(requestContext, (Microsoft.IdentityModel.Tokens.SecurityToken) jwtToken, allowedValidators);
        if (tokenValidator == null)
        {
          requestContext.Trace(5510500, TraceLevel.Error, "Authentication", "Service", "Could not find validator for token issuer: " + jwtToken.Issuer);
          requestContext.Trace(5510500, TraceLevel.Info, "Authentication", "Service", string.Format("Could not find validator for token issuer: {0}", (object) jwtToken));
          throw new SecurityTokenValidationException("Could not determine token validator for token.");
        }
        selectedValidator = tokenValidator.ValidatorType;
        IssuerValidator issuerValidator = (IssuerValidator) ((issuer, token, paramms) =>
        {
          if (string.IsNullOrEmpty(issuer) || string.IsNullOrWhiteSpace(issuer))
            throw new SecurityTokenValidationException("The token does not contain the required issuer claim. Expected an issuer claim.");
          if (!(token is JwtSecurityToken token2))
            throw new SecurityTokenValidationException("Security token is not a JwtSecurityToken.");
          if (!tokenValidator.CanValidateToken(requestContext, token2))
            throw new SecurityTokenValidationException("Invalid token issuer. The token issuer " + issuer + " is not trusted.");
          return issuer;
        });
        tokenForTokenCaching.IssuerValidator = issuerValidator;
        innerParameters.IssuerValidator = issuerValidator;
        innerParameters.AudienceValidator = (AudienceValidator) ((audiences, token, @params) => tokenValidator.ValidateAudience(requestContext, audiences, token, @params));
        innerParameters.RoleClaimType = tokenValidator.RoleClaimType;
        innerParameters.NameClaimType = tokenValidator.NameClaimType;
        CryptoProviderFactory.Default.CacheSignatureProviders = true;
        Microsoft.IdentityModel.Tokens.SecurityToken validatedToken;
        ClaimsPrincipal principal = OAuth2AuthenticationService.CreateJwtSecurityTokenHandler().ValidateToken(encodedToken, tokenForTokenCaching, out validatedToken);
        ((ClaimsIdentity) principal.Identity).AddClaim(new Claim("OAuth2Validator", tokenValidator.GetType().ToString()));
        jwtToken = (JwtSecurityToken) validatedToken;
        requestContext.Trace(5510500, TraceLevel.Info, "Authentication", "Service", "JWT token validation for token from issuer {0} and identity {1} successful.", (object) jwtToken.Issuer, (object) principal.Identity.Name);
        Stopwatch stopwatch = new Stopwatch();
        VssPerformanceEventSource.Log.ValidateJWTokenStart(requestContext.E2EId, jwtToken.Issuer, principal.Identity.Name);
        try
        {
          stopwatch.Start();
          tokenValidator.ValidateToken(requestContext, jwtToken, principal, out impersonating, out validIdentity);
        }
        finally
        {
          stopwatch.Stop();
          VssPerformanceEventSource.Log.ValidateJWTokenStop(requestContext.E2EId, jwtToken.Issuer, principal.Identity.Name, stopwatch.ElapsedMilliseconds);
        }
        if (validIdentity && !flag)
          this.AddTokenToCache(requestContext, jwtToken, encodedToken);
        return principal;
      }
      catch (SecurityTokenValidationException ex)
      {
        requestContext.TraceAlways(5510501, TraceLevel.Warning, "Authentication", "Service", "Failed to validate token, validFrom: {0:s}, validTo: {1:s}, now: {2:s}. Exception {3}", (object) jwtToken.ValidFrom, (object) jwtToken.ValidTo, (object) DateTime.UtcNow, (object) ex);
        impersonating = false;
        validIdentity = false;
        jwtToken = (JwtSecurityToken) null;
        selectedValidator = OAuth2TokenValidators.None;
        return (ClaimsPrincipal) null;
      }
      catch (ArgumentException ex)
      {
        requestContext.TraceAlways(5510501, TraceLevel.Warning, "Authentication", "Service", "Failed to validate token. ArgumentException was thrown. Exception {0}", (object) ex);
        impersonating = false;
        validIdentity = false;
        jwtToken = (JwtSecurityToken) null;
        return (ClaimsPrincipal) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5510501, "Authentication", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5510500, "Authentication", "Service", nameof (ValidateToken));
      }
    }

    private bool CheckTokenCache(
      IVssRequestContext requestContext,
      JwtSecurityToken jwtToken,
      string encodedToken)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.UseJwtTokenCache") || jwtToken.Actor != null)
        return false;
      string y;
      bool flag;
      return !string.IsNullOrEmpty(jwtToken.Payload.Jti) ? requestContext.GetService<JwtSecurityTokenCacheByJtiService>().TryGetValue(requestContext, jwtToken.Payload.Jti, out y) && StringComparer.Ordinal.Equals(encodedToken, y) : requestContext.GetService<JwtSecurityTokenCacheService>().TryGetValue(requestContext, encodedToken, out flag) && flag;
    }

    private void AddTokenToCache(
      IVssRequestContext requestContext,
      JwtSecurityToken jwtToken,
      string encodedToken,
      bool isValid = true)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.UseJwtTokenCache") || jwtToken.Actor != null)
        return;
      if (!string.IsNullOrEmpty(jwtToken.Payload.Jti))
        requestContext.GetService<JwtSecurityTokenCacheByJtiService>().TryAdd(requestContext, jwtToken.Payload.Jti, encodedToken);
      else
        requestContext.GetService<JwtSecurityTokenCacheService>().TryAdd(requestContext, encodedToken, isValid);
    }

    public IEnumerable<IdentityDescriptor> ProcessScopes(
      IVssRequestContext requestContext,
      ClaimsPrincipal claimsPrincipal)
    {
      IEnumerable<Claim> claims = claimsPrincipal.Claims;
      string typeName = claims != null ? claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "OAuth2Validator"))?.Value : (string) null;
      if (typeName != null)
      {
        Type type = Type.GetType(typeName, false);
        if (type != (Type) null)
        {
          IOAuth2TokenValidator validator = this.GetValidator(requestContext, type);
          if (validator != null)
            return validator.ProcessScopes(requestContext, claimsPrincipal);
        }
      }
      return (IEnumerable<IdentityDescriptor>) null;
    }

    private TokenValidationParameters GetValidationParametersForToken(
      IVssRequestContext requestContext,
      JwtSecurityToken jwtToken,
      out TokenValidationParameters innerParameters)
    {
      innerParameters = this.VerifyParameters(requestContext);
      TokenValidationParameters parametersForToken = innerParameters;
      if (jwtToken.Actor != null)
        parametersForToken = new TokenValidationParameters()
        {
          ClockSkew = innerParameters.ClockSkew,
          RequireSignedTokens = false,
          RequireExpirationTime = true,
          ValidateIssuerSigningKey = false,
          ValidateLifetime = true,
          ValidateAudience = true,
          ValidateIssuer = true,
          ValidateActor = true,
          ValidAudiences = innerParameters.ValidAudiences,
          ValidIssuers = innerParameters.ValidIssuers,
          ActorValidationParameters = innerParameters,
          SignatureValidator = (SignatureValidator) ((token, parameters) => (Microsoft.IdentityModel.Tokens.SecurityToken) new JwtSecurityToken(token))
        };
      return parametersForToken;
    }

    private TokenValidationParameters GetValidationParametersForToken_ForTokenCaching(
      IVssRequestContext requestContext,
      JwtSecurityToken jwtToken,
      bool validateSignature,
      out TokenValidationParameters innerParameters)
    {
      innerParameters = this.VerifyParameters(requestContext);
      if (!validateSignature)
      {
        if (jwtToken.Actor != null)
          throw new ArgumentException("Cannot skip signature validation for actor tokens");
        innerParameters.RequireSignedTokens = false;
        innerParameters.SignatureValidator = (SignatureValidator) ((token, parameters) => (Microsoft.IdentityModel.Tokens.SecurityToken) new JwtSecurityToken(token));
      }
      TokenValidationParameters tokenForTokenCaching = innerParameters;
      if (jwtToken.Actor != null)
        tokenForTokenCaching = new TokenValidationParameters()
        {
          ClockSkew = innerParameters.ClockSkew,
          RequireSignedTokens = false,
          RequireExpirationTime = true,
          ValidateIssuerSigningKey = false,
          ValidateLifetime = true,
          ValidateAudience = true,
          ValidateIssuer = true,
          ValidateActor = true,
          ValidAudiences = innerParameters.ValidAudiences,
          ValidIssuers = innerParameters.ValidIssuers,
          ActorValidationParameters = innerParameters,
          SignatureValidator = (SignatureValidator) ((token, parameters) => (Microsoft.IdentityModel.Tokens.SecurityToken) new JwtSecurityToken(token))
        };
      return tokenForTokenCaching;
    }

    internal TokenValidationParameters VerifyParameters(IVssRequestContext requestContext)
    {
      bool flag = false;
      long curVersion = this._settings.GetVersion(requestContext);
      if (curVersion > this._oauthSettingsVersion)
      {
        IDisposableReadOnlyList<IOAuth2TokenValidator> newValidators = (IDisposableReadOnlyList<IOAuth2TokenValidator>) null;
        IDictionary<string, IList<IOAuth2TokenValidator>> newValidatorMap = (IDictionary<string, IList<IOAuth2TokenValidator>>) null;
        try
        {
          OAuth2AuthenticationService.LoadValidators(requestContext, this._settings, out newValidators, out newValidatorMap);
          using (requestContext.Lock(this._initLock))
          {
            curVersion = this._settings.GetVersion(requestContext);
            if (curVersion == this._oauthSettingsVersion)
              return this._tokenValidationParameters.Clone();
            this.PopulateTokenValidationParameters(requestContext);
            requestContext.TraceConditionally(877553, TraceLevel.Info, "Authentication", "Service", (Func<string>) (() => string.Format("Updated validation keys to version {0}", (object) curVersion)));
            flag = true;
            IDisposableReadOnlyList<IOAuth2TokenValidator> validators = this.m_validators;
            this.m_validators = newValidators;
            newValidators = validators;
            this.m_validatorMap = newValidatorMap;
            this._oauthSettingsVersion = curVersion;
          }
        }
        finally
        {
          newValidators?.Dispose();
        }
      }
      if (!flag && requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.EnableExpirationLogic") && this._settings.UpdateUserAuthCertificatesIfExpired(requestContext))
      {
        requestContext.TraceConditionally(877556, TraceLevel.Info, "Authentication", "Service", (Func<string>) (() => "Updated user auth validation keys as it is expired"));
        this._tokenValidationParameters.IssuerSigningKeys = (IEnumerable<SecurityKey>) this._settings.GetValidationCertificates(requestContext).Select<KeyValuePair<string, X509Certificate2>, X509SecurityKey>((Func<KeyValuePair<string, X509Certificate2>, X509SecurityKey>) (x => new X509SecurityKey(x.Value))).ToArray<X509SecurityKey>();
      }
      return this._tokenValidationParameters.Clone();
    }

    private void PopulateTokenValidationParameters(IVssRequestContext requestContext)
    {
      IOAuth2CommonSettings commonSettings = this._settings.GetCommonSettings(requestContext);
      this._tokenValidationParameters = new TokenValidationParameters()
      {
        ClockSkew = TimeSpan.FromSeconds((double) commonSettings.ClockSkewInSeconds),
        RequireSignedTokens = true,
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateActor = true,
        ValidAudiences = commonSettings.AllowedAudiences,
        IssuerSigningKeys = (IEnumerable<SecurityKey>) this._settings.GetValidationCertificates(requestContext).Select<KeyValuePair<string, X509Certificate2>, X509SecurityKey>((Func<KeyValuePair<string, X509Certificate2>, X509SecurityKey>) (x => new X509SecurityKey(x.Value))).ToArray<X509SecurityKey>()
      };
    }

    private static JwtSecurityTokenHandler CreateJwtSecurityTokenHandler() => new JwtSecurityTokenHandler()
    {
      InboundClaimFilter = {
        "http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor"
      }
    };

    private static void LoadValidators(
      IVssRequestContext requestContext,
      IOAuth2SettingsService settingsService,
      out IDisposableReadOnlyList<IOAuth2TokenValidator> newValidators,
      out IDictionary<string, IList<IOAuth2TokenValidator>> newValidatorMap)
    {
      IDisposableReadOnlyList<IOAuth2TokenValidator> extensions1 = requestContext.GetExtensions<IOAuth2TokenValidator>();
      IDisposableReadOnlyList<IOAuth2TokenValidator> disposableReadOnlyList = (IDisposableReadOnlyList<IOAuth2TokenValidator>) null;
      using (IDisposableReadOnlyList<IOAuth2TokenValidatorFilter> extensions2 = requestContext.GetExtensions<IOAuth2TokenValidatorFilter>())
        disposableReadOnlyList = OAuth2AuthenticationService.ApplyFilters(requestContext, extensions1, (IReadOnlyList<IOAuth2TokenValidatorFilter>) extensions2);
      newValidators = disposableReadOnlyList;
      newValidatorMap = (IDictionary<string, IList<IOAuth2TokenValidator>>) new Dictionary<string, IList<IOAuth2TokenValidator>>();
      foreach (IOAuth2TokenValidator oauth2TokenValidator in (IEnumerable<IOAuth2TokenValidator>) newValidators)
      {
        oauth2TokenValidator.Initialize(requestContext, settingsService);
        foreach (string validIssuer in oauth2TokenValidator.ValidIssuers)
        {
          IList<IOAuth2TokenValidator> oauth2TokenValidatorList;
          if (!newValidatorMap.TryGetValue(validIssuer, out oauth2TokenValidatorList))
          {
            oauth2TokenValidatorList = (IList<IOAuth2TokenValidator>) new List<IOAuth2TokenValidator>();
            newValidatorMap.Add(validIssuer, oauth2TokenValidatorList);
          }
          oauth2TokenValidatorList.Add(oauth2TokenValidator);
        }
      }
    }

    private static IDisposableReadOnlyList<IOAuth2TokenValidator> ApplyFilters(
      IVssRequestContext requestContext,
      IDisposableReadOnlyList<IOAuth2TokenValidator> validators,
      IReadOnlyList<IOAuth2TokenValidatorFilter> filters)
    {
      if (filters.Count == 0)
        return validators;
      List<IOAuth2TokenValidator> elements = new List<IOAuth2TokenValidator>();
      foreach (IOAuth2TokenValidator validator in (IEnumerable<IOAuth2TokenValidator>) validators)
      {
        foreach (IOAuth2TokenValidatorFilter filter in (IEnumerable<IOAuth2TokenValidatorFilter>) filters)
        {
          if (filter.IsValidatorAllowed(requestContext, validator))
          {
            elements.Add(validator);
            break;
          }
        }
      }
      return (IDisposableReadOnlyList<IOAuth2TokenValidator>) new DisposableCollection<IOAuth2TokenValidator>((IReadOnlyList<IOAuth2TokenValidator>) elements);
    }

    private IOAuth2TokenValidator GetValidator(
      IVssRequestContext requestContext,
      Microsoft.IdentityModel.Tokens.SecurityToken token,
      OAuth2TokenValidators allowedValidators)
    {
      JwtSecurityToken jwtToken = token as JwtSecurityToken;
      if (jwtToken == null)
        return (IOAuth2TokenValidator) null;
      IList<IOAuth2TokenValidator> validatorCandidates = this.GetValidatorCandidates(jwtToken.Issuer);
      if (validatorCandidates.Count == 0 && jwtToken.Actor != null)
        validatorCandidates = (IList<IOAuth2TokenValidator>) this.m_validators.ToList<IOAuth2TokenValidator>();
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.OAuth2.DisableValidatorTypeFilter"))
        validatorCandidates = (IList<IOAuth2TokenValidator>) validatorCandidates.Where<IOAuth2TokenValidator>((Func<IOAuth2TokenValidator, bool>) (x => (allowedValidators & x.ValidatorType) != 0)).ToList<IOAuth2TokenValidator>();
      if (validatorCandidates.Count == 0)
      {
        requestContext.TraceAlways(5510500, TraceLevel.Warning, "Authentication", "Service", "Found zero validator candidates for jwtToken issuer: " + jwtToken.Issuer);
        return (IOAuth2TokenValidator) null;
      }
      if (validatorCandidates.Count == 1)
      {
        requestContext.TraceConditionally(5510500, TraceLevel.Info, "Authentication", "Service", (Func<string>) (() => "Found single validator candidate for jwtToken issuer: " + jwtToken.Issuer + ", Validator: " + validatorCandidates[0].GetType().FullName + " "));
        return validatorCandidates[0];
      }
      IOAuth2TokenValidator ret = (IOAuth2TokenValidator) null;
      bool flag = false;
      foreach (IOAuth2TokenValidator oauth2TokenValidator in (IEnumerable<IOAuth2TokenValidator>) validatorCandidates)
      {
        if (oauth2TokenValidator.CanValidateToken(requestContext, jwtToken))
        {
          if (ret != null)
            flag = true;
          else
            ret = oauth2TokenValidator;
        }
      }
      if (flag)
      {
        StringBuilder stringBuilder = new StringBuilder("There were muliple candidates that reported that they could validate this jwtToken.");
        stringBuilder.AppendLine("The candidates are these:");
        foreach (IOAuth2TokenValidator oauth2TokenValidator in (IEnumerable<IOAuth2TokenValidator>) validatorCandidates)
          stringBuilder.AppendLine("\t" + oauth2TokenValidator.GetType().FullName);
        stringBuilder.AppendLine(string.Format("The token is this: {0}", (object) jwtToken));
        string message = stringBuilder.ToString();
        requestContext.Trace(5510500, TraceLevel.Error, "Authentication", "Service", message);
      }
      requestContext.TraceConditionally(5510500, ret == null ? TraceLevel.Warning : TraceLevel.Info, "Authentication", "Service", (Func<string>) (() =>
      {
        if (ret == null)
          return "Found multiple validator candidates for jwtToken issuer: " + jwtToken.Issuer + ", but none could validate.";
        return "Found multiple validator candidates for jwtToken issuer: " + jwtToken.Issuer + ", we chose this validator: " + validatorCandidates[0].GetType().FullName + " ";
      }));
      return ret;
    }

    private IOAuth2TokenValidator GetValidator(
      IVssRequestContext requestContext,
      Type validatorType)
    {
      return this.m_validators.FirstOrDefault<IOAuth2TokenValidator>((Func<IOAuth2TokenValidator, bool>) (x => x.GetType() == validatorType));
    }

    private IList<IOAuth2TokenValidator> GetValidatorCandidates(string issuer)
    {
      List<IOAuth2TokenValidator> validatorCandidates = new List<IOAuth2TokenValidator>();
      IList<IOAuth2TokenValidator> collection1;
      if (this.m_validatorMap.TryGetValue(issuer, out collection1))
        validatorCandidates.AddRange((IEnumerable<IOAuth2TokenValidator>) collection1);
      Uri result;
      IList<IOAuth2TokenValidator> collection2;
      if (Uri.TryCreate(issuer, UriKind.Absolute, out result) && this.m_validatorMap.TryGetValue(result.Host, out collection2))
        validatorCandidates.AddRange((IEnumerable<IOAuth2TokenValidator>) collection2);
      return (IList<IOAuth2TokenValidator>) validatorCandidates;
    }

    private string NormalizeIssuer(string issuerClaimValue)
    {
      Uri result;
      return !Uri.TryCreate(issuerClaimValue, UriKind.Absolute, out result) ? issuerClaimValue : result.Host;
    }
  }
}
