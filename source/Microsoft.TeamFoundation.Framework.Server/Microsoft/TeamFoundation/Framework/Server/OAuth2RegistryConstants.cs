// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2RegistryConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class OAuth2RegistryConstants
  {
    internal static readonly string Root = "/Configuration/OAuth";
    public static readonly string S2SRoot = "/Configuration/OAuthS2S";
    internal static readonly string AADRoot = "/Configuration/OAuthAAD";
    public static readonly string SigningKeys = OAuth2RegistryConstants.Root + "/SigningKeys";
    internal static readonly string TrustedIssuers = OAuth2RegistryConstants.Root + "/TrustedIssuers";
    internal static readonly string TrustedIssuerClaim = OAuth2RegistryConstants.Root + "/TrustedIssuerClaim";
    internal static readonly string AllowedAudiences = OAuth2RegistryConstants.Root + "/AllowedAudiences";
    public static readonly string ClockSkewInSeconds = OAuth2RegistryConstants.Root + "/ClockSkewInSeconds";
    internal static readonly string ClientAuthEnabled = OAuth2RegistryConstants.Root + "/AuthEnabled";
    internal static readonly string IsDeploymentLevelOnlyService = OAuth2RegistryConstants.Root + "/IsDeploymentLevelOnlyService";
    internal static readonly string OAuthSigningKeyExpiryInMinutes = OAuth2RegistryConstants.Root + "/SigningKeysConfiguration/ExpiryInMinutes";
    internal static readonly string S2SClientEnabled = OAuth2RegistryConstants.S2SRoot + "/ClientEnabled";
    internal static readonly string S2SAuthEnabled = OAuth2RegistryConstants.S2SRoot + "/AuthEnabled";
    public static readonly string S2STenantId = OAuth2RegistryConstants.S2SRoot + "/TenantId";
    internal static readonly string FirstPartyS2STenantId = OAuth2RegistryConstants.S2SRoot + "/FirstPartyS2STenantId";
    public static readonly string S2STenantDomain = OAuth2RegistryConstants.S2SRoot + "/TenantDomain";
    internal static readonly string FirstPartyS2STenantDomain = OAuth2RegistryConstants.S2SRoot + "/FirstPartyS2STenantDomain";
    public static readonly string S2SIssuanceEndpoint = OAuth2RegistryConstants.S2SRoot + "/IssuanceEndpoint";
    internal static readonly string FirstPartyS2SIssuanceEndpoint = OAuth2RegistryConstants.S2SRoot + "/FirstPartyS2SIssuanceEndpoint";
    public static readonly string S2SIssuer = OAuth2RegistryConstants.S2SRoot + "/Issuer";
    internal static readonly string FirstPartyS2SIssuer = OAuth2RegistryConstants.S2SRoot + "/FirstPartyS2SIssuer";
    internal static readonly string S2SDefaultIssuer = "https://sts.windows.net/{0}/";
    internal static readonly string S2SFallbackIssuanceEndpoint = OAuth2RegistryConstants.S2SRoot + "/FallbackIssuanceEndpoint";
    internal static readonly string S2SPrimaryServicePrincipal = OAuth2RegistryConstants.S2SRoot + "/PrimaryServicePrincipal";
    internal static readonly string S2SRegisteredPrincipalsTTL = OAuth2RegistryConstants.S2SRoot + "/RegisteredPrincipalsTTL";
    internal static readonly string S2SDefaultIssuanceEndpoint = "https://login.microsoftonline.com/{0}/oauth2/token";
    internal static readonly string S2SDefaultFallbackIssuanceEndpoint = "https://accounts.accesscontrol.windows.net/{0}/tokens/OAuth/2";
    internal static readonly string S2SSyncServicePrincipals = OAuth2RegistryConstants.S2SRoot + "/SyncServicePrincipals";
    public static readonly string S2SSigningCertDrawerName = "AADSigningCertificates";
    internal static readonly string S2SWellKnownServicePrincipalName = "00000001-0000-0000-c000-000000000000";
    internal static readonly string S2SPrincipalSettingPrefix = "S2SPrincipal.";
    internal static readonly string S2SDisableAADTestSlice = OAuth2RegistryConstants.S2SRoot + "/DisableAADTestSlice";
    public static readonly string S2SL2TenantId = OAuth2RegistryConstants.S2SRoot + "/L2TenantId";
    internal static readonly string S2SUseSecondarySigningCertificate = OAuth2RegistryConstants.S2SRoot + "/UseSecondarySigningCertificate";
    internal static readonly string S2SSwitchToPrimaryAfter = OAuth2RegistryConstants.S2SRoot + "/SwitchToPrimaryAfter";
    internal static readonly string FirstPartyIntAADApplicationId = "3ac84afb-ab04-4ec4-a2eb-8a2abf4be447";
    internal static readonly string FirstPartyProdAADApplicationId = "c5dfc2b3-0feb-40dd-8f90-1b82ceaede71";
    internal static readonly string AADAuthEnabled = OAuth2RegistryConstants.AADRoot + "/AuthEnabled";
    internal static readonly string AADAuthority = OAuth2RegistryConstants.AADRoot + "/Authority";
    internal static readonly string AADCreateTenants = OAuth2RegistryConstants.AADRoot + "/CreateTenants";
    public static readonly string AADCertThumbprint = OAuth2RegistryConstants.AADRoot + "/CertThumbprint";
    public static readonly string IntAADCertThumbprint = OAuth2RegistryConstants.AADRoot + "/IntCertThumbprint";
    internal static readonly string AADGraphResource = OAuth2RegistryConstants.AADRoot + "/GraphResource";
    internal static readonly string EnableMsaPassthroughForTenantPicker = OAuth2RegistryConstants.AADRoot + "/EnableMsaPassthroughForTenantPicker";
    internal static readonly string BlockedAADAppIds = OAuth2RegistryConstants.AADRoot + "/BlockedAADAppIds";
    internal static readonly string MsaPassthroughBlockedAADAppIds = OAuth2RegistryConstants.AADRoot + "/MsaPassthroughBlockedAADAppIds";
    internal static readonly string OnBehalfOfAllowedAADAppIds = OAuth2RegistryConstants.AADRoot + "/OnBehalfOfAllowedAADAppIds";
    internal static readonly string AADDefaultGraphResource = "https://management.core.windows.net/";
  }
}
