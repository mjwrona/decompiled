// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.SetCoverFactory`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring;
using Microsoft.TeamFoundation.Git.Server.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap
{
  internal class SetCoverFactory<TVertex>
  {
    private readonly IDirectedGraph<int, TVertex> m_graph;
    private readonly Heap<int, long> m_prioQueue;
    private readonly HashSet<int> m_reachedLabels;

    public SetCoverFactory(IDirectedGraph<int, TVertex> graph)
    {
      this.m_graph = graph;
      this.m_prioQueue = new Heap<int, long>((IComparer<long>) new StackComparer());
      this.m_reachedLabels = new HashSet<int>();
    }

    public Dictionary<int, RoaringBitmap<int>> GetReachabilityBetweenLabels(
      List<int> fromLabels,
      List<int> toLabels)
    {
      int num1 = toLabels.Min<int>((Func<int, int>) (label => this.m_graph.GetAncestryDepth(label)));
      ITwoWayReadOnlyList<int> objectList = (ITwoWayReadOnlyList<int>) new ListWrapper(fromLabels);
      Dictionary<int, RoaringBitmap<int>> dictionary = new Dictionary<int, RoaringBitmap<int>>();
      this.m_reachedLabels.Clear();
      this.m_prioQueue.Clear();
      int count = fromLabels.Count;
      for (int index = 0; index < fromLabels.Count; ++index)
      {
        int fromLabel = fromLabels[index];
        RoaringBitmap<int> roaringBitmap = new RoaringBitmap<int>(objectList);
        roaringBitmap.AddIndex(index);
        dictionary[fromLabel] = roaringBitmap;
        int ancestryDepth = this.m_graph.GetAncestryDepth(fromLabel);
        if (this.m_reachedLabels.Add(fromLabel))
          this.m_prioQueue.Enqueue(fromLabel, (long) ancestryDepth);
      }
      int num2;
      while (this.m_prioQueue.TryDequeueBeforeThreshold((long) (num1 - 1), out num2))
      {
        RoaringBitmap<int> withBitmap = dictionary[num2];
        foreach (int num3 in this.m_graph.OutNeighborsOfLabel(num2))
        {
          long ancestryDepth = (long) this.m_graph.GetAncestryDepth(num3);
          if (this.m_reachedLabels.Add(num3))
            this.m_prioQueue.Enqueue(num3, ancestryDepth);
          RoaringBitmap<int> bitmap;
          if (dictionary.TryGetValue(num3, out bitmap))
          {
            bitmap = RoaringBitmapCombiner<int>.Union(bitmap, withBitmap);
          }
          else
          {
            bitmap = new RoaringBitmap<int>(objectList);
            foreach (int index in withBitmap.Indices)
              bitmap.AddIndex(index);
          }
          dictionary[num3] = bitmap;
        }
      }
      Dictionary<int, RoaringBitmap<int>> reachabilityBetweenLabels = new Dictionary<int, RoaringBitmap<int>>();
      foreach (int toLabel in toLabels)
      {
        RoaringBitmap<int> roaringBitmap;
        if (!dictionary.TryGetValue(toLabel, out roaringBitmap))
          roaringBitmap = new RoaringBitmap<int>(objectList);
        reachabilityBetweenLabels[toLabel] = roaringBitmap;
      }
      return reachabilityBetweenLabels;
    }
  }
}
