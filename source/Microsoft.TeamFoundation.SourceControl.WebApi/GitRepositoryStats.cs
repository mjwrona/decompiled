// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitRepositoryStats
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitRepositoryStats
  {
    public GitRepositoryStats()
    {
    }

    public GitRepositoryStats(
      string repositoryId,
      int commitsCount,
      int branchesCount,
      int activePullRequestsCount)
    {
      this.RepositoryId = repositoryId;
      this.CommitsCount = commitsCount;
      this.BranchesCount = branchesCount;
      this.ActivePullRequestsCount = activePullRequestsCount;
    }

    [DataMember]
    public string RepositoryId { get; set; }

    [DataMember]
    public int CommitsCount { get; set; }

    [DataMember]
    public int BranchesCount { get; set; }

    [DataMember]
    public int ActivePullRequestsCount { get; set; }
  }
}
