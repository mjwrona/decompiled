// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminServicesController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.ConnectedService.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Admin", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  [DemandFeature("65AC9DB3-BB0A-42fe-B584-A690FB0D817B", true)]
  public class AdminServicesController : AdminAreaController
  {
    public AdminServicesController() => this.m_executeContributedRequestHandlers = true;

    [HttpGet]
    public ActionResult Index()
    {
      this.CheckManageServicesPermission();
      return (ActionResult) this.View();
    }

    [HttpGet]
    public ActionResult Callback(string code, string state)
    {
      string providerId = (string) null;
      if (this.Request.RequestContext.RouteData.Values.ContainsKey("parameters"))
        providerId = this.Request.RequestContext.RouteData.Values["parameters"] as string;
      if (providerId != null)
      {
        ConnectedServiceProvider provider = this.TfsRequestContext.GetService<ConnectedServiceProviderService>().GetProvider(this.TfsRequestContext, providerId);
        if (provider != null)
        {
          bool redirect;
          string str = provider.CompleteCallback(this.TfsRequestContext, code, state, this.CreateHtmlHelper().GenerateNonce(), out redirect);
          return redirect ? (ActionResult) this.Redirect(str) : (ActionResult) this.Content(str);
        }
      }
      return (ActionResult) this.View("Error");
    }

    [HttpGet]
    public ActionResult CompleteCallback(
      [ClientQueryParameter] string accessTokenKey,
      [ClientQueryParameter] string projectId = null,
      [ClientQueryParameter] string endpointId = null)
    {
      string format1 = "vstsaadoauthcompleted=\"true\";vstsaadoauthaccesstokenkey=\"{0}\"";
      string format2 = "vstsaadoauthcompleted=\"true\";vstsaadoautherrormessage=\"{0}\"";
      if (string.IsNullOrEmpty(accessTokenKey) || !Guid.TryParse(accessTokenKey, out Guid _))
        return (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format2, (object) AdminServerResources.InvalidVstsAccessTokenKey));
      if (string.IsNullOrWhiteSpace(endpointId))
        return (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format1, (object) accessTokenKey));
      if (string.IsNullOrWhiteSpace(projectId))
        return (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format2, (object) AdminServerResources.InvalidVstsProjectId));
      string errorMessage;
      this.TfsRequestContext.GetService<IServiceEndpointService2>().UpdateEndpointAccessToken(this.TfsRequestContext, new Guid(projectId), new Guid(endpointId), accessTokenKey, out errorMessage);
      return !string.IsNullOrWhiteSpace(errorMessage) ? (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format2, (object) errorMessage)) : (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format1, (object) accessTokenKey));
    }

    [HttpPost]
    [TfsBypassAntiForgeryValidation]
    public ActionResult CompleteCallbackByAuthCode(string code, string state, string id_token)
    {
      string format1 = "vstsaadoauthcompleted=\"true\";vstsaadoauthaccesstokenkey=\"{0}\"";
      string format2 = "vstsaadoauthcompleted=\"true\";vstsaadoautherrormessage=\"{0}\"";
      if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(id_token))
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, AdminServerResources.InvalidParameter, string.IsNullOrEmpty(state) ? (object) nameof (state) : (string.IsNullOrEmpty(code) ? (object) nameof (code) : (object) nameof (id_token)));
        this.Trace(100142000, TraceLevel.Error, message);
        return (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format2, (object) message));
      }
      NameValueCollection queryString = UriUtility.ParseQueryString(state);
      string errorMessage;
      JwtSecurityToken token = this.AcquireAadAccessToken(queryString, code, id_token, out errorMessage);
      if (token == null)
      {
        this.Trace(100142001, TraceLevel.Error, errorMessage);
        return (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format2, (object) errorMessage));
      }
      string str = this.SecureStateInStrongBox(queryString, token, id_token, out errorMessage);
      if (str != null)
        return (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format1, (object) str));
      this.Trace(100142002, TraceLevel.Error, errorMessage);
      return (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format2, (object) errorMessage));
    }

    private JwtSecurityToken AcquireAadAccessToken(
      NameValueCollection stateNameValuePairs,
      string authCode,
      string idToken,
      out string errorMessage)
    {
      errorMessage = string.Empty;
      string stateNameValuePair1 = stateNameValuePairs["ReplyToUri"];
      string stateNameValuePair2 = stateNameValuePairs["TenantId"];
      string stateNameValuePair3 = stateNameValuePairs["Resource"];
      if (string.IsNullOrEmpty(stateNameValuePair1) || string.IsNullOrEmpty(stateNameValuePair2) || string.IsNullOrEmpty(stateNameValuePair3))
      {
        string str = string.IsNullOrEmpty(stateNameValuePair1) ? "replyToUri" : (string.IsNullOrEmpty(stateNameValuePair2) ? "tenantId" : "resource");
        errorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, AdminServerResources.InvalidParameter, (object) str);
        return (JwtSecurityToken) null;
      }
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment).Elevate();
      bool validIdentity;
      ClaimsPrincipal claimsPrincipal = vssRequestContext.GetService<IOAuth2AuthenticationService>().ValidateToken(vssRequestContext, idToken, OAuth2TokenValidators.AAD | OAuth2TokenValidators.DelegatedAuth, out JwtSecurityToken _, out bool _, out validIdentity);
      if (!validIdentity)
      {
        errorMessage = AdminServerResources.LoginIdentityInvalid;
        return (JwtSecurityToken) null;
      }
      Microsoft.VisualStudio.Services.Identity.Identity identityDescriptor = vssRequestContext.GetService<IIdentityAuthenticationService>().ResolveIdentity(vssRequestContext, claimsPrincipal.Identity);
      if (identityDescriptor == null)
      {
        errorMessage = AdminServerResources.UnableToResolveIdentity;
        return (JwtSecurityToken) null;
      }
      JwtSecurityToken jwtSecurityToken = vssRequestContext.GetService<IAadTokenService>().AcquireTokenByAuthorizationCode(this.TfsRequestContext.Elevate(), authCode, stateNameValuePair3, stateNameValuePair2, new Uri(stateNameValuePair1), identityDescriptor);
      if (jwtSecurityToken != null)
        return jwtSecurityToken;
      errorMessage = AdminServerResources.FailedToAcquireAccessToken;
      return (JwtSecurityToken) null;
    }

    private string SecureStateInStrongBox(
      NameValueCollection state,
      JwtSecurityToken token,
      string idToken,
      out string errorMessage)
    {
      errorMessage = string.Empty;
      Dictionary<string, string> toSerialize = new Dictionary<string, string>();
      toSerialize["AccessToken"] = token.RawData;
      toSerialize["IdToken"] = idToken;
      string str1 = state["nonce"];
      if (!string.IsNullOrWhiteSpace(str1))
        toSerialize["nonce"] = str1;
      string str2 = state["CompleteCallbackPayload"];
      if (!string.IsNullOrWhiteSpace(str2))
        toSerialize["CompleteCallbackPayload"] = str2;
      string str3 = JsonUtility.ToString((object) toSerialize);
      try
      {
        ITeamFoundationStrongBoxService service = this.TfsRequestContext.GetService<ITeamFoundationStrongBoxService>();
        IVssRequestContext requestContext = this.TfsRequestContext.Elevate();
        Guid drawerId = service.UnlockDrawer(requestContext, "AadOauthCallbackDrawer", false);
        if (drawerId == Guid.Empty)
          drawerId = service.CreateDrawer(requestContext, "AadOauthCallbackDrawer");
        string lookupKey = Guid.NewGuid().ToString();
        service.AddString(requestContext, drawerId, lookupKey, str3);
        return lookupKey;
      }
      catch (Exception ex)
      {
        errorMessage = ex.Message;
        return (string) null;
      }
    }

    private ContentResult CreateJavaScript(string javaScript) => this.Content("<script type=\"text/javascript\"" + this.CreateHtmlHelper().GenerateNonce(true) + "> " + javaScript + "</script>");

    private HtmlHelper CreateHtmlHelper()
    {
      ViewContext viewContext = new ViewContext();
      viewContext.Controller = (ControllerBase) this;
      viewContext.RequestContext = this.Request.RequestContext;
      viewContext.HttpContext = this.HttpContext;
      return new HtmlHelper(viewContext, (IViewDataContainer) new ViewPage());
    }
  }
}
