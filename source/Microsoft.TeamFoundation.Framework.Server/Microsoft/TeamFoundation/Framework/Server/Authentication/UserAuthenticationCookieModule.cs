// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.UserAuthenticationCookieModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.HostAuthentication;
using System;
using System.Security.Principal;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  internal class UserAuthenticationCookieModule : IHttpModule
  {
    private readonly IAadAuthenticationSessionTokenProvider aadAuthenticationSessionTokenProvider;
    internal static readonly byte[] SignOutImage = new byte[143]
    {
      (byte) 71,
      (byte) 73,
      (byte) 70,
      (byte) 56,
      (byte) 57,
      (byte) 97,
      (byte) 17,
      (byte) 0,
      (byte) 13,
      (byte) 0,
      (byte) 162,
      (byte) 0,
      (byte) 0,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      (byte) 169,
      (byte) 240,
      (byte) 169,
      (byte) 125,
      (byte) 232,
      (byte) 125,
      (byte) 82,
      (byte) 224,
      (byte) 82,
      (byte) 38,
      (byte) 216,
      (byte) 38,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 33,
      (byte) 249,
      (byte) 4,
      (byte) 5,
      (byte) 0,
      (byte) 0,
      (byte) 5,
      (byte) 0,
      (byte) 44,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 17,
      (byte) 0,
      (byte) 13,
      (byte) 0,
      (byte) 0,
      (byte) 8,
      (byte) 84,
      (byte) 0,
      (byte) 11,
      (byte) 8,
      (byte) 28,
      (byte) 72,
      (byte) 112,
      (byte) 32,
      (byte) 128,
      (byte) 131,
      (byte) 5,
      (byte) 19,
      (byte) 22,
      (byte) 56,
      (byte) 24,
      (byte) 128,
      (byte) 64,
      (byte) 0,
      (byte) 0,
      (byte) 10,
      (byte) 13,
      (byte) 54,
      (byte) 116,
      (byte) 8,
      (byte) 49,
      (byte) 226,
      (byte) 193,
      (byte) 1,
      (byte) 4,
      (byte) 6,
      (byte) 32,
      (byte) 36,
      (byte) 88,
      (byte) 113,
      (byte) 97,
      (byte) 0,
      (byte) 140,
      (byte) 26,
      (byte) 11,
      (byte) 30,
      (byte) 68,
      (byte) 8,
      (byte) 64,
      (byte) 0,
      (byte) 129,
      (byte) 140,
      (byte) 29,
      (byte) 5,
      (byte) 2,
      (byte) 56,
      (byte) 73,
      (byte) 209,
      (byte) 36,
      (byte) 202,
      (byte) 132,
      (byte) 37,
      (byte) 79,
      (byte) 14,
      (byte) 112,
      (byte) 73,
      (byte) 81,
      (byte) 97,
      (byte) 76,
      (byte) 150,
      (byte) 53,
      (byte) 109,
      (byte) 210,
      (byte) 36,
      (byte) 32,
      (byte) 32,
      (byte) 37,
      (byte) 76,
      (byte) 151,
      (byte) 33,
      (byte) 35,
      (byte) 26,
      (byte) 20,
      (byte) 16,
      (byte) 84,
      (byte) 168,
      (byte) 65,
      (byte) 159,
      (byte) 9,
      (byte) 3,
      (byte) 2,
      (byte) 0,
      (byte) 59
    };
    private static readonly string s_Area = nameof (UserAuthenticationCookieModule);
    private static readonly string s_Layer = "IHttpModule";

    public UserAuthenticationCookieModule()
      : this((IAadAuthenticationSessionTokenProvider) new AadAuthenticationSessionTokenProvider())
    {
    }

    internal UserAuthenticationCookieModule(
      IAadAuthenticationSessionTokenProvider aadAuthenticationSessionTokenProvider)
    {
      this.aadAuthenticationSessionTokenProvider = aadAuthenticationSessionTokenProvider ?? (IAadAuthenticationSessionTokenProvider) new AadAuthenticationSessionTokenProvider();
    }

    public void Init(HttpApplication context) => context.AuthenticateRequest += new EventHandler(this.OnAuthenticateRequest);

    public void Dispose()
    {
    }

    private void OnAuthenticateRequest(object sender, EventArgs eventArgs)
    {
      HttpApplication httpApplication = (HttpApplication) sender;
      HttpContextWrapper httpContextWrapper = new HttpContextWrapper(httpApplication.Context);
      if (httpContextWrapper.User != null && httpContextWrapper.User.Identity.IsAuthenticated || !(httpContextWrapper.Items[(object) HttpContextConstants.IVssRequestContext] is IVssRequestContext vssRequestContext))
        return;
      vssRequestContext.TraceEnter(715002, UserAuthenticationCookieModule.s_Area, UserAuthenticationCookieModule.s_Layer, "UserAuthenticationCookieModule.OnAuthenticateRequest");
      if (httpContextWrapper.Items.Contains((object) HttpContextConstants.ArrRequestRouted) || httpContextWrapper.Items.Contains((object) HttpContextConstants.IsStaticContentRequest))
        return;
      if (AuthenticationHelpers.IsSignOutRequest(httpApplication.Context.Request))
      {
        UserAuthenticationSessionTokenHandler.DeleteSessionToken(vssRequestContext, (HttpContextBase) httpContextWrapper);
        FederatedAuthCookieHelper.DeleteSessionTokenCookie(vssRequestContext);
        try
        {
          this.aadAuthenticationSessionTokenProvider.DeleteSessionToken(vssRequestContext, (HttpContextBase) httpContextWrapper);
        }
        catch (Exception ex)
        {
          vssRequestContext.TraceException(734924, UserAuthenticationCookieModule.s_Area, UserAuthenticationCookieModule.s_Layer, ex);
        }
        if (vssRequestContext.IsDevOpsDomainRequestUsingRootContext())
          HostAuthenticationCookie.DeleteHostAuthenticationCookie(vssRequestContext);
        HttpResponseBase response = httpContextWrapper.Response;
        response.Cache.SetCacheability(HttpCacheability.NoCache);
        response.ContentType = "image/gif";
        response.ClearContent();
        response.BinaryWrite(UserAuthenticationCookieModule.SignOutImage);
        httpApplication.CompleteRequest();
      }
      else
      {
        try
        {
          UserAuthenticationSessionToken authenticationSessionToken = UserAuthenticationSessionTokenHandler.ReadSessionToken(vssRequestContext, (HttpContextBase) httpContextWrapper);
          try
          {
            UserAuthenticationSessionToken aadAccessToken;
            if (this.aadAuthenticationSessionTokenProvider.TryGetAadAuthenticationSessionCookie(vssRequestContext, (HttpContextBase) httpContextWrapper, out aadAccessToken))
            {
              if (aadAccessToken != null)
              {
                httpContextWrapper.User = (IPrincipal) aadAccessToken.User;
                OAuth2AuthCredential credential = new OAuth2AuthCredential(aadAccessToken.User, aadAccessToken.SecurityToken);
                vssRequestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal().SetAuthenticationCredential((IAuthCredential) credential);
                AuthenticationHelpers.SetAuthenticationMechanism(vssRequestContext, aadAccessToken.AuthenticationMechanism);
                return;
              }
            }
          }
          catch (Exception ex)
          {
            vssRequestContext.TraceException(734925, UserAuthenticationCookieModule.s_Area, UserAuthenticationCookieModule.s_Layer, ex);
          }
          if (authenticationSessionToken == null)
            return;
          httpContextWrapper.User = (IPrincipal) authenticationSessionToken.User;
          OAuth2AuthCredential credential1 = new OAuth2AuthCredential(authenticationSessionToken.User, authenticationSessionToken.SecurityToken);
          vssRequestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal().SetAuthenticationCredential((IAuthCredential) credential1);
          AuthenticationHelpers.SetAuthenticationMechanism(vssRequestContext, authenticationSessionToken.AuthenticationMechanism);
        }
        catch (Exception ex)
        {
          vssRequestContext.TraceException(734924, UserAuthenticationCookieModule.s_Area, UserAuthenticationCookieModule.s_Layer, ex);
        }
        finally
        {
          vssRequestContext.TraceLeave(857465, UserAuthenticationCookieModule.s_Area, UserAuthenticationCookieModule.s_Layer, "UserAuthenticationCookieModule.OnAuthenticateRequest");
        }
      }
    }
  }
}
