// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.Sha1IdDeltaForest
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class Sha1IdDeltaForest : 
    Sha1IdVertexSet,
    IDeltaForest<int, Sha1Id>,
    IVertexSet<int, Sha1Id>
  {
    private int[] m_parents;
    private int[] m_lengths;
    private const int c_noParent = -1;

    public Sha1IdDeltaForest(int capacity = 128)
      : base()
    {
      this.m_parents = new int[capacity];
      this.m_lengths = new int[capacity];
    }

    public Sha1IdDeltaForest(Sha1IdLookup vertices, int[] parents, int[] lengths)
      : base(vertices)
    {
      this.m_parents = parents;
      this.m_lengths = lengths;
    }

    public bool AddBaseVertex(Sha1Id id)
    {
      if (this.HasVertex(id))
        return false;
      int count = this.m_vertices.Count;
      this.m_vertices.Add(id);
      this.ExtendArrays();
      this.m_lengths[count] = 0;
      this.m_parents[count] = -1;
      return true;
    }

    public bool AddDeltaVertex(Sha1Id id, Sha1Id parent)
    {
      if (this.HasVertex(id))
        return false;
      int label = this.GetLabel(parent);
      int count = this.m_vertices.Count;
      this.m_vertices.Add(id);
      this.ExtendArrays();
      this.m_lengths[count] = this.m_lengths[label] + 1;
      this.m_parents[count] = label;
      return true;
    }

    public int GetDeltaChainLength(int label) => this.HasLabel(label) ? this.m_lengths[label] : throw new GraphLabelNotFoundException<int>(label);

    public bool TryGetParent(int label, out int parent)
    {
      parent = this.HasLabel(label) ? this.m_parents[label] : throw new GraphLabelNotFoundException<int>(label);
      return parent != -1;
    }

    private void ExtendArrays()
    {
      int[] destinationArray1;
      for (; this.m_parents.Length <= this.m_vertices.Count; this.m_parents = destinationArray1)
      {
        destinationArray1 = new int[2 * this.m_parents.Length + 1];
        Array.Copy((Array) this.m_parents, (Array) destinationArray1, this.m_parents.Length);
      }
      int[] destinationArray2;
      for (; this.m_lengths.Length <= this.m_vertices.Count; this.m_lengths = destinationArray2)
      {
        destinationArray2 = new int[2 * this.m_lengths.Length + 1];
        Array.Copy((Array) this.m_lengths, (Array) destinationArray2, this.m_lengths.Length);
      }
    }

    public int NumVertices => this.m_vertices.Count;

    internal int[] Parents => this.m_parents;

    internal int[] ChainLengths => this.m_lengths;

    internal Sha1Id[] Vertices => this.m_vertices.ObjectIds;

    public long GetSize() => this.m_vertices.GetSize() + (long) ((this.m_parents.Length + this.m_lengths.Length) * 4);
  }
}
