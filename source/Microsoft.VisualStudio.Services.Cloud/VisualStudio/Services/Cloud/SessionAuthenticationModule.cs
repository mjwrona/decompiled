// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SessionAuthenticationModule
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.HostAuthentication;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class SessionAuthenticationModule : System.IdentityModel.Services.SessionAuthenticationModule
  {
    protected override void InitializeModule(HttpApplication context)
    {
      base.InitializeModule(context);
      if (context == null)
        return;
      context.EndRequest += new EventHandler(this.OnEndRequest);
      context.Error += new EventHandler(this.OnError);
    }

    protected override void OnAuthenticateRequest(object sender, EventArgs eventArgs)
    {
      HttpApplication httpApplication = (HttpApplication) sender;
      HttpContext context = httpApplication.Context;
      if (!(context.Items[(object) "IVssRequestContext"] is IVssRequestContext vssRequestContext))
        return;
      bool flag1 = AuthenticationHelpers.IsSignedInRequest(context.Request);
      bool flag2 = context.User != null && context.User.Identity.IsAuthenticated;
      if (flag2 && !flag1)
        return;
      HttpCookie cookie = context.Request.Cookies["FedAuth"];
      if (cookie != null && cookie.Value == "UserAuthenticationToken")
        return;
      vssRequestContext.GetService<ITeamFoundationAuthenticationService>().ConfigureRequest(vssRequestContext);
      if (context.Items.Contains((object) HttpContextConstants.ArrRequestRouted) || context.Items.Contains((object) HttpContextConstants.IsStaticContentRequest))
        return;
      if (AuthenticationHelpers.IsSignOutRequest(context.Request))
      {
        UserAuthenticationSessionTokenHandler.DeleteSessionToken(vssRequestContext, (HttpContextBase) new HttpContextWrapper(context));
        FederatedAuthCookieHelper.DeleteSessionTokenCookie(vssRequestContext);
        if (vssRequestContext.IsDevOpsDomainRequestUsingRootContext())
          HostAuthenticationCookie.DeleteHostAuthenticationCookie(vssRequestContext);
        HttpResponse response = context.Response;
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
          if (!vssRequestContext.ServiceHost.IsProduction && context.Request.Headers["X-VSS-SessionAuthentication-FaultInjection"] != null && vssRequestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.SessionAuthenticationFaultInjection"))
            throw new CryptographicException();
          if (AuthenticationHelpers.IsSignedInRequest(context.Request))
          {
            byte[] sessionCookie = this.ReadInternal(this.CookieHandler.Name, context.Request.Form);
            if (sessionCookie != null)
            {
              SessionSecurityToken sessionToken1 = this.ReadSessionTokenFromCookie(sessionCookie);
              if (sessionToken1 != null)
              {
                SessionSecurityToken token = sessionToken1;
                SessionSecurityTokenReceivedEventArgs args = new SessionSecurityTokenReceivedEventArgs(sessionToken1);
                this.OnSessionSecurityTokenReceived(args);
                if (args.Cancel)
                  return;
                SessionSecurityToken sessionToken2 = args.SessionToken;
                if (args.ReissueCookie)
                {
                  this.RemoveSessionTokenFromCache(token);
                  SessionSecurityTokenCreatedEventArgs createdEventArgs = this.RaiseSessionCreatedEvent(sessionToken2, true);
                  sessionToken2 = createdEventArgs.SessionToken;
                  int num = createdEventArgs.WriteSessionCookie ? 1 : 0;
                }
                if (flag2)
                {
                  if (vssRequestContext.IsIssueFedAuthTokenDisabled())
                    return;
                  this.WriteSessionTokenToCookie(sessionToken2);
                  return;
                }
                try
                {
                  this.AuthenticateSessionSecurityToken(sessionToken2, true);
                  this.MarkRequestContextIfAuthenticated();
                  return;
                }
                catch (FederatedAuthenticationSessionEndingException ex)
                {
                  this.SignOut();
                  return;
                }
              }
            }
          }
          if (flag2 || !(context.Request.QueryString["nosession"] != "1"))
            return;
          base.OnAuthenticateRequest(sender, eventArgs);
          this.MarkRequestContextIfAuthenticated();
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("ID6040:"))
        {
          this.TraceException(vssRequestContext, 570001, (Exception) ex);
        }
        catch (CryptographicException ex) when (ex.Message.Contains(FrameworkServerConstants.InvalidPadding))
        {
          this.TraceException(vssRequestContext, 570002, (Exception) ex);
          SessionAuthenticationPerfCounters.PaddingErrorsPerSec.Increment();
        }
        catch (XmlException ex) when (ex.Message.Contains(FrameworkServerConstants.UnexpectedEndOfFile) || ex.Message.Contains(FrameworkServerConstants.NotAValidBase64Sequence))
        {
          this.TraceException(vssRequestContext, 570002, (Exception) ex);
          throw new TeamFoundationInvalidTokenException(ex.Message);
        }
        catch (FormatException ex) when (ex.Message.Contains(FrameworkServerConstants.NotAValidBase64String))
        {
          this.TraceException(vssRequestContext, 570002, (Exception) ex);
          throw new TeamFoundationInvalidTokenException(ex.Message);
        }
        catch (Exception ex)
        {
          this.TraceException(vssRequestContext, 570003, ex);
          throw new TeamFoundationSessionAuthenticationException(ex.Message, ex);
        }
      }
    }

    protected virtual void OnEndRequest(object sender, EventArgs e)
    {
      HttpApplication httpApplication = (HttpApplication) sender;
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) httpApplication.Response.Cookies.AllKeys, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (stringSet.Contains("FedAuth") || stringSet.Contains("FedSignOut") || stringSet.Contains("MsalJsSignout"))
        httpApplication.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      if (httpApplication.Response.StatusCode != 401 || Thread.CurrentPrincipal.Identity.IsAuthenticated)
        return;
      if (!(httpApplication.Context.Items[(object) "IVssRequestContext"] is WebRequestContext requestContext))
        throw new UnauthorizedRequestException(FrameworkResources.InvalidArgumentsOnRedirectToIdentityProvider(), HttpStatusCode.Unauthorized);
      if ((requestContext.RequestRestrictions().MechanismsToAdvertise & AuthenticationMechanisms.FederatedRedirect) == AuthenticationMechanisms.None && (requestContext.RequestRestrictions().MechanismsToAdvertise & AuthenticationMechanisms.Federated) == AuthenticationMechanisms.None)
        return;
      bool force;
      requestContext.RootContext.Items.TryGetValue<bool>("$UserAuthenticationSkipped", out force);
      if (requestContext.Items.ContainsKey("SuppressAuthenticationRedirect"))
        requestContext.Items.Remove("SuppressAuthenticationRedirect");
      else
        this.RedirectToIdentityProvider((IVssRequestContext) requestContext, false, force);
    }

    protected virtual void OnError(object sender, EventArgs eventArgs)
    {
      HttpApplication httpApplication = (HttpApplication) sender;
      HttpContext context = httpApplication.Context;
      IVssRequestContext requestContext = (IVssRequestContext) context.Items[(object) "IVssRequestContext"];
      if (requestContext == null)
        return;
      Exception lastError = httpApplication.Server.GetLastError();
      if (lastError == null)
        return;
      if (requestContext.IsAnonymous())
      {
        switch (lastError)
        {
          case UnauthorizedRequestException _:
          case IdentityLoopingLoginException _:
            if (!requestContext.IsTracing(273910, TraceLevel.Info, nameof (SessionAuthenticationModule), nameof (OnError)))
              return;
            requestContext.TraceException(273910, TraceLevel.Info, nameof (SessionAuthenticationModule), nameof (OnError), lastError);
            return;
        }
      }
      requestContext.TraceException(273912, TraceLevel.Info, nameof (SessionAuthenticationModule), nameof (OnError), lastError);
      Exception exception;
      switch (lastError)
      {
        case SecurityTokenExpiredException _:
          if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.SessionAuthenticationUseLegacyRedirect"))
          {
            this.CompleteInvalidRequest(requestContext, context, true, false);
            return;
          }
          this.RedirectToIdentityProvider(requestContext, true, false);
          return;
        case CryptographicException _:
label_15:
          FederatedAuthCookieHelper.DeleteSessionTokenCookie(requestContext);
          if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.SessionAuthenticationUseLegacyRedirect"))
          {
            this.CompleteInvalidRequest(requestContext, context, true, false);
            return;
          }
          this.RedirectToIdentityProvider(requestContext, true, false);
          return;
        case null:
          exception = (Exception) null;
          break;
        default:
          // ISSUE: explicit non-virtual call
          exception = __nonvirtual (lastError.InnerException);
          break;
      }
      if (!(exception is CryptographicException) && (!(lastError is InvalidOperationException) || !lastError.Message.StartsWith("ID6040", StringComparison.Ordinal)) && (!(lastError is FormatException) || !lastError.Message.StartsWith("ID1007", StringComparison.Ordinal)))
      {
        if (!(lastError is TeamFoundationSessionAuthenticationException) && !(lastError is TeamFoundationInvalidTokenException))
          return;
        FederatedAuthCookieHelper.DeleteSessionTokenCookie(requestContext);
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.SessionAuthenticationUseLegacyRedirect"))
          this.CompleteInvalidRequest(requestContext, context, true, true);
        else
          this.RedirectToIdentityProvider(requestContext, true, true);
      }
      else
        goto label_15;
    }

    private void RedirectToIdentityProvider(
      IVssRequestContext requestContext,
      bool hasInvalidToken,
      bool force)
    {
      requestContext.GetService<ITeamFoundationAuthenticationService>().RedirectToIdentityProvider(requestContext, hasInvalidToken, force);
    }

    private void CompleteInvalidRequest(
      IVssRequestContext requestContext,
      HttpContext context,
      bool hasInvalidToken,
      bool force)
    {
      context.ClearError();
      ITeamFoundationAuthenticationService service1 = requestContext.GetService<ITeamFoundationAuthenticationService>();
      IInvalidRequestCompletionService service2 = requestContext.GetService<IInvalidRequestCompletionService>();
      string redirectLocation = service1.GetSignInRedirectLocation(requestContext, force);
      IVssRequestContext requestContext1 = requestContext;
      ITeamFoundationAuthenticationService authenticationService = service1;
      string authenticateErrorReason = hasInvalidToken ? "InvalidToken" : (string) null;
      string signinRedirectLocation = redirectLocation;
      service2.CompleteInvalidRequest(requestContext1, authenticationService, authenticateErrorReason, nameof (SessionAuthenticationModule), signinRedirectLocation: signinRedirectLocation);
    }

    private void MarkRequestContextIfAuthenticated()
    {
      HttpContextBase current = HttpContextFactory.Current;
      IVssRequestContext requestContext = (IVssRequestContext) current.Items[(object) "IVssRequestContext"];
      if (requestContext == null || current.User == null)
        return;
      AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.FedAuth);
      AuthenticationHelpers.SetCanIssueFedAuthToken(requestContext, true);
      requestContext.RootContext.Items["AuthenticationWithSessionAuth"] = (object) true;
    }

    private new void RemoveSessionTokenFromCache(SessionSecurityToken token)
    {
      ArgumentUtility.CheckForNull<SessionSecurityToken>(token, nameof (token));
      if (!(this.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers[typeof (SessionSecurityToken)] is SessionSecurityTokenHandler securityTokenHandler))
        throw new InvalidOperationException();
      securityTokenHandler.Configuration.Caches.SessionSecurityTokenCache.Remove(new SessionSecurityTokenCacheKey(token.EndpointId, token.ContextId, token.KeyGeneration));
    }

    private SessionSecurityTokenCreatedEventArgs RaiseSessionCreatedEvent(
      SessionSecurityToken sessionToken,
      bool reissueCookie)
    {
      SessionSecurityTokenCreatedEventArgs args = new SessionSecurityTokenCreatedEventArgs(sessionToken);
      args.WriteSessionCookie = reissueCookie;
      this.OnSessionSecurityTokenCreated(args);
      return args;
    }

    private byte[] ReadInternal(string name, NameValueCollection formValues)
    {
      StringBuilder stringBuilder = (StringBuilder) null;
      foreach (string tokenChunk in this.GetTokenChunks(name, formValues))
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder();
        stringBuilder.Append(tokenChunk);
      }
      if (stringBuilder == null)
        return (byte[]) null;
      try
      {
        return Convert.FromBase64String(stringBuilder.ToString());
      }
      catch (FormatException ex)
      {
        return (byte[]) null;
      }
    }

    private IEnumerable<string> GetTokenChunks(string baseName, NameValueCollection formValues)
    {
      int num = 0;
      string formValue;
      for (string chunkName = SessionAuthenticationModule.GetChunkName(baseName, num); (formValue = formValues[chunkName]) != null; chunkName = SessionAuthenticationModule.GetChunkName(baseName, ++num))
        yield return formValue;
    }

    private static string GetChunkName(string baseName, int chunkIndex) => chunkIndex != 0 ? baseName + chunkIndex.ToString((IFormatProvider) CultureInfo.InvariantCulture) : baseName;

    private void TraceException(IVssRequestContext requestContext, int tracepoint, Exception ex) => requestContext.TraceException(tracepoint, TraceLevel.Error, "Authentication", nameof (SessionAuthenticationModule), ex);
  }
}
