// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Presentation.SignoutController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Compliance;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Presentation
{
  [SupportedRouteArea(NavigationContextLevels.All)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class SignoutController : TfsController
  {
    private const string c_lastUserCookie = "TSWA-Last-User";
    private const string SignoutControllerEnablePostRedirectSignOutPath = "Identity.SignoutController.EnablePostRedirectSignOut";
    private static readonly IConfigPrototype<bool> SignoutControllerEnablePostRedirectSignOutPrototype = ConfigPrototype.Create<bool>("Identity.SignoutController.EnablePostRedirectSignOut", false);
    private readonly IConfigQueryable<bool> SignoutControllerEnablePostRedirectSignOutConfig;
    public const string s_area = "Signout";
    public const string s_layer = "Tfs SignoutController";

    public SignoutController()
      : this(ConfigProxy.Create<bool>(SignoutController.SignoutControllerEnablePostRedirectSignOutPrototype))
    {
    }

    public SignoutController(
      IConfigQueryable<bool> signoutControllerEnablePostRedirectSignOutConfig)
    {
      this.SignoutControllerEnablePostRedirectSignOutConfig = signoutControllerEnablePostRedirectSignOutConfig;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(550000, 550010)]
    public ActionResult Index(string mode, string redirectUrl, bool forceSignout = false)
    {
      LogoutMode logoutMode = string.IsNullOrEmpty(mode) ? LogoutMode.SignOut : mode.ParseEnumDefault<LogoutMode>(LogoutMode.SignOut);
      string urlText = this.Url.RouteUrl("ServiceHost", "index", "home", (object) new
      {
        routeArea = "",
        serviceHost = this.TfsRequestContext.ServiceHost.DeploymentServiceHost
      });
      if (string.IsNullOrEmpty(redirectUrl) && !this.TfsWebContext.IsHosted)
        redirectUrl = urlText;
      this.ViewData["HomeUrl"] = (object) redirectUrl;
      this.ViewData["LogOutMode"] = (object) logoutMode;
      this.ViewData["SignOutPostRedirectEnabled"] = (object) false;
      switch (logoutMode)
      {
        case LogoutMode.SignOut:
          if (this.TfsWebContext.IsHosted)
          {
            if (!string.IsNullOrEmpty(redirectUrl))
            {
              Uri result;
              if (Uri.TryCreate(redirectUrl, UriKind.Relative, out result))
              {
                ILocationService service = this.TfsRequestContext.GetService<ILocationService>();
                string str = this.TfsRequestContext.VirtualPath();
                if (str.Length > 1 && redirectUrl.StartsWith(str, StringComparison.OrdinalIgnoreCase))
                  redirectUrl = redirectUrl.Substring(str.Length - 1);
                redirectUrl = service.LocationForAccessMapping(this.TfsRequestContext, redirectUrl, RelativeToSetting.Context, service.DetermineAccessMapping(this.TfsRequestContext));
              }
              result = new Uri(redirectUrl);
            }
            IdentityService service1 = this.TfsRequestContext.GetService<IdentityService>();
            string str1 = (string) null;
            if (this.TfsRequestContext.GetUserIdentity() != null)
              str1 = service1.GetSignoutToken(this.TfsRequestContext);
            else
              this.TfsRequestContext.Trace(10002012, TraceLevel.Info, "Signout", "Tfs SignoutController", "Anonymous signout request. Unable to retrieve token.");
            ITeamFoundationAuthenticationService service2 = this.TfsRequestContext.GetService<ITeamFoundationAuthenticationService>();
            string url;
            if (str1 != null)
            {
              if (this.SignoutControllerEnablePostRedirectSignOutConfig.QueryByCtx<bool>(this.TfsRequestContext))
              {
                this.ViewData["SignOutPostRedirectEnabled"] = (object) true;
                this.ViewData["Mode"] = (object) mode;
                this.ViewData["RedirectUrl"] = (object) redirectUrl;
                this.ViewData["ForceSignout"] = (object) forceSignout;
                this.ViewData["Id_token"] = (object) str1;
                string relativePath = this.Url.RouteUrl("ServiceHostControllerAction", "index", "signout", (object) new
                {
                  routeArea = "",
                  serviceHost = this.TfsRequestContext.ServiceHost.DeploymentServiceHost
                });
                this.ViewData["LocationForRealm"] = (object) service2.LocationForRealm(this.TfsRequestContext, relativePath);
                IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
                this.ViewData["Nonce"] = (object) vssRequestContext.GetService<IContentSecurityPolicyNonceManagementService>().GetNonceValue(vssRequestContext, HttpContextFactory.Current);
                service2.SignOutFromSessionModule(this.TfsRequestContext);
                return (ActionResult) this.View();
              }
              url = service2.LocationForRealm(this.TfsRequestContext, this.Url.RouteUrl("ServiceHostControllerAction", "index", "signout", (object) new
              {
                routeArea = "",
                serviceHost = this.TfsRequestContext.ServiceHost.DeploymentServiceHost,
                mode = mode,
                redirectUrl = redirectUrl,
                forceSignout = forceSignout,
                id_token = str1
              }));
            }
            else
              url = service2.LocationForRealm(this.TfsRequestContext, this.Url.RouteUrl("ServiceHostControllerAction", "index", "signout", (object) new
              {
                routeArea = "",
                serviceHost = this.TfsRequestContext.ServiceHost.DeploymentServiceHost,
                mode = mode,
                redirectUrl = redirectUrl,
                forceSignout = forceSignout
              }));
            service2.SignOutFromSessionModule(this.TfsRequestContext);
            return (ActionResult) this.Redirect(url);
          }
          this.ViewData["HomeUrl"] = (object) this.Url.RouteUrl("ServiceHost", "index", "home", (object) new
          {
            routeArea = "",
            serviceHost = this.TfsRequestContext.ServiceHost.DeploymentServiceHost
          });
          this.ViewData["SignInAsDifferentUserUrl"] = (object) this.Url.RouteUrl("ServiceHostControllerAction", "index", "signout", (object) new
          {
            routeArea = "",
            serviceHost = this.TfsRequestContext.ServiceHost.DeploymentServiceHost,
            redirectUrl = redirectUrl,
            mode = "SignInAsDifferentUser"
          });
          break;
        case LogoutMode.ChangeUser:
          HttpCookie cookie1 = this.Request.Cookies["TSWA-Last-User"];
          if (this.Request.IsAuthenticated && cookie1 != null)
          {
            HttpCookie cookie2 = new HttpCookie("TSWA-Last-User", string.Empty);
            cookie2.Expires = DateTime.Now.AddYears(-5);
            cookie2.Secure = this.TfsRequestContext.ExecutionEnvironment.IsSslOnly;
            cookie2.HttpOnly = true;
            Uri result;
            if (Uri.TryCreate(redirectUrl, UriKind.Absolute, out result))
            {
              if (this.TfsWebContext.IsHosted)
              {
                SecureFlowLocation location1;
                int num1 = (int) SecureFlowLocation.TryCreate(this.TfsRequestContext, urlText, SecureFlowLocation.GetDefaultLocation(this.TfsRequestContext), out location1);
                SecureFlowLocation location2;
                int num2 = (int) SecureFlowLocation.TryCreate(this.TfsRequestContext, redirectUrl, location1, out location2);
                redirectUrl = location2.ToString();
              }
              else
              {
                bool flag = false;
                IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
                foreach (AccessMapping accessMapping in vssRequestContext.GetService<ILocationService>().GetAccessMappings(vssRequestContext))
                {
                  if (new Uri(accessMapping.AccessPoint).Host == result.Host)
                  {
                    flag = true;
                    break;
                  }
                }
                if (!flag)
                  redirectUrl = urlText;
              }
            }
            else if (!Uri.TryCreate(redirectUrl, UriKind.Relative, out Uri _))
              redirectUrl = urlText;
            CookieModifier.AddSameSiteNoneToCookie(this.TfsRequestContext, cookie2);
            this.Response.Cookies.Set(cookie2);
            return (ActionResult) this.Redirect(redirectUrl);
          }
          string empty = string.Empty;
          if (this.Request.IsAuthenticated)
            empty = this.TfsRequestContext.GetUserId().ToString();
          HttpCookie cookie3 = new HttpCookie("TSWA-Last-User", empty);
          cookie3.Secure = this.TfsRequestContext.ExecutionEnvironment.IsSslOnly;
          cookie3.HttpOnly = true;
          CookieModifier.AddSameSiteNoneToCookie(this.TfsRequestContext, cookie3);
          this.Response.Cookies.Set(cookie3);
          this.Response.AppendHeader("Connection", "close");
          this.Response.StatusCode = 401;
          this.Response.Clear();
          this.Response.Write(WACommonResources.NotAuthorizedToAccessPage);
          this.Response.End();
          return (ActionResult) new EmptyResult();
        case LogoutMode.CloseConnection:
          this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
          this.Response.AppendHeader("Connection", "close");
          this.Response.StatusCode = 200;
          this.Response.Clear();
          this.Response.End();
          return (ActionResult) new EmptyResult();
      }
      return (ActionResult) this.View();
    }
  }
}
