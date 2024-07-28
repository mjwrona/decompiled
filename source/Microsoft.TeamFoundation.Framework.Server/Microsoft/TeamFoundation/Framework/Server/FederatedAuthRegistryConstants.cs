// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FederatedAuthRegistryConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class FederatedAuthRegistryConstants
  {
    public static readonly string Root = "/Configuration/FederatedAuth";
    public static readonly string IssueTokenSignature = FederatedAuthRegistryConstants.Root + "/IssueTokenSignature";
    public static readonly string TokenSignatureRequired = FederatedAuthRegistryConstants.Root + "/TokenSignatureRequired";
    public static readonly string AudienceRestrictionUris = FederatedAuthRegistryConstants.Root + "/AudienceRestriction/Uris";
    public static readonly string CertificateValidationMode = FederatedAuthRegistryConstants.Root + "/CertificateValidationMode";
    public static readonly string Enabled = FederatedAuthRegistryConstants.Root + "/Enabled";
    public static readonly string MaxClockSkew = FederatedAuthRegistryConstants.Root + "/MaxClockSkew";
    public static readonly string MetadataLocation = FederatedAuthRegistryConstants.Root + "/MetadataLocation";
    public static readonly string ProxyLocation = FederatedAuthRegistryConstants.Root + "/ProxyLocation";
    public static readonly string UseStrongBox = FederatedAuthRegistryConstants.Root + "/UseStrongBox";
    public static readonly string TrustedStoreLocation = FederatedAuthRegistryConstants.Root + "/TrustedStoreLocation";
    public static readonly string TrustedIssuer = FederatedAuthRegistryConstants.Root + "/TrustedIssuer";
    public static readonly string CookieHandler = FederatedAuthRegistryConstants.Root + "/cookieHandler";
    public static readonly string RequireSsl = FederatedAuthRegistryConstants.CookieHandler + "/RequireSsl";
    public static readonly string SlidingExpirationSeconds = FederatedAuthRegistryConstants.CookieHandler + "/SlidingExpirationSeconds";
    public static readonly string AlternateDomains = FederatedAuthRegistryConstants.CookieHandler + "/AlternateDomains";
    public static readonly string WSFederation = FederatedAuthRegistryConstants.Root + "/wsFederation";
    public static readonly string Issuer = FederatedAuthRegistryConstants.WSFederation + "/Issuer";
    public static readonly string DefaultRealm = FederatedAuthRegistryConstants.WSFederation + "/Realm";
    public static readonly string RequireHttps = FederatedAuthRegistryConstants.WSFederation + "/RequireHttps";
    public static readonly string SignOutLocations = FederatedAuthRegistryConstants.WSFederation + "/SignOutLocations";
    public static readonly string AlternateRealms = FederatedAuthRegistryConstants.WSFederation + "/AlternateRealms";
    public static readonly string ClientSignInOptions = FederatedAuthRegistryConstants.WSFederation + "/ClientSignInOptions";
    public static readonly string SigningCertDrawName = "FedAuthSigningCertificates";
    public static readonly string ServiceIdentitySigningKeyLookupKey = "ServiceIdentitySigningKeys";
  }
}
