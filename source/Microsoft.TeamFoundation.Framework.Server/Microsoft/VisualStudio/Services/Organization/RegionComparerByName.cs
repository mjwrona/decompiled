// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.RegionComparerByName
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Organization.Client;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Organization
{
  internal class RegionComparerByName : IEqualityComparer<Region>
  {
    public bool Equals(Region x, Region y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && StringComparer.OrdinalIgnoreCase.Equals(x.Name, y.Name);
    }

    public int GetHashCode(Region obj) => obj != null && obj.Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name) : 0;

    internal static RegionComparerByName Instance { get; } = new RegionComparerByName();
  }
}
