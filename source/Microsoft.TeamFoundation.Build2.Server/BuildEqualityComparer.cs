// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildEqualityComparer
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildEqualityComparer : IEqualityComparer<BuildData>
  {
    public bool Equals(BuildData x, BuildData y)
    {
      if (x == null && y == null)
        return true;
      if (x != null)
      {
        Guid projectId1 = x.ProjectId;
        if (y != null)
        {
          Guid projectId2 = y.ProjectId;
          return x.ProjectId.Equals(y.ProjectId) && x.Id.Equals(y.Id);
        }
      }
      return false;
    }

    public int GetHashCode(BuildData obj) => obj.Id.GetHashCode();
  }
}
