// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationAuthenticationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Common.Utility;
using Microsoft.VisualStudio.Services.Compliance;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Identity.Cache;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class TeamFoundationAuthenticationService : 
    ITeamFoundationAuthenticationService,
    IVssFrameworkService,
    ITeamFoundationAuthenticationServiceInternal
  {
    public const string FedAuthCookieName = "FedAuth";
    public const string FedAuth1CookieName = "FedAuth1";
    public const string FedAuthSignoutCookieName = "FedSignOut";
    public const string MsalJsSignoutCookieName = "MsalJsSignout";
    public const string RequestAuthenticationVaidationFeature = "VisualStudio.AuthenticationService.RequestAuthenticationValidation";
    public const string DreamlifterRedirectToGitHubWhenAuthIsRequiredFeature = "Dreamlifter.RedirectToGitHubWhenAuthIsRequired";
    private const string c_aadSignoutUrlFmtLocation = "/OrgId/Authentication/SignOutUrl";
    private const string c_OrgIdAuthenticationDisambiguationUrlRegistryKey = "/OrgId/Authentication/DisambiguationEndpointUrl";
    private const string c_defaultFedAuthIssuer = "https://www.visualstudio.com/";
    private const string c_CompleteInvalidRequestInValidateSessionSecurityTokenUpdateHandlersFlag = "VisualStudio.Services.Authentication.SessionSecurityTokenUpdateHandlers.CompleteInvalidRequest";
    private const string c_CompactQueryStringKey = "compact";
    private const string c_IsClientRequestTrueQueryStringValue = "1";
    private const string c_SigninNotificationProtocol = "protocol";
    private const string c_JavascriptNotifyProtocol = "javascriptnotify";
    private const string confPropertyPath = "Identity.UserAuthenticationSessionToken.PreserveValidFromOnRefresh";
    private static readonly IConfigPrototype<bool> preserveValidFromConfigPrototype = ConfigPrototype.Create<bool>("Identity.UserAuthenticationSessionToken.PreserveValidFromOnRefresh", false);
    private readonly IConfigQueryable<bool> preserveValidFromConfig;
    private readonly IAadAuthenticationSessionTokenProvider aadAuthenticationSessionTokenProvider;
    private Guid m_ServiceHostId;
    private bool m_fedAuthEnabled;
    private string m_signinRealm;
    private CookieHandlerSettings m_cookieHandlerSettings;
    private FederatedAuthenticationSettings m_federatedAuthenticationSettings;
    private static readonly HashSet<string> s_redirectionSuppressedHttpMethods = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "OPTIONS",
      "HEAD",
      "PROPFIND",
      "PATCH"
    };
    private const string s_area = "TeamFoundationAuthenticationService";
    private const string s_layer = "IVssFrameworkService";
    internal const string SessionTokenKeyName = "SessionToken";
    internal const string AuthCredentialKeyName = "AuthCredential";

    public TeamFoundationAuthenticationService()
      : this(ConfigProxy.Create<bool>(TeamFoundationAuthenticationService.preserveValidFromConfigPrototype), (IAadAuthenticationSessionTokenProvider) new AadAuthenticationSessionTokenProvider())
    {
    }

    internal TeamFoundationAuthenticationService(
      IConfigQueryable<bool> config,
      IAadAuthenticationSessionTokenProvider aadAuthenticationSessionTokenProvider)
    {
      this.preserveValidFromConfig = config;
      this.aadAuthenticationSessionTokenProvider = aadAuthenticationSessionTokenProvider ?? (IAadAuthenticationSessionTokenProvider) new AadAuthenticationSessionTokenProvider();
    }

    internal TeamFoundationAuthenticationService(
      CookieHandlerSettings cookieHandlerSettings,
      IAadAuthenticationSessionTokenProvider aadAuthenticationSessionTokenProvider = null)
    {
      this.m_fedAuthEnabled = true;
      this.m_cookieHandlerSettings = cookieHandlerSettings;
      this.aadAuthenticationSessionTokenProvider = aadAuthenticationSessionTokenProvider ?? (IAadAuthenticationSessionTokenProvider) new AadAuthenticationSessionTokenProvider();
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
      this.m_ServiceHostId = requestContext.ServiceHost.InstanceId;
      this.LoadSettings(requestContext);
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), FederatedAuthRegistryConstants.Root + "/...", HighTrustIdentitiesRegistryConstants.Root + "/...");
      if (!this.m_fedAuthEnabled)
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      service.RegisterNotification(vssRequestContext, new StrongBoxItemChangedCallback(this.OnFedAuthCertStrongBoxItemChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[2]
      {
        ServicingTokenConstants.FedAuthCookieSigningCertificateThumbprint,
        ServicingTokenConstants.SecondaryFedAuthCookieSigningCertificateThumbprint
      });
      service.RegisterNotification(vssRequestContext, new StrongBoxItemChangedCallback(this.OnServiceIdentityStrongBoxItemChanged), FederatedAuthRegistryConstants.SigningCertDrawName, (IEnumerable<string>) new string[1]
      {
        FederatedAuthRegistryConstants.ServiceIdentitySigningKeyLookupKey
      });
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      if (!this.m_fedAuthEnabled)
        return;
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      service.UnregisterNotification(vssRequestContext, new StrongBoxItemChangedCallback(this.OnFedAuthCertStrongBoxItemChanged));
      service.UnregisterNotification(vssRequestContext, new StrongBoxItemChangedCallback(this.OnServiceIdentityStrongBoxItemChanged));
    }

    public void ConfigureBasic(IVssRequestContext requestContext, Uri realm)
    {
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      service.SetValue<string>(requestContext, FederatedAuthRegistryConstants.DefaultRealm, realm.AbsoluteUri);
      if (this.m_federatedAuthenticationSettings != null)
        this.m_federatedAuthenticationSettings.DefaultRealm = realm.AbsoluteUri;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      service.SetValue<string>(requestContext, FederatedAuthRegistryConstants.Issuer, "https://www.visualstudio.com/");
      service.SetValue<string>(requestContext, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) FederatedAuthRegistryConstants.SignOutLocations, (object) "Microsoft Account"), "https://login.live.com/login.srf?wa=wsignout1.0");
      service.SetValue<bool>(requestContext, FederatedAuthRegistryConstants.RequireSsl, realm.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase));
      service.SetValue<bool>(requestContext, FederatedAuthRegistryConstants.Enabled, true);
    }

    public void ConfigureRequest(IVssRequestContext requestContext)
    {
      if (!this.m_fedAuthEnabled)
        return;
      this.ConfigureRequestImpl(requestContext);
    }

    public List<SessionSecurityTokenData> GetSessionSecurityTokenDataFromCookies(
      IVssRequestContext requestContext)
    {
      HttpContext current = HttpContext.Current;
      HttpContextBase context = (HttpContextBase) new HttpContextWrapper(current);
      List<SessionSecurityTokenData> tokenDataFromCookies = new List<SessionSecurityTokenData>();
      UserAuthenticationSessionToken authenticationSessionToken1 = UserAuthenticationSessionTokenHandler.ReadSessionToken(requestContext, context);
      if (authenticationSessionToken1 != null)
      {
        string[] tokenData = new string[1]
        {
          authenticationSessionToken1.SecurityToken.RawData
        };
        tokenDataFromCookies.Add(new SessionSecurityTokenData(SessionTokenType.UserAuthentication, (IReadOnlyList<string>) tokenData));
      }
      UserAuthenticationSessionToken authenticationSessionToken2 = this.aadAuthenticationSessionTokenProvider.ReadSessionToken(requestContext, context);
      if (authenticationSessionToken2 != null)
      {
        List<string> tokenData = new List<string>()
        {
          authenticationSessionToken2.SecurityToken.RawData
        };
        string str;
        if (requestContext.RootContext.Items.TryGetValue<string>("SpaAuthorizationCode", out str) && !string.IsNullOrEmpty(str))
          tokenData.Add(str);
        tokenDataFromCookies.Add(new SessionSecurityTokenData(SessionTokenType.AadAuthentication, (IReadOnlyList<string>) tokenData));
      }
      List<string> stringList = new List<string>();
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) current.Response.Cookies.AllKeys, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HttpCookieCollection cookies = current.Response.Cookies;
      if (!stringSet.Contains("FedAuth"))
      {
        cookies = current.Request.Cookies;
        stringSet = new HashSet<string>((IEnumerable<string>) current.Request.Cookies.AllKeys, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
      if (stringSet.Contains("FedAuth"))
      {
        stringList.Add(cookies["FedAuth"].Value);
        for (int index = 1; index < 50; ++index)
        {
          string name = "FedAuth" + index.ToString();
          if (stringSet.Contains(name))
            stringList.Add(cookies[name].Value);
          else
            break;
        }
      }
      if (stringList.Count > 0)
        tokenDataFromCookies.Add(new SessionSecurityTokenData(SessionTokenType.FedAuth, (IReadOnlyList<string>) stringList.ToArray()));
      return tokenDataFromCookies;
    }

    public IAuthCredential GetAuthCredential() => TeamFoundationAuthenticationService.GetContextItem("AuthCredential") as IAuthCredential;

    public string GetSignInRedirectLocation(
      IVssRequestContext requestContext,
      bool force = false,
      IDictionary<string, string> parameters = null,
      Uri replyToOverride = null,
      SwitchHintParameter switchHintParameter = null)
    {
      if (requestContext.IsFeatureEnabled("Dreamlifter.RedirectToGitHubWhenAuthIsRequired"))
        return "https://github.com";
      Uri uri = requestContext.RequestUri();
      bool flag1 = uri.Query.IndexOf("protocol=javascriptnotify", StringComparison.OrdinalIgnoreCase) >= 0;
      bool flag2 = uri.Query.IndexOf("compact=1", StringComparison.OrdinalIgnoreCase) >= 0;
      bool isIDE = flag1 | flag2;
      StringBuilder queryStringBuilder = new StringBuilder(this.GetRedirectLocation(requestContext, "_signin", "redirect=1", replyToOverride, switchHintParameter, isIDE));
      if (requestContext.IsPublicResourceLicense())
        queryStringBuilder.AppendVisibility("1");
      if (flag1)
        queryStringBuilder.Append("&protocol=javascriptnotify");
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        queryStringBuilder.Append("&hid=" + requestContext.ServiceHost.InstanceId.ToString("D"));
      if (force)
        queryStringBuilder.Append("&force=1");
      string str1 = SignInContextFactory.Deconstruct(SignInContextFactory.Construct(requestContext));
      if (!string.IsNullOrEmpty(str1))
      {
        queryStringBuilder.Append("&context=");
        queryStringBuilder.Append(str1);
      }
      if (parameters != null)
      {
        foreach (KeyValuePair<string, string> parameter in (IEnumerable<KeyValuePair<string, string>>) parameters)
        {
          queryStringBuilder.Append("&");
          queryStringBuilder.Append(parameter.Key);
          queryStringBuilder.Append("=");
          queryStringBuilder.Append(parameter.Value);
        }
      }
      FederatedAuthenticationSettings authenticationSettings = this.AuthenticationServiceInternal().GetFederatedAuthenticationSettings(requestContext);
      if (requestContext.UserAgent != null && authenticationSettings.ClientSignInOptions.Count > 0)
      {
        string upperInvariant = requestContext.UserAgent.ToUpperInvariant();
        foreach (FederatedAuthenticationSettings.CustomClientSignInOptions clientSignInOption in authenticationSettings.ClientSignInOptions)
        {
          if (PatternUtility.Match(upperInvariant, clientSignInOption.UserAgent))
          {
            queryStringBuilder.Append(clientSignInOption.Options);
            break;
          }
        }
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      VssSignInContextModel signInContextModel = new VssSignInContextModel()
      {
        SignInCookieDomains = new List<string>(2)
      };
      IAADAuthSettings aadAuthSettings = vssRequestContext.GetService<IOAuth2SettingsService>().GetAADAuthSettings(vssRequestContext);
      if (!string.IsNullOrEmpty(aadAuthSettings.Authority))
        signInContextModel.SignInCookieDomains.Add("https://" + aadAuthSettings.Authority);
      string uriString = vssRequestContext.GetService<ICachedRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/OrgId/Authentication/DisambiguationEndpointUrl", false, (string) null);
      if (!string.IsNullOrEmpty(uriString))
      {
        string host = new Uri(uriString).Host;
        signInContextModel.SignInCookieDomains.Add("https://" + host);
      }
      string str2 = UrlEncodingUtility.UrlTokenEncode(JsonConvert.SerializeObject((object) signInContextModel));
      queryStringBuilder.Append("#ctx=");
      queryStringBuilder.Append(str2);
      return queryStringBuilder.ToString();
    }

    public void AddFederatedAuthHeaders(
      IVssRequestContext requestContext,
      HttpResponseBase response)
    {
      if (!this.m_fedAuthEnabled)
        return;
      if (string.IsNullOrWhiteSpace(response.Headers.Get("X-TFS-FedAuthRealm")))
      {
        string realm = this.DetermineRealm(requestContext);
        response.AddHeader("X-TFS-FedAuthRealm", realm);
      }
      if (string.IsNullOrWhiteSpace(response.Headers.Get("X-TFS-FedAuthIssuer")))
      {
        IFederatedAuthIssuer extension = requestContext.GetExtension<IFederatedAuthIssuer>();
        if (extension != null && extension.IsEnabled(requestContext))
          response.AddHeader("X-TFS-FedAuthIssuer", extension.GetIssuerLocation(requestContext));
        else
          response.AddHeader("X-TFS-FedAuthIssuer", this.m_federatedAuthenticationSettings.Issuer);
      }
      if (!string.IsNullOrWhiteSpace(response.Headers.Get("X-VSS-AuthorizationEndpoint")))
        return;
      string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.SPS, AccessMappingConstants.PublicAccessMappingMoniker);
      if (string.IsNullOrEmpty(locationServiceUrl))
        return;
      response.AddHeader("X-VSS-AuthorizationEndpoint", locationServiceUrl);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AddFederatedAuthHeaders(
      IVssRequestContext requestContext,
      HttpResponseMessage response)
    {
      if (!this.m_fedAuthEnabled)
        return;
      string realm = this.DetermineRealm(requestContext);
      response.Headers.Add("X-TFS-FedAuthRealm", realm);
      IFederatedAuthIssuer extension = requestContext.GetExtension<IFederatedAuthIssuer>();
      if (extension != null && extension.IsEnabled(requestContext))
        response.Headers.Add("X-TFS-FedAuthIssuer", extension.GetIssuerLocation(requestContext));
      else
        response.Headers.Add("X-TFS-FedAuthIssuer", this.m_federatedAuthenticationSettings.Issuer);
      string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.SPS, AccessMappingConstants.PublicAccessMappingMoniker);
      if (string.IsNullOrEmpty(locationServiceUrl))
        return;
      response.Headers.Add("X-VSS-AuthorizationEndpoint", locationServiceUrl);
    }

    public void SignOutFromSessionModule(IVssRequestContext requestContext) => this.DeleteSessionSecurityToken(requestContext);

    public string LocationForRealm(IVssRequestContext requestContext, string relativePath)
    {
      if (!this.m_fedAuthEnabled)
        throw new InvalidOperationException();
      string path1 = this.m_signinRealm;
      if (string.IsNullOrEmpty(path1))
        path1 = this.DetermineRealm(requestContext);
      return TFCommonUtil.CombinePaths(path1, relativePath);
    }

    public string DetermineRealm(IVssRequestContext requestContext)
    {
      string realm;
      if (!requestContext.RootContext.TryGetItem<string>(RequestContextItemsKeys.Realm, out realm))
        this.DetermineRealmAndDomain(requestContext, out realm, out string _);
      return realm;
    }

    public void RedirectToIdentityProvider(
      IVssRequestContext requestContext,
      bool hasInvalidToken = false,
      bool force = false)
    {
      HttpContextBase current = HttpContextFactory.Current;
      if (current.Items.Contains((object) HttpContextConstants.ArrRequestRouted))
        return;
      AuthenticationHelpers.EnterMethodIfNull(requestContext, "Authentication", "RedirectingToAuthProvider", true);
      if (hasInvalidToken)
        current.Response.AddHeader("X-VSS-AuthenticateError", "InvalidToken");
      this.AddFederatedAuthHeaders(requestContext, current.Response);
      this.AddTenantInfoResponseHeader(requestContext, current.Response);
      string redirectLocation = this.GetSignInRedirectLocation(requestContext, force, (IDictionary<string, string>) null, (Uri) null, (SwitchHintParameter) null);
      if ((TeamFoundationAuthenticationService.IsFedAuthRedirectSuppressed(current.Request) ? 1 : ((requestContext.RequestRestrictions().MechanismsToAdvertise & AuthenticationMechanisms.FederatedRedirect) != AuthenticationMechanisms.None ? 0 : ((requestContext.RequestRestrictions().MechanismsToAdvertise & AuthenticationMechanisms.Federated) != 0 ? 1 : 0))) != 0 || TeamFoundationAuthenticationService.s_redirectionSuppressedHttpMethods.Contains(current.Request.HttpMethod))
      {
        if (!string.IsNullOrWhiteSpace(HttpContext.Current.Response.Headers.Get("X-TFS-FedAuthRedirect")))
          return;
        current.Response.AddHeader("WWW-Authenticate", "TFS-Federated");
        current.Response.AddHeader("X-TFS-FedAuthRedirect", redirectLocation);
      }
      else
      {
        current.Server.ClearError();
        current.Response.Redirect(redirectLocation, false);
        current.ApplicationInstance.CompleteRequest();
      }
    }

    public void ProcessSignOutCookie(IVssRequestContext requestContext, Uri realmUri)
    {
      Uri uri1 = new UriBuilder(realmUri)
      {
        Path = "/_signout",
        Query = "wa=wsignoutcleanup1.0"
      }.Uri;
      SignOutUris signOutUris = this.ReadSignOutCookie(requestContext);
      if (signOutUris.CookieSignOutUris.Add(uri1))
        this.WriteSignOutCookie(requestContext, signOutUris.CookieSignOutUris);
      if (!this.aadAuthenticationSessionTokenProvider.Configuration.IssueAadAuthenticationCookieEnabled(requestContext))
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string parentDomain = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, in ConfigurationConstants.DevOpsRootDomain);
      UriBuilder uriBuilder = new UriBuilder(realmUri)
      {
        Path = "/_public/_msaljssignout"
      };
      if (UriUtility.IsSubdomainOf(uriBuilder.Host, parentDomain))
        uriBuilder.Host = parentDomain;
      Uri uri2 = uriBuilder.Uri;
      if (!signOutUris.MsalJsSignOutUris.Add(uri2))
        return;
      this.WriteMsalJsSignOutCookie(requestContext, signOutUris.MsalJsSignOutUris);
    }

    public string BuildHostedSignOutUrl(IVssRequestContext requestContext)
    {
      requestContext.CheckHostedDeployment();
      return TFCommonUtil.CombinePaths(TeamFoundationAuthenticationService.GetSpsDeploymentUrl(requestContext), "_signout");
    }

    public string BuildAADSignOutUrl(IVssRequestContext requestContext, string callBackUrl = null)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string format = vssRequestContext.GetService<CachedRegistryService>().GetValue(vssRequestContext, (RegistryQuery) "/OrgId/Authentication/SignOutUrl", "https://login.microsoftonline.com/common/oauth2/logout?post_logout_redirect_uri={0}");
      Uri uri = new Uri(requestContext.GetService<ILocationService>().DetermineAccessMapping(requestContext).AccessPoint);
      if (string.IsNullOrEmpty(callBackUrl))
      {
        callBackUrl = new UriBuilder(uri.Scheme, uri.Host, uri.Port, "/brand/logo").Uri.AbsoluteUri;
      }
      else
      {
        SecureFlowLocation location;
        int num = (int) SecureFlowLocation.TryCreate(requestContext, callBackUrl, SecureFlowLocation.GetDefaultLocation(requestContext), out location);
        callBackUrl = location.ToString();
      }
      string str = Uri.EscapeDataString(callBackUrl);
      return new UriBuilder(string.Format(format, (object) str)).Uri.AbsoluteUri;
    }

    public SignOutUris ReadSignOutCookie(IVssRequestContext requestContext)
    {
      ISet<Uri> cookieSignOutUris = this.ReadSignOutCookie(requestContext, "FedSignOut");
      ISet<Uri> uriSet = (ISet<Uri>) null;
      if (this.aadAuthenticationSessionTokenProvider.Configuration.IssueAadAuthenticationCookieEnabled(requestContext))
        uriSet = this.ReadSignOutCookie(requestContext, "MsalJsSignout");
      ISet<Uri> msalJsSignOutUris = uriSet;
      return new SignOutUris(cookieSignOutUris, msalJsSignOutUris);
    }

    private ISet<Uri> ReadSignOutCookie(IVssRequestContext requestContext, string cookieName)
    {
      HttpCookie httpCookie = HttpContext.Current.Request.Cookies.Get(cookieName);
      if (httpCookie == null)
        return (ISet<Uri>) new HashSet<Uri>();
      try
      {
        return (ISet<Uri>) new HashSet<Uri>(((IEnumerable<string>) Encoding.UTF8.GetString(Convert.FromBase64String(httpCookie.Value)).Split(';')).Select<string, Uri>((Func<string, Uri>) (token => new Uri(token))));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1010600, TraceLevel.Warning, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", ex);
        return (ISet<Uri>) new HashSet<Uri>();
      }
    }

    public void ClearSignOutCookie(IVssRequestContext requestContext)
    {
      this.WriteSignOutCookie(requestContext, (ISet<Uri>) null);
      this.WriteMsalJsSignOutCookie(requestContext, (ISet<Uri>) null);
    }

    private void WriteMsalJsSignOutCookie(IVssRequestContext requestContext, ISet<Uri> signOutUris) => this.WriteSignOutCookie(requestContext, "MsalJsSignout", signOutUris);

    private void WriteSignOutCookie(IVssRequestContext requestContext, ISet<Uri> signOutUris) => this.WriteSignOutCookie(requestContext, "FedSignOut", signOutUris);

    private void WriteSignOutCookie(
      IVssRequestContext requestContext,
      string cookieName,
      ISet<Uri> signOutUris)
    {
      HttpContext current = HttpContext.Current;
      string str = (string) null;
      if (signOutUris != null)
      {
        string[] strArray = new string[signOutUris.Count];
        int num = 0;
        foreach (Uri signOutUri in (IEnumerable<Uri>) signOutUris)
          strArray[num++] = signOutUri.AbsoluteUri;
        str = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join(";", strArray)));
      }
      HttpCookie cookie = new HttpCookie(cookieName, str)
      {
        Path = "/",
        HttpOnly = true,
        Secure = this.m_cookieHandlerSettings.RequireSsl
      };
      cookie.Expires = str != null ? (current.Request.Cookies["Tfs-StaySignedIn"] == null ? DateTime.UtcNow.AddYears(1) : new DateTime(2037, 12, 30, 16, 0, 0, DateTimeKind.Utc)) : DateTime.UtcNow.AddDays(-1.0);
      CookieModifier.AddSameSiteNoneToCookie(requestContext, cookie);
      current.Response.Cookies.Set(cookie);
    }

    bool ITeamFoundationAuthenticationServiceInternal.IsRequestAuthenticationValid(
      IVssWebRequestContext requestContext,
      bool isSshRequest)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || requestContext.UserContext == (IdentityDescriptor) null)
        return true;
      if (requestContext.RequestRestrictions() != null && requestContext.RequestRestrictions().RequiredAuthentication >= RequiredAuthentication.Authenticated)
      {
        if (!requestContext.RequestRestrictions().HasAnyLabel("Signout"))
        {
          NameValueCollection queryString = HttpUtility.ParseQueryString(requestContext.RequestUri.Query);
          bool result;
          if (((string.IsNullOrEmpty(queryString["forceSignout"]) ? 0 : (bool.TryParse(queryString["forceSignout"], out result) ? 1 : 0)) & (result ? 1 : 0)) != 0)
          {
            requestContext.Trace(1011110, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "Skipping IsRequestAuthenticationValid. Current request query params contains forceSignout.");
            return true;
          }
          foreach (IRequestAuthenticationValidator extension in (IEnumerable<IRequestAuthenticationValidator>) requestContext.GetExtensions<IRequestAuthenticationValidator>(ExtensionLifetime.Service))
          {
            if (!extension.IsValid((IVssRequestContext) requestContext, (ITeamFoundationAuthenticationService) this))
            {
              requestContext.Trace(1011112, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "Current request authentication is not valid. Request validator {0} returned false. Completing invalid request", (object) extension.GetType().Name);
              if (!isSshRequest)
                extension.CompleteInvalidRequest((IVssRequestContext) requestContext, (ITeamFoundationAuthenticationService) this);
              else
                extension.ThrowNonHttpException((IVssRequestContext) requestContext);
              return false;
            }
          }
          requestContext.Trace(1011110, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "Current request authentication is valid.");
          return true;
        }
      }
      requestContext.Trace(1011110, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "Skipping IsRequestAuthenticationValid. Current request restriction labels {0} matched Signout request", requestContext.RequestRestrictions() == null ? (object) string.Empty : (object) requestContext.RequestRestrictions().Label);
      return true;
    }

    FederatedAuthenticationSettings ITeamFoundationAuthenticationServiceInternal.GetFederatedAuthenticationSettings(
      IVssRequestContext requestContext)
    {
      return this.m_fedAuthEnabled ? this.m_federatedAuthenticationSettings : (FederatedAuthenticationSettings) null;
    }

    void ITeamFoundationAuthenticationServiceInternal.SetAuthenticationCredential(
      IAuthCredential credential)
    {
      if (credential == null)
        return;
      TeamFoundationAuthenticationService.SetContextItem("AuthCredential", (object) credential);
    }

    string ITeamFoundationAuthenticationServiceInternal.GetCookieRootDomain(
      IVssRequestContext requestContext)
    {
      return this.m_cookieHandlerSettings.GetCookieDomain(requestContext.RequestUri()?.Host) ?? this.m_cookieHandlerSettings.DefaultDomain;
    }

    string ITeamFoundationAuthenticationServiceInternal.GetSessionCookieDomain(
      IVssRequestContext requestContext)
    {
      return this.GetSessionCookieDomain(requestContext);
    }

    internal string GetSessionCookieDomain(IVssRequestContext requestContext) => this.m_cookieHandlerSettings.GetCookieDomain(requestContext.RequestUri()?.Host);

    bool ITeamFoundationAuthenticationServiceInternal.IssueSessionSecurityToken(
      IVssRequestContext requestContext,
      ClaimsPrincipal claimsPrincipal,
      IEnumerable<Claim> additionalClaims)
    {
      return AuthenticationHelpers.IssueSessionSecurityToken(requestContext, claimsPrincipal, additionalClaims, (Action<SessionSecurityToken>) (sessionToken => TeamFoundationAuthenticationService.SetContextItem("SessionToken", (object) sessionToken)));
    }

    void ITeamFoundationAuthenticationServiceInternal.ReissueFedAuthToken(
      IVssRequestContext requestContext)
    {
      if (!(TeamFoundationAuthenticationService.GetContextItem("SessionToken") is SessionSecurityToken contextItem))
        return;
      ClaimsPrincipal claimsPrincipal = contextItem.ClaimsPrincipal;
      this.ValidateSessionSecurityTokenUpdateHandlers(requestContext, claimsPrincipal);
      SessionSecurityToken sessionToken = this.IssueNewSessionSecurityToken(requestContext, claimsPrincipal, contextItem);
      FederatedAuthentication.SessionAuthenticationModule.WriteSessionTokenToCookie(sessionToken);
      TeamFoundationAuthenticationService.SetContextItem("SessionToken", (object) sessionToken);
    }

    internal SessionSecurityToken UpdateSessionSecurityToken(
      IVssRequestContext requestContext,
      SessionSecurityToken currentToken)
    {
      requestContext.TraceEnter(1010610, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", nameof (UpdateSessionSecurityToken));
      SessionSecurityToken sessionSecurityToken = (SessionSecurityToken) null;
      Stopwatch stopwatch = new Stopwatch();
      try
      {
        if (AuthenticationHelpers.ShouldSkipReissuingAuthToken(requestContext) || currentToken.ValidTo <= DateTime.UtcNow)
          return (SessionSecurityToken) null;
        stopwatch.Start();
        CookieHandlerSettings cookieHandlerSettings = this.GetCookieHandlerSettings(requestContext);
        if (cookieHandlerSettings.SlidingExpiration > TimeSpan.Zero)
        {
          TimeSpan timeSpan1 = currentToken.ValidTo - DateTime.UtcNow;
          TimeSpan timeSpan2 = cookieHandlerSettings.SlidingExpiration - timeSpan1;
          if (timeSpan2 < TimeSpan.Zero || timeSpan2 > cookieHandlerSettings.TokenReissueDelay)
          {
            if (requestContext.IsTracing(1010601, TraceLevel.Verbose, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService"))
              requestContext.Trace(1010601, TraceLevel.Verbose, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "Reissuing token: timeSinceIssued: {0} | reissueDelay: {1}", (object) timeSpan2.TotalSeconds, (object) cookieHandlerSettings.TokenReissueDelay.TotalSeconds);
            VssPerformanceEventSource.Log.RefreshSecurityTokenStart(requestContext.UniqueIdentifier, currentToken.ClaimsPrincipal.Identity.Name);
            ClaimsPrincipal claimsPrincipal = currentToken.ClaimsPrincipal;
            this.ValidateSessionSecurityTokenUpdateHandlers(requestContext, claimsPrincipal);
            if (HttpContextFactory.Current?.Request?.Cookies["UserAuthentication"] == null)
            {
              if (this.preserveValidFromConfig.QueryByCtx<bool>(requestContext))
              {
                requestContext.Trace(17000, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "Upgrading FedAuth token to UserAuth, while preserving ValidFrom property");
                UserAuthenticationSessionTokenHandler.UpgradeFromFedAuthToUserAuthToken(requestContext, (HttpContextBase) new HttpContextWrapper(HttpContext.Current), claimsPrincipal, (System.IdentityModel.Tokens.SecurityToken) currentToken);
              }
              else
              {
                requestContext.Trace(17001, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "Issuing UserAuth session token");
                UserAuthenticationSessionTokenHandler.IssueSessionToken(requestContext, (HttpContextBase) new HttpContextWrapper(HttpContext.Current), claimsPrincipal);
              }
              return (SessionSecurityToken) null;
            }
            requestContext.TraceAlways(1010614, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "Did not have a UserAuth cookie, will issue a FedAuth");
            sessionSecurityToken = this.IssueNewSessionSecurityToken(requestContext, claimsPrincipal, currentToken);
          }
        }
        return sessionSecurityToken;
      }
      finally
      {
        if (sessionSecurityToken != null)
        {
          stopwatch.Stop();
          Guid guid;
          sessionSecurityToken.ContextId.TryGetGuid(out guid);
          VssPerformanceEventSource.Log.RefreshSecurityTokenStop(requestContext.UniqueIdentifier, sessionSecurityToken.ClaimsPrincipal.Identity.Name, sessionSecurityToken.ValidFrom, sessionSecurityToken.ValidTo, guid, stopwatch.ElapsedMilliseconds);
        }
        requestContext.TraceLeave(1010611, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", nameof (UpdateSessionSecurityToken));
      }
    }

    IdentityValidationResult ITeamFoundationAuthenticationServiceInternal.CompleteUnauthorizedRequest(
      IVssRequestContext requestContext,
      HttpResponseBase response,
      IdentityValidationResult validationResult,
      Uri redirectUrl)
    {
      requestContext.TraceConditionally(15191601, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", (Func<string>) (() => string.Format("Completing response with status {0} --- {1}", (object) validationResult.HttpStatusCode, (object) EnvironmentWrapper.ToReadableStackTrace())));
      if (this.m_fedAuthEnabled)
      {
        this.AddFederatedAuthHeaders(requestContext, response);
        SecureFlowLocation.GetDefaultLocation(requestContext);
        try
        {
          this.AddTenantInfoResponseHeader(requestContext, response);
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(1010645, TraceLevel.Warning, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", ex.Message);
        }
        if (validationResult.HttpStatusCode == HttpStatusCode.Unauthorized && !requestContext.IsAnonymous())
        {
          if (string.IsNullOrWhiteSpace(response.Headers.Get("X-TFS-FedAuthRedirect")))
          {
            string redirectLocation = this.GetSignInRedirectLocation(requestContext, false, (IDictionary<string, string>) null, (Uri) null, (SwitchHintParameter) null);
            response.AddHeader("WWW-Authenticate", "TFS-Federated");
            response.AddHeader("X-TFS-FedAuthRedirect", redirectLocation);
          }
          response.AddHeader("X-VSS-AuthenticateError", "Unauthorized");
          AuthenticationHelpers.EnterMethodIfNull(requestContext, "Authentication", "CompleteUnauthorizedRequest");
        }
        else if (validationResult.HttpStatusCode == HttpStatusCode.Forbidden && redirectUrl != (Uri) null && (requestContext.RequestRestrictions().MechanismsToAdvertise & AuthenticationMechanisms.FederatedRedirect) != AuthenticationMechanisms.None)
        {
          string clientSku = (string) null;
          string clientVersion = (string) null;
          Uri uri = requestContext.RequestUri();
          SecureFlowLocation defaultLocation = SecureFlowLocation.GetDefaultLocation(requestContext);
          SecureFlowLocation location1;
          int num1 = (int) SecureFlowLocation.TryCreate(requestContext, uri.AbsoluteUri, defaultLocation, out location1);
          if (redirectUrl.AbsolutePath.Contains("profile"))
            TeamFoundationAuthenticationService.ExtractClientSkuAndVersion(ref location1, out clientSku, out clientVersion);
          NameValueCollection state = AadAuthUrlUtility.ParseState(requestContext);
          if (state != null)
          {
            string urlText = state["reply_to"];
            if (!string.IsNullOrEmpty(urlText))
            {
              SecureFlowLocation location2;
              int num2 = (int) SecureFlowLocation.TryCreate(requestContext, urlText, defaultLocation, out location2);
              location1.Parameters.Clear();
              location1.NextLocation = location2;
              location1.ApplyParameters(state);
              if (redirectUrl.AbsolutePath.Contains("profile"))
                TeamFoundationAuthenticationService.ExtractClientSkuAndVersion(ref location2, out clientSku, out clientVersion);
            }
            if (location1.Path.Contains("_signedin"))
            {
              string realmFromReplyTos = this.GetRealmFromReplyTos(location1);
              if (!string.IsNullOrEmpty(realmFromReplyTos))
                this.ApplyRealmToAllSignedInLocations(location1, realmFromReplyTos);
              state["realm"] = realmFromReplyTos;
            }
          }
          if (location1.Path.StartsWith("/profile/", StringComparison.InvariantCultureIgnoreCase))
            location1.Path = "/go/profile";
          string parameter1 = location1.Parameters["cache_key"];
          if (parameter1 != null)
          {
            string key = Uri.UnescapeDataString(parameter1);
            string urlText;
            if (requestContext.GetService<IL2CacheService>().TryGet<string>(requestContext, key, out urlText))
            {
              SecureFlowLocation location3;
              int num3 = (int) SecureFlowLocation.TryCreate(requestContext, urlText, (SecureFlowLocation) null, out location3);
              location1.Parameters.Remove("cache_key");
              location1.NextLocation = location3;
            }
          }
          if (location1.NextLocation != null)
          {
            if (location1.NextLocation.Path.StartsWith("/profile/", StringComparison.InvariantCultureIgnoreCase))
              location1.NextLocation.Path = "/go/profile";
            else if (location1.NextLocation.NextLocation != null && location1.NextLocation.NextLocation.Path.StartsWith("/profile/", StringComparison.InvariantCultureIgnoreCase))
              location1.NextLocation.NextLocation.Path = "/go/profile";
          }
          SecureFlowLocation location4;
          int num4 = (int) SecureFlowLocation.TryCreate(requestContext, redirectUrl, defaultLocation, out location4);
          SecureFlowLocation location5 = location4;
          if (location4.HasSameTarget(location1, true))
          {
            location5 = location4.Clone();
            location5.ApplyParameters(location1.Parameters);
          }
          else if (location1.NextLocation != null && location4.HasSameTarget(location1.NextLocation))
          {
            location5 = location4.Clone();
            location5.ApplyParameters(location1.NextLocation.Parameters);
            SecureFlowLocation location6 = location1.Clone();
            location6.NextLocation = (SecureFlowLocation) null;
            location5.NextLocation = location6.ForContext(requestContext, true);
          }
          else if (location1.NextLocation?.NextLocation != null && location4.HasSameTarget(location1.NextLocation.NextLocation))
          {
            location5 = location4.Clone();
            location5.ApplyParameters(location1.NextLocation.NextLocation.Parameters);
            SecureFlowLocation location7 = location1.Clone();
            location7.NextLocation = (SecureFlowLocation) null;
            location5.NextLocation = location7.ForContext(requestContext, true);
          }
          else
          {
            location5.NextLocation = location1.ForContext(requestContext, true);
            string parameter2 = location1.Parameters["mkt"];
            string parameter3 = location5.Parameters["mkt"];
            if (!string.IsNullOrEmpty(parameter2) && parameter2 != parameter3)
              location5.Parameters["mkt"] = parameter2;
            if (clientVersion != null)
              location5.Parameters.Add("cv", clientVersion);
            if (clientSku != null)
              location5.Parameters.Add("cs", clientSku);
            if (!string.IsNullOrEmpty(location1.Parameters["mode"]))
              location5.ApplyParameters(location1.Parameters);
            else if (location1.NextLocation != null)
            {
              if (!string.IsNullOrEmpty(location1.NextLocation.Parameters["mode"]))
                location5.ApplyParameters(location1.NextLocation.Parameters);
              else if (location1.NextLocation.NextLocation != null && !string.IsNullOrEmpty(location1.NextLocation.NextLocation.Parameters["mode"]))
                location5.ApplyParameters(location1.NextLocation.NextLocation.Parameters);
            }
          }
          string str = location5.ForFlow(location1).ForContext(requestContext, false).ToString();
          response.AddHeader("X-TFS-FedAuthRedirect", str);
          return IdentityValidationResult.Forbidden(FrameworkResources.NoncompliantUserResolutionError((object) validationResult.ResultMessage, (object) str));
        }
      }
      return validationResult;
    }

    internal static object GetContextItem(string name) => HttpContextFactory.Current != null ? HttpContextFactory.Current.Items[(object) TeamFoundationAuthenticationService.GetContextKey(name)] : (object) null;

    internal static void SetContextItem(string name, object value)
    {
      if (HttpContextFactory.Current == null)
        return;
      HttpContextFactory.Current.Items[(object) TeamFoundationAuthenticationService.GetContextKey(name)] = value;
    }

    private static void OnSessionSecurityTokenReceived(
      object sender,
      SessionSecurityTokenReceivedEventArgs e)
    {
      SessionSecurityToken sessionSecurityToken = TeamFoundationAuthenticationService.UpdateSessionSecurityToken(e.SessionToken);
      if (sessionSecurityToken != null)
      {
        e.SessionToken = sessionSecurityToken;
        e.ReissueCookie = true;
        sessionSecurityToken.ContextId.TryGetGuid(out Guid _);
      }
      TeamFoundationAuthenticationService.SetContextItem("SessionToken", (object) e.SessionToken);
      TeamFoundationAuthenticationService.SetContextItem("AuthCredential", (object) new FederatedAuthCredential(e.SessionToken));
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadSettings(requestContext);
    }

    private void OnFedAuthCertStrongBoxItemChanged(
      IVssRequestContext deploymentContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      using (IVssRequestContext requestContext = deploymentContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(deploymentContext, this.m_ServiceHostId, RequestContextType.SystemContext))
        this.LoadSettings(requestContext);
    }

    private void OnServiceIdentityStrongBoxItemChanged(
      IVssRequestContext deploymentContext,
      IEnumerable<StrongBoxItemName> strongBoxItemNames)
    {
      using (IVssRequestContext requestContext = deploymentContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(deploymentContext, this.m_ServiceHostId, RequestContextType.SystemContext))
        this.LoadSettings(requestContext);
    }

    private static void ExtractClientSkuAndVersion(
      ref SecureFlowLocation sourceLocation,
      out string clientSku,
      out string clientVersion)
    {
      clientSku = "";
      clientVersion = "";
      if (!string.IsNullOrEmpty(sourceLocation.Parameters["cs"]))
      {
        clientSku = sourceLocation.Parameters["cs"];
        sourceLocation.Parameters.Remove("cs");
      }
      if (string.IsNullOrEmpty(sourceLocation.Parameters["cv"]))
        return;
      clientVersion = sourceLocation.Parameters["cv"];
      sourceLocation.Parameters.Remove("cv");
    }

    private string GetRealmFromReplyTos(SecureFlowLocation location)
    {
      if (location == null)
        return (string) null;
      string parameter = location.Parameters["realm"];
      return !string.IsNullOrEmpty(parameter) ? parameter : this.GetRealmFromReplyTos(location.NextLocation);
    }

    private void ApplyRealmToAllSignedInLocations(SecureFlowLocation location, string realm)
    {
      if (location == null)
        return;
      if (location.Path.Contains("_signedin") && string.IsNullOrEmpty(location.Parameters[nameof (realm)]))
        location.Parameters[nameof (realm)] = realm;
      this.ApplyRealmToAllSignedInLocations(location.NextLocation, realm);
    }

    private CookieHandlerSettings GetCookieHandlerSettings(IVssRequestContext requestContext) => this.m_fedAuthEnabled ? this.m_cookieHandlerSettings : (CookieHandlerSettings) null;

    private void ValidateSessionSecurityTokenUpdateHandlers(
      IVssRequestContext requestContext,
      ClaimsPrincipal claimsPrincipalForNewToken)
    {
      IInvalidRequestCompletionService service = requestContext.GetService<IInvalidRequestCompletionService>();
      try
      {
        IDisposableReadOnlyList<ISessionSecurityTokenUpdateHandler> extensions = requestContext.GetExtensions<ISessionSecurityTokenUpdateHandler>(ExtensionLifetime.Service);
        if (extensions.Any<ISessionSecurityTokenUpdateHandler>())
        {
          foreach (ISessionSecurityTokenUpdateHandler tokenUpdateHandler in (IEnumerable<ISessionSecurityTokenUpdateHandler>) extensions)
          {
            ISessionSecurityTokenUpdateHandler handler = tokenUpdateHandler;
            requestContext.TraceDataConditionally(1010622, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "Running Extension", (Func<object>) (() => (object) new
            {
              Name = handler.GetType().Name
            }), nameof (ValidateSessionSecurityTokenUpdateHandlers));
            if (!handler.ValidateSessionTokenSecurityUpdate(requestContext, claimsPrincipalForNewToken) && requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.SessionSecurityTokenUpdateHandlers.CompleteInvalidRequest"))
            {
              requestContext.TraceDataConditionally(1010623, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "Completing invalid request on behalf of Extension", (Func<object>) (() => (object) new
              {
                Name = handler.GetType().Name
              }), nameof (ValidateSessionSecurityTokenUpdateHandlers));
              service.CompleteInvalidRequest(requestContext, (ITeamFoundationAuthenticationService) this, nameof (TeamFoundationAuthenticationService));
            }
            else
              requestContext.TraceDataConditionally(1010624, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "Session security token update validated on behalf of Extension", (Func<object>) (() => (object) new
              {
                Name = handler.GetType().Name
              }), nameof (ValidateSessionSecurityTokenUpdateHandlers));
          }
        }
        else
          requestContext.Trace(1010625, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "No implementations found for interface IInvalidRequestCompletionService");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1010626, TraceLevel.Error, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", ex);
      }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ConfigureRequestImpl(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      FederationConfiguration federationConfiguration = vssRequestContext.GetService<FederationConfigurationService>().GetFederationConfiguration(vssRequestContext);
      SessionAuthenticationModule authenticationModule = FederatedAuthentication.SessionAuthenticationModule;
      if (authenticationModule != null)
      {
        authenticationModule.FederationConfiguration = federationConfiguration;
        CookieHandler cookieHandler1 = authenticationModule.CookieHandler;
        if (cookieHandler1 == FederatedAuthentication.FederationConfiguration.CookieHandler)
        {
          ChunkedCookieHandler chunkedCookieHandler = new ChunkedCookieHandler(((ChunkedCookieHandler) cookieHandler1).ChunkSize);
          chunkedCookieHandler.Domain = cookieHandler1.Domain;
          chunkedCookieHandler.HideFromClientScript = cookieHandler1.HideFromClientScript;
          chunkedCookieHandler.Name = cookieHandler1.Name;
          chunkedCookieHandler.Path = cookieHandler1.Path;
          chunkedCookieHandler.PersistentSessionLifetime = cookieHandler1.PersistentSessionLifetime;
          chunkedCookieHandler.RequireSsl = cookieHandler1.RequireSsl;
          CookieHandler cookieHandler2 = (CookieHandler) chunkedCookieHandler;
          FederatedAuthentication.SessionAuthenticationModule.CookieHandler = cookieHandler2;
          cookieHandler1 = cookieHandler2;
        }
        cookieHandler1.Domain = this.GetSessionCookieDomain(requestContext);
        if (this.m_cookieHandlerSettings.HideFromClientScript.HasValue)
          cookieHandler1.HideFromClientScript = this.m_cookieHandlerSettings.HideFromClientScript.Value;
        cookieHandler1.RequireSsl = this.m_cookieHandlerSettings.RequireSsl;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        EventHandler<SessionSecurityTokenReceivedEventArgs> eventHandler = TeamFoundationAuthenticationService.\u003C\u003EO.\u003C0\u003E__OnSessionSecurityTokenReceived ?? (TeamFoundationAuthenticationService.\u003C\u003EO.\u003C0\u003E__OnSessionSecurityTokenReceived = new EventHandler<SessionSecurityTokenReceivedEventArgs>(TeamFoundationAuthenticationService.OnSessionSecurityTokenReceived));
        authenticationModule.SessionSecurityTokenReceived -= eventHandler;
        authenticationModule.SessionSecurityTokenReceived += eventHandler;
      }
      else
        TeamFoundationTrace.Error("Unable to configure the session module. The module wasn't loaded");
    }

    private static SessionSecurityToken UpdateSessionSecurityToken(SessionSecurityToken currentToken) => !(HttpContextFactory.Current?.Items[(object) "IVssRequestContext"] is IVssRequestContext vssRequestContext) ? (SessionSecurityToken) null : vssRequestContext.GetService<TeamFoundationAuthenticationService>().UpdateSessionSecurityToken(vssRequestContext, currentToken);

    private SessionSecurityToken IssueNewSessionSecurityToken(
      IVssRequestContext requestContext,
      ClaimsPrincipal claimsPrincipal,
      SessionSecurityToken currentToken = null)
    {
      requestContext.TraceEnter(1010612, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", nameof (IssueNewSessionSecurityToken));
      try
      {
        SessionSecurityToken sessionSecurityToken;
        if (currentToken != null)
        {
          CookieHandlerSettings cookieHandlerSettings = this.GetCookieHandlerSettings(requestContext);
          sessionSecurityToken = new SessionSecurityToken(claimsPrincipal, new UniqueId(), currentToken.Context, currentToken.EndpointId, new DateTime?(currentToken.ValidFrom), new DateTime?(DateTime.UtcNow.Add(cookieHandlerSettings.SlidingExpiration)), currentToken.SecurityKeys.Cast<SymmetricSecurityKey>().FirstOrDefault<SymmetricSecurityKey>())
          {
            IsPersistent = currentToken.IsPersistent,
            IsReferenceMode = currentToken.IsReferenceMode
          };
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_reissued_tokens_persec").Increment();
        }
        else
          sessionSecurityToken = FederatedAuthentication.SessionAuthenticationModule.CreateSessionSecurityToken(claimsPrincipal, Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(7.0), true);
        if (requestContext.IsTracing(1010608, TraceLevel.Verbose, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService"))
          requestContext.Trace(1010608, TraceLevel.Verbose, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "New token info: validFrom: {0}, validTo: {1}", (object) sessionSecurityToken.ValidFrom, (object) sessionSecurityToken.ValidTo);
        if (requestContext.IsTracing(1010609, TraceLevel.Verbose, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService"))
          this.TraceClaimsFromClaimsIdentity(requestContext, 1010609, TraceLevel.Verbose, sessionSecurityToken.ClaimsPrincipal.Identity as ClaimsIdentity);
        return sessionSecurityToken;
      }
      finally
      {
        requestContext.TraceLeave(1010613, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", nameof (IssueNewSessionSecurityToken));
      }
    }

    private void DeleteSessionSecurityToken(IVssRequestContext requestContext)
    {
      HttpContextBase httpContext = (HttpContextBase) new HttpContextWrapper(HttpContext.Current);
      UserAuthenticationSessionTokenHandler.DeleteSessionToken(requestContext, httpContext);
      FederatedAuthCookieHelper.DeleteSessionTokenCookie(requestContext);
      try
      {
        this.aadAuthenticationSessionTokenProvider.DeleteSessionToken(requestContext, httpContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(734924, "Authentication", nameof (DeleteSessionSecurityToken), ex);
      }
      if (httpContext == null)
        return;
      HttpCookieCollection cookies = httpContext.Response.Cookies;
      foreach (object key in cookies.Keys)
      {
        if (key is string str && this.IsFedAuthCookie(str))
        {
          HttpCookie httpCookie = cookies[str];
          httpCookie.HttpOnly = true;
          httpCookie.Secure = true;
        }
      }
    }

    private bool IsFedAuthCookie(string cookieName)
    {
      if (cookieName == "FedAuth")
        return true;
      return cookieName.StartsWith("FedAuth") && int.TryParse(cookieName.Substring("FedAuth".Length), out int _);
    }

    private void TraceClaimsFromClaimsIdentity(
      IVssRequestContext requestContext,
      int tracePoint,
      TraceLevel traceLevel,
      ClaimsIdentity claimsIdentity)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (Claim claim in claimsIdentity.Claims)
        stringBuilder.AppendLine(string.Format("Key: {0} | Value: {1}", (object) claim.Type, (object) claim.Value));
      requestContext.Trace(tracePoint, traceLevel, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", stringBuilder.ToString());
    }

    private void LoadSettings(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.InstanceId != this.m_ServiceHostId)
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.m_fedAuthEnabled = service.GetValue<bool>(requestContext, (RegistryQuery) FederatedAuthRegistryConstants.Enabled, true, false);
      IVssRequestContext requestContext1 = requestContext.IsVirtualServiceHost() ? requestContext.To(TeamFoundationHostType.Parent) : requestContext;
      RegistryQuery registryQuery;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS || requestContext1.IsFeatureEnabled("Microsoft.VisualStudio.Services.Authentication.DontUseLocationServiceForRealm"))
      {
        IVssRegistryService registryService = service;
        IVssRequestContext requestContext2 = requestContext;
        registryQuery = (RegistryQuery) "/Configuration/SharedService/Realm";
        ref RegistryQuery local = ref registryQuery;
        this.m_signinRealm = registryService.GetValue<string>(requestContext2, in local, true);
      }
      else
        this.m_signinRealm = TeamFoundationAuthenticationService.GetSpsDeploymentUrl(requestContext);
      if (!this.m_fedAuthEnabled)
        return;
      List<string> signingKeys = new List<string>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.GetServiceIdentitySigningKeysFromStrongBox(vssRequestContext, ref signingKeys);
      IVssRegistryService registryService1 = service;
      IVssRequestContext requestContext3 = requestContext;
      registryQuery = (RegistryQuery) (FederatedAuthRegistryConstants.WSFederation + "/**");
      ref RegistryQuery local1 = ref registryQuery;
      RegistryEntryCollection settings = registryService1.ReadEntriesFallThru(requestContext3, in local1);
      if (signingKeys.Count == 0)
        signingKeys = new List<string>()
        {
          settings["ServiceIdentitySigningKey"].GetValue<string>((string) null)
        };
      this.m_federatedAuthenticationSettings = new FederatedAuthenticationSettings(settings, (IEnumerable<string>) signingKeys);
      IVssRegistryService registryService2 = service;
      IVssRequestContext requestContext4 = requestContext;
      registryQuery = (RegistryQuery) (FederatedAuthRegistryConstants.CookieHandler + "/**");
      ref RegistryQuery local2 = ref registryQuery;
      this.m_cookieHandlerSettings = new CookieHandlerSettings(registryService2.ReadEntriesFallThru(requestContext4, in local2));
      if (string.IsNullOrEmpty(this.m_cookieHandlerSettings.DefaultDomain))
      {
        Uri uri = new Uri(vssRequestContext.GetService<ILocationService>().GetPublicAccessMapping(vssRequestContext).AccessPoint);
        if (uri.Host.Contains("."))
          this.m_cookieHandlerSettings.DefaultDomain = "." + uri.Host;
      }
      if (this.m_signinRealm != null || string.IsNullOrEmpty(this.m_federatedAuthenticationSettings.DefaultRealm))
        return;
      this.m_signinRealm = this.m_federatedAuthenticationSettings.DefaultRealm;
    }

    private bool GetServiceIdentitySigningKeysFromStrongBox(
      IVssRequestContext requestContext,
      ref List<string> signingKeys)
    {
      if (requestContext.ServiceInstanceType() != ServiceInstanceTypes.TFS)
        return false;
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(requestContext, FederatedAuthRegistryConstants.SigningCertDrawName, false);
      if (drawerId == Guid.Empty)
        return false;
      try
      {
        string str1 = service.GetString(requestContext, drawerId, FederatedAuthRegistryConstants.ServiceIdentitySigningKeyLookupKey);
        char[] separator = new char[1]{ ';' };
        foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
          signingKeys.Add(str2);
      }
      catch (StrongBoxItemNotFoundException ex)
      {
      }
      return signingKeys.Count != 0;
    }

    private string GetRedirectLocation(
      IVssRequestContext requestContext,
      string path,
      string query,
      Uri replyTo,
      SwitchHintParameter switchHintParameter,
      bool isIDE)
    {
      CultureInfo currentUiCulture1 = Thread.CurrentThread.CurrentUICulture;
      RequestLanguage.Apply(requestContext);
      try
      {
        requestContext = requestContext.Elevate();
        ILocationService service = requestContext.GetService<ILocationService>();
        AccessMapping accessMapping = service.DetermineAccessMapping(requestContext);
        if (replyTo == (Uri) null)
          replyTo = new Uri(service.LocationForAccessMapping(requestContext, requestContext.RelativeUrl(), RelativeToSetting.Context, accessMapping));
        SecureFlowLocation location;
        int num = (int) SecureFlowLocation.TryCreate(requestContext, replyTo, SecureFlowLocation.GetDefaultLocation(requestContext), out location);
        if (location != null)
        {
          while (location.NextLocation != null && location.Path.Equals("_signedin", StringComparison.OrdinalIgnoreCase))
            location = location.NextLocation;
          replyTo = new Uri(location.ToString());
        }
        if (string.IsNullOrWhiteSpace(query))
          query = string.Empty;
        if (query.Length > 0 && !query.StartsWith("&"))
          query = "&" + query;
        CultureInfo currentUiCulture2 = Thread.CurrentThread.CurrentUICulture;
        if (currentUiCulture2 != currentUiCulture1)
          query = query + "&mkt=" + currentUiCulture2.Name;
        if (switchHintParameter != null && !string.IsNullOrEmpty(switchHintParameter.SwitchType))
          query = query + "&switch_hint=" + switchHintParameter.ToString();
        return this.LocationForRealm(requestContext, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}?realm={1}&reply_to={2}{3}", (object) path, (object) replyTo.Host, (object) Uri.EscapeDataString(replyTo.AbsoluteUri), (object) query));
      }
      finally
      {
        RequestLanguage.Revert();
      }
    }

    private static bool IsFedAuthRedirectSuppressed(HttpRequestBase request) => request.Headers["X-TFS-FedAuthRedirect"] == "Suppress" || request.Headers["X-TFS-FedAuthRedirect"] == "Suppress,Suppress";

    private static string GetContextKey(string name) => typeof (TeamFoundationAuthenticationService).Name + (object) '.' + name;

    private void DetermineRealmAndDomain(
      IVssRequestContext requestContext,
      out string realm,
      out string cookieDomain)
    {
      requestContext.TraceEnter(1010100, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", nameof (DetermineRealmAndDomain));
      AccessMapping accessMapping = requestContext.GetService<ILocationService>().DetermineAccessMapping(requestContext);
      requestContext.Trace(1010101, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "DetermineAccessMapping returned: {0} {1}", (object) accessMapping.Moniker, (object) accessMapping.AccessPoint);
      if (!this.m_federatedAuthenticationSettings.Realms.TryGetValue(accessMapping.Moniker, out realm))
      {
        realm = this.m_federatedAuthenticationSettings.DefaultRealm;
        requestContext.Trace(1010102, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "Couldn't find an alternate realm for moniker {0}. Defaulting realm to {1}.", (object) accessMapping.Moniker, (object) this.m_federatedAuthenticationSettings.DefaultRealm);
      }
      if (!this.m_cookieHandlerSettings.Domains.TryGetValue(accessMapping.Moniker, out cookieDomain))
      {
        cookieDomain = this.m_cookieHandlerSettings.DefaultDomain;
        requestContext.Trace(1010103, TraceLevel.Info, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", "Couldn't find an alternate cookie domain for moniker {0}. Defaulting cookie domain to {1}.", (object) accessMapping.Moniker, (object) this.m_cookieHandlerSettings.DefaultDomain);
      }
      requestContext.RootContext.Items[RequestContextItemsKeys.Realm] = (object) realm;
      requestContext.TraceLeave(1010110, nameof (TeamFoundationAuthenticationService), "IVssFrameworkService", nameof (DetermineRealmAndDomain));
    }

    internal void AddTenantInfoResponseHeader(
      IVssRequestContext requestContext,
      HttpResponseBase response)
    {
      Guid guid = Guid.Empty;
      if (response.Headers["X-VSS-ResourceTenant"] != null)
        return;
      try
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          return;
        guid = requestContext.GetOrganizationAadTenantId();
      }
      finally
      {
        response.AddHeader("X-VSS-ResourceTenant", guid.ToString());
        if (guid != Guid.Empty)
        {
          string str = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Service/Aad/AuthAuthority", false, (string) null);
          response.AddHeader("WWW-Authenticate", string.Format("Bearer authorization_uri=https://{0}/{1}", (object) str, (object) guid));
        }
      }
    }

    public void AddTenantInfoResponseHeader(
      IVssRequestContext requestContext,
      HttpResponseMessage response)
    {
      Guid guid = Guid.Empty;
      if (response.Headers.Contains("X-VSS-ResourceTenant"))
        return;
      try
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          return;
        guid = requestContext.GetOrganizationAadTenantId();
      }
      finally
      {
        response.Headers.TryAddWithoutValidation("X-VSS-ResourceTenant", guid.ToString());
        if (guid != Guid.Empty)
        {
          string str = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Service/Aad/AuthAuthority", false, (string) null);
          response.Headers.TryAddWithoutValidation("WWW-Authenticate", string.Format("Bearer authorization_uri=https://{0}/{1}", (object) str, (object) guid));
        }
      }
    }

    internal static string GetSpsDeploymentUrl(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        ILocationService service = vssRequestContext.GetService<ILocationService>();
        return (service.GetAccessMapping(vssRequestContext, AccessMappingConstants.PublicAccessMappingMoniker) ?? service.GetAccessMapping(vssRequestContext, AccessMappingConstants.HostGuidAccessMappingMoniker))?.AccessPoint;
      }
      ServiceDefinition serviceDefinition = requestContext.GetService<ILocationService>().FindServiceDefinition(requestContext, "LocationService2", ServiceInstanceTypes.SPS);
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        serviceDefinition = vssRequestContext.GetService<ILocationService>().FindServiceDefinition(vssRequestContext, serviceDefinition.ParentServiceType, serviceDefinition.ParentIdentifier);
      }
      return (serviceDefinition.GetLocationMapping(AccessMappingConstants.PublicAccessMappingMoniker) ?? serviceDefinition.GetLocationMapping(AccessMappingConstants.HostGuidAccessMappingMoniker))?.Location;
    }
  }
}
