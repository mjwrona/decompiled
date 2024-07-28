// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase15
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase15 : TestManagementDatabase14
  {
    internal override List<TestResultAttachment> QueryAttachments2(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      this.PrepareDynamicProcedure("TestResult.prc_QueryAttachments2");
      this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@orderByClause", orderBy, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.QueryAttachmentsColumns2 attachmentsColumns2 = new TestManagementDatabase.QueryAttachmentsColumns2();
      while (reader.Read())
        resultAttachmentList.Add(attachmentsColumns2.Bind(reader));
      return resultAttachmentList;
    }

    public override void RestoreRequirementToTestLink(
      GuidAndString projectId,
      int workItemId,
      Guid updatedBy)
    {
      this.PrepareStoredProcedure("TestResult.prc_RestoreRequirementToTestLinks");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId.GuidId));
      this.BindInt("@workItemId", workItemId);
      this.BindGuid("@restoredBy", updatedBy);
      this.ExecuteReader();
    }

    public override void DestroyRequirementToTestLink(IEnumerable<int> workItemIds, int batchSize)
    {
      this.PrepareStoredProcedure("TestResult.prc_DestroyRequirementToTestLinks");
      this.BindIdTypeTable("@workItemIds", workItemIds);
      this.BindInt("@destroyBatchSize", batchSize);
      this.ExecuteReader();
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
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindIdTypeTable("@workItemIds", (IEnumerable<int>) workItemIds);
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

    internal TestManagementDatabase15(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase15()
    {
    }

    protected override void BindQueryTestResultTrendParams(Guid projectId, ResultsFilter filter)
    {
      base.BindQueryTestResultTrendParams(projectId, filter);
      int parameterValue = 0;
      if (filter.TestResultsContext.ContextType == TestResultsContextType.Release)
        parameterValue = filter.TestResultsContext.Release.EnvironmentDefinitionId;
      this.BindInt("@environmentDefId", parameterValue);
    }

    public override List<AggregatedDataForResultTrend> QueryTestResultTrendForRelease(
      Guid projectId,
      TestResultTrendFilter filter)
    {
      string empty = string.Empty;
      string str1 = string.Empty;
      string str2 = string.Format(SQLConstants.FilterClause_IN, (object) TestResultsConstants.ReleaseDefinitionIdColumnName, (object) "SELECT Id FROM @definitionIds");
      if (filter.TestRunTitles != null && filter.TestRunTitles.Count > 0)
        str1 = string.Format(SQLConstants.FilterClause_IN, (object) TestResultsConstants.TestRunTitleColumnName, (object) "SELECT Name FROM @runTitles");
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementDynamicSqlBatchStatements.dynprc_QueryTestResultTrendForRelease, !string.IsNullOrEmpty(str2) ? (object) str2 : (object) TestResultsConstants.TrueCondition, !string.IsNullOrEmpty(str1) ? (object) str1 : (object) TestResultsConstants.TrueCondition);
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement, 0, true, true);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", filter.PublishContext, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindIdTypeTable("@definitionIds", filter.DefinitionIds != null ? filter.DefinitionIds.Distinct<int>() : (IEnumerable<int>) null);
      this.BindNameTypeTable("@runTitles", (IEnumerable<string>) filter.TestRunTitles);
      this.BindInt("@buildCount", filter.BuildCount);
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      TestManagementDatabase15.QueryAggregatedResultsForRelease resultsForRelease = new TestManagementDatabase15.QueryAggregatedResultsForRelease();
      Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap = new Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader.HasRows)
        {
          while (reader.Read())
            resultsForRelease.bindBuildReference(reader, aggregatedResultsMap);
          if (reader.NextResult())
          {
            while (reader.Read())
              resultsForRelease.bindAggregateValues(reader, aggregatedResultsMap);
          }
        }
      }
      return aggregatedResultsMap.Values.Select<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>((System.Func<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>) (a => a.Item2)).ToList<AggregatedDataForResultTrend>();
    }

    protected override string GetDynmSprocAndFieldsToFetch(
      List<string> coreFields,
      out List<string> fieldsToFetch)
    {
      HashSet<string> source = new HashSet<string>((IEnumerable<string>) coreFields);
      if (source.Contains<string>(TestResultsConstants.TestResultPropertyOwner, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        source.Add(TestResultsConstants.TestResultPropertyTestCaseOwner);
      if (source.Contains<string>(TestResultsConstants.TestResultPropertyDuration, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        if (!source.Contains<string>(TestResultsConstants.TestResultPropertyDateStarted, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          source.Add(TestResultsConstants.TestResultPropertyDateStarted);
        if (!source.Contains<string>(TestResultsConstants.TestResultPropertyDateCompleted, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          source.Add(TestResultsConstants.TestResultPropertyDateCompleted);
      }
      fieldsToFetch = source.ToList<string>();
      return TestManagementDynamicSqlBatchStatements.dynprc_QueryTestResultsByIds_UnifyingViews;
    }

    protected class FetchReleaseInformationColumns
    {
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseEnvId = new SqlColumnBinder(nameof (ReleaseEnvId));
      private SqlColumnBinder ReleaseName = new SqlColumnBinder(nameof (ReleaseName));

      internal ReleaseReference bind(SqlDataReader reader) => new ReleaseReference()
      {
        ReleaseId = this.ReleaseId.GetInt32((IDataReader) reader),
        ReleaseName = this.ReleaseName.GetString((IDataReader) reader, true),
        ReleaseEnvId = this.ReleaseEnvId.GetInt32((IDataReader) reader)
      };
    }

    protected class QueryAggregatedResultsForRelease
    {
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseName = new SqlColumnBinder(nameof (ReleaseName));
      private SqlColumnBinder ReleaseUri = new SqlColumnBinder(nameof (ReleaseUri));
      private SqlColumnBinder DefinitionId = new SqlColumnBinder("ReleaseDefId");
      private SqlColumnBinder TestOutcome = new SqlColumnBinder(nameof (TestOutcome));
      private SqlColumnBinder ResultCount = new SqlColumnBinder(nameof (ResultCount));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder RunDuration = new SqlColumnBinder(nameof (RunDuration));

      internal void bindBuildReference(
        SqlDataReader reader,
        Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap)
      {
        int int32_1 = this.ReleaseId.GetInt32((IDataReader) reader);
        this.ReleaseUri.GetString((IDataReader) reader, false);
        string str = this.ReleaseName.GetString((IDataReader) reader, true);
        int int32_2 = this.DefinitionId.GetInt32((IDataReader) reader);
        if (aggregatedResultsMap.ContainsKey(int32_1))
          return;
        aggregatedResultsMap[int32_1] = new Tuple<HashSet<int>, AggregatedDataForResultTrend>(new HashSet<int>(), new AggregatedDataForResultTrend()
        {
          TestResultsContext = new TestResultsContext()
          {
            ContextType = TestResultsContextType.Release,
            Release = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
            {
              Id = int32_1,
              Name = str,
              DefinitionId = int32_2
            }
          },
          ResultsByOutcome = (IDictionary<TestOutcome, AggregatedResultsByOutcome>) new Dictionary<TestOutcome, AggregatedResultsByOutcome>(),
          Duration = new TimeSpan()
        });
      }

      internal void bindAggregateValues(
        SqlDataReader reader,
        Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap)
      {
        int int32_1 = this.ReleaseId.GetInt32((IDataReader) reader);
        TestOutcome key = (TestOutcome) this.TestOutcome.GetByte((IDataReader) reader);
        int int32_2 = this.ResultCount.GetInt32((IDataReader) reader);
        int int32_3 = this.TestRunId.GetInt32((IDataReader) reader);
        long int64 = Convert.ToInt64(this.RunDuration.GetObject((IDataReader) reader));
        if (!aggregatedResultsMap.ContainsKey(int32_1))
          return;
        if (!aggregatedResultsMap[int32_1].Item2.ResultsByOutcome.ContainsKey(key))
          aggregatedResultsMap[int32_1].Item2.ResultsByOutcome[key] = new AggregatedResultsByOutcome()
          {
            Count = 0,
            Outcome = key
          };
        aggregatedResultsMap[int32_1].Item2.ResultsByOutcome[key].Count += int32_2;
        if (aggregatedResultsMap[int32_1].Item1.Contains(int32_3))
          return;
        aggregatedResultsMap[int32_1].Item1.Add(int32_3);
        aggregatedResultsMap[int32_1].Item2.Duration = Validator.CheckOverflowAndGetSafeValue(aggregatedResultsMap[int32_1].Item2.Duration, TimeSpan.FromMilliseconds((double) int64));
      }
    }
  }
}
