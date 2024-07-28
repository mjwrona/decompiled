// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuthenticationConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class AuthenticationConstants
  {
    public const string AadTenantDisambiguationClaim = "tenant_disambiguate";
    public const string AadMsaPassthroughClaim = "msapt";
    public const string IsAadAuthFlow = "IsAadAuthFlow";
    public const string TenantHintParam = "tenant";
    public const string ModeParam = "mode";
    public const string ProfileMode = "profile";
    public const string SecureMode = "secure";
    public const string UserMode = "user";
    public const string SelectHomeTenantParam = "select_home";
    public const string ShowMsaParam = "show_msa";
    public const string LiveTenant = "live.com";
    public const string DomainHintParam = "domain_hint";
    public const string ForceAad = "forceAad";
    public const string ForceAadParam = "1";
    public const string TenantId = "tenantId";
    public const string UserName = "userName";
    public const string WindowsLiveIDProvider = "Windows Live ID";
    public const string PreAuthSilentAadProfileCreationRequested = "PreAuthSilentAadProfileCreationRequested";
    public const string AuthenticationByIdentityProvider = "AuthenticationByIdentityProvider";
    public const string PostAuthCreateSilentAadProfile = "PostAuthCreateSilentAadProfile";
    public const string RequestSilentAADProfileParam = "request_silent_aad_profile";
    public const string SpaAuthorizationCode = "SpaAuthorizationCode";
    public const string AuthorizationGrantScope = "vso.authorization_grant";
    public const string AuthenticationWithSessionAuth = "AuthenticationWithSessionAuth";
    public const string AuthenticationWithSWT = "AuthenticationWithAcs";
    public const string CustomTenantPickerMessageParam = "ctpm";
    public const string RefeshTokenValidatorTokenCookieName = "RefreshToken";
    public const string Bearer = "Bearer";
    public const string CredentialValidFrom = "CredentialValidFrom";
    public const string PromptType = "prompttype";
    public const string WIFAction = "wa";
    public const string WIFActionSignoutCleanup = "wsignoutcleanup1.0";
    public const string PoP = "pop";
    public const string SessionTrackingCookieName = "VstsSession";
    internal const string SessionAuthenticationFaultInjection = "VisualStudio.Services.Authentication.SessionAuthenticationFaultInjection";
    internal const string SessionAuthenticationUseLegacyRedirect = "VisualStudio.Services.Authentication.SessionAuthenticationUseLegacyRedirect";

    public enum CustomTenantPickerMessage
    {
      None,
      MarketPlace,
    }
  }
}
