// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Runs1Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Runs", ResourceVersion = 1)]
  public class Runs1Controller : TestResultsControllerBase
  {
    private RunsHelper m_runsHelper;
    private StatisticsHelper m_statisticsHelper;
    private BuildCoverageHelper m_buildCoverageHelper;
    private ITeamFoundationTestManagementRunService m_testManagementRunService;

    internal RunsHelper RunsHelper
    {
      get
      {
        if (this.m_runsHelper == null)
          this.m_runsHelper = (RunsHelper) new TfsRunsHelper(this.TestManagementRequestContext);
        return this.m_runsHelper;
      }
    }

    internal StatisticsHelper StatisticsHelper
    {
      get
      {
        if (this.m_statisticsHelper == null)
          this.m_statisticsHelper = new StatisticsHelper(this.TestManagementRequestContext);
        return this.m_statisticsHelper;
      }
    }

    internal BuildCoverageHelper BuildCoverageHelper
    {
      get
      {
        if (this.m_buildCoverageHelper == null)
          this.m_buildCoverageHelper = new BuildCoverageHelper(this.TestManagementRequestContext);
        return this.m_buildCoverageHelper;
      }
    }

    internal ITeamFoundationTestManagementRunService TestManagementRunService
    {
      get
      {
        if (this.m_testManagementRunService == null)
          this.m_testManagementRunService = this.TfsRequestContext.GetService<ITeamFoundationTestManagementRunService>();
        return this.m_testManagementRunService;
      }
    }

    internal TcmHttpClient TcmHttpClient => this.TestManagementRequestContext.RequestContext.GetClient<TcmHttpClient>();

    [HttpGet]
    [ActionName("Runs")]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>), null, null)]
    [ClientLocationId("DEDD48A7-82F6-48AC-86E8-3E0A1D99D785")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GET__test__projectName__runs.json", "General example", null, null)]
    [ClientExample("GET__test__projectName__runs_top-3.json", "Get top 3 test runs", null, null)]
    [ClientExample("GET__test__projectName__runs_includeRunDetails-true.json", "Get runs with details", null, null)]
    public HttpResponseMessage GetTestRuns(
      string projectId,
      string buildUri = "",
      string owner = "",
      string tmiRunId = "",
      int planId = -1,
      bool includeRunDetails = false,
      bool? automated = null,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647)
    {
      return this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) this.GetTestRunsInternal(projectId, buildUri, owner, tmiRunId, planId, includeRunDetails, automated, skip, top));
    }

    [HttpGet]
    [ActionName("Runs")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("DEDD48A7-82F6-48AC-86E8-3E0A1D99D785")]
    [ClientExample("GET__test__projectName__runs__runId_.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun GetTestRunById(
      string projectId,
      int runId)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.RunsHelper.GetTestRunById(projectId, runId, true);
      this.RunsHelper.CheckForViewTestResultPermission(this.ProjectName);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun = TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() => this.TestResultsHttpClient.GetTestRunByIdAsync(this.ProjectId.ToString(), runId, new bool?(true), new bool?(), (object) null, new CancellationToken())?.Result));
      int result;
      if (testRun.Plan != null && int.TryParse(testRun.Plan.Id, out result) && result > 0)
      {
        testRun.Iteration = this.TestManagementRequestContext.PlannedTestResultsHelper.GetTestPlanIteration(this.TestManagementRequestContext, this.ProjectName, result);
        if (string.IsNullOrEmpty(testRun.Plan.Name))
          testRun.Plan = this.TestManagementRequestContext.PlannedTestResultsHelper.GetShallowTestPlan(this.TestManagementRequestContext, this.ProjectName, result);
      }
      new TfsTestRunDataContractConverter(this.TestManagementRequestContext).SecureTestRunWebApiObject(testRun, this.ProjectId);
      return testRun;
    }

    [HttpDelete]
    [ActionName("Runs")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("DEDD48A7-82F6-48AC-86E8-3E0A1D99D785")]
    [ClientExample("DELETE__test__projectName__runs__newRunId_.json", null, null, null)]
    public void DeleteTestRun(string projectId, int runId)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
      {
        this.RunsHelper.DeleteTestRun(projectId, runId);
      }
      else
      {
        try
        {
          this.TestResultsHttpClient.DeleteTestRunAsync(projectId, runId)?.Wait();
        }
        catch (AggregateException ex)
        {
          if (ex.InnerException != null)
            throw ex.InnerException;
          throw;
        }
      }
    }

    [HttpPatch]
    [ActionName("Runs")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("DEDD48A7-82F6-48AC-86E8-3E0A1D99D785")]
    [ClientExample("PATCH__test__projectName__runs__runId_.json", "General example", null, null)]
    [ClientExample("PATCH__test__projectName__runs__comment.json", "Updating run comment", null, null)]
    [ClientExample("PATCH__test__projectName__runs__duedate.json", "Updating due date", null, null)]
    [ClientExample("PATCH__test__projectName__runs__startedDate.json", "Updating started date", null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun PatchTestRun(
      string projectId,
      int runId,
      RunUpdateModel runUpdateModel)
    {
      return !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? this.RunsHelper.PatchTestRun(projectId, runId, runUpdateModel) : TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() => this.TestResultsHttpClient.UpdateTestRunAsync(runUpdateModel, projectId, runId, (object) null, new CancellationToken())?.Result));
    }

    [HttpPost]
    [ActionName("Runs")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("DEDD48A7-82F6-48AC-86E8-3E0A1D99D785")]
    [ClientExample("POST__test__projectName__runs.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun CreateTestRun(
      string projectId,
      RunCreateModel testRun)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.RunsHelper.CreateTestRun(projectId, testRun);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun newTestRun = TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() => this.TestResultsHttpClient.CreateTestRunAsync(testRun, projectId, (object) null, new CancellationToken())?.Result));
      this.PlannedTestResultsHelper.PopulateAndCreatePlannedResults(this.TestManagementRequestContext, this.ProjectId, this.ProjectName, testRun, newTestRun);
      return newTestRun;
    }

    [HttpGet]
    [ActionName("Statistics")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("3B3ADAD0-61FB-462A-B906-C13D1B33D1FA")]
    [ClientExample("GET_run_statistics.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic GetTestRunStatistics(
      string projectId,
      int runId)
    {
      return !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? this.StatisticsHelper.GetTestRunStatisticsForRunId(projectId, runId) : TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic>) (() => this.TestResultsHttpClient.GetTestRunStatisticsAsync(projectId, runId, (object) null, new CancellationToken())?.Result));
    }

    [HttpGet]
    [ActionName("Coverage")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>), null, null)]
    [ClientLocationId("AC160FA4-78A2-4E25-87C2-73A0AFE8F5D7")]
    public HttpResponseMessage GetBuildCoverage(
      string projectId,
      int runId,
      string buildUri,
      int flags)
    {
      return this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>) this.BuildCoverageHelper.GetBuildCoverage(projectId, runId, buildUri, (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) flags));
    }

    protected List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> GetTestRunsInternal(
      string projectId,
      string buildUri,
      string owner,
      string tmiRunId,
      int planId,
      bool includeRunDetails,
      bool? automated,
      int skip,
      int top)
    {
      return !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? this.RunsHelper.GetTestRuns(projectId, buildUri, owner, tmiRunId, planId, includeRunDetails, automated, skip, top) : TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>) (() => this.TestResultsHttpClient.GetTestRunsAsync(projectId, buildUri, owner, tmiRunId, new int?(planId), new bool?(includeRunDetails), automated, new int?(skip), new int?(top))?.Result));
    }
  }
}
