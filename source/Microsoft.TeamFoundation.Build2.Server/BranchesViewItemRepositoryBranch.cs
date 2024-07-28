// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BranchesViewItemRepositoryBranch
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BranchesViewItemRepositoryBranch
  {
    public BranchesViewItemRepositoryBranch(
      int buildId,
      int repositoryId,
      string repositoryIdentifier,
      string repositoryType,
      int branchId,
      string branchName)
    {
      this.BuildId = buildId;
      this.RepositoryId = repositoryId;
      this.Repository = new MinimalBuildRepository()
      {
        Id = repositoryIdentifier,
        Type = repositoryType
      };
      this.BranchId = branchId;
      this.BranchName = branchName;
    }

    public int BuildId { get; }

    public int RepositoryId { get; }

    public MinimalBuildRepository Repository { get; }

    public int BranchId { get; }

    public string BranchName { get; }
  }
}
