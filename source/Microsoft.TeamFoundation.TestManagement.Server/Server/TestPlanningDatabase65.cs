// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase65
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase65 : TestPlanningDatabase64
  {
    internal override bool CleanDeletedTestPoints(int waitDaysForCleanup, int deletionBatchSize)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestPointDatabase.CleanDeletedTestPoints"))
      {
        this.PrepareStoredProcedure("TestManagement.prc_DeleteTestPoint");
        this.BindInt("@waitDaysForCleanup", waitDaysForCleanup);
        this.BindInt("@deletionBatchSize", deletionBatchSize);
        return (int) this.ExecuteScalar() == 0;
      }
    }

    internal TestPlanningDatabase65(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase65()
    {
    }

    internal override TestArtifactsAssociatedItemsModel QueryTestPlanAssociatedTestArtifacts(
      TestManagementRequestContext context,
      Guid projectGuid,
      int testPlanId,
      bool isTcmService,
      int pointQueryLimit)
    {
      context.TraceEnter("Database", "TestPlanDatabase.QueryTestPlanAssociatedTestArtifacts");
      this.PrepareStoredProcedure("prc_QueryTestPlanAssociatedTestArtifacts");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@testPlanId", testPlanId);
      this.BindBoolean("@isTcmService", isTcmService);
      this.BindInt("@pointQueryLimit", pointQueryLimit);
      TestArtifactsAssociatedItemsModel associatedItemsModel = this.GetTestArtifactsAssociatedItemsModel(context, "prc_QueryTestPlanAssociatedTestArtifacts");
      context.TraceLeave("Database", "TestPlanDatabase.QueryTestPlanAssociatedTestArtifacts");
      return associatedItemsModel;
    }

    internal override TestArtifactsAssociatedItemsModel QueryTestSuiteAssociatedTestArtifacts(
      TestManagementRequestContext context,
      Guid projectGuid,
      int testSuiteId,
      bool isTcmService,
      int pointQueryLimit)
    {
      context.TraceEnter("Database", "SuiteDatabase.QueryTestSuiteAssociatedTestArtifacts");
      this.PrepareStoredProcedure("prc_QueryTestSuiteAssociatedTestArtifacts");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@testSuiteId", testSuiteId);
      this.BindBoolean("@isTcmService", isTcmService);
      this.BindInt("@pointQueryLimit", pointQueryLimit);
      TestArtifactsAssociatedItemsModel associatedItemsModel = this.GetTestArtifactsAssociatedItemsModel(context, "prc_QueryTestSuiteAssociatedTestArtifacts");
      context.TraceLeave("Database", "SuiteDatabase.QueryTestSuiteAssociatedTestArtifacts");
      return associatedItemsModel;
    }
  }
}
