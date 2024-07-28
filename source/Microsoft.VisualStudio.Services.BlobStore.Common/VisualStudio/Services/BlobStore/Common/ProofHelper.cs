// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ProofHelper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public static class ProofHelper
  {
    public static IReadOnlyDictionary<DedupIdentifier, NodeDedupIdentifier> CreateParentLookup(
      IReadOnlyDictionary<NodeDedupIdentifier, DedupNode> allNodes)
    {
      return ProofHelper.CreateParentLookup(allNodes, ChunkerHelper.DefaultChunkHashType);
    }

    public static IReadOnlyDictionary<DedupIdentifier, NodeDedupIdentifier> CreateParentLookup(
      IReadOnlyDictionary<NodeDedupIdentifier, DedupNode> allNodes,
      HashType hashType)
    {
      Dictionary<DedupIdentifier, NodeDedupIdentifier> parentLookup = new Dictionary<DedupIdentifier, NodeDedupIdentifier>();
      foreach (KeyValuePair<NodeDedupIdentifier, DedupNode> allNode in (IEnumerable<KeyValuePair<NodeDedupIdentifier, DedupNode>>) allNodes)
      {
        foreach (DedupNode childNode in (IEnumerable<DedupNode>) allNode.Value.ChildNodes)
          parentLookup[childNode.GetDedupIdentifier()] = allNode.Key;
      }
      return (IReadOnlyDictionary<DedupIdentifier, NodeDedupIdentifier>) parentLookup;
    }

    public static HashSet<DedupNode> CreateProofNodes(
      IEnumerable<DedupNode> allNodes,
      IEnumerable<DedupIdentifier> dedupIds)
    {
      Dictionary<NodeDedupIdentifier, DedupNode> dictionary = allNodes.ToDictionary<DedupNode, NodeDedupIdentifier, DedupNode>((Func<DedupNode, NodeDedupIdentifier>) (n => n.CalculateNodeDedupIdentifier()), (Func<DedupNode, DedupNode>) (n => n));
      return ProofHelper.CreateProofNodes((IReadOnlyDictionary<NodeDedupIdentifier, DedupNode>) dictionary, ProofHelper.CreateParentLookup((IReadOnlyDictionary<NodeDedupIdentifier, DedupNode>) dictionary), dedupIds);
    }

    public static HashSet<DedupNode> CreateProofNodes(
      IEnumerable<DedupNode> allNodes,
      IEnumerable<DedupIdentifier> dedupIds,
      HashType hashType)
    {
      Dictionary<NodeDedupIdentifier, DedupNode> dictionary = allNodes.ToDictionary<DedupNode, NodeDedupIdentifier, DedupNode>((Func<DedupNode, NodeDedupIdentifier>) (n => n.CalculateNodeDedupIdentifier()), (Func<DedupNode, DedupNode>) (n => n));
      return ProofHelper.CreateProofNodes((IReadOnlyDictionary<NodeDedupIdentifier, DedupNode>) dictionary, ProofHelper.CreateParentLookup((IReadOnlyDictionary<NodeDedupIdentifier, DedupNode>) dictionary), dedupIds);
    }

    public static HashSet<DedupNode> CreateProofNodes(
      IReadOnlyDictionary<NodeDedupIdentifier, DedupNode> allNodes,
      IReadOnlyDictionary<DedupIdentifier, NodeDedupIdentifier> parentLookup,
      IEnumerable<DedupIdentifier> dedupIds)
    {
      HashSet<DedupNode> proofNodes = new HashSet<DedupNode>();
      foreach (DedupIdentifier dedupId in dedupIds)
      {
        DedupIdentifier key1 = dedupId;
        bool flag = false;
        NodeDedupIdentifier key2;
        for (; parentLookup.TryGetValue(key1, out key2); key1 = (DedupIdentifier) key2)
        {
          flag = true;
          DedupNode dedupNode;
          if (!allNodes.TryGetValue(key2, out dedupNode))
            throw new ArgumentException(string.Format("{0} did not contain the parent {1} of {2} which has hash {3}. Not found among {4} nodes.", (object) nameof (allNodes), (object) "DedupNode", (object) key1.ValueString, (object) key2.ValueString, (object) allNodes.Count));
          if (!proofNodes.Add(dedupNode))
            break;
        }
        if (!flag)
        {
          NodeDedupIdentifier key3 = !ChunkerHelper.IsChunk(dedupId.AlgorithmId) ? (NodeDedupIdentifier) dedupId : throw new ArgumentException(string.Format("{0} did not contain parent of {1} {2}. Not found among {3} nodes.", (object) nameof (parentLookup), (object) "ChunkLeaf", (object) dedupId.ValueString, (object) parentLookup.Count));
          DedupNode dedupNode;
          if (!allNodes.TryGetValue(key3, out dedupNode))
            throw new ArgumentException(string.Format("Neither {0} nor {1} contained a {2} for {3} {4}. Not found among {5} {6} nodes and {7} {8} nodes.", (object) nameof (parentLookup), (object) nameof (allNodes), (object) "DedupNode", (object) "InnerNode", (object) dedupId.ValueString, (object) parentLookup.Count, (object) nameof (parentLookup), (object) allNodes.Count, (object) nameof (allNodes)));
          proofNodes.Add(dedupNode);
        }
      }
      return proofNodes;
    }

    public static ISet<NodeDedupIdentifier> ApproximatelyOptimalMinCoverage(
      ISet<DedupNode> proofNodes,
      IDictionary<DedupIdentifier, ulong> idsToValidateWithSize)
    {
      return (ISet<NodeDedupIdentifier>) ProofHelper.ApproximatelyOptimalMinCoverage((IReadOnlyDictionary<NodeDedupIdentifier, DedupNode>) proofNodes.ToDictionary<DedupNode, NodeDedupIdentifier, DedupNode>((Func<DedupNode, NodeDedupIdentifier>) (n => n.CalculateNodeDedupIdentifier()), (Func<DedupNode, DedupNode>) (n => n)), idsToValidateWithSize);
    }

    public static ISet<NodeDedupIdentifier> ApproximatelyOptimalMinCoverage(
      ISet<DedupNode> proofNodes,
      IDictionary<DedupIdentifier, ulong> idsToValidateWithSize,
      HashType hashType)
    {
      return (ISet<NodeDedupIdentifier>) ProofHelper.ApproximatelyOptimalMinCoverage((IReadOnlyDictionary<NodeDedupIdentifier, DedupNode>) proofNodes.ToDictionary<DedupNode, NodeDedupIdentifier, DedupNode>((Func<DedupNode, NodeDedupIdentifier>) (n => n.CalculateNodeDedupIdentifier()), (Func<DedupNode, DedupNode>) (n => n)), idsToValidateWithSize, hashType);
    }

    public static HashSet<NodeDedupIdentifier> ApproximatelyOptimalMinCoverage(
      IReadOnlyDictionary<NodeDedupIdentifier, DedupNode> proofNodes,
      IDictionary<DedupIdentifier, ulong> idsToValidateWithClaimedSize)
    {
      return ProofHelper.ApproximatelyOptimalMinCoverage(proofNodes, idsToValidateWithClaimedSize, ChunkerHelper.DefaultChunkHashType);
    }

    public static HashSet<NodeDedupIdentifier> ApproximatelyOptimalMinCoverage(
      IReadOnlyDictionary<NodeDedupIdentifier, DedupNode> proofNodes,
      IDictionary<DedupIdentifier, ulong> idsToValidateWithClaimedSize,
      HashType hashType)
    {
      Dictionary<NodeDedupIdentifier, HashSet<DedupIdentifier>> dictionary1 = proofNodes.ToDictionary<KeyValuePair<NodeDedupIdentifier, DedupNode>, NodeDedupIdentifier, HashSet<DedupIdentifier>>((Func<KeyValuePair<NodeDedupIdentifier, DedupNode>, NodeDedupIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<NodeDedupIdentifier, DedupNode>, HashSet<DedupIdentifier>>) (_ => new HashSet<DedupIdentifier>()));
      Dictionary<DedupIdentifier, ulong> dictionary2 = new Dictionary<DedupIdentifier, ulong>(idsToValidateWithClaimedSize);
      Dictionary<DedupIdentifier, HashSet<NodeDedupIdentifier>> dictionary3 = dictionary2.ToDictionary<KeyValuePair<DedupIdentifier, ulong>, DedupIdentifier, HashSet<NodeDedupIdentifier>>((Func<KeyValuePair<DedupIdentifier, ulong>, DedupIdentifier>) (idToCover => idToCover.Key), (Func<KeyValuePair<DedupIdentifier, ulong>, HashSet<NodeDedupIdentifier>>) (_ => new HashSet<NodeDedupIdentifier>()));
      foreach (KeyValuePair<NodeDedupIdentifier, HashSet<DedupIdentifier>> keyValuePair in dictionary1)
        ProofHelper.FillCoverSet(proofNodes, (ISet<DedupIdentifier>) keyValuePair.Value, (IDictionary<DedupIdentifier, HashSet<NodeDedupIdentifier>>) dictionary3, (IDictionary<DedupIdentifier, ulong>) dictionary2, keyValuePair.Key, proofNodes[keyValuePair.Key], hashType);
      HashSet<NodeDedupIdentifier> nodeDedupIdentifierSet = new HashSet<NodeDedupIdentifier>();
      while (dictionary2.Any<KeyValuePair<DedupIdentifier, ulong>>())
      {
        if (!dictionary1.Any<KeyValuePair<NodeDedupIdentifier, HashSet<DedupIdentifier>>>())
          throw new ArgumentException(string.Format("ProofNodes do not cover all dedups. (DedupId: {0})", (object) dictionary2.First<KeyValuePair<DedupIdentifier, ulong>>()));
        int num = -1;
        NodeDedupIdentifier bestNodeId = (NodeDedupIdentifier) null;
        foreach (KeyValuePair<NodeDedupIdentifier, HashSet<DedupIdentifier>> keyValuePair in dictionary1)
        {
          if (keyValuePair.Value.Count > num)
          {
            num = keyValuePair.Value.Count;
            bestNodeId = keyValuePair.Key;
          }
        }
        nodeDedupIdentifierSet.Add(bestNodeId);
        HashSet<DedupIdentifier> dedupIdentifierSet = dictionary1[bestNodeId];
        dictionary1.Remove(bestNodeId);
        foreach (DedupIdentifier key1 in dedupIdentifierSet)
        {
          dictionary2.Remove(key1);
          HashSet<NodeDedupIdentifier> source;
          if (dictionary3.TryGetValue(key1, out source))
          {
            foreach (NodeDedupIdentifier key2 in source.Where<NodeDedupIdentifier>((Func<NodeDedupIdentifier, bool>) (n => (DedupIdentifier) n != (DedupIdentifier) bestNodeId)))
              dictionary1[key2].Remove(key1);
          }
        }
      }
      return nodeDedupIdentifierSet;
    }

    private static void FillCoverSet(
      IReadOnlyDictionary<NodeDedupIdentifier, DedupNode> proofNodes,
      ISet<DedupIdentifier> coverageSet,
      IDictionary<DedupIdentifier, HashSet<NodeDedupIdentifier>> nodesThatCover,
      IDictionary<DedupIdentifier, ulong> idsToCover,
      NodeDedupIdentifier currentNodeId,
      DedupNode currentNode,
      HashType hashType)
    {
      Action<DedupIdentifier, DedupNode> action = (Action<DedupIdentifier, DedupNode>) ((id, node) =>
      {
        if (!idsToCover.Keys.Contains(id))
          return;
        ulong transitiveContentBytes = node.TransitiveContentBytes;
        ulong num = idsToCover[id];
        if ((long) transitiveContentBytes != (long) num)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(string.Format("The dedup size is not consistent with ProofNodes: DedupId={0},", (object) id));
          stringBuilder.Append(string.Format("{0}={1}, ", (object) "proofNodeSize", (object) transitiveContentBytes));
          stringBuilder.Append(string.Format("{0}={1}", (object) "idToValidateWithClaimedSize", (object) num));
          throw new ArgumentException(stringBuilder.ToString());
        }
        coverageSet.Add(id);
        nodesThatCover[id].Add(currentNodeId);
      });
      if (coverageSet.Count == 0)
        action((DedupIdentifier) currentNodeId, currentNode);
      foreach (DedupNode childNode in (IEnumerable<DedupNode>) currentNode.ChildNodes)
      {
        DedupIdentifier dedupIdentifier = childNode.GetDedupIdentifier();
        action(dedupIdentifier, childNode);
        NodeDedupIdentifier nodeDedupIdentifier = dedupIdentifier as NodeDedupIdentifier;
        DedupNode currentNode1;
        if ((DedupIdentifier) nodeDedupIdentifier != (DedupIdentifier) null && proofNodes.TryGetValue(nodeDedupIdentifier, out currentNode1))
          ProofHelper.FillCoverSet(proofNodes, coverageSet, nodesThatCover, idsToCover, nodeDedupIdentifier, currentNode1, hashType);
      }
    }

    public static IEnumerable<DedupIdentifier> DetermineUnvalidatedIds(
      ISet<DedupNode> proofNodes,
      ISet<NodeDedupIdentifier> validatedRoots,
      IDictionary<DedupIdentifier, ulong> idsToValidate)
    {
      return ProofHelper.DetermineUnvalidatedIds(proofNodes, validatedRoots, idsToValidate, ChunkerHelper.DefaultChunkHashType);
    }

    public static IEnumerable<DedupIdentifier> DetermineUnvalidatedIds(
      ISet<DedupNode> proofNodes,
      ISet<NodeDedupIdentifier> validatedRoots,
      IDictionary<DedupIdentifier, ulong> idsToValidate,
      HashType hashType)
    {
      return ProofHelper.DetermineUnvalidatedIds((IDictionary<NodeDedupIdentifier, DedupNode>) proofNodes.ToDictionary<DedupNode, NodeDedupIdentifier, DedupNode>((Func<DedupNode, NodeDedupIdentifier>) (n => n.CalculateNodeDedupIdentifier()), (Func<DedupNode, DedupNode>) (n => n)), validatedRoots, idsToValidate, hashType);
    }

    public static IEnumerable<DedupIdentifier> DetermineUnvalidatedIds(
      IDictionary<NodeDedupIdentifier, DedupNode> proofNodes,
      ISet<NodeDedupIdentifier> roots,
      IDictionary<DedupIdentifier, ulong> idsToValidate)
    {
      return ProofHelper.DetermineUnvalidatedIds(proofNodes, roots, idsToValidate, ChunkerHelper.DefaultChunkHashType);
    }

    public static IEnumerable<DedupIdentifier> DetermineUnvalidatedIds(
      IDictionary<NodeDedupIdentifier, DedupNode> proofNodes,
      ISet<NodeDedupIdentifier> roots,
      IDictionary<DedupIdentifier, ulong> idsToValidate,
      HashType hashType)
    {
      HashSet<DedupIdentifier> dedupIdentifierSet = new HashSet<DedupIdentifier>();
      Dictionary<DedupIdentifier, ulong> validIds = new Dictionary<DedupIdentifier, ulong>();
      Queue<NodeDedupIdentifier> source = new Queue<NodeDedupIdentifier>((IEnumerable<NodeDedupIdentifier>) roots);
      while (source.Any<NodeDedupIdentifier>())
      {
        NodeDedupIdentifier key = source.Dequeue();
        DedupNode dedupNode;
        if (dedupIdentifierSet.Add((DedupIdentifier) key) && proofNodes.TryGetValue(key, out dedupNode))
        {
          if (!validIds.ContainsKey((DedupIdentifier) key))
            validIds.Add((DedupIdentifier) key, dedupNode.TransitiveContentBytes);
          if (dedupNode.ChildNodes != null)
          {
            foreach (DedupNode childNode in (IEnumerable<DedupNode>) dedupNode.ChildNodes)
            {
              DedupIdentifier dedupIdentifier = childNode.GetDedupIdentifier();
              if (!validIds.ContainsKey(dedupIdentifier))
                validIds.Add(dedupIdentifier, childNode.TransitiveContentBytes);
              NodeDedupIdentifier nodeDedupIdentifier = dedupIdentifier as NodeDedupIdentifier;
              if ((DedupIdentifier) nodeDedupIdentifier != (DedupIdentifier) null)
                source.Enqueue(nodeDedupIdentifier);
            }
          }
        }
      }
      foreach (KeyValuePair<DedupIdentifier, ulong> keyValuePair in (IEnumerable<KeyValuePair<DedupIdentifier, ulong>>) idsToValidate)
      {
        if (!validIds.ContainsKey(keyValuePair.Key))
          yield return keyValuePair.Key;
        else if ((long) keyValuePair.Value != (long) validIds[keyValuePair.Key])
          yield return keyValuePair.Key;
      }
    }
  }
}
