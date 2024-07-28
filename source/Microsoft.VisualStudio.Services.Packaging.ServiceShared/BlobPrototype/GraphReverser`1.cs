// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.GraphReverser`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class GraphReverser<T> : IGraphReverser<T>
  {
    public IEnumerable<GraphNode<T>> Reverse(
      IEnumerable<GraphNode<T>> nodes,
      Func<T, T, bool> shouldReverseEdge = null)
    {
      Dictionary<GraphNode<T>, GraphNode<T>> newGraphNodes = new Dictionary<GraphNode<T>, GraphNode<T>>();
      foreach (GraphNode<T> node in nodes)
      {
        GraphNode<T> updateDictionary1 = this.CreateGraphNodeIfNeededAndUpdateDictionary(node, (IDictionary<GraphNode<T>, GraphNode<T>>) newGraphNodes);
        foreach (GraphNode<T> edge in (IEnumerable<GraphNode<T>>) node.Edges)
        {
          GraphNode<T> updateDictionary2 = this.CreateGraphNodeIfNeededAndUpdateDictionary(edge, (IDictionary<GraphNode<T>, GraphNode<T>>) newGraphNodes);
          if (shouldReverseEdge == null || shouldReverseEdge(node.Data, edge.Data))
            updateDictionary2.Edges.Add(updateDictionary1);
          else
            updateDictionary1.Edges.Add(updateDictionary2);
        }
      }
      return (IEnumerable<GraphNode<T>>) newGraphNodes.Values;
    }

    private GraphNode<T> CreateGraphNodeIfNeededAndUpdateDictionary(
      GraphNode<T> oldGraphNode,
      IDictionary<GraphNode<T>, GraphNode<T>> newGraphNodes)
    {
      if (newGraphNodes.ContainsKey(oldGraphNode))
        return newGraphNodes[oldGraphNode];
      GraphNode<T> updateDictionary = new GraphNode<T>(oldGraphNode.Data);
      newGraphNodes.Add(oldGraphNode, updateDictionary);
      return updateDictionary;
    }
  }
}
