// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.GitHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class GitHelper
  {
    internal static bool IsPullRequest(string branchName) => !string.IsNullOrEmpty(branchName) && branchName.StartsWith("refs/pull/");

    internal static string GetModifiedBranchName(string branchName) => !GitHelper.IsPullRequest(branchName) ? branchName : "refs/pull/*";
  }
}
