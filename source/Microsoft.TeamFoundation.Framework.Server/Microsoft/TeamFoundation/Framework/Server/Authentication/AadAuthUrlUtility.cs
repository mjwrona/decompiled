// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.AadAuthUrlUtility
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authentication.Settings;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public static class AadAuthUrlUtility
  {
    public const string SignedInEndPoint = "_signedin";
    public const string CommonTenant = "common";
    private const string Market = "mkt";
    internal const string State = "state";
    private const string Nonce = "nonce";
    private const string Compact = "compact";
    private const string Mode = "mode";
    private const string UserMode = "user";
    private const string s_area = "Identity";
    private const string s_layer = "AadAuthUrlUtility";

    public static string GetAuthenticationAuthority(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return new Uri(vssRequestContext.GetService<IAadConfigurationService>().GetSettings(vssRequestContext).AuthBaseUrl).GetLeftPart(UriPartial.Authority);
    }

    public static NameValueCollection ParseState(IVssRequestContext requestContext = null)
    {
      string query = (string) null;
      if (requestContext != null)
        query = HttpUtility.ParseQueryString(requestContext.RequestUri().Query)["state"];
      if (string.IsNullOrEmpty(query))
        query = HttpContextFactory.Current?.Request?.Params?["state"];
      return string.IsNullOrEmpty(query) ? new NameValueCollection() : HttpUtility.ParseQueryString(query);
    }

    public static bool TryCreateAadProfileSilently(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity requestIdentity,
      string requestedDisplayName)
    {
      IDictionary<string, object> items = requestContext.To(TeamFoundationHostType.Deployment).Items;
      AadAuthUrlUtility.SilentProfileCreationFailureReason? nullable = new AadAuthUrlUtility.SilentProfileCreationFailureReason?();
      requestContext.TraceEnter(2000000, "Identity", nameof (AadAuthUrlUtility), nameof (TryCreateAadProfileSilently));
      if (requestIdentity.Properties.ContainsKey("ComplianceValidated"))
        return true;
      IUserService service = requestContext.GetService<IUserService>();
      try
      {
        CreateUserParameters userParameters = new CreateUserParameters();
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        string str1 = (string) null;
        try
        {
          str1 = vssRequestContext.GetService<IGeoLocationService>().GetRequestCountryCode(vssRequestContext);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(2000007, "Identity", nameof (AadAuthUrlUtility), ex);
        }
        if (str1 == null)
        {
          nullable = new AadAuthUrlUtility.SilentProfileCreationFailureReason?(AadAuthUrlUtility.SilentProfileCreationFailureReason.CountryLookUpFailed);
          requestContext.Trace(2000008, TraceLevel.Error, "Identity", nameof (AadAuthUrlUtility), "County Lookup failed for User {0}, IPAddress {1}", (object) requestIdentity.Id, (object) requestContext.RemoteIPAddress());
          return false;
        }
        string str2 = (string) null;
        requestIdentity.Properties.TryGetValue<string>("Account", out str2);
        string str3 = (string) null;
        if (requestIdentity.Properties.TryGetValue<string>("Mail", out str3) && string.IsNullOrWhiteSpace(str3))
          str3 = str2;
        string displayName = requestedDisplayName.Equals(string.Empty) ? requestIdentity.DisplayName : requestedDisplayName;
        requestContext.Trace(2000004, TraceLevel.Verbose, "Identity", nameof (AadAuthUrlUtility), "Using email address {0} for user {1} displayName {2} country {3}", (object) str3, (object) requestIdentity.Id, (object) displayName, (object) str1);
        userParameters.DisplayName = displayName;
        userParameters.Mail = str3;
        userParameters.Country = str1;
        Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (vssRequestContext.Items.ContainsKey(ProfileCreateSourceConstants.Key))
        {
          string empty = string.Empty;
          vssRequestContext.TryGetItem<string>(ProfileCreateSourceConstants.Key, out empty);
          dictionary.Add(ProfileCreateSourceConstants.Key, (object) empty);
        }
        else
          dictionary.Add(ProfileCreateSourceConstants.Key, (object) ProfileCreateSourceConstants.IdentityMeEndpoint);
        service.CreateUser(vssRequestContext, userParameters);
        AuthenticatedUserCookie.Set(requestContext, displayName);
        requestContext.Trace(2000005, TraceLevel.Verbose, "Identity", nameof (AadAuthUrlUtility), "Silent AAD profile creation successful for user {0}, accountName {1}", (object) requestIdentity.Id, (object) str2.ToString());
        return true;
      }
      catch (UserAlreadyExistsException ex)
      {
        requestContext.TraceException(2000002, TraceLevel.Info, "Identity", nameof (AadAuthUrlUtility), (Exception) ex);
        return true;
      }
      catch (Exception ex)
      {
        nullable = new AadAuthUrlUtility.SilentProfileCreationFailureReason?(AadAuthUrlUtility.SilentProfileCreationFailureReason.System);
        requestContext.TraceException(2000006, "Identity", nameof (AadAuthUrlUtility), ex);
        return false;
      }
      finally
      {
        if (nullable.HasValue)
        {
          string source = (string) null;
          if (!requestContext.TryGetItem<string>(ProfileCreateSourceConstants.Key, out source))
            source = ProfileCreateSourceConstants.IDE;
          AadAuthUrlUtility.LogSilentProfileFailureCIEvent(requestContext, source, requestIdentity, nullable.Value.ToString("G"));
        }
        requestContext.TraceLeave(2000007, "Identity", nameof (AadAuthUrlUtility), nameof (TryCreateAadProfileSilently));
      }
    }

    private static void LogSilentProfileFailureCIEvent(
      IVssRequestContext requestContext,
      string source,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      string failureReason)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add(CustomerIntelligenceProperty.CreationSource, source);
      intelligenceData.Add(CustomerIntelligenceProperty.ProfileId, (object) userIdentity.Id);
      intelligenceData.Add(CustomerIntelligenceProperty.ProfileCUID, (object) userIdentity.Cuid());
      intelligenceData.Add(CustomerIntelligenceProperty.FailureReason, failureReason);
      IVssRequestContext requestContext1 = requestContext;
      string profile = CustomerIntelligenceArea.Profile;
      string profileCreationFailure = CustomerIntelligenceFeature.SileProfileCreationFailure;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, profile, profileCreationFailure, properties);
    }

    private static CultureInfo GetRetainableCulture()
    {
      HttpContext current = HttpContext.Current;
      CultureInfo threadUiCulture = current == null ? (CultureInfo) null : RequestLanguage.GetThreadUICulture(current.Items);
      CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
      return threadUiCulture == null || threadUiCulture == currentUiCulture ? (CultureInfo) null : currentUiCulture;
    }

    private static string BuildState(IDictionary<string, string> queryString) => SignInContext.ConvertQueryStringToURLSegment(queryString);

    private static Uri BuildRedirectUri(IVssRequestContext requestContext)
    {
      AccessMapping accessMapping = requestContext.GetService<ILocationService>().DetermineAccessMapping(requestContext);
      return UriUtility.Combine(accessMapping.AccessPoint, PathUtility.Combine(accessMapping.VirtualDirectory, "_signedin"), true);
    }

    public class AuthUrlBuilder
    {
      public string UserHint { get; set; }

      public string Tenant { get; set; }

      public string Client { get; set; }

      public string RedirectLocation { get; set; }

      public string Resource { get; set; }

      public IDictionary<string, string> QueryString { get; set; }

      public string DomainHint { get; set; }

      public bool ForceWorkAccount { get; set; }

      public AadAuthUrlUtility.PromptOption PromptOption { get; set; }

      public string Build(IVssRequestContext requestContext)
      {
        this.QueryString = this.QueryString ?? (IDictionary<string, string>) new Dictionary<string, string>();
        Guid result = Guid.Empty;
        if (string.IsNullOrWhiteSpace(this.Tenant) || Guid.TryParse(this.Tenant, out result) && result == Guid.Empty)
          this.Tenant = "common";
        else if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisableCommonTenantForLiveTenant") && this.Tenant.Equals("live.com"))
          this.Tenant = "common";
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        AadSettings settings = vssRequestContext.GetService<IAadConfigurationService>().GetSettings(vssRequestContext);
        Guid empty = Guid.Empty;
        if (string.IsNullOrWhiteSpace(this.Client))
          this.Client = settings.ClientId;
        if (string.IsNullOrWhiteSpace(this.RedirectLocation))
          this.RedirectLocation = AadAuthUrlUtility.BuildRedirectUri(requestContext).AbsoluteUri;
        if (string.IsNullOrWhiteSpace(this.Resource))
        {
          string graphApiResource = settings.GraphApiResource;
          this.Resource = new UriBuilder(Uri.UriSchemeHttps, new Uri(graphApiResource).Host).Uri.AbsoluteUri;
        }
        string authUrlPathTemplate = settings.AuthUrlPathTemplate;
        string authSiteId = settings.AuthSiteId;
        SessionTrackingState sessionState;
        requestContext.GetSessionTrackingState(out sessionState);
        requestContext.TraceSerializedConditionally(15180040, TraceLevel.Verbose, "Identity", nameof (AadAuthUrlUtility), "Retrieved sessionState: {0}", (object) sessionState);
        ISessionCookieProcessor sessionCookieProcessor = SessionCookieHelper.GetSessionCookieProcessor(requestContext);
        if (sessionState != null && sessionState.PendingAuthenticationSessionId == Guid.Empty)
        {
          sessionCookieProcessor.StartNewAuthenticationSession(requestContext);
          requestContext.GetSessionTrackingState(out sessionState);
          requestContext.TraceSerializedConditionally(15180041, TraceLevel.Warning, "Identity", nameof (AadAuthUrlUtility), "AadAuthUrlUtility invoked without pendingAuthSession. New state: {0}", (object) sessionState);
        }
        else if (sessionState == null)
        {
          sessionCookieProcessor.EnsureSessionCookieExists(requestContext);
          sessionCookieProcessor.StartNewAuthenticationSession(requestContext);
          requestContext.GetSessionTrackingState(out sessionState);
          requestContext.TraceSerializedConditionally(15180042, TraceLevel.Error, "Identity", nameof (AadAuthUrlUtility), "AadAuthUrlUtility invoked without any session state. This should be impossible. New state: {0}", (object) sessionState);
        }
        Guid guid1;
        Guid guid2;
        if (sessionState == null || sessionState.PendingAuthenticationSessionId == Guid.Empty)
        {
          requestContext.TraceSerializedConditionally(15180044, TraceLevel.Error, "Identity", nameof (AadAuthUrlUtility), "sessionState is not properly initialized. sessionState: {0}", (object) sessionState);
          guid1 = Guid.Empty;
          guid2 = requestContext.ActivityId;
        }
        else
        {
          guid1 = sessionState.PendingAuthenticationSessionId;
          guid2 = sessionState.PendingAuthenticationSessionId;
        }
        this.QueryString["nonce"] = guid1.ToString();
        if (!this.QueryString.ContainsKey("mkt"))
        {
          CultureInfo retainableCulture = AadAuthUrlUtility.GetRetainableCulture();
          if (retainableCulture != null)
            this.QueryString["mkt"] = retainableCulture.Name;
        }
        string stringToEscape1 = AadAuthUrlUtility.BuildState(this.QueryString);
        string stringToEscape2 = this.BuildCapLoopingLoginPreventionClaimsChallenge(requestContext);
        UriBuilder uriBuilder = new UriBuilder(new Uri(new Uri(AadAuthUrlUtility.GetAuthenticationAuthority(requestContext)), string.Format(authUrlPathTemplate, (object) Uri.EscapeDataString(this.Tenant), (object) Uri.EscapeDataString(this.Client), (object) authSiteId, (object) Uri.EscapeDataString(this.RedirectLocation), (object) guid1, (object) Uri.EscapeDataString(stringToEscape1), (object) Uri.EscapeDataString(this.Resource), (object) guid2, (object) Uri.EscapeDataString(stringToEscape2))));
        if (this.Tenant == "common")
        {
          if (this.QueryString.ContainsKey("provider"))
            uriBuilder.AppendProvider(this.QueryString["provider"]);
          else
            uriBuilder.AppendGithubSignIn();
          uriBuilder.AppendQuery("msaoauth2", "true");
        }
        if (this.QueryString.ContainsKey("mkt"))
          uriBuilder.AppendQuery("mkt", this.QueryString["mkt"]);
        if (!string.IsNullOrWhiteSpace(this.UserHint))
          uriBuilder.AppendQuery("login_hint", this.UserHint);
        if (!string.IsNullOrWhiteSpace(this.DomainHint))
          uriBuilder.AppendQuery("domain_hint", this.DomainHint);
        if (this.ForceWorkAccount)
          uriBuilder.AppendQuery("msafed", "0");
        switch (this.PromptOption)
        {
          case AadAuthUrlUtility.PromptOption.Login:
            uriBuilder.AppendQuery("prompt", "login");
            break;
          case AadAuthUrlUtility.PromptOption.SelectAccount:
            uriBuilder.AppendQuery("prompt", "select_account");
            break;
          case AadAuthUrlUtility.PromptOption.FreshLogin:
            uriBuilder.AppendQuery("fresh_login", "1");
            break;
          case AadAuthUrlUtility.PromptOption.FreshLoginWithMfa:
            uriBuilder.AppendQuery("prompt", "login");
            uriBuilder.AppendQuery("amr_values", "mfa");
            break;
        }
        if (this.QueryString.ContainsKey("compact"))
        {
          uriBuilder.AppendQuery("display", "host");
          if (this.PromptOption == AadAuthUrlUtility.PromptOption.NoOption && this.QueryString.ContainsKey("mode") && string.Equals(this.QueryString["mode"], "user", StringComparison.OrdinalIgnoreCase))
            uriBuilder.AppendQuery("prompt", "select_account");
          uriBuilder.AppendQuery("haschrome", "1");
        }
        string str = uriBuilder.AbsoluteUri();
        requestContext.TraceSerializedConditionally(15180043, TraceLevel.Verbose, "Identity", nameof (AadAuthUrlUtility), "Generated Auth URI: {0}", (object) str);
        return str;
      }

      private string BuildCapLoopingLoginPreventionClaimsChallenge(IVssRequestContext requestContext)
      {
        string str = requestContext.RemoteIPAddress();
        return string.IsNullOrWhiteSpace(str) ? string.Empty : "{\"access_token\":{\"xms_rp_ipaddr\":{\"essential\":true,\"value\":\"" + str + "\"}}}";
      }
    }

    public enum PromptOption
    {
      NoOption,
      Login,
      SelectAccount,
      FreshLogin,
      FreshLoginWithMfa,
    }

    public enum SilentProfileCreationFailureReason
    {
      System,
      AadServiceException,
      CountryLookUpFailed,
      InvalidDisplayName,
    }
  }
}
