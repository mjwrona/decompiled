// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.IdentityConstants
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Common
{
  [GenerateSpecificConstants(null)]
  public static class IdentityConstants
  {
    public const string WindowsType = "System.Security.Principal.WindowsIdentity";
    public const string TeamFoundationType = "Microsoft.TeamFoundation.Identity";
    public const string ClaimsType = "Microsoft.IdentityModel.Claims.ClaimsIdentity";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string Wif45ClaimsIdentityType = "System.Security.Claims.ClaimsIdentity";
    public const string AlternateLoginType = "Microsoft.VisualStudio.Services.Cloud.AlternateLoginIdentity";
    public const string BindPendingIdentityType = "Microsoft.TeamFoundation.BindPendingIdentity";
    public const string ServerTestIdentity = "Microsoft.VisualStudio.Services.Identity.ServerTestIdentity";
    public const string UnauthenticatedIdentityType = "Microsoft.TeamFoundation.UnauthenticatedIdentity";
    public const string ServiceIdentityType = "Microsoft.TeamFoundation.ServiceIdentity";
    public const string AggregateIdentityType = "Microsoft.TeamFoundation.AggregateIdentity";
    public const string ImportedIdentityType = "Microsoft.TeamFoundation.ImportedIdentity";
    public const string UnknownIdentityType = "Microsoft.VisualStudio.Services.Identity.UnknownIdentity";
    public const string CspPartnerIdentityType = "Microsoft.TeamFoundation.Claims.CspPartnerIdentity";
    public const string PermissionLevelDefinitionType = "Microsoft.VisualStudio.Services.PermissionLevel.PermissionLevelIdentity";
    public const string AadServicePrincipalType = "Microsoft.VisualStudio.Services.Claims.AadServicePrincipal";
    public const string GroupScopeType = "Microsoft.VisualStudio.Services.Graph.GraphScope";
    public const string SystemPrefix = "System:";
    public const string System_ServicePrincipal = "System:ServicePrincipal";
    public const string System_WellKnownGroup = "System:WellKnownGroup";
    public const string System_License = "System:License";
    public const string System_Scope = "System:Scope";
    public const string System_CspPartner = "System:CspPartner";
    public const string System_PublicAccess = "System:PublicAccess";
    public const string System_AccessControl = "System:AccessControl";
    public const int MaxIdLength = 256;
    public const int MaxTypeLength = 128;
    public const byte UnknownIdentityTypeId = 255;
    public const byte UnknownSocialTypeId = 255;
    public const int ActiveUniqueId = 0;
    public const string SchemaClassGroup = "Group";
    public const string SchemaClassUser = "User";
    public const string BindPendingSidPrefix = "upn:";
    [GenerateConstant(null)]
    public const string MsaDomain = "Windows Live ID";
    [GenerateConstant(null)]
    public const string GitHubDomain = "github.com";
    public const string DomainQualifiedAccountNameFormat = "{0}\\{1}";
    public const string DomainQualifiedAadServicePrincipalFormat = "{0}\\{1}";
    public const string MsaSidSuffix = "@Live.com";
    public const string AadOidPrefix = "oid:";
    public const string FrameworkIdentityIdentifierDelimiter = ":";
    public const string IdentityDescriptorPartsSeparator = ";";
    public const string IdentityMinimumResourceVersion = "IdentityMinimumResourceVersion";
    public const int DefaultMinimumResourceVersion = -1;
    public const char DomainAccountNameSeparator = '\\';
    public const bool DefaultUseAccountNameAsDirectoryAlias = true;
    public static readonly TimeSpan AuthenticationCredentialValidFromBackDate = TimeSpan.FromMinutes(5.0);
    public const string SwitchHintQueryKey = "switch_hint";
    public const char SwitchToPersonal = 'P';
    public const char SwitchToWork = 'W';
    public const string AllowNonServiceIdentitiesInDeploymentAdminsGroup = "AllowNonServiceIdentitiesInDeploymentAdminsGroup";
    public const byte DefaultResourceVersion = 2;
    [Obsolete]
    public const byte ScopeManifestIssuance = 2;
    [Obsolete]
    public const byte ScopeManifestEnforcementWithInitialGrace = 3;
    [Obsolete]
    public const byte ScopeManifestEnforcementWithoutInitialGrace = 4;
    public const string GlobalScope = "[SERVER]";
    public static readonly Guid LinkedId = new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
    public static readonly IReadOnlyDictionary<string, string> IdentityTypeMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "System.Security.Principal.WindowsIdentity",
        "System.Security.Principal.WindowsIdentity"
      },
      {
        "Microsoft.TeamFoundation.Identity",
        "Microsoft.TeamFoundation.Identity"
      },
      {
        "Microsoft.IdentityModel.Claims.ClaimsIdentity",
        "Microsoft.IdentityModel.Claims.ClaimsIdentity"
      },
      {
        "Microsoft.TeamFoundation.BindPendingIdentity",
        "Microsoft.TeamFoundation.BindPendingIdentity"
      },
      {
        "Microsoft.TeamFoundation.UnauthenticatedIdentity",
        "Microsoft.TeamFoundation.UnauthenticatedIdentity"
      },
      {
        "Microsoft.TeamFoundation.ServiceIdentity",
        "Microsoft.TeamFoundation.ServiceIdentity"
      },
      {
        "Microsoft.TeamFoundation.AggregateIdentity",
        "Microsoft.TeamFoundation.AggregateIdentity"
      },
      {
        "Microsoft.VisualStudio.Services.Identity.ServerTestIdentity",
        "Microsoft.VisualStudio.Services.Identity.ServerTestIdentity"
      },
      {
        "Microsoft.TeamFoundation.ImportedIdentity",
        "Microsoft.TeamFoundation.ImportedIdentity"
      },
      {
        "Microsoft.VisualStudio.Services.Graph.GraphScope",
        "Microsoft.VisualStudio.Services.Graph.GraphScope"
      },
      {
        "Microsoft.TeamFoundation.Claims.CspPartnerIdentity",
        "Microsoft.TeamFoundation.Claims.CspPartnerIdentity"
      },
      {
        "Microsoft.VisualStudio.Services.Claims.AadServicePrincipal",
        "Microsoft.VisualStudio.Services.Claims.AadServicePrincipal"
      },
      {
        "System:ServicePrincipal",
        "System:ServicePrincipal"
      },
      {
        "System:License",
        "System:License"
      },
      {
        "System:Scope",
        "System:Scope"
      },
      {
        "Microsoft.VisualStudio.Services.PermissionLevel.PermissionLevelIdentity",
        "Microsoft.VisualStudio.Services.PermissionLevel.PermissionLevelIdentity"
      }
    };
    public const string AADSPTokenValidationEnabled = "Identity.AADSPTokenValidationEnabled.M207";
    public const string AadSPCrudOperationsEnabled = "Identity.AADSPCrudOperationsEnabled.M207";
    public const string AadSPGraphOperationsEnabled = "Identity.AadSPGraphOperationsEnabled.M209";
    public const string SearchingForServicePrincipalsEnabled = "Identity.SearchingForServicePrincipalsEnabled";
    public const string UserHubSupportServicePrincipalsEnabled = "Identity.UserHubSupportServicePrincipalsEnabled";
    public const string OrgGroupMembershipSupportServicePrincipalsEnabled = "Identity.OrgGroupMembershipSupportServicePrincipalsEnabled";
    public const string SPSyncGroupMembershipsConfig = "Identity.AADSPSyncGroupMembershipsEnabled.M212";
    public const string GroupRulesSupportForServicePrincipalEnabled = "Identity.ServicePrincipalGroupRules.M214";
    public const string FixServicePrincipalsWithUnknownMetaType = "Identity.FixServicePrincipalsWithUnknownMetaType.M214";
    public const string RestrictionOfCertainApisForServicePrincipalsEnabled = "Identity.RestrictionOfCertainApisForSPsEnabled.215";
    public const string RequireAadBackedOrgAttributeEnalbed = "Identity.RequireAadBackedOrgAttributeEnalbed.M216";
    public const string SPPermissionsOperationsInUIEnabled = "Identity.SPPermissionsOperationsInUIEnabled";
    public const string MergeUserAndSPPermissionsInUIEnabled = "Identity.MergeUserAndSPPermissionsInUIEnabled";
    public const string DisableServicePrincipalOrgOwners = "VisualStudio.Services.Organization.DisableServicePrincipalOrgOwners";
    public const string ObjectLevelSecurityMembersSupportServicePrincipalsEnabled = "Identity.ObjectLevelSecurityMembersSupportServicePrincipalsEnabled.M218";
    public const string BranchPolicyStatusChecksSupportServicePrincipalsEnabled = "Identity.BranchPolicyStatusChecksSupportServicePrincipalsEnabled.M218";
    public const string ArtifactsPermissionsSupportServicePrincipalsEnabled = "Identity.ArtifactsPermissionsSupportServicePrincipalsEnabled.M220";
    public const string EmsPermissionsSupportServicePrincipalsEnabled = "Identity.EmsPermissionsSupportServicePrincipalsEnabled.M220";
    public const string AllowedSubjectTypesEnabled = "Identity.AllowedSubjectTypesEnabled.M226";
    public const string AadServicePrincipalRestrictionWhiteList = "Identity.AadServicePrincipalRestrictionWhiteList.LongTerm";
    public const string AadServicePrincipalRestrictionWhiteListCategorySessionToken = "WhiteList.SessionToken";
    public const string UseIPagedScopedIdentityReaderInterfaceEnabled = "Identity.UseIPagedScopedIdentityReaderInterface.M209";
    public const string UseMemberEntitlementDataBuilderEnabled = "Identity.UseMemberEntitlementDataBuilder.M213";
    public const string ResolveDisconnectedUsersApiEnabled = "Identity.ResolveDisconnectedUsersApiAvailable.M211";

    public static class EtwIdentityProviderName
    {
      public const string Aad = "Aad";
      public const string Msa = "Msa";
      public const string Vsts = "Vsts";
    }

    public static class EtwIdentityCategory
    {
      public const string AuthenticatedIdentity = "AuthenticatedIdentity";
      public const string UnauthenticatedIdentity = "UnauthenticatedIdentity";
      public const string ServiceIdentity = "ServiceIdentity";
      public const string UnexpectedIdentityType = "UnexpectedIdentityType";
    }
  }
}
