// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitTree
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class TfsGitTree : TfsGitObject
  {
    internal static readonly Sha1Id EmptyTreeId = new Sha1Id("4b825dc642cb6eb9a060e54bf8d69288fbee4904");
    private ArraySegment<byte>? m_content;

    internal TfsGitTree(ICachedGitObjectSet objectSet, Sha1Id objectId)
      : base(objectSet, objectId)
    {
    }

    public override GitObjectType ObjectType => GitObjectType.Tree;

    internal override GitPackObjectType PackType => GitPackObjectType.Tree;

    internal override IEnumerable<Sha1Id> ReferencedObjectIds => this.GetParserEntries().Select<TreeParser.Entry, Sha1Id>((Func<TreeParser.Entry, Sha1Id>) (x => x.ObjectId));

    internal IEnumerable<TreeParser.Entry> GetParserEntries()
    {
      this.EnsureContent();
      return TreeParser.Parse(this.m_content.Value, TreeFsckOptions.None);
    }

    public IEnumerable<TfsGitTreeEntry> GetTreeEntries() => this.GetParserEntries().Select<TreeParser.Entry, TfsGitTreeEntry>((Func<TreeParser.Entry, TfsGitTreeEntry>) (x => new TfsGitTreeEntry(this.ObjectSet, x)));

    public IEnumerable<TreeEntryAndPath> GetTreeEntriesRecursive(
      int depth = 2147483647,
      bool preventGitTreeOverflow = false)
    {
      return (IEnumerable<TreeEntryAndPath>) new TfsGitTree.TfsGitTreeDepthFirstEnumerable(this, depth, preventGitTreeOverflow);
    }

    public IEnumerable<TfsGitBlob> GetBlobs() => this.GetParserEntries().Where<TreeParser.Entry>((Func<TreeParser.Entry, bool>) (x => x.PackType == GitPackObjectType.Blob)).Select<TreeParser.Entry, TfsGitBlob>((Func<TreeParser.Entry, TfsGitBlob>) (x => new TfsGitBlob(this.ObjectSet, x.ObjectId)));

    public IEnumerable<TfsGitTree> GetTrees() => this.GetParserEntries().Where<TreeParser.Entry>((Func<TreeParser.Entry, bool>) (x => x.PackType == GitPackObjectType.Tree)).Select<TreeParser.Entry, TfsGitTree>((Func<TreeParser.Entry, TfsGitTree>) (x => new TfsGitTree(this.ObjectSet, x.ObjectId)));

    public IEnumerable<TfsGitObject> GetObjects()
    {
      TfsGitTree tfsGitTree = this;
      foreach (TreeParser.Entry parserEntry in tfsGitTree.GetParserEntries())
      {
        if (GitPackObjectType.Tree == parserEntry.PackType)
          yield return (TfsGitObject) new TfsGitTree(tfsGitTree.ObjectSet, parserEntry.ObjectId);
        else if (GitPackObjectType.Blob == parserEntry.PackType)
          yield return (TfsGitObject) new TfsGitBlob(tfsGitTree.ObjectSet, parserEntry.ObjectId);
      }
    }

    public bool FindEntry(ArraySegment<byte> nameBytes, out TfsGitTreeEntry result)
    {
      foreach (TreeParser.Entry parserEntry in this.GetParserEntries())
      {
        if (TreeParser.GitCompareBytes(parserEntry.NameBytes, parserEntry.PackType, nameBytes, parserEntry.PackType) == 0)
        {
          result = new TfsGitTreeEntry(this.ObjectSet, parserEntry);
          return true;
        }
      }
      result = (TfsGitTreeEntry) null;
      return false;
    }

    private void EnsureContent()
    {
      if (this.m_content.HasValue)
        return;
      using (Stream content = this.GetContent())
      {
        long length = content.Length;
        this.m_content = length <= 16777216L ? new ArraySegment<byte>?(GitStreamUtil.GetBuffer(content, length)) : throw new GitObjectTooLargeException(this.ObjectId, length, 16777216L);
      }
    }

    private class TfsGitTreeDepthFirstEnumerable : IEnumerable<TreeEntryAndPath>, IEnumerable
    {
      private readonly TfsGitTree m_toEnumerate;
      private readonly int m_depth;
      private readonly bool m_preventGitTreeOverflow;

      public TfsGitTreeDepthFirstEnumerable(
        TfsGitTree toEnumerate,
        int depth = 2147483647,
        bool preventGitTreeOverflow = false)
      {
        this.m_toEnumerate = toEnumerate;
        this.m_depth = depth;
        this.m_preventGitTreeOverflow = preventGitTreeOverflow;
      }

      public IEnumerator<TreeEntryAndPath> GetEnumerator() => (IEnumerator<TreeEntryAndPath>) new TfsGitTreeDepthFirstEnumerator(this.m_toEnumerate, depth: this.m_depth, isEntriesCountLimited: this.m_preventGitTreeOverflow);

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new TfsGitTreeDepthFirstEnumerator(this.m_toEnumerate, depth: this.m_depth, isEntriesCountLimited: this.m_preventGitTreeOverflow);
    }
  }
}
