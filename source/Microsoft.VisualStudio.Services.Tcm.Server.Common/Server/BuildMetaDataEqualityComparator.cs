// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.BuildMetaDataEqualityComparator
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class BuildMetaDataEqualityComparator : IEqualityComparer<BuildMetaData>
  {
    public bool Equals(BuildMetaData x, BuildMetaData y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && string.Equals(x.BuildFlavor, y.BuildFlavor, StringComparison.OrdinalIgnoreCase) && string.Equals(x.BuildPlatform, y.BuildPlatform, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(BuildMetaData obj) => obj.BuildFlavor.GetHashCode() ^ obj.BuildPlatform.GetHashCode();
  }
}
