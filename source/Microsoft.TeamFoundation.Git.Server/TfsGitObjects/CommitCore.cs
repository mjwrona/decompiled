// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitObjects.CommitCore
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.TfsGitObjects
{
  internal sealed class CommitCore : IGitObjectCore
  {
    private readonly List<Sha1Id> m_parents;

    private CommitCore()
    {
      this.m_parents = new List<Sha1Id>(1);
      this.EstimatedSize = CacheUtil.ObjectOverhead + 20 + IntPtr.Size + CacheUtil.ObjectOverhead + IntPtr.Size + IntPtr.Size + 4;
    }

    public Sha1Id Tree { get; private set; }

    public IReadOnlyList<Sha1Id> Parents => (IReadOnlyList<Sha1Id>) this.m_parents;

    public IdentityAndDate Author { get; private set; }

    public IdentityAndDate Committer { get; private set; }

    public int CommentByteOffset { get; private set; }

    public static CommitCore Parse(Stream stream)
    {
      CommitCore parent = new CommitCore();
      CommitCore.ParserHandler handler = new CommitCore.ParserHandler(parent);
      CommitParser.ParseMetadata(stream, stream.Length, CommitParserOptions.None, (ICommitParserHandler) handler);
      parent.m_parents.TrimExcess();
      return parent;
    }

    public GitPackObjectType PackType => GitPackObjectType.Commit;

    public int EstimatedSize { get; private set; }

    private class ParserHandler : ICommitParserHandler
    {
      private readonly CommitCore m_parent;

      public ParserHandler(CommitCore parent) => this.m_parent = parent;

      public void OnTree(Sha1Id tree) => this.m_parent.Tree = tree;

      public void OnParent(Sha1Id parent)
      {
        this.m_parent.m_parents.Add(parent);
        this.m_parent.EstimatedSize += 20;
      }

      public void OnAuthor(IdentityAndDate author)
      {
        this.m_parent.Author = author;
        this.m_parent.EstimatedSize += author.EstimatedSize;
      }

      public void OnCommitter(IdentityAndDate committer)
      {
        this.m_parent.Committer = committer;
        this.m_parent.EstimatedSize += committer.EstimatedSize;
      }

      public void OnCommentByteOffset(int commentByteOffset) => this.m_parent.CommentByteOffset = commentByteOffset;
    }
  }
}
