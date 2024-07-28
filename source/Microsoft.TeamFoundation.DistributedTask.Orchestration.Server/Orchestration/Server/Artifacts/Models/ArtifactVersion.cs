// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models.ArtifactVersion
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models
{
  public class ArtifactVersion : IEquatable<ArtifactVersion>
  {
    private IList<SubArtifactVersion> _subArtifactVersions;

    public ArtifactCategory ArtifactCategory { get; set; }

    public string ArtifactType { get; }

    public string Alias { get; }

    public string UniqueResourceIdentifier { get; set; }

    public Guid ConnectionId { get; set; }

    public string ArtifactVersionId { get; }

    public string ArtifactVersionName { get; set; }

    public IDictionary<string, string> ArtifactVersionProperties { get; set; }

    public IList<SubArtifactVersion> SubArtifactVersions
    {
      get
      {
        if (this._subArtifactVersions == null)
          this._subArtifactVersions = (IList<SubArtifactVersion>) new List<SubArtifactVersion>();
        return this._subArtifactVersions;
      }
    }

    public ArtifactVersion(string artifactType, string alias, string artifactVersionId)
    {
      this.ArtifactType = artifactType;
      this.Alias = alias;
      this.ArtifactVersionId = artifactVersionId;
    }

    public IList<ArtifactSourceVersion> GetSourceVersions()
    {
      HashSet<ArtifactSourceVersion> source = new HashSet<ArtifactSourceVersion>();
      foreach (SubArtifactVersion subArtifactVersion in (IEnumerable<SubArtifactVersion>) this.SubArtifactVersions)
      {
        foreach (ArtifactSourceVersion artifactSourceVersion in (IEnumerable<ArtifactSourceVersion>) subArtifactVersion.ArtifactSourceVersions)
          source.Add(artifactSourceVersion);
      }
      return (IList<ArtifactSourceVersion>) source.ToList<ArtifactSourceVersion>();
    }

    public override bool Equals(object obj) => this.Equals(obj as ArtifactVersion);

    public bool Equals(ArtifactVersion other) => other != null && this.ArtifactType == other.ArtifactType && this.Alias == other.Alias && this.ArtifactVersionId == other.ArtifactVersionId;

    public override int GetHashCode() => ((1085092482 * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ArtifactType)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Alias)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ArtifactVersionId);
  }
}
