// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitBillableCommitterDetail
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitBillableCommitterDetail : GitBillableCommitter
  {
    public GitBillableCommitterDetail(
      Guid projectId,
      string projectName,
      Guid repoId,
      string repoName,
      Sha1Id commitId,
      Guid vsid,
      string committerEmail,
      DateTime commitTime,
      int pushId,
      DateTime pushedTime)
      : base(vsid, repoId)
    {
      this.ProjectId = projectId;
      this.ProjectName = projectName;
      this.RepoName = repoName;
      this.CommitId = commitId;
      this.CommitterEmail = committerEmail;
      this.CommitTime = commitTime;
      this.PushId = pushId;
      this.PushedTime = pushedTime;
    }

    public GitBillableCommitterDetail(
      Guid projectId,
      string projectName,
      Guid repoId,
      string repoName,
      Sha1Id commitId,
      Guid vsid,
      string committerEmail,
      DateTime commitTime,
      int pushId,
      DateTime pushedTime,
      Guid pusherid,
      string samAccountName,
      string mailNickName,
      string displayName)
      : base(vsid, repoId)
    {
      this.ProjectId = projectId;
      this.ProjectName = projectName;
      this.RepoName = repoName;
      this.CommitId = commitId;
      this.CommitterEmail = committerEmail;
      this.CommitTime = commitTime;
      this.PushId = pushId;
      this.PushedTime = pushedTime;
      this.Pusherid = pusherid;
      this.SamAccountName = samAccountName;
      this.MailNickName = mailNickName;
      this.DisplayName = displayName;
    }

    public Guid ProjectId { get; }

    public string ProjectName { get; }

    public string RepoName { get; }

    public Sha1Id CommitId { get; }

    public string CommitterEmail { get; }

    public DateTime CommitTime { get; }

    public int PushId { get; }

    public DateTime PushedTime { get; }

    public Guid Pusherid { get; }

    public string SamAccountName { get; }

    public string MailNickName { get; }

    public string DisplayName { get; }
  }
}
