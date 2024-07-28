// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2AuthenticationModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class OAuth2AuthenticationModule : VssfAuthenticationHttpModuleBase
  {
    internal const string BearerScheme = "bearer ";
    internal const string FeatureFlagRegistryPath = "/FeatureAvailability/Entries/";
    internal const string SignedInUrlEndpoint = "_signedin";
    private static readonly string s_Area = nameof (OAuth2AuthenticationModule);
    private static readonly string s_Layer = "IHttpModule";
    private readonly IAadAuthenticationSessionTokenProvider aadAuthenticationSessionTokenProvider;
    private const string AcceptIdTokenPostSignoutPath = "Identity.OAuth2AuthenticationModule.AcceptIdTokenPostSignout";
    private static readonly IConfigPrototype<bool> EnableAcceptIdTokenPostSignoutPrototype = ConfigPrototype.Create<bool>("Identity.OAuth2AuthenticationModule.AcceptIdTokenPostSignout", false);
    private readonly IConfigQueryable<bool> enableAcceptIdTokenPostSignoutConfig;
    private static readonly IdentityDescriptor s_scopePrimaryDescriptor = new IdentityDescriptor("System:Scope", "Scope");

    public OAuth2AuthenticationModule()
      : this((IAadAuthenticationSessionTokenProvider) new AadAuthenticationSessionTokenProvider(), ConfigProxy.Create<bool>(OAuth2AuthenticationModule.EnableAcceptIdTokenPostSignoutPrototype))
    {
    }

    internal OAuth2AuthenticationModule(
      IAadAuthenticationSessionTokenProvider aadAuthenticationSessionTokenProvider,
      IConfigQueryable<bool> enableAcceptIdTokenPostSignoutConfig)
    {
      this.aadAuthenticationSessionTokenProvider = aadAuthenticationSessionTokenProvider ?? (IAadAuthenticationSessionTokenProvider) new AadAuthenticationSessionTokenProvider();
      this.enableAcceptIdTokenPostSignoutConfig = enableAcceptIdTokenPostSignoutConfig;
    }

    protected override bool SkipIfAlreadyAuthenticated => false;

    public override void OnAuthenticateRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
      requestContext.TraceEnter(908756, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, "OAuth2AuthenticationModule.OnAuthenticateRequest");
      try
      {
        if (this.HasAadAuthenticationError(requestContext))
        {
          AuthenticationHelpers.EnterMethodIfNull(requestContext, "Authentication", "OAuth2AuthenticationModule.OnAuthenticateRequest");
          TeamFoundationApplicationCore.CompleteRequest(requestContext, httpContext.GetApplicationInstance(), HttpStatusCode.Forbidden, (Exception) new AadAuthorizationException());
        }
        else
        {
          bool validIdentity = false;
          bool impersonating = false;
          JwtSecurityToken jwtToken = (JwtSecurityToken) null;
          OAuth2TokenValidators allowedValidators = OAuth2TokenValidators.AAD | OAuth2TokenValidators.S2S | OAuth2TokenValidators.DelegatedAuth | OAuth2TokenValidators.AADServicePrincipal;
          ClaimsPrincipal validatedPrincipal;
          try
          {
            validatedPrincipal = this.GetValidatedPrincipal(requestContext, httpContext, allowedValidators, out jwtToken, out impersonating, out validIdentity);
          }
          catch (CircuitBreakerException ex)
          {
            requestContext.TraceException(280106, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, (Exception) ex);
            TeamFoundationApplicationCore.CompleteRequest(requestContext, httpContext.GetApplicationInstance(), HttpStatusCode.ServiceUnavailable, (Exception) ex);
            return;
          }
          catch (Exception ex)
          {
            requestContext.TraceDataConditionally(866306, TraceLevel.Info, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, "Failed to validate token.", (Func<object>) (() => (object) new
            {
              exception = ex
            }), nameof (OnAuthenticateRequest));
            return;
          }
          if ((!AuthenticationHelpers.IsSignedInRequest(httpContext.Request) ? 0 : (requestContext.IsTracing(5510711, TraceLevel.Info, "Server", nameof (OAuth2AuthenticationModule)) ? 1 : 0)) != 0)
            IdentityTracing.TraceToken(nameof (OAuth2AuthenticationModule), jwtToken, AadAuthUrlUtility.ParseState()["nonce"]);
          if (validatedPrincipal != null && !validIdentity)
          {
            bool flag;
            if (requestContext.RootContext.Items.TryGetValue<bool>("$UserAuthenticationSkipped", out flag) & flag)
            {
              requestContext.Trace(575335, TraceLevel.Info, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, "UserAuthentication was present but ignored.");
            }
            else
            {
              AuthenticationHelpers.EnterMethodIfNull(requestContext, "Authentication", "OAuth2AuthenticationModule.OnAuthenticateRequest");
              requestContext.WebRequestContextInternal().SetAuthenticationMechanismsToAdvertise(AuthenticationMechanisms.OAuth);
              string message;
              requestContext.Items.TryGetValue<string>(RequestContextItemsKeys.ValidatorIdentityError, out message);
              requestContext.Items.Remove(RequestContextItemsKeys.ValidatorIdentityError);
              TeamFoundationApplicationCore.CompleteRequest(requestContext, HttpApplicationFactory.Current, HttpStatusCode.Unauthorized, (Exception) new InvalidIdentityException(validatedPrincipal.Identity?.Name, message));
            }
          }
          else if (validatedPrincipal == null)
          {
            requestContext.Trace(575334, TraceLevel.Info, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, "Principal is null.");
          }
          else
          {
            ClaimsPrincipal claimsPrincipal = validatedPrincipal;
            if (impersonating)
            {
              ClaimsIdentity identity = (ClaimsIdentity) validatedPrincipal.Identity;
              claimsPrincipal = new ClaimsPrincipal((IIdentity) identity.Actor);
              requestContext.Items.Add(RequestContextItemsKeys.TfsImpersonate, (object) identity);
            }
            httpContext.User = (IPrincipal) claimsPrincipal;
            bool flag = requestContext.To(TeamFoundationHostType.Deployment).Items.ContainsKey(RequestContextItemsKeys.AlternateAuthCredentialsContextKey);
            OAuth2AuthCredential credential = new OAuth2AuthCredential(claimsPrincipal, jwtToken, !flag);
            ITeamFoundationAuthenticationServiceInternal authenticationServiceInternal = requestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal();
            authenticationServiceInternal.SetAuthenticationCredential((IAuthCredential) credential);
            if (this.IsSignedInRequest(requestContext) && authenticationServiceInternal.IssueSessionSecurityToken(requestContext, validatedPrincipal))
            {
              requestContext.RootContext.Items["AuthenticationByIdentityProvider"] = (object) true;
              if (jwtToken != null)
                requestContext.RootContext.Items["CredentialValidFrom"] = (object) jwtToken.ValidFrom;
            }
            this.aadAuthenticationSessionTokenProvider.UpdateSessionToken(requestContext, httpContext, jwtToken);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(280105, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, ex);
        TeamFoundationApplicationCore.CompleteRequest(requestContext, httpContext.GetApplicationInstance(), HttpStatusCode.InternalServerError, ex);
      }
      finally
      {
        requestContext.TraceLeave(213215, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, "OAuth2AuthenticationModule.OnAuthenticateRequest");
      }
    }

    public override void OnPostAuthenticateRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
      requestContext.TraceEnter(213216, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, "OAuth2AuthenticationModule.OnPostAuthenticateRequest");
      try
      {
        if (!(httpContext.User is ClaimsPrincipal user))
          return;
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IEnumerable<IdentityDescriptor> identityDescriptors = vssRequestContext.GetService<IOAuth2AuthenticationService>().ProcessScopes(vssRequestContext, user);
        if (identityDescriptors == null)
          return;
        if (identityDescriptors.Count<IdentityDescriptor>() == 0)
        {
          requestContext.Trace(213220, TraceLevel.Error, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, "Invalid modern scope for scale unit access.");
          TeamFoundationApplicationCore.CompleteRequest(requestContext, httpContext.GetApplicationInstance(), HttpStatusCode.Unauthorized, (Exception) new UnauthorizedAccessException(FrameworkResources.InvalidScopes()));
        }
        IRequestActor userActor = requestContext.GetUserActor();
        if (userActor != null && userActor.TryAppendPrincipal(SubjectType.Scope, new EvaluationPrincipal(OAuth2AuthenticationModule.s_scopePrimaryDescriptor, (IdentityDescriptor) null, identityDescriptors)))
          return;
        requestContext.Trace(213217, TraceLevel.Error, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, "Failed to add the scopes on the user actor!");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(213218, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, ex);
        TeamFoundationApplicationCore.CompleteRequest(requestContext, httpContext.GetApplicationInstance(), HttpStatusCode.InternalServerError, ex);
      }
      finally
      {
        requestContext.TraceLeave(213219, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, "OAuth2AuthenticationModule.OnPostAuthenticateRequest");
      }
    }

    private bool IsSignedInRequest(IVssRequestContext requestContext) => requestContext.RequestRestrictions().HasAnyLabel("SignedInPage");

    private string GetAuthorizationToken(IVssRequestContext requestContext, HttpContextBase context)
    {
      string authorizationToken = context.Request.Headers.Get("Authorization");
      if (!string.IsNullOrEmpty(authorizationToken) && !authorizationToken.StartsWith("bearer ", StringComparison.OrdinalIgnoreCase))
        return (string) null;
      if (string.IsNullOrEmpty(authorizationToken))
        authorizationToken = context.Request.QueryString["Authorization"];
      if (string.IsNullOrEmpty(authorizationToken))
        authorizationToken = context.Request.QueryString.Get("id_token");
      if (string.IsNullOrEmpty(authorizationToken) && AuthenticationHelpers.IsSignedInRequest(context.Request))
      {
        authorizationToken = context.Request.Form["Authorization"];
        if (string.IsNullOrEmpty(authorizationToken))
          authorizationToken = context.Request.Form["id_token"];
      }
      if (string.IsNullOrEmpty(authorizationToken) && AuthenticationHelpers.IsPostSignOutRequest(context.Request) && this.enableAcceptIdTokenPostSignoutConfig.QueryByCtx<bool>(requestContext))
        authorizationToken = context.Request.Form["id_token"];
      if (!string.IsNullOrEmpty(authorizationToken) && authorizationToken.StartsWith("bearer ", StringComparison.OrdinalIgnoreCase))
        authorizationToken = authorizationToken.Substring("bearer ".Length).TrimStart();
      return authorizationToken;
    }

    private ClaimsPrincipal GetValidatedPrincipal(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      OAuth2TokenValidators allowedValidators,
      out JwtSecurityToken jwtToken,
      out bool impersonating,
      out bool validIdentity)
    {
      jwtToken = (JwtSecurityToken) null;
      impersonating = false;
      validIdentity = false;
      bool flag = this.IsSignedInRequest(requestContext);
      string authorizationToken = this.GetAuthorizationToken(requestContext, httpContext);
      IOAuth2AuthenticationService service = requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetService<IOAuth2AuthenticationService>();
      if (flag)
      {
        try
        {
          string token = httpContext.Request.Form["aad_token"];
          if (!string.IsNullOrEmpty(token))
          {
            ClaimsPrincipal validatedPrincipal = service.ValidateToken(requestContext, token, OAuth2TokenValidators.AADCookie, out jwtToken, out impersonating, out validIdentity);
            if (validatedPrincipal != null)
              return validatedPrincipal;
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(866307, TraceLevel.Info, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, ex, "Caught an exception processing aad_token.");
        }
      }
      if (flag)
        allowedValidators |= OAuth2TokenValidators.UserAuthentication;
      return service.ValidateToken(requestContext, authorizationToken, allowedValidators, out jwtToken, out impersonating, out validIdentity);
    }

    private bool HasAadAuthenticationError(IVssRequestContext requestContext)
    {
      NameValueCollection queryString = HttpUtility.ParseQueryString(requestContext.RequestUri().Query);
      string str1 = queryString["error"];
      string str2 = queryString["error_description"];
      bool flag = false;
      if (!string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2))
      {
        if (requestContext.RequestRestrictions().HasAnyLabel("SignedInPage"))
        {
          flag = true;
          requestContext.Trace(5510710, TraceLevel.Info, "Server", nameof (OAuth2AuthenticationModule), "AAD Error: {0} | ErrorDescription: {1}", (object) str1, (object) str2);
        }
      }
      return flag;
    }

    public override void OnEndRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
      requestContext.TraceEnter(311835, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, "OAuth2AuthenticationModule.OnEndRequest");
      try
      {
        if (httpContext.Response.StatusCode != 401 || (requestContext.RequestRestrictions().MechanismsToAdvertise & AuthenticationMechanisms.OAuth) == AuthenticationMechanisms.None)
          return;
        IS2SAuthSettings s2SauthSettings = requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetService<IOAuth2SettingsService>().GetS2SAuthSettings(requestContext);
        if (s2SauthSettings.Enabled)
        {
          string str = string.Format("{0}/visualstudio.com", (object) s2SauthSettings.PrimaryServicePrincipal.ToString("D"));
          httpContext.Response.AddHeader("X-VSS-S2STargetService", str);
        }
        AuthenticationHelpers.SetWWWAuthenticateHeaderIfNotPresent(httpContext, "Bearer");
        if (requestContext.RequestRestrictions().MechanismsToAdvertise != AuthenticationMechanisms.OAuth)
          return;
        AuthenticationHelpers.EnterMethodIfNull(requestContext, "Authentication", "OAuth2AuthenticationModule.OnEndRequest");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(249319, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, ex);
        TeamFoundationApplicationCore.CompleteRequest(requestContext, httpContext.GetApplicationInstance(), HttpStatusCode.InternalServerError, ex);
      }
      finally
      {
        requestContext.TraceLeave(236250, OAuth2AuthenticationModule.s_Area, OAuth2AuthenticationModule.s_Layer, "OAuth2AuthenticationModule.OnEndRequest");
      }
    }
  }
}
