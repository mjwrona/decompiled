// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DefinitionTagDataEqualityComparer
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class DefinitionTagDataEqualityComparer : IEqualityComparer<DefinitionTagData>
  {
    public static readonly DefinitionTagDataEqualityComparer Default = new DefinitionTagDataEqualityComparer();

    public bool Equals(DefinitionTagData x, DefinitionTagData y)
    {
      if (x == y)
        return true;
      return x != null && y != null && x.DefinitionId == y.DefinitionId && x.DefinitionVersion == y.DefinitionVersion && x.Tag == y.Tag;
    }

    public int GetHashCode(DefinitionTagData obj) => obj == null ? 0 : obj.GetHashCode();
  }
}
