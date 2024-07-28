// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ContributionRestriction
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  public static class ContributionRestriction
  {
    public const string AnonymousClaim = "anonymous";
    public const string MemberClaim = "member";
    public const string PublicClaim = "public";
    private static readonly char[] sm_restrictedToAndSeparator = new char[1]
    {
      '+'
    };
    private const string c_restrictedToClaimsKey = "restrictedToClaims";
    private static readonly string[] sm_defaultClaims = new string[1]
    {
      "member"
    };

    public static IEnumerable<string> GetDefaultValues() => (IEnumerable<string>) ContributionRestriction.sm_defaultClaims;

    public static List<List<string>> ParseRestrictedClaimGroups(Contribution contribution)
    {
      List<List<string>> restrictedClaimGroups = contribution.GetAssociatedObject<List<List<string>>>("restrictedToClaims");
      if (restrictedClaimGroups == null)
      {
        restrictedClaimGroups = new List<List<string>>();
        if (contribution.RestrictedTo != null)
        {
          foreach (string str in contribution.RestrictedTo)
          {
            List<string> stringList = new List<string>((IEnumerable<string>) str.Split(ContributionRestriction.sm_restrictedToAndSeparator));
            restrictedClaimGroups.Add(stringList);
          }
        }
        contribution.SetAssociatedObject<List<List<string>>>("restrictedToClaims", restrictedClaimGroups);
      }
      return restrictedClaimGroups;
    }

    public static bool HasAnyClaim(Contribution contribution, params string[] claims)
    {
      bool flag = false;
      foreach (IEnumerable<string> restrictedClaimGroup in ContributionRestriction.ParseRestrictedClaimGroups(contribution))
      {
        if (restrictedClaimGroup.Intersect<string>((IEnumerable<string>) claims, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Any<string>())
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    internal static IEnumerable<string> GetValidatedValues(
      IEnumerable<string> restrictedToValues,
      IEnumerable<string> defaultRestrictedToValues)
    {
      List<string> source = new List<string>();
      if (restrictedToValues != null)
        source.AddRange(restrictedToValues.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
      return !source.Any<string>() ? defaultRestrictedToValues : (IEnumerable<string>) source;
    }
  }
}
