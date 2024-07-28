// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.RecentActivityIdComparer
// Assembly: Microsoft.Azure.Boards.RecentActivity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 684DCFA4-4764-4794-94A6-960AF811434C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.RecentActivity.dll

using System.Collections.Generic;

namespace Microsoft.Azure.Boards.RecentActivity
{
  public class RecentActivityIdComparer : IEqualityComparer<Microsoft.Azure.Boards.RecentActivity.RecentActivity>
  {
    public static RecentActivityIdComparer Instance = new RecentActivityIdComparer();

    bool IEqualityComparer<Microsoft.Azure.Boards.RecentActivity.RecentActivity>.Equals(
      Microsoft.Azure.Boards.RecentActivity.RecentActivity x,
      Microsoft.Azure.Boards.RecentActivity.RecentActivity y)
    {
      if (x == y)
        return true;
      return x != null && y != null && x.ArtifactId == y.ArtifactId;
    }

    int IEqualityComparer<Microsoft.Azure.Boards.RecentActivity.RecentActivity>.GetHashCode(
      Microsoft.Azure.Boards.RecentActivity.RecentActivity obj)
    {
      return obj.ArtifactId.GetHashCode();
    }
  }
}
