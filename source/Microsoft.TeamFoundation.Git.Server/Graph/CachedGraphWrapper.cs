// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.CachedGraphWrapper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal abstract class CachedGraphWrapper : GitCommitGraphWrapper
  {
    protected readonly IGitCommitGraph m_commitGraph;
    private HashSet<int> m_computedLabels;
    private HashSet<int> m_acceptLabels;
    private Dictionary<int, int[]> m_neighbors;

    public CachedGraphWrapper(IGitCommitGraph graph)
      : base(graph)
    {
      this.m_commitGraph = graph;
      this.m_acceptLabels = new HashSet<int>();
      this.m_computedLabels = new HashSet<int>();
      this.m_neighbors = new Dictionary<int, int[]>();
    }

    public bool IsLabelAccepted(int label)
    {
      if (!this.m_computedLabels.Contains(label))
        this.OutNeighborsOfLabel(label);
      return this.m_acceptLabels.Contains(label);
    }

    public void SetLabelAccepted(int label) => this.m_acceptLabels.Add(label);

    public virtual void ClearLabel(int label)
    {
      this.m_neighbors.Remove(label);
      this.m_acceptLabels.Remove(label);
      this.m_computedLabels.Remove(label);
    }

    public override void ForEachOutNeighborsOfLabel(int label, Predicate<int> predicate)
    {
      int[] cachedAdjacencies = this.GetCachedAdjacencies(label);
      int index = 0;
      while (index < cachedAdjacencies.Length && predicate(cachedAdjacencies[index]))
        ++index;
    }

    public override IEnumerable<Sha1Id> OutNeighbors(Sha1Id vertex) => this.GetVertices(this.OutNeighborsOfLabel(this.GetLabel(vertex)));

    public override IEnumerable<int> OutNeighborsOfLabel(int label) => (IEnumerable<int>) this.GetCachedAdjacencies(label);

    public int[] GetCachedAdjacencies(int label)
    {
      int[] cachedAdjacencies;
      if (this.m_neighbors.TryGetValue(label, out cachedAdjacencies))
        return cachedAdjacencies;
      int[] dataForLabel;
      this.m_neighbors[label] = dataForLabel = this.ComputeDataForLabel(label);
      this.m_computedLabels.Add(label);
      return dataForLabel;
    }

    protected int[] GetStoredNeighbors(int label)
    {
      int[] numArray;
      return this.m_neighbors.TryGetValue(label, out numArray) ? numArray : (int[]) null;
    }

    protected abstract int[] ComputeDataForLabel(int label);
  }
}
