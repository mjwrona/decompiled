// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.GitCommitSubgraph
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class GitCommitSubgraph : GitCommitGraphWrapper
  {
    private readonly IReadOnlySet<int> m_restrictedLabels;

    public GitCommitSubgraph(IGitCommitGraph graph, IReadOnlySet<int> restrictedLabels)
      : base(graph)
    {
      this.m_restrictedLabels = restrictedLabels;
    }

    public override bool HasVertex(Sha1Id vertex) => this.m_graph.HasVertex(vertex) && this.HasLabel(this.m_graph.GetLabel(vertex));

    public override bool HasLabel(int label) => this.m_graph.HasLabel(label) && this.m_restrictedLabels.Contains(label);

    public override void ForEachOutNeighborsOfLabel(int label, Predicate<int> predicate) => this.m_graph.ForEachOutNeighborsOfLabel(label, (Predicate<int>) (x => !this.m_restrictedLabels.Contains(x) || predicate(x)));

    public override IEnumerable<Sha1Id> OutNeighbors(Sha1Id vertex) => this.GetVertices(this.OutNeighborsOfLabel(this.GetLabel(vertex)));

    public override IEnumerable<int> OutNeighborsOfLabel(int label) => this.m_graph.OutNeighborsOfLabel(label).Where<int>((Func<int, bool>) (parent => this.m_restrictedLabels.Contains(parent)));
  }
}
