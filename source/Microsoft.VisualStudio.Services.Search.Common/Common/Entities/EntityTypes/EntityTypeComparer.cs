// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityTypes.EntityTypeComparer
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.EntityTypes
{
  public class EntityTypeComparer : IEqualityComparer<IEntityType>
  {
    public bool Equals(IEntityType x, IEntityType y)
    {
      if (x == y)
        return true;
      return x != null && y != null && x.ID.Equals(y.ID) && string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(IEntityType obj)
    {
      if (obj == null)
        throw new ArgumentNullException(nameof (obj));
      return obj.Name.GetHashCode() ^ obj.ID.GetHashCode();
    }
  }
}
