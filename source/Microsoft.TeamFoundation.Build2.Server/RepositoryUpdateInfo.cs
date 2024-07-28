// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.RepositoryUpdateInfo
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class RepositoryUpdateInfo
  {
    public string RepositoryType { get; set; }

    public string RepositoryId { get; set; }

    public long UpdateId { get; set; }

    public List<RefUpdateInfo> RefUpdates { get; set; }

    public List<Change> IncludedChanges { get; set; }

    public bool IsBitBucketRepository() => string.Equals(this.RepositoryType, "Bitbucket", StringComparison.OrdinalIgnoreCase);

    public bool IsGithubRepository() => string.Equals(this.RepositoryType, "GitHub", StringComparison.OrdinalIgnoreCase);
  }
}
