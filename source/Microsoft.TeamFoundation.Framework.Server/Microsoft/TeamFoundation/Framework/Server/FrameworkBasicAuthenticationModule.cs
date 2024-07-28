// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FrameworkBasicAuthenticationModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.IdentityModel.Tokens;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FrameworkBasicAuthenticationModule : VssfAuthenticationHttpModuleBase
  {
    protected static readonly string Area = "BasicAuthentication";
    protected static readonly string Layer = "Module";
    private static readonly Regex Base32NoPaddingFormatRegex = new Regex("^[A-Z2-7]{52}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly string s_DelegatedAuthArea = "DelegatedAuthorizationService";
    private static readonly string s_DelegatedAuthLayer = "BasicAuthenticationModule";
    private const string AuthStartString = "{\"__authToken\"";
    private const string BasicScheme = "Basic";
    private const string PatExpiredErrorMessaging = "VisualStudio.Services.Authentication.PatExpiredErrorMessaging";
    private const string ThrowIfAccessTokenFails = "Microsoft.AzureDevOps.Authentication.ThrowIfAccessTokenValidationFails.M190";
    private static SparseTree<bool> s_iisBasicAuthTree = (SparseTree<bool>) null;

    public override void OnAuthenticateRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
      requestContext.TraceEnter(10070629, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, nameof (OnAuthenticateRequest));
      if (requestContext.IsFeatureEnabled("VisualStudio.BasicAuth.Disable"))
      {
        requestContext.Trace(10070630, TraceLevel.Info, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Basic Auth is turned off");
      }
      else
      {
        FrameworkBasicAuthenticationModule.AuthenticationStatus authenticationStatus = FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
        try
        {
          authenticationStatus = this.Authenticate(requestContext, httpContext);
        }
        catch (NotSupportedException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(1007060, TraceLevel.Error, FrameworkBasicAuthenticationModule.Area, FrameworkBasicAuthenticationModule.Layer, (Exception) ex);
          authenticationStatus = !requestContext.IsFeatureEnabled("Microsoft.AzureDevOps.Authentication.ThrowIfAccessTokenValidationFails.M190") ? FrameworkBasicAuthenticationModule.AuthenticationStatus.Rejected : FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
          throw;
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(1007060, TraceLevel.Error, FrameworkBasicAuthenticationModule.Area, FrameworkBasicAuthenticationModule.Layer, ex);
          if (requestContext.IsFeatureEnabled("Microsoft.AzureDevOps.Authentication.ThrowIfAccessTokenValidationFails.M190"))
          {
            authenticationStatus = FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
            throw;
          }
          else
            authenticationStatus = FrameworkBasicAuthenticationModule.AuthenticationStatus.Rejected;
        }
        finally
        {
          requestContext.Trace(10070638, TraceLevel.Info, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, string.Format("auth status : {0}", (object) authenticationStatus));
          switch (authenticationStatus)
          {
            case FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled:
            case FrameworkBasicAuthenticationModule.AuthenticationStatus.Authenticated:
              requestContext.TraceLeave(10070637, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, nameof (OnAuthenticateRequest));
            case FrameworkBasicAuthenticationModule.AuthenticationStatus.Rejected_ExpiredPat:
              this.CompleteInvalidRequestWithErrorMessage(requestContext, httpContext, FrameworkResources.PatExpired());
              goto case FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
            case FrameworkBasicAuthenticationModule.AuthenticationStatus.Rejected_AltCredsDeprecated:
              this.CompleteInvalidRequestWithErrorMessage(requestContext, httpContext, FrameworkResources.PolicyDisallowBasicAuthentication());
              goto case FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
            default:
              this.CompleteInvalidRequest(requestContext, httpContext);
              goto case FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
          }
        }
      }
    }

    public override void OnEndRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.BasicAuth.Disable") || httpContext.Response.StatusCode != 401 && httpContext.Response.StatusCode != 302 || httpContext.User?.Identity != null && httpContext.User.Identity.IsAuthenticated || (requestContext.RequestRestrictions().MechanismsToAdvertise & AuthenticationMechanisms.Basic) == AuthenticationMechanisms.None)
        return;
      ITeamFoundationAuthenticationService service = requestContext.GetService<ITeamFoundationAuthenticationService>();
      try
      {
        TeamFoundationExecutionEnvironment executionEnvironment = requestContext.ExecutionEnvironment;
        string str = !executionEnvironment.IsOnPremisesDeployment ? service.DetermineRealm(requestContext) : requestContext.GetService<ILocationService>().DetermineAccessMapping(requestContext).AccessPoint;
        bool flag = false;
        executionEnvironment = requestContext.ExecutionEnvironment;
        if (executionEnvironment.IsOnPremisesDeployment)
          flag = FrameworkBasicAuthenticationModule.IsBasicAuthEnabled(requestContext.RequestUri());
        if (!flag)
        {
          executionEnvironment = requestContext.ExecutionEnvironment;
          if (executionEnvironment.IsOnPremisesDeployment)
          {
            if (httpContext?.Request != null)
            {
              if (!httpContext.Request.Browser.IsBrowser("FireFox"))
              {
                if (httpContext.Request.Browser.IsBrowser("Safari"))
                  goto label_12;
              }
              else
                goto label_12;
            }
            else
              goto label_12;
          }
          httpContext.Response.AddHeader("WWW-Authenticate", "Basic realm=\"" + str + "\"");
        }
      }
      catch
      {
      }
label_12:
      if (requestContext.RequestRestrictions().MechanismsToAdvertise != AuthenticationMechanisms.Basic)
        return;
      AuthenticationHelpers.EnterMethodIfNull(requestContext, "Authentication", "FrameworkBasicAuthenticationModule.OnEndRequest");
    }

    protected void CompleteInvalidRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext)
    {
      AuthenticationHelpers.EnterMethodIfNull(requestContext, "Authentication", "FrameworkBasicAuthenticationModule.CompleteInvalidRequest");
      requestContext.WebRequestContextInternal().SetAuthenticationMechanismsToAdvertise(AuthenticationMechanisms.Basic);
      httpContext.Response.Clear();
      httpContext.Response.StatusCode = 401;
      httpContext.Response.TrySkipIisCustomErrors = true;
      HttpApplicationFactory.Current.CompleteRequest();
    }

    protected void CompleteInvalidRequestWithErrorMessage(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      string errorMessage)
    {
      AuthenticationHelpers.EnterMethodIfNull(requestContext, "Authentication", "FrameworkBasicAuthenticationModule.CompleteInvalidRequestWithErrorMessage");
      requestContext.WebRequestContextInternal().SetAuthenticationMechanismsToAdvertise(AuthenticationMechanisms.Basic);
      TeamFoundationApplicationCore.CompleteRequest(httpContext.GetApplicationInstance(), HttpStatusCode.Unauthorized, (string) null, (IEnumerable<KeyValuePair<string, string>>) null, (Exception) new AccessCheckException(errorMessage), (string) null, (string) null);
    }

    private FrameworkBasicAuthenticationModule.AuthenticationStatus Authenticate(
      IVssRequestContext requestContext,
      HttpContextBase context)
    {
      if (requestContext.RequestRestrictions().RequiredAuthentication == RequiredAuthentication.Anonymous)
      {
        requestContext.Trace(10070631, TraceLevel.Info, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Request doesn't need identity validation");
        return FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
      }
      string str = context.Request.Headers.Get("Authorization");
      if (!string.IsNullOrEmpty(str) && !str.StartsWith("Basic", StringComparison.OrdinalIgnoreCase))
      {
        requestContext.Trace(10070632, TraceLevel.Info, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Authorization header doesn't start with 'Basic'");
        return FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
      }
      string token = str?.Substring("Basic".Length).TrimStart();
      if (!string.IsNullOrEmpty(token))
        return this.ValidateCredentials(requestContext, context, token);
      requestContext.TraceAlways(10070635, TraceLevel.Info, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Basic auth token is empty");
      return FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
    }

    private bool IsBasicAuthDisabledForHost(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return false;
      try
      {
        IOrganizationPolicyService service = requestContext.GetService<IOrganizationPolicyService>();
        bool flag = requestContext.IsFeatureEnabled("VisualStudio.Services.AdminEngagement.OrganizationPolicies.AltCredDefaultToTrue");
        IVssRequestContext context = requestContext.Elevate();
        int num = flag ? 1 : 0;
        Policy<bool> policy = service.GetPolicy<bool>(context, "Policy.DisallowBasicAuthentication", num != 0);
        if (policy.EffectiveValue)
          requestContext.Trace(1007062, TraceLevel.Info, FrameworkBasicAuthenticationModule.Area, FrameworkBasicAuthenticationModule.Layer, "Basic authentication is disabled for host {0}. Completing request as 401", (object) requestContext.ServiceHost.InstanceId);
        return policy.EffectiveValue;
      }
      catch (NotSupportedException ex)
      {
        requestContext.TraceException(1007063, FrameworkBasicAuthenticationModule.Area, FrameworkBasicAuthenticationModule.Layer, (Exception) ex);
        throw;
      }
    }

    private FrameworkBasicAuthenticationModule.AuthenticationStatus ValidateCredentials(
      IVssRequestContext requestContext,
      HttpContextBase context,
      string token)
    {
      string str1 = Encoding.UTF8.GetString(Convert.FromBase64String(token));
      int length = str1.IndexOf(":");
      if (length < 0)
      {
        requestContext.TraceAlways(10070636, TraceLevel.Info, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Basic auth token doesn't contain ':'");
        return FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
      }
      string str2 = str1.Substring(0, length);
      string password = str1.Substring(length + 1);
      FrameworkBasicAuthenticationModule.AuthenticationStatus authenticationStatus = this.AuthenticateDelegatedAuthAccessToken(requestContext, context, str2, password);
      if (authenticationStatus != FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled)
        return authenticationStatus;
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || length <= 0 || string.IsNullOrWhiteSpace(str2))
        return FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
      return this.IsBasicAuthDisabledForHost(requestContext) ? FrameworkBasicAuthenticationModule.AuthenticationStatus.Rejected : this.AuthenticateAsAlternateCredential(requestContext, context, str2, password);
    }

    private FrameworkBasicAuthenticationModule.AuthenticationStatus AuthenticateDelegatedAuthAccessToken(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      string userName,
      string password)
    {
      requestContext.TraceEnter(1048200, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, nameof (AuthenticateDelegatedAuthAccessToken));
      try
      {
        string str = (string) null;
        AuthenticationMechanism authenticationMechanism = AuthenticationMechanism.None;
        bool completeInvalidRequest = false;
        if (FrameworkBasicAuthenticationModule.Base32NoPaddingFormatRegex.IsMatch(userName))
        {
          requestContext.Trace(1048202, TraceLevel.Info, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Username matched compact session token format.");
          str = this.GetDelegatedAuthAccessTokenFromCompactSessionToken(requestContext, userName, out completeInvalidRequest);
          authenticationMechanism = AuthenticationMechanism.PAT;
        }
        else if (FrameworkBasicAuthenticationModule.Base32NoPaddingFormatRegex.IsMatch(password))
        {
          requestContext.Trace(1048202, TraceLevel.Info, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Password matched compact session token format.");
          str = this.GetDelegatedAuthAccessTokenFromCompactSessionToken(requestContext, password, out completeInvalidRequest);
          authenticationMechanism = AuthenticationMechanism.PAT;
        }
        else if (!string.IsNullOrWhiteSpace(userName) && this.IsValidSelfDescribingSessionToken(userName))
        {
          requestContext.Trace(1048202, TraceLevel.Info, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Username matched self describing session token format.");
          str = userName;
          authenticationMechanism = AuthenticationMechanism.Basic_SessionToken;
        }
        else if (!string.IsNullOrWhiteSpace(password) && this.IsValidSelfDescribingSessionToken(password))
        {
          requestContext.Trace(1048202, TraceLevel.Info, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Password matched self describing session token format.");
          str = password;
          authenticationMechanism = AuthenticationMechanism.Basic_SessionToken;
        }
        if (completeInvalidRequest)
          return FrameworkBasicAuthenticationModule.AuthenticationStatus.Rejected;
        if (string.IsNullOrWhiteSpace(str))
        {
          requestContext.TraceAlways(1048202, TraceLevel.Info, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Username or password does not match any of the compact or self describing session token formats or the resulting encoded token is not valid.");
          return FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
        }
        requestContext.Trace(1048203, TraceLevel.Info, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Authenticating credentials as OAuth credential");
        requestContext.Items.Add(RequestContextItemsKeys.IgnoreOAuthEnabledChecks, (object) true);
        IOAuth2AuthenticationService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<IOAuth2AuthenticationService>();
        bool validIdentity = false;
        JwtSecurityToken jwtToken = (JwtSecurityToken) null;
        OAuth2TokenValidators selectedValidator = OAuth2TokenValidators.None;
        OAuth2TokenValidators allowedValidators = OAuth2TokenValidators.AAD | OAuth2TokenValidators.DelegatedAuth | OAuth2TokenValidators.AADServicePrincipal;
        ClaimsPrincipal claimsPrincipal;
        try
        {
          claimsPrincipal = service.ValidateToken(requestContext, str, allowedValidators, out jwtToken, out bool _, out validIdentity, out selectedValidator);
        }
        catch
        {
          requestContext.Trace(1048204, TraceLevel.Error, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Exception validating session token.");
          return FrameworkBasicAuthenticationModule.AuthenticationStatus.Rejected;
        }
        if (claimsPrincipal == null)
        {
          requestContext.TraceAlways(1048205, TraceLevel.Warning, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Failed to validate session token.");
          if (authenticationMechanism == AuthenticationMechanism.PAT && requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.PatExpiredErrorMessaging"))
          {
            JwtSecurityToken jwtSecurityToken = (JwtSecurityToken) null;
            try
            {
              jwtSecurityToken = new JwtSecurityToken(str);
              Validators.ValidateLifetime(new DateTime?(jwtSecurityToken.ValidFrom), new DateTime?(jwtSecurityToken.ValidTo), (SecurityToken) jwtSecurityToken, new TokenValidationParameters());
            }
            catch (SecurityTokenValidationException ex) when (ex.Message.StartsWith("IDX10223:"))
            {
              requestContext.TraceAlways(1048205, TraceLevel.Warning, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "The PAT with AuthorizationId '{0}' has expired. {1}", jwtSecurityToken != null ? (object) jwtSecurityToken.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (t => t.Type == DelegatedAuthorizationTokenClaims.AuthorizationId))?.Value : (object) (string) null, (object) ex.Message);
              return FrameworkBasicAuthenticationModule.AuthenticationStatus.Rejected_ExpiredPat;
            }
            catch (SecurityTokenInvalidLifetimeException ex) when (ex.Message.StartsWith("IDX10224:"))
            {
              requestContext.TraceAlways(1048205, TraceLevel.Warning, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "The PAT with AuthorizationId '{0}' is invalid. {1}", jwtSecurityToken != null ? (object) jwtSecurityToken.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (t => t.Type == DelegatedAuthorizationTokenClaims.AuthorizationId))?.Value : (object) (string) null, (object) ex.Message);
              return FrameworkBasicAuthenticationModule.AuthenticationStatus.Rejected;
            }
            catch (ArgumentException ex) when (ex.Message.StartsWith("IDX12709:"))
            {
              requestContext.TraceAlways(1048205, TraceLevel.Warning, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "The PAT with AuthorizationId '{0}' is invalid. {1}", jwtSecurityToken != null ? (object) jwtSecurityToken.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (t => t.Type == DelegatedAuthorizationTokenClaims.AuthorizationId))?.Value : (object) (string) null, (object) ex.Message);
              return FrameworkBasicAuthenticationModule.AuthenticationStatus.Rejected;
            }
          }
          return FrameworkBasicAuthenticationModule.AuthenticationStatus.Rejected;
        }
        if (!validIdentity)
        {
          requestContext.TraceAlways(1048205, TraceLevel.Warning, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Token was validated and we have a ClaimsPrincipal but the identity that was resolved is not valid.");
          return FrameworkBasicAuthenticationModule.AuthenticationStatus.Rejected;
        }
        if (authenticationMechanism == AuthenticationMechanism.PAT)
        {
          bool flag1 = AuthenticationHelpers.IsGlobalToken(jwtToken);
          bool flag2 = AuthenticationHelpers.IsUnscopedToken(jwtToken);
          if (flag1 & flag2)
            authenticationMechanism = AuthenticationMechanism.PAT_UnscopedGlobal;
          else if (flag1)
            authenticationMechanism = AuthenticationMechanism.PAT_Global;
          else if (flag2)
            authenticationMechanism = AuthenticationMechanism.PAT_Unscoped;
        }
        if (authenticationMechanism == AuthenticationMechanism.Basic_SessionToken)
        {
          switch (selectedValidator)
          {
            case OAuth2TokenValidators.AAD:
              authenticationMechanism = AuthenticationMechanism.Basic_AAD;
              break;
            case OAuth2TokenValidators.AADServicePrincipal:
              authenticationMechanism = AuthenticationMechanism.Basic_AADServicePrincipal;
              break;
          }
        }
        AuthenticationHelpers.SetAuthenticationMechanism(requestContext, authenticationMechanism);
        httpContext.User = (IPrincipal) claimsPrincipal;
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.DisableSetCredentialInBasicAuth") && (authenticationMechanism == AuthenticationMechanism.Basic_SessionToken || authenticationMechanism == AuthenticationMechanism.Basic_AAD))
        {
          bool flag = requestContext.To(TeamFoundationHostType.Deployment).Items.ContainsKey(RequestContextItemsKeys.AlternateAuthCredentialsContextKey);
          OAuth2AuthCredential credential = new OAuth2AuthCredential(claimsPrincipal, jwtToken, !flag);
          requestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal().SetAuthenticationCredential((IAuthCredential) credential);
        }
        return FrameworkBasicAuthenticationModule.AuthenticationStatus.Authenticated;
      }
      finally
      {
        requestContext.TraceLeave(1048206, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, nameof (AuthenticateDelegatedAuthAccessToken));
      }
    }

    private static bool IsBasicAuthEnabled(Uri requestUri)
    {
      try
      {
        if (FrameworkBasicAuthenticationModule.s_iisBasicAuthTree == null)
          FrameworkBasicAuthenticationModule.s_iisBasicAuthTree = FrameworkBasicAuthenticationModule.LoadIisBasicAuthSettingsTree();
        return FrameworkBasicAuthenticationModule.s_iisBasicAuthTree.EnumParents(requestUri.LocalPath, EnumParentsOptions.None).FirstOrDefault<EnumeratedSparseTreeNode<bool>>().ReferencedObject;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceCatchRaw(570004, TraceLevel.Error, "Authentication", nameof (FrameworkBasicAuthenticationModule), ex);
        return false;
      }
    }

    private static SparseTree<bool> LoadIisBasicAuthSettingsTree()
    {
      SparseTree<bool> sparseTree = new SparseTree<bool>('/', StringComparison.OrdinalIgnoreCase);
      string[] commandLineArgs = Environment.GetCommandLineArgs();
      string path = (string) null;
      for (int index = 0; index < commandLineArgs.Length - 1; ++index)
      {
        if (commandLineArgs[index].Equals("-h", StringComparison.OrdinalIgnoreCase))
        {
          path = commandLineArgs[index + 1];
          break;
        }
      }
      if (path != null)
      {
        XDocument node = (XDocument) null;
        using (FileStream input = System.IO.File.OpenRead(path))
        {
          XmlReaderSettings settings = new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null
          };
          node = XDocument.Load(XmlReader.Create((Stream) input, settings));
        }
        string siteName = HostingEnvironment.SiteName;
        foreach (XElement xpathSelectElement in node.XPathSelectElements("/configuration/location"))
        {
          string str = xpathSelectElement.Attribute((XName) "path")?.Value;
          if (str != null && str.StartsWith(siteName, StringComparison.OrdinalIgnoreCase))
          {
            string token = str.Substring(siteName.Length);
            bool referencedObject = string.Equals(xpathSelectElement.XPathSelectElement("system.webServer/security/authentication/basicAuthentication")?.Attribute((XName) "enabled")?.Value, bool.TrueString, StringComparison.OrdinalIgnoreCase);
            sparseTree.Add(token, referencedObject);
          }
        }
      }
      return sparseTree;
    }

    private string GetDelegatedAuthAccessTokenFromCompactSessionToken(
      IVssRequestContext requestContext,
      string compactSessionToken,
      out bool completeInvalidRequest)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      string acccessHash;
      try
      {
        acccessHash = CryptoStringSecretGeneratorHelper.GenerateAcccessHash(requestContext1, compactSessionToken, "Strongbox drawer not found.");
      }
      catch (StrongBoxDrawerNotFoundException ex)
      {
        completeInvalidRequest = true;
        return (string) null;
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        completeInvalidRequest = true;
        return (string) null;
      }
      AccessTokenResult accessTokenResult;
      try
      {
        accessTokenResult = requestContext.GetService<IDelegatedAuthorizationService>().Exchange(requestContext.Elevate(), acccessHash);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1048208, TraceLevel.Error, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Error in exchanging access token key for valid access token, " + ex.Message);
        if (requestContext.IsFeatureEnabled("Microsoft.AzureDevOps.Authentication.ThrowIfAccessTokenValidationFails.M190"))
        {
          throw;
        }
        else
        {
          completeInvalidRequest = true;
          return (string) null;
        }
      }
      requestContext.Trace(1048208, TraceLevel.Info, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "Exchanged access token key for access token.");
      if (accessTokenResult?.AccessToken != null)
      {
        completeInvalidRequest = false;
        return accessTokenResult.AccessToken.EncodedToken;
      }
      completeInvalidRequest = true;
      requestContext.TraceAlways(1048209, TraceLevel.Warning, FrameworkBasicAuthenticationModule.s_DelegatedAuthArea, FrameworkBasicAuthenticationModule.s_DelegatedAuthLayer, "AccessToken result is null.");
      return (string) null;
    }

    private bool IsValidSelfDescribingSessionToken(string selfDescribingSessionToken)
    {
      if (selfDescribingSessionToken.Length >= 20)
      {
        if (selfDescribingSessionToken.Contains("."))
        {
          try
          {
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(selfDescribingSessionToken);
            return true;
          }
          catch
          {
            return false;
          }
        }
      }
      return false;
    }

    protected virtual FrameworkBasicAuthenticationModule.AuthenticationStatus AuthenticateAsAlternateCredential(
      IVssRequestContext requestContext,
      HttpContextBase context,
      string username,
      string password)
    {
      return FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
    }

    protected enum AuthenticationStatus
    {
      Unhandled,
      Authenticated,
      Rejected,
      Rejected_ExpiredPat,
      Rejected_AltCredsDeprecated,
    }
  }
}
