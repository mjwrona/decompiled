// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.Sha1IdVertexSet
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class Sha1IdVertexSet : IVertexSet<int, Sha1Id>
  {
    protected readonly Sha1IdLookup m_vertices;

    public Sha1IdVertexSet(int capacity = 128) => this.m_vertices = new Sha1IdLookup(capacity);

    public Sha1IdVertexSet(Sha1IdLookup vertices) => this.m_vertices = vertices;

    public virtual int GetLabel(Sha1Id vertex) => this.m_vertices.GetIndex(vertex);

    public virtual IEnumerable<int> GetLabels(IEnumerable<Sha1Id> vertices) => vertices.Select<Sha1Id, int>(new Func<Sha1Id, int>(this.GetLabel));

    public virtual Sha1Id GetVertex(int label) => this.m_vertices.GetValue(label);

    public virtual IEnumerable<Sha1Id> GetVertices() => (IEnumerable<Sha1Id>) this.m_vertices;

    public virtual IEnumerable<Sha1Id> GetVertices(IEnumerable<int> labels) => labels.Select<int, Sha1Id>(new Func<int, Sha1Id>(this.GetVertex));

    public virtual bool HasVertex(Sha1Id vertex) => this.m_vertices.TryGetIndex(vertex, out int _);

    public virtual bool HasLabel(int label) => 0 <= label && label < this.m_vertices.Count;
  }
}
