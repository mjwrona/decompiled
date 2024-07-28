// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TestAuthenticationModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TestAuthenticationModule : VssfAuthenticationHttpModuleBase
  {
    private const string s_authenticateIdentityHeader = "X-SPS-AuthenticateTestIdentity";
    private const string s_createIdentityHeader = "X-SPS-CreateTestIdentity";
    private const string s_dumpSessionTokenHeader = "X-VSS-DumpSessionToken";
    private const string s_identityHeader = "X-TFS-TestIdentity";
    private const string AcsIdentityProviderClaimType = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";

    protected override bool SkipIfAlreadyAuthenticated => false;

    public override void OnAuthenticateRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
      requestContext.RootContext.Items["#\\Configuration\\Identity\\Providers\\Test\\Enabled\\"] = (object) true;
      string requestData1 = TestAuthenticationModule.GetRequestData(httpContext.Request, "X-SPS-CreateTestIdentity");
      if (!string.IsNullOrWhiteSpace(requestData1))
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        TestAuthenticationModule.CreateTestIdentity(requestContext, service, requestData1);
      }
      string requestData2 = TestAuthenticationModule.GetRequestData(httpContext.Request, "X-SPS-AuthenticateTestIdentity");
      if (!string.IsNullOrWhiteSpace(requestData2))
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        httpContext.User = (IPrincipal) TestAuthenticationModule.AuthenticateTestIdentity((VssRequestContext) requestContext, service, requestData2);
        AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.Test);
      }
      else
      {
        string requestData3 = TestAuthenticationModule.GetRequestData(httpContext.Request, "X-TFS-TestIdentity");
        if (requestData3 == null)
          return;
        httpContext.User = (IPrincipal) new ServerTestPrincipal()
        {
          Identity = (IIdentity) new ServerTestIdentity()
          {
            Identifier = requestData3
          }
        };
        AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.Test);
      }
    }

    public override void OnPostAuthenticateRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
      string header = httpContext.Request.QueryString["X-VSS-DumpSessionToken"];
      if (string.IsNullOrWhiteSpace(header))
        header = httpContext.Request.Headers["X-VSS-DumpSessionToken"];
      if (string.IsNullOrWhiteSpace(header))
        return;
      SessionSecurityToken contextItem = TeamFoundationAuthenticationService.GetContextItem("SessionToken") as SessionSecurityToken;
      string responseText = TestAuthenticationModule.DumpSessionToken(httpContext, httpContext.User != null && httpContext.User.Identity.IsAuthenticated, contextItem);
      if (httpContext.Items[(object) "OnErrorFormatEvent"] != null)
        httpContext.Items.Remove((object) "OnErrorFormatEvent");
      TeamFoundationApplicationCore.CompleteRequest(httpContext.GetApplicationInstance(), HttpStatusCode.OK, (string) null, (IEnumerable<KeyValuePair<string, string>>) null, (Exception) null, (string) null, responseText);
    }

    private static string GetRequestData(HttpRequestBase request, string key)
    {
      string header = request.Headers[key];
      if (string.IsNullOrWhiteSpace(header))
      {
        HttpCookie cookie = request.Cookies[key];
        if (cookie != null)
          header = cookie.Value;
      }
      return header;
    }

    private static ClaimsPrincipal AuthenticateTestIdentity(
      VssRequestContext requestContext,
      IdentityService identityService,
      string headerValue)
    {
      try
      {
        NameValueCollection nameValueCollection = new FormDataCollection(headerValue).ReadAsNameValueCollection();
        Dictionary<string, Claim> dictionary = new Dictionary<string, Claim>();
        foreach (string key in nameValueCollection.Keys)
        {
          string str = nameValueCollection[key];
          if (!string.IsNullOrWhiteSpace(str))
          {
            switch (key)
            {
              case "Name":
                dictionary["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] = new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", str);
                continue;
              case "NameIdentifier":
                dictionary["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] = new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", str);
                continue;
              case "Email":
                dictionary["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] = new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", str);
                continue;
              case "Puid":
                dictionary["PUID"] = new Claim("PUID", str);
                continue;
              case "AcsIdentityProvider":
                dictionary["http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider"] = new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", str);
                continue;
              case "IdentityProvider":
                dictionary["http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider"] = new Claim("http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider", str);
                continue;
              default:
                dictionary[key] = new Claim(key, str);
                continue;
            }
          }
        }
        Claim valueOrDefault1 = dictionary.GetValueOrDefault<string, Claim>("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", (Claim) null);
        Claim valueOrDefault2 = dictionary.GetValueOrDefault<string, Claim>("PUID", (Claim) null);
        Claim valueOrDefault3 = dictionary.GetValueOrDefault<string, Claim>("Account", (Claim) null);
        Claim valueOrDefault4 = dictionary.GetValueOrDefault<string, Claim>("Domain", (Claim) null);
        if ((valueOrDefault1 == null || valueOrDefault2 == null) && valueOrDefault3 != null && valueOrDefault4 != null && !string.IsNullOrEmpty(valueOrDefault3.Value) && !string.IsNullOrEmpty(valueOrDefault4.Value))
        {
          string factorValue = valueOrDefault4.Value + (object) '\\' + valueOrDefault3.Value;
          Microsoft.VisualStudio.Services.Identity.Identity identity = identityService.ReadIdentities(requestContext.Elevate(true), IdentitySearchFilter.AccountName, factorValue, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          if (identity != null)
          {
            if (valueOrDefault1 == null)
              dictionary["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] = new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", identity.Descriptor.Identifier);
            if (valueOrDefault2 == null)
              dictionary["PUID"] = new Claim("PUID", identity.GetProperty<string>("PUID", string.Empty));
          }
        }
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal((IEnumerable<ClaimsIdentity>) new ClaimsIdentity[1]
        {
          new ClaimsIdentity((IEnumerable<Claim>) dictionary.Values, "Federation")
        });
        requestContext.RootContext.Items["AuthenticationByIdentityProvider"] = (object) true;
        requestContext.RootContext.Items["CredentialValidFrom"] = (object) DateTime.UtcNow;
        requestContext.GetService<IIdentityAuthenticationService>().ResolveIdentity(requestContext.Elevate(true), claimsPrincipal.Identity);
        return claimsPrincipal;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Authentication", nameof (TestAuthenticationModule), ex);
      }
      return (ClaimsPrincipal) null;
    }

    private static void CreateTestIdentity(
      IVssRequestContext requestContext,
      IdentityService identityService,
      string headerValue)
    {
      try
      {
        NameValueCollection nameValueCollection = new FormDataCollection(headerValue).ReadAsNameValueCollection();
        IdentityDescriptor identityDescriptor = new IdentityDescriptor()
        {
          Identifier = nameValueCollection["Identifier"],
          IdentityType = "Microsoft.IdentityModel.Claims.ClaimsIdentity"
        };
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity();
        identity1.Descriptor = identityDescriptor;
        identity1.ProviderDisplayName = nameValueCollection["DisplayName"];
        identity1.IsActive = true;
        identity1.UniqueUserId = 0;
        identity1.IsContainer = false;
        identity1.Members = (ICollection<IdentityDescriptor>) null;
        identity1.MemberOf = (ICollection<IdentityDescriptor>) null;
        Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity1;
        identity2.SetProperty("Description", (object) (nameValueCollection["Description"] ?? "Test Claims Identity"));
        identity2.SetProperty("Domain", (object) (nameValueCollection["Domain"] ?? "Windows Live ID"));
        identity2.SetProperty("Account", (object) nameValueCollection["AccountName"]);
        identity2.SetProperty("Mail", (object) nameValueCollection["MailAddress"]);
        identity2.SetProperty("PUID", (object) nameValueCollection["Puid"]);
        identityService.UpdateIdentities(requestContext.Elevate(), (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
        {
          identity2
        });
      }
      catch
      {
      }
    }

    private static string DumpSessionToken(
      HttpContextBase httpContext,
      bool authenticated,
      SessionSecurityToken sessionToken)
    {
      StringBuilder dump = new StringBuilder("<!DOCTYPE html><html><head><title>Session token</title><link rel='stylesheet' href='http://metroui.org.ua/css/modern.css' type='text/css'></head><body class='metrouicss'><h2>Session token</h2><table class='bordered'><thead><th>Name</th><th>Value</th></thead>");
      TestAuthenticationModule.DumpSessionTokenValue(dump, "Authenticated", (object) authenticated);
      TestAuthenticationModule.DumpSessionTokenValue(dump, "Has Token", (object) (sessionToken != null));
      TestAuthenticationModule.DumpSessionTokenValue(dump, "UTC Time", (object) DateTime.UtcNow);
      if (sessionToken != null)
      {
        TestAuthenticationModule.DumpSessionTokenValue(dump, "&nbsp;", (object) "&nbsp;");
        TestAuthenticationModule.DumpSessionTokenValue(dump, "ID", (object) sessionToken.Id);
        TestAuthenticationModule.DumpSessionTokenValue(dump, "Valid From", (object) sessionToken.ValidFrom);
        TestAuthenticationModule.DumpSessionTokenValue(dump, "Valid To", (object) sessionToken.ValidTo);
        TestAuthenticationModule.DumpSessionTokenValue(dump, "Is Persistent", (object) sessionToken.IsPersistent);
        TestAuthenticationModule.DumpSessionTokenValue(dump, "Is Reference Mode", (object) sessionToken.IsReferenceMode);
        TestAuthenticationModule.DumpSessionTokenValue(dump, "Context", (object) sessionToken.Context);
        TestAuthenticationModule.DumpSessionTokenValue(dump, "Context ID", (object) sessionToken.ContextId);
        TestAuthenticationModule.DumpSessionTokenValue(dump, "Endpoint ID", (object) sessionToken.EndpointId);
        TestAuthenticationModule.DumpSessionTokenValue(dump, "Key Effective Time", (object) sessionToken.KeyEffectiveTime);
        TestAuthenticationModule.DumpSessionTokenValue(dump, "Key Expiration Time", (object) sessionToken.KeyExpirationTime);
        TestAuthenticationModule.DumpSessionTokenValue(dump, "Key Generation", (object) sessionToken.KeyGeneration);
        TestAuthenticationModule.DumpSessionTokenValue(dump, "Secure Conversation Version", (object) sessionToken.SecureConversationVersion);
        TestAuthenticationModule.DumpSessionTokenValue(dump, "Security Keys", sessionToken.SecurityKeys == null ? (object) null : (object) sessionToken.SecurityKeys.Count);
        ClaimsPrincipal claimsPrincipal = sessionToken.ClaimsPrincipal;
        if (claimsPrincipal != null)
        {
          TestAuthenticationModule.DumpSessionTokenValue(dump, "&nbsp;", (object) "&nbsp;");
          IEnumerable<ClaimsIdentity> identities = claimsPrincipal.Identities;
          TestAuthenticationModule.DumpSessionTokenValue(dump, "Identities", identities == null ? (object) null : (object) identities.Count<ClaimsIdentity>());
          if (identities != null)
          {
            TestAuthenticationModule.DumpSessionTokenValue(dump, "&nbsp;", (object) "&nbsp;");
            foreach (ClaimsIdentity claimsIdentity in identities)
            {
              IEnumerable<Claim> claims = claimsIdentity.Claims;
              TestAuthenticationModule.DumpSessionTokenValue(dump, "Claims", claims == null ? (object) null : (object) claimsIdentity.Claims.Count<Claim>());
              foreach (Claim claim in claims)
                TestAuthenticationModule.DumpSessionTokenValue(dump, claim.Type, (object) claim.Value);
            }
          }
        }
      }
      dump.Append("</table></body></html>");
      return dump.ToString();
    }

    private static void DumpSessionTokenValue(StringBuilder dump, string name, object value)
    {
      dump.Append("<tr><td>");
      dump.Append(name ?? "&nbsp;");
      dump.Append("</td><td>");
      dump.Append(value ?? (object) "(none)");
      dump.Append("</td></tr>");
    }
  }
}
