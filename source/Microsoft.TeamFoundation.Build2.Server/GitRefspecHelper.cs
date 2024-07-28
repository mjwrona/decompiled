// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.GitRefspecHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class GitRefspecHelper
  {
    public static string NormalizeSourceBranch(string sourceBranch)
    {
      if (!string.IsNullOrEmpty(sourceBranch) && !GitRefspecHelper.SourceBranchIsNormalized(sourceBranch))
        sourceBranch = "refs/heads/" + sourceBranch;
      return sourceBranch;
    }

    public static bool SourceBranchIsNormalized(string sourceBranch) => !string.IsNullOrEmpty(sourceBranch) && sourceBranch.StartsWith("refs/", StringComparison.InvariantCulture);
  }
}
