// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BranchViewItemComparer
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BranchViewItemComparer : IEqualityComparer<BranchesViewItem>
  {
    public bool Equals(BranchesViewItem x, BranchesViewItem y)
    {
      if (x == y)
        return true;
      return x != null && y != null && x.BuildId == y.BuildId;
    }

    public int GetHashCode(BranchesViewItem obj) => obj.BuildId.GetHashCode();
  }
}
