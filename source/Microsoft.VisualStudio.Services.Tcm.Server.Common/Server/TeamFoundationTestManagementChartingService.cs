// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementChartingService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Charting;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TeamFoundationTestManagementChartingService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestManagementChartingService,
    IVssFrameworkService
  {
    public TeamFoundationTestManagementChartingService()
    {
    }

    public TeamFoundationTestManagementChartingService(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestExecutionReportData GetTestRunSummaryReport(
      TestManagementRequestContext requestContext,
      Guid projectId,
      int runId,
      List<string> dimensionList)
    {
      ChartingHelper.ValidateDimensions(dimensionList, "runsummary");
      Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData runSummaryReport2;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
        runSummaryReport2 = managementDatabase.GetTestRunSummaryReport2(projectId, runId, dimensionList);
      return runSummaryReport2?.ToWebApiModel();
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestExecutionReportData GetTestExecutionSummaryReport(
      TestManagementRequestContext requestContext,
      Guid projectId,
      int planId,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails> testAuthoringDetails,
      List<string> dimensionList)
    {
      ChartingHelper.ValidateDimensions(dimensionList, "execution");
      Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData executionReportData;
      if (requestContext.IsFeatureEnabled("TestManagement.Server.BypassPointResultDetails"))
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
          executionReportData = managementDatabase.GetTestExecutionReport3(projectId, planId, testAuthoringDetails, dimensionList);
      }
      else
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
          executionReportData = managementDatabase.GetTestExecutionReport2(projectId, planId, testAuthoringDetails, dimensionList);
      }
      return executionReportData?.ToWebApiModel();
    }
  }
}
