// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RefUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class RefUtil
  {
    private static readonly char[] s_invalidRefNameCharacters = new char[7]
    {
      '~',
      '^',
      ':',
      '?',
      '*',
      '[',
      '\\'
    };

    public static bool IsValidRefName(string refName, bool enforceTfsSpecificRequirements)
    {
      if (string.IsNullOrWhiteSpace(refName) || !refName.StartsWith("refs/", StringComparison.Ordinal) || enforceTfsSpecificRequirements && refName.Length > GitConstants.MaxGitRefNameLength)
        return false;
      int num = refName.IndexOf('/', "refs/".Length);
      if (num == -1 || num == refName.Length - 1 || refName.Contains("/.") || refName.Contains(".lock/") || refName.EndsWith(".lock", StringComparison.Ordinal) || refName.EndsWith(".", StringComparison.Ordinal) || refName.EndsWith("/", StringComparison.Ordinal) || refName.Contains("//") || refName.Contains("..") || refName.Contains("@{") || refName.IndexOfAny(RefUtil.s_invalidRefNameCharacters) > -1)
        return false;
      for (int index = 0; index < refName.Length; ++index)
      {
        char ch = refName[index];
        if (ch <= ' ' || ch == '\u007F' || enforceTfsSpecificRequirements && index < num && (ch > '\u007F' || ch >= 'A' && ch <= 'Z'))
          return false;
      }
      return true;
    }
  }
}
