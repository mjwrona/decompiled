// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.SubgraphWrapper`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class SubgraphWrapper<TLabel, TVertex> : DirectedGraphWrapper<TLabel, TVertex>
  {
    protected readonly bool m_includeBoundary;
    protected readonly Predicate<TLabel> m_include;

    public SubgraphWrapper(
      IDirectedGraph<TLabel, TVertex> graph,
      Predicate<TLabel> includeLabel,
      bool includeBoundary = false)
      : base(graph)
    {
      this.m_include = includeLabel;
      this.m_includeBoundary = includeBoundary;
    }

    public override bool HasVertex(TVertex vertex) => base.HasVertex(vertex) && this.m_include(this.GetLabel(vertex));

    public override bool HasLabel(TLabel label) => base.HasLabel(label) && this.m_include(label);

    public override void ForEachOutNeighborsOfLabel(TLabel label, Predicate<TLabel> predicate)
    {
      if (!this.HasLabel(label))
        return;
      if (this.m_includeBoundary)
        base.ForEachOutNeighborsOfLabel(label, predicate);
      base.ForEachOutNeighborsOfLabel(label, (Predicate<TLabel>) (x => !this.HasLabel(x) || predicate(x)));
    }

    public override IEnumerable<TVertex> OutNeighbors(TVertex vertex) => this.GetVertices(this.OutNeighborsOfLabel(this.GetLabel(vertex)));

    public override IEnumerable<TLabel> OutNeighborsOfLabel(TLabel label)
    {
      if (!this.HasLabel(label))
        return Enumerable.Empty<TLabel>();
      return this.m_includeBoundary ? base.OutNeighborsOfLabel(label) : base.OutNeighborsOfLabel(label).Where<TLabel>((Func<TLabel, bool>) (l => this.HasLabel(l)));
    }
  }
}
