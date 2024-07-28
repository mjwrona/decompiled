// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.ChangeTypeOracleExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal static class ChangeTypeOracleExtensions
  {
    public static bool TryGetChangeType(
      this IChangeTypeOracle oracle,
      ITfsGitRepository repo,
      IGitCommitGraph commitGraph,
      Sha1Id commitId,
      string path,
      out ChangeAndObjectType changeType,
      out string relativePath,
      out string renameSourceItemPath)
    {
      if (oracle.TryGetChangeType(commitId, out changeType))
      {
        int label1 = commitGraph.GetLabel(commitId);
        List<int> list = commitGraph.OutNeighborsOfLabel(label1).ToList<int>();
        if ((changeType.ChangeType == TfsGitChangeType.Add || changeType.ChangeType == TfsGitChangeType.Delete) && list.Count > 0)
        {
          int label2 = list[0];
          foreach (TfsGitDiffEntry diffTree in (IEnumerable<TfsGitDiffEntry>) TfsGitDiffHelper.DiffTrees(repo, repo.LookupObject<TfsGitTree>(commitGraph.GetRootTreeId(label2)), repo.LookupObject<TfsGitTree>(commitGraph.GetRootTreeId(label1)), true))
          {
            if ((diffTree.ChangeType & (TfsGitChangeType.Rename | TfsGitChangeType.SourceRename)) != (TfsGitChangeType) 0 && diffTree.RelativePath == path)
            {
              changeType = new ChangeAndObjectType(diffTree.ChangeType, changeType.ObjectType);
              relativePath = diffTree.RelativePath;
              renameSourceItemPath = diffTree.RenameSourceItemPath;
              return true;
            }
          }
        }
        relativePath = path;
        renameSourceItemPath = (string) null;
        return true;
      }
      relativePath = (string) null;
      renameSourceItemPath = (string) null;
      return false;
    }
  }
}
