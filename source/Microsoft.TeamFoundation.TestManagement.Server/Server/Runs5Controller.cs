// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Runs5Controller
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
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Runs", ResourceVersion = 2)]
  public class Runs5Controller : TestResultsControllerBase
  {
    private RunsHelper m_runsHelper;
    private StatisticsHelper m_statisticsHelper;
    private BuildCoverageHelper m_buildCoverageHelper;

    [HttpGet]
    [ActionName("Runs")]
    [ClientLocationId("CADB3810-D47D-4A3C-A234-FE5F3BE50138")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [FeatureEnabled("TestManagement.Server.QueryTestRunsFilter")]
    [ClientResponseType(typeof (IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>), null, null)]
    public HttpResponseMessage QueryTestRuns(
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState? state = null,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string planIds = "",
      bool? isAutomated = null,
      TestRunPublishContext? publishContext = null,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string buildIds = "",
      [ClientParameterAsIEnumerable(typeof (int), ',')] string buildDefIds = "",
      string branchName = null,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string releaseIds = "",
      [ClientParameterAsIEnumerable(typeof (int), ',')] string releaseDefIds = "",
      [ClientParameterAsIEnumerable(typeof (int), ',')] string releaseEnvIds = "",
      [ClientParameterAsIEnumerable(typeof (int), ',')] string releaseEnvDefIds = "",
      string runTitle = null,
      [FromUri(Name = "$top")] int top = 100,
      string continuationToken = null)
    {
      string mergedContinuationToken = (string) null;
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> collection;
      if (this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
      {
        List<int> planIdList = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, planIds, nameof (planIds));
        List<int> buildIdList = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, buildIds, nameof (buildIds));
        List<int> buildDefIdList = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, buildDefIds, nameof (buildDefIds));
        List<int> releaseIdList = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, releaseIds, nameof (releaseIds));
        List<int> releaseDefIdList = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, releaseDefIds, nameof (releaseDefIds));
        List<int> releaseEnvIdList = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, releaseEnvIds, nameof (releaseEnvIds));
        List<int> releaseEnvDefIdList = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, releaseEnvDefIds, nameof (releaseEnvDefIds));
        IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> source = TestManagementController.InvokeAction<IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>((Func<IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>) (() => this.TestResultsHttpClient.QueryTestRunsAsync2(this.ProjectId.ToString(), minLastUpdatedDate, maxLastUpdatedDate, state, (IEnumerable<int>) planIdList, isAutomated, publishContext, (IEnumerable<int>) buildIdList, (IEnumerable<int>) buildDefIdList, branchName, (IEnumerable<int>) releaseIdList, (IEnumerable<int>) releaseDefIdList, (IEnumerable<int>) releaseEnvIdList, (IEnumerable<int>) releaseEnvDefIdList, runTitle, new int?(top), continuationToken, (object) null, new CancellationToken())?.Result));
        collection = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>();
        mergedContinuationToken = source.ContinuationToken;
      }
      else
        collection = this.RunsHelper.QueryTestRuns(this.ProjectId, this.Url, minLastUpdatedDate, maxLastUpdatedDate, state, planIds, isAutomated, publishContext, buildIds, buildDefIds, branchName, releaseIds, releaseDefIds, releaseEnvIds, releaseEnvDefIds, runTitle, top, continuationToken, out mergedContinuationToken);
      HttpResponseMessage response = this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) collection);
      if (collection != null && !string.IsNullOrEmpty(mergedContinuationToken))
        Utils.SetContinuationToken(response, mergedContinuationToken);
      return response;
    }

    [HttpGet]
    [ActionName("Runs")]
    [DemandFeature("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9", true)]
    [ClientLocationId("CADB3810-D47D-4A3C-A234-FE5F3BE50138")]
    [PublicProjectRequestRestrictions]
    [ClientExample("GET_test_run_id.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun GetTestRunById(
      int runId,
      bool includeDetails = true)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.RunsHelper.GetTestRunById(this.ProjectId.ToString(), runId, includeDetails);
      this.RunsHelper.CheckForViewTestResultPermission(this.ProjectName);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun = TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() => this.TestResultsHttpClient.GetTestRunByIdAsync(this.ProjectId.ToString(), runId, new bool?(includeDetails), new bool?(), (object) null, new CancellationToken())?.Result));
      int result;
      if (testRun.Plan != null && int.TryParse(testRun.Plan.Id, out result) && result > 0)
      {
        testRun.Iteration = this.TestManagementRequestContext.PlannedTestResultsHelper.GetTestPlanIteration(this.TestManagementRequestContext, this.ProjectName, result);
        if (string.IsNullOrEmpty(testRun.Plan.Name) & includeDetails)
          testRun.Plan = this.TestManagementRequestContext.PlannedTestResultsHelper.GetShallowTestPlan(this.TestManagementRequestContext, this.ProjectName, result);
      }
      new TfsTestRunDataContractConverter(this.TestManagementRequestContext).SecureTestRunWebApiObject(testRun, this.ProjectId);
      return testRun;
    }

    [HttpGet]
    [ActionName("Runs")]
    [ClientLocationId("CADB3810-D47D-4A3C-A234-FE5F3BE50138")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GET__test__projectName__runs.json", "General example", null, null)]
    [ClientExample("GET__test__projectName__runs_top-3.json", "Get top 3 test runs", null, null)]
    [ClientExample("GET__test__projectName__runs_includeRunDetails-true.json", "Get runs with details", null, null)]
    public IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> GetTestRuns(
      string buildUri = "",
      string owner = "",
      string tmiRunId = "",
      int planId = -1,
      bool includeRunDetails = false,
      bool? automated = null,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647)
    {
      return !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) this.RunsHelper.GetTestRuns(this.ProjectId.ToString(), buildUri, owner, tmiRunId, planId, includeRunDetails, automated, skip, top) : (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>) (() => this.TestResultsHttpClient.GetTestRunsAsync(this.ProjectId.ToString(), buildUri, owner, tmiRunId, new int?(planId), new bool?(includeRunDetails), automated, new int?(skip), new int?(top))?.Result));
    }

    [HttpDelete]
    [ActionName("Runs")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("CADB3810-D47D-4A3C-A234-FE5F3BE50138")]
    [ClientExample("DELETE_test_runs_id.json", null, null, null)]
    public void DeleteTestRun(int runId)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
      {
        this.RunsHelper.DeleteTestRun(this.ProjectId.ToString(), runId);
      }
      else
      {
        try
        {
          this.TestResultsHttpClient.DeleteTestRunAsync(this.ProjectId, runId)?.Wait();
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
    [ClientLocationId("CADB3810-D47D-4A3C-A234-FE5F3BE50138")]
    [ClientExample("PATCH__test__projectName__runs__runId_.json", "General example", null, null)]
    [ClientExample("PATCH__test__projectName__runs__comment.json", "Updating run comment", null, null)]
    [ClientExample("PATCH__test__projectName__runs__duedate.json", "Updating due date", null, null)]
    [ClientExample("PATCH__test__projectName__runs__startedDate.json", "Updating started date", null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun UpdateTestRun(
      int runId,
      RunUpdateModel runUpdateModel)
    {
      return !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? this.RunsHelper.PatchTestRun(this.ProjectId.ToString(), runId, runUpdateModel) : TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() => this.TestResultsHttpClient.UpdateTestRunAsync(runUpdateModel, this.ProjectId, runId, (object) null, new CancellationToken())?.Result));
    }

    [HttpPost]
    [ActionName("Runs")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("CADB3810-D47D-4A3C-A234-FE5F3BE50138")]
    [ClientExample("POST__test__projectName__runs.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun CreateTestRun(
      RunCreateModel testRun)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.RunsHelper.CreateTestRun(this.ProjectId.ToString(), testRun);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun newTestRun = TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() => this.TestResultsHttpClient.CreateTestRunAsync(testRun, this.ProjectId.ToString(), (object) null, new CancellationToken())?.Result));
      this.PlannedTestResultsHelper.PopulateAndCreatePlannedResults(this.TestManagementRequestContext, this.ProjectId, this.ProjectName, testRun, newTestRun);
      return newTestRun;
    }

    [HttpGet]
    [ActionName("Statistics")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("0A42C424-D764-4A16-A2D5-5C85F87D0AE8")]
    [ClientExample("GET_run_statistics.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic GetTestRunStatistics(
      int runId)
    {
      return !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? this.StatisticsHelper.GetTestRunStatisticsForRunId(this.ProjectId.ToString(), runId) : TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic>) (() => this.TestResultsHttpClient.GetTestRunStatisticsAsync(this.ProjectId.ToString(), runId, (object) null, new CancellationToken())?.Result));
    }

    [HttpGet]
    [ActionName("Coverage")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>), null, null)]
    [ClientLocationId("D370B94C-B134-489A-93B1-497FCB399680")]
    public HttpResponseMessage GetBuildCoverage(int runId, string buildUri, int flags) => this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>) this.BuildCoverageHelper.GetBuildCoverage(this.ProjectId.ToString(), runId, buildUri, (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) flags));

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
  }
}
