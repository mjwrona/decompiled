// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.GitRepoGraph
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class GitRepoGraph : 
    IDirectedGraph<Sha1Id, TfsGitCommit>,
    IVertexSet<Sha1Id, TfsGitCommit>
  {
    private IGitObjectSet m_objectSet;

    public GitRepoGraph(IGitObjectSet objectSet) => this.m_objectSet = objectSet;

    public TfsGitCommit GetVertex(Sha1Id label) => this.m_objectSet.LookupObject<TfsGitCommit>(label);

    public Sha1Id GetLabel(TfsGitCommit vertex) => vertex.ObjectId;

    public bool HasVertex(TfsGitCommit vertex) => this.HasLabel(vertex.ObjectId);

    public bool HasLabel(Sha1Id label) => this.m_objectSet.TryLookupObject<TfsGitCommit>(label) != null;

    public IEnumerable<TfsGitCommit> GetVertices(IEnumerable<Sha1Id> labels) => labels.Select<Sha1Id, TfsGitCommit>((Func<Sha1Id, TfsGitCommit>) (id => this.m_objectSet.LookupObject<TfsGitCommit>(id)));

    public IEnumerable<Sha1Id> GetLabels(IEnumerable<TfsGitCommit> vertices) => vertices.Select<TfsGitCommit, Sha1Id>((Func<TfsGitCommit, Sha1Id>) (commit => commit.ObjectId));

    public void ForEachOutNeighborsOfLabel(Sha1Id label, Predicate<Sha1Id> predicate)
    {
      foreach (Sha1Id sha1Id in this.OutNeighborsOfLabel(label))
      {
        if (!predicate(sha1Id))
          break;
      }
    }

    public IEnumerable<TfsGitCommit> OutNeighbors(TfsGitCommit vertex) => (IEnumerable<TfsGitCommit>) vertex.GetParents();

    public IEnumerable<Sha1Id> OutNeighborsOfLabel(Sha1Id label) => this.GetLabels(this.OutNeighbors(this.GetVertex(label)));

    public IEnumerable<TfsGitCommit> GetVertices() => throw new NotImplementedException();

    public int NumVertices => throw new NotImplementedException();

    public int GetAncestryDepth(Sha1Id label) => 0;
  }
}
