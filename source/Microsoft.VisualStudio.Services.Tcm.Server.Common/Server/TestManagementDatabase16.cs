// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase16
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
  public class TestManagementDatabase16 : TestManagementDatabase15
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
      this.BindString("@repositoryId", buildConfigurationInfo.RepositoryId, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
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

    internal TestManagementDatabase16(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase16()
    {
    }
  }
}
