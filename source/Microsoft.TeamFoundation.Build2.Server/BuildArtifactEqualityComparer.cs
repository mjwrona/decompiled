// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildArtifactEqualityComparer
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildArtifactEqualityComparer : IEqualityComparer<BuildArtifact>
  {
    public static readonly BuildArtifactEqualityComparer Default = new BuildArtifactEqualityComparer();

    public bool Equals(BuildArtifact x, BuildArtifact y)
    {
      int? id1 = x?.Id;
      int? id2 = y?.Id;
      return id1.GetValueOrDefault() == id2.GetValueOrDefault() & id1.HasValue == id2.HasValue;
    }

    public int GetHashCode(BuildArtifact obj) => obj.Id;
  }
}
