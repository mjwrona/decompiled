// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase53
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase53 : TestManagementDatabase52
  {
    public override int QueryTestPullRequestBuildId(
      Guid projectGuid,
      Guid repositoryId,
      int pullRequestId,
      int pullRequestIterationId)
    {
      this.PrepareStoredProcedure("prc_QueryTestPullRequestBuilds");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindGuid("@repositoryId", repositoryId);
      this.BindInt("@pullRequestId", pullRequestId);
      this.BindInt("@pullRequestIterationId", pullRequestIterationId);
      object obj = this.ExecuteScalar();
      int result;
      return obj != null && int.TryParse(obj.ToString(), out result) ? result : 0;
    }

    public override void AddOrUpdateTestPullRequestBuilds(
      Guid projectGuid,
      Guid repositoryId,
      int pullRequestId,
      int pullRequestIterationId,
      int buildId)
    {
      this.PrepareStoredProcedure("prc_AddOrUpdateTestPullRequestBuilds");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindGuid("@repositoryId", repositoryId);
      this.BindInt("@pullRequestId", pullRequestId);
      this.BindInt("@pullRequestIterationId", pullRequestIterationId);
      this.BindInt("@buildId", buildId);
      this.ExecuteNonQuery();
    }

    internal TestManagementDatabase53(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase53()
    {
    }
  }
}
