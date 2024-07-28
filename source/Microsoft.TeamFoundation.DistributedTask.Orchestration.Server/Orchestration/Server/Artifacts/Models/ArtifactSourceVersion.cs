// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models.ArtifactSourceVersion
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models
{
  public class ArtifactSourceVersion : IEquatable<ArtifactSourceVersion>
  {
    private string _mUniqueIdentifier;

    public string UniqueIdentifier
    {
      get
      {
        if (string.IsNullOrWhiteSpace(this._mUniqueIdentifier))
          this._mUniqueIdentifier = this.GetRepositoryUniqueIdentifier();
        return this._mUniqueIdentifier;
      }
    }

    public string RepositoryType { get; set; }

    public string RepositoryId { get; set; }

    public string RepositoryName { get; set; }

    public string SourceBranch { get; set; }

    public string Version { get; set; }

    public Guid ServiceConnectionId { get; set; }

    public IDictionary<string, string> Properties { get; set; }

    public override bool Equals(object obj) => this.Equals(obj as ArtifactSourceVersion);

    public bool Equals(ArtifactSourceVersion other) => other != null && this.UniqueIdentifier == other.UniqueIdentifier;

    public override int GetHashCode() => 1780733181 + EqualityComparer<string>.Default.GetHashCode(this.UniqueIdentifier);

    private string GetRepositoryUniqueIdentifier()
    {
      if (this.ServiceConnectionId != Guid.Empty)
        return string.Format("{0}:{1}:{2}", (object) this.ServiceConnectionId.ToString(), (object) (this.RepositoryId ?? this.RepositoryName), (object) this.SourceBranch);
      string str;
      return this.Properties != null && this.Properties.TryGetValue(ArtifactTraceabilityPropertyKeys.ProjectId, out str) ? string.Format("{0}:{1}:{2}", (object) str, (object) (this.RepositoryId ?? this.RepositoryName), (object) this.SourceBranch) : string.Format("{0}:{1}", (object) (this.RepositoryId ?? this.RepositoryName), (object) this.SourceBranch);
    }
  }
}
