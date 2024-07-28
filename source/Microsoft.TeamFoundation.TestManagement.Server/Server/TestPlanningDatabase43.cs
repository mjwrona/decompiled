// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase43
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase43 : TestPlanningDatabase42
  {
    internal override IEnumerable<SuiteEntry> GetSuiteEntries(Guid projectId, int suiteId)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "SuiteEntryDatabase.GetSuiteEntries"))
      {
        this.PrepareStoredProcedure("prc_GetSuiteEntriesForSuite");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindInt("@suiteId", suiteId);
        return this.GetSuiteEntriesFromDb(suiteId);
      }
    }

    internal override IEnumerable<SuiteEntry> ReorderSuiteEntries(
      Guid projectId,
      int suiteId,
      IEnumerable<SuiteEntryUpdateModel> suiteEntries)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "SuiteEntryDatabase.ReorderSuiteEntries"))
      {
        this.PrepareStoredProcedure("prc_ReorderSuiteEntries");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindInt("@suiteId", suiteId);
        this.BindSuiteEntryOrderTypeTable2("@suiteEntries", suiteEntries);
        return this.GetSuiteEntriesFromDb(suiteId);
      }
    }

    internal TestPlanningDatabase43(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase43()
    {
    }

    protected class FetchTestPointRecord
    {
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder ChangeNumber = new SqlColumnBinder(nameof (ChangeNumber));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder Active = new SqlColumnBinder(nameof (Active));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder AssignedTo = new SqlColumnBinder(nameof (AssignedTo));
      private SqlColumnBinder LastTestRunId = new SqlColumnBinder(nameof (LastTestRunId));
      private SqlColumnBinder LastTestResultId = new SqlColumnBinder(nameof (LastTestResultId));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder ResultState = new SqlColumnBinder("LastResultState");
      private SqlColumnBinder ResultOutcome = new SqlColumnBinder("LastResultOutcome");
      private SqlColumnBinder LastResetToActive = new SqlColumnBinder(nameof (LastResetToActive));
      private SqlColumnBinder OutcomeMigrationDate = new SqlColumnBinder(nameof (OutcomeMigrationDate));

      internal virtual TestPointRecord bind(
        SqlDataReader reader,
        TestArtifactSource datasource,
        out DateTime outcomeMigrationDate,
        out int pointId)
      {
        outcomeMigrationDate = this.OutcomeMigrationDate.ColumnExists((IDataReader) reader) ? this.OutcomeMigrationDate.GetDateTime((IDataReader) reader) : new DateTime();
        TestPointRecord testPointRecord = new TestPointRecord();
        testPointRecord.TestPointId = pointId = this.PointId.GetInt32((IDataReader) reader);
        testPointRecord.TestPlanId = this.PlanId.GetInt32((IDataReader) reader);
        testPointRecord.Revision = this.ChangeNumber.GetInt32((IDataReader) reader);
        testPointRecord.State = this.State.GetByte((IDataReader) reader);
        testPointRecord.Enabled = this.Active.GetBoolean((IDataReader) reader);
        testPointRecord.ChangedDate = this.LastUpdated.GetDateTime((IDataReader) reader);
        testPointRecord.TestSuiteId = this.SuiteId.GetInt32((IDataReader) reader, 0, 0);
        testPointRecord.TestCaseId = this.TestCaseId.GetInt32((IDataReader) reader, 0, 0);
        testPointRecord.TestConfigurationId = this.ConfigurationId.GetInt32((IDataReader) reader, 0, 0);
        TesterInfo testerInfo;
        if (!(this.AssignedTo.GetGuid((IDataReader) reader) != Guid.Empty))
        {
          testerInfo = (TesterInfo) null;
        }
        else
        {
          testerInfo = new TesterInfo();
          testerInfo.Id = new Guid?(this.AssignedTo.GetGuid((IDataReader) reader));
        }
        testPointRecord.Tester = testerInfo;
        testPointRecord.TestRunId = new int?(this.LastTestRunId.GetInt32((IDataReader) reader, 0, 0));
        testPointRecord.TestResultId = new int?(this.LastTestResultId.GetInt32((IDataReader) reader, 0, 0));
        testPointRecord.DataSourceId = datasource;
        testPointRecord.IsDeleted = this.IsDeleted.ColumnExists((IDataReader) reader) && this.IsDeleted.GetBoolean((IDataReader) reader, false);
        testPointRecord.TestResultState = this.ResultState.ColumnExists((IDataReader) reader) ? this.ResultState.GetByte((IDataReader) reader, (byte) 0) : (byte) 0;
        testPointRecord.TestResultOutcome = this.ResultOutcome.ColumnExists((IDataReader) reader) ? this.ResultOutcome.GetByte((IDataReader) reader, (byte) 0) : (byte) 0;
        testPointRecord.LastResetToActive = this.LastResetToActive.ColumnExists((IDataReader) reader) ? this.LastResetToActive.GetDateTime((IDataReader) reader, new DateTime()) : new DateTime();
        return testPointRecord;
      }
    }

    protected class FetchTestPointHistoryRecord
    {
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder ChangeNumber = new SqlColumnBinder(nameof (ChangeNumber));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder Active = new SqlColumnBinder(nameof (Active));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder AssignedTo = new SqlColumnBinder(nameof (AssignedTo));
      private SqlColumnBinder LastTestRunId = new SqlColumnBinder(nameof (LastTestRunId));
      private SqlColumnBinder LastTestResultId = new SqlColumnBinder(nameof (LastTestResultId));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder WatermarkDate = new SqlColumnBinder(nameof (WatermarkDate));

      internal virtual TestPointHistoryRecord bind(
        SqlDataReader reader,
        TestArtifactSource datasource,
        out DateTime watermarkDate,
        out int pointId,
        out int changeNumber)
      {
        watermarkDate = this.WatermarkDate.ColumnExists((IDataReader) reader) ? this.WatermarkDate.GetDateTime((IDataReader) reader) : new DateTime();
        TestPointHistoryRecord pointHistoryRecord = new TestPointHistoryRecord();
        pointHistoryRecord.TestPointId = pointId = this.PointId.GetInt32((IDataReader) reader);
        pointHistoryRecord.TestPlanId = this.PlanId.GetInt32((IDataReader) reader);
        pointHistoryRecord.Revision = changeNumber = this.ChangeNumber.GetInt32((IDataReader) reader);
        pointHistoryRecord.State = this.State.GetByte((IDataReader) reader);
        pointHistoryRecord.Enabled = this.Active.GetBoolean((IDataReader) reader);
        pointHistoryRecord.ChangedDate = this.LastUpdated.GetDateTime((IDataReader) reader);
        pointHistoryRecord.TestSuiteId = this.SuiteId.GetInt32((IDataReader) reader, 0, 0);
        pointHistoryRecord.TestCaseId = this.TestCaseId.GetInt32((IDataReader) reader, 0, 0);
        pointHistoryRecord.TestConfigurationId = this.ConfigurationId.GetInt32((IDataReader) reader, 0, 0);
        TesterInfo testerInfo;
        if (!(this.AssignedTo.GetGuid((IDataReader) reader) != Guid.Empty))
        {
          testerInfo = (TesterInfo) null;
        }
        else
        {
          testerInfo = new TesterInfo();
          testerInfo.Id = new Guid?(this.AssignedTo.GetGuid((IDataReader) reader));
        }
        pointHistoryRecord.Tester = testerInfo;
        pointHistoryRecord.TestRunId = new int?(this.LastTestRunId.GetInt32((IDataReader) reader, 0, 0));
        pointHistoryRecord.TestResultId = new int?(this.LastTestResultId.GetInt32((IDataReader) reader, 0, 0));
        pointHistoryRecord.DataSourceId = datasource;
        pointHistoryRecord.TestResultOutcome = this.Outcome.ColumnExists((IDataReader) reader) ? this.Outcome.GetByte((IDataReader) reader, (byte) 0) : (byte) 0;
        pointHistoryRecord.IsDeleted = this.IsDeleted.ColumnExists((IDataReader) reader) && this.IsDeleted.GetBoolean((IDataReader) reader, false);
        return pointHistoryRecord;
      }
    }
  }
}
