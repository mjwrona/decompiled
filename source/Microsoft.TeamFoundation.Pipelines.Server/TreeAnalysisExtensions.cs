// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.TreeAnalysisExtensions
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal static class TreeAnalysisExtensions
  {
    public static bool TryGetFileContents(
      this TreeAnalysis treeAnalysis,
      IVssRequestContext requestContext,
      IFileContentsProvider fileContentsProvider,
      string filename,
      out string contents)
    {
      FilePath filePath;
      if (treeAnalysis.TryGetFilePath(filename, out filePath))
        return fileContentsProvider.TryGetFileContents(requestContext, filePath.ToString(), out contents);
      contents = (string) null;
      return false;
    }

    public static IEnumerable<FilePath> GetMatchingFilesByExtension(
      this TreeAnalysis treeAnalysis,
      params string[] extensions)
    {
      foreach (KeyValuePair<string, IList<TreeNode>> node in treeAnalysis.NodeDictionary)
      {
        KeyValuePair<string, IList<TreeNode>> pair = node;
        string[] strArray = extensions;
        for (int index = 0; index < strArray.Length; ++index)
        {
          if (pair.Key.EndsWith(strArray[index], StringComparison.OrdinalIgnoreCase))
          {
            foreach (TreeNode treeNode in (IEnumerable<TreeNode>) pair.Value)
            {
              if (!treeNode.IsDirectory)
                yield return new FilePath(treeNode.Path);
            }
          }
        }
        strArray = (string[]) null;
        pair = new KeyValuePair<string, IList<TreeNode>>();
      }
    }

    public static bool TryGetFilePath(
      this TreeAnalysis treeAnalysis,
      string filename,
      out FilePath filePath)
    {
      IReadOnlyList<FilePath> filePaths;
      if (!treeAnalysis.TryGetFilePaths(filename, out filePaths))
      {
        filePath = (FilePath) null;
        return false;
      }
      filePath = filePaths.FirstOrDefault<FilePath>();
      return filePath != (FilePath) null;
    }

    public static bool TryGetFilePaths(
      this TreeAnalysis treeAnalysis,
      string filename,
      out IReadOnlyList<FilePath> filePaths)
    {
      IList<TreeNode> source;
      if (!treeAnalysis.NodeDictionary.TryGetValue(filename, out source))
      {
        filePaths = (IReadOnlyList<FilePath>) null;
        return false;
      }
      filePaths = (IReadOnlyList<FilePath>) source.Where<TreeNode>((Func<TreeNode, bool>) (n => !n.IsDirectory)).Select<TreeNode, FilePath>((Func<TreeNode, FilePath>) (n => new FilePath(n.Path))).ToList<FilePath>();
      return filePaths.Count > 0;
    }
  }
}
