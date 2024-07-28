// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase7
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
  public class TestPlanningDatabase7 : TestPlanningDatabase6
  {
    internal TestPlanningDatabase7(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase7()
    {
    }

    internal override List<TestPoint> QueryTestPoints(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestPlanDatabase.QueryTestPoints"))
        return this.QueryTestPoints("prc_QueryPoints2", whereClause, orderBy, displayNameInGroupList, false);
    }

    internal override List<TestPoint> QueryTestPointsWithLastResults(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestPlanDatabase.QueryTestPointsWithLastResult"))
        return this.QueryTestPoints("prc_QueryPointsWithLastResults", whereClause, orderBy, displayNameInGroupList, true);
    }

    internal List<TestPoint> QueryTestPoints(
      string procName,
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList,
      bool includeLastResultDetails)
    {
      this.PrepareDynamicProcedure(procName);
      this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@orderByClause", orderBy, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
      SqlDataReader reader = this.ExecuteReader();
      List<TestPoint> testPointList = new List<TestPoint>();
      TestPlanningDatabase7.QueryTestPointColumns testPointColumns = new TestPlanningDatabase7.QueryTestPointColumns();
      while (reader.Read())
        testPointList.Add(testPointColumns.bind(reader, includeLastResultDetails));
      return testPointList;
    }

    internal override List<TestPoint> FetchTestPointsWithLastResults(
      Guid projectGuid,
      int planId,
      IdAndRev[] idsToFetch,
      List<int> deletedIds)
    {
      return this.FetchTestPointsWithLastResults(projectGuid, planId, idsToFetch, deletedIds, true);
    }

    internal override List<TestPoint> FetchTestPoints(
      Guid projectGuid,
      int planId,
      IdAndRev[] idsToFetch,
      List<int> deletedIds)
    {
      return this.FetchTestPoints(projectGuid, planId, idsToFetch, deletedIds, true);
    }

    protected new class QueryTestPointColumns
    {
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      private SqlColumnBinder LastResultState = new SqlColumnBinder(nameof (LastResultState));
      private SqlColumnBinder LastResultOutcome = new SqlColumnBinder(nameof (LastResultOutcome));
      private SqlColumnBinder ConfigurationName = new SqlColumnBinder(nameof (ConfigurationName));
      private SqlColumnBinder AssignedTo = new SqlColumnBinder(nameof (AssignedTo));
      private SqlColumnBinder SuiteName = new SqlColumnBinder(nameof (SuiteName));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder LastRunBy = new SqlColumnBinder(nameof (LastRunBy));
      private SqlColumnBinder LastRunDuration = new SqlColumnBinder(nameof (LastRunDuration));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder SequenceNumber = new SqlColumnBinder(nameof (SequenceNumber));

      internal TestPoint bind(SqlDataReader reader, bool includeLastResultDetails = false)
      {
        TestPoint testPoint = new TestPoint();
        testPoint.PointId = this.PointId.GetInt32((IDataReader) reader);
        testPoint.State = this.State.GetByte((IDataReader) reader);
        testPoint.TestCaseId = this.TestCaseId.GetInt32((IDataReader) reader);
        testPoint.LastResultState = this.LastResultState.GetByte((IDataReader) reader, (byte) 0);
        testPoint.LastResultOutcome = this.LastResultOutcome.GetByte((IDataReader) reader, (byte) 0);
        testPoint.ConfigurationName = this.ConfigurationName.GetString((IDataReader) reader, true);
        testPoint.AssignedTo = this.AssignedTo.GetGuid((IDataReader) reader, false);
        testPoint.SuiteName = this.SuiteName.GetString((IDataReader) reader, true);
        testPoint.ConfigurationId = this.ConfigurationId.GetInt32((IDataReader) reader);
        testPoint.SequenceNumber = this.SequenceNumber.GetInt32((IDataReader) reader, -1, -1);
        if (includeLastResultDetails)
        {
          testPoint.LastRunBuildNumber = this.BuildNumber.GetString((IDataReader) reader, true);
          testPoint.LastResultDetails = new LastResultDetails()
          {
            Duration = this.LastRunDuration.GetInt64((IDataReader) reader, 0L),
            RunBy = this.LastRunBy.GetGuid((IDataReader) reader, true)
          };
        }
        return testPoint;
      }
    }
  }
}
