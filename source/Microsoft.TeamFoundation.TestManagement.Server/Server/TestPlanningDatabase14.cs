// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase14
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
  public class TestPlanningDatabase14 : TestPlanningDatabase13
  {
    internal TestPlanningDatabase14(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase14()
    {
    }

    internal override TestPlanHubData FetchTestPlanHubData(
      Guid projectGuid,
      int planId,
      int suiteId,
      bool includeTestPoints)
    {
      TestPlanningDatabase.FetchTestPlanHubDataColumnsBase fetchTestPlanHubDataColumns = (TestPlanningDatabase.FetchTestPlanHubDataColumnsBase) new TestPlanningDatabase14.FetchTestPlanHubDataColumns();
      return this.FetchTestPlanHubData(projectGuid, planId, suiteId, includeTestPoints, fetchTestPlanHubDataColumns);
    }

    internal TestPlanHubData FetchTestPlanHubData(
      Guid projectGuid,
      int planId,
      int suiteId,
      bool includeTestPoints,
      TestPlanningDatabase.FetchTestPlanHubDataColumnsBase fetchTestPlanHubDataColumns)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestPlanDatabase.FetchTestPlanHubData"))
      {
        this.PrepareStoredProcedure("prc_FetchTestPlanHubData");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@planId", planId);
        this.BindInt("@suiteId", suiteId);
        this.BindBoolean("@includeTestPoints", includeTestPoints);
        SqlDataReader reader = this.ExecuteReader();
        return fetchTestPlanHubDataColumns.bind(reader, "prc_FetchTestPlanHubData", includeTestPoints);
      }
    }

    internal override List<int> GetSessionIdsOfTeam(
      string projectId,
      int period,
      Guid sessionOwner,
      List<int> sourceList,
      List<int> stateList,
      bool isTeamFieldAreaPath,
      List<string> teamFieldsOfTeam)
    {
      List<int> sessionIdsOfTeam = new List<int>();
      this.PrepareStoredProcedure("prc_QueryTestSessionIds");
      this.BindInt("@dataspaceId", this.GetDataspaceId(new Guid(projectId)));
      this.BindInt("@period", period);
      this.BindGuid("@sessionOwner", sessionOwner);
      this.BindIdTypeTable("@sourceTable", (IEnumerable<int>) sourceList);
      this.BindIdTypeTable("@stateTable", (IEnumerable<int>) stateList);
      this.BindBoolean("@isTeamFieldAreaPath", isTeamFieldAreaPath);
      this.BindNameTypeTable("@teamFieldTable", (IEnumerable<string>) teamFieldsOfTeam);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("SessionId");
      while (reader.Read())
        sessionIdsOfTeam.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
      return sessionIdsOfTeam;
    }

    private class FetchTestPlanHubDataColumns : TestPlanningDatabase.FetchTestPlanHubDataColumnsBase
    {
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder SuiteName = new SqlColumnBinder(nameof (SuiteName));
      private SqlColumnBinder SuiteParent = new SqlColumnBinder(nameof (SuiteParent));
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder LastResultOutcome = new SqlColumnBinder(nameof (LastResultOutcome));
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
        if (int32_1 <= 0)
          return (TestPlanHubData) null;
        testPlanHubData.TestPlan = new TestPlan()
        {
          PlanId = int32_1
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
          ServerTestSuite serverTestSuite = new ServerTestSuite()
          {
            Id = int32_2,
            Title = str,
            ParentId = int32_3
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
