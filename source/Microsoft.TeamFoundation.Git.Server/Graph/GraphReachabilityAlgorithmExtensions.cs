// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.GraphReachabilityAlgorithmExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal static class GraphReachabilityAlgorithmExtensions
  {
    public static IEnumerable<TVertex> Order<TLabel, TVertex>(
      this AncestralGraphAlgorithm<TLabel, TVertex> algorithm,
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TVertex> reachableFromSet = null,
      IEnumerable<TVertex> notReachableFromSet = null,
      PriorityDelegate<TLabel> getPriorityOfLabel = null,
      PriorityDelegate<TVertex> getPriorityOfVertex = null)
    {
      return graph.GetVertices(algorithm.OrderByLabels<TLabel, TVertex>(graph, reachableFromSet != null ? graph.GetLabels(reachableFromSet) : (IEnumerable<TLabel>) null, notReachableFromSet != null ? graph.GetLabels(notReachableFromSet) : (IEnumerable<TLabel>) null, getPriorityOfLabel, getPriorityOfVertex));
    }

    public static IEnumerable<TLabel> OrderByLabels<TLabel, TVertex>(
      this AncestralGraphAlgorithm<TLabel, TVertex> algorithm,
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TLabel> reachableFromSet = null,
      IEnumerable<TLabel> notReachableFromSet = null,
      PriorityDelegate<TLabel> getPriorityOfLabel = null,
      PriorityDelegate<TVertex> getPriorityOfVertex = null)
    {
      IReadOnlySet<TLabel> restrictedLabels = notReachableFromSet == null ? (IReadOnlySet<TLabel>) ReadOnlyUniversalSet<TLabel>.Instance : (IReadOnlySet<TLabel>) new NotReachableLabels<TLabel, TVertex>(graph, notReachableFromSet);
      return algorithm.OrderByLabels(graph, reachableFromSet, restrictedLabels, getPriorityOfLabel, getPriorityOfVertex);
    }

    public static IEnumerable<TVertex> GetReachable<TLabel, TVertex>(
      this AncestralGraphAlgorithm<TLabel, TVertex> algorithm,
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TVertex> reachableFrom = null,
      IEnumerable<TVertex> notReachableFrom = null)
    {
      return graph.GetVertices(algorithm.GetReachableLabels<TLabel, TVertex>(graph, reachableFrom != null ? graph.GetLabels(reachableFrom) : (IEnumerable<TLabel>) null, notReachableFrom != null ? graph.GetLabels(notReachableFrom) : (IEnumerable<TLabel>) null));
    }

    public static IEnumerable<TLabel> GetReachableLabels<TLabel, TVertex>(
      this AncestralGraphAlgorithm<TLabel, TVertex> algorithm,
      IDirectedGraph<TLabel, TVertex> graph,
      IEnumerable<TLabel> reachableFrom = null,
      IEnumerable<TLabel> notReachableFrom = null)
    {
      IReadOnlySet<TLabel> restrictedLabels = notReachableFrom == null ? (IReadOnlySet<TLabel>) ReadOnlyUniversalSet<TLabel>.Instance : (IReadOnlySet<TLabel>) new NotReachableLabels<TLabel, TVertex>(graph, notReachableFrom);
      return algorithm.GetReachableLabels(graph, reachableFrom, restrictedLabels);
    }
  }
}
