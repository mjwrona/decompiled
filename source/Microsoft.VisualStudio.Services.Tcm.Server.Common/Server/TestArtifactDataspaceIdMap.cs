// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestArtifactDataspaceIdMap
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestArtifactDataspaceIdMap : IEquatable<TestArtifactDataspaceIdMap>
  {
    public int ArtifactId;
    public int DataSpaceId;

    public TestArtifactDataspaceIdMap(int queryId, int dataSpaceId)
    {
      this.ArtifactId = queryId;
      this.DataSpaceId = dataSpaceId;
    }

    public override bool Equals(object obj) => this.Equals(obj as TestArtifactDataspaceIdMap);

    public bool Equals(TestArtifactDataspaceIdMap other) => other != null && this.ArtifactId == other.ArtifactId && this.DataSpaceId == other.DataSpaceId;

    public override int GetHashCode() => this.ArtifactId.GetHashCode() + this.DataSpaceId.GetHashCode();
  }
}
