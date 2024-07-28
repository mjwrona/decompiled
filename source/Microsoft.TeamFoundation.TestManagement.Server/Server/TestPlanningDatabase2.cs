// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase2
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
  public class TestPlanningDatabase2 : TestPlanningDatabase
  {
    internal TestPlanningDatabase2(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase2()
    {
    }

    internal override List<TestPoint> FetchTestPointsWithLastResults(
      Guid projectGuid,
      int planId,
      IdAndRev[] idsToFetch,
      List<int> deletedIds)
    {
      return this.FetchTestPointsWithLastResults(projectGuid, planId, idsToFetch, deletedIds, false);
    }

    protected List<TestPoint> FetchTestPointsWithLastResults(
      Guid projectGuid,
      int planId,
      IdAndRev[] idsToFetch,
      List<int> deletedIds,
      bool includeSuiteName)
    {
      this.PrepareStoredProcedure("TestManagement.prc_FetchTestPointsWithLastResults");
      this.BindIdAndRevTypeTable("@idsTable", (IEnumerable<IdAndRev>) idsToFetch);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@planId", planId);
      List<TestPoint> testPointList = new List<TestPoint>(idsToFetch.Length);
      TestPlanningDatabase2.FetchTestPointsWithLastResultDetailsColumns resultDetailsColumns = new TestPlanningDatabase2.FetchTestPointsWithLastResultDetailsColumns();
      SqlDataReader reader = this.ExecuteReader();
      while (reader.Read())
        testPointList.Add(resultDetailsColumns.bind(reader, includeSuiteName));
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("TestManagement.prc_FetchTestPointsWithLastResults");
      new TestPlanningDatabase.IdsPropertyColumns().bind(reader, deletedIds);
      return testPointList;
    }

    private class FetchTestPointsWithLastResultDetailsColumns
    {
      private SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
      private SqlColumnBinder DateStarted = new SqlColumnBinder(nameof (DateStarted));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder RunBy = new SqlColumnBinder(nameof (RunBy));
      private SqlColumnBinder Duration = new SqlColumnBinder(nameof (Duration));
      private SqlColumnBinder SuiteName = new SqlColumnBinder(nameof (SuiteName));

      internal TestPoint bind(SqlDataReader reader, bool includeSuiteName)
      {
        TestPoint testPoint = new TestPlanningDatabase.FetchTestPointsColumns().bind(reader);
        testPoint.LastRunBuildNumber = this.BuildNumber.GetString((IDataReader) reader, true);
        DateTime dateTime = this.DateCompleted.GetDateTime((IDataReader) reader, new DateTime());
        Guid guid = this.RunBy.GetGuid((IDataReader) reader, true);
        DateTime startDate = this.DateStarted.ColumnExists((IDataReader) reader) ? this.DateStarted.GetDateTime((IDataReader) reader, new DateTime()) : new DateTime();
        long duration = this.Duration.ColumnExists((IDataReader) reader) ? this.Duration.GetInt64((IDataReader) reader, 0L) : TestPlanningDatabase.GetDurationFromStartAndCompleteDates(startDate, dateTime);
        testPoint.LastResultDetails = new LastResultDetails(dateTime, duration, guid);
        if (includeSuiteName)
          testPoint.SuiteName = this.SuiteName.GetString((IDataReader) reader, true);
        return testPoint;
      }
    }
  }
}
