// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultsReportsForBuild5_1Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ResultSummaryByBuild", ResourceVersion = 3)]
  [DemandFeature("D104EA57-16EA-4191-9B60-160D664EE9A8", true)]
  public class TestResultsReportsForBuild5_1Controller : TestResultsControllerBase
  {
    private ITestReportsHelper _testReportsHelper;

    [HttpGet]
    [ClientLocationId("000EF77B-FEA2-498D-A10D-AD1A037F559F")]
    [PublicProjectRequestRestrictions]
    public TestResultSummary QueryTestResultsReportForBuild(
      int buildId,
      string publishContext = "",
      bool includeFailureDetails = false,
      [FromUri(Name = "buildToCompare")] BuildReference buildToCompare = null)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
      {
        ITestReportsHelper testReportsHelper = this.TestReportsHelper;
        GuidAndString projectId = new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id);
        BuildReference build = new BuildReference();
        build.Id = buildId;
        string sourceWorkflow = publishContext;
        BuildReference buildToCompare1 = buildToCompare;
        int num = includeFailureDetails ? 1 : 0;
        return testReportsHelper.QueryTestReportForBuild(projectId, build, sourceWorkflow, buildToCompare1, true, num != 0);
      }
      TestResultSummary summary = TestManagementController.InvokeAction<TestResultSummary>((Func<TestResultSummary>) (() => this.TestResultsHttpClient.QueryTestResultsReportForBuildAsync(this.ProjectId.ToString(), buildId, publishContext, new bool?(includeFailureDetails), buildToCompare)?.Result));
      this.TestReportsHelper.SecureTestResultSummary(summary);
      return summary;
    }

    internal ITestReportsHelper TestReportsHelper
    {
      get => this._testReportsHelper ?? (ITestReportsHelper) new Microsoft.TeamFoundation.TestManagement.Server.TestReportsHelper(this.TestManagementRequestContext);
      set => this._testReportsHelper = value;
    }
  }
}
