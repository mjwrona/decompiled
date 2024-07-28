// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ICommitParserHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  internal interface ICommitParserHandler
  {
    void OnTree(Sha1Id tree);

    void OnParent(Sha1Id parent);

    void OnAuthor(IdentityAndDate author);

    void OnCommitter(IdentityAndDate committer);

    void OnCommentByteOffset(int commentByteOffset);
  }
}
