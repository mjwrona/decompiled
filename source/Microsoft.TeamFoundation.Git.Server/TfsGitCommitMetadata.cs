// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitCommitMetadata
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitCommitMetadata
  {
    internal TfsGitCommitMetadata(
      Sha1Id commitId,
      string shortComment,
      string committer,
      string author,
      DateTime commitTime,
      DateTime authorTime,
      bool isCommentTruncated)
    {
      this.CommitId = commitId;
      this.ShortComment = shortComment;
      this.Committer = committer;
      this.Author = author;
      this.CommitTime = commitTime;
      this.AuthorTime = authorTime;
      this.ShortCommentIsTruncated = isCommentTruncated;
    }

    public TfsGitCommitMetadata(TfsGitCommit commit)
      : this(commit.ObjectId, commit.GetShortComment(), commit.GetCommitter().NameAndEmail, commit.GetAuthor().NameAndEmail, commit.GetCommitter().Time, commit.GetAuthor().Time, commit.GetShortCommentIsTruncated())
    {
    }

    public Sha1Id CommitId { get; }

    public string ShortComment { get; }

    public bool ShortCommentIsTruncated { get; }

    public string Committer { get; }

    public string Author { get; }

    public DateTime CommitTime { get; }

    public DateTime AuthorTime { get; }
  }
}
