// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase44
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase44 : TestPlanningDatabase43
  {
    internal override TestArtifactsAssociatedItemsModel QueryTestCaseAssociatedTestArtifacts(
      TestManagementRequestContext context,
      Guid projectGuid,
      int testCaseId,
      bool isTcmService)
    {
      context.TraceEnter("Database", "TestCaseDatabase.QueryTestCaseAssociatedTestArtifacts");
      string storedProcedure = "prc_QueryTestCaseAssociatedTestArtifactsV2";
      this.PrepareStoredProcedure(storedProcedure);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@testCaseId", testCaseId);
      this.BindBoolean("@isTcmService", isTcmService);
      TestArtifactsAssociatedItemsModel associatedItemsModel = this.GetTestArtifactsAssociatedItemsModel(context, storedProcedure);
      context.TraceLeave("Database", "TestCaseDatabase.QueryTestCaseAssociatedTestArtifacts");
      return associatedItemsModel;
    }

    internal TestPlanningDatabase44(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase44()
    {
    }

    internal override UpdatedProperties UpdateTestPoint(
      Guid projectGuid,
      TestPoint point,
      Guid updatedBy,
      bool updateRunResultsInTCMbool,
      bool considerUnassignedTesters = false,
      bool ResetToActive = false)
    {
      try
      {
        this.PrepareStoredProcedure("TestManagement.prc_UpdateTestPoint");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@planId", point.PlanId);
        this.BindInt("@pointId", point.PointId);
        this.BindStringPreserveNull("@comment", point.Comment, 1048576, SqlDbType.NVarChar);
        if (considerUnassignedTesters)
          this.BindGuid("@assignedTo", point.AssignedTo);
        else
          this.BindGuidPreserveNull("@assignedTo", point.AssignedTo);
        this.BindInt("@revision", point.Revision);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindByte("@state", point.State, (byte) 0);
        this.BindBoolean("@updateRunResultsInTCM", updateRunResultsInTCMbool);
        SqlDataReader reader = this.ExecuteReader();
        UpdatedProperties updatedProperties = reader.Read() ? new TestPlanningDatabase.UpdatedPropertyColumns().bindUpdatedProperties(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateTestPoint");
        updatedProperties.LastUpdatedBy = updatedBy;
        return updatedProperties;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
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
      TestArtifactsAssociatedItemsModel associatedItemsModel = this.GetTestArtifactsAssociatedItemsModel(context, "prc_QueryTestSuiteAssociatedTestArtifacts");
      context.TraceLeave("Database", "SuiteDatabase.QueryTestSuiteAssociatedTestArtifacts");
      return associatedItemsModel;
    }
  }
}
