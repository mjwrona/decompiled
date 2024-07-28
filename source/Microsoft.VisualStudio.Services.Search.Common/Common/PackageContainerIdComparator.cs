// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.PackageContainerIdComparator
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class PackageContainerIdComparator : IEqualityComparer<PackageContainer>
  {
    public bool Equals(PackageContainer x, PackageContainer y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && !(x.GetType().FullName != y.GetType().FullName) && x.ContainerId == y.ContainerId;
    }

    public int GetHashCode(PackageContainer obj) => obj != null ? obj.ContainerId.GetHashCode() : throw new ArgumentNullException(nameof (obj));
  }
}
