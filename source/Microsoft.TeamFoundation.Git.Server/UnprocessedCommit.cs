// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.UnprocessedCommit
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class UnprocessedCommit
  {
    private readonly Sha1Id m_objectId;
    private readonly int m_pushId;
    private readonly string m_comment;
    private readonly string m_committer;
    private readonly string m_author;
    private readonly DateTime m_commitTime;
    private readonly DateTime m_authorTime;

    public UnprocessedCommit(
      RepoKey repoKey,
      Sha1Id objectId,
      int pushId,
      string comment,
      string committer,
      string author,
      DateTime commitTime,
      DateTime authorTime)
    {
      this.RepoKey = repoKey;
      this.m_objectId = objectId;
      this.m_pushId = pushId;
      this.m_comment = comment;
      this.m_committer = committer;
      this.m_author = author;
      this.m_commitTime = commitTime;
      this.m_authorTime = authorTime;
    }

    public static UnprocessedCommit FromTfsGitCommit(
      RepoKey repoKey,
      TfsGitCommit commit,
      int pushId)
    {
      return new UnprocessedCommit(repoKey, commit.ObjectId, pushId, commit.GetComment(), commit.GetCommitter().NameAndEmail, commit.GetAuthor().NameAndEmail, commit.GetCommitter().Time, commit.GetAuthor().Time);
    }

    public RepoKey RepoKey { get; private set; }

    public Sha1Id ObjectId => this.m_objectId;

    public int PushId => this.m_pushId;

    public string Comment => this.m_comment;

    public string Committer => this.m_committer;

    public string Author => this.m_author;

    public DateTime CommitTime => this.m_commitTime;

    public DateTime AuthorTime => this.m_authorTime;
  }
}
