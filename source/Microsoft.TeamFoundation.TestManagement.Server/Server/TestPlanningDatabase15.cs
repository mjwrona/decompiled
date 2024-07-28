// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase15
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase15 : TestPlanningDatabase14
  {
    private static readonly SqlMetaData[] typ_SuiteEntryOrderTypeTable2 = new SqlMetaData[3]
    {
      new SqlMetaData("TestCaseId", SqlDbType.Int),
      new SqlMetaData("ChildSuiteId", SqlDbType.Int),
      new SqlMetaData("SequenceNumber", SqlDbType.Int)
    };

    internal override IEnumerable<SuiteEntry> ReorderSuiteEntries(
      Guid projectId,
      int suiteId,
      IEnumerable<SuiteEntryUpdateModel> suiteEntries)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "SuiteEntryDatabase.ReorderSuiteEntries"))
      {
        this.PrepareStoredProcedure("prc_ReorderSuiteEntries");
        this.BindInt("@suiteId", suiteId);
        this.BindSuiteEntryOrderTypeTable2("@suiteEntries", suiteEntries);
        return this.GetSuiteEntriesFromDb(suiteId);
      }
    }

    protected SqlParameter BindSuiteEntryOrderTypeTable2(
      string parameterName,
      IEnumerable<SuiteEntryUpdateModel> suiteEntries)
    {
      suiteEntries = suiteEntries ?? Enumerable.Empty<SuiteEntryUpdateModel>();
      return this.BindTable(parameterName, "typ_SuiteEntryOrderTypeTable2", this.BindSuiteEntryOrderTypeTableRows(suiteEntries));
    }

    private IEnumerable<SqlDataRecord> BindSuiteEntryOrderTypeTableRows(
      IEnumerable<SuiteEntryUpdateModel> suiteEntries)
    {
      foreach (SuiteEntryUpdateModel suiteEntry in suiteEntries)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase15.typ_SuiteEntryOrderTypeTable2);
        sqlDataRecord.SetInt32(0, suiteEntry.TestCaseId);
        sqlDataRecord.SetInt32(1, suiteEntry.ChildSuiteId);
        sqlDataRecord.SetInt32(2, suiteEntry.SequenceNumber);
        yield return sqlDataRecord;
      }
    }

    internal TestPlanningDatabase15(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase15()
    {
    }

    internal override TestPlanHubData FetchTestPlanHubData(
      Guid projectGuid,
      int planId,
      int suiteId,
      bool includeTestPoints)
    {
      TestPlanningDatabase.FetchTestPlanHubDataColumnsBase fetchTestPlanHubDataColumns = (TestPlanningDatabase.FetchTestPlanHubDataColumnsBase) new TestPlanningDatabase15.FetchTestPlanHubDataColumns();
      return this.FetchTestPlanHubData(projectGuid, planId, suiteId, includeTestPoints, fetchTestPlanHubDataColumns);
    }

    internal override TestPlanHubData FetchTestPlanHubData(
      Guid projectGuid,
      int planId,
      int suiteId,
      bool includeTestPoints,
      int configuration,
      Guid tester)
    {
      return this.FetchTestPlanHubData(projectGuid, planId, suiteId, includeTestPoints);
    }

    internal override bool? IsSuiteOrderMigratedForPlan(Guid projectGuid, int planId)
    {
      this.PrepareStoredProcedure("prc_QuerySuiteOrderMigratedForPlan");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@planId", planId);
      return (bool?) this.ExecuteScalar();
    }

    internal override bool CleanDeletedTestSuites(int waitDaysForCleanup)
    {
      this.PrepareStoredProcedure("prc_DeleteTestSuite");
      this.BindInt("@waitDaysForCleanup", waitDaysForCleanup);
      return (int) this.ExecuteScalar() == 0;
    }

    internal override UpdatedProperties DeleteSuiteEntries(
      Guid projectGuid,
      UpdatedProperties parentProps,
      List<TestSuiteEntry> entries,
      out (int, List<int>) planPointIdsMap)
    {
      try
      {
        planPointIdsMap = (-1, (List<int>) null);
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.DeleteSuiteEntries");
        this.PrepareStoredProcedure("prc_DeleteSuiteEntriesV2");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", parentProps.Id);
        this.BindInt("@parentSuiteRevision", parentProps.Revision);
        this.BindTestSuiteEntryTypeTable("@entriesTable", (IEnumerable<TestSuiteEntry>) entries);
        this.BindDateTime("@lastUpdated", parentProps.LastUpdated);
        this.BindGuid("@lastUpdatedBy", parentProps.LastUpdatedBy);
        this.ExecuteNonQuery();
        return parentProps;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.DeleteSuiteEntries");
      }
    }

    internal new class FetchTestPlanHubDataColumns : 
      TestPlanningDatabase.FetchTestPlanHubDataColumnsBase
    {
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder RootSuiteId = new SqlColumnBinder(nameof (RootSuiteId));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder SuiteName = new SqlColumnBinder(nameof (SuiteName));
      private SqlColumnBinder SuiteParent = new SqlColumnBinder(nameof (SuiteParent));
      private SqlColumnBinder SuiteType = new SqlColumnBinder(nameof (SuiteType));
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder LastResultOutcome = new SqlColumnBinder(nameof (LastResultOutcome));
      private SqlColumnBinder LastResultState = new SqlColumnBinder(nameof (LastResultState));
      private SqlColumnBinder PointState = new SqlColumnBinder("State");
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));

      internal override TestPlanHubData bind(
        SqlDataReader reader,
        string storedProcedure,
        bool includeTestPoints)
      {
        TestPlanHubData testPlanHubData = new TestPlanHubData();
        if (!reader.Read())
          return (TestPlanHubData) null;
        int int32_1 = this.PlanId.GetInt32((IDataReader) reader);
        int num1 = 0;
        if (int32_1 <= 0)
          return (TestPlanHubData) null;
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException(storedProcedure);
        if (reader.Read())
          num1 = this.RootSuiteId.GetInt32((IDataReader) reader);
        testPlanHubData.TestPlan = new TestPlan()
        {
          PlanId = int32_1,
          RootSuiteId = num1
        };
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException(storedProcedure);
        if (reader.Read())
          testPlanHubData.SelectedSuiteId = this.SuiteId.GetInt32((IDataReader) reader);
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException(storedProcedure);
        List<ServerTestSuite> serverTestSuiteList = new List<ServerTestSuite>();
        while (reader.Read())
        {
          int int32_2 = this.SuiteId.GetInt32((IDataReader) reader);
          string str = this.SuiteName.GetString((IDataReader) reader, true);
          int int32_3 = this.SuiteParent.GetInt32((IDataReader) reader);
          byte num2 = this.SuiteType.GetByte((IDataReader) reader);
          ServerTestSuite serverTestSuite = new ServerTestSuite()
          {
            Id = int32_2,
            Title = str,
            ParentId = int32_3,
            SuiteType = num2
          };
          serverTestSuiteList.Add(serverTestSuite);
        }
        testPlanHubData.TestSuites = serverTestSuiteList;
        if (!includeTestPoints || !reader.NextResult())
          return testPlanHubData;
        List<TestPoint> testPointList = new List<TestPoint>();
        while (reader.Read())
        {
          TestPoint testPoint = new TestPoint()
          {
            PointId = this.PointId.GetInt32((IDataReader) reader),
            LastResultOutcome = this.LastResultOutcome.GetByte((IDataReader) reader, (byte) 0),
            LastResultState = this.LastResultState.GetByte((IDataReader) reader, (byte) 0),
            State = this.PointState.GetByte((IDataReader) reader, (byte) 0),
            TestCaseId = this.TestCaseId.GetInt32((IDataReader) reader)
          };
          testPointList.Add(testPoint);
        }
        testPlanHubData.TestPoints = testPointList;
        return testPlanHubData;
      }
    }
  }
}
