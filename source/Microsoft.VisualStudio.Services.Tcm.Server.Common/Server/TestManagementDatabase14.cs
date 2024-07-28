// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase14
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
  public class TestManagementDatabase14 : TestManagementDatabase13
  {
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
      SqlDataReader reader = this.ExecuteReader();
      Dictionary<int, TestSummaryForWorkItem> testSummaryForWorkItems = new Dictionary<int, TestSummaryForWorkItem>();
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

    internal TestManagementDatabase14(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase14()
    {
    }

    protected class FetchBuildInformationColumns
    {
      private SqlColumnBinder BuildId = new SqlColumnBinder(nameof (BuildId));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder BuildDefinitionId = new SqlColumnBinder(nameof (BuildDefinitionId));
      private SqlColumnBinder BranchName = new SqlColumnBinder(nameof (BranchName));

      internal BuildConfiguration bind(SqlDataReader reader) => new BuildConfiguration()
      {
        BuildId = this.BuildId.GetInt32((IDataReader) reader),
        BuildNumber = this.BuildNumber.GetString((IDataReader) reader, true),
        BuildDefinitionId = this.BuildDefinitionId.GetInt32((IDataReader) reader),
        BranchName = this.BranchName.GetString((IDataReader) reader, true)
      };
    }

    protected class FetchAggregatedDataByRequirementColumns
    {
      private SqlColumnBinder WorkItemId = new SqlColumnBinder(nameof (WorkItemId));
      private SqlColumnBinder TestOutcome = new SqlColumnBinder(nameof (TestOutcome));
      private SqlColumnBinder TestCount = new SqlColumnBinder(nameof (TestCount));
      private SqlColumnBinder MappedWorkItemId = new SqlColumnBinder(nameof (MappedWorkItemId));

      internal void bindAndAdd(
        SqlDataReader reader,
        Dictionary<int, TestSummaryForWorkItem> testSummaryForWorkItems)
      {
        int int32_1 = this.WorkItemId.GetInt32((IDataReader) reader);
        TestOutcome key = (TestOutcome) this.TestOutcome.GetByte((IDataReader) reader);
        AggregatedResultsByOutcome resultsByOutcome = new AggregatedResultsByOutcome();
        resultsByOutcome.Outcome = key;
        resultsByOutcome.Count = this.TestCount.GetInt32((IDataReader) reader);
        int int32_2 = !this.MappedWorkItemId.ColumnExists((IDataReader) reader) || this.MappedWorkItemId.IsNull((IDataReader) reader) ? 0 : this.MappedWorkItemId.GetInt32((IDataReader) reader);
        if (testSummaryForWorkItems.ContainsKey(int32_1) && testSummaryForWorkItems[int32_1] != null)
        {
          testSummaryForWorkItems[int32_1].Summary.ResultsByOutcome.Add(key, resultsByOutcome);
        }
        else
        {
          TestSummaryForWorkItem summaryForWorkItem = new TestSummaryForWorkItem()
          {
            WorkItem = new WorkItemReference()
            {
              Id = int32_1.ToString()
            },
            Summary = new AggregatedDataForResultTrend()
            {
              TestResultsContext = new TestResultsContext(),
              ResultsByOutcome = (IDictionary<TestOutcome, AggregatedResultsByOutcome>) new Dictionary<TestOutcome, AggregatedResultsByOutcome>()
            }
          };
          if (int32_2 > 0)
            summaryForWorkItem.Summary.ResultsByOutcome.Add(key, resultsByOutcome);
          testSummaryForWorkItems[int32_1] = summaryForWorkItem;
        }
      }
    }
  }
}
