// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccountPropertyConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [GenerateSpecificConstants(null)]
  public static class AccountPropertyConstants
  {
    public const string SocialCodePropertyName = "Microsoft.TeamFoundation.Account.SocialCode";
    public const string MaxCollectionsPropertyName = "Microsoft.TeamFoundation.Account.MaxCollections";
    public const string ServiceCreatorId = "Microsoft.VisualStudio.Services.Account.ServiceCreatorId";
    public const string PrecreatedAccount = "Microsoft.VisualStudio.Services.Account.PrecreatedAccount";
    public const string PrecreatePoolId = "Microsoft.VisualStudio.Services.Account.PrecreatePoolId";
    public const string TfsSignupRequired = "Microsoft.VisualStudio.Services.Account.TfsSignupRequired";
    [GenerateConstant(null)]
    public const string TfsAccountRegion = "Microsoft.VisualStudio.Services.Account.TfsAccountRegion";
    public const string TfsAccountRegionDisplayName = "Microsoft.VisualStudio.Services.Account.TfsAccountRegionDisplayName";
    public const string TfsAccountGeographyDisplayName = "Microsoft.VisualStudio.Services.Account.TfsAccountGeographyDisplayName";
    [GenerateConstant(null)]
    public const string SignupEntryPoint = "Microsoft.VisualStudio.Services.Account.SignupEntryPoint";
    public const string ConnectedServerId = "Microsoft.VisualStudio.Services.Account.ConnectedServerId";
    public const string ConnectedServerName = "Microsoft.VisualStudio.Services.Account.ConnectedServerName";
    public const string ConnectedServerHostName = "Microsoft.VisualStudio.Services.Account.ConnectedServerHostName";
    public const string ConnectedServerHostId = "Microsoft.VisualStudio.Services.Account.ConnectedServerHostId";
    public const string BrandNew = "Microsoft.VisualStudio.Services.Account.BrandNew";
    public const string ForcedAccountId = "Microsoft.VisualStudio.Services.Account.ForcedAccountId";
    public const string TenantId = "Microsoft.VisualStudio.Services.Account.TenantId";
    public const string DataImportAccount = "Microsoft.VisualStudio.Services.Account.DataImport";
    public static readonly string AuthenticationCredentialValidFrom = "Microsoft.VisualStudio.Services.Account.AuthenticationValidFrom";
    public static readonly string TrialStartDate = "Microsoft.VisualStudio.Services.Account.SystemProperty.TrialStartDate";
    public static readonly string TrialEndDate = "Microsoft.VisualStudio.Services.Account.SystemProperty.TrialEndDate";
    public static readonly string Internal = "Microsoft.VisualStudio.Services.Account.SystemProperty.Internal";
    [GenerateConstant(null)]
    public static readonly string SoftDeletedAccountName = "Microsoft.VisualStudio.Services.Account.SystemProperty.SoftDeletedAccountName";
    public static readonly string AccountSoftDeletedDate = "Microsoft.VisualStudio.Services.Account.SystemProperty.AccountSoftDeletedDate";
    public const string DissallowBasicAuthentication = "Microsoft.VisualStudio.Services.Account.DisallowBasicAuthentication";
    public const string DissallowOAuthAuthentication = "Microsoft.VisualStudio.Services.Account.DisallowOAuthAuthentication";
    public const string SSHDisabled = "Microsoft.VisualStudio.Services.Account.DisallowSSH";
    public const string GuestUserDisabledProperty = "Microsoft.VisualStudio.Services.Account.GuestUserDisabled";
    public const string AuthorizationEnforcementPolicy = "Microsoft.VisualStudio.Services.Account.AuthorizationEnforcementPolicy";
    public const string FinalHostState = "Microsoft.VisualStudio.Services.Account.FinalHostState";
    public const string FinalHostStatusReason = "Microsoft.VisualStudio.Services.Account.FinalHostStatusReason";
    public const string UseAccountIdAsOrganizationId = "Microsoft.VisualStudio.Services.Account.UseAccountIdAsOrganizationId";
    public const string ServiceUrlPrefix = "Microsoft.VisualStudio.Services.Account.ServiceUrl.";
    [GenerateConstant(null)]
    public const string CampaignId = "Microsoft.VisualStudio.Services.Account.CampaignId";
    public const string AllowAnonymousAccess = "Microsoft.VisualStudio.Services.Account.AllowAnonymousAccess";
    public const string TimeZone = "Microsoft.VisualStudio.Services.Account.TimeZone";
    public static readonly Dictionary<string, string> KnownOldAccountPropertyToOrganizationAccountPropertyMap = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        AccountPropertyConstants.AuthenticationCredentialValidFrom,
        "Policy.AuthenticationCredentialValidFrom"
      },
      {
        "Microsoft.VisualStudio.Services.Account.DisallowBasicAuthentication",
        "Policy.DisallowBasicAuthentication"
      },
      {
        "Microsoft.VisualStudio.Services.Account.DisallowOAuthAuthentication",
        "Policy.DisallowOAuthAuthentication"
      },
      {
        "Microsoft.VisualStudio.Services.Account.GuestUserDisabled",
        "Policy.DisallowAadGuestUserAccess"
      },
      {
        "Microsoft.VisualStudio.Services.Account.DisallowSSH",
        "Policy.DisallowSecureShell"
      },
      {
        "Microsoft.VisualStudio.Services.Account.AuthorizationEnforcementPolicy",
        "Policy.EnforceAadAuthorization"
      },
      {
        "Microsoft.VisualStudio.Services.Account.AllowAnonymousAccess",
        "Policy.AllowAnonymousAccess"
      },
      {
        AccountPropertyConstants.Internal,
        "Policy.IsInternal"
      },
      {
        "Microsoft.VisualStudio.Services.Account.DataImport",
        "SystemProperty.OnPremImportId"
      },
      {
        AccountPropertyConstants.TrialStartDate,
        "SystemProperty.TrialStartDate"
      },
      {
        AccountPropertyConstants.TrialEndDate,
        "SystemProperty.TrialEndDate"
      },
      {
        AccountPropertyConstants.AccountSoftDeletedDate,
        "SystemProperty.LastLogicalDeletedDate"
      },
      {
        AccountPropertyConstants.SoftDeletedAccountName,
        "SystemProperty.PreviousName"
      },
      {
        "Microsoft.VisualStudio.Services.Account.TimeZone",
        "SystemProperty.TimeZone"
      }
    };
    public static readonly Dictionary<string, string> KnownOrganizationAccountPropertyToOldAccountPropertyMap = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Policy.AuthenticationCredentialValidFrom",
        AccountPropertyConstants.AuthenticationCredentialValidFrom
      },
      {
        "Policy.DisallowBasicAuthentication",
        "Microsoft.VisualStudio.Services.Account.DisallowBasicAuthentication"
      },
      {
        "Policy.DisallowOAuthAuthentication",
        "Microsoft.VisualStudio.Services.Account.DisallowOAuthAuthentication"
      },
      {
        "Policy.DisallowAadGuestUserAccess",
        "Microsoft.VisualStudio.Services.Account.GuestUserDisabled"
      },
      {
        "Policy.DisallowSecureShell",
        "Microsoft.VisualStudio.Services.Account.DisallowSSH"
      },
      {
        "Policy.EnforceAadAuthorization",
        "Microsoft.VisualStudio.Services.Account.AuthorizationEnforcementPolicy"
      },
      {
        "Policy.AllowAnonymousAccess",
        "Microsoft.VisualStudio.Services.Account.AllowAnonymousAccess"
      },
      {
        "Policy.IsInternal",
        AccountPropertyConstants.Internal
      },
      {
        "Policy.IsInternal.Enforce",
        AccountPropertyConstants.Internal
      },
      {
        "SystemProperty.OnPremImportId",
        "Microsoft.VisualStudio.Services.Account.DataImport"
      },
      {
        "SystemProperty.TrialStartDate",
        AccountPropertyConstants.TrialStartDate
      },
      {
        "SystemProperty.TrialEndDate",
        AccountPropertyConstants.TrialEndDate
      },
      {
        "SystemProperty.LastLogicalDeletedDate",
        AccountPropertyConstants.AccountSoftDeletedDate
      },
      {
        "SystemProperty.PreviousName",
        AccountPropertyConstants.SoftDeletedAccountName
      },
      {
        "SystemProperty.TimeZone",
        "Microsoft.VisualStudio.Services.Account.TimeZone"
      }
    };
    internal static readonly HashSet<string> OldAccountPropertiesThatBecomeCoreOrganizationAccountProperties = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "Microsoft.VisualStudio.Services.Account.TenantId",
      "Microsoft.VisualStudio.Services.Account.TfsAccountRegion"
    };
    internal static readonly HashSet<string> OldAccountPropertiesThatAreComputedProperties = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "Microsoft.VisualStudio.Services.Account.TfsAccountRegionDisplayName",
      "Microsoft.VisualStudio.Services.Account.TfsAccountGeographyDisplayName"
    };

    public static class SystemProperty
    {
      public const string Namespace = "Microsoft.VisualStudio.Services.Account.SystemProperty";
      public const char NamespaceSeparator = '.';
      public const string TrialStartDatePropertyName = "TrialStartDate";
      public const string TrialEndDatePropertyName = "TrialEndDate";
      public const string InternalFlagPropertyName = "Internal";
      public const string SoftDeletedAccountName = "SoftDeletedAccountName";
      public const string AccountSoftDeletedDate = "AccountSoftDeletedDate";
    }
  }
}
