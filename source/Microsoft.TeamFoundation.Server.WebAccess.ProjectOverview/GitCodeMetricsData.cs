// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.GitCodeMetricsData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97A9928B-E499-4978-909F-1EBC8C5535AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview
{
  [DataContract]
  public class GitCodeMetricsData : AbstractProjectSecuredObject
  {
    public GitCodeMetricsData(
      int commitsPushedCount,
      int pullRequestsCreatedCount,
      int pullRequestsCompletedCount,
      int authorsCount,
      int[] commitsTrend)
    {
      this.CommitsPushedCount = commitsPushedCount;
      this.PullRequestsCreatedCount = pullRequestsCreatedCount;
      this.PullRequestsCompletedCount = pullRequestsCompletedCount;
      this.AuthorsCount = authorsCount;
      this.CommitsTrend = commitsTrend;
    }

    [DataMember]
    public int CommitsPushedCount { get; }

    [DataMember]
    public int PullRequestsCreatedCount { get; }

    [DataMember]
    public int PullRequestsCompletedCount { get; }

    [DataMember]
    public int AuthorsCount { get; }

    [DataMember]
    public int[] CommitsTrend { get; }
  }
}
