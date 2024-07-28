// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PathUtils
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class PathUtils
  {
    public static bool CheckPathsMatchPatterns(
      IEnumerable<string> paths,
      IEnumerable<string> patterns,
      out string exampleMatchingPath,
      out int countOfMatchingPaths)
    {
      countOfMatchingPaths = 0;
      exampleMatchingPath = (string) null;
      foreach (string path in paths)
      {
        if (PathUtils.CheckPathMatchesPatterns(path, patterns) != null)
        {
          exampleMatchingPath = path;
          ++countOfMatchingPaths;
        }
      }
      return countOfMatchingPaths > 0;
    }

    public static bool FastCheckPathsMatchPatterns(
      IEnumerable<string> paths,
      IEnumerable<string> patterns)
    {
      foreach (string path in paths)
      {
        if (PathUtils.CheckPathMatchesPatterns(path, patterns) != null)
          return true;
      }
      return false;
    }

    public static string CheckPathMatchesPatterns(string path, IEnumerable<string> patterns)
    {
      bool flag1 = false;
      string str = (string) null;
      if (patterns == null)
        patterns = (IEnumerable<string>) Array.Empty<string>();
      foreach (string pattern1 in patterns)
      {
        bool flag2 = pattern1.StartsWithChar('!');
        if (flag2 & flag1)
        {
          string pattern2 = pattern1.Substring(1);
          if (Wildcard.Match(path, pattern2))
          {
            flag1 = false;
            str = (string) null;
          }
        }
        else if (!flag2 && !flag1 && Wildcard.Match(path, pattern1))
        {
          flag1 = true;
          str = pattern1;
        }
      }
      return !flag1 ? (string) null : str;
    }
  }
}
