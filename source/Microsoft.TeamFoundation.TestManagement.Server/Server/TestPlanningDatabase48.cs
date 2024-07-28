// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase48
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase48 : TestPlanningDatabase47
  {
    internal TestPlanningDatabase48(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase48()
    {
    }

    internal override TestPlan CreateTestPlanInternal(
      TestManagementRequestContext context,
      Guid projectGuid,
      TestPlan plan,
      Guid updatedBy,
      TestExternalLink[] links,
      TestPlanSource type)
    {
      try
      {
        context.TraceEnter("Database", "TestPlanDatabase.CreateTestPlanInternal");
        Validator.CheckStartEndDatesInOrder(plan.StartDate, plan.EndDate);
        this.PrepareStoredProcedure("prc_CreatePlan");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@planId", plan.PlanId);
        this.BindByte("@type", (byte) type);
        this.BindByte("@planState", plan.State);
        this.BindString("@buildUri", plan.BuildUri, 64, false, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@buildDefinition", plan.BuildDefinition, 260, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@buildQuality", plan.BuildQuality, 256, SqlDbType.NVarChar);
        this.BindInt("@testSettingsId", plan.TestSettingsId);
        this.BindInt("@automatedTestSettingsId", plan.AutomatedTestSettingsId);
        this.BindGuid("@manualTestEnvironmentId", plan.ManualTestEnvironmentId);
        this.BindGuid("@automatedTestEnvironmentId", plan.AutomatedTestEnvironmentId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindInt("@rootSuiteId", plan.RootSuiteId);
        this.BindString("@rootSuiteStatus", plan.RootSuiteStatus, 256, false, SqlDbType.NVarChar);
        this.BindInt("@buildDefinitionId", plan.BuildDefinitionId);
        int parameterValue1 = 0;
        int parameterValue2 = 0;
        if (plan.ReleaseEnvDef != null)
        {
          parameterValue1 = plan.ReleaseEnvDef.ReleaseDefinitionId;
          parameterValue2 = plan.ReleaseEnvDef.ReleaseEnvDefinitionId;
        }
        this.BindInt("@releaseDefinitionId", parameterValue1);
        this.BindInt("@releaseEnvDefinitionId", parameterValue2);
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
          throw new UnexpectedDatabaseResultException("prc_CreatePlan");
        TestPlanningDatabase.CreateTestPlanColumns createTestPlanColumns = new TestPlanningDatabase.CreateTestPlanColumns();
        plan.RootSuiteId = createTestPlanColumns.RootSuiteId.GetInt32((IDataReader) reader);
        TestPlanningDatabase.UpdateTestPlansColumns testPlansColumns = new TestPlanningDatabase.UpdateTestPlansColumns();
        plan.BuildTakenDate = testPlansColumns.BuildTakenDate.GetDateTime((IDataReader) reader);
        plan.LastUpdatedBy = updatedBy;
        return plan;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      finally
      {
        context.TraceLeave("Database", "TestPlanDatabase.CreateTestPlanInternal");
      }
    }

    internal override BlockedPointProperties BlockTestPoint2(
      Guid projectGuid,
      TestPoint point,
      TestCaseResult result,
      Guid updatedBy)
    {
      this.PrepareStoredProcedure("TestManagement.prc_BlockTestPoint2");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@pointId", point.PointId);
      this.BindInt("@planId", point.PlanId);
      this.BindInt("@testRunId", result.TestRunId);
      this.BindInt("@testResultId", result.TestResultId);
      this.BindInt("@testCaseId", result.TestCaseId);
      this.BindString("@testCaseArea", result.TestCaseAreaUri, 256, true, SqlDbType.NVarChar);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindInt("@revision", point.Revision);
      SqlDataReader reader = this.ExecuteReader();
      BlockedPointProperties blockedPointProperties = reader.Read() ? new TestPlanningDatabase.UpdatedPropertyColumns().bindBlockedTestPointProperties(reader) : throw new UnexpectedDatabaseResultException("prc_BlockTestPoint2");
      blockedPointProperties.LastTestRunId = result.TestRunId;
      blockedPointProperties.LastUpdatedBy = updatedBy;
      return blockedPointProperties;
    }

    internal override (int PlanId, List<int> PointIds) DestroyTestCase(
      int testCaseId,
      List<UpdatedProperties> suiteProps,
      out Guid projectGuid)
    {
      projectGuid = Guid.Empty;
      (int, List<int>) valueTuple = (-1, new List<int>());
      this.PrepareStoredProcedure("prc_DestroyTestCase");
      this.BindInt("@testCaseId", testCaseId);
      this.BindUpdatedPropertiesTypeTable("@suitePropTable", (IEnumerable<UpdatedProperties>) suiteProps);
      SqlDataReader reader = this.ExecuteReader();
      if (reader.Read())
      {
        int int32 = new SqlColumnBinder("DataspaceId").GetInt32((IDataReader) reader);
        if (int32 > 0)
          projectGuid = this.GetDataspaceIdentifier(int32);
      }
      if (reader.NextResult() && reader.Read())
        valueTuple.Item1 = new SqlColumnBinder("PlanId").GetInt32((IDataReader) reader);
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("PointId");
      if (reader.NextResult())
      {
        while (reader.Read())
          valueTuple.Item2.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
      }
      return valueTuple;
    }
  }
}
