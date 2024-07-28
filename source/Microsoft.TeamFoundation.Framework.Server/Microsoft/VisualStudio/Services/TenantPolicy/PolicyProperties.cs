// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.PolicyProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.TenantPolicy
{
  [GenerateAllConstants(null)]
  public static class PolicyProperties
  {
    public const string AllowedUsersAndGroupObjectIds = "AllowedUsersAndGroupObjectIds";
    public const string MaxPatLifespanInDays = "MaxPatLifespanInDays";
    public const string ErrorMessage = "ErrorMessage";
    public static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> TenantPolicyProperties = (IReadOnlyDictionary<string, IReadOnlyList<string>>) new Dictionary<string, IReadOnlyList<string>>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase)
    {
      {
        "TenantPolicy.OrganizationCreationRestriction",
        (IReadOnlyList<string>) new List<string>()
        {
          nameof (AllowedUsersAndGroupObjectIds),
          nameof (ErrorMessage)
        }
      },
      {
        "TenantPolicy.RestrictGlobalPersonalAccessToken",
        (IReadOnlyList<string>) new List<string>()
        {
          nameof (AllowedUsersAndGroupObjectIds)
        }
      },
      {
        "TenantPolicy.RestrictFullScopePersonalAccessToken",
        (IReadOnlyList<string>) new List<string>()
        {
          nameof (AllowedUsersAndGroupObjectIds)
        }
      },
      {
        "TenantPolicy.RestrictPersonalAccessTokenLifespan",
        (IReadOnlyList<string>) new List<string>()
        {
          nameof (MaxPatLifespanInDays),
          nameof (AllowedUsersAndGroupObjectIds)
        }
      },
      {
        "TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocation",
        (IReadOnlyList<string>) new List<string>()
      }
    };
  }
}
