// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TreeAnalysis
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class TreeAnalysis
  {
    private TreeAnalysis()
    {
    }

    public Dictionary<string, IList<TreeNode>> NodeDictionary { get; } = new Dictionary<string, IList<TreeNode>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public HashSet<string> FileTypes { get; } = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public HashSet<string> DirectoryExtensions { get; } = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public int FileCount { get; private set; }

    public int DirectoryCount { get; private set; }

    public long TimeToRetrieveDataMs { get; private set; }

    public long TimeToTraverseTreeMs { get; private set; }

    public class Builder
    {
      private readonly IEnumerable<TreeNode> m_nodeList;
      private readonly long m_timeToRetrieveDataMs;
      private readonly long m_timeToTraverseTreeMs;

      public Builder(
        IEnumerable<TreeNode> nodeList,
        long timeToRetrieveDataMs,
        long timeToTraverseTreeMs)
      {
        this.m_nodeList = nodeList;
        this.m_timeToRetrieveDataMs = timeToRetrieveDataMs;
        this.m_timeToTraverseTreeMs = timeToTraverseTreeMs;
      }

      public Builder(TreeTraversalResult treeTraversalResult)
      {
        this.m_nodeList = treeTraversalResult.TreeNodes;
        this.m_timeToRetrieveDataMs = treeTraversalResult.TimeToRetrieveDataMs;
        this.m_timeToTraverseTreeMs = treeTraversalResult.TimeToTraverseTreeMs;
      }

      public TreeAnalysis Build()
      {
        TreeAnalysis treeAnalysis = new TreeAnalysis()
        {
          TimeToRetrieveDataMs = this.m_timeToRetrieveDataMs,
          TimeToTraverseTreeMs = this.m_timeToTraverseTreeMs
        };
        foreach (TreeNode node in this.m_nodeList)
        {
          if (node.IsDirectory)
          {
            ++treeAnalysis.DirectoryCount;
            treeAnalysis.DirectoryExtensions.Add(TreeAnalysis.Builder.GetExtension(node.Name));
          }
          else
          {
            ++treeAnalysis.FileCount;
            treeAnalysis.FileTypes.Add(TreeAnalysis.Builder.GetExtension(node.Name));
          }
          treeAnalysis.NodeDictionary.AddOrUpdate<string, IList<TreeNode>>(node.Name, (IList<TreeNode>) new List<TreeNode>()
          {
            node
          }, (Func<IList<TreeNode>, IList<TreeNode>, IList<TreeNode>>) ((existingValue, newValue) => existingValue.AddRange<TreeNode, IList<TreeNode>>((IEnumerable<TreeNode>) newValue)));
        }
        return treeAnalysis;
      }

      private static string GetExtension(string name)
      {
        int startIndex = name.LastIndexOf('.');
        return startIndex <= 0 ? string.Empty : name.Substring(startIndex);
      }
    }
  }
}
