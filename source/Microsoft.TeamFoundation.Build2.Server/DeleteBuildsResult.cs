// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DeleteBuildsResult
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class DeleteBuildsResult
  {
    private IList<BuildData> m_deletedBuilds;
    private IList<BuildArtifact> m_deletedArtifacts;

    public IList<BuildData> DeletedBuilds
    {
      get
      {
        if (this.m_deletedBuilds == null)
          this.m_deletedBuilds = (IList<BuildData>) new List<BuildData>();
        return this.m_deletedBuilds;
      }
      set => this.m_deletedBuilds = value;
    }

    public IList<BuildArtifact> DeletedArtifacts
    {
      get
      {
        if (this.m_deletedArtifacts == null)
          this.m_deletedArtifacts = (IList<BuildArtifact>) new List<BuildArtifact>();
        return this.m_deletedArtifacts;
      }
      set => this.m_deletedArtifacts = value;
    }
  }
}
