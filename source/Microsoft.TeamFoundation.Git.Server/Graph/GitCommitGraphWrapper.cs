// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.GitCommitGraphWrapper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class GitCommitGraphWrapper : 
    IGitCommitGraph,
    IDirectedGraph<int, Sha1Id>,
    IVertexSet<int, Sha1Id>,
    IPackedBloomFilters
  {
    protected readonly IGitCommitGraph m_graph;

    public GitCommitGraphWrapper(IGitCommitGraph graph) => this.m_graph = graph;

    public virtual int NumVertices => this.m_graph.NumVertices;

    public virtual int GetAncestryDepth(int label) => this.m_graph.GetAncestryDepth(label);

    public virtual long GetCommitTime(int label) => this.m_graph.GetCommitTime(label);

    public virtual int GetLabel(Sha1Id vertex) => this.m_graph.GetLabel(vertex);

    public virtual IEnumerable<int> GetLabels(IEnumerable<Sha1Id> vertices) => this.m_graph.GetLabels(vertices);

    public virtual Sha1Id GetRootTreeId(int label) => this.m_graph.GetRootTreeId(label);

    public virtual Sha1Id GetVertex(int label) => this.m_graph.GetVertex(label);

    public virtual IEnumerable<Sha1Id> GetVertices() => this.m_graph.GetVertices();

    public virtual IEnumerable<Sha1Id> GetVertices(IEnumerable<int> labels) => this.m_graph.GetVertices(labels);

    public virtual bool HasLabel(int label) => this.m_graph.HasLabel(label);

    public virtual bool HasVertex(Sha1Id vertex) => this.m_graph.HasVertex(vertex);

    public virtual void ForEachOutNeighborsOfLabel(int label, Predicate<int> predicate) => this.m_graph.ForEachOutNeighborsOfLabel(label, predicate);

    public virtual IEnumerable<Sha1Id> OutNeighbors(Sha1Id vertex) => this.m_graph.OutNeighbors(vertex);

    public virtual IEnumerable<int> OutNeighborsOfLabel(int label) => this.m_graph.OutNeighborsOfLabel(label);

    public virtual BloomKey EncodeKey(string path) => this.m_graph.EncodeKey(path);

    public virtual BloomFilterStatus GetFilterStatus(int label) => this.m_graph.GetFilterStatus(label);

    public virtual IReadOnlyBloomFilter GetReadOnlyFilter(int label) => this.m_graph.GetReadOnlyFilter(label);
  }
}
