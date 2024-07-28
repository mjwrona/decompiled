// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitBranchStats
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitBranchStats : VersionControlSecuredObject
  {
    public GitBranchStats()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public GitBranchStats(GitCommitRef commit, string name, ISecuredObject securedObject)
      : this(commit, name, 0, 0, securedObject)
    {
    }

    public GitBranchStats(GitCommit commit, string name)
      : this((GitCommitRef) commit, name, (ISecuredObject) null)
    {
    }

    public GitBranchStats(GitCommitRef commit, string name, int aheadCount, int behindCount)
      : this(commit, name, aheadCount, behindCount, (ISecuredObject) null)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public GitBranchStats(
      GitCommitRef commit,
      string name,
      int aheadCount,
      int behindCount,
      ISecuredObject securedObject)
      : base(securedObject)
    {
      this.Commit = commit;
      this.Name = GitBranchStats.ResolveBranchName(name);
      this.AheadCount = aheadCount;
      this.BehindCount = behindCount;
      this.Commit?.SetSecuredObject(securedObject);
    }

    [DataMember]
    public GitCommitRef Commit { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public int AheadCount { get; set; }

    [DataMember]
    public int BehindCount { get; set; }

    [DataMember]
    public bool IsBaseVersion { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.Commit?.SetSecuredObject(securedObject);
    }

    private static string ResolveBranchName(string nameIn) => nameIn != null && nameIn.StartsWith("refs/heads/", StringComparison.Ordinal) ? nameIn.Substring("refs/heads/".Length) : nameIn;
  }
}
