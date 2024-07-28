// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase42
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase42 : TestPlanningDatabase41_1
  {
    internal TestPlanningDatabase42(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase42()
    {
    }

    internal override Dictionary<int, List<string>> QueryAssociatedWorkItemsForSessions(
      Guid projectId,
      int[] sessionIds)
    {
      Dictionary<int, List<string>> dictionary = new Dictionary<int, List<string>>();
      this.PrepareStoredProcedure("prc_QueryAssociatedWorkItemsForSession");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindIdTypeTable("@sessionIdsTable", (IEnumerable<int>) sessionIds);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("WorkItemUri");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("SessionId");
      while (reader.Read())
      {
        int int32 = sqlColumnBinder2.GetInt32((IDataReader) reader);
        string str = sqlColumnBinder1.GetString((IDataReader) reader, false);
        List<string> stringList = (List<string>) null;
        if (!dictionary.TryGetValue(int32, out stringList))
        {
          stringList = new List<string>();
          dictionary.Add(int32, stringList);
        }
        stringList.Add(str);
      }
      return dictionary;
    }

    internal override List<TestSuiteRecord> QueryTestSuitesByChangedDate(
      int dataspaceId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource)
    {
      this.PrepareStoredProcedure("TestManagement.prc_QueryTestSuitesByChangedDate");
      this.BindInt("@dataspaceId", dataspaceId);
      if (DateTime.Compare(fromDate, SqlDateTime.MinValue.Value) < 0)
        fromDate = SqlDateTime.MinValue.Value;
      this.BindInt("@batchSize", batchSize);
      this.BindDateTime("@fromChangedDate", fromDate, true);
      SqlDataReader reader = this.ExecuteReader();
      List<TestSuiteRecord> testSuiteRecordList = new List<TestSuiteRecord>();
      TestPlanningDatabase42.FetchTestSuitesRecord testSuitesRecord = new TestPlanningDatabase42.FetchTestSuitesRecord();
      Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
      toDate = fromDate;
      while (reader.Read())
        testSuiteRecordList.Add(testSuitesRecord.bind(reader, dataspaceIdentifier, dataSource, out toDate));
      return testSuiteRecordList;
    }

    protected class FetchTestSuitesRecord
    {
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder ParentSuiteId = new SqlColumnBinder(nameof (ParentSuiteId));
      private SqlColumnBinder SuiteType = new SqlColumnBinder(nameof (SuiteType));
      private SqlColumnBinder RequirementId = new SqlColumnBinder(nameof (RequirementId));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder SuitePath = new SqlColumnBinder(nameof (SuitePath));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder SequenceNumber = new SqlColumnBinder(nameof (SequenceNumber));

      internal virtual TestSuiteRecord bind(
        SqlDataReader reader,
        Guid projectId,
        TestArtifactSource datasource,
        out DateTime lastUpdatedDate)
      {
        lastUpdatedDate = this.LastUpdated.GetDateTime((IDataReader) reader);
        return new TestSuiteRecord()
        {
          ProjectGuid = projectId,
          TestSuiteId = this.SuiteId.GetInt32((IDataReader) reader),
          TestPlanId = this.PlanId.GetInt32((IDataReader) reader),
          ParentSuiteId = this.ParentSuiteId.GetInt32((IDataReader) reader),
          SuiteType = this.SuiteType.GetByte((IDataReader) reader),
          RequirementId = this.RequirementId.GetInt32((IDataReader) reader),
          DataSourceId = datasource,
          SuitePath = this.SuitePath.ColumnExists((IDataReader) reader) ? Convert.ToBase64String(this.SuitePath.GetBytes((IDataReader) reader, false)) : (string) null,
          IsDeleted = this.IsDeleted.ColumnExists((IDataReader) reader) && this.IsDeleted.GetBoolean((IDataReader) reader),
          SequenceNumber = this.SequenceNumber.ColumnExists((IDataReader) reader) ? this.SequenceNumber.GetInt32((IDataReader) reader) : 0
        };
      }
    }
  }
}
