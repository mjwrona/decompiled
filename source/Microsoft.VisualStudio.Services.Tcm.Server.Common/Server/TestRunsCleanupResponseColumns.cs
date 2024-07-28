// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestRunsCleanupResponseColumns
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestRunsCleanupResponseColumns
  {
    private SqlColumnBinder RemainingRuns = new SqlColumnBinder(nameof (RemainingRuns));
    private SqlColumnBinder SumRemainingResults = new SqlColumnBinder(nameof (SumRemainingResults));
    private SqlColumnBinder MaxRemainingResults = new SqlColumnBinder(nameof (MaxRemainingResults));
    private SqlColumnBinder CleanedRunsCount = new SqlColumnBinder(nameof (CleanedRunsCount));
    private SqlColumnBinder CleanedResultsCount = new SqlColumnBinder(nameof (CleanedResultsCount));

    internal TestRunsCleanupResponse bind(SqlDataReader reader) => new TestRunsCleanupResponse()
    {
      RemainingRuns = this.RemainingRuns.GetInt32((IDataReader) reader),
      SumRemainingResults = this.SumRemainingResults.GetInt32((IDataReader) reader),
      MaxRemainingResults = this.MaxRemainingResults.GetInt32((IDataReader) reader),
      CleanedRunsCount = this.CleanedRunsCount.GetInt32((IDataReader) reader),
      CleanedResultsCount = this.CleanedResultsCount.ColumnExists((IDataReader) reader) ? this.CleanedResultsCount.GetInt32((IDataReader) reader) : 0
    };

    protected class QueryTestCaseResultColumns
    {
      private SqlColumnBinder AutomatedTestName = new SqlColumnBinder(nameof (AutomatedTestName));
      private SqlColumnBinder Duration = new SqlColumnBinder("duration");
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder DateStarted = new SqlColumnBinder(nameof (DateStarted));
      private SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));

      internal TestCaseResult bind(SqlDataReader reader)
      {
        TestCaseResult testCaseResult = new TestCaseResult();
        testCaseResult.AutomatedTestName = this.AutomatedTestName.GetString((IDataReader) reader, false);
        testCaseResult.Duration = (long) this.Duration.GetInt32((IDataReader) reader);
        testCaseResult.Outcome = this.Outcome.GetByte((IDataReader) reader);
        testCaseResult.DateStarted = this.DateStarted.GetDateTime((IDataReader) reader);
        testCaseResult.DateCompleted = this.DateCompleted.GetDateTime((IDataReader) reader);
        testCaseResult.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        return testCaseResult;
      }
    }
  }
}
