// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.ActiveDirectoryProperty
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class ActiveDirectoryProperty
  {
    internal const string Department = "Department";
    internal const string Description = "Description";
    internal const string DisplayName = "DisplayName";
    internal const string DistinguishedName = "Distinguishedname";
    internal const string Name = "name";
    internal const string Domain = "Domain";
    internal const string ObjectSid = "ObjectSid";
    internal const string Mail = "Mail";
    internal const string MailNickname = "MailNickname";
    internal const string Manager = "Manager";
    internal const string ObjectClass = "objectClass";
    internal const string PhysicalDeliveryOfficeName = "PhysicalDeliveryOfficeName";
    internal const string SamAccountName = "SamAccountName";
    internal const string SN = "SN";
    internal const string Title = "Title";
    internal const string ThumbnailPhoto = "ThumbnailPhoto";
    internal const string UserPrincipalName = "UserPrincipalName";
    internal static readonly IReadOnlyDictionary<string, string> PropertiesToSearchMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer)
    {
      {
        nameof (DisplayName),
        nameof (DisplayName)
      },
      {
        "Distinguishedname",
        "Distinguishedname"
      },
      {
        nameof (Mail),
        nameof (Mail)
      },
      {
        nameof (MailNickname),
        nameof (MailNickname)
      },
      {
        "PrincipalName",
        nameof (UserPrincipalName)
      },
      {
        "SignInAddress",
        nameof (SamAccountName)
      },
      {
        "Surname",
        nameof (SN)
      },
      {
        nameof (SamAccountName),
        nameof (SamAccountName)
      }
    };
    internal static readonly IReadOnlyDictionary<string, string> PropertiesToReturnMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer)
    {
      {
        nameof (DisplayName),
        nameof (DisplayName)
      },
      {
        nameof (Department),
        nameof (Department)
      },
      {
        nameof (Description),
        nameof (Description)
      },
      {
        "Distinguishedname",
        "Distinguishedname"
      },
      {
        "ScopeName",
        nameof (Domain)
      },
      {
        "JobTitle",
        nameof (Title)
      },
      {
        nameof (Mail),
        nameof (Mail)
      },
      {
        nameof (MailNickname),
        nameof (MailNickname)
      },
      {
        nameof (Manager),
        nameof (Manager)
      },
      {
        nameof (PhysicalDeliveryOfficeName),
        nameof (PhysicalDeliveryOfficeName)
      },
      {
        "PrincipalName",
        nameof (UserPrincipalName)
      },
      {
        "SignInAddress",
        nameof (SamAccountName)
      },
      {
        "Surname",
        nameof (SN)
      },
      {
        nameof (SamAccountName),
        nameof (SamAccountName)
      },
      {
        nameof (ThumbnailPhoto),
        nameof (ThumbnailPhoto)
      }
    };
    internal static readonly string[] DirectoryEntityRequiredProperties = new string[2]
    {
      nameof (DisplayName),
      "PrincipalName"
    };
    internal static readonly string[] ActiveDirectoryRequiredProperties = new string[3]
    {
      nameof (ObjectSid),
      "objectClass",
      "Distinguishedname"
    };
  }
}
