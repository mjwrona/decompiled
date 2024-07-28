// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase47
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
  public class TestManagementDatabase47 : TestManagementDatabase46
  {
    internal override AfnStrip CreateAfnStrip(
      Guid projectId,
      int tfsFileId,
      AfnStrip afnStrip,
      Guid createdBy,
      bool changeCounterInterval = false)
    {
      this.PrepareStoredProcedure("TestResult.prc_CreateAfnStrip");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testCaseId", afnStrip.TestCaseId);
      this.BindGuid("@createdBy", createdBy);
      this.BindInt("@tfsFileId", tfsFileId);
      this.BindLong("@uncompressedLength", afnStrip.UnCompressedStreamLength);
      this.BindBinary("@emptyStringHash", this.GetSHA256Hash(string.Empty), 32, SqlDbType.VarBinary);
      this.BindBoolean("@changeCounterInterval", changeCounterInterval);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("TestRunId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("TestResultId");
      SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder("AttachmentId");
      reader.Read();
      return new AfnStrip()
      {
        Id = sqlColumnBinder3.GetInt32((IDataReader) reader),
        TestRunId = sqlColumnBinder1.GetInt32((IDataReader) reader),
        TestResultId = sqlColumnBinder2.GetInt32((IDataReader) reader),
        TestCaseId = afnStrip.TestCaseId,
        UnCompressedStreamLength = afnStrip.UnCompressedStreamLength
      };
    }

    internal override List<TestResultAttachment> QueryAttachmentsById(
      TestManagementRequestContext context,
      Guid projectId,
      int attachmentId,
      bool getSiblingAttachments)
    {
      int lazyInitialization = !projectId.Equals(Guid.Empty) ? this.GetDataspaceIdWithLazyInitialization(projectId) : 0;
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      this.PrepareStoredProcedure("TestResult.prc_QueryAttachment");
      this.BindInt("@dataspaceId", lazyInitialization);
      this.BindInt("@attachmentId", attachmentId);
      this.BindBoolean("@getSiblingAttachments", getSiblingAttachments);
      SqlDataReader reader = this.ExecuteReader();
      int testRunId;
      this.GetAttachmentOwnerIds(context, reader, out testRunId, out int _, out int _);
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryAttachment");
      if (testRunId > 0 && !reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryAttachment");
      TestManagementDatabase.QueryAttachmentsColumns attachmentsColumns = new TestManagementDatabase.QueryAttachmentsColumns();
      while (reader.Read())
        resultAttachmentList.Add(attachmentsColumns.Bind(reader));
      return resultAttachmentList;
    }

    internal override Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData GetTestRunSummaryReport2(
      Guid projectGuid,
      int runId,
      List<string> dimensionList)
    {
      this.RequestContext.TraceEnter("Database", "ChartingDatabase.GetTestRunSummaryReport2");
      Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData runSummaryReport2 = new Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData();
      try
      {
        this.PrepareStoredProcedure("TestResult.prc_GetTestRunSummaryReport2");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@runId", runId);
        this.BindNameTypeTable("@dimensions", (IEnumerable<string>) dimensionList);
        SqlDataReader reader = this.ExecuteReader();
        if (reader.HasRows)
        {
          while (reader.Read())
          {
            Dictionary<string, object> dimensionValues = new Dictionary<string, object>();
            foreach (string dimension in dimensionList)
            {
              object dimensionValue = new SqlColumnBinder(dimension).GetObject((IDataReader) reader);
              object obj = (object) Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData.MapDimensionValue(this.RequestContext, dimension, dimensionValue);
              dimensionValues[dimension] = obj;
            }
            long int64 = Convert.ToInt64(new SqlColumnBinder("AggTestsCount").GetInt32((IDataReader) reader));
            runSummaryReport2.AddReportDatarow(dimensionValues, int64);
          }
        }
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "ChartingDatabase.GetTestRunSummaryReport2");
      }
      return runSummaryReport2;
    }

    internal override Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData GetTestExecutionReport2(
      Guid projectId,
      int planId,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails> testAuthoringDetails,
      List<string> dimensionList)
    {
      this.RequestContext.TraceEnter("Database", "ChartingDatabase.GetTestExecutionReport2");
      Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData executionReport2 = new Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData();
      try
      {
        this.PrepareStoredProcedure("TestManagement.prc_GetTestExecutionReport2");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@planId", planId);
        this.BindTestAuthoringDetailsTypeTableTable("@authoringDetails", (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails>) testAuthoringDetails);
        this.BindNameTypeTable("@dimensions", (IEnumerable<string>) dimensionList);
        SqlDataReader reader = this.ExecuteReader();
        if (reader.HasRows)
        {
          while (reader.Read())
          {
            Dictionary<string, object> dimensionValues = new Dictionary<string, object>();
            foreach (string dimension in dimensionList)
            {
              object obj = new SqlColumnBinder(dimension).GetObject((IDataReader) reader);
              dimensionValues[dimension] = obj;
            }
            long int64 = Convert.ToInt64(new SqlColumnBinder("AggTestsCount").GetInt32((IDataReader) reader));
            executionReport2.AddReportDatarow(dimensionValues, int64);
          }
        }
        return executionReport2;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "ChartingDatabase.GetTestExecutionReport2");
      }
    }

    public override List<BuildCoverage> QueryBuildCoverage(
      Guid projectGuid,
      string buildUri,
      Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags)
    {
      this.PrepareStoredProcedure("prc_QueryBuildCoverage");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectGuid));
      this.BindString("@buildUri", buildUri, 64, false, SqlDbType.NVarChar);
      this.BindInt("@flags", (int) flags);
      SqlDataReader reader = this.ExecuteReader();
      SortedDictionary<int, Coverage> coverageById = new SortedDictionary<int, Coverage>();
      TestManagementDatabase47.BuildCoverageColumns buildCoverageColumns = new TestManagementDatabase47.BuildCoverageColumns();
      while (reader.Read())
      {
        BuildCoverage buildCoverage = buildCoverageColumns.Bind(reader);
        coverageById.Add(buildCoverage.Id, (Coverage) buildCoverage);
      }
      this.ReadCoverage(flags, "prc_QueryBuildCoverage", reader, coverageById);
      List<BuildCoverage> buildCoverageList = new List<BuildCoverage>(coverageById.Values.Count);
      foreach (BuildCoverage buildCoverage in coverageById.Values)
        buildCoverageList.Add(buildCoverage);
      return buildCoverageList;
    }

    public override Dictionary<int, AggregatedDataForResultTrend> QueryAggregatedDataByRequirementForBuild2(
      GuidAndString projectId,
      List<int> testCaseRefIds,
      BuildConfiguration buildConfigurationInfo,
      string sourceWorkflow,
      int numberOfDaysAgo,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryMostRecentSummaryByRequirementForBuild2");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId.GuidId));
      this.BindInt("@buildDefinitionId", buildConfigurationInfo.BuildDefinitionId);
      this.BindString("@branchName", buildConfigurationInfo.BranchName, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("@repositoryId", buildConfigurationInfo.RepositoryId, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindInt("@numberOfDaysAgo", numberOfDaysAgo);
      this.BindIdTypeTable("@testCaseRefIds", (IEnumerable<int>) testCaseRefIds);
      this.BindInt("@runIdThreshold", runIdThreshold);
      SqlDataReader reader = this.ExecuteReader();
      Dictionary<int, AggregatedDataForResultTrend> testCaseRefToAggregateOutcomeMap = new Dictionary<int, AggregatedDataForResultTrend>();
      TestManagementDatabase32.FetchAggregatedDataByRequirementColumns2 requirementColumns2 = new TestManagementDatabase32.FetchAggregatedDataByRequirementColumns2();
      while (reader.Read())
        requirementColumns2.bindAndAdd(reader, testCaseRefToAggregateOutcomeMap);
      if (reader.NextResult() && reader.Read())
      {
        BuildConfiguration buildConfiguration = new TestManagementDatabase14.FetchBuildInformationColumns().bind(reader);
        testCaseRefToAggregateOutcomeMap.Values.ToList<AggregatedDataForResultTrend>().ForEach((Action<AggregatedDataForResultTrend>) (d => d.TestResultsContext = new TestResultsContext()
        {
          ContextType = TestResultsContextType.Build,
          Build = new BuildReference()
          {
            Id = buildConfiguration.BuildId,
            Number = buildConfiguration.BuildNumber,
            DefinitionId = buildConfiguration.BuildDefinitionId,
            BranchName = buildConfiguration.BranchName
          }
        }));
      }
      return testCaseRefToAggregateOutcomeMap;
    }

    public override Dictionary<int, AggregatedDataForResultTrend> QueryAggregatedDataByRequirementForRelease2(
      GuidAndString projectId,
      List<int> testCaseRefIds,
      int releaseDefinitionId,
      int releaseEnvironmentDefId,
      string sourceWorkflow,
      int numberOfDaysAgo,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryMostRecentSummaryByRequirementForRelease2");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId.GuidId));
      this.BindInt("@releaseDefinitionId", releaseDefinitionId);
      this.BindInt("@releaseEnvironmentDefinitionId", releaseEnvironmentDefId);
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindInt("@numberOfDaysAgo", numberOfDaysAgo);
      this.BindIdTypeTable("@testCaseRefIds", (IEnumerable<int>) testCaseRefIds);
      this.BindInt("@runIdThreshold", runIdThreshold);
      SqlDataReader reader = this.ExecuteReader();
      Dictionary<int, AggregatedDataForResultTrend> testCaseRefToAggregateOutcomeMap = new Dictionary<int, AggregatedDataForResultTrend>();
      TestManagementDatabase32.FetchAggregatedDataByRequirementColumns2 requirementColumns2 = new TestManagementDatabase32.FetchAggregatedDataByRequirementColumns2();
      while (reader.Read())
        requirementColumns2.bindAndAdd(reader, testCaseRefToAggregateOutcomeMap);
      if (reader.NextResult() && reader.Read())
      {
        ReleaseReference releaseRef = new TestManagementDatabase15.FetchReleaseInformationColumns().bind(reader);
        testCaseRefToAggregateOutcomeMap.Values.ToList<AggregatedDataForResultTrend>().ForEach((Action<AggregatedDataForResultTrend>) (d => d.TestResultsContext = new TestResultsContext()
        {
          ContextType = TestResultsContextType.Release,
          Release = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
          {
            Id = releaseRef.ReleaseId,
            EnvironmentId = releaseRef.ReleaseEnvId,
            Name = releaseRef.ReleaseName
          }
        }));
      }
      return testCaseRefToAggregateOutcomeMap;
    }

    public override Dictionary<int, TestSummaryForWorkItem> QueryAggregatedDataByRequirementForBuild(
      GuidAndString projectId,
      List<int> workItemIds,
      BuildConfiguration buildConfigurationInfo,
      string sourceWorkflow,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryMostRecentSummaryByRequirementForBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId.GuidId));
      this.BindInt("@buildDefinitionId", buildConfigurationInfo.BuildDefinitionId);
      this.BindString("@branchName", buildConfigurationInfo.BranchName, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("@repositoryId", buildConfigurationInfo.RepositoryId, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindIdTypeTable("@workItemIds", (IEnumerable<int>) workItemIds);
      this.BindInt("@runIdThreshold", runIdThreshold);
      SqlDataReader reader = this.ExecuteReader();
      Dictionary<int, TestSummaryForWorkItem> testSummaryForWorkItems = new Dictionary<int, TestSummaryForWorkItem>();
      foreach (int workItemId in workItemIds)
        testSummaryForWorkItems.Add(workItemId, (TestSummaryForWorkItem) null);
      TestManagementDatabase14.FetchAggregatedDataByRequirementColumns requirementColumns = new TestManagementDatabase14.FetchAggregatedDataByRequirementColumns();
      while (reader.Read())
        requirementColumns.bindAndAdd(reader, testSummaryForWorkItems);
      if (reader.NextResult() && reader.Read())
      {
        BuildConfiguration buildConfiguration = new TestManagementDatabase14.FetchBuildInformationColumns().bind(reader);
        foreach (TestSummaryForWorkItem summaryForWorkItem in testSummaryForWorkItems.Values)
        {
          summaryForWorkItem.Summary.TestResultsContext.ContextType = TestResultsContextType.Build;
          summaryForWorkItem.Summary.TestResultsContext.Build = new BuildReference()
          {
            Id = buildConfiguration.BuildId,
            Number = buildConfiguration.BuildNumber,
            DefinitionId = buildConfiguration.BuildDefinitionId,
            BranchName = buildConfiguration.BranchName
          };
        }
      }
      return testSummaryForWorkItems;
    }

    public override Dictionary<int, TestSummaryForWorkItem> QueryAggregatedDataByRequirementForRelease(
      GuidAndString projectId,
      List<int> workItemIds,
      int releaseDefinitionId,
      int releaseEnvironmentDefId,
      string sourceWorkflow,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryMostRecentSummaryByRequirementForRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId.GuidId));
      this.BindInt("@releaseDefinitionId", releaseDefinitionId);
      this.BindInt("@releaseEnvironmentDefinitionId", releaseEnvironmentDefId);
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindIdTypeTable("@workItemIds", (IEnumerable<int>) workItemIds);
      this.BindInt("@runIdThreshold", runIdThreshold);
      SqlDataReader reader = this.ExecuteReader();
      Dictionary<int, TestSummaryForWorkItem> testSummaryForWorkItems = new Dictionary<int, TestSummaryForWorkItem>();
      foreach (int workItemId in workItemIds)
        testSummaryForWorkItems.Add(workItemId, (TestSummaryForWorkItem) null);
      TestManagementDatabase14.FetchAggregatedDataByRequirementColumns requirementColumns = new TestManagementDatabase14.FetchAggregatedDataByRequirementColumns();
      while (reader.Read())
        requirementColumns.bindAndAdd(reader, testSummaryForWorkItems);
      if (reader.NextResult() && reader.Read())
      {
        ReleaseReference releaseReference = new TestManagementDatabase15.FetchReleaseInformationColumns().bind(reader);
        foreach (TestSummaryForWorkItem summaryForWorkItem in testSummaryForWorkItems.Values)
        {
          summaryForWorkItem.Summary.TestResultsContext.ContextType = TestResultsContextType.Release;
          summaryForWorkItem.Summary.TestResultsContext.Release = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
          {
            Id = releaseReference.ReleaseId,
            EnvironmentId = releaseReference.ReleaseEnvId,
            Name = releaseReference.ReleaseName
          };
        }
      }
      return testSummaryForWorkItems;
    }

    internal TestManagementDatabase47(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase47()
    {
    }

    internal override TestResultsGroupsData GetTestResultAutomatedTestStorageAndOwnersByBuild(
      Guid projectId,
      int buildId,
      string publishContext,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsGroupsByBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", publishContext, 128, true, SqlDbType.NVarChar);
      this.BindInt("@buildId", buildId);
      this.BindInt("@runIdThreshold", runIdThreshold);
      SqlDataReader reader = this.ExecuteReader();
      TestResultsGroupsData andOwnersByBuild = new TestResultsGroupsData();
      TestResultsGroupsData testResultsGroupsData = andOwnersByBuild;
      TestManagementDatabase47.GetTestResultsGroupResponse(reader, testResultsGroupsData);
      return andOwnersByBuild;
    }

    internal override TestResultsGroupsData GetTestResultAutomatedTestStorageAndOwnersByRelease(
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsGroupsByRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", publishContext, 128, true, SqlDbType.NVarChar);
      this.BindNullableInt("@releaseId", releaseId, 0);
      this.BindNullableInt("@releaseEnvId", releaseEnvId, 0);
      this.BindInt("@runIdThreshold", runIdThreshold);
      SqlDataReader reader = this.ExecuteReader();
      TestResultsGroupsData andOwnersByRelease = new TestResultsGroupsData();
      TestResultsGroupsData testResultsGroupsData = andOwnersByRelease;
      TestManagementDatabase47.GetTestResultsGroupResponse(reader, testResultsGroupsData);
      return andOwnersByRelease;
    }

    internal override TestResultsGroupsDataWithWaterMark GetTestResultAutomatedTestStorageAndOwnersByRelease(
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top,
      int runIdThreshold)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsGroupsByReleaseWithWatermark");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", publishContext, 128, true, SqlDbType.NVarChar);
      this.BindNullableInt("@releaseId", releaseId, 0);
      this.BindNullableInt("@releaseEnvId", releaseEnvId, 0);
      this.BindInt("@continuationTokenRunId", continuationTokenRunId);
      this.BindInt("@continuationTokenResultId", continuationTokenResultId);
      this.BindInt("@top", top);
      this.BindInt("@runIdThreshold", runIdThreshold);
      SqlDataReader reader = this.ExecuteReader();
      TestCaseResultIdentifier resultIdentifier = TestManagementDatabase47.GetLastResultIdentifier(reader);
      TestResultsGroupsData resultsGroupsData = new TestResultsGroupsData();
      if (reader.NextResult())
        TestManagementDatabase47.GetTestResultsGroupResponse(reader, resultsGroupsData);
      return new TestResultsGroupsDataWithWaterMark(resultsGroupsData, resultIdentifier);
    }

    internal override TestResultsGroupsDataWithWaterMark GetTestResultAutomatedTestStorageAndOwnersByBuild(
      Guid projectId,
      int buildId,
      string publishContext,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsGroupsByBuildWithWaterMark");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", publishContext, 128, true, SqlDbType.NVarChar);
      this.BindInt("@buildId", buildId);
      this.BindInt("@continuationTokenRunId", continuationTokenRunId);
      this.BindInt("@continuationTokenResultId", continuationTokenResultId);
      this.BindInt("@top", top);
      this.BindInt("@runIdThreshold", runIdThreshold);
      SqlDataReader reader = this.ExecuteReader();
      TestCaseResultIdentifier resultIdentifier = TestManagementDatabase47.GetLastResultIdentifier(reader);
      TestResultsGroupsData resultsGroupsData = new TestResultsGroupsData();
      if (reader.NextResult())
        TestManagementDatabase47.GetTestResultsGroupResponse(reader, resultsGroupsData);
      return new TestResultsGroupsDataWithWaterMark(resultsGroupsData, resultIdentifier);
    }

    private static void GetTestResultsGroupResponse(
      SqlDataReader reader,
      TestResultsGroupsData testResultsGroupsData)
    {
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("AutomatedTestStorage");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("Owner");
      while (reader.Read())
      {
        testResultsGroupsData.AutomatedTestStorage.Add(sqlColumnBinder1.GetString((IDataReader) reader, true));
        testResultsGroupsData.Owner.Add(sqlColumnBinder2.GetString((IDataReader) reader, true));
      }
    }

    private static TestCaseResultIdentifier GetLastResultIdentifier(SqlDataReader reader)
    {
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("LastRunId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("LastResultId");
      return reader.Read() ? new TestCaseResultIdentifier(sqlColumnBinder1.GetInt32((IDataReader) reader), sqlColumnBinder2.GetInt32((IDataReader) reader)) : (TestCaseResultIdentifier) null;
    }

    internal override Dictionary<int, string> QueryShallowTestConfigurations(
      Guid projectId,
      List<int> configIds)
    {
      Dictionary<int, string> dictionary = new Dictionary<int, string>();
      this.PrepareStoredProcedure("TestManagement.prc_QueryShallowConfigurations");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindIdTypeTable("@configIds", (IEnumerable<int>) configIds);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase47.QueryShallowTestConfigurationsColumns configurationsColumns = new TestManagementDatabase47.QueryShallowTestConfigurationsColumns();
      while (reader.Read())
      {
        (int, string) tuple = configurationsColumns.bind(reader);
        dictionary[tuple.Item1] = tuple.Item2;
      }
      return dictionary;
    }

    public override TestHistoryContinuationTokenAndResults QueryTestHistory(
      Guid projectId,
      TestHistoryQuery filter,
      int continuationTokenMinRunId,
      int continuationTokenMaxRunId,
      int testResultBatchLimit,
      int runIdThreshold = 0)
    {
      string empty = string.Empty;
      string storedProcedure;
      if (filter.GroupBy == TestResultGroupBy.Branch)
      {
        storedProcedure = "TestResult.prc_QueryTestHistoryByBranch";
        this.PrepareStoredProcedure(storedProcedure);
        this.BindInt("@buildDefinitionId", filter.BuildDefinitionId);
      }
      else
      {
        storedProcedure = "TestResult.prc_QueryTestHistoryByEnvironment";
        this.PrepareStoredProcedure(storedProcedure);
        this.BindInt("@releaseEnvDefinitionId", filter.ReleaseEnvDefinitionId);
      }
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindBinary("@automatedTestNameHash", this.GetSHA256Hash(filter.AutomatedTestName ?? string.Empty), 32, SqlDbType.VarBinary);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindString("@branchName", filter.Branch, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindInt("@continuationTokenMinRunId", continuationTokenMinRunId);
      this.BindInt("@continuationTokenMaxRunId", continuationTokenMaxRunId);
      this.BindInt("@testResultBatchLimit", testResultBatchLimit);
      this.BindInt("@runIdThreshold", runIdThreshold);
      TestResultHistoryGroupBy groupBy = TestResultHistoryGroupBy.Branch;
      if (filter.GroupBy == TestResultGroupBy.Branch)
      {
        if (filter.BuildDefinitionId > 0)
          groupBy = TestResultHistoryGroupBy.BuildDefinitionId;
      }
      else
        groupBy = TestResultHistoryGroupBy.Environment;
      TestHistoryContinuationTokenAndResults continuationTokenAndResults = new TestHistoryContinuationTokenAndResults()
      {
        ContinuationTokenMinRunId = 0,
        ContinuationTokenMaxRunId = 0,
        results = new Dictionary<string, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>()
      };
      Dictionary<string, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>> resultsMap = new Dictionary<string, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>();
      TestManagementDatabase42.QueryTestHistoryColumns testHistoryColumns = new TestManagementDatabase42.QueryTestHistoryColumns();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          testHistoryColumns.bindResultMap(reader, resultsMap, groupBy, filter.BuildDefinitionId, projectId);
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException(storedProcedure);
        reader.Read();
        int continuationTokenMinRunId1;
        int continuationTokenMaxRunId1;
        new TestManagementDatabase42.QueryTestHistoryColumns().bindContinuationToken(reader, out continuationTokenMinRunId1, out continuationTokenMaxRunId1);
        continuationTokenAndResults.ContinuationTokenMinRunId = continuationTokenMinRunId1;
        continuationTokenAndResults.ContinuationTokenMaxRunId = continuationTokenMaxRunId1;
      }
      continuationTokenAndResults.results = resultsMap;
      return continuationTokenAndResults;
    }

    public override TestResultHistory QueryTestCaseResultHistory2(
      Guid projectId,
      ResultsFilter filter,
      int runIdThreshold = 0)
    {
      string groupBy = filter.GroupBy;
      int definitionId = 0;
      byte contextTypeFromFilter = this.GetResultContextTypeFromFilter(filter, out definitionId);
      if (groupBy.Equals("Branch", StringComparison.OrdinalIgnoreCase))
        this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsTrendByBranch");
      else if (groupBy.Equals("Environment", StringComparison.OrdinalIgnoreCase))
        this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsTrendByEnvironment");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindBinary("@automatedTestNameHash", this.GetSHA256Hash(filter.AutomatedTestName ?? string.Empty), 32, SqlDbType.VarBinary);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      if (groupBy.Equals("Branch", StringComparison.OrdinalIgnoreCase))
      {
        this.BindInt("@buildDays", 15);
        this.BindByte("@testResultContextType", contextTypeFromFilter);
      }
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindInt("@definitionId", definitionId);
      this.BindInt("@runIdThreshold", runIdThreshold);
      if (groupBy.Equals("Environment", StringComparison.OrdinalIgnoreCase))
        this.BindString("@branchName", filter.Branch, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      TestResultHistory testResultHistory = new TestResultHistory();
      testResultHistory.GroupByField = groupBy;
      Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap = new Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      Dictionary<string, TestResultHistoryDetailsForGroup> aggregatedResultsMap = new Dictionary<string, TestResultHistoryDetailsForGroup>();
      TestManagementDatabase23.QueryTestCaseResultHistoryColumns2 resultHistoryColumns2_1 = new TestManagementDatabase23.QueryTestCaseResultHistoryColumns2();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          resultHistoryColumns2_1.bindResultsMap(reader, resultsMap);
        if (reader.NextResult())
        {
          TestManagementDatabase23.QueryTestCaseResultHistoryColumns2 resultHistoryColumns2_2 = new TestManagementDatabase23.QueryTestCaseResultHistoryColumns2();
          while (reader.Read())
            resultHistoryColumns2_2.bindGroupedResultHistory2(reader, aggregatedResultsMap, resultsMap, groupBy);
        }
      }
      testResultHistory.ResultsForGroup = (IList<TestResultHistoryDetailsForGroup>) aggregatedResultsMap.Values.ToList<TestResultHistoryDetailsForGroup>();
      return testResultHistory;
    }

    public override Dictionary<ReleaseReference, RunSummaryAndInsights> QueryTestRunSummaryForReleases(
      GuidAndString projectId,
      List<ReleaseReference> releases,
      string categoryName,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("prc_QueryTestRunSummaryForReleases");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindReleaseRefTypeTable4("@releases", (IEnumerable<ReleaseReference>) releases);
      this.BindString("@categoryName", categoryName, 128, true, SqlDbType.NVarChar);
      this.BindInt("@runIdThreshold", runIdThreshold);
      SqlDataReader reader = this.ExecuteReader();
      Dictionary<ReleaseReference, RunSummaryAndInsights> runSummaryAndInsightsMap = new Dictionary<ReleaseReference, RunSummaryAndInsights>();
      TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
      TestManagementDatabase36.FetchTestRunsByStateColumns runsByStateColumns = new TestManagementDatabase36.FetchTestRunsByStateColumns();
      while (reader.Read())
      {
        ReleaseReference releaseReference = runsByStateColumns.bindRelease(reader);
        this.AddEmptySummaryEntryIfNotPresent(runSummaryAndInsightsMap, releaseReference);
        runSummaryAndInsightsMap[releaseReference].TestRunSummary.CurrentAggregatedRunsByState.Add(runsByStateColumns.bind(reader));
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryForReleases");
      while (reader.Read())
      {
        ReleaseReference releaseReference = runSummaryColumns.bindRelease(reader);
        this.AddEmptySummaryEntryIfNotPresent(runSummaryAndInsightsMap, releaseReference);
        runSummaryAndInsightsMap[releaseReference].TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
      }
      if (!string.IsNullOrEmpty(categoryName))
      {
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryForReleases");
        while (reader.Read())
        {
          TestManagementDatabase19.FetchTestRunSummaryByCategoryColumns byCategoryColumns = new TestManagementDatabase19.FetchTestRunSummaryByCategoryColumns();
          ReleaseReference key = runSummaryColumns.bindRelease(reader);
          runSummaryAndInsightsMap[key].TestRunSummary.CurrentAggregateDataByReportingCategory.Add(byCategoryColumns.bind(reader));
        }
      }
      return runSummaryAndInsightsMap;
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
      SqlDataReader reader = this.ExecuteReader();
      bool flag = true;
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
      if (reader.Read())
      {
        sqlColumnBinder = new SqlColumnBinder("IsRunsAvailable");
        flag = sqlColumnBinder.GetBoolean((IDataReader) reader);
      }
      if (!flag)
        return runSummaryAndInsights;
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      if (reader.Read())
      {
        ref bool local = ref isBuildOld;
        sqlColumnBinder = new SqlColumnBinder("IsBuildOld");
        int num = sqlColumnBinder.GetBoolean((IDataReader) reader) ? 1 : 0;
        local = num != 0;
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      TestManagementDatabase36.FetchTestRunsByStateColumns runsByStateColumns = new TestManagementDatabase36.FetchTestRunsByStateColumns();
      while (reader.Read())
      {
        RunSummaryByState runSummaryByState = runsByStateColumns.bind(reader);
        runSummaryAndInsights.TestRunSummary.CurrentAggregatedRunsByState.Add(runSummaryByState);
        runsCount += runSummaryByState.RunsCount;
      }
      if (runsCount > 0)
      {
        if (returnSummary)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
          TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
          while (reader.Read())
            runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          if (!isBuildOld)
          {
            if (!reader.NextResult())
              throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
            while (reader.Read())
              runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          }
        }
        if (returnFailureDetails && !isBuildOld)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
          TestManagementDatabase2.FetchTestFailureDetailsColumns failureDetailsColumns = new TestManagementDatabase2.FetchTestFailureDetailsColumns();
          while (reader.Read())
            runSummaryAndInsights.TestResultInsights.Add(failureDetailsColumns.bind(reader));
        }
        TestManagementDatabase19.PopulateAggregateDataByCategory(categoryName, reader, runSummaryAndInsights, "prc_QueryTestRunSummaryAndInsightsForBuild");
      }
      return runSummaryAndInsights;
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
      TestManagementDatabase36.FetchTestRunsByStateColumns runsByStateColumns = new TestManagementDatabase36.FetchTestRunsByStateColumns();
      while (reader.Read())
      {
        RunSummaryByState runSummaryByState = runsByStateColumns.bind(reader);
        runSummaryAndInsights.TestRunSummary.CurrentAggregatedRunsByState.Add(runSummaryByState);
        runsCount += runSummaryByState.RunsCount;
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

    private void AddEmptySummaryEntryIfNotPresent(
      Dictionary<ReleaseReference, RunSummaryAndInsights> runSummaryAndInsightsMap,
      ReleaseReference release)
    {
      if (runSummaryAndInsightsMap.ContainsKey(release))
        return;
      runSummaryAndInsightsMap[release] = new RunSummaryAndInsights()
      {
        TestRunSummary = new RunSummary()
        {
          CurrentAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          PreviousAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          CurrentAggregatedRunsByState = (IList<RunSummaryByState>) new List<RunSummaryByState>(),
          CurrentAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>()
        }
      };
    }

    public override List<AggregatedDataForResultTrend> QueryTestResultTrendForBuild3(
      Guid projectId,
      TestResultTrendFilter filter,
      bool calculateEffectiveRunDuration,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultTrendForBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", filter.PublishContext, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindIdTypeTable("@definitionIds", filter.DefinitionIds != null ? filter.DefinitionIds.Distinct<int>() : (IEnumerable<int>) null);
      this.BindBranchNameTypeTable("@branchNames", (IEnumerable<string>) filter.BranchNames);
      this.BindNameTypeTable("@runTitles", (IEnumerable<string>) filter.TestRunTitles);
      this.BindInt("@buildCount", filter.BuildCount);
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      this.BindInt("@runIdThreshold", runIdThreshold);
      TestManagementDatabase36.QueryAggregatedResultsForBuild4 resultsForBuild4 = new TestManagementDatabase36.QueryAggregatedResultsForBuild4();
      Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap = new Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>>();
      Dictionary<int, SortedSet<RunSummaryByOutcome>> executionDetailsForTestRuns = new Dictionary<int, SortedSet<RunSummaryByOutcome>>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          resultsForBuild4.bindAggregatedResultsForBuild(reader, aggregatedResultsMap, executionDetailsForTestRuns);
      }
      foreach (KeyValuePair<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> keyValuePair in aggregatedResultsMap)
        keyValuePair.Value.Item2.Duration = TimeSpan.FromMilliseconds((double) TestManagementServiceUtility.CalculateEffectiveTestRunDuration(executionDetailsForTestRuns[keyValuePair.Key], calculateEffectiveRunDuration));
      return aggregatedResultsMap.Values.Select<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>((System.Func<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>) (a => a.Item2)).ToList<AggregatedDataForResultTrend>();
    }

    public override List<AggregatedDataForResultTrend> QueryTestResultTrendForRelease3(
      Guid projectId,
      TestResultTrendFilter filter,
      bool calculateEffectiveRunDuration,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultTrendForRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", filter.PublishContext, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindIdTypeTable("@definitionIds", filter.DefinitionIds != null ? filter.DefinitionIds.Distinct<int>() : (IEnumerable<int>) null);
      this.BindIdTypeTable("@envDefinitionIds", filter.EnvDefinitionIds != null ? filter.EnvDefinitionIds.Distinct<int>() : (IEnumerable<int>) null);
      this.BindNameTypeTable("@runTitles", (IEnumerable<string>) filter.TestRunTitles);
      this.BindInt("@releaseCount", filter.BuildCount);
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      this.BindInt("@runIdThreshold", runIdThreshold);
      TestManagementDatabase36.QueryAggregatedResultsForRelease4 resultsForRelease4 = new TestManagementDatabase36.QueryAggregatedResultsForRelease4();
      Dictionary<int, SortedSet<RunSummaryByOutcome>> executionDetailsForTestRuns = new Dictionary<int, SortedSet<RunSummaryByOutcome>>();
      Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap = new Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          resultsForRelease4.bindAggregatedResultsForRelease(reader, aggregatedResultsMap, executionDetailsForTestRuns);
      }
      foreach (KeyValuePair<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> keyValuePair in aggregatedResultsMap)
        keyValuePair.Value.Item2.Duration = TimeSpan.FromMilliseconds((double) TestManagementServiceUtility.CalculateEffectiveTestRunDuration(executionDetailsForTestRuns[keyValuePair.Key], calculateEffectiveRunDuration));
      return aggregatedResultsMap.Values.Select<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>((System.Func<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>) (a => a.Item2)).ToList<AggregatedDataForResultTrend>();
    }

    private new class BuildCoverageColumns
    {
      private SqlColumnBinder BuildConfigurationId = new SqlColumnBinder(nameof (BuildConfigurationId));
      private SqlColumnBinder OldBuildConfigurationId = new SqlColumnBinder(nameof (OldBuildConfigurationId));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder BuildPlatform = new SqlColumnBinder(nameof (BuildPlatform));
      private SqlColumnBinder BuildFlavor = new SqlColumnBinder(nameof (BuildFlavor));
      private SqlColumnBinder CoverageId = new SqlColumnBinder(nameof (CoverageId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder LastError = new SqlColumnBinder(nameof (LastError));
      private SqlColumnBinder DateCreated = new SqlColumnBinder(nameof (DateCreated));
      private SqlColumnBinder DateModified = new SqlColumnBinder(nameof (DateModified));

      internal BuildCoverage Bind(SqlDataReader reader)
      {
        BuildCoverage buildCoverage = new BuildCoverage()
        {
          Configuration = new BuildConfiguration()
        };
        buildCoverage.Configuration.BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader);
        buildCoverage.Configuration.OldBuildConfigurationId = this.OldBuildConfigurationId.ColumnExists((IDataReader) reader) ? this.OldBuildConfigurationId.GetInt32((IDataReader) reader) : 0;
        buildCoverage.Configuration.BuildUri = this.BuildUri.GetString((IDataReader) reader, false);
        buildCoverage.Configuration.BuildPlatform = this.BuildPlatform.GetString((IDataReader) reader, false);
        buildCoverage.Configuration.BuildFlavor = this.BuildFlavor.GetString((IDataReader) reader, false);
        buildCoverage.Id = this.CoverageId.GetInt32((IDataReader) reader);
        buildCoverage.State = this.State.GetByte((IDataReader) reader);
        buildCoverage.LastError = this.LastError.GetString((IDataReader) reader, true);
        return buildCoverage;
      }
    }

    protected class QueryShallowTestConfigurationsColumns
    {
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));

      internal (int, string) bind(SqlDataReader reader) => (this.ConfigurationId.GetInt32((IDataReader) reader), this.Name.GetString((IDataReader) reader, false));
    }
  }
}
