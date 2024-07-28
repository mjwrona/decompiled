// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.DirectedGraphWrapper`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal abstract class DirectedGraphWrapper<TLabel, TVertex> : 
    IDirectedGraph<TLabel, TVertex>,
    IVertexSet<TLabel, TVertex>
  {
    protected readonly IDirectedGraph<TLabel, TVertex> m_graph;

    public DirectedGraphWrapper(IDirectedGraph<TLabel, TVertex> graph) => this.m_graph = graph;

    public virtual void ForEachOutNeighborsOfLabel(TLabel label, Predicate<TLabel> predicate) => this.m_graph.ForEachOutNeighborsOfLabel(label, predicate);

    public virtual TLabel GetLabel(TVertex vertex) => this.m_graph.GetLabel(vertex);

    public virtual IEnumerable<TLabel> GetLabels(IEnumerable<TVertex> vertices) => this.m_graph.GetLabels(vertices);

    public virtual TVertex GetVertex(TLabel label) => this.m_graph.GetVertex(label);

    public virtual IEnumerable<TVertex> GetVertices() => this.m_graph.GetVertices();

    public virtual IEnumerable<TVertex> GetVertices(IEnumerable<TLabel> labels) => this.m_graph.GetVertices(labels);

    public virtual bool HasLabel(TLabel label) => this.m_graph.HasLabel(label);

    public virtual bool HasVertex(TVertex vertex) => this.m_graph.HasVertex(vertex);

    public virtual IEnumerable<TVertex> OutNeighbors(TVertex vertex) => this.m_graph.OutNeighbors(vertex);

    public virtual IEnumerable<TLabel> OutNeighborsOfLabel(TLabel label) => this.m_graph.OutNeighborsOfLabel(label);

    public int NumVertices => this.m_graph.NumVertices;

    public virtual int GetAncestryDepth(TLabel label) => this.m_graph.GetAncestryDepth(label);
  }
}
