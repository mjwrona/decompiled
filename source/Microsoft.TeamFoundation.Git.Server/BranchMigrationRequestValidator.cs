// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.BranchMigrationRequestValidator
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class BranchMigrationRequestValidator
  {
    public static void Validate(RepoMigrationRequest request)
    {
      if (request.MigrateMetadata && (request.MigrateCode || request.MigrateSecurity))
        throw new ArgumentException("MigrateMetadata should not be set to 'True' if either MigrateCode or MigrateSecurity are also 'True'.");
      if (!request.MigrateCode)
        return;
      Array.ForEach<string>(request.ParentRefNames, (Action<string>) (x => BranchMigrationRequestValidator.CheckIsBranch(x)));
      foreach (string str in request.BranchesToMigrate)
      {
        string branchToMigrate = str;
        BranchMigrationRequestValidator.CheckIsBranch(branchToMigrate);
        Array.ForEach<string>(request.ParentRefNames, (Action<string>) (parentRefName =>
        {
          if (branchToMigrate == parentRefName)
            throw new ArgumentException("Cannot request to migrate the already migrated parent.");
        }));
      }
    }

    private static void CheckIsBranch(string branchName)
    {
      ArgumentUtility.CheckForNull<string>(branchName, nameof (branchName));
      if (!branchName.StartsWith("refs/heads/"))
        throw new ArgumentException("The migrated ref " + branchName + " must be a branch.");
    }
  }
}
