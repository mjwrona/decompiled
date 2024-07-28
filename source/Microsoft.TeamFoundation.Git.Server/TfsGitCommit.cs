// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitCommit
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Git.Server.TfsGitObjects;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class TfsGitCommit : TfsGitObject
  {
    private CommitCore m_core;
    private IReadOnlyList<TfsGitCommit> m_parents;
    private string m_comment;
    private string m_shortComment;
    private bool m_shortCommentIsTruncated;
    private const int c_maxShortCommentLength = 200;

    internal TfsGitCommit(ICachedGitObjectSet objectSet, Sha1Id objectId)
      : base(objectSet, objectId)
    {
    }

    public override GitObjectType ObjectType => GitObjectType.Commit;

    internal override GitPackObjectType PackType => GitPackObjectType.Commit;

    internal override IEnumerable<Sha1Id> ReferencedObjectIds
    {
      get
      {
        this.EnsureCore();
        return this.m_core.Parents.Concat<Sha1Id>((IEnumerable<Sha1Id>) new Sha1Id[1]
        {
          this.m_core.Tree
        });
      }
    }

    public TfsGitTree GetTree()
    {
      this.EnsureCore();
      return new TfsGitTree(this.ObjectSet, this.m_core.Tree);
    }

    public IReadOnlyList<TfsGitCommit> GetParents()
    {
      if (this.m_parents == null)
      {
        this.EnsureCore();
        this.m_parents = (IReadOnlyList<TfsGitCommit>) new TfsGitCommit.IdToCommitList(this.ObjectSet, this.m_core.Parents);
      }
      return this.m_parents;
    }

    public IdentityAndDate GetAuthor()
    {
      this.EnsureCore();
      return this.m_core.Author;
    }

    public IdentityAndDate GetCommitter()
    {
      this.EnsureCore();
      return this.m_core.Committer;
    }

    public string GetComment()
    {
      if (this.m_comment == null)
      {
        this.EnsureCore();
        using (Stream content = this.GetContent())
          this.m_comment = CommitParser.ParseComment(content, this.m_core.CommentByteOffset);
      }
      return this.m_comment;
    }

    public string GetShortComment()
    {
      this.EnsureShortComment();
      return this.m_shortComment;
    }

    public bool GetShortCommentIsTruncated()
    {
      this.EnsureShortComment();
      return this.m_shortCommentIsTruncated;
    }

    private void EnsureCore()
    {
      if (this.m_core != null || this.ObjectSet.TryGetObjectCoreFromCache<CommitCore>(this.ObjectId, out this.m_core))
        return;
      using (Stream content = this.GetContent())
        this.m_core = CommitCore.Parse(content);
      this.ObjectSet.TryCacheObjectCore(this.ObjectId, (IGitObjectCore) this.m_core);
    }

    private void EnsureShortComment()
    {
      if (this.m_shortComment != null)
        return;
      this.EnsureCore();
      using (Stream content = this.GetContent())
      {
        int maxCommentLength = 201;
        this.m_shortComment = CommitParser.ParseComment(content, this.m_core.CommentByteOffset, maxCommentLength);
      }
      this.m_shortComment = StringUtil.TruncateToFirstLine(this.m_shortComment, 200, out this.m_shortCommentIsTruncated);
    }

    private class IdToCommitList : VirtualReadOnlyListBase<TfsGitCommit>
    {
      private readonly ICachedGitObjectSet m_cachedObjectSet;
      private readonly IReadOnlyList<Sha1Id> m_commitIds;

      public IdToCommitList(ICachedGitObjectSet cachedObjectSet, IReadOnlyList<Sha1Id> commitIds)
      {
        this.m_cachedObjectSet = cachedObjectSet;
        this.m_commitIds = commitIds;
      }

      public override int Count => this.m_commitIds.Count;

      protected override TfsGitCommit DoGet(int index) => new TfsGitCommit(this.m_cachedObjectSet, this.m_commitIds[index]);
    }
  }
}
