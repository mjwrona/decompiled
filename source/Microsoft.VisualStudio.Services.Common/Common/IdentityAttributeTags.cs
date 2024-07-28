// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.IdentityAttributeTags
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class IdentityAttributeTags
  {
    public const string WildCard = "*";
    public const string AccountName = "Account";
    public const string Alias = "Alias";
    public const string CrossProject = "CrossProject";
    public const string Description = "Description";
    public const string Disambiguation = "Disambiguation";
    public const string DistinguishedName = "DN";
    public const string Domain = "Domain";
    public const string GlobalScope = "GlobalScope";
    public const string MailAddress = "Mail";
    public const string RestrictedVisible = "RestrictedVisible";
    public const string SchemaClassName = "SchemaClassName";
    public const string ScopeName = "ScopeName";
    public const string SecurityGroup = "SecurityGroup";
    public const string SpecialType = "SpecialType";
    public const string ScopeId = "ScopeId";
    public const string ScopeType = "ScopeType";
    public const string LocalScopeId = "LocalScopeId";
    public const string SecuringHostId = "SecuringHostId";
    public const string VirtualPlugin = "VirtualPlugin";
    public const string ProviderDisplayName = "ProviderDisplayName";
    public const string IsGroupDeleted = "IsGroupDeleted";
    public const string Cuid = "CUID";
    public const string CuidState = "CUIDState";
    public const string Puid = "PUID";
    public const string Oid = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    public const string ConsumerPuid = "ConsumerPUID";
    public const string ComplianceValidated = "ComplianceValidated";
    public const string AuthenticationCredentialValidFrom = "AuthenticationCredentialValidFrom";
    public const string MetadataUpdateDate = "MetadataUpdateDate";
    public const string DirectoryAlias = "DirectoryAlias";
    public const string CacheMaxAge = "CacheMaxAge";
    public const string ServiceStorageKey = "http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid";
    public const string ProvData = "prov_data";
    public const string AadRefreshToken = "vss:AadRefreshToken";
    public const string AadRefreshTokenUpdated = "Microsoft.VisualStudio.Aad.AadRefreshTokenUpdateDate";
    public const string AadUserPrincipalName = "AadUserPrincipalName";
    public const string AcsIdentityProvider = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";
    public const string AadIdentityProvider = "http://schemas.microsoft.com/identity/claims/identityprovider";
    public const string IdentityProviderClaim = "http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider";
    public const string NameIdentifierClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    public const string TenantIdentifierClaim = "http://schemas.microsoft.com/identity/claims/tenantid";
    public const string AadTenantDisambiguationClaim = "tenant_disambiguate";
    public const string AadMsaPassthroughClaim = "msapt";
    public const string AppidClaim = "appid";
    public const string ServicePrincipalMetaTypeClaim = "spmetatype";
    public const string IdentityTypeClaim = "IdentityTypeClaim";
    public const string IsClientClaim = "IsClient";
    public const string ConfirmedNotificationAddress = "ConfirmedNotificationAddress";
    public const string CustomNotificationAddresses = "CustomNotificationAddresses";
    public const string IsDeletedInOrigin = "IsDeletedInOrigin";
    public const string ApplicationId = "ApplicationId";
    public const string ImageId = "Microsoft.TeamFoundation.Identity.Image.Id";
    public const string ImageData = "Microsoft.TeamFoundation.Identity.Image.Data";
    public const string ImageType = "Microsoft.TeamFoundation.Identity.Image.Type";
    public const string ImageUploadDate = "Microsoft.TeamFoundation.Identity.Image.UploadDate";
    public const string CandidateImageData = "Microsoft.TeamFoundation.Identity.CandidateImage.Data";
    public const string CandidateImageUploadDate = "Microsoft.TeamFoundation.Identity.CandidateImage.UploadDate";
    public const string LastAccessedTime = "LastAccessedTime";
    public const string UserId = "UserId";
    [Obsolete]
    public const string EmailConfirmationSendDates = "EmailConfirmationSendDates";
    [Obsolete]
    public const string MsdnLicense = "MSDNLicense";
    [Obsolete]
    public const string BasicAuthPwdKey = "Microsoft.TeamFoundation.Identity.BasicAuthPwd";
    [Obsolete]
    public const string BasicAuthSaltKey = "Microsoft.TeamFoundation.Identity.BasicAuthSalt";
    [Obsolete]
    public const string BasicAuthAlgorithm = "Microsoft.TeaFoundation.Identity.BasicAuthAlgorithm";
    [Obsolete]
    public const string BasicAuthFailures = "Microsoft.TeaFoundation.Identity.BasicAuthFailures";
    [Obsolete]
    public const string BasicAuthDisabled = "Microsoft.TeaFoundation.Identity.BasicAuthDisabled";
    [Obsolete]
    public const string BasicAuthPasswordChanges = "Microsoft.TeamFoundation.Identity.BasicAuthSettingsChanges";
    public static readonly HashSet<string> ReadOnlyProperties = new HashSet<string>((IEnumerable<string>) new string[34]
    {
      "Account",
      nameof (Alias),
      nameof (ComplianceValidated),
      nameof (CrossProject),
      nameof (Description),
      nameof (Disambiguation),
      "DN",
      nameof (Domain),
      nameof (GlobalScope),
      "Mail",
      nameof (RestrictedVisible),
      nameof (SchemaClassName),
      nameof (ScopeName),
      nameof (SecurityGroup),
      nameof (SpecialType),
      nameof (ScopeId),
      nameof (ScopeType),
      nameof (LocalScopeId),
      nameof (SecuringHostId),
      "CUID",
      "CUIDState",
      "PUID",
      nameof (VirtualPlugin),
      "http://schemas.microsoft.com/identity/claims/objectidentifier",
      "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
      "http://schemas.microsoft.com/identity/claims/identityprovider",
      "tenant_disambiguate",
      "msapt",
      "http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider",
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
      "IsClient",
      nameof (UserId),
      nameof (CacheMaxAge),
      nameof (IsGroupDeleted)
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    public static readonly HashSet<string> GroupReadOnlyProperties = new HashSet<string>((IEnumerable<string>) new string[32]
    {
      nameof (Alias),
      nameof (ComplianceValidated),
      nameof (CrossProject),
      nameof (Disambiguation),
      "DN",
      nameof (Domain),
      nameof (GlobalScope),
      "Mail",
      nameof (RestrictedVisible),
      nameof (SchemaClassName),
      nameof (ScopeName),
      nameof (SecurityGroup),
      nameof (SpecialType),
      nameof (ScopeId),
      nameof (ScopeType),
      nameof (LocalScopeId),
      nameof (SecuringHostId),
      "CUID",
      "CUIDState",
      "PUID",
      nameof (VirtualPlugin),
      "http://schemas.microsoft.com/identity/claims/objectidentifier",
      "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
      "http://schemas.microsoft.com/identity/claims/identityprovider",
      "tenant_disambiguate",
      "msapt",
      "http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider",
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
      "IsClient",
      nameof (UserId),
      nameof (CacheMaxAge),
      nameof (IsGroupDeleted)
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    [Obsolete]
    public static readonly ISet<string> WhiteListedProperties = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  }
}
