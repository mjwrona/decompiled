// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.IntSha1IdGraph
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class IntSha1IdGraph : 
    Sha1IdVertexSet,
    IDirectedGraph<int, Sha1Id>,
    IVertexSet<int, Sha1Id>
  {
    private int[] m_edgeStartPositions;
    private int[] m_depths;
    private int m_numEdges;
    private int[] m_edges;
    private readonly object m_lockObject;
    private const int c_vertexHasNoNeighbors = -2147483648;

    public IntSha1IdGraph(int capacity = 128)
      : base(capacity)
    {
      this.m_numEdges = 0;
      this.m_depths = new int[capacity];
      this.m_edgeStartPositions = new int[capacity];
      this.m_edges = new int[2 * capacity];
      this.m_lockObject = new object();
    }

    internal IntSha1IdGraph(Sha1Id[] vertices, int numVertices, int[] edges, int[] depths)
      : base(new Sha1IdLookup(vertices, numVertices))
    {
      this.m_depths = depths;
      this.m_edges = edges;
      this.m_numEdges = edges.Length;
      this.m_edgeStartPositions = new int[vertices.Length];
      bool flag = true;
      int index1 = 0;
      for (int index2 = 0; index1 < this.m_edgeStartPositions.Length && index2 < this.m_numEdges; ++index2)
      {
        if (flag)
        {
          this.m_edgeStartPositions[index1] = index2;
          ++index1;
        }
        flag = this.m_edges[index2] < 0;
      }
      this.m_lockObject = new object();
    }

    public bool AddVertex(Sha1Id vertex, IEnumerable<Sha1Id> neighbors)
    {
      lock (this.m_lockObject)
      {
        bool flag1 = this.AddVertex(vertex);
        if (neighbors == null)
          return flag1;
        int index = this.m_vertices.GetIndex(vertex);
        if (this.m_edgeStartPositions[index] >= 0)
          return false;
        int numEdges = this.m_numEdges;
        bool flag2 = false;
        foreach (Sha1Id neighbor in neighbors)
        {
          this.AddVertex(neighbor);
          flag2 = true;
          EnsureEdgeArraySize();
          this.m_edges[this.m_numEdges] = this.m_vertices.GetIndex(neighbor);
          ++this.m_numEdges;
        }
        if (flag2)
        {
          this.m_edges[this.m_numEdges - 1] = this.GetInvolution(this.m_edges[this.m_numEdges - 1]);
        }
        else
        {
          EnsureEdgeArraySize();
          this.m_edges[this.m_numEdges] = int.MinValue;
          ++this.m_numEdges;
        }
        this.m_edgeStartPositions[index] = numEdges;
        return true;
      }

      void EnsureEdgeArraySize()
      {
        if (this.m_numEdges < this.m_edges.Length)
          return;
        int[] destinationArray = new int[Math.Max(this.m_edges.Length * 2, this.m_numEdges + 1)];
        Array.Copy((Array) this.m_edges, (Array) destinationArray, this.m_edges.Length);
        this.m_edges = destinationArray;
      }
    }

    public bool AddVertex(Sha1Id vertex)
    {
      lock (this.m_lockObject)
      {
        int label;
        if (this.m_vertices.TryGetIndex(vertex, out label) && label < this.NumVertices)
          return false;
        int numVertices = this.NumVertices;
        this.ExtendArrays();
        this.m_depths[numVertices] = -1;
        this.m_edgeStartPositions[numVertices] = -1;
        this.m_vertices.Add(vertex);
        return true;
      }
    }

    private void ExtendArrays()
    {
      int length1 = this.m_edgeStartPositions.Length;
      if (this.NumVertices < length1)
        return;
      int length2 = Math.Max(length1 * 2, this.NumVertices + 1);
      int[] destinationArray1 = new int[length2];
      int[] destinationArray2 = new int[length2];
      Array.Copy((Array) this.m_depths, (Array) destinationArray1, this.m_depths.Length);
      Array.Copy((Array) this.m_edgeStartPositions, (Array) destinationArray2, this.m_edgeStartPositions.Length);
      this.m_depths = destinationArray1;
      this.m_edgeStartPositions = destinationArray2;
    }

    public bool HasFullLabel(int label) => this.HasLabel(label) && this.m_edgeStartPositions[label] >= 0;

    public override bool HasVertex(Sha1Id vertex)
    {
      int label;
      return this.m_vertices.TryGetIndex(vertex, out label) && this.m_edgeStartPositions[label] >= 0;
    }

    public void ForEachOutNeighborsOfLabel(int label, Predicate<int> predicate)
    {
      if (!this.HasFullLabel(label))
        return;
      int edgeStartPosition;
      for (edgeStartPosition = this.m_edgeStartPositions[label]; edgeStartPosition < this.m_edges.Length && this.m_edges[edgeStartPosition] >= 0; ++edgeStartPosition)
      {
        if (!predicate(this.m_edges[edgeStartPosition]))
          return;
      }
      if (this.m_edges[edgeStartPosition] == int.MinValue)
        return;
      int involution = this.GetInvolution(this.m_edges[edgeStartPosition]);
      int num = predicate(involution) ? 1 : 0;
    }

    public IEnumerable<Sha1Id> OutNeighbors(Sha1Id vertex) => this.GetVertices(this.OutNeighborsOfLabel(this.GetLabel(vertex)));

    public IEnumerable<int> OutNeighborsOfLabel(int label)
    {
      if (this.HasFullLabel(label))
      {
        int i;
        for (i = this.m_edgeStartPositions[label]; i < this.m_edges.Length && this.m_edges[i] >= 0; ++i)
          yield return this.m_edges[i];
        if (this.m_edges[i] != int.MinValue)
          yield return this.GetInvolution(this.m_edges[i]);
      }
    }

    public int GetDegreeOfLabel(int label) => this.HasFullLabel(label) ? this.OutNeighborsOfLabel(label).Count<int>() : throw new GraphLabelNotFoundException<int>(label);

    public void SetAncestryDepth(int label, int depth)
    {
      lock (this.m_lockObject)
      {
        if (!this.HasLabel(label))
          return;
        this.m_depths[label] = depth;
      }
    }

    public int GetAncestryDepth(int label) => this.HasLabel(label) ? this.m_depths[label] : -1;

    public long GetSize() => (long) (CacheUtil.ObjectOverhead + IntPtr.Size) + this.m_vertices.GetSize() + (long) IntPtr.Size + (long) CacheUtil.ObjectOverhead + (long) (4 * this.m_edgeStartPositions.Length) + (long) IntPtr.Size + (long) CacheUtil.ObjectOverhead + (long) (4 * this.m_depths.Length) + 4L + (long) IntPtr.Size + (long) CacheUtil.ObjectOverhead + (long) (4 * this.m_edges.Length) + 8L + (long) CacheUtil.ObjectOverhead;

    public int NumVertices => this.m_vertices.Count;

    internal int NumEdges => this.m_numEdges;

    internal int[] Depths => this.m_depths;

    internal Sha1Id[] Vertices => this.m_vertices.ObjectIds;

    private int GetInvolution(int i) => -(i + 1);
  }
}
