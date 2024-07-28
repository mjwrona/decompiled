// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase54
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase54 : TestPlanningDatabase53
  {
    internal TestPlanningDatabase54(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase54()
    {
    }

    protected override List<TestPoint> FetchTestPoints(
      Guid projectGuid,
      int planId,
      IdAndRev[] idsToFetch,
      List<int> deletedIds,
      bool includeSuiteName)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestPlanDatabase.FetchTestPoints"))
      {
        this.PrepareStoredProcedure("prc_FetchTestPoints");
        this.BindIdAndRevTypeTable("@idsTable", (IEnumerable<IdAndRev>) idsToFetch);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@planId", planId);
        List<TestPoint> testPointList = new List<TestPoint>();
        TestPlanningDatabase54.FetchTestPointsColumns testPointsColumns = new TestPlanningDatabase54.FetchTestPointsColumns();
        SqlDataReader reader = this.ExecuteReader();
        while (reader.Read())
          testPointList.Add(testPointsColumns.bind(reader, includeSuiteName));
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_FetchTestPoints");
        new TestPlanningDatabase.IdsPropertyColumns().bind(reader, deletedIds);
        return testPointList;
      }
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
        this.BindBoolean("@ResetToActive", ResetToActive);
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

    internal override List<TestPoint> QueryTestPointHistory(int testPointId, Guid projectGuid)
    {
      List<TestPoint> testPointList = new List<TestPoint>();
      this.PrepareStoredProcedure("TestManagement.prc_QueryPointHistory");
      this.BindInt("@testPointId", testPointId);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      SqlDataReader reader = this.ExecuteReader();
      TestPlanningDatabase54.QueryTestPointHistoryColumns pointHistoryColumns = new TestPlanningDatabase54.QueryTestPointHistoryColumns();
      if (reader.HasRows)
      {
        while (reader.Read())
          testPointList.Add(pointHistoryColumns.bind(reader));
      }
      return testPointList;
    }

    protected new class FetchTestPointsColumns
    {
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder ConfigurationName = new SqlColumnBinder(nameof (ConfigurationName));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder FailureType = new SqlColumnBinder(nameof (FailureType));
      private SqlColumnBinder LastTestRunId = new SqlColumnBinder(nameof (LastTestRunId));
      private SqlColumnBinder LastTestResultId = new SqlColumnBinder(nameof (LastTestResultId));
      private SqlColumnBinder LastResultState = new SqlColumnBinder(nameof (LastResultState));
      private SqlColumnBinder LastResultOutcome = new SqlColumnBinder(nameof (LastResultOutcome));
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder AssignedTo = new SqlColumnBinder(nameof (AssignedTo));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder LastResolutionStateId = new SqlColumnBinder(nameof (LastResolutionStateId));
      private SqlColumnBinder SuiteName = new SqlColumnBinder(nameof (SuiteName));
      private SqlColumnBinder SequenceNumber = new SqlColumnBinder(nameof (SequenceNumber));
      private SqlColumnBinder LastResetToActive = new SqlColumnBinder(nameof (LastResetToActive));

      internal TestPoint bind(SqlDataReader reader, bool includeSuiteName = false)
      {
        TestPoint testPoint = new TestPoint();
        testPoint.PointId = this.PointId.GetInt32((IDataReader) reader);
        testPoint.PlanId = this.PlanId.GetInt32((IDataReader) reader);
        testPoint.SuiteId = this.SuiteId.GetInt32((IDataReader) reader);
        testPoint.ConfigurationId = this.ConfigurationId.GetInt32((IDataReader) reader);
        testPoint.ConfigurationName = this.ConfigurationName.GetString((IDataReader) reader, true);
        testPoint.State = this.State.GetByte((IDataReader) reader);
        testPoint.FailureType = this.FailureType.GetByte((IDataReader) reader, (byte) 0);
        testPoint.LastTestRunId = this.LastTestRunId.GetInt32((IDataReader) reader);
        testPoint.LastTestResultId = this.LastTestResultId.GetInt32((IDataReader) reader);
        testPoint.LastResultState = this.LastResultState.GetByte((IDataReader) reader, (byte) 0);
        testPoint.LastResultOutcome = this.LastResultOutcome.GetByte((IDataReader) reader, (byte) 0);
        testPoint.TestCaseId = this.TestCaseId.GetInt32((IDataReader) reader);
        testPoint.Comment = this.Comment.GetString((IDataReader) reader, true);
        testPoint.AssignedTo = this.AssignedTo.GetGuid((IDataReader) reader, false);
        testPoint.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        testPoint.LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false);
        testPoint.Revision = this.Revision.GetInt32((IDataReader) reader);
        testPoint.LastResolutionStateId = this.LastResolutionStateId.GetInt32((IDataReader) reader, 0);
        testPoint.SequenceNumber = this.SequenceNumber.GetInt32((IDataReader) reader, -1, -1);
        testPoint.LastResetToActive = this.LastResetToActive.GetDateTime((IDataReader) reader);
        if (includeSuiteName)
          testPoint.SuiteName = this.SuiteName.GetString((IDataReader) reader, true);
        return testPoint;
      }

      internal TestPoint bindTestPointForInlineTest(SqlDataReader reader) => new TestPoint()
      {
        PointId = this.PointId.GetInt32((IDataReader) reader),
        PlanId = this.PlanId.GetInt32((IDataReader) reader),
        SuiteId = this.SuiteId.GetInt32((IDataReader) reader),
        State = this.State.GetByte((IDataReader) reader),
        LastResultState = this.LastResultState.GetByte((IDataReader) reader, (byte) 0),
        LastResultOutcome = this.LastResultOutcome.GetByte((IDataReader) reader, (byte) 0),
        TestCaseId = this.TestCaseId.GetInt32((IDataReader) reader),
        LastResetToActive = this.LastResetToActive.GetDateTime((IDataReader) reader),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader),
        SequenceNumber = this.SequenceNumber.GetInt32((IDataReader) reader, -1, -1)
      };
    }

    protected new class QueryTestPointHistoryColumns
    {
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder FailureType = new SqlColumnBinder(nameof (FailureType));
      private SqlColumnBinder LastTestRunId = new SqlColumnBinder(nameof (LastTestRunId));
      private SqlColumnBinder LastTestResultId = new SqlColumnBinder(nameof (LastTestResultId));
      private SqlColumnBinder LastResultState = new SqlColumnBinder(nameof (LastResultState));
      private SqlColumnBinder LastResultOutcome = new SqlColumnBinder(nameof (LastResultOutcome));
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder AssignedTo = new SqlColumnBinder(nameof (AssignedTo));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder LastResolutionStateId = new SqlColumnBinder(nameof (LastResolutionStateId));
      private SqlColumnBinder LastResetToActive = new SqlColumnBinder(nameof (LastResetToActive));

      internal TestPoint bind(SqlDataReader reader) => new TestPoint()
      {
        PointId = this.PointId.GetInt32((IDataReader) reader),
        PlanId = this.PlanId.GetInt32((IDataReader) reader),
        SuiteId = this.SuiteId.GetInt32((IDataReader) reader),
        ConfigurationId = this.ConfigurationId.GetInt32((IDataReader) reader),
        State = this.State.GetByte((IDataReader) reader),
        FailureType = this.FailureType.GetByte((IDataReader) reader, (byte) 0),
        LastTestRunId = this.LastTestRunId.GetInt32((IDataReader) reader),
        LastTestResultId = this.LastTestResultId.GetInt32((IDataReader) reader),
        LastResultState = this.LastResultState.GetByte((IDataReader) reader, (byte) 0),
        LastResultOutcome = this.LastResultOutcome.GetByte((IDataReader) reader, (byte) 0),
        TestCaseId = this.TestCaseId.GetInt32((IDataReader) reader),
        Comment = this.Comment.GetString((IDataReader) reader, true),
        AssignedTo = this.AssignedTo.GetGuid((IDataReader) reader, false),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader),
        LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false),
        Revision = this.Revision.GetInt32((IDataReader) reader),
        LastResolutionStateId = this.LastResolutionStateId.GetInt32((IDataReader) reader, 0),
        LastResetToActive = this.LastResetToActive.GetDateTime((IDataReader) reader)
      };
    }
  }
}
