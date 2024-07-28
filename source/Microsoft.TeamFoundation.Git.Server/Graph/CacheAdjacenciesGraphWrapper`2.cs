// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.CacheAdjacenciesGraphWrapper`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class CacheAdjacenciesGraphWrapper<TLabel, TVertex> : 
    DirectedGraphWrapper<TLabel, TVertex>
  {
    private Dictionary<TLabel, TVertex> m_vertices;
    private Dictionary<TLabel, List<TLabel>> m_adjacencies;

    public CacheAdjacenciesGraphWrapper(IDirectedGraph<TLabel, TVertex> graph)
      : base(graph)
    {
      this.m_vertices = new Dictionary<TLabel, TVertex>();
      this.m_adjacencies = new Dictionary<TLabel, List<TLabel>>();
    }

    public override TVertex GetVertex(TLabel label)
    {
      TVertex vertex1;
      if (this.m_vertices.TryGetValue(label, out vertex1))
        return vertex1;
      TVertex vertex2 = base.GetVertex(label);
      this.m_vertices[label] = vertex2;
      return vertex2;
    }

    public override void ForEachOutNeighborsOfLabel(TLabel label, Predicate<TLabel> predicate)
    {
      List<TLabel> cachedAdjacencies = this.GetCachedAdjacencies(label);
      int index = 0;
      while (index < cachedAdjacencies.Count && predicate(cachedAdjacencies[index]))
        ++index;
    }

    public override IEnumerable<TVertex> OutNeighbors(TVertex vertex) => this.GetVertices(this.OutNeighborsOfLabel(this.GetLabel(vertex)));

    public override IEnumerable<TLabel> OutNeighborsOfLabel(TLabel label) => (IEnumerable<TLabel>) this.GetCachedAdjacencies(label);

    private List<TLabel> GetCachedAdjacencies(TLabel label)
    {
      List<TLabel> cachedAdjacencies;
      if (this.m_adjacencies.TryGetValue(label, out cachedAdjacencies))
        return cachedAdjacencies;
      List<TLabel> list = this.m_graph.OutNeighborsOfLabel(label).ToList<TLabel>();
      this.m_adjacencies.Add(label, list);
      return list;
    }

    public void ClearLabel(TLabel label)
    {
      this.m_vertices.Remove(label);
      this.m_adjacencies.Remove(label);
    }
  }
}
