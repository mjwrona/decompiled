// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase28
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
  public class TestManagementDatabase28 : TestManagementDatabase27
  {
    internal TestManagementDatabase28(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase28()
    {
    }

    public override List<AggregatedDataForResultTrend> QueryTestResultTrendForRelease(
      Guid projectId,
      TestResultTrendFilter filter)
    {
      string empty = string.Empty;
      string str1 = string.Empty;
      string str2 = string.Empty;
      string str3 = string.Format(SQLConstants.FilterClause_IN, (object) TestResultsConstants.ReleaseDefinitionIdColumnName, (object) "SELECT Id FROM @definitionIds");
      if (filter.TestRunTitles != null && filter.TestRunTitles.Count > 0)
        str1 = string.Format(SQLConstants.FilterClause_IN, (object) TestResultsConstants.TestRunTitleColumnName, (object) "SELECT Name FROM @runTitles");
      if (filter.EnvDefinitionIds != null && filter.EnvDefinitionIds.Count > 0)
        str2 = string.Format(SQLConstants.FilterClause_IN, (object) TestResultsConstants.ReleaseEnvDefinitionIdColumnName, (object) "SELECT Id FROM @envDefinitionIds");
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementDynamicSqlBatchStatements.dynprc_QueryTestResultTrendForRelease, !string.IsNullOrEmpty(str3) ? (object) str3 : (object) TestResultsConstants.TrueCondition, !string.IsNullOrEmpty(str1) ? (object) str1 : (object) TestResultsConstants.TrueCondition, !string.IsNullOrEmpty(str2) ? (object) str2 : (object) TestResultsConstants.TrueCondition);
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement, 0, true, true);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", filter.PublishContext, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindIdTypeTable("@definitionIds", filter.DefinitionIds != null ? filter.DefinitionIds.Distinct<int>() : (IEnumerable<int>) null);
      this.BindIdTypeTable("@envDefinitionIds", filter.EnvDefinitionIds != null ? filter.EnvDefinitionIds.Distinct<int>() : (IEnumerable<int>) null);
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

    public override List<AggregatedDataForResultTrend> QueryTestResultTrendForRelease2(
      Guid projectId,
      TestResultTrendFilter filter)
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
      TestManagementDatabase23.QueryAggregatedResultsForRelease2 resultsForRelease2 = new TestManagementDatabase23.QueryAggregatedResultsForRelease2();
      Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap = new Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          resultsForRelease2.bindAggregatedResultsForRelease(reader, aggregatedResultsMap);
      }
      return aggregatedResultsMap.Values.Select<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>((System.Func<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>) (a => a.Item2)).ToList<AggregatedDataForResultTrend>();
    }
  }
}
