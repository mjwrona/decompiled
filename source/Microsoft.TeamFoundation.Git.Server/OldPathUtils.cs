// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.OldPathUtils
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  [Obsolete("Use Microsoft.TeamFoundation.Git.Server.PathUtil.cs instead, left it here to have a fall back covered by FF")]
  public class OldPathUtils
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
        if (OldPathUtils.CheckPathMatchesPatterns(path, patterns, out string _))
        {
          exampleMatchingPath = path;
          ++countOfMatchingPaths;
        }
      }
      return countOfMatchingPaths > 0;
    }

    public static bool CheckPathMatchesPatterns(
      string path,
      IEnumerable<string> patterns,
      out string exampleMatchingPattern)
    {
      bool flag1 = false;
      if (patterns == null)
        patterns = (IEnumerable<string>) Array.Empty<string>();
      exampleMatchingPattern = (string) null;
      foreach (string pattern1 in patterns)
      {
        bool flag2 = pattern1.StartsWith("!");
        if (flag2 & flag1)
        {
          string pattern2 = pattern1.Substring(1);
          if (Wildcard.Match(path, pattern2))
          {
            flag1 = false;
            exampleMatchingPattern = (string) null;
          }
        }
        else if (!flag2 && !flag1 && Wildcard.Match(path, pattern1))
        {
          flag1 = true;
          exampleMatchingPattern = pattern1;
        }
      }
      return flag1;
    }

    public static bool CheckPathsMatchPatterns2(
      IEnumerable<string> paths,
      IEnumerable<string> patterns)
    {
      foreach (string path in paths)
      {
        if (OldPathUtils.CheckPathMatchesPatterns2(path, patterns))
          return true;
      }
      return false;
    }

    private static bool CheckPathMatchesPatterns2(string path, IEnumerable<string> patterns)
    {
      bool flag1 = false;
      patterns = (IEnumerable<string>) ((object) patterns ?? (object) Array.Empty<string>());
      foreach (string pattern1 in patterns)
      {
        bool flag2 = pattern1.Length > 0 && pattern1[0] == '!';
        if (flag2 & flag1)
        {
          string pattern2 = pattern1.Substring(1);
          if (Wildcard.Match(path, pattern2))
            flag1 = false;
        }
        else if (!flag2 && !flag1 && Wildcard.Match(path, pattern1))
          flag1 = true;
      }
      return flag1;
    }
  }
}
