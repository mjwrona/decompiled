// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.IntGraph
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class IntGraph
  {
    private readonly Dictionary<int, int> m_edgeStartPositions;
    private int m_numEdges;
    private int[] m_edges;
    private const int c_vertexIncomplete = -1;
    private const int c_vertexHasNoNeighbors = -2147483648;

    public IntGraph(int capacity = 128)
    {
      this.m_numEdges = 0;
      this.m_edgeStartPositions = new Dictionary<int, int>();
      this.m_edges = new int[2 * capacity];
    }

    public int NumEdges => this.m_numEdges;

    public int NumVertices => this.m_edgeStartPositions.Count;

    public IEnumerable<int> Vertices => (IEnumerable<int>) this.m_edgeStartPositions.Keys;

    public bool HasVertex(int vertex)
    {
      int num;
      if (vertex < 0 || !this.m_edgeStartPositions.TryGetValue(vertex, out num))
        return false;
      return num >= 0 || num == int.MinValue;
    }

    public bool AddVertex(int vertex, List<int> outNeighbors)
    {
      if (this.HasVertex(vertex))
        return false;
      this.m_edgeStartPositions[vertex] = -1;
      int[] destinationArray;
      for (int count = outNeighbors.Count; this.m_numEdges + count >= this.m_edges.Length; this.m_edges = destinationArray)
      {
        destinationArray = new int[this.m_edges.Length * 2];
        Array.Copy((Array) this.m_edges, (Array) destinationArray, this.m_edges.Length);
      }
      int numEdges = this.m_numEdges;
      bool flag = false;
      foreach (int outNeighbor in outNeighbors)
      {
        flag = true;
        this.m_edges[this.m_numEdges] = outNeighbor;
        ++this.m_numEdges;
      }
      if (flag)
      {
        this.m_edges[this.m_numEdges - 1] = this.GetInvolution(this.m_edges[this.m_numEdges - 1]);
        this.m_edgeStartPositions[vertex] = numEdges;
      }
      else
        this.m_edgeStartPositions[vertex] = int.MinValue;
      return true;
    }

    public IEnumerable<int> OutNeighbors(int vertex)
    {
      int i = this.HasVertex(vertex) ? this.m_edgeStartPositions[vertex] : throw new GraphLabelNotFoundException<int>(vertex);
      if (i != int.MinValue)
      {
        for (; i < this.m_edges.Length && this.m_edges[i] >= 0; ++i)
          yield return this.m_edges[i];
        yield return this.GetInvolution(this.m_edges[i]);
      }
    }

    private int GetInvolution(int i) => -(i + 1);
  }
}
