// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitTreeDepthFirstEnumerator
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitTreeDepthFirstEnumerator : 
    IEnumerator<TreeEntryAndPath>,
    IDisposable,
    IEnumerator
  {
    private readonly TfsGitTree m_baseTree;
    private readonly string m_basePath;
    private readonly bool m_recursive;
    private readonly int m_depth;
    private readonly int m_maxEntriesCount;
    private readonly bool m_isEntriesCountLimited;
    private readonly Stack<TfsGitTreeDepthFirstEnumerator.TreeToEnumerate> m_stack;
    private TfsGitTreeDepthFirstEnumerator.TreeToEnumerate m_currTree;

    public TfsGitTreeDepthFirstEnumerator(
      TfsGitTree tree,
      bool recursive = true,
      int depth = 2147483647,
      bool isEntriesCountLimited = false,
      int maxEntriesCount = 1000000)
      : this(tree, "/", recursive, depth, isEntriesCountLimited, maxEntriesCount)
    {
    }

    private TfsGitTreeDepthFirstEnumerator(
      TfsGitTree tree,
      string basePath,
      bool recursive = true,
      int depth = 2147483647,
      bool isEntriesCountLimited = false,
      int maxEntriesCount = 1000000)
    {
      this.m_baseTree = tree;
      this.m_basePath = basePath;
      this.m_recursive = recursive;
      this.m_depth = depth;
      this.m_stack = new Stack<TfsGitTreeDepthFirstEnumerator.TreeToEnumerate>();
      this.m_isEntriesCountLimited = isEntriesCountLimited;
      this.m_maxEntriesCount = maxEntriesCount;
      this.Reset();
    }

    public void Dispose()
    {
    }

    public TreeEntryAndPath Current { get; private set; }

    public int CurrentEntriesCount { get; private set; }

    object IEnumerator.Current => (object) this.Current;

    public void Reset()
    {
      this.Current = (TreeEntryAndPath) null;
      this.CurrentDepth = 0;
      this.CurrentEntriesCount = 0;
      this.m_stack.Clear();
      this.m_currTree = new TfsGitTreeDepthFirstEnumerator.TreeToEnumerate(this.m_basePath, 0, this.m_baseTree);
    }

    public bool MoveNext()
    {
      if (this.m_depth <= 0)
        return false;
      ++this.CurrentEntriesCount;
      if (this.m_isEntriesCountLimited && this.CurrentEntriesCount >= this.m_maxEntriesCount)
        throw new DiffChangesLimitReachedException(this.CurrentEntriesCount);
      if (this.m_recursive && this.CurrentDepth < this.m_depth && this.Current != null && this.Current.Entry.PackType == GitPackObjectType.Tree)
      {
        this.m_stack.Push(this.m_currTree);
        this.m_currTree = new TfsGitTreeDepthFirstEnumerator.TreeToEnumerate(this.Current.RelativePath, this.CurrentDepth, (TfsGitTree) this.Current.Entry.Object);
      }
      return this.DoMoveNext();
    }

    public bool SkipEntriesUntilPath(NormalizedGitPath path, GitObjectType objType)
    {
      if (this.Current != null || this.m_currTree == null)
        throw new InvalidOperationException("This method cannot be called on a treeEnum that is already being enumerated");
      if (!this.m_recursive)
        throw new InvalidOperationException("This method cannot be called on a treeEnum that is not fully recursive");
      if (!this.m_basePath.Equals("/"))
        throw new InvalidOperationException("This method cannot be called on a treeEnum not based at root.");
      GitPackObjectType packType = objType.GetPackType();
      if (path.IsRoot)
        return this.MoveNext();
      int count = path.Parts.Count;
      int index = 0;
      while (this.m_currTree != null && this.m_currTree.Enumerator.MoveNext())
      {
        this.Current = new TreeEntryAndPath(this.m_currTree.BasePath, this.m_currTree.Enumerator.Current);
        this.CurrentDepth = this.m_currTree.Depth + 1;
        int num = TreeParser.GitCompareBytes(this.Current.Entry.NameBytes, this.Current.Entry.PackType, path.Parts[index], index < count - 1 ? GitPackObjectType.Tree : packType);
        if (num == 0)
        {
          ++index;
          if (index == count)
            return true;
          if (this.Current.Entry.PackType == GitPackObjectType.Tree)
          {
            this.m_stack.Push(this.m_currTree);
            this.m_currTree = new TfsGitTreeDepthFirstEnumerator.TreeToEnumerate(this.Current.RelativePath, this.CurrentDepth, (TfsGitTree) this.Current.Entry.Object);
          }
        }
        if (num > 0)
          return true;
      }
      return this.DoMoveNext();
    }

    private bool DoMoveNext()
    {
      while (this.m_currTree == null || !this.m_currTree.Enumerator.MoveNext())
      {
        this.m_currTree = this.m_stack.Count != 0 ? this.m_stack.Pop() : (TfsGitTreeDepthFirstEnumerator.TreeToEnumerate) null;
        if (this.m_currTree == null)
        {
          this.Current = (TreeEntryAndPath) null;
          this.CurrentDepth = 0;
          return false;
        }
      }
      this.Current = new TreeEntryAndPath(this.m_currTree.BasePath, this.m_currTree.Enumerator.Current);
      this.CurrentDepth = this.m_currTree.Depth + 1;
      return true;
    }

    public int CurrentDepth { get; private set; }

    public bool SkipSubtreeAndMoveNext()
    {
      if (this.Current != null && this.Current.Entry.PackType == GitPackObjectType.Tree)
        return this.DoMoveNext();
      throw new InvalidOperationException();
    }

    private class TreeToEnumerate
    {
      public TreeToEnumerate(string basePath, int depth, TfsGitTree tree)
      {
        this.BasePath = basePath;
        this.Depth = depth;
        this.Enumerator = tree.GetTreeEntries().GetEnumerator();
      }

      public string BasePath { get; }

      public int Depth { get; }

      public IEnumerator<TfsGitTreeEntry> Enumerator { get; }
    }
  }
}
