// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RepoKey
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class RepoKey : RepoScope, IEquatable<RepoKey>
  {
    public RepoKey(Guid projectId, Guid repoId)
      : this(projectId, repoId, Guid.Empty)
    {
    }

    internal RepoKey(Guid projectId, Guid repoId, Guid containerId)
      : base(projectId, repoId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(repoId, nameof (repoId));
      this.OdbId = new OdbId(containerId);
    }

    internal OdbId OdbId { get; }

    public bool Equals(RepoKey other) => (RepoScope) this == (RepoScope) other;
  }
}
