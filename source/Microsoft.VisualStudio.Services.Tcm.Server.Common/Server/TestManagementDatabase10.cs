// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase10
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase10 : TestManagementDatabase9
  {
    internal TestManagementDatabase10(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase10()
    {
    }

    public override List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> QueryTestResultTrendReport(
      Guid projectId,
      ResultsFilter filter)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultTrend");
      this.BindQueryTestResultTrendParams(projectId, filter);
      SqlDataReader reader = this.ExecuteReader();
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      TestManagementDatabase4.QueryTestResultTrendColumns resultTrendColumns = new TestManagementDatabase4.QueryTestResultTrendColumns();
      while (reader.Read())
        testCaseResultList.Add(resultTrendColumns.bind(reader));
      return testCaseResultList;
    }

    protected virtual void BindQueryTestResultTrendParams(Guid projectId, ResultsFilter filter)
    {
      int parameterValue1 = 0;
      int parameterValue2 = 0;
      if (filter.TestResultsContext.ContextType == TestResultsContextType.Build)
        parameterValue1 = filter.TestResultsContext.Build.DefinitionId;
      else if (filter.TestResultsContext.ContextType == TestResultsContextType.Release)
        parameterValue2 = filter.TestResultsContext.Release.DefinitionId;
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@automatedTestName", filter.AutomatedTestName, 256, true, SqlDbType.NVarChar);
      this.BindInt("@buildDefId", parameterValue1);
      this.BindInt("@releaseDefId", parameterValue2);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      this.BindInt("@trendDays", filter.TrendDays);
      this.BindInt("@resultsCount", filter.ResultsCount);
    }
  }
}
