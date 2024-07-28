// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase61
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase61 : TestManagementDatabase60
  {
    internal TestManagementDatabase61(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase61()
    {
    }

    public override RunSummaryAndInsights QueryTestRunSummaryAndInsightsForRelease(
      GuidAndString projectId,
      string sourceWorkflow,
      ReleaseReference releaseRef,
      bool returnSummary,
      bool returnFailureDetails,
      string categoryName,
      out int runsCount,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("prc_QueryTestRunSummaryAndInsightsForRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, false, SqlDbType.NVarChar);
      this.BindInt("@releaseId", releaseRef != null ? releaseRef.ReleaseId : 0);
      this.BindInt("@releaseEnvId", releaseRef != null ? releaseRef.ReleaseEnvId : 0);
      this.BindInt("@attempt", releaseRef != null ? releaseRef.Attempt : 0);
      this.BindBoolean("@returnSummary", returnSummary);
      this.BindBoolean("@returnFailureDetails", returnFailureDetails);
      this.BindString("@categoryName", categoryName, 128, true, SqlDbType.NVarChar);
      this.BindInt("@runIdThreshold", runIdThreshold);
      SqlDataReader reader = this.ExecuteReader();
      RunSummaryAndInsights runSummaryAndInsights = new RunSummaryAndInsights()
      {
        TestRunSummary = new RunSummary()
        {
          CurrentAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          PreviousAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          CurrentAggregatedRunsByState = (IList<RunSummaryByState>) new List<RunSummaryByState>(),
          CurrentAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>()
        },
        TestResultInsights = new List<ResultInsights>()
      };
      runsCount = 0;
      bool flag = true;
      if (reader.Read())
        flag = new SqlColumnBinder("TestRunExists").GetBoolean((IDataReader) reader);
      if (!flag)
        return runSummaryAndInsights;
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      if (reader.Read())
      {
        int totalRunsCount;
        int noConfigRunsCount;
        new TestManagementDatabase58.FetchTestRunsCount().bind(reader, out totalRunsCount, out noConfigRunsCount);
        runSummaryAndInsights.TestRunSummary.TotalRunsCount = totalRunsCount;
        runSummaryAndInsights.TestRunSummary.NoConfigRunsCount = noConfigRunsCount;
      }
      TestManagementDatabase36.FetchTestRunsByStateColumns runsByStateColumns = new TestManagementDatabase36.FetchTestRunsByStateColumns();
      if (reader.NextResult())
      {
        while (reader.Read())
        {
          RunSummaryByState runSummaryByState = runsByStateColumns.bind(reader);
          runSummaryAndInsights.TestRunSummary.CurrentAggregatedRunsByState.Add(runSummaryByState);
          runsCount += runSummaryByState.RunsCount;
        }
      }
      if (runsCount > 0)
      {
        if (returnSummary)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForRelease");
          TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
          while (reader.Read())
            runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          if (reader.NextResult())
          {
            while (reader.Read())
              runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          }
        }
        if (returnFailureDetails)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForRelease");
          TestManagementDatabase2.FetchTestFailureDetailsColumns failureDetailsColumns = new TestManagementDatabase2.FetchTestFailureDetailsColumns();
          while (reader.Read())
            runSummaryAndInsights.TestResultInsights.Add(failureDetailsColumns.bind(reader));
        }
        TestManagementDatabase19.PopulateAggregateDataByCategory(categoryName, reader, runSummaryAndInsights, "prc_QueryTestRunSummaryAndInsightsForRelease");
      }
      return runSummaryAndInsights;
    }

    public override RunSummaryAndInsights QueryTestRunSummaryAndInsightsForBuild(
      GuidAndString projectId,
      string sourceWorkflow,
      BuildConfiguration buildRef,
      bool returnSummary,
      bool returnFailureDetails,
      string categoryName,
      out int runsCount,
      out bool isBuildOld,
      int rundIdThreshold = 0)
    {
      this.PrepareStoredProcedure("prc_QueryTestRunSummaryAndInsightsForBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, false, SqlDbType.NVarChar);
      this.BindInt("@buildId", buildRef != null ? buildRef.BuildId : 0);
      this.BindString("@buildUri", buildRef?.BuildUri, 256, true, SqlDbType.NVarChar);
      this.BindBoolean("@returnSummary", returnSummary);
      this.BindBoolean("@returnFailureDetails", returnFailureDetails);
      this.BindString("@categoryName", categoryName, 128, true, SqlDbType.NVarChar);
      this.BindInt("@runIdThreshold", rundIdThreshold);
      SqlDataReader reader1 = this.ExecuteReader();
      bool flag1 = true;
      RunSummaryAndInsights runSummaryAndInsights = new RunSummaryAndInsights()
      {
        TestRunSummary = new RunSummary()
        {
          CurrentAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          PreviousAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          CurrentAggregatedRunsByState = (IList<RunSummaryByState>) new List<RunSummaryByState>(),
          CurrentAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>()
        },
        TestResultInsights = new List<ResultInsights>()
      };
      isBuildOld = false;
      runsCount = 0;
      SqlColumnBinder sqlColumnBinder;
      if (reader1.Read())
      {
        sqlColumnBinder = new SqlColumnBinder("IsRunsAvailable");
        flag1 = sqlColumnBinder.GetBoolean((IDataReader) reader1);
      }
      if (!flag1)
        return runSummaryAndInsights;
      if (!reader1.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      if (reader1.Read())
      {
        ref bool local = ref isBuildOld;
        sqlColumnBinder = new SqlColumnBinder("IsBuildOld");
        int num = sqlColumnBinder.GetBoolean((IDataReader) reader1) ? 1 : 0;
        local = num != 0;
      }
      if (!reader1.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      bool flag2 = true;
      if (reader1.Read())
      {
        sqlColumnBinder = new SqlColumnBinder("TestRunExists");
        flag2 = sqlColumnBinder.GetBoolean((IDataReader) reader1);
      }
      if (!flag2)
        return runSummaryAndInsights;
      if (!reader1.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      TestManagementDatabase58.FetchTestRunsCount fetchTestRunsCount = new TestManagementDatabase58.FetchTestRunsCount();
      reader1.Read();
      SqlDataReader reader2 = reader1;
      int num1;
      ref int local1 = ref num1;
      int num2;
      ref int local2 = ref num2;
      fetchTestRunsCount.bind(reader2, out local1, out local2);
      runSummaryAndInsights.TestRunSummary.TotalRunsCount = num1;
      runSummaryAndInsights.TestRunSummary.NoConfigRunsCount = num2;
      if (!reader1.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      TestManagementDatabase36.FetchTestRunsByStateColumns runsByStateColumns = new TestManagementDatabase36.FetchTestRunsByStateColumns();
      while (reader1.Read())
      {
        RunSummaryByState runSummaryByState = runsByStateColumns.bind(reader1);
        runSummaryAndInsights.TestRunSummary.CurrentAggregatedRunsByState.Add(runSummaryByState);
        runsCount += runSummaryByState.RunsCount;
      }
      if (runsCount > 0)
      {
        if (returnSummary)
        {
          if (!reader1.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
          TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
          while (reader1.Read())
            runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader1));
          if (!isBuildOld)
          {
            if (!reader1.NextResult())
              throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
            while (reader1.Read())
              runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome.Add(runSummaryColumns.bind(reader1));
          }
        }
        if (returnFailureDetails && !isBuildOld)
        {
          if (!reader1.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
          TestManagementDatabase2.FetchTestFailureDetailsColumns failureDetailsColumns = new TestManagementDatabase2.FetchTestFailureDetailsColumns();
          while (reader1.Read())
            runSummaryAndInsights.TestResultInsights.Add(failureDetailsColumns.bind(reader1));
        }
        TestManagementDatabase19.PopulateAggregateDataByCategory(categoryName, reader1, runSummaryAndInsights, "prc_QueryTestRunSummaryAndInsightsForBuild");
      }
      return runSummaryAndInsights;
    }
  }
}
