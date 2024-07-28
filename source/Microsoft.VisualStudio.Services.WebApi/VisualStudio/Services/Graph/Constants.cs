// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Constants
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class Constants
  {
    public static readonly IReadOnlyDictionary<string, string> SubjectTypeMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "aad",
        "aad"
      },
      {
        "msa",
        "msa"
      },
      {
        "unusr",
        "unusr"
      },
      {
        "aadgp",
        "aadgp"
      },
      {
        "vssgp",
        "vssgp"
      },
      {
        "ungrp",
        "ungrp"
      },
      {
        "bnd",
        "bnd"
      },
      {
        "win",
        "win"
      },
      {
        "uauth",
        "uauth"
      },
      {
        "svc",
        "svc"
      },
      {
        "agg",
        "agg"
      },
      {
        "imp",
        "imp"
      },
      {
        "tst",
        "tst"
      },
      {
        "scp",
        "scp"
      },
      {
        "csp",
        "csp"
      },
      {
        "s2s",
        "s2s"
      },
      {
        "slic",
        "slic"
      },
      {
        "spa",
        "spa"
      },
      {
        "sace",
        "sace"
      },
      {
        "sscp",
        "sscp"
      },
      {
        "acs",
        "acs"
      },
      {
        "ukn",
        "ukn"
      },
      {
        "aadsp",
        "aadsp"
      }
    };
    public static readonly IReadOnlyDictionary<string, string> SocialTypeMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "ghb",
        "ghb"
      },
      {
        "ukn",
        "ukn"
      }
    };
    public const int MaximumRestResultSize = 500;
    public const string JsonPatchMediaType = "application/json-patch+json";
    public const string JsonPatchOperationPathPrefix = "/";
    public const char SubjectListSeparator = ',';
    public const char SubjectDescriptorPartsSeparator = '.';
    public const char SocialListSeparator = ',';
    public const char SocialDescriptorPartsSeparator = '.';
    public const string SocialDescriptorPrefix = "@";
    public const string ForceFilterIdentities = "x-ForceFilterIdentities";
    public const int ResolveDisconnectedUsersApiLimit = 20;

    [GenerateSpecificConstants(null)]
    public static class SubjectKind
    {
      [GenerateConstant(null)]
      public const string Group = "group";
      public const string Scope = "scope";
      [GenerateConstant(null)]
      public const string User = "user";
      public const string SystemSubject = "systemSubject";
      [GenerateConstant(null)]
      public const string ServicePrincipal = "servicePrincipal";
    }

    [GenerateSpecificConstants(null)]
    public static class SubjectType
    {
      [GenerateConstant(null)]
      public const string AadUser = "aad";
      [GenerateConstant(null)]
      public const string MsaUser = "msa";
      public const string UnknownUser = "unusr";
      [GenerateConstant(null)]
      public const string AadGroup = "aadgp";
      [GenerateConstant(null)]
      public const string AadServicePrincipal = "aadsp";
      [GenerateConstant(null)]
      public const string VstsGroup = "vssgp";
      public const string UnknownGroup = "ungrp";
      [GenerateConstant(null)]
      public const string BindPendingUser = "bnd";
      public const string WindowsIdentity = "win";
      public const string UnauthenticatedIdentity = "uauth";
      public const string ServiceIdentity = "svc";
      public const string AggregateIdentity = "agg";
      public const string ImportedIdentity = "imp";
      public const string ServerTestIdentity = "tst";
      public const string GroupScopeType = "scp";
      public const string CspPartnerIdentity = "csp";
      public const string SystemServicePrincipal = "s2s";
      public const string SystemLicense = "slic";
      public const string SystemScope = "sscp";
      public const string SystemCspPartner = "scsp";
      public const string SystemPublicAccess = "spa";
      public const string SystemAccessControl = "sace";
      public const string AcsServiceIdentity = "acs";
      public const string Unknown = "ukn";
    }

    [GenerateSpecificConstants(null)]
    public static class SocialType
    {
      public const string GitHub = "ghb";
      public const string Unknown = "ukn";
    }

    public static class ScopeUpdateFields
    {
      public const string Name = "name";
    }

    public static class GroupUpdateFields
    {
      public const string DisplayName = "displayName";
      public const string Description = "description";
    }

    public static class Links
    {
      public const string Self = "self";
      public const string Memberships = "memberships";
      public const string MembershipState = "membershipState";
      public const string StorageKey = "storageKey";
      public const string Groups = "groups";
      public const string Descriptor = "descriptor";
      public const string Subject = "subject";
      public const string Member = "member";
      public const string Conainer = "container";
      public const string Avatar = "avatar";
    }

    [GenerateSpecificConstants(null)]
    public static class OriginName
    {
      public const string ActiveDirectory = "ad";
      [GenerateConstant(null)]
      public const string AzureActiveDirectory = "aad";
      [GenerateConstant(null)]
      public const string MicrosoftAccount = "msa";
      [GenerateConstant(null)]
      public const string VisualStudioTeamServices = "vsts";
      [GenerateConstant(null)]
      public const string GitHubDirectory = "ghb";
    }

    public static class FederatedProviderName
    {
      public const string GitHub = "github.com";
    }

    public static class TraversalDepth
    {
      public const int Direct = 1;
      public const int Expanded = -1;
    }

    [GenerateSpecificConstants(null)]
    public static class UserMetaType
    {
      [GenerateConstant(null)]
      public const string Member = "member";
      [GenerateConstant(null)]
      public const string Guest = "guest";
      public const string CompanyAdministrator = "companyAdministrator";
      public const string HelpdeskAdministrator = "helpdeskAdministrator";
      [GenerateConstant(null)]
      public const string Application = "application";
      [GenerateConstant(null)]
      public const string ManagedIdentity = "managedIdentity";
      public const string Unknown = "unknown";
    }

    internal static class SubjectDescriptorPolicies
    {
      internal const int MaxSubjectTypeLength = 5;
      internal const int MinSubjectTypeLength = 3;
      internal const int MinSubjectDescriptorStringLength = 6;
      internal const int MaxIdentifierLength = 256;
    }

    internal static class SocialDescriptorPolicies
    {
      internal const int MaxSocialTypeLength = 4;
      internal const int MinSocialTypeLength = 3;
      internal const int MinSocialDescriptorStringLength = 6;
      internal const int MaxIdentifierLength = 256;
    }

    internal static class Version
    {
      internal const int Unspecified = -1;
    }
  }
}
