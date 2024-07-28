// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Authentication.SignedInProcessor
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Compliance;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Microsoft.VisualStudio.Services.Authentication
{
  internal class SignedInProcessor : ISignedInProcessor
  {
    private readonly IAadAuthenticationSessionTokenProvider aadAuthenticationSessionTokenProvider;
    private const string c_area = "Authentication";
    private const string c_layer = "SignedInProcessor";
    private const string RealmFormInputFormat = "<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />";
    private const string RealmFormInputDelimeter = "\r\n            ";
    private static readonly RequestRestrictions s_signedInRestrictions = new RequestRestrictions(RequiredAuthentication.Authenticated, AllowedHandler.All, "SignedInRequestRestrictions");

    public SignedInProcessor()
      : this((IAadAuthenticationSessionTokenProvider) new AadAuthenticationSessionTokenProvider())
    {
    }

    internal SignedInProcessor(
      IAadAuthenticationSessionTokenProvider aadAuthenticationSessionTokenProvider)
    {
      this.aadAuthenticationSessionTokenProvider = aadAuthenticationSessionTokenProvider ?? (IAadAuthenticationSessionTokenProvider) new AadAuthenticationSessionTokenProvider();
    }

    public bool IsSignedInRequest(IVssRequestContext requestContext, IHttpApplication application)
    {
      HttpRequestBase request = application.Context.Request;
      if (!request.Path.EndsWith("/_signedin", StringComparison.OrdinalIgnoreCase))
        return false;
      return StringComparer.OrdinalIgnoreCase.Equals(request.HttpMethod, "GET") || StringComparer.OrdinalIgnoreCase.Equals(request.HttpMethod, "POST");
    }

    public void HandleSignedInRequest(
      IVssRequestContext requestContext,
      IHttpApplication application)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.DelayedIdentityValidation"))
      {
        requestContext.WebRequestContextInternal().RequestRestrictions = SignedInProcessor.s_signedInRestrictions;
        requestContext.ValidateIdentity();
      }
      HttpRequestBase request = application.Context.Request;
      string realm = request.QueryString["realm"];
      string protocol = request.QueryString["protocol"];
      string replyTo = request.QueryString["reply_to"];
      string hid = request.QueryString["hid"];
      string visibility = request.QueryString["visibility"];
      string str = request.Form != null ? request.Form["spa_authorization_code"] : (string) null;
      if (!string.IsNullOrEmpty(str))
        requestContext.RootContext.Items["SpaAuthorizationCode"] = (object) str;
      SignedInProcessor.CachedSignInState cachedSignInState = new SignedInProcessor.CachedSignInState()
      {
        ReplyTo = replyTo,
        Context = (SignInContext) null
      };
      if (string.IsNullOrWhiteSpace(replyTo))
        replyTo = cachedSignInState.ReplyTo;
      bool aadCallback;
      NameValueCollection aadStateParams;
      this.ExtractAadStateParams(requestContext, ref realm, ref protocol, ref replyTo, ref hid, ref visibility, out aadCallback, out aadStateParams);
      SignedInParameters parameters;
      SignedInResult result;
      try
      {
        parameters = SignedInParameters.BuildSignedInParameters(requestContext, realm, protocol, replyTo, hid, visibility, request.Url, cachedSignInState.Context);
        result = requestContext.GetExtensions<ISignedInHandler>(ExtensionLifetime.Service).FirstOrDefault<ISignedInHandler>().SignedIn(requestContext, parameters);
      }
      catch (InvalidReplyToException ex)
      {
        requestContext.TraceException(1450705, "Authentication", nameof (SignedInProcessor), (Exception) ex);
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.SignIn.ValidateRedirectUrls.ThrowInvalidReplyToException"))
        {
          requestContext.Trace(1450705, TraceLevel.Error, "Authentication", nameof (SignedInProcessor), "Throwing InvalidReplyToException with message: " + ex.Message);
          throw;
        }
        else
        {
          this.CompleteRequestWithError(application, HttpStatusCode.BadRequest, FrameworkResources.InvalidReplyTo());
          return;
        }
      }
      if (this.aadAuthenticationSessionTokenProvider.Configuration.IssueAadAuthenticationCookieEnabled(requestContext, parameters.ProcessorParameters.HostId))
        this.aadAuthenticationSessionTokenProvider.IssueSessionToken(requestContext, application.Context);
      else
        this.aadAuthenticationSessionTokenProvider.DeleteSessionToken(requestContext, application.Context);
      switch (result.SignedInResultAction)
      {
        case SignedInResultAction.FinalEndpoint:
          if (!string.IsNullOrEmpty(protocol) && protocol.Equals("javascriptnotify", StringComparison.OrdinalIgnoreCase) && !result.IgnoreJavascriptNotify)
          {
            this.CompleteModernIdeSignin(requestContext, application, protocol, aadCallback, aadStateParams, result);
            break;
          }
          this.CompleteStandardWebSignin(requestContext, application, aadCallback, result);
          break;
        case SignedInResultAction.PostFedAuthToken:
          this.RedirectToNextRealm(requestContext, application, parameters.ProcessorParameters, result);
          break;
        case SignedInResultAction.PostAdalToken:
          this.TenantSwitchViaAdalToken(requestContext, application, result);
          break;
        case SignedInResultAction.RedirectToIDP:
          this.RedirectToIDP(requestContext, application, result);
          break;
        case SignedInResultAction.TenantSwitchError:
          this.CompleteInvalidRequest(requestContext, application, result.TenantSwitchException);
          break;
        case SignedInResultAction.Completed:
          break;
        default:
          throw new ArgumentOutOfRangeException("SignedInResultAction");
      }
    }

    private void CompleteInvalidRequest(
      IVssRequestContext requestContext,
      IHttpApplication application,
      TenantSwitchException resultTenantSwitchException)
    {
      FederatedAuthCookieHelper.DeleteSessionTokenCookie(requestContext);
      TeamFoundationApplicationCore.CompleteRequest(requestContext, application, HttpStatusCode.Unauthorized, (Exception) resultTenantSwitchException);
    }

    private void CompleteRequestWithError(
      IHttpApplication application,
      HttpStatusCode statusCode,
      string errorMessage)
    {
      HttpResponseBase response = application.Context.Response;
      response.StatusCode = (int) statusCode;
      response.TrySkipIisCustomErrors = true;
      response.Write(errorMessage);
      application.CompleteRequest();
    }

    private void CompleteStandardWebSignin(
      IVssRequestContext requestContext,
      IHttpApplication application,
      bool aadCallback,
      SignedInResult result)
    {
      requestContext.TraceEnter(1450730, "Authentication", nameof (SignedInProcessor), nameof (CompleteStandardWebSignin));
      string str;
      if (requestContext.RootContext.Items.TryGetValue<string>("SpaAuthorizationCode", out str))
      {
        List<SessionSecurityTokenData> sessionSecurityTokens = new List<SessionSecurityTokenData>()
        {
          new SessionSecurityTokenData(SessionTokenType.SpaAuthorizationCode, (IReadOnlyList<string>) new string[2]
          {
            str,
            result.NextLocation.ToString()
          })
        };
        SecureFlowLocation secureFlowLocation = result.NextLocation.Clone();
        secureFlowLocation.Path = "/_public/_MsalSignedIn";
        this.CreateCiEventPublisher().Publish(requestContext, AuthenticationCiEvent.Sources.Web, aadCallback);
        requestContext.TraceLeave(1450738, "Authentication", nameof (SignedInProcessor), nameof (CompleteStandardWebSignin));
        HttpResponseBase response = application.Context.Response;
        response.StatusCode = 200;
        response.ContentEncoding = Encoding.UTF8;
        response.ContentType = "text/html";
        response.Write(SignedInProcessor.GetRealmSigninContent(secureFlowLocation.ToString(), sessionSecurityTokens));
      }
      else
      {
        string url = result.NextLocation.ToString();
        this.CreateCiEventPublisher().Publish(requestContext, AuthenticationCiEvent.Sources.Web, aadCallback);
        requestContext.TraceLeave(1450739, "Authentication", nameof (SignedInProcessor), nameof (CompleteStandardWebSignin));
        application.Context.Response.Redirect(url, false);
      }
      application.CompleteRequest();
    }

    private void RedirectToIDP(
      IVssRequestContext requestContext,
      IHttpApplication application,
      SignedInResult result)
    {
      requestContext.TraceEnter(1450731, "Authentication", nameof (SignedInProcessor), nameof (RedirectToIDP));
      FederatedAuthCookieHelper.DeleteSessionTokenCookie(requestContext);
      string url = result.NextLocation.ToString();
      application.Context.Response.Redirect(url, false);
      requestContext.TraceLeave(1450732, "Authentication", nameof (SignedInProcessor), nameof (RedirectToIDP));
      application.CompleteRequest();
    }

    private void CompleteModernIdeSignin(
      IVssRequestContext requestContext,
      IHttpApplication application,
      string protocol,
      bool aadCallback,
      NameValueCollection aadStateParams,
      SignedInResult result)
    {
      requestContext.TraceEnter(1450740, "Authentication", nameof (SignedInProcessor), nameof (CompleteModernIdeSignin));
      try
      {
        HttpRequestBase request = application.Context.Request;
        HttpResponseBase response = application.Context.Response;
        string mode;
        string tenantHint;
        this.ExtractIdeParams(requestContext, request, aadStateParams, out mode, out tenantHint);
        if (!string.IsNullOrEmpty(mode) && mode.Equals("secure", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(tenantHint))
        {
          requestContext.Trace(5500100, TraceLevel.Info, "Authentication", nameof (SignedInProcessor), "IDE signin: completed ACS, starting silent AAD auth");
          string url = this.BuildIdeSilentAadAuthUrl(requestContext, protocol, mode, result.NextLocation);
          response.Redirect(url, false);
          application.CompleteRequest();
        }
        else
        {
          requestContext.Trace(5500100, TraceLevel.Info, "Authentication", nameof (SignedInProcessor), "Signin Complete: javascriptnotify");
          this.AddClaimsToSecurityToken(requestContext, (IEnumerable<Claim>) new Claim[1]
          {
            new Claim("IsClient", bool.TrueString)
          });
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_new_client_tokens").Increment();
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_new_client_tokens_persec").Increment();
          List<SessionSecurityTokenData> tokenDataFromCookies = requestContext.GetService<ITeamFoundationAuthenticationService>().GetSessionSecurityTokenDataFromCookies(requestContext);
          SessionSecurityTokenData securityTokenData = tokenDataFromCookies.FirstOrDefault<SessionSecurityTokenData>((Func<SessionSecurityTokenData, bool>) (x => x.TokenType == SessionTokenType.UserAuthentication));
          string s;
          if (securityTokenData != null)
          {
            s = SignedInProcessor.GetJavascriptNotifyContentForUserAuthenticationCookie(securityTokenData.TokenData);
          }
          else
          {
            requestContext.TraceAlways(1010615, TraceLevel.Info, "Authentication", nameof (SignedInProcessor), "Did not have a UserAuth cookie, will use FedAuth to create response payload");
            s = SignedInProcessor.GetJavascriptNotifyContent(tokenDataFromCookies.FirstOrDefault<SessionSecurityTokenData>((Func<SessionSecurityTokenData, bool>) (x => x.TokenType == SessionTokenType.FedAuth)).TokenData);
          }
          response.StatusCode = 200;
          response.ContentEncoding = Encoding.UTF8;
          response.ContentType = "text/html";
          response.Write(s);
          this.CreateCiEventPublisher().Publish(requestContext, AuthenticationCiEvent.Sources.Ide, aadCallback);
          application.CompleteRequest();
        }
      }
      finally
      {
        requestContext.TraceLeave(1450749, "Authentication", nameof (SignedInProcessor), nameof (CompleteModernIdeSignin));
      }
    }

    private void AddClaimsToSecurityToken(
      IVssRequestContext requestContext,
      IEnumerable<Claim> claims)
    {
      requestContext.TraceEnter(1450750, "Authentication", nameof (SignedInProcessor), nameof (AddClaimsToSecurityToken));
      try
      {
        if (claims == null)
          return;
        ClaimsPrincipal user = HttpContext.Current.User as ClaimsPrincipal;
        requestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal().IssueSessionSecurityToken(requestContext, user, claims);
      }
      finally
      {
        requestContext.TraceLeave(1450759, "Authentication", nameof (SignedInProcessor), nameof (AddClaimsToSecurityToken));
      }
    }

    private void RedirectToNextRealm(
      IVssRequestContext requestContext,
      IHttpApplication application,
      SignedInProcessorParameters parameters,
      SignedInResult result)
    {
      requestContext.TraceEnter(1450760, "Authentication", nameof (SignedInProcessor), nameof (RedirectToNextRealm));
      requestContext.Trace(5500100, TraceLevel.Info, "Authentication", nameof (SignedInProcessor), "Signin Complete: {0}", (object) parameters.Realm);
      List<SessionSecurityTokenData> tokenDataFromCookies = requestContext.GetService<ITeamFoundationAuthenticationService>().GetSessionSecurityTokenDataFromCookies(requestContext);
      HttpResponseBase response = application.Context.Response;
      response.StatusCode = 200;
      response.ContentEncoding = Encoding.UTF8;
      response.ContentType = "text/html";
      response.Write(SignedInProcessor.GetRealmSigninContent(result.NextLocation.ToString(), tokenDataFromCookies));
      requestContext.TraceLeave(1450769, "Authentication", nameof (SignedInProcessor), nameof (RedirectToNextRealm));
      application.CompleteRequest();
    }

    private void TenantSwitchViaAdalToken(
      IVssRequestContext requestContext,
      IHttpApplication application,
      SignedInResult result)
    {
      requestContext.TraceEnter(1450733, "Authentication", nameof (SignedInProcessor), nameof (TenantSwitchViaAdalToken));
      FederatedAuthCookieHelper.DeleteSessionTokenCookie(requestContext);
      HttpResponseBase response = application.Context.Response;
      response.StatusCode = 200;
      response.ContentEncoding = Encoding.UTF8;
      response.ContentType = "text/html";
      response.Write(SignedInProcessor.GetTenantSwitchContent(result.NextLocation.ToString(), result.TenantSwitchAccessToken));
      requestContext.TraceEnter(1450734, "Authentication", nameof (SignedInProcessor), nameof (TenantSwitchViaAdalToken));
      application.CompleteRequest();
    }

    private void ExtractAadStateParams(
      IVssRequestContext requestContext,
      ref string realm,
      ref string protocol,
      ref string reply_to,
      ref string hid,
      ref string visibility,
      out bool aadCallback,
      out NameValueCollection aadStateParams)
    {
      requestContext.TraceEnter(1450780, "Authentication", nameof (SignedInProcessor), nameof (ExtractAadStateParams));
      aadCallback = false;
      aadStateParams = (NameValueCollection) null;
      if (realm == null && protocol == null && reply_to == null)
      {
        aadStateParams = AadAuthUrlUtility.ParseState(requestContext);
        if (aadStateParams != null)
        {
          realm = aadStateParams[nameof (realm)];
          protocol = aadStateParams[nameof (protocol)];
          reply_to = aadStateParams[nameof (reply_to)];
          hid = aadStateParams[nameof (hid)];
          visibility = aadStateParams[nameof (visibility)];
          aadCallback = true;
        }
      }
      requestContext.TraceLeave(1450785, "Authentication", nameof (SignedInProcessor), nameof (ExtractAadStateParams));
    }

    private string BuildIdeSilentAadAuthUrl(
      IVssRequestContext requestContext,
      string protocol,
      string mode,
      SecureFlowLocation nextLocation)
    {
      requestContext.TraceEnter(1450786, "Authentication", nameof (SignedInProcessor), nameof (BuildIdeSilentAadAuthUrl));
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          "reply_to",
          nextLocation.ToString()
        },
        {
          nameof (protocol),
          protocol
        },
        {
          nameof (mode),
          mode
        },
        {
          "tenant",
          "live.com"
        }
      };
      string str = new AadAuthUrlUtility.AuthUrlBuilder()
      {
        DomainHint = "live.com",
        Tenant = "live.com",
        QueryString = ((IDictionary<string, string>) dictionary)
      }.Build(requestContext);
      requestContext.TraceLeave(1450789, "Authentication", nameof (SignedInProcessor), nameof (BuildIdeSilentAadAuthUrl));
      return str;
    }

    private void ExtractIdeParams(
      IVssRequestContext requestContext,
      HttpRequestBase request,
      NameValueCollection aadStateParams,
      out string mode,
      out string tenantHint)
    {
      requestContext.TraceEnter(1450790, "Authentication", nameof (SignedInProcessor), nameof (ExtractIdeParams));
      mode = string.Empty;
      tenantHint = string.Empty;
      if (!string.IsNullOrEmpty(request.Url.Query))
      {
        mode = request.QueryString[nameof (mode)];
        tenantHint = request.QueryString["tenant"];
        if ((string.IsNullOrEmpty(tenantHint) || string.IsNullOrEmpty(mode)) && aadStateParams == null)
        {
          aadStateParams = AadAuthUrlUtility.ParseState(requestContext);
          if (aadStateParams != null)
          {
            if (string.IsNullOrEmpty(mode))
              mode = aadStateParams[nameof (mode)];
            if (string.IsNullOrEmpty(tenantHint))
              tenantHint = aadStateParams["tenant"];
          }
        }
      }
      requestContext.TraceLeave(1450795, "Authentication", nameof (SignedInProcessor), nameof (ExtractIdeParams));
    }

    protected virtual SignedInCiEventPublisher CreateCiEventPublisher() => new SignedInCiEventPublisher();

    internal static string GetRealmSigninContent(
      string actionUrl,
      List<SessionSecurityTokenData> sessionSecurityTokens)
    {
      List<string> values = new List<string>();
      foreach (SessionSecurityTokenData sessionSecurityToken in sessionSecurityTokens)
      {
        SessionSecurityTokenData sessionSecurityTokenData = sessionSecurityToken;
        if (sessionSecurityTokenData.TokenType == SessionTokenType.UserAuthentication)
          values.AddRange(sessionSecurityTokenData.TokenData.Select<string, string>((Func<string, int, string>) ((part, index) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />", (object) "id_token", (object) sessionSecurityTokenData.TokenData[index], (object) part))));
        else if (sessionSecurityTokenData.TokenType == SessionTokenType.AadAuthentication)
        {
          values.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />", (object) "aad_token", (object) sessionSecurityTokenData.TokenData[0]));
          if (sessionSecurityTokenData.TokenData.Count > 1)
            values.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />", (object) "spa_authorization_code", (object) sessionSecurityTokenData.TokenData[1]));
        }
        else if (sessionSecurityTokenData.TokenType == SessionTokenType.SpaAuthorizationCode)
        {
          values.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />", (object) "spa_authorization_code", (object) sessionSecurityTokenData.TokenData[0]));
          values.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />", (object) "location", (object) sessionSecurityTokenData.TokenData[1]));
        }
        else
          values.AddRange(sessionSecurityTokenData.TokenData.Select<string, string>((Func<string, int, string>) ((part, index) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />", (object) SignedInProcessor.GetFedAuthCookieName(index), (object) sessionSecurityTokenData.TokenData[index], (object) part))));
      }
      string newValue = string.Join("\r\n            ", (IEnumerable<string>) values);
      return FrameworkResources.SignedInContentForRealm().Replace("$$FormAction$$", actionUrl).Replace("$$FormInputs$$", newValue);
    }

    private static string GetTenantSwitchContent(string actionUrl, string tenantSwitchToken)
    {
      string newValue = string.Join("\r\n            ", new string[1]
      {
        string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />", (object) "Authorization", (object) tenantSwitchToken)
      });
      return FrameworkResources.SignedInContentForRealm().Replace("$$FormAction$$", actionUrl).Replace("$$FormInputs$$", newValue);
    }

    private static string GetFedAuthCookieName(int index) => "FedAuth" + (index > 0 ? index.ToString() : string.Empty);

    private static string GetJavascriptNotifyContent(IReadOnlyList<string> authTokenParts)
    {
      string newValue = string.Join(",", authTokenParts.Select<string, string>((Func<string, string>) (part => "\"" + part + "\"")));
      return FrameworkResources.SignedInContentForJavascriptNotify().Replace("$$TokenData$$", newValue);
    }

    private static string GetJavascriptNotifyContentForUserAuthenticationCookie(
      IReadOnlyList<string> authTokenParts)
    {
      if (authTokenParts.Count != 1)
        return string.Empty;
      string newValue = string.Join(",", new string[2]
      {
        "\"" + "UserAuthenticationToken" + "\"",
        "\"" + authTokenParts.First<string>() + "\""
      });
      return FrameworkResources.SignedInContentForJavascriptNotify().Replace("$$TokenData$$", newValue);
    }

    private class CachedSignInState
    {
      internal SignInContext Context { get; set; }

      internal string ReplyTo { get; set; }
    }
  }
}
