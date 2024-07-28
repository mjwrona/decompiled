// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildRepositoryEqualityComparer
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildRepositoryEqualityComparer : IEqualityComparer<BuildRepository>
  {
    public bool Equals(BuildRepository x, BuildRepository y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && string.Equals(x.Type, y.Type, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Id, y.Id, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(BuildRepository obj)
    {
      int hashCode = 0;
      if (obj != null)
      {
        if (!string.IsNullOrEmpty(obj.Type))
          hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Type);
        if (!string.IsNullOrEmpty(obj.Id))
          hashCode ^= StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Id);
      }
      return hashCode;
    }
  }
}
