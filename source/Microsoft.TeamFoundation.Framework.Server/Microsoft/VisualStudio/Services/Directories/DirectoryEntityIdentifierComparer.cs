// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryEntityIdentifierComparer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Directories
{
  internal class DirectoryEntityIdentifierComparer : IEqualityComparer<DirectoryEntityIdentifier>
  {
    public bool Equals(DirectoryEntityIdentifier x, DirectoryEntityIdentifier y)
    {
      if (x == y)
        return true;
      return x != null && y != null && x.Version.Equals(y.Version) && string.Equals(x.Encode(), y.Encode(), StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(DirectoryEntityIdentifier obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Encode());
  }
}
