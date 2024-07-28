// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.RewriteParentsWrapper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class RewriteParentsWrapper : CachedGraphWrapper
  {
    private readonly CachedGraphWrapper m_graphToSimplify;
    private readonly Dictionary<int, int> m_simplification;
    private const int c_labelSimplifiedAway = -1;

    public RewriteParentsWrapper(CachedGraphWrapper graph)
      : base((IGitCommitGraph) graph)
    {
      this.m_graphToSimplify = graph;
      this.m_simplification = new Dictionary<int, int>();
    }

    public override void ClearLabel(int label)
    {
      base.ClearLabel(label);
      this.m_graphToSimplify.ClearLabel(label);
    }

    protected override int[] ComputeDataForLabel(int label)
    {
      int simplification = this.GetSimplification(label);
      if (simplification == -1)
        return Array.Empty<int>();
      if (label != simplification)
        return new int[1]{ simplification };
      int[] array = this.m_graphToSimplify.OutNeighborsOfLabel(label).Select<int, int>((Func<int, int>) (parent => this.GetSimplification(parent))).Where<int>((Func<int, bool>) (simplifiedParent => simplifiedParent != -1)).ToArray<int>();
      switch (array.Length)
      {
        case 0:
        case 1:
          return array;
        case 2:
          if (array[0] != array[1])
            return array;
          return new int[1]{ array[0] };
        default:
          return ((IEnumerable<int>) array).Distinct<int>().ToArray<int>();
      }
    }

    private int GetSimplification(int label)
    {
      int simplification;
      if (this.m_simplification.TryGetValue(label, out simplification))
        return simplification;
      int[] cachedAdjacencies = this.m_graphToSimplify.GetCachedAdjacencies(label);
      bool flag1 = false;
      Stack<int> intStack = new Stack<int>();
      bool flag2;
      for (; !(flag2 = this.m_simplification.TryGetValue(label, out simplification)) && !(flag1 = this.m_graphToSimplify.IsLabelAccepted(label)) && cachedAdjacencies.Length == 1; cachedAdjacencies = this.m_graphToSimplify.GetCachedAdjacencies(label))
      {
        intStack.Push(label);
        label = cachedAdjacencies[0];
      }
      if (!flag2)
      {
        if (flag1 || cachedAdjacencies.Length >= 2)
        {
          this.SetLabelAccepted(label);
          simplification = label;
        }
        else
          simplification = -1;
        this.m_simplification[label] = simplification;
      }
      while (intStack.Count > 0)
      {
        label = intStack.Pop();
        this.m_simplification[label] = simplification;
      }
      return simplification;
    }
  }
}
