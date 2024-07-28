// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.GraphReachabilityAlgorithmHelpers
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  public static class GraphReachabilityAlgorithmHelpers
  {
    public static long GetIncrementalPriority<TLabel, TVertex>(
      IDirectedGraph<TLabel, TVertex> graph,
      TLabel label,
      ref long storedPriority,
      PriorityDelegate<TLabel> getPriorityOfLabel,
      PriorityDelegate<TVertex> getPriorityOfVertex)
    {
      if (getPriorityOfLabel != null)
        return getPriorityOfLabel(label);
      if (getPriorityOfVertex != null)
        return getPriorityOfVertex(graph.GetVertex(label));
      long incrementalPriority = storedPriority;
      ++storedPriority;
      return incrementalPriority;
    }
  }
}
