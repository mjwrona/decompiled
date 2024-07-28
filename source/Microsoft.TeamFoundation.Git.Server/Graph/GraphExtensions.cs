// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.GraphExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  public static class GraphExtensions
  {
    public static IEnumerable<TLabel> TryGetLabels<TLabel, TVertex>(
      this IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TVertex> vertices)
    {
      return vertices == null ? (IEnumerable<TLabel>) null : graph.GetLabels(vertices);
    }

    public static IEnumerable<TVertex> TryGetVertices<TLabel, TVertex>(
      this IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TLabel> labels)
    {
      return labels == null ? (IEnumerable<TVertex>) null : graph.GetVertices(labels);
    }
  }
}
