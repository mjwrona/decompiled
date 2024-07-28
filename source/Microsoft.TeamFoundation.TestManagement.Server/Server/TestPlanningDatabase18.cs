// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase18
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
  public class TestPlanningDatabase18 : TestPlanningDatabase17
  {
    internal TestPlanningDatabase18(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase18()
    {
    }

    internal override TestPlanHubData FetchTestPlanHubData(
      Guid projectGuid,
      int planId,
      int suiteId,
      bool includeTestPoints,
      int configuration,
      Guid tester)
    {
      TestPlanningDatabase18.FetchTestPlanHubDataColumns2 fetchTestPlanHubDataColumns = new TestPlanningDatabase18.FetchTestPlanHubDataColumns2();
      return this.FetchTestPlanHubData(projectGuid, planId, suiteId, includeTestPoints, configuration, tester, (TestPlanningDatabase.FetchTestPlanHubDataColumnsBase) fetchTestPlanHubDataColumns);
    }

    internal class FetchTestPlanHubDataColumns2 : 
      TestPlanningDatabase.FetchTestPlanHubDataColumnsBase
    {
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder RootSuiteId = new SqlColumnBinder(nameof (RootSuiteId));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder SuiteName = new SqlColumnBinder(nameof (SuiteName));
      private SqlColumnBinder SuiteParent = new SqlColumnBinder(nameof (SuiteParent));
      private SqlColumnBinder SuiteType = new SqlColumnBinder(nameof (SuiteType));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
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
          int int32_4 = this.Revision.GetInt32((IDataReader) reader);
          ServerTestSuite serverTestSuite = new ServerTestSuite()
          {
            Id = int32_2,
            Title = str,
            ParentId = int32_3,
            SuiteType = num2,
            Revision = int32_4
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
