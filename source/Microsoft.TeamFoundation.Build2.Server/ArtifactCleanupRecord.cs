// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ArtifactCleanupRecord
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class ArtifactCleanupRecord
  {
    public BuildData Build { get; set; }

    public BuildArtifact Artifact { get; set; }

    public override string ToString() => string.Format("[Project: {0} Build: {1} Artifact: {2} ResourceType: {3}]", (object) this.Build?.ProjectId, (object) this.Build?.Id, (object) this.Artifact?.Id, (object) this.Artifact?.Resource?.Type);
  }
}
