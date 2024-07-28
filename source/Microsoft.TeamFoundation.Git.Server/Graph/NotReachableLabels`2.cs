// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.NotReachableLabels`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class NotReachableLabels<TLabel, TVertex> : IReadOnlySet<TLabel>
  {
    private readonly IDirectedGraph<TLabel, TVertex> m_graph;
    private readonly HashSet<TLabel> m_excludedLabels;
    private readonly Heap<TLabel, long> m_walkQueue;
    private readonly int m_minAllowedDepth;

    public NotReachableLabels(
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TLabel> notReachableFromLabels,
      bool tryPreventWalkAround = false)
    {
      this.m_graph = graph;
      this.m_excludedLabels = new HashSet<TLabel>();
      this.m_walkQueue = new Heap<TLabel, long>((IComparer<long>) new StackComparer());
      this.m_minAllowedDepth = tryPreventWalkAround ? int.MaxValue : int.MinValue;
      foreach (TLabel reachableFromLabel in notReachableFromLabels)
      {
        if (this.m_excludedLabels.Add(reachableFromLabel))
        {
          int ancestryDepth = this.m_graph.GetAncestryDepth(reachableFromLabel);
          this.m_walkQueue.Enqueue(reachableFromLabel, (long) ancestryDepth);
          if (ancestryDepth < this.m_minAllowedDepth)
            this.m_minAllowedDepth = ancestryDepth + 1;
        }
      }
    }

    public bool Contains(TLabel item)
    {
      if (!this.m_graph.HasLabel(item))
        return false;
      int ancestryDepth = this.m_graph.GetAncestryDepth(item);
      if (ancestryDepth < this.m_minAllowedDepth)
        return false;
      this.ExploreToDepth(ancestryDepth);
      return !this.m_excludedLabels.Contains(item);
    }

    private void ExploreToDepth(int depth)
    {
      TLabel label1;
      while (this.m_walkQueue.TryDequeueBeforeThreshold((long) (depth - 1), out label1))
      {
        foreach (TLabel label2 in this.m_graph.OutNeighborsOfLabel(label1))
        {
          if (this.m_excludedLabels.Add(label2))
            this.m_walkQueue.Enqueue(label2, (long) this.m_graph.GetAncestryDepth(label2));
        }
      }
    }
  }
}
