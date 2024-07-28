// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.FirstParentGraph
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class FirstParentGraph : GitCommitGraphWrapper
  {
    public FirstParentGraph(IGitCommitGraph graph)
      : base(graph)
    {
    }

    public override void ForEachOutNeighborsOfLabel(int label, Predicate<int> predicate) => this.m_graph.ForEachOutNeighborsOfLabel(label, (Predicate<int>) (x => predicate(x) && false));

    public override IEnumerable<Sha1Id> OutNeighbors(Sha1Id vertex) => this.m_graph.OutNeighbors(vertex).Take<Sha1Id>(1);

    public override IEnumerable<int> OutNeighborsOfLabel(int label) => this.m_graph.OutNeighborsOfLabel(label).Take<int>(1);
  }
}
