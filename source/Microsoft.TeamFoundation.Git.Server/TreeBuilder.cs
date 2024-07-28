// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TreeBuilder
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Server.Core.Security;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class TreeBuilder
  {
    private List<TreeBuilderEntry> m_treeEntries;
    private Sha1Id? m_objectId;

    public TreeBuilder() => this.m_treeEntries = new List<TreeBuilderEntry>();

    public TreeBuilder(TfsGitTree tree)
    {
      this.m_treeEntries = tree.GetTreeEntries().Select<TfsGitTreeEntry, TreeBuilderEntry>((Func<TfsGitTreeEntry, TreeBuilderEntry>) (x => new TreeBuilderEntry(x))).ToList<TreeBuilderEntry>();
      this.m_objectId = new Sha1Id?(tree.ObjectId);
    }

    public IReadOnlyList<TreeBuilderEntry> Entries => (IReadOnlyList<TreeBuilderEntry>) this.m_treeEntries;

    public Sha1Id ObjectId
    {
      get
      {
        if (!this.m_objectId.HasValue)
          this.CreateTree(out byte[] _);
        return this.m_objectId.Value;
      }
    }

    public void UpdateTreeEntry(
      TreeFsckOptions fsckOptions,
      string name,
      Sha1Id newObjectId,
      GitPackObjectType objectType)
    {
      ArgumentUtility.CheckForNull<string>(name, nameof (name));
      this.m_objectId = new Sha1Id?();
      TreeBuilderEntry treeBuilderEntry = new TreeBuilderEntry(objectType, name, newObjectId);
      TreeParser.CheckEntryName(new ArraySegment<byte>(treeBuilderEntry.NameBytes), fsckOptions);
      for (int index = 0; index < this.m_treeEntries.Count; ++index)
      {
        TreeBuilderEntry treeEntry = this.m_treeEntries[index];
        if (treeEntry.Name.Equals(name))
        {
          this.m_treeEntries[index] = this.m_treeEntries[index].CopyAndModify(objectType, newObjectId);
          return;
        }
        if (TreeParser.GitCompareBytes(new ArraySegment<byte>(treeEntry.NameBytes), treeEntry.PackType, new ArraySegment<byte>(treeBuilderEntry.NameBytes), treeBuilderEntry.PackType) > 0)
        {
          this.m_treeEntries.Insert(index, treeBuilderEntry);
          return;
        }
      }
      this.m_treeEntries.Add(treeBuilderEntry);
    }

    public void RemoveTreeEntry(string name)
    {
      this.m_objectId = new Sha1Id?();
      for (int index = 0; index < this.m_treeEntries.Count; ++index)
      {
        if (this.m_treeEntries[index].Name.Equals(name))
        {
          this.m_treeEntries.RemoveAt(index);
          return;
        }
      }
      throw new InvalidOperationException("Tree entry " + name + " not found!");
    }

    public Sha1Id CreateTree(out byte[] finalTreeBytes)
    {
      this.m_treeEntries.Sort((Comparison<TreeBuilderEntry>) ((left, right) => TreeParser.GitCompareBytes(new ArraySegment<byte>(left.NameBytes), left.PackType, new ArraySegment<byte>(right.NameBytes), right.PackType)));
      this.m_objectId = new Sha1Id?(TreeBuilder.CreateTreeUnsafe(this.m_treeEntries, out finalTreeBytes));
      return this.m_objectId.Value;
    }

    internal static Sha1Id CreateTreeUnsafe(
      List<TreeBuilderEntry> entries,
      out byte[] finalTreeBytes)
    {
      List<byte[]> numArrayList = new List<byte[]>();
      long length = 0;
      foreach (TreeBuilderEntry entry in entries)
      {
        string str = Convert.ToString((int) entry.Mode, 8);
        length += (long) (str.Length + 1 + entry.NameBytes.Length + 1 + 20);
      }
      finalTreeBytes = new byte[length];
      long destinationIndex1 = 0;
      foreach (TreeBuilderEntry entry in entries)
      {
        byte[] bytes = GitEncodingUtil.SafeUtf8NoBom.GetBytes(Convert.ToString((int) entry.Mode, 8));
        Array.Copy((Array) bytes, 0L, (Array) finalTreeBytes, destinationIndex1, (long) bytes.Length);
        long index1 = destinationIndex1 + (long) bytes.Length;
        finalTreeBytes[index1] = (byte) 32;
        long destinationIndex2 = index1 + 1L;
        Array.Copy((Array) entry.NameBytes, 0L, (Array) finalTreeBytes, destinationIndex2, (long) entry.NameBytes.Length);
        long index2 = destinationIndex2 + (long) entry.NameBytes.Length;
        finalTreeBytes[index2] = (byte) 0;
        long destinationIndex3 = index2 + 1L;
        Array.Copy((Array) entry.ObjectId.ToByteArray(), 0L, (Array) finalTreeBytes, destinationIndex3, 20L);
        destinationIndex1 = destinationIndex3 + 20L;
      }
      using (HashingStream<SHA1Cng2> hashingStream = new HashingStream<SHA1Cng2>())
      {
        hashingStream.Setup(Stream.Null, FileAccess.Write, leaveOpen: true);
        byte[] objectHeader = GitServerUtils.CreateObjectHeader(GitPackObjectType.Tree, (long) finalTreeBytes.Length);
        hashingStream.AddToHash(objectHeader);
        hashingStream.Write(finalTreeBytes, 0, finalTreeBytes.Length);
        return new Sha1Id(hashingStream.Hash);
      }
    }

    internal void TEST_AddTreeEntry(TreeBuilderEntry entry) => this.m_treeEntries.Add(entry);
  }
}
