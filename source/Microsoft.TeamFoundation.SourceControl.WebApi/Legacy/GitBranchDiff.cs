// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitBranchDiff
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class GitBranchDiff
  {
    private const string c_headsPrefix = "refs/heads/";

    private GitBranchDiff(string refName, IdentityRef isLockedBy)
    {
      if (refName.StartsWith("refs/heads/", StringComparison.Ordinal))
        this.BranchName = refName.Substring("refs/heads/".Length);
      this.IsLockedBy = isLockedBy;
    }

    public GitBranchDiff(GitCommit commit, string refName, IdentityRef isLockedBy)
      : this(refName, isLockedBy)
    {
      this.Commit = commit;
      this.AheadCount = 0;
      this.BehindCount = 0;
      this.IsBaseCommit = true;
    }

    public GitBranchDiff(
      GitCommit commit,
      string refName,
      IdentityRef isLockedBy,
      int aheadCount,
      int behindCount)
      : this(refName, isLockedBy)
    {
      this.Commit = commit;
      this.AheadCount = aheadCount;
      this.BehindCount = behindCount;
    }

    [DataMember(Name = "commit")]
    public GitCommit Commit { get; private set; }

    [DataMember(Name = "branchName")]
    public string BranchName { get; private set; }

    [DataMember(Name = "aheadCount")]
    public int AheadCount { get; private set; }

    [DataMember(Name = "behindCount")]
    public int BehindCount { get; private set; }

    [DataMember(Name = "isBaseCommit")]
    public bool IsBaseCommit { get; private set; }

    [DataMember(Name = "isLockedBy", EmitDefaultValue = false)]
    public IdentityRef IsLockedBy { get; private set; }
  }
}
