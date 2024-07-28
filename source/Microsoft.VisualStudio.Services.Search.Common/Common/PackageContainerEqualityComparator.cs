// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.PackageContainerEqualityComparator
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class PackageContainerEqualityComparator : IEqualityComparer<PackageContainer>
  {
    public bool Equals(PackageContainer x, PackageContainer y)
    {
      if (x == null && y == null)
        return true;
      if (x == null || y == null || x.GetType().FullName != y.GetType().FullName || !(x.ContainerId == y.ContainerId) || x.Type != y.Type || !x.Name.Equals(y.Name, StringComparison.Ordinal) || !(x.Token == y.Token))
        return false;
      if (x.SecurityHashCode.Length == 0 && y.SecurityHashCode == null)
        return true;
      return y.SecurityHashCode != null && ((IEnumerable<byte>) x.SecurityHashCode).Take<byte>(20).SequenceEqual<byte>(((IEnumerable<byte>) y.SecurityHashCode).Take<byte>(20));
    }

    public int GetHashCode(PackageContainer obj) => obj != null ? obj.ContainerId.GetHashCode() : throw new ArgumentNullException(nameof (obj));
  }
}
