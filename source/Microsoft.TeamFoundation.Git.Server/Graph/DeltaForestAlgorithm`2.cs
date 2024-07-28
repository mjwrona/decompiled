// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.DeltaForestAlgorithm`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class DeltaForestAlgorithm<TLabel, TVertex>
  {
    public DeltaForestWalkResult<TLabel, TVertex> CompareDeltaChains(
      IDeltaForest<TLabel, TVertex> forest,
      IEnumerable<TLabel> reachableFrom,
      IEnumerable<TLabel> notReachableFrom)
    {
      Queue<TLabel> labelQueue = new Queue<TLabel>();
      HashSet<TLabel> source1 = new HashSet<TLabel>();
      HashSet<TLabel> source2 = new HashSet<TLabel>();
      HashSet<TLabel> labelSet = new HashSet<TLabel>();
      if (notReachableFrom != null)
      {
        foreach (TLabel label in notReachableFrom)
        {
          if (labelSet.Add(label))
            labelQueue.Enqueue(label);
        }
        while (labelQueue.Count > 0)
        {
          TLabel label = labelQueue.Dequeue();
          source1.Add(label);
          TLabel parent;
          if (forest.TryGetParent(label, out parent) && labelSet.Add(parent))
            labelQueue.Enqueue(parent);
        }
      }
      foreach (TLabel label in reachableFrom)
      {
        if (labelSet.Add(label))
          labelQueue.Enqueue(label);
      }
      while (labelQueue.Count > 0)
      {
        TLabel label = labelQueue.Dequeue();
        source2.Add(label);
        TLabel parent;
        if (forest.TryGetParent(label, out parent) && labelSet.Add(parent))
          labelQueue.Enqueue(parent);
      }
      return new DeltaForestWalkResult<TLabel, TVertex>((IReadOnlyList<TLabel>) source2.ToList<TLabel>(), (IReadOnlyList<TLabel>) source1.ToList<TLabel>());
    }
  }
}
