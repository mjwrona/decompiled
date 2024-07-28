// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.EventCommitParserHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class EventCommitParserHandler : ICommitParserHandler
  {
    void ICommitParserHandler.OnTree(Sha1Id tree)
    {
      EventCommitParserHandler.TreeEvent tree1 = this.Tree;
      if (tree1 == null)
        return;
      tree1(tree);
    }

    void ICommitParserHandler.OnParent(Sha1Id parent)
    {
      EventCommitParserHandler.ParentEvent parent1 = this.Parent;
      if (parent1 == null)
        return;
      parent1(parent);
    }

    void ICommitParserHandler.OnAuthor(IdentityAndDate author)
    {
      EventCommitParserHandler.AuthorEvent author1 = this.Author;
      if (author1 == null)
        return;
      author1(author);
    }

    void ICommitParserHandler.OnCommitter(IdentityAndDate committer)
    {
      EventCommitParserHandler.CommitterEvent committer1 = this.Committer;
      if (committer1 == null)
        return;
      committer1(committer);
    }

    void ICommitParserHandler.OnCommentByteOffset(int commentByteOffset)
    {
      EventCommitParserHandler.CommentByteOffsetEvent commentByteOffset1 = this.CommentByteOffset;
      if (commentByteOffset1 == null)
        return;
      commentByteOffset1(commentByteOffset);
    }

    public event EventCommitParserHandler.TreeEvent Tree;

    public event EventCommitParserHandler.ParentEvent Parent;

    public event EventCommitParserHandler.AuthorEvent Author;

    public event EventCommitParserHandler.CommitterEvent Committer;

    public event EventCommitParserHandler.CommentByteOffsetEvent CommentByteOffset;

    public delegate void TreeEvent(Sha1Id tree);

    public delegate void ParentEvent(Sha1Id parent);

    public delegate void AuthorEvent(IdentityAndDate author);

    public delegate void CommitterEvent(IdentityAndDate committer);

    public delegate void CommentByteOffsetEvent(int commentByteOffset);
  }
}
