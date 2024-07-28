// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.ConsistentHashShardManager`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class ConsistentHashShardManager<TPhysicalNode> : IShardManager<TPhysicalNode> where TPhysicalNode : class, IPhysicalNode
  {
    private static readonly TimeSpan LockTimeout = TimeSpan.FromSeconds(10.0);
    private readonly AsyncReaderWriterLock readerWriterLock = new AsyncReaderWriterLock();
    private readonly HashAlgorithm hasher = (HashAlgorithm) new SHA1Managed();
    internal readonly List<VirtualNode<TPhysicalNode>> sortedNodes = new List<VirtualNode<TPhysicalNode>>();
    private readonly HashSet<string> uniqueNamesEncountered = new HashSet<string>();
    private readonly Dictionary<VirtualNode<TPhysicalNode>, VirtualNode<TPhysicalNode>> migratingNodes = new Dictionary<VirtualNode<TPhysicalNode>, VirtualNode<TPhysicalNode>>();

    public ConsistentHashShardManager(
      IEnumerable<TPhysicalNode> nodes,
      int virtualNodesPerPhysicalNode)
    {
      foreach (TPhysicalNode node in nodes)
        this.AddNodeInternal(node, virtualNodesPerPhysicalNode);
      this.Sort();
    }

    public List<VirtualNode<TPhysicalNode>> VirtualNodes
    {
      get
      {
        using (this.readerWriterLock.ReaderLockAsync(ConsistentHashShardManager<TPhysicalNode>.LockTimeout).Result)
          return this.sortedNodes.ToList<VirtualNode<TPhysicalNode>>();
      }
    }

    public List<MigratedKeyRange<TPhysicalNode>> AddNode(TPhysicalNode node, int virtualNodeCount)
    {
      using (this.readerWriterLock.WriterLockAsync(ConsistentHashShardManager<TPhysicalNode>.LockTimeout).Result)
      {
        List<VirtualNode<TPhysicalNode>> list1 = this.sortedNodes.ToList<VirtualNode<TPhysicalNode>>();
        this.AddNodeInternal(node, virtualNodeCount);
        this.Sort();
        List<VirtualNode<TPhysicalNode>> sortedNodes = this.sortedNodes;
        List<MigratedKeyRange<TPhysicalNode>> list2 = ConsistentHashShardManager<TPhysicalNode>.DetermineMigratedKeyRanges(list1, sortedNodes).ToList<MigratedKeyRange<TPhysicalNode>>();
        foreach (MigratedKeyRange<TPhysicalNode> migratedKeyRange in list2)
        {
          VirtualNode<TPhysicalNode> virtualNode;
          if (this.migratingNodes.TryGetValue(migratedKeyRange.NewNode, out virtualNode))
          {
            if (virtualNode != migratedKeyRange.OriginalNode)
              throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Node {0} is cannot be migrating from {1} as it is already migrating from {2}.", (object) migratedKeyRange.NewNode.NodeId, (object) migratedKeyRange.OriginalNode.NodeId, (object) virtualNode.NodeId));
          }
          else
            this.migratingNodes.Add(migratedKeyRange.NewNode, migratedKeyRange.OriginalNode);
        }
        return list2;
      }
    }

    public void MarkMigrationComplete(MigratedKeyRange<TPhysicalNode> completedKeyRange)
    {
      using (this.readerWriterLock.WriterLockAsync(ConsistentHashShardManager<TPhysicalNode>.LockTimeout).Result)
      {
        if (!this.migratingNodes.Remove(completedKeyRange.NewNode))
          throw new ArgumentException("Key range is not currently migrating: " + completedKeyRange.ToString(), nameof (completedKeyRange));
      }
    }

    public VirtualNodeContext<TPhysicalNode> FindNode(ulong key)
    {
      IDisposable readerWriterLockReleaser = this.readerWriterLock.ReaderLockAsync(ConsistentHashShardManager<TPhysicalNode>.LockTimeout).SyncResult<IDisposable>();
      try
      {
        VirtualNode<TPhysicalNode> node1 = ConsistentHashShardManager<TPhysicalNode>.FindNode(key, this.sortedNodes);
        VirtualNode<TPhysicalNode> previousNode;
        this.migratingNodes.TryGetValue(node1, out previousNode);
        ConsistentHashShardManager<TPhysicalNode>.VirtualNodeContextWithLock node2 = new ConsistentHashShardManager<TPhysicalNode>.VirtualNodeContextWithLock(node1, previousNode, readerWriterLockReleaser);
        readerWriterLockReleaser = (IDisposable) null;
        return (VirtualNodeContext<TPhysicalNode>) node2;
      }
      finally
      {
        readerWriterLockReleaser?.Dispose();
      }
    }

    public IEnumerable<PhysicalNodeContext<TKey, TPhysicalNode>> FindNodes<TKey>(
      IEnumerable<KeyValuePair<TKey, ulong>> keys)
    {
      using (this.readerWriterLock.ReaderLockAsync(ConsistentHashShardManager<TPhysicalNode>.LockTimeout).SyncResult<IDisposable>())
      {
        IEnumerable<IGrouping<VirtualNode<TPhysicalNode>, KeyValuePair<TKey, ulong>>> source = keys.GroupBy<KeyValuePair<TKey, ulong>, VirtualNode<TPhysicalNode>>((Func<KeyValuePair<TKey, ulong>, VirtualNode<TPhysicalNode>>) (key => ConsistentHashShardManager<TPhysicalNode>.FindNode(key.Value, this.sortedNodes)));
        List<IDisposable> disposables = new List<IDisposable>();
        try
        {
          List<PhysicalNodeContext<TKey, TPhysicalNode>> list = source.GroupBy<IGrouping<VirtualNode<TPhysicalNode>, KeyValuePair<TKey, ulong>>, TPhysicalNode, ConsistentHashShardManager<TPhysicalNode>.VirtualNodeContextWithKeysAndLock<TKey>>((Func<IGrouping<VirtualNode<TPhysicalNode>, KeyValuePair<TKey, ulong>>, TPhysicalNode>) (virtualNodeGroup => virtualNodeGroup.Key.PhysicalNode), (Func<IGrouping<VirtualNode<TPhysicalNode>, KeyValuePair<TKey, ulong>>, ConsistentHashShardManager<TPhysicalNode>.VirtualNodeContextWithKeysAndLock<TKey>>) (virtualNodeGroup =>
          {
            VirtualNode<TPhysicalNode> key = virtualNodeGroup.Key;
            IDisposable readerWriterLockReleaser = this.readerWriterLock.ReaderLockAsync().SyncResult<IDisposable>();
            disposables.Add(readerWriterLockReleaser);
            VirtualNode<TPhysicalNode> previousNode = (VirtualNode<TPhysicalNode>) null;
            this.migratingNodes.TryGetValue(key, out previousNode);
            return new ConsistentHashShardManager<TPhysicalNode>.VirtualNodeContextWithKeysAndLock<TKey>(key, previousNode, (IEnumerable<KeyValuePair<TKey, ulong>>) virtualNodeGroup, readerWriterLockReleaser);
          })).Select<IGrouping<TPhysicalNode, ConsistentHashShardManager<TPhysicalNode>.VirtualNodeContextWithKeysAndLock<TKey>>, PhysicalNodeContext<TKey, TPhysicalNode>>((Func<IGrouping<TPhysicalNode, ConsistentHashShardManager<TPhysicalNode>.VirtualNodeContextWithKeysAndLock<TKey>>, PhysicalNodeContext<TKey, TPhysicalNode>>) (physicalNodeGrouping => new PhysicalNodeContext<TKey, TPhysicalNode>(physicalNodeGrouping.Key, physicalNodeGrouping.Cast<VirtualNodeContextWithKeys<TKey, TPhysicalNode>>().ToList<VirtualNodeContextWithKeys<TKey, TPhysicalNode>>()))).ToList<PhysicalNodeContext<TKey, TPhysicalNode>>();
          disposables.Clear();
          return (IEnumerable<PhysicalNodeContext<TKey, TPhysicalNode>>) list;
        }
        finally
        {
          foreach (IDisposable disposable in disposables)
            disposable.Dispose();
        }
      }
    }

    protected virtual ulong ComputeNodeId(TPhysicalNode physicalNode, int virtualNodeIndex)
    {
      string s = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) physicalNode.UniqueName, (object) virtualNodeIndex);
      return BitConverter.ToUInt64(this.hasher.ComputeHash(StrictEncodingWithoutBOM.UTF8.GetBytes(s)), 0);
    }

    public static IEnumerable<ConsistentHashShardManager<TPhysicalNode>.KeyRange> GetKeyRanges(
      List<VirtualNode<TPhysicalNode>> nodes)
    {
      ulong startKey = 0;
      foreach (VirtualNode<TPhysicalNode> node in nodes)
      {
        yield return new ConsistentHashShardManager<TPhysicalNode>.KeyRange(startKey, node.NodeId, node);
        startKey = node.NodeId + 1UL;
      }
      yield return new ConsistentHashShardManager<TPhysicalNode>.KeyRange(startKey, ulong.MaxValue, nodes[0]);
    }

    private static VirtualNode<TPhysicalNode> FindNode(
      ulong key,
      List<VirtualNode<TPhysicalNode>> nodeList)
    {
      VirtualNode<TPhysicalNode> virtualNode = new VirtualNode<TPhysicalNode>(default (TPhysicalNode), -1, -1, key);
      int index = nodeList.BinarySearch(virtualNode, VirtualNode<TPhysicalNode>.ComparerInstance);
      if (index < 0)
      {
        int num = index ^ -1;
        index = num != nodeList.Count ? num : 0;
      }
      return nodeList[index];
    }

    private static IEnumerable<MigratedKeyRange<TPhysicalNode>> DetermineMigratedKeyRanges(
      List<VirtualNode<TPhysicalNode>> oldNodes,
      List<VirtualNode<TPhysicalNode>> newNodes)
    {
      foreach (ConsistentHashShardManager<TPhysicalNode>.KeyRange keyRange in ConsistentHashShardManager<TPhysicalNode>.GetKeyRanges(newNodes))
      {
        if (oldNodes.Count == 0)
        {
          yield return new MigratedKeyRange<TPhysicalNode>(keyRange.StartKey, keyRange.EndKey, (VirtualNode<TPhysicalNode>) null, keyRange.Node);
        }
        else
        {
          VirtualNode<TPhysicalNode> node = ConsistentHashShardManager<TPhysicalNode>.FindNode(keyRange.StartKey, oldNodes);
          if ((long) node.NodeId != (long) keyRange.Node.NodeId)
            yield return new MigratedKeyRange<TPhysicalNode>(keyRange.StartKey, keyRange.EndKey, node, keyRange.Node);
        }
      }
    }

    private void Sort() => this.sortedNodes.Sort(VirtualNode<TPhysicalNode>.ComparerInstance);

    private void AddNodeInternal(TPhysicalNode node, int virtualNodeCount)
    {
      if (!this.uniqueNamesEncountered.Add(node.UniqueName))
        throw new ArgumentException(string.Format("Node with given UniqueName '{0}' already exists!", (object) node.UniqueName));
      for (int virtualNodeIndex = 0; virtualNodeIndex < virtualNodeCount; ++virtualNodeIndex)
      {
        ulong nodeId = this.ComputeNodeId(node, virtualNodeIndex);
        if (this.sortedNodes.Count > 0)
        {
          VirtualNode<TPhysicalNode> node1 = ConsistentHashShardManager<TPhysicalNode>.FindNode(nodeId, this.sortedNodes);
          if ((long) nodeId == (long) node1.NodeId)
            throw new ArgumentException("Node with computed NodeId already exists!");
        }
        this.sortedNodes.Add(new VirtualNode<TPhysicalNode>(node, virtualNodeIndex, virtualNodeCount, nodeId));
      }
    }

    public struct KeyRange
    {
      public readonly ulong StartKey;
      public readonly ulong EndKey;
      public readonly VirtualNode<TPhysicalNode> Node;

      public KeyRange(ulong startKey, ulong endKey, VirtualNode<TPhysicalNode> virtualNode)
      {
        this.StartKey = startKey;
        this.EndKey = endKey;
        this.Node = virtualNode;
      }
    }

    private sealed class VirtualNodeContextWithLock : VirtualNodeContext<TPhysicalNode>
    {
      private readonly IDisposable readerWriterLockReleaser;

      public VirtualNodeContextWithLock(
        VirtualNode<TPhysicalNode> virtualNode,
        VirtualNode<TPhysicalNode> previousNode,
        IDisposable readerWriterLockReleaser)
        : base(virtualNode, previousNode)
      {
        this.readerWriterLockReleaser = readerWriterLockReleaser;
      }

      protected override void Dispose(bool disposing) => this.readerWriterLockReleaser.Dispose();
    }

    private sealed class VirtualNodeContextWithKeysAndLock<TKey> : 
      VirtualNodeContextWithKeys<TKey, TPhysicalNode>
    {
      private readonly IDisposable readerWriterLockReleaser;

      public VirtualNodeContextWithKeysAndLock(
        VirtualNode<TPhysicalNode> virtualNode,
        VirtualNode<TPhysicalNode> previousNode,
        IEnumerable<KeyValuePair<TKey, ulong>> keys,
        IDisposable readerWriterLockReleaser)
        : base(virtualNode, previousNode, keys)
      {
        this.readerWriterLockReleaser = readerWriterLockReleaser;
      }

      protected override void Dispose(bool disposing) => this.readerWriterLockReleaser.Dispose();
    }
  }
}
