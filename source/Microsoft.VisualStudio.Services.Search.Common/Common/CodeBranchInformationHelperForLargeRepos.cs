// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.CodeBranchInformationHelperForLargeRepos
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class CodeBranchInformationHelperForLargeRepos
  {
    public static string GetRegistryKey(string projectName, string repoName, string branchName)
    {
      if (string.IsNullOrWhiteSpace(projectName) || string.IsNullOrWhiteSpace(repoName) || string.IsNullOrWhiteSpace(branchName))
        return (string) null;
      return projectName + ";" + repoName + ";" + CustomUtils.GetBranchNameWithoutPrefix("refs/heads/", branchName);
    }

    public static bool IsBranchIndexing(
      RegistryManagerV2 registryManager,
      string value,
      string collectionId)
    {
      return registryManager.GetRegistryEntry(value, collectionId) != null;
    }

    public static void AddBranchAsIndexing(
      RegistryManagerV2 registryManager,
      string value,
      string collectionId)
    {
      if (CodeBranchInformationHelperForLargeRepos.IsBranchIndexing(registryManager, value, collectionId))
        return;
      registryManager.AddOrUpdateRegistryValue(value, collectionId, true.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static void DeleteBranchFromIndexingList(
      RegistryManagerV2 registryManager,
      string value,
      string collectionId)
    {
      if (!CodeBranchInformationHelperForLargeRepos.IsBranchIndexing(registryManager, value, collectionId))
        return;
      registryManager.RemoveRegistryEntry(value, collectionId);
    }
  }
}
