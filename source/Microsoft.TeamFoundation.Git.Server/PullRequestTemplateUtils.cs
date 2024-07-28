// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestTemplateUtils
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class PullRequestTemplateUtils
  {
    private static readonly char[] c_pathSeparators = new char[2]
    {
      '/',
      '\\'
    };
    private const int c_MaxRecursionDepth = 100;

    public static TfsGitObject FindMember(
      TfsGitTree tree,
      ref string path,
      out TfsGitTreeEntry treeEntry)
    {
      return PullRequestTemplateUtils.FindMember(tree, ref path, out treeEntry, 0);
    }

    private static TfsGitObject FindMember(
      TfsGitTree tree,
      ref string path,
      out TfsGitTreeEntry treeEntry,
      int recursionDepth)
    {
      if (recursionDepth > 100)
        throw new GitRecursionLimitReachedException(100);
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      string str = (string) null;
      string path1 = (string) null;
      TfsGitObject tree1 = (TfsGitObject) null;
      treeEntry = (TfsGitTreeEntry) null;
      int length;
      do
      {
        length = path.IndexOfAny(PullRequestTemplateUtils.c_pathSeparators);
        if (length == 0)
          path = path.Substring(1);
        else if (length < 0)
          str = path;
        else if (length == path.Length - 1)
        {
          path = path.Substring(0, path.Length - 1);
          length = 0;
        }
        else
        {
          str = path.Substring(0, length);
          path1 = path.Substring(length + 1);
        }
      }
      while (length == 0);
      List<TfsGitTreeEntry> tfsGitTreeEntryList = new List<TfsGitTreeEntry>();
      foreach (TfsGitTreeEntry treeEntry1 in tree.GetTreeEntries())
      {
        if (treeEntry1.Name.Equals(str, StringComparison.Ordinal))
        {
          if (treeEntry1.ObjectType != GitObjectType.Commit)
            tree1 = treeEntry1.Object;
          treeEntry = treeEntry1;
          break;
        }
        if (treeEntry1.Name.Equals(str, StringComparison.CurrentCultureIgnoreCase))
          tfsGitTreeEntryList.Add(treeEntry1);
      }
      if (treeEntry == null && tfsGitTreeEntryList.Count == 1)
      {
        TfsGitTreeEntry tfsGitTreeEntry = tfsGitTreeEntryList[0];
        if (tfsGitTreeEntry.ObjectType == GitObjectType.Commit)
          path = tfsGitTreeEntry.Name;
        else
          tree1 = tfsGitTreeEntry.Object;
        str = tfsGitTreeEntry.Name;
        treeEntry = tfsGitTreeEntry;
      }
      if (path1 == null)
      {
        path = str;
        return tree1;
      }
      TfsGitObject member = (TfsGitObject) null;
      treeEntry = (TfsGitTreeEntry) null;
      if (tree1 is TfsGitTree)
        member = PullRequestTemplateUtils.FindMember((TfsGitTree) tree1, ref path1, out treeEntry, recursionDepth + 1);
      path = str + "/" + path1;
      return member;
    }
  }
}
