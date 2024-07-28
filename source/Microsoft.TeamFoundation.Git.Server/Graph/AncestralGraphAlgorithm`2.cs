// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.AncestralGraphAlgorithm`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class AncestralGraphAlgorithm<TLabel, TVertex>
  {
    private Heap<TLabel, long> m_prioQueue;
    private Queue<AncestralGraphAlgorithm<TLabel, TVertex>.LabelDistancePair> m_bfsQueue;
    private HashSet<TLabel> m_reachedLabels;
    private HashSet<TLabel> m_targetSet;
    private int m_inUse;

    public IEnumerable<TLabel> OrderByLabels(
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TLabel> reachableFromSet = null,
      IReadOnlySet<TLabel> restrictedLabels = null,
      PriorityDelegate<TLabel> getPriorityOfLabel = null,
      PriorityDelegate<TVertex> getPriorityOfVertex = null)
    {
      AncestralGraphAlgorithm<TLabel, TVertex> parent = this;
      if (reachableFromSet == null)
        reachableFromSet = graph.GetLabels(graph.GetVertices());
      if (restrictedLabels == null)
        restrictedLabels = (IReadOnlySet<TLabel>) ReadOnlyUniversalSet<TLabel>.Instance;
      using (new AncestralGraphAlgorithm<TLabel, TVertex>.AlgorithmLock(parent))
      {
        HashSet<TLabel> reachedLabels = parent.GetReachedLabels();
        Heap<TLabel, long> degreeQueue = new Heap<TLabel, long>((IComparer<long>) new StackComparer());
        Dictionary<TLabel, int> inDegrees = new Dictionary<TLabel, int>();
        Heap<TLabel, long> topoWalkQueue = new Heap<TLabel, long>((IComparer<long>) new StackComparer());
        long storedPriority = 1;
        int minAncestryDepth1 = parent.GetMinAncestryDepth(graph, reachableFromSet);
        foreach (TLabel reachableFrom in reachableFromSet)
        {
          if (restrictedLabels.Contains(reachableFrom) && !reachedLabels.Contains(reachableFrom))
          {
            int ancestryDepth = graph.GetAncestryDepth(reachableFrom);
            if (!inDegrees.ContainsKey(reachableFrom))
            {
              degreeQueue.Enqueue(reachableFrom, (long) ancestryDepth);
              inDegrees.Add(reachableFrom, 0);
            }
          }
        }
        parent.AdvanceInDegreeWalk(graph, degreeQueue, inDegrees, restrictedLabels, reachedLabels, minAncestryDepth1);
        foreach (TLabel reachableFrom in reachableFromSet)
        {
          int num;
          if (inDegrees.TryGetValue(reachableFrom, out num) && num == 0 && reachedLabels.Add(reachableFrom))
            topoWalkQueue.Enqueue(reachableFrom, GraphReachabilityAlgorithmHelpers.GetIncrementalPriority<TLabel, TVertex>(graph, reachableFrom, ref storedPriority, getPriorityOfLabel, getPriorityOfVertex));
        }
        TLabel current;
        while (topoWalkQueue.TryDequeueBeforeThreshold(long.MinValue, out current))
        {
          yield return current;
          int minAncestryDepth2 = parent.GetMinAncestryDepth(graph, graph.OutNeighborsOfLabel(current), reachedLabels);
          parent.AdvanceInDegreeWalk(graph, degreeQueue, inDegrees, restrictedLabels, reachedLabels, minAncestryDepth2);
          foreach (TLabel label in graph.OutNeighborsOfLabel(current))
          {
            int num;
            if (inDegrees.TryGetValue(label, out num))
            {
              inDegrees[label] = num - 1;
              if (num == 1 && reachedLabels.Add(label))
                topoWalkQueue.Enqueue(label, GraphReachabilityAlgorithmHelpers.GetIncrementalPriority<TLabel, TVertex>(graph, label, ref storedPriority, getPriorityOfLabel, getPriorityOfVertex));
            }
          }
        }
        reachedLabels = (HashSet<TLabel>) null;
        degreeQueue = (Heap<TLabel, long>) null;
        inDegrees = (Dictionary<TLabel, int>) null;
        topoWalkQueue = (Heap<TLabel, long>) null;
        current = default (TLabel);
      }
    }

    private void AdvanceInDegreeWalk(
      IDirectedGraph<TLabel, TVertex> graph,
      Heap<TLabel, long> degreeQueue,
      Dictionary<TLabel, int> inDegrees,
      IReadOnlySet<TLabel> restrictedLabels,
      HashSet<TLabel> reachedLabels,
      int goalAncestryDepth)
    {
      TLabel label1;
      while (degreeQueue.TryDequeueBeforeThreshold((long) (goalAncestryDepth - 1), out label1))
      {
        foreach (TLabel label2 in graph.OutNeighborsOfLabel(label1))
        {
          if (restrictedLabels.Contains(label2) && !reachedLabels.Contains(label2))
          {
            int num;
            if (inDegrees.TryGetValue(label2, out num))
            {
              inDegrees[label2] = num + 1;
            }
            else
            {
              inDegrees.Add(label2, 1);
              degreeQueue.Enqueue(label2, (long) graph.GetAncestryDepth(label2));
            }
          }
        }
      }
    }

    public IEnumerable<TLabel> OrderLabelsByAncestryDepth(
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TLabel> reachableFromSet = null,
      IReadOnlySet<TLabel> restrictedLabels = null,
      bool firstParentOnly = false)
    {
      AncestralGraphAlgorithm<TLabel, TVertex> parent = this;
      if (reachableFromSet == null)
        reachableFromSet = graph.GetLabels(graph.GetVertices());
      if (restrictedLabels == null)
        restrictedLabels = (IReadOnlySet<TLabel>) ReadOnlyUniversalSet<TLabel>.Instance;
      using (new AncestralGraphAlgorithm<TLabel, TVertex>.AlgorithmLock(parent))
      {
        HashSet<TLabel> reachedLabels = parent.GetReachedLabels();
        Heap<TLabel, long> queue = new Heap<TLabel, long>((IComparer<long>) new StackComparer());
        foreach (TLabel reachableFrom in reachableFromSet)
          AddToQueue(reachableFrom);
        TLabel current;
        while (queue.TryDequeueBeforeThreshold(long.MinValue, out current))
        {
          yield return current;
          graph.ForEachOutNeighborsOfLabel(current, (Predicate<TLabel>) (x =>
          {
            AddToQueue(x);
            return !firstParentOnly;
          }));
        }

        void AddToQueue(TLabel label)
        {
          if (!reachedLabels.Add(label) || !restrictedLabels.Contains(label))
            return;
          queue.Enqueue(label, (long) graph.GetAncestryDepth(label));
        }
      }
    }

    public IEnumerable<TVertex> GetReachable(
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TVertex> reachableFrom,
      IReadOnlySet<TLabel> restrictedLabels = null,
      int maxDistance = 2147483647)
    {
      return graph.GetVertices(this.GetReachableLabels(graph, graph.GetLabels(reachableFrom), restrictedLabels, maxDistance));
    }

    public IEnumerable<TLabel> GetReachableLabels(
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TLabel> reachableFrom,
      IReadOnlySet<TLabel> restrictedLabels = null,
      int maxDistance = 2147483647)
    {
      AncestralGraphAlgorithm<TLabel, TVertex> parent = this;
      if (restrictedLabels == null)
        restrictedLabels = (IReadOnlySet<TLabel>) ReadOnlyUniversalSet<TLabel>.Instance;
      using (new AncestralGraphAlgorithm<TLabel, TVertex>.AlgorithmLock(parent))
      {
        HashSet<TLabel> reachedLabels = parent.GetReachedLabels();
        Heap<AncestralGraphAlgorithm<TLabel, TVertex>.LabelDistancePair, long> walkQueue = new Heap<AncestralGraphAlgorithm<TLabel, TVertex>.LabelDistancePair, long>((IComparer<long>) new StackComparer());
        foreach (TLabel label in reachableFrom)
        {
          if (restrictedLabels.Contains(label) && reachedLabels.Add(label))
            walkQueue.Enqueue(new AncestralGraphAlgorithm<TLabel, TVertex>.LabelDistancePair(label, 0), (long) graph.GetAncestryDepth(label));
        }
        AncestralGraphAlgorithm<TLabel, TVertex>.LabelDistancePair current;
        while (walkQueue.TryDequeueBeforeThreshold(long.MinValue, out current))
        {
          yield return current.Label;
          if (current.Distance < maxDistance)
          {
            foreach (TLabel label in graph.OutNeighborsOfLabel(current.Label))
            {
              if (restrictedLabels.Contains(label) && reachedLabels.Add(label))
                walkQueue.Enqueue(new AncestralGraphAlgorithm<TLabel, TVertex>.LabelDistancePair(label, current.Distance + 1), (long) graph.GetAncestryDepth(label));
            }
          }
        }
        reachedLabels = (HashSet<TLabel>) null;
        walkQueue = (Heap<AncestralGraphAlgorithm<TLabel, TVertex>.LabelDistancePair, long>) null;
        current = new AncestralGraphAlgorithm<TLabel, TVertex>.LabelDistancePair();
      }
    }

    public ReachableSetAndBoundary<TVertex> GetReachableWithBoundary(
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TVertex> reachableFrom,
      IEnumerable<TVertex> notReachableFrom)
    {
      ReachableSetAndBoundary<TLabel> labelsWithBoundary = this.GetReachableLabelsWithBoundary(graph, graph.GetLabels(reachableFrom), graph.GetLabels(notReachableFrom));
      return new ReachableSetAndBoundary<TVertex>((ISet<TVertex>) new HashSet<TVertex>(graph.GetVertices(labelsWithBoundary.ReachableSet)), (ISet<TVertex>) new HashSet<TVertex>(graph.GetVertices(labelsWithBoundary.Boundary)));
    }

    private ReachableSetAndBoundary<TLabel> GetReachableLabelsWithBoundary(
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TLabel> reachableFrom,
      IEnumerable<TLabel> notReachableFrom)
    {
      HashSet<TLabel> boundary = new HashSet<TLabel>();
      using (new AncestralGraphAlgorithm<TLabel, TVertex>.AlgorithmLock(this))
      {
        HashSet<TLabel> reachedLabels = this.GetReachedLabels();
        NotReachableLabels<TLabel, TVertex> labelsToWalk = new NotReachableLabels<TLabel, TVertex>(graph, notReachableFrom);
        SubgraphWrapper<TLabel, TVertex> subgraphWrapper = new SubgraphWrapper<TLabel, TVertex>(graph, (Predicate<TLabel>) (label => labelsToWalk.Contains(label)), true);
        Queue<TLabel> labelQueue = new Queue<TLabel>();
        this.GetMinAncestryDepth(graph, reachableFrom);
        foreach (TLabel label in reachableFrom)
        {
          if (labelsToWalk.Contains(label) && reachedLabels.Add(label))
            labelQueue.Enqueue(label);
          else
            boundary.Add(label);
        }
        while (labelQueue.Count > 0)
        {
          TLabel label1 = labelQueue.Dequeue();
          foreach (TLabel label2 in graph.OutNeighborsOfLabel(label1))
          {
            if (!labelsToWalk.Contains(label2))
              boundary.Add(label2);
            else if (reachedLabels.Add(label2))
              labelQueue.Enqueue(label2);
          }
        }
        return new ReachableSetAndBoundary<TLabel>((ISet<TLabel>) reachedLabels, (ISet<TLabel>) boundary);
      }
    }

    public bool CanReach(
      IDirectedGraph<TLabel, TVertex> graph,
      TVertex source,
      TVertex target,
      int maxDistance = 2147483647)
    {
      return this.CanReachLabels(graph, graph.GetLabel(source), graph.GetLabel(target), maxDistance);
    }

    public bool CanReachLabels(
      IDirectedGraph<TLabel, TVertex> graph,
      TLabel source,
      TLabel target,
      int maxDistance = 2147483647)
    {
      if (source.Equals((object) target))
        return true;
      return this.CanReachLabels(graph, source, (IEnumerable<TLabel>) new TLabel[1]
      {
        target
      }, maxDistance);
    }

    public bool CanReach(
      IDirectedGraph<TLabel, TVertex> graph,
      TVertex source,
      IEnumerable<TVertex> targets,
      int maxDistance = 2147483647)
    {
      return this.CanReachLabels(graph, graph.GetLabel(source), graph.GetLabels(targets), maxDistance);
    }

    public bool CanReachLabels(
      IDirectedGraph<TLabel, TVertex> graph,
      TLabel source,
      IEnumerable<TLabel> targets,
      int maxDistance = 2147483647)
    {
      int ancestryDepth1 = graph.GetAncestryDepth(source);
      int val2 = ancestryDepth1;
      using (new AncestralGraphAlgorithm<TLabel, TVertex>.AlgorithmLock(this))
      {
        HashSet<TLabel> reachedLabels = this.GetReachedLabels();
        HashSet<TLabel> targetSet = this.GetTargetSet();
        Queue<AncestralGraphAlgorithm<TLabel, TVertex>.LabelDistancePair> bfsQueue = this.GetBfsQueue();
        foreach (TLabel target in targets)
        {
          if (target.Equals((object) source))
            return true;
          int ancestryDepth2 = graph.GetAncestryDepth(target);
          if (ancestryDepth2 <= ancestryDepth1)
          {
            targetSet.Add(target);
            val2 = Math.Min(ancestryDepth2, val2);
          }
        }
        if (targetSet.Count == 0)
          return false;
        bfsQueue.Enqueue(new AncestralGraphAlgorithm<TLabel, TVertex>.LabelDistancePair(source, 0));
        reachedLabels.Add(source);
        while (bfsQueue.Count > 0)
        {
          AncestralGraphAlgorithm<TLabel, TVertex>.LabelDistancePair labelDistancePair = bfsQueue.Dequeue();
          if (labelDistancePair.Distance < maxDistance && graph.GetAncestryDepth(labelDistancePair.Label) >= val2)
          {
            foreach (TLabel label in graph.OutNeighborsOfLabel(labelDistancePair.Label))
            {
              if (targetSet.Contains(label))
                return true;
              if (reachedLabels.Add(label) && graph.GetAncestryDepth(label) >= val2)
                bfsQueue.Enqueue(new AncestralGraphAlgorithm<TLabel, TVertex>.LabelDistancePair(label, labelDistancePair.Distance + 1));
            }
          }
        }
        return false;
      }
    }

    public bool TryGetMergeBase(
      IDirectedGraph<TLabel, TVertex> graph,
      TVertex v1,
      TVertex v2,
      out TVertex mergeBase)
    {
      List<TVertex> mergeBases;
      int num = this.TryGetMergeBases(graph, v1, v2, out mergeBases) ? 1 : 0;
      mergeBase = mergeBases.FirstOrDefault<TVertex>();
      return num != 0;
    }

    public bool HasMultipleMergeBases(
      IDirectedGraph<TLabel, TVertex> graph,
      TVertex v1,
      TVertex v2)
    {
      List<TVertex> mergeBases;
      return this.TryGetMergeBases(graph, v1, v2, out mergeBases) && mergeBases.Count > 1;
    }

    public bool TryGetMergeBases(
      IDirectedGraph<TLabel, TVertex> graph,
      TVertex v1,
      TVertex v2,
      out List<TVertex> mergeBases)
    {
      mergeBases = new List<TVertex>();
      TLabel label1 = graph.GetLabel(v1);
      TLabel label2 = graph.GetLabel(v2);
      Dictionary<TLabel, byte> dictionary = new Dictionary<TLabel, byte>();
      using (new AncestralGraphAlgorithm<TLabel, TVertex>.AlgorithmLock(this))
      {
        HashSet<TLabel> reachedLabels = this.GetReachedLabels();
        Heap<TLabel, long> heap = this.GetHeap();
        reachedLabels.Add(label1);
        if (reachedLabels.Add(label2))
        {
          dictionary[label1] = (byte) 1;
          dictionary[label2] = (byte) 2;
          heap.Enqueue(label1, (long) graph.GetAncestryDepth(label1));
          heap.Enqueue(label2, (long) graph.GetAncestryDepth(label2));
          TLabel label3;
          while (heap.TryDequeueBeforeThreshold(long.MinValue, out label3))
          {
            byte num1 = dictionary[label3];
            dictionary.Remove(label3);
            if (num1 == (byte) 3)
            {
              mergeBases.Add(graph.GetVertex(label3));
            }
            else
            {
              foreach (TLabel label4 in graph.OutNeighborsOfLabel(label3))
              {
                long ancestryDepth = (long) graph.GetAncestryDepth(label4);
                if (reachedLabels.Add(label4))
                  heap.Enqueue(label4, ancestryDepth);
                byte num2;
                dictionary[label4] = !dictionary.TryGetValue(label4, out num2) ? num1 : (byte) ((uint) num1 | (uint) num2);
              }
            }
          }
        }
        else
        {
          mergeBases.Add(v1);
          return true;
        }
      }
      return mergeBases.Any<TVertex>();
    }

    public IReadOnlyList<AheadBehind<TVertex>> GetAheadBehind(
      IDirectedGraph<TLabel, TVertex> graph,
      TVertex baseVertex,
      IEnumerable<TVertex> compareVertices)
    {
      return (IReadOnlyList<AheadBehind<TVertex>>) this.GetAheadBehindLabels(graph, graph.GetLabel(baseVertex), graph.GetLabels(compareVertices).ToList<TLabel>()).Select<AheadBehind<TLabel>, AheadBehind<TVertex>>((Func<AheadBehind<TLabel>, AheadBehind<TVertex>>) (aheadBehind => new AheadBehind<TVertex>(graph.GetVertex(aheadBehind.Item), aheadBehind.NumAhead, aheadBehind.NumBehind))).ToList<AheadBehind<TVertex>>().AsReadOnly();
    }

    private List<AheadBehind<TLabel>> GetAheadBehindLabels(
      IDirectedGraph<TLabel, TVertex> graph,
      TLabel baseLabel,
      List<TLabel> compareLabels)
    {
      if (compareLabels.Count == 0)
        return new List<AheadBehind<TLabel>>();
      int count = compareLabels.Count;
      int length = compareLabels.Count + 1;
      Dictionary<TLabel, BitArray> dictionary = new Dictionary<TLabel, BitArray>();
      using (new AncestralGraphAlgorithm<TLabel, TVertex>.AlgorithmLock(this))
      {
        HashSet<TLabel> reachedLabels = this.GetReachedLabels();
        Heap<TLabel, long> heap = this.GetHeap();
        reachedLabels.Add(baseLabel);
        dictionary[baseLabel] = new BitArray(length)
        {
          [count] = true
        };
        long ancestryDepth1 = (long) graph.GetAncestryDepth(baseLabel);
        long val2 = ancestryDepth1;
        heap.Enqueue(baseLabel, ancestryDepth1);
        int index1 = 0;
        foreach (TLabel compareLabel in compareLabels)
        {
          if (reachedLabels.Add(compareLabel))
          {
            dictionary[compareLabel] = new BitArray(length)
            {
              [index1] = true
            };
            long ancestryDepth2 = (long) graph.GetAncestryDepth(compareLabel);
            heap.Enqueue(compareLabel, ancestryDepth2);
            val2 = Math.Min(ancestryDepth2, val2);
          }
          else
            dictionary[compareLabel].Set(index1, true);
          ++index1;
        }
        int[] numArray1 = new int[compareLabels.Count];
        int[] numArray2 = new int[compareLabels.Count];
        TLabel label1;
        while (heap.TryDequeueBeforeThreshold(val2 - 1L, out label1))
        {
          BitArray bits = dictionary[label1];
          dictionary.Remove(label1);
          if (bits[count])
          {
            for (int index2 = 0; index2 < compareLabels.Count; ++index2)
            {
              if (!bits[index2])
                ++numArray2[index2];
            }
          }
          else
          {
            for (int index3 = 0; index3 < compareLabels.Count; ++index3)
            {
              if (bits[index3])
                ++numArray1[index3];
            }
          }
          foreach (TLabel label2 in graph.OutNeighborsOfLabel(label1))
          {
            long ancestryDepth3 = (long) graph.GetAncestryDepth(label2);
            if (reachedLabels.Add(label2))
              heap.Enqueue(label2, ancestryDepth3);
            BitArray bitArray;
            if (dictionary.TryGetValue(label2, out bitArray))
            {
              bitArray.Or(bits);
            }
            else
            {
              bitArray = new BitArray(bits);
              dictionary[label2] = bitArray;
            }
            if (ancestryDepth3 < val2 && !this.BitArrayAllBitsOn(bitArray))
              val2 = ancestryDepth3;
          }
        }
        List<AheadBehind<TLabel>> aheadBehindLabels = new List<AheadBehind<TLabel>>();
        for (int index4 = 0; index4 < compareLabels.Count; ++index4)
          aheadBehindLabels.Add(new AheadBehind<TLabel>(compareLabels[index4], numArray1[index4], numArray2[index4]));
        return aheadBehindLabels;
      }
    }

    private bool BitArrayAllBitsOn(BitArray bitArray)
    {
      for (int index = 0; index < bitArray.Count; ++index)
      {
        if (!bitArray[index])
          return false;
      }
      return true;
    }

    public bool UseGetLabelsThatCanReachNew { get; set; }

    public IEnumerable<TLabel> GetLabelsThatCanReach(
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TLabel> labelsToTest,
      IEnumerable<TLabel> labelsToReach)
    {
      return !this.UseGetLabelsThatCanReachNew ? this.GetLabelsThatCanReachOld(graph, labelsToTest, labelsToReach) : this.GetLabelsThatCanReachNew(graph, labelsToTest, labelsToReach);
    }

    public IEnumerable<TLabel> GetLabelsThatCanReachOld(
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TLabel> labelsToTest,
      IEnumerable<TLabel> labelsToReach)
    {
      int minDepth = graph.NumVertices;
      HashSet<TLabel> targetSet = this.GetTargetSet();
      foreach (TLabel label in labelsToReach)
      {
        if (targetSet.Add(label))
        {
          int ancestryDepth = graph.GetAncestryDepth(label);
          if (ancestryDepth < minDepth)
            minDepth = ancestryDepth;
        }
      }
      if (targetSet.Count != 0)
      {
        List<TLabel> orderedLabelsToTest = new List<TLabel>();
        foreach (TLabel label in labelsToTest)
        {
          if (targetSet.Contains(label))
            yield return label;
          else if (graph.GetAncestryDepth(label) > minDepth)
            orderedLabelsToTest.Add(label);
        }
        orderedLabelsToTest.Sort((Comparison<TLabel>) ((l1, l2) => graph.GetAncestryDepth(l1) - graph.GetAncestryDepth(l2)));
        HashSet<TLabel> reachedLabels = this.GetReachedLabels();
        Heap<TLabel, long> prio = this.GetHeap();
        Dictionary<TLabel, HashSet<TLabel>> reverseEdges = new Dictionary<TLabel, HashSet<TLabel>>();
        for (int i = 0; i < orderedLabelsToTest.Count; ++i)
        {
          TLabel outerLabel = orderedLabelsToTest[i];
          if (targetSet.Contains(outerLabel))
          {
            yield return outerLabel;
          }
          else
          {
            if (!reverseEdges.ContainsKey(outerLabel))
              reverseEdges.Add(outerLabel, new HashSet<TLabel>());
            reachedLabels.Add(outerLabel);
            prio.Enqueue(outerLabel, (long) graph.GetAncestryDepth(outerLabel));
            Queue<TLabel> bfsQueue = new Queue<TLabel>();
            bool foundOuterLabel = false;
            TLabel label1;
            while (!foundOuterLabel && prio.TryDequeueBeforeThreshold(long.MinValue, out label1))
            {
              foreach (TLabel label2 in graph.OutNeighborsOfLabel(label1))
              {
                if (targetSet.Contains(label2) && targetSet.Add(label1))
                {
                  bfsQueue.Clear();
                  bfsQueue.Enqueue(label1);
                  while (bfsQueue.Count > 0)
                  {
                    TLabel key = bfsQueue.Dequeue();
                    foreach (TLabel label3 in reverseEdges[key])
                    {
                      if (targetSet.Add(label3))
                        bfsQueue.Enqueue(label3);
                    }
                    reverseEdges.Remove(key);
                  }
                  if (targetSet.Contains(outerLabel))
                  {
                    foundOuterLabel = true;
                    yield return outerLabel;
                    break;
                  }
                  break;
                }
                if (graph.GetAncestryDepth(label2) > minDepth)
                {
                  if (reachedLabels.Add(label2))
                    prio.Enqueue(label2, (long) graph.GetAncestryDepth(label2));
                  HashSet<TLabel> labelSet;
                  if (reverseEdges.TryGetValue(label2, out labelSet))
                  {
                    labelSet.Add(label1);
                  }
                  else
                  {
                    labelSet = new HashSet<TLabel>();
                    labelSet.Add(label1);
                    reverseEdges.Add(label2, labelSet);
                  }
                }
              }
            }
            outerLabel = default (TLabel);
            bfsQueue = (Queue<TLabel>) null;
          }
        }
      }
    }

    private IEnumerable<TLabel> GetLabelsThatCanReachNew(
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TLabel> labelsToTest,
      IEnumerable<TLabel> labelsToReach)
    {
      HashSet<TLabel> labelsThatCanReach = this.GetTargetSet();
      labelsThatCanReach.UnionWith(labelsToReach);
      if (labelsThatCanReach.Count == 0)
        return Enumerable.Empty<TLabel>();
      NotReachableLabels<TLabel, TVertex> restrictedLabels = new NotReachableLabels<TLabel, TVertex>(graph, (IEnumerable<TLabel>) labelsThatCanReach, true);
      List<TLabel> labelsInPaths = GetLabelsInPaths(labelsToTest, true).ToList<TLabel>();
      GrowLabelsToReachInReverseTopoOrder();
      labelsInPaths.Clear();
      labelsInPaths.AddRange(GetLabelsInPaths(labelsToTest.Where<TLabel>((Func<TLabel, bool>) (x => !labelsThatCanReach.Contains(x))), false));
      GrowLabelsToReachInReverseTopoOrder();
      return labelsToTest.Where<TLabel>((Func<TLabel, bool>) (x => labelsThatCanReach.Contains(x)));

      IEnumerable<TLabel> GetLabelsInPaths(
        IEnumerable<TLabel> sublabelsToTest,
        bool firstParentOnly)
      {
        return this.OrderLabelsByAncestryDepth(graph, sublabelsToTest, (IReadOnlySet<TLabel>) restrictedLabels, firstParentOnly);
      }

      void GrowLabelsToReachInReverseTopoOrder()
      {
        TLabel l = default (TLabel);
        Predicate<TLabel> predicate = (Predicate<TLabel>) (outNeighborOfL =>
        {
          if (!labelsThatCanReach.Contains(outNeighborOfL))
            return true;
          labelsThatCanReach.Add(l);
          return false;
        });
        for (int index = labelsInPaths.Count - 1; index >= 0; --index)
        {
          l = labelsInPaths[index];
          graph.ForEachOutNeighborsOfLabel(l, predicate);
        }
      }
    }

    private int GetMinAncestryDepth(
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TLabel> labels,
      HashSet<TLabel> avoidSet = null)
    {
      int minAncestryDepth = int.MaxValue;
      foreach (TLabel label in labels)
      {
        // ISSUE: explicit non-virtual call
        if (avoidSet == null || !__nonvirtual (avoidSet.Contains(label)))
        {
          int ancestryDepth = graph.GetAncestryDepth(label);
          if (minAncestryDepth > ancestryDepth)
            minAncestryDepth = ancestryDepth;
        }
      }
      return minAncestryDepth;
    }

    private Heap<TLabel, long> GetHeap()
    {
      if (this.m_prioQueue == null)
        this.m_prioQueue = new Heap<TLabel, long>((IComparer<long>) new StackComparer());
      else
        this.m_prioQueue.Clear();
      return this.m_prioQueue;
    }

    private Queue<AncestralGraphAlgorithm<TLabel, TVertex>.LabelDistancePair> GetBfsQueue()
    {
      if (this.m_bfsQueue == null)
        this.m_bfsQueue = new Queue<AncestralGraphAlgorithm<TLabel, TVertex>.LabelDistancePair>();
      else
        this.m_bfsQueue.Clear();
      return this.m_bfsQueue;
    }

    private HashSet<TLabel> GetTargetSet()
    {
      if (this.m_targetSet == null)
        this.m_targetSet = new HashSet<TLabel>();
      else
        this.m_targetSet.Clear();
      return this.m_targetSet;
    }

    private HashSet<TLabel> GetReachedLabels()
    {
      if (this.m_reachedLabels == null)
        this.m_reachedLabels = new HashSet<TLabel>();
      else
        this.m_reachedLabels.Clear();
      return this.m_reachedLabels;
    }

    private struct LabelDistancePair
    {
      public TLabel Label;
      public int Distance;

      public LabelDistancePair(TLabel label, int distance)
      {
        this.Label = label;
        this.Distance = distance;
      }
    }

    private class AlgorithmLock : IDisposable
    {
      private readonly AncestralGraphAlgorithm<TLabel, TVertex> m_parent;

      public AlgorithmLock(AncestralGraphAlgorithm<TLabel, TVertex> parent)
      {
        this.m_parent = parent;
        if (Interlocked.CompareExchange(ref parent.m_inUse, 1, 0) == 1)
          throw new InvalidOperationException();
      }

      public void Dispose() => this.m_parent.m_inUse = 0;
    }
  }
}
