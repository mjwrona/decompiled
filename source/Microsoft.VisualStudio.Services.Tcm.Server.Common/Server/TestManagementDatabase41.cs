// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase41
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase41 : TestManagementDatabase40
  {
    internal TestManagementDatabase41(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase41()
    {
    }

    internal override void CleanDeletedTestRunDimensions(
      Guid projectId,
      int maxDimensionRowsToDelete,
      int deletionBatchSize,
      int waitDurationForCleanup,
      out int? deletedTestCaseRefs,
      int cleanDeletedTestRunDimensionsSprocExecTimeOutInSec)
    {
      deletedTestCaseRefs = new int?(0);
      this.PrepareStoredProcedure("TestResult.prc_DeleteTestRunDimensions", cleanDeletedTestRunDimensionsSprocExecTimeOutInSec);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@batchSize", deletionBatchSize);
      this.BindInt("@maxDetailsToDelete", maxDimensionRowsToDelete);
      this.BindInt("@waitDaysForCleanup", waitDurationForCleanup);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      if (!sqlDataReader.Read())
        return;
      deletedTestCaseRefs = new int?(sqlDataReader.GetInt32(0));
    }

    internal override TestResultsGroupsDataWithWaterMark GetTestResultAutomatedTestStorageAndOwnersByBuild(
      Guid projectId,
      int buildId,
      string publishContext,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top,
      int runIdThreshold = 0)
    {
      return new TestResultsGroupsDataWithWaterMark(this.GetTestResultAutomatedTestStorageAndOwnersByBuild(projectId, buildId, publishContext, 0), (TestCaseResultIdentifier) null);
    }

    internal override TestResultsGroupsDataWithWaterMark GetTestResultAutomatedTestStorageAndOwnersByRelease(
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top,
      int runIdThreshold = 0)
    {
      return new TestResultsGroupsDataWithWaterMark(this.GetTestResultAutomatedTestStorageAndOwnersByRelease(projectId, releaseId, releaseEnvId, publishContext, 0), (TestCaseResultIdentifier) null);
    }
  }
}
