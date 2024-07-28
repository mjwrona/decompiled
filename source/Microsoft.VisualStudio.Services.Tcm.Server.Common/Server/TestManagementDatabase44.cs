// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase44
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
  public class TestManagementDatabase44 : TestManagementDatabase43
  {
    public override void DeleteTestBuild(
      Guid projectId,
      string[] buildUris,
      Guid lastUpdatedBy,
      bool deleteOnlyAutomatedRuns = false,
      bool isTcmService = false)
    {
      this.PrepareStoredProcedure("TestResult.prc_DeleteTestBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindNameTypeTable("@buildUris", (IEnumerable<string>) buildUris);
      this.BindGuid("@lastUpdatedBy", lastUpdatedBy);
      this.BindBoolean("@deleteOnlyAutomatedRuns", deleteOnlyAutomatedRuns);
      this.BindBoolean("@isTcmService", isTcmService);
      this.ExecuteNonQuery();
    }

    public override BuildConfiguration GetBuildConfigurationIdFromFlavorAndPlatform(
      Guid projectId,
      int buildId,
      string platform,
      string flavor)
    {
      BuildConfiguration flavorAndPlatform = new BuildConfiguration();
      this.PrepareStoredProcedure("prc_QueryBuildConfigurationFromFlavorAndPlatform");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@buildId", buildId);
      this.BindString("@buildPlatform", platform, 256, false, SqlDbType.NVarChar);
      this.BindString("@buildFlavor", flavor, 256, false, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      if (reader.Read())
      {
        int dataspaceId;
        flavorAndPlatform = new TestManagementDatabase44.QueryBuildConfigurationsColumns().bind(reader, out dataspaceId);
        projectId = this.GetDataspaceIdentifier(dataspaceId);
      }
      return flavorAndPlatform;
    }

    protected override void ReadCoverage(
      Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags,
      string storedProcedure,
      SqlDataReader reader,
      SortedDictionary<int, Coverage> coverageById)
    {
      if ((flags & Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags.Modules) == (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) 0)
        return;
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException(storedProcedure);
      TestManagementDatabase44.ModuleCoverageColumns moduleCoverageColumns = new TestManagementDatabase44.ModuleCoverageColumns();
      IList<int> values = (IList<int>) new List<int>();
      while (reader.Read())
      {
        ModuleCoverage moduleCoverage = moduleCoverageColumns.Bind(reader, flags);
        values.Add(moduleCoverage.CoverageId);
        if (coverageById.ContainsKey(moduleCoverage.CoverageId))
          coverageById[moduleCoverage.CoverageId].Modules.Add(moduleCoverage);
        else
          throw new KeyNotFoundException("Coverage ids available: " + string.Join<int>(", ", (IEnumerable<int>) coverageById.Keys) + ".\nCoverage ids processed so far: " + string.Join<int>(", ", (IEnumerable<int>) values) + ".");
      }
      if ((flags & Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags.Functions) == (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) 0)
        return;
      SortedDictionary<int, SortedDictionary<int, ModuleCoverage>> sortedDictionary1 = new SortedDictionary<int, SortedDictionary<int, ModuleCoverage>>();
      foreach (Coverage coverage in coverageById.Values)
      {
        SortedDictionary<int, ModuleCoverage> sortedDictionary2 = new SortedDictionary<int, ModuleCoverage>();
        foreach (ModuleCoverage module in coverage.Modules)
          sortedDictionary2.Add(module.ModuleId, module);
        sortedDictionary1.Add(coverage.Id, sortedDictionary2);
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException(storedProcedure);
      TestManagementDatabase.FunctionCoverageColumns functionCoverageColumns = new TestManagementDatabase.FunctionCoverageColumns();
      while (reader.Read())
      {
        FunctionCoverage functionCoverage = functionCoverageColumns.Bind(reader);
        sortedDictionary1[functionCoverage.CoverageId][functionCoverage.ModuleId].Functions.Add(functionCoverage);
      }
    }

    public override void UpdateBuildCoverage(
      int buildConfigurationId,
      Coverage coverage,
      int coverageChangeId,
      Guid projectId)
    {
      this.PrepareStoredProcedure("prc_UpdateBuildCoverage");
      this.BindInt("@buildConfigurationId", buildConfigurationId);
      this.BindByte("@state", coverage.State);
      this.BindInt("@coverageChangeId", coverageChangeId);
      this.BindStringPreserveNull("@errorLog", coverage.LastError, int.MaxValue, SqlDbType.NVarChar);
      this.BindModuleCoverageTypeTable1("@moduleInfo", (IEnumerable<ModuleCoverage>) coverage.Modules);
      this.BindFunctionCoverageTypeTable("@functionInfo", (IEnumerable<FunctionCoverage>) this.GetFunctionCoverages(coverage));
      this.ExecuteNonQuery();
    }

    internal override void UpdateTestRunCoverage(int testRunId, Coverage coverage, Guid projectId)
    {
      this.PrepareStoredProcedure("prc_UpdateTestRunCoverage");
      this.BindInt("@testRunId", testRunId);
      this.BindByte("@state", coverage.State);
      this.BindStringPreserveNull("@errorLog", coverage.LastError, int.MaxValue, SqlDbType.NVarChar);
      this.BindModuleCoverageTypeTable1("@moduleInfo", (IEnumerable<ModuleCoverage>) coverage.Modules);
      this.BindFunctionCoverageTypeTable("@functionInfo", (IEnumerable<FunctionCoverage>) this.GetFunctionCoverages(coverage));
      this.ExecuteNonQuery();
    }

    public override void DeleteTestReleases(
      Guid projectGuid,
      List<int> releaseIds,
      Guid lastUpdatedBy,
      int testRunDeletionBatchSize,
      bool isTcmService)
    {
      this.PrepareStoredProcedure("TestResult.prc_DeleteTestReleases");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt32Table(nameof (releaseIds), (IEnumerable<int>) releaseIds);
      this.BindGuid("@lastUpdatedBy", lastUpdatedBy);
      this.BindInt("@testRunDeletionBatchSize", testRunDeletionBatchSize);
      this.BindBoolean("@isTcmService", isTcmService);
      this.ExecuteNonQuery();
    }

    internal TestManagementDatabase44(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase44()
    {
    }

    public override void QueueDeleteRunsByRetentionSettings(
      TestManagementRequestContext context,
      Guid projectId,
      DateTime currentUtcDate,
      Guid deletedBy,
      int runsDeletionBatchSize,
      int automatedTestRetentionDuration,
      int manualTestRetentionDuration,
      out int automatedRunsDeleted,
      out int manualRunsDeleted,
      bool isTcmService,
      bool isOnpremService,
      int queueDeleteRunsByRetentionSettingsSprocExecTimeOutInSec)
    {
      try
      {
        context.TraceEnter("Database", "ResultRetentionDatabase.QueueDeleteRunsByRetentionSettings");
        this.PrepareStoredProcedure("TestResult.prc_QueueDeleteRunsByRetentionSettings", queueDeleteRunsByRetentionSettingsSprocExecTimeOutInSec);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@automatedDuration", automatedTestRetentionDuration);
        this.BindInt("@manualDuration", manualTestRetentionDuration);
        this.BindDateTime("@currentUtcDate", currentUtcDate);
        this.BindGuid("@deletedBy", deletedBy);
        this.BindInt("@runsDeletionBatchSize", runsDeletionBatchSize);
        this.BindBoolean("@isTcmService", isTcmService);
        SqlDataReader reader = this.ExecuteReader();
        automatedRunsDeleted = 0;
        manualRunsDeleted = 0;
        if (!reader.Read())
          return;
        automatedRunsDeleted = new SqlColumnBinder("AutomatedRunsDeleted").GetInt32((IDataReader) reader);
        manualRunsDeleted = new SqlColumnBinder("ManualRunsDeleted").GetInt32((IDataReader) reader);
      }
      finally
      {
        context.TraceLeave("Database", "ResultRetentionDatabase.QueueDeleteRunsByRetentionSettings");
      }
    }

    public override List<TestCaseResult> FetchTestResultsByRun(
      Guid projectId,
      int testRunId,
      List<int> resultIds,
      bool includeActionResults,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      return this.FetchTestResults(resultIds.Select<int, TestCaseResultIdAndRev>((System.Func<int, TestCaseResultIdAndRev>) (r => new TestCaseResultIdAndRev(new TestCaseResultIdentifier(testRunId, r), 0))).ToArray<TestCaseResultIdAndRev>(), (List<TestCaseResultIdentifier>) null, projectId, includeActionResults, out actionResults, out parameters, out attachments);
    }

    public override List<TestCaseResult> FetchTestResults(
      TestCaseResultIdAndRev[] resultIds,
      List<TestCaseResultIdentifier> deletedIds,
      Guid projectId,
      bool includeActionResults,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.FetchTestResults");
        this.PrepareStoredProcedure("TestResult.prc_FetchTestResults");
        this.BindBoolean("@includeActionResults", includeActionResults);
        this.BindTestCaseResultIdAndRevTypeTable("@idsTable", (IEnumerable<TestCaseResultIdAndRev>) resultIds);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@maxCommentSize", TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext));
        List<TestCaseResult> testCaseResultList = new List<TestCaseResult>(resultIds.Length);
        Dictionary<TestCaseResultIdentifier, TestCaseResult> resultsMap = new Dictionary<TestCaseResultIdentifier, TestCaseResult>(resultIds.Length);
        TestManagementDatabase.FetchTestResultsColumnsWithSquence columnsWithSquence = new TestManagementDatabase.FetchTestResultsColumnsWithSquence();
        using (SqlDataReader reader = this.ExecuteReader())
        {
          while (reader.Read())
          {
            TestCaseResult testCaseResult = columnsWithSquence.bind(reader);
            testCaseResult.CustomFields = new List<TestExtensionField>();
            testCaseResultList.Add(testCaseResult);
          }
          testCaseResultList.Sort((Comparison<TestCaseResult>) ((a, b) => a.SequenceId.CompareTo(b.SequenceId)));
          testCaseResultList.ForEach((Action<TestCaseResult>) (result => resultsMap.Add(new TestCaseResultIdentifier(result.TestRunId, result.TestResultId), result)));
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("TestResult.prc_FetchTestResults");
          TestManagementDatabase.FetchTestResultsExColumns resultsExColumns = new TestManagementDatabase.FetchTestResultsExColumns();
          while (reader.Read())
          {
            Tuple<int, int, TestExtensionField> tuple = resultsExColumns.bind(reader);
            TestCaseResultIdentifier key = new TestCaseResultIdentifier(tuple.Item1, tuple.Item2);
            if (resultsMap.ContainsKey(key))
            {
              if (string.Equals("StackTrace", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].StackTrace = tuple.Item3;
              else if (string.Equals("FailingSince", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string jsonString)
                {
                  FailingSince convertedObject = (FailingSince) null;
                  if (TestResultHelper.TryJsonConvertWithRetry<FailingSince>(jsonString, out convertedObject, true))
                    resultsMap[key].FailingSince = convertedObject;
                }
              }
              else if (string.Equals("Comment", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].Comment = tuple.Item3.Value as string;
              else if (string.Equals("ErrorMessage", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].ErrorMessage = tuple.Item3.Value as string;
              else if (string.Equals("UnsanitizedTestCaseTitle", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string str1)
                  resultsMap[key].TestCaseTitle = str1;
              }
              else if (string.Equals("UnsanitizedAutomatedTestName", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string str2)
                  resultsMap[key].AutomatedTestName = str2;
              }
              else if (string.Equals("MaxReservedSubResultId", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].SubResultCount = (int) tuple.Item3.Value;
              else if (string.Equals("TestResultGroupType", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].ResultGroupType = (ResultGroupType) TestManagementServiceUtility.ValidateAndGetEnumValue<ResultGroupType>((string) tuple.Item3.Value, ResultGroupType.None);
              else
                resultsMap[key].CustomFields.Add(tuple.Item3);
            }
          }
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("TestResult.prc_FetchTestResults");
          if (deletedIds != null)
          {
            TestManagementDatabase.QueryTestResultsColumns testResultsColumns = new TestManagementDatabase.QueryTestResultsColumns();
            while (reader.Read())
              deletedIds.Add(testResultsColumns.bindDeleted(reader));
          }
          if (includeActionResults)
          {
            actionResults = new List<TestActionResult>();
            parameters = new List<TestResultParameter>();
            attachments = new List<TestResultAttachment>();
            this.ReadTestActionResults(actionResults, parameters, attachments, reader, "prc_FetchTestResults");
          }
          else
          {
            actionResults = (List<TestActionResult>) null;
            parameters = (List<TestResultParameter>) null;
            attachments = (List<TestResultAttachment>) null;
          }
        }
        return resultsMap.Values.ToList<TestCaseResult>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.FetchTestResults");
      }
    }

    public override void QueueDeleteTestRun(
      Guid projectId,
      int testRunId,
      Guid updatedBy,
      bool isTcmService)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueueDeleteTestRun");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindBoolean("@isTcmService", isTcmService);
      this.ExecuteNonQuery();
    }

    internal override List<TestCaseResult> FetchSuiteAndConfigurationDetails(
      Guid projectId,
      List<TestCaseResult> results)
    {
      this.PrepareStoredProcedure("TestManagement.prc_FetchSuiteAndConfigurationDetails");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindTestResult_PlannedMetadataIdTypeTable("@results", (IEnumerable<TestCaseResult>) results);
      SqlDataReader reader = this.ExecuteReader();
      List<TestCaseResult> testCaseResultList = new List<TestCaseResult>();
      TestManagementDatabase.FetchTestResultsColumns testResultsColumns = new TestManagementDatabase.FetchTestResultsColumns();
      while (reader.Read())
        testCaseResultList.Add(testResultsColumns.bind(reader));
      return testCaseResultList;
    }

    public override List<TestRun> QueryTestRunsbyFilters(
      Guid projectId,
      QueryTestRunsFilter filters,
      int top,
      int batchSize,
      out int minNextBatchRunId,
      out DateTime minNextBatchLastUpdated)
    {
      try
      {
        minNextBatchRunId = -1;
        minNextBatchLastUpdated = DateTime.MaxValue;
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunsbyFilters");
        Dictionary<int, TestRun> dictionary = new Dictionary<int, TestRun>();
        this.PrepareStoredProcedure("TestResult.prc_QueryTestRuns");
        this.BindIdTypeTable("@planIds", (IEnumerable<int>) filters.PlanIds);
        this.BindNullableInt("@state", filters.State, -1);
        this.BindNullableDateTime("@minLastUpdatedDate", filters.MinLastUpdatedDate);
        this.BindNullableDateTime("@maxLastUpdatedDate", filters.MaxLastUpdatedDate);
        this.BindNullableBoolean("@isAutomated", filters.IsAutomated);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindIdTypeTable("@buildIds", (IEnumerable<int>) filters.BuildIds);
        this.BindIdTypeTable("@buildDefIds", (IEnumerable<int>) filters.BuildDefIds);
        this.BindIdTypeTable("@releaseIds", (IEnumerable<int>) filters.ReleaseIds);
        this.BindIdTypeTable("@releaseDefIds", (IEnumerable<int>) filters.ReleaseDefIds);
        this.BindIdTypeTable("@releaseEnvIds", (IEnumerable<int>) filters.ReleaseEnvIds);
        this.BindIdTypeTable("@releaseEnvDefIds", (IEnumerable<int>) filters.ReleaseEnvDefIds);
        this.BindString("@branchName", filters.BranchName, 400, true, SqlDbType.NVarChar);
        this.BindString("@runTitle", filters.RunTitle, 256, true, SqlDbType.NVarChar);
        this.BindString("@sourceWorkflow", filters.SourceWorkflow, 128, true, SqlDbType.NVarChar);
        this.BindInt("@top", top);
        this.BindInt("@batchMinTestRunId", filters.MinNextBatchTestRunId);
        this.BindInt("@batchSize", batchSize);
        TestManagementDatabase29.QueryTestRunColumnsByFilters columnsByFilters = new TestManagementDatabase29.QueryTestRunColumnsByFilters();
        SqlDataReader reader = this.ExecuteReader();
        while (reader.Read())
        {
          TestRun testRun = columnsByFilters.bind(reader);
          dictionary.Add(testRun.TestRunId, testRun);
        }
        if (reader.NextResult() && reader.Read())
        {
          DateTime minLastUpdatedDate;
          int minBatchRunId;
          new TestManagementDatabase44.QueryRunsByFilterBatchingDetails().bind(reader, out minLastUpdatedDate, out minBatchRunId);
          minNextBatchLastUpdated = minLastUpdatedDate;
          minNextBatchRunId = minBatchRunId;
        }
        return dictionary.Values.ToList<TestRun>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunsbyFilters");
      }
    }

    private new class QueryBuildConfigurationsColumns
    {
      private SqlColumnBinder BuildConfigurationId = new SqlColumnBinder(nameof (BuildConfigurationId));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder BuildPlatform = new SqlColumnBinder(nameof (BuildPlatform));
      private SqlColumnBinder BuildFlavor = new SqlColumnBinder(nameof (BuildFlavor));

      internal BuildConfiguration bind(SqlDataReader reader, out int dataspaceId)
      {
        BuildConfiguration buildConfiguration = new BuildConfiguration();
        buildConfiguration.BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader);
        buildConfiguration.BuildUri = this.BuildUri.GetString((IDataReader) reader, false);
        dataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        buildConfiguration.BuildPlatform = this.BuildPlatform.GetString((IDataReader) reader, false);
        buildConfiguration.BuildFlavor = this.BuildFlavor.GetString((IDataReader) reader, false);
        return buildConfiguration;
      }
    }

    private new class ModuleCoverageColumns
    {
      private SqlColumnBinder CoverageId = new SqlColumnBinder(nameof (CoverageId));
      private SqlColumnBinder ModuleId = new SqlColumnBinder(nameof (ModuleId));
      private SqlColumnBinder Signature = new SqlColumnBinder(nameof (Signature));
      private SqlColumnBinder SignatureAge = new SqlColumnBinder(nameof (SignatureAge));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder LinesCovered = new SqlColumnBinder(nameof (LinesCovered));
      private SqlColumnBinder LinesPartiallyCovered = new SqlColumnBinder(nameof (LinesPartiallyCovered));
      private SqlColumnBinder LinesNotCovered = new SqlColumnBinder(nameof (LinesNotCovered));
      private SqlColumnBinder BlocksCovered = new SqlColumnBinder(nameof (BlocksCovered));
      private SqlColumnBinder BlocksNotCovered = new SqlColumnBinder(nameof (BlocksNotCovered));
      private SqlColumnBinder BlockCount = new SqlColumnBinder(nameof (BlockCount));
      private SqlColumnBinder BlockDataLength = new SqlColumnBinder(nameof (BlockDataLength));
      private SqlColumnBinder BlockData = new SqlColumnBinder(nameof (BlockData));
      private SqlColumnBinder CoverageFileUrl = new SqlColumnBinder(nameof (CoverageFileUrl));

      internal ModuleCoverage Bind(SqlDataReader reader, Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags)
      {
        ModuleCoverage moduleCoverage = new ModuleCoverage()
        {
          CoverageId = this.CoverageId.GetInt32((IDataReader) reader),
          ModuleId = this.ModuleId.GetInt32((IDataReader) reader),
          Name = this.Name.GetString((IDataReader) reader, false),
          Signature = this.Signature.GetGuid((IDataReader) reader, true),
          SignatureAge = this.SignatureAge.GetInt32((IDataReader) reader, 0),
          Statistics = new CoverageStatistics()
        };
        moduleCoverage.Statistics.LinesCovered = this.LinesCovered.GetInt32((IDataReader) reader, 0, 0);
        moduleCoverage.Statistics.LinesPartiallyCovered = this.LinesPartiallyCovered.GetInt32((IDataReader) reader, 0, 0);
        moduleCoverage.Statistics.LinesNotCovered = this.LinesNotCovered.GetInt32((IDataReader) reader, 0, 0);
        moduleCoverage.Statistics.BlocksCovered = this.BlocksCovered.GetInt32((IDataReader) reader, 0, 0);
        moduleCoverage.Statistics.BlocksNotCovered = this.BlocksNotCovered.GetInt32((IDataReader) reader, 0, 0);
        moduleCoverage.CoverageFileUrl = this.CoverageFileUrl.GetString((IDataReader) reader, true);
        if ((flags & Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags.BlockData) != (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) 0)
        {
          moduleCoverage.BlockCount = this.BlockCount.GetInt32((IDataReader) reader, 0);
          int int32 = this.BlockDataLength.GetInt32((IDataReader) reader, 0);
          byte[] bytes = this.BlockData.GetBytes((IDataReader) reader, true);
          if (int32 != -1 && int32 != bytes.Length)
            Array.Resize<byte>(ref bytes, int32);
          moduleCoverage.BlockData = bytes;
        }
        return moduleCoverage;
      }
    }

    private class QueryRunsByFilterBatchingDetails
    {
      private SqlColumnBinder MinLastUpdatedDate = new SqlColumnBinder(nameof (MinLastUpdatedDate));
      private SqlColumnBinder MinBatchRunId = new SqlColumnBinder(nameof (MinBatchRunId));

      internal virtual void bind(
        SqlDataReader reader,
        out DateTime minLastUpdatedDate,
        out int minBatchRunId)
      {
        minLastUpdatedDate = this.MinLastUpdatedDate.GetDateTime((IDataReader) reader);
        minBatchRunId = this.MinBatchRunId.GetInt32((IDataReader) reader);
      }
    }
  }
}
