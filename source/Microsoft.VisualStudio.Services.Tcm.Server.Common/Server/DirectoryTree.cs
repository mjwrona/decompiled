// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DirectoryTree
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Coverage.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class DirectoryTree
  {
    internal DirectoryTree.DirectoryTreeNode Root = new DirectoryTree.DirectoryTreeNode(string.Empty);

    internal DirectoryTree(
      SortedDictionary<string, DirectoryCoverageSummary> allDirectoriesCoverageSummary)
    {
      foreach (string key in allDirectoriesCoverageSummary.Keys)
      {
        if (!(key == string.Empty))
        {
          string[] strArray = key.Split(Path.DirectorySeparatorChar);
          DirectoryTree.DirectoryTreeNode directoryTreeNode = this.Root;
          foreach (string directoryName in strArray)
          {
            bool flag = false;
            foreach (DirectoryTree.DirectoryTreeNode child in directoryTreeNode.Children)
            {
              if (directoryName.Equals(child.DirectoryName, StringComparison.OrdinalIgnoreCase))
              {
                directoryTreeNode = child;
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              directoryTreeNode.Children.Add(new DirectoryTree.DirectoryTreeNode(directoryName));
              directoryTreeNode = directoryTreeNode.Children.Last<DirectoryTree.DirectoryTreeNode>();
            }
          }
        }
      }
    }

    internal void ComputeSummaryForImmediateDirectories(
      DirectoryTree.DirectoryTreeNode directoryTreeNode,
      string directoryPath,
      SortedDictionary<string, DirectoryCoverageSummary> directoriesCoverageSummary,
      string scope)
    {
      directoryPath = Path.Combine(directoryPath, directoryTreeNode.DirectoryName);
      foreach (DirectoryTree.DirectoryTreeNode child in directoryTreeNode.Children)
      {
        this.ComputeSummaryForImmediateDirectories(child, directoryPath, directoriesCoverageSummary, scope);
        if (!directoriesCoverageSummary.ContainsKey(directoryPath))
        {
          DirectoryCoverageSummary directoryCoverageSummary = new DirectoryCoverageSummary()
          {
            Scope = scope,
            Summary = new CoverageSummary()
            {
              Path = directoryPath,
              IsDirectory = true
            },
            Children = new List<CoverageSummary>()
          };
          directoriesCoverageSummary.Add(directoryPath, directoryCoverageSummary);
        }
        string key = Path.Combine(directoryPath, child.DirectoryName);
        directoriesCoverageSummary[directoryPath].Summary.Covered += directoriesCoverageSummary[key].Summary.Covered;
        directoriesCoverageSummary[directoryPath].Summary.PartiallyCovered += directoriesCoverageSummary[key].Summary.PartiallyCovered;
        directoriesCoverageSummary[directoryPath].Summary.NotCovered += directoriesCoverageSummary[key].Summary.NotCovered;
        directoriesCoverageSummary[directoryPath].Children.Add(directoriesCoverageSummary[key].Summary);
      }
    }

    internal class DirectoryTreeNode
    {
      public string DirectoryName { get; set; }

      public List<DirectoryTree.DirectoryTreeNode> Children { get; set; }

      public DirectoryTreeNode(string directoryName)
      {
        this.DirectoryName = directoryName;
        this.Children = new List<DirectoryTree.DirectoryTreeNode>();
      }
    }
  }
}
