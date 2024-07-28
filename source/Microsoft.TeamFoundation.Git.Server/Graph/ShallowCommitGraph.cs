// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.ShallowCommitGraph
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class ShallowCommitGraph : DirectedGraphWrapper<int, Sha1Id>
  {
    private HashSet<int> m_shallowLabels;

    public ShallowCommitGraph(IDirectedGraph<int, Sha1Id> graph, IEnumerable<Sha1Id> shallows)
      : base(graph)
    {
      if (shallows != null)
        this.m_shallowLabels = new HashSet<int>(graph.GetLabels(shallows));
      else
        this.m_shallowLabels = new HashSet<int>();
    }

    public override void ForEachOutNeighborsOfLabel(int label, Predicate<int> predicate)
    {
      if (this.m_shallowLabels.Contains(label))
        return;
      this.m_graph.ForEachOutNeighborsOfLabel(label, predicate);
    }

    public override IEnumerable<int> OutNeighborsOfLabel(int label) => this.m_shallowLabels.Contains(label) ? (IEnumerable<int>) Array.Empty<int>() : this.m_graph.OutNeighborsOfLabel(label);
  }
}
