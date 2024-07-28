// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CultureResolution
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class CultureResolution
  {
    public static CultureInfo GetBestCultureMatch(
      IList<CultureInfo> orderedAcceptableCultures,
      ISet<CultureInfo> availableCultures)
    {
      int num = int.MaxValue;
      CultureInfo bestCultureMatch = (CultureInfo) null;
      foreach (CultureInfo acceptableCulture in (IEnumerable<CultureInfo>) orderedAcceptableCultures)
      {
        foreach (CultureInfo availableCulture in (IEnumerable<CultureInfo>) availableCultures)
        {
          int relationshipDistance = CultureResolution.GetCultureRelationshipDistance(acceptableCulture, availableCulture);
          if (relationshipDistance == 0)
            return availableCulture;
          if (relationshipDistance > 0 && relationshipDistance < num)
          {
            num = relationshipDistance;
            bestCultureMatch = availableCulture;
          }
        }
        if (bestCultureMatch != null)
          return bestCultureMatch;
      }
      return (CultureInfo) null;
    }

    public static int GetCultureRelationshipDistance(CultureInfo cultureA, CultureInfo cultureB)
    {
      CultureInfo invariantCulture = CultureInfo.InvariantCulture;
      if (cultureA == null || cultureB == null || cultureA.Equals((object) invariantCulture) || cultureB.Equals((object) invariantCulture))
        return -1;
      int parentCulture = DistanceToParentCulture(cultureA, cultureB);
      return parentCulture >= 0 ? parentCulture : DistanceToParentCulture(cultureB, cultureA);

      static int DistanceToParentCulture(CultureInfo childCulture, CultureInfo parentCulture)
      {
        int parentCulture1 = 0;
        CultureInfo cultureInfo = childCulture;
        while (cultureInfo != null && cultureInfo != CultureInfo.InvariantCulture && cultureInfo.Parent != cultureInfo)
        {
          if (cultureInfo.Equals((object) parentCulture))
            return parentCulture1;
          cultureInfo = cultureInfo.Parent;
          ++parentCulture1;
        }
        return -1;
      }
    }
  }
}
