// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.DedupTreeBuilder
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public class DedupTreeBuilder
  {
    private IDedupProcessingQueue processingQueue;
    public const int IdentifiersPerNode = 512;
    private List<LinkedList<DedupNode>> identifiers = new List<LinkedList<DedupNode>>();

    public DedupTreeBuilder(IDedupProcessingQueue processingQueue)
    {
      this.processingQueue = processingQueue;
      this.identifiers.Add(new LinkedList<DedupNode>());
    }

    public void AddNode(DedupNode newNode)
    {
      this.identifiers[0].AddLast(newNode);
      for (int index = 0; index < this.identifiers.Count; ++index)
      {
        if (this.identifiers[index].Count<DedupNode>() >= 512)
        {
          DedupNode removeIdentifiers = this.CreateNodeAndRemoveIdentifiers(this.identifiers[index], 512);
          if (this.identifiers.Count == index + 1)
            this.identifiers.Add(new LinkedList<DedupNode>());
          this.identifiers[index + 1].AddLast(removeIdentifiers);
        }
      }
    }

    public DedupIdentifier CreateRootNode()
    {
      LinkedList<DedupNode> identifiers = this.FlattenLevels(this.identifiers);
      if (identifiers.Count == 0)
        return ChunkBlobHasher.Instance.OfNothing.ToDedupIdentifier();
      return identifiers.Count == 1 ? identifiers.First.Value.GetDedupIdentifier() : (DedupIdentifier) this.CreateNodeAndRemoveIdentifiers(identifiers, identifiers.Count).GetNodeIdentifier();
    }

    private DedupNode CreateNodeAndRemoveIdentifiers(LinkedList<DedupNode> identifiers, int count)
    {
      List<DedupNode> childNodes = new List<DedupNode>();
      while (childNodes.Count < count && identifiers.Count > 0)
      {
        childNodes.Add(identifiers.First<DedupNode>());
        identifiers.RemoveFirst();
      }
      DedupNode node = new DedupNode((IEnumerable<DedupNode>) childNodes);
      this.processingQueue.Add((DedupIdentifier) node.GetNodeIdentifier(), node.Serialize());
      return new DedupNode(node.Type, node.TransitiveContentBytes, node.Hash, node.Height);
    }

    private LinkedList<DedupNode> FlattenLevels(List<LinkedList<DedupNode>> identifiers)
    {
      int index = 1;
      LinkedList<DedupNode> identifier1 = identifiers[0];
      for (; index < identifiers.Count; ++index)
      {
        LinkedList<DedupNode> identifier2 = identifiers[index - 1];
        identifier1 = identifiers[index];
        while (identifier2.Count > 0)
        {
          LinkedListNode<DedupNode> first = identifier2.First;
          identifier2.RemoveFirst();
          identifier1.AddLast(first);
        }
        if (identifier1.Count<DedupNode>() >= 512)
        {
          DedupNode removeIdentifiers = this.CreateNodeAndRemoveIdentifiers(identifier1, 512);
          identifier1.AddFirst(removeIdentifiers);
        }
      }
      return identifier1;
    }
  }
}
