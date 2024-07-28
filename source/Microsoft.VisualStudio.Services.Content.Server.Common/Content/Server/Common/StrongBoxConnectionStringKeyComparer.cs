// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.StrongBoxConnectionStringKeyComparer
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class StrongBoxConnectionStringKeyComparer : IEqualityComparer<StrongBoxConnectionString>
  {
    public bool Equals(StrongBoxConnectionString x, StrongBoxConnectionString y)
    {
      if ((object) x == (object) y)
        return true;
      return (object) x != null && (object) y != null && !(x.GetType() != y.GetType()) && x.StrongBoxItemKey == y.StrongBoxItemKey;
    }

    public int GetHashCode(StrongBoxConnectionString obj) => obj.StrongBoxItemKey == null ? 0 : obj.StrongBoxItemKey.GetHashCode();
  }
}
