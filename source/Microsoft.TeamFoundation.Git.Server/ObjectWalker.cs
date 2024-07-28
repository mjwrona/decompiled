// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ObjectWalker
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class ObjectWalker
  {
    private readonly Queue<TfsGitTree> m_treesToWalk;

    public ObjectWalker() => this.m_treesToWalk = new Queue<TfsGitTree>();

    public void Walk(
      IEnumerable<TfsGitObject> startObjects,
      Func<Sha1Id, GitObjectType, bool> walkCondition)
    {
      this.m_treesToWalk.Clear();
      foreach (TfsGitObject startObject in startObjects)
      {
        if (walkCondition(startObject.ObjectId, startObject.ObjectType))
        {
          if (startObject.PackType == GitPackObjectType.Commit)
          {
            TfsGitTree tree = ((TfsGitCommit) startObject).GetTree();
            if (walkCondition(tree.ObjectId, tree.ObjectType))
              this.m_treesToWalk.Enqueue(tree);
          }
          else if (startObject.PackType == GitPackObjectType.Tree)
            this.m_treesToWalk.Enqueue((TfsGitTree) startObject);
        }
      }
      while (this.m_treesToWalk.Count > 0)
      {
        TfsGitTree tfsGitTree = this.m_treesToWalk.Dequeue();
        foreach (TreeParser.Entry parserEntry in tfsGitTree.GetParserEntries())
        {
          if (parserEntry.PackType != GitPackObjectType.Commit && walkCondition(parserEntry.ObjectId, GitObjectType.Tree) && parserEntry.PackType == GitPackObjectType.Tree)
            this.m_treesToWalk.Enqueue(new TfsGitTree(tfsGitTree.ObjectSet, parserEntry.ObjectId));
        }
      }
    }
  }
}
