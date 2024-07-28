// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.WindowsMachineDirectoryProperty
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class WindowsMachineDirectoryProperty
  {
    internal const string FullName = "FullName";
    internal const string DistinguishedName = "DistinguishedName";
    internal const string Description = "Description";
    internal const string Name = "Name";
    internal const string ObjectSid = "ObjectSid";
    internal const string ObjectClass = "ObjectClass";
    internal const string MachineName = "MachineName";
    internal const string SamAccountName = "SamAccountName";
    internal static readonly IReadOnlyDictionary<string, string> PropertiesToSearchMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer)
    {
      {
        "DisplayName",
        nameof (FullName)
      },
      {
        nameof (SamAccountName),
        nameof (Name)
      },
      {
        "PrincipalName",
        nameof (Name)
      },
      {
        "SignInAddress",
        nameof (Name)
      }
    };
    internal static readonly IReadOnlyDictionary<string, string> GroupPropertiesToReturnMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer)
    {
      {
        nameof (Description),
        nameof (Description)
      },
      {
        "ScopeName",
        nameof (MachineName)
      },
      {
        "PrincipalName",
        nameof (Name)
      },
      {
        nameof (SamAccountName),
        nameof (Name)
      }
    };
    internal static readonly IReadOnlyDictionary<string, string> UserPropertiesToReturnMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer)
    {
      {
        "DisplayName",
        nameof (FullName)
      },
      {
        "ScopeName",
        nameof (MachineName)
      },
      {
        "PrincipalName",
        nameof (Name)
      },
      {
        "SignInAddress",
        nameof (Name)
      },
      {
        nameof (SamAccountName),
        nameof (Name)
      }
    };
    internal static readonly string[] DirectoryEntityRequiredProperties = new string[2]
    {
      "DisplayName",
      "PrincipalName"
    };
  }
}
