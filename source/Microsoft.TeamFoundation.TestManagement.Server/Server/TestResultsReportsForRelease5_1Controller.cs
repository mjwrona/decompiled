// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultsReportsForRelease5_1Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ResultSummaryByRelease", ResourceVersion = 3)]
  [DemandFeature("D104EA57-16EA-4191-9B60-160D664EE9A8", true)]
  public class TestResultsReportsForRelease5_1Controller : TestResultsControllerBase
  {
    private ITestReportsHelper _testReportsHelper;

    [HttpGet]
    [ClientLocationId("85765790-AC68-494E-B268-AF36C3929744")]
    [PublicProjectRequestRestrictions]
    public TestResultSummary QueryTestResultsReportForRelease(
      int releaseId,
      int releaseEnvId,
      string publishContext = "",
      bool includeFailureDetails = false,
      [FromUri(Name = "releaseToCompare")] Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseToCompare = null)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
      {
        ITestReportsHelper testReportsHelper = this.TestReportsHelper;
        GuidAndString projectId = new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id);
        Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference release = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference();
        release.Id = releaseId;
        release.EnvironmentId = releaseEnvId;
        string sourceWorkflow = publishContext;
        Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseToCompare1 = releaseToCompare;
        int num = includeFailureDetails ? 1 : 0;
        return testReportsHelper.QueryTestReportForRelease(projectId, release, sourceWorkflow, releaseToCompare1, true, num != 0);
      }
      TestResultSummary summary = TestManagementController.InvokeAction<TestResultSummary>((Func<TestResultSummary>) (() => this.TestResultsHttpClient.QueryTestResultsReportForReleaseAsync(this.ProjectId.ToString(), releaseId, releaseEnvId, publishContext, new bool?(includeFailureDetails), releaseToCompare)?.Result));
      this.TestReportsHelper.SecureTestResultSummary(summary);
      return summary;
    }

    [HttpPost]
    [ClientLocationId("85765790-AC68-494E-B268-AF36C3929744")]
    [PublicProjectRequestRestrictions]
    public List<TestResultSummary> QueryTestResultsSummaryForReleases(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference> releases)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.TestReportsHelper.QueryTestSummaryForReleases(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), releases);
      List<TestResultSummary> testResultSummaryList = TestManagementController.InvokeAction<List<TestResultSummary>>((Func<List<TestResultSummary>>) (() => this.TestResultsHttpClient.QueryTestResultsSummaryForReleasesAsync((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference>) releases, this.ProjectId)?.Result));
      if (testResultSummaryList == null)
        return testResultSummaryList;
      testResultSummaryList.ForEach((Action<TestResultSummary>) (summary => this.TestReportsHelper.SecureTestResultSummary(summary)));
      return testResultSummaryList;
    }

    internal ITestReportsHelper TestReportsHelper
    {
      get => this._testReportsHelper ?? (ITestReportsHelper) new Microsoft.TeamFoundation.TestManagement.Server.TestReportsHelper(this.TestManagementRequestContext);
      set => this._testReportsHelper = value;
    }
  }
}
