// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RepoScope
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class RepoScope : IEquatable<RepoScope>
  {
    public RepoScope(Guid projectId, Guid repoId)
    {
      if (repoId != Guid.Empty)
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      this.ProjectId = projectId;
      this.RepoId = repoId;
    }

    public Guid ProjectId { get; }

    public Guid RepoId { get; }

    public override bool Equals(object other) => this == other as RepoScope;

    public bool Equals(RepoScope other) => this == other;

    public override int GetHashCode() => this.RepoId.GetHashCode();

    public override string ToString() => string.Format("{0}/{1}", (object) this.ProjectId, (object) this.RepoId);

    public static bool operator ==(RepoScope a, RepoScope b)
    {
      if ((object) a == (object) b)
        return true;
      return (object) a != null && (object) b != null && a.ProjectId == b.ProjectId && a.RepoId == b.RepoId;
    }

    public static bool operator !=(RepoScope a, RepoScope b) => !(a == b);
  }
}
