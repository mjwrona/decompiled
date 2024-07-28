// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildSourceVersion
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Common
{
  public static class BuildSourceVersion
  {
    public static readonly string GitPrefix = "LG";
    public static readonly string GitSeparator = ":";
    public static readonly string GitLatest = "LG:master:";

    public static bool IsValidSourceVersion(string sourceVersion) => BuildSourceVersion.IsGit(sourceVersion);

    public static bool IsGit(string sourceVersion) => !string.IsNullOrEmpty(sourceVersion) && sourceVersion.StartsWith(BuildSourceVersion.GitPrefix, StringComparison.OrdinalIgnoreCase) && sourceVersion.Contains(BuildSourceVersion.GitSeparator);

    public static string FormatGit(string branch, string commit) => BuildSourceVersion.GitPrefix + BuildSourceVersion.GitSeparator + branch + BuildSourceVersion.GitSeparator + commit;

    public static string FormatCommit(byte[] commit) => commit != null && commit.Length != 0 ? string.Join(string.Empty, ((IEnumerable<byte>) commit).Select<byte, string>((Func<byte, string>) (b => b.ToString("x2"))).ToArray<string>()) : string.Empty;

    public static bool TryParseGit(string sourceVersion, out string branch, out string commit)
    {
      branch = string.Empty;
      commit = string.Empty;
      if (!BuildSourceVersion.IsGit(sourceVersion))
        return false;
      string[] strArray = sourceVersion.Split(new string[1]
      {
        BuildSourceVersion.GitSeparator
      }, StringSplitOptions.None);
      if (strArray.Length > 1)
        branch = strArray[1];
      if (strArray.Length > 2)
        commit = strArray[2];
      return true;
    }
  }
}
