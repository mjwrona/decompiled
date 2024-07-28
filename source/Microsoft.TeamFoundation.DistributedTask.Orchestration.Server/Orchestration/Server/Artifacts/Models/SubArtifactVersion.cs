// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models.SubArtifactVersion
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models
{
  public class SubArtifactVersion : IEquatable<SubArtifactVersion>
  {
    private IList<ArtifactSourceVersion> _artifactSourceVersions;

    public string ArtifactName { get; }

    public SubArtifactVersion(string artifactName) => this.ArtifactName = artifactName;

    public IList<ArtifactSourceVersion> ArtifactSourceVersions
    {
      get
      {
        if (this._artifactSourceVersions == null)
          this._artifactSourceVersions = (IList<ArtifactSourceVersion>) new List<ArtifactSourceVersion>();
        return this._artifactSourceVersions;
      }
    }

    public override bool Equals(object obj) => this.Equals(obj as SubArtifactVersion);

    public bool Equals(SubArtifactVersion other) => other != null && this.ArtifactName == other.ArtifactName;

    public override int GetHashCode() => EqualityComparer<string>.Default.GetHashCode(this.ArtifactName) - 997522968;
  }
}
