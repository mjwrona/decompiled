// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase68
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase68 : TestManagementDatabase67
  {
    public override long UpdateLogStoreProjectSummary(
      Guid projectId,
      long blobSize,
      bool isinitialize = false)
    {
      this.RequestContext.TraceEnter(0, "TestManagement", "LogStorage", nameof (UpdateLogStoreProjectSummary));
      this.PrepareStoredProcedure("prc_UpdateLogStoreProjectSummary");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindLong("@blobSize", blobSize);
      this.BindBoolean("@isInitialize", isinitialize);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TotalBlobSize");
      if (reader.Read())
        return sqlColumnBinder.GetInt64((IDataReader) reader);
      this.RequestContext.TraceLeave(0, "TestManagement", "LogStorage", nameof (UpdateLogStoreProjectSummary));
      return 0;
    }

    internal TestManagementDatabase68(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase68()
    {
    }

    public override void UpdateTestRunExtension(
      Guid projectId,
      IEnumerable<Tuple<int, TestExtensionField>> extensionFields)
    {
      this.RequestContext.TraceEnter(0, "TestManagement", "LogStorage", nameof (UpdateTestRunExtension));
      this.PrepareStoredProcedure("prc_UpdateTestRunExt");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindTestExtensionFieldValuesTypeTable("@additionalRunFields", (IEnumerable<Tuple<int, int, TestExtensionField>>) this.GetExtensionFieldsMap(extensionFields));
      this.ExecuteNonQuery();
      this.RequestContext.TraceLeave(0, "TestManagement", "LogStorage", nameof (UpdateTestRunExtension));
    }

    public override long UpdateLogStoreContentSizeByBuild(
      Guid projectId,
      int buildId,
      int fieldId,
      int stateFieldId,
      int stateFieldValue,
      int newStateFieldValue,
      bool isDeleted = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", nameof (UpdateLogStoreContentSizeByBuild));
        this.PrepareStoredProcedure("prc_UpdateLogStoreContentSizeByBuild");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@buildId", buildId);
        this.BindInt("@fieldId", fieldId);
        this.BindInt("@stateFieldId", stateFieldId);
        this.BindInt("@stateFieldValue", stateFieldValue);
        this.BindInt("@newStateFieldValue", newStateFieldValue);
        this.BindBoolean("@isDeleted", isDeleted);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TotalBlobSize");
        if (reader.Read())
          return sqlColumnBinder.GetInt64((IDataReader) reader);
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", nameof (UpdateLogStoreContentSizeByBuild));
      }
      return 0;
    }

    public override long UpdateLogStoreContentSizeByRuns(
      Guid projectId,
      List<int> runIds,
      int fieldId,
      int stateFieldId,
      int stateFieldValue,
      int newStateFieldValue,
      bool isDeleted = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", nameof (UpdateLogStoreContentSizeByRuns));
        this.PrepareStoredProcedure("prc_UpdateLogStoreContentSizeByRuns");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindIdTypeTable("@runIds", (IEnumerable<int>) runIds);
        this.BindInt("@fieldId", fieldId);
        this.BindInt("@stateFieldId", stateFieldId);
        this.BindInt("@stateFieldValue", stateFieldValue);
        this.BindInt("@newStateFieldValue", newStateFieldValue);
        this.BindBoolean("@isDeleted", isDeleted);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TotalBlobSize");
        if (reader.Read())
          return sqlColumnBinder.GetInt64((IDataReader) reader);
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", nameof (UpdateLogStoreContentSizeByRuns));
      }
      return 0;
    }

    private List<Tuple<int, int, TestExtensionField>> GetExtensionFieldsMap(
      IEnumerable<Tuple<int, TestExtensionField>> extensionFields)
    {
      List<Tuple<int, int, TestExtensionField>> extensionFieldsMap = new List<Tuple<int, int, TestExtensionField>>();
      foreach (Tuple<int, TestExtensionField> extensionField in extensionFields)
        extensionFieldsMap.Add(new Tuple<int, int, TestExtensionField>(extensionField.Item1, 0, extensionField.Item2));
      return extensionFieldsMap;
    }

    public override RunSummaryAndResultInsightsInPipeline GetPipelineTestMetrics(
      GuidAndString projectId,
      PipelineReference pipelineReference,
      bool resultSummaryFlag,
      bool resultsAnalysisFlag,
      bool runSummaryFlag,
      bool groupByNode = false)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryPipelineTestMetrics");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindInt("@pipelineId", pipelineReference.PipelineId);
      this.BindString("@stageName", pipelineReference.StageReference?.StageName, 256, true, SqlDbType.NVarChar);
      this.BindString("@phaseName", pipelineReference.PhaseReference?.PhaseName, 256, true, SqlDbType.NVarChar);
      this.BindString("@jobName", pipelineReference.JobReference?.JobName, 256, true, SqlDbType.NVarChar);
      this.BindBoolean("@returnResultSummary", resultSummaryFlag);
      this.BindBoolean("@returnRunSummary", runSummaryFlag);
      this.BindBoolean("@returnResultAnalysis", resultsAnalysisFlag);
      this.BindBoolean("@groupByNode", groupByNode);
      RunSummaryAndResultInsightsInPipeline pipelineTestMetrics = new RunSummaryAndResultInsightsInPipeline()
      {
        RunSummaryByCounts = new List<RunSummaryByCountInPipeline>(),
        NoConfigRunSummaryByCounts = new List<RunSummaryByCountInPipeline>(),
        RunSummaryByState = new List<RunSummaryByStateInPipeline>(),
        CurrentRunSummaryByOutcome = new List<RunSummaryByOutcomeInPipeline>(),
        PreviousRunSummaryByOutcome = new List<RunSummaryByOutcomeInPipeline>(),
        ResultInsights = new List<ResultInsightsInPipeline>()
      };
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.HasRows)
        return (RunSummaryAndResultInsightsInPipeline) null;
      if (runSummaryFlag)
      {
        TestManagementDatabase68.FetchTestRunsByCountColumns runsByCountColumns1 = new TestManagementDatabase68.FetchTestRunsByCountColumns(pipelineReference.PipelineId);
        while (reader.Read())
        {
          RunSummaryByCountInPipeline byCountInPipeline = runsByCountColumns1.bind(reader);
          pipelineTestMetrics.RunSummaryByCounts.Add(byCountInPipeline);
        }
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("TestResult.prc_QueryPipelineTestMetrics");
        TestManagementDatabase68.FetchTestRunsByCountColumns runsByCountColumns2 = new TestManagementDatabase68.FetchTestRunsByCountColumns(pipelineReference.PipelineId);
        while (reader.Read())
        {
          RunSummaryByCountInPipeline byCountInPipeline = runsByCountColumns2.bind(reader);
          pipelineTestMetrics.NoConfigRunSummaryByCounts.Add(byCountInPipeline);
        }
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("TestResult.prc_QueryPipelineTestMetrics");
        TestManagementDatabase68.FetchTestRunsByStateInPipelineColumns inPipelineColumns = new TestManagementDatabase68.FetchTestRunsByStateInPipelineColumns(pipelineReference.PipelineId);
        while (reader.Read())
        {
          RunSummaryByStateInPipeline byStateInPipeline = inPipelineColumns.bind(reader);
          pipelineTestMetrics.RunSummaryByState.Add(byStateInPipeline);
        }
      }
      if (runSummaryFlag && !reader.NextResult())
        throw new UnexpectedDatabaseResultException("TestResult.prc_QueryPipelineTestMetrics");
      if (runSummaryFlag | resultSummaryFlag | resultsAnalysisFlag)
      {
        TestManagementDatabase68.FetchTestRunSummaryByOutcomeColumns byOutcomeColumns = new TestManagementDatabase68.FetchTestRunSummaryByOutcomeColumns(pipelineReference.PipelineId);
        while (reader.Read())
        {
          RunSummaryByOutcomeInPipeline outcomeInPipeline = byOutcomeColumns.bind(reader);
          pipelineTestMetrics.CurrentRunSummaryByOutcome.Add(outcomeInPipeline);
        }
      }
      if (resultsAnalysisFlag)
      {
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("TestResult.prc_QueryPipelineTestMetrics");
        TestManagementDatabase68.FetchTestRunSummaryByOutcomeColumns byOutcomeColumns = new TestManagementDatabase68.FetchTestRunSummaryByOutcomeColumns(pipelineReference.PipelineId);
        while (reader.Read())
        {
          RunSummaryByOutcomeInPipeline outcomeInPipeline = byOutcomeColumns.bind(reader);
          pipelineTestMetrics.PreviousRunSummaryByOutcome.Add(outcomeInPipeline);
        }
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("TestResult.prc_QueryPipelineTestMetrics");
        TestManagementDatabase68.FetchResultInsightsColumns resultInsightsColumns = new TestManagementDatabase68.FetchResultInsightsColumns(pipelineReference.PipelineId);
        while (reader.Read())
        {
          ResultInsightsInPipeline insightsInPipeline = resultInsightsColumns.bind(reader);
          pipelineTestMetrics.ResultInsights.Add(insightsInPipeline);
        }
      }
      return pipelineTestMetrics;
    }

    protected class FetchResultInsightsColumns
    {
      private int PipelineId;
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder StageName = new SqlColumnBinder(nameof (StageName));
      private SqlColumnBinder PhaseName = new SqlColumnBinder(nameof (PhaseName));
      private SqlColumnBinder JobName = new SqlColumnBinder(nameof (JobName));
      private SqlColumnBinder NewFailures = new SqlColumnBinder(nameof (NewFailures));
      private SqlColumnBinder ExistingFailures = new SqlColumnBinder(nameof (ExistingFailures));
      private SqlColumnBinder FixedTests = new SqlColumnBinder(nameof (FixedTests));
      private SqlColumnBinder NewFailedResults = new SqlColumnBinder(nameof (NewFailedResults));
      private SqlColumnBinder ExistingFailedResults = new SqlColumnBinder(nameof (ExistingFailedResults));
      private SqlColumnBinder FixedTestResults = new SqlColumnBinder(nameof (FixedTestResults));
      private SqlColumnBinder PrevPipelineRefId = new SqlColumnBinder(nameof (PrevPipelineRefId));

      public FetchResultInsightsColumns(int pipelineId) => this.PipelineId = pipelineId;

      internal ResultInsightsInPipeline bind(SqlDataReader reader)
      {
        ResultInsightsInPipeline insightsInPipeline = new ResultInsightsInPipeline()
        {
          PipelineReference = new PipelineReference()
          {
            PipelineId = this.PipelineId,
            StageReference = new StageReference(),
            PhaseReference = new PhaseReference(),
            JobReference = new JobReference()
          },
          TestRunId = this.TestRunId.GetInt32((IDataReader) reader)
        };
        insightsInPipeline.PipelineReference.StageReference.StageName = this.StageName.GetString((IDataReader) reader, true);
        insightsInPipeline.PipelineReference.PhaseReference.PhaseName = this.PhaseName.GetString((IDataReader) reader, true);
        insightsInPipeline.PipelineReference.JobReference.JobName = this.JobName.GetString((IDataReader) reader, true);
        insightsInPipeline.NewFailures = this.NewFailures.GetInt32((IDataReader) reader);
        insightsInPipeline.ExistingFailures = this.ExistingFailures.GetInt32((IDataReader) reader);
        insightsInPipeline.FixedTests = this.FixedTests.GetInt32((IDataReader) reader);
        insightsInPipeline.NewFailedResults = this.NewFailedResults.GetString((IDataReader) reader, true);
        insightsInPipeline.ExistingFailedResults = this.ExistingFailedResults.GetString((IDataReader) reader, true);
        insightsInPipeline.FixedTestResults = this.FixedTestResults.GetString((IDataReader) reader, true);
        insightsInPipeline.PrevPipelineRefId = this.PrevPipelineRefId.GetInt32((IDataReader) reader);
        return insightsInPipeline;
      }
    }

    protected class FetchTestRunSummaryByOutcomeColumns
    {
      private int PipelineId;
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder StageName = new SqlColumnBinder(nameof (StageName));
      private SqlColumnBinder PhaseName = new SqlColumnBinder(nameof (PhaseName));
      private SqlColumnBinder JobName = new SqlColumnBinder(nameof (JobName));
      private SqlColumnBinder TestRunContextId = new SqlColumnBinder(nameof (TestRunContextId));
      private SqlColumnBinder TestRunCompletedDate = new SqlColumnBinder(nameof (TestRunCompletedDate));
      private SqlColumnBinder TestOutcome = new SqlColumnBinder(nameof (TestOutcome));
      private SqlColumnBinder ResultCount = new SqlColumnBinder(nameof (ResultCount));
      private SqlColumnBinder ResultDuration = new SqlColumnBinder(nameof (ResultDuration));
      private SqlColumnBinder RunDuration = new SqlColumnBinder(nameof (RunDuration));
      private SqlColumnBinder ResultMetadata = new SqlColumnBinder(nameof (ResultMetadata));
      private SqlColumnBinder RunState = new SqlColumnBinder("TestRunState");

      public FetchTestRunSummaryByOutcomeColumns(int pipelineId) => this.PipelineId = pipelineId;

      internal RunSummaryByOutcomeInPipeline bind(SqlDataReader reader)
      {
        RunSummaryByOutcomeInPipeline outcomeInPipeline = new RunSummaryByOutcomeInPipeline()
        {
          PipelineReference = new PipelineReference()
          {
            PipelineId = this.PipelineId,
            StageReference = new StageReference(),
            PhaseReference = new PhaseReference(),
            JobReference = new JobReference()
          },
          TestRunId = this.TestRunId.GetInt32((IDataReader) reader)
        };
        outcomeInPipeline.PipelineReference.StageReference.StageName = this.StageName.GetString((IDataReader) reader, true);
        outcomeInPipeline.PipelineReference.PhaseReference.PhaseName = this.PhaseName.GetString((IDataReader) reader, true);
        outcomeInPipeline.PipelineReference.JobReference.JobName = this.JobName.GetString((IDataReader) reader, true);
        outcomeInPipeline.TestRunContextId = this.TestRunContextId.GetInt32((IDataReader) reader);
        outcomeInPipeline.RunCompletedDate = this.TestRunCompletedDate.GetDateTime((IDataReader) reader);
        outcomeInPipeline.TestOutcome = (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome) this.TestOutcome.GetByte((IDataReader) reader);
        outcomeInPipeline.ResultCount = this.ResultCount.GetInt32((IDataReader) reader);
        outcomeInPipeline.ResultDuration = Convert.ToInt64(this.ResultDuration.GetObject((IDataReader) reader));
        outcomeInPipeline.RunDuration = Convert.ToInt64(this.RunDuration.GetObject((IDataReader) reader));
        outcomeInPipeline.ResultMetadata = this.ResultMetadata.ColumnExists((IDataReader) reader) ? this.ResultMetadata.GetByte((IDataReader) reader) : (byte) 0;
        outcomeInPipeline.TestRunState = this.RunState.ColumnExists((IDataReader) reader) ? (Microsoft.TeamFoundation.TestManagement.Client.TestRunState) this.RunState.GetByte((IDataReader) reader) : Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Completed;
        return outcomeInPipeline;
      }
    }

    protected class FetchTestRunsByStateInPipelineColumns
    {
      private int PipelineId;
      private SqlColumnBinder StageName = new SqlColumnBinder(nameof (StageName));
      private SqlColumnBinder PhaseName = new SqlColumnBinder(nameof (PhaseName));
      private SqlColumnBinder JobName = new SqlColumnBinder(nameof (JobName));
      private SqlColumnBinder RunState = new SqlColumnBinder("State");
      private SqlColumnBinder RunsCount = new SqlColumnBinder(nameof (RunsCount));

      public FetchTestRunsByStateInPipelineColumns(int pipelineId) => this.PipelineId = pipelineId;

      internal RunSummaryByStateInPipeline bind(SqlDataReader reader)
      {
        RunSummaryByStateInPipeline byStateInPipeline = new RunSummaryByStateInPipeline()
        {
          PipelineReference = new PipelineReference()
          {
            PipelineId = this.PipelineId,
            StageReference = new StageReference(),
            PhaseReference = new PhaseReference(),
            JobReference = new JobReference()
          }
        };
        byStateInPipeline.PipelineReference.StageReference.StageName = this.StageName.ColumnExists((IDataReader) reader) ? this.StageName.GetString((IDataReader) reader, true) : string.Empty;
        byStateInPipeline.PipelineReference.PhaseReference.PhaseName = this.PhaseName.ColumnExists((IDataReader) reader) ? this.PhaseName.GetString((IDataReader) reader, true) : string.Empty;
        byStateInPipeline.PipelineReference.JobReference.JobName = this.JobName.ColumnExists((IDataReader) reader) ? this.JobName.GetString((IDataReader) reader, true) : string.Empty;
        byStateInPipeline.RunState = (Microsoft.TeamFoundation.TestManagement.Client.TestRunState) this.RunState.GetByte((IDataReader) reader);
        byStateInPipeline.RunsCount = this.RunsCount.GetInt32((IDataReader) reader);
        return byStateInPipeline;
      }
    }

    protected class FetchTestRunsByCountColumns
    {
      private int PipelineId;
      private SqlColumnBinder StageName = new SqlColumnBinder(nameof (StageName));
      private SqlColumnBinder PhaseName = new SqlColumnBinder(nameof (PhaseName));
      private SqlColumnBinder JobName = new SqlColumnBinder(nameof (JobName));
      private SqlColumnBinder RunsCount = new SqlColumnBinder(nameof (RunsCount));

      public FetchTestRunsByCountColumns(int pipelineId) => this.PipelineId = pipelineId;

      internal RunSummaryByCountInPipeline bind(SqlDataReader reader)
      {
        RunSummaryByCountInPipeline byCountInPipeline = new RunSummaryByCountInPipeline()
        {
          PipelineReference = new PipelineReference()
          {
            PipelineId = this.PipelineId,
            StageReference = new StageReference(),
            PhaseReference = new PhaseReference(),
            JobReference = new JobReference()
          }
        };
        byCountInPipeline.PipelineReference.StageReference.StageName = this.StageName.GetString((IDataReader) reader, true);
        byCountInPipeline.PipelineReference.PhaseReference.PhaseName = this.PhaseName.GetString((IDataReader) reader, true);
        byCountInPipeline.PipelineReference.JobReference.JobName = this.JobName.GetString((IDataReader) reader, true);
        byCountInPipeline.RunsCount = this.RunsCount.GetInt32((IDataReader) reader);
        return byCountInPipeline;
      }
    }
  }
}
