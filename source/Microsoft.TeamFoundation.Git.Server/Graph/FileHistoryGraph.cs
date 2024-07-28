// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.FileHistoryGraph
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class FileHistoryGraph : CachedGraphWrapper, IChangeTypeOracle
  {
    private readonly NormalizedGitPath m_path;
    private readonly int m_lastIndex;
    private readonly bool m_fullHistory;
    private readonly IPathInfoOracle m_comparer;
    private Dictionary<int, ChangeAndObjectType> m_types;
    private readonly HashSet<int> m_boundaryLabels;
    private readonly bool m_stopAtAdds;
    private List<int> m_neighborList;
    private static readonly ChangeAndObjectType s_rootChangeType = new ChangeAndObjectType(TfsGitChangeType.Edit, GitObjectType.Tree);

    public FileHistoryGraph(
      IGitCommitGraph graph,
      IGitObjectSet repo,
      NormalizedGitPath path,
      bool fullHistory = false,
      bool stopAtAdds = false)
      : this(graph, path, (IPathInfoOracle) new PathInfoOracle(graph, repo, path), fullHistory, stopAtAdds)
    {
    }

    public FileHistoryGraph(
      IGitCommitGraph graph,
      NormalizedGitPath path,
      IPathInfoOracle pathOracle,
      bool fullHistory = false,
      bool stopAtAdds = false)
      : base(graph)
    {
      this.m_path = path;
      this.m_fullHistory = fullHistory;
      this.m_stopAtAdds = stopAtAdds;
      if (stopAtAdds)
        this.m_boundaryLabels = new HashSet<int>();
      this.m_lastIndex = Math.Max(0, path.Parts.Count - 1);
      this.m_comparer = pathOracle;
      this.m_types = new Dictionary<int, ChangeAndObjectType>();
      this.m_neighborList = new List<int>();
    }

    public override void ClearLabel(int label)
    {
      base.ClearLabel(label);
      this.m_comparer.ClearCachedTreesameInfo(label);
      if (this.m_stopAtAdds)
        this.m_boundaryLabels.Remove(label);
      this.m_types.Remove(label);
    }

    public bool TryGetChangeType(Sha1Id vertex, out ChangeAndObjectType changeType) => this.m_types.TryGetValue(this.GetLabel(vertex), out changeType);

    protected override int[] ComputeDataForLabel(int label)
    {
      if (this.m_path.Parts.Count == 0)
      {
        this.SetLabelAccepted(label);
        this.m_types[label] = FileHistoryGraph.s_rootChangeType;
        return this.m_commitGraph.OutNeighborsOfLabel(label).ToList<int>().ToArray();
      }
      if (this.m_stopAtAdds && this.m_boundaryLabels.Contains(label))
        return Array.Empty<int>();
      int num1 = -1;
      bool flag1 = this.m_comparer.IsTreesameToFirstParent(label);
      if (flag1 && !this.m_fullHistory)
        return this.m_graph.OutNeighborsOfLabel(label).Take<int>(1).ToArray<int>();
      bool flag2 = this.m_comparer.HasPath(label);
      GitObjectType objectType = this.m_comparer.GetObjectType(label);
      TfsGitChangeType? nullable = new TfsGitChangeType?();
      bool flag3 = false;
      bool flag4 = false;
      bool flag5 = true;
      this.m_neighborList.Clear();
      foreach (int num2 in this.m_graph.OutNeighborsOfLabel(label))
      {
        flag3 = true;
        if (num2 != num1)
          flag1 = this.m_comparer.IsPairTreesame(label, num2);
        if (flag1)
        {
          flag4 = true;
          if (this.m_fullHistory)
          {
            this.m_neighborList.Add(num2);
          }
          else
          {
            this.m_neighborList.Clear();
            this.m_neighborList.Add(num2);
            break;
          }
        }
        else
        {
          bool flag6 = this.m_comparer.HasPath(num2);
          if (this.m_stopAtAdds & flag2 && !flag6)
            this.m_boundaryLabels.Add(num2);
          flag5 = false;
          this.m_neighborList.Add(num2);
          if (!nullable.HasValue)
          {
            if (flag2)
              nullable = !flag6 ? new TfsGitChangeType?(TfsGitChangeType.Add) : new TfsGitChangeType?(TfsGitChangeType.Edit);
            else if (flag6)
            {
              nullable = new TfsGitChangeType?(TfsGitChangeType.Delete);
              objectType = this.m_comparer.GetObjectType(num2);
            }
          }
        }
      }
      if (!this.m_fullHistory ? !flag4 && flag3 | flag2 : !flag5 || !flag3 & flag2)
      {
        this.SetLabelAccepted(label);
        this.m_types[label] = new ChangeAndObjectType((TfsGitChangeType) ((int) nullable ?? 1), objectType);
      }
      return this.m_neighborList.ToArray();
    }
  }
}
