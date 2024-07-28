// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase91
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase91 : TestManagementDatabase90
  {
    internal override void SyncTestRunSummary(IEnumerable<TestRunSummary2> testRunSummary)
    {
      this.PrepareStoredProcedure("prc_SyncTestRunSummary");
      IEnumerable<Guid> projectIds = testRunSummary.Select<TestRunSummary2, Guid>((System.Func<TestRunSummary2, Guid>) (x => x.ProjectId));
      this.BindTestRunSummary3TypeTable("@testRunSummary", testRunSummary, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal override List<TestRunSummary2> FetchTestRunSummary(
      int waterMarkDataspaceId,
      int waterMarkTestRunId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestRunSummary");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkTestRunId", waterMarkTestRunId);
      this.BindInt("@batchSize", batchSize);
      List<TestRunSummary2> testRunSummary2List = new List<TestRunSummary2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase91.TestRunSummaryColumns runSummaryColumns = new TestManagementDatabase91.TestRunSummaryColumns();
      while (reader.Read())
        testRunSummary2List.Add(runSummaryColumns.Bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return testRunSummary2List;
    }

    internal TestManagementDatabase91(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase91()
    {
    }

    private new class TestRunSummaryColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestRunStatsId = new SqlColumnBinder(nameof (TestRunStatsId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestRunContextId = new SqlColumnBinder(nameof (TestRunContextId));
      private SqlColumnBinder TestRunCompletedDate = new SqlColumnBinder(nameof (TestRunCompletedDate));
      private SqlColumnBinder TestOutcome = new SqlColumnBinder(nameof (TestOutcome));
      private SqlColumnBinder ResultCount = new SqlColumnBinder(nameof (ResultCount));
      private SqlColumnBinder ResultDuration = new SqlColumnBinder(nameof (ResultDuration));
      private SqlColumnBinder RunDuration = new SqlColumnBinder(nameof (RunDuration));
      private SqlColumnBinder IsRerun = new SqlColumnBinder(nameof (IsRerun));

      internal TestRunSummary2 Bind(SqlDataReader reader, System.Func<int, Guid> GetDataspaceIdentifier)
      {
        TestRunSummary2 testRunSummary2 = new TestRunSummary2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        testRunSummary2.ProjectId = GetDataspaceIdentifier(int32);
        testRunSummary2.TestRunStatsId = this.TestRunStatsId.GetInt64((IDataReader) reader);
        testRunSummary2.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testRunSummary2.TestRunContextId = this.TestRunContextId.GetInt32((IDataReader) reader);
        testRunSummary2.TestRunCompletedDate = this.TestRunCompletedDate.GetDateTime((IDataReader) reader);
        testRunSummary2.TestOutcome = this.TestOutcome.GetByte((IDataReader) reader);
        testRunSummary2.ResultCount = this.ResultCount.GetInt32((IDataReader) reader);
        testRunSummary2.ResultDuration = this.ResultDuration.GetInt64((IDataReader) reader, 0L);
        testRunSummary2.RunDuration = this.RunDuration.GetInt64((IDataReader) reader);
        testRunSummary2.IsRerun = this.IsRerun.GetBoolean((IDataReader) reader);
        return testRunSummary2;
      }
    }
  }
}
