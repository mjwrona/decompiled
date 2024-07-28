// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Results4Controller
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
using System.Threading;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Results", ResourceVersion = 4)]
  [DemandFeature("402E4502-9389-420C-BA11-796CDA2E4867", true)]
  public class Results4Controller : TestResultsControllerBase
  {
    private ResultsHelper m_resultsHelper;

    [HttpGet]
    [ClientLocationId("4637D869-3A76-4468-8057-0BB02AA385CF")]
    [ClientExample("GET__test_runs__runId__results_3_0.json", "Get a list of test results", null, null)]
    [ClientExample("GET__test_runs__runId__results_detailsToInclude-WorkItems__top-100_3_0.json", "With workitem details", null, null)]
    [ClientExample("GET__test_runs_31_results__top-100_detailsToInclude-WorkItems,Iterations.json", "With test iterations and workitem details", null, null)]
    public IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetTestResults(
      int runId,
      ResultDetails detailsToInclude = ResultDetails.None,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int? top = null,
      [ClientParameterAsIEnumerable(typeof (TestOutcome), ',')] string outcomes = "")
    {
      bool includeAssociatedWorkItems = (detailsToInclude & ResultDetails.WorkItems) == ResultDetails.WorkItems;
      bool includeIterationDetails = (detailsToInclude & ResultDetails.Iterations) == ResultDetails.Iterations;
      bool includePoint = (detailsToInclude & ResultDetails.Point) == ResultDetails.Point;
      int validatedTop = this.ResultsHelper.ValidateAndSetMaxPageSizeForRunArtifacts(top, includeAssociatedWorkItems | includeIterationDetails, nameof (top), 1000);
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.ResultsHelper.GetTestResults(this.ProjectId.ToString(), runId, includeIterationDetails, includeAssociatedWorkItems, includePoint, (IList<TestOutcome>) this.GetListOfOutcome(outcomes), skip, validatedTop, detailsToInclude, true);
      this.ResultsHelper.CheckForViewTestResultPermission(this.ProjectName);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testResults = TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.GetTestResultsAsync(this.ProjectId.ToString(), runId, new ResultDetails?(detailsToInclude), new int?(skip), new int?(validatedTop), (IEnumerable<TestOutcome>) this.GetListOfOutcome(outcomes), new bool?(), (object) null, new CancellationToken())?.Result));
      if (includePoint)
        this.TestManagementRequestContext.PlannedTestResultsHelper.PopulateTestSuiteDetails(testResults, this.ProjectName);
      TfsRestApiHelper.UpdateConfigurgationNameForResults(this.TestManagementRequestContext, testResults, this.ProjectName);
      testResults.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (r => this.ResultsHelper.SecureTestResultWebApiObject(r)));
      return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testResults;
    }

    [HttpGet]
    [ClientLocationId("4637D869-3A76-4468-8057-0BB02AA385CF")]
    [PublicProjectRequestRestrictions]
    [ClientExample("GET__test_runs__runId__results_100000_3_0.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult GetTestResultById(
      int runId,
      int testCaseResultId,
      ResultDetails detailsToInclude = ResultDetails.None)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
      {
        bool includeIterations;
        bool includeAssociatedWorkItems;
        bool includeSubResults;
        TfsRestApiHelper.ResultDetailsToIncludeVariable(detailsToInclude, out includeIterations, out includeAssociatedWorkItems, out includeSubResults);
        return this.ResultsHelper.GetTestResultById(this.ProjectId.ToString(), runId, testCaseResultId, includeIterations, includeAssociatedWorkItems, includeSubResults);
      }
      this.ResultsHelper.CheckForViewTestResultPermission(this.ProjectName);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testResult = TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (() => this.TestResultsHttpClient.GetTestResultByIdAsync(this.ProjectId.ToString(), runId, testCaseResultId, new ResultDetails?(detailsToInclude))?.Result));
      IPlannedTestResultsHelper testResultsHelper = this.TestManagementRequestContext.PlannedTestResultsHelper;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResults = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      testCaseResults.Add(testResult);
      string projectName = this.ProjectName;
      testResultsHelper.PopulateTestSuiteDetails(testCaseResults, projectName);
      this.ResultsHelper.SecureTestResultWebApiObject(testResult);
      return testResult;
    }

    [HttpPost]
    [ClientLocationId("6711DA49-8E6F-4D35-9F73-CEF7A3C81A5B")]
    [PublicProjectRequestRestrictions]
    [ClientInternalUseOnly(false)]
    public TestResultsQuery GetTestResultsByQuery(TestResultsQuery query)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.ResultsHelper.GetTestResults(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), query);
      this.ResultsHelper.CheckForViewTestResultPermission(this.ProjectName);
      query = this.TestResultsHttpClient.GetTestResultsByQueryAsync(query, this.ProjectId, (object) null, new CancellationToken())?.Result;
      this.TestManagementRequestContext.RequestContext.CheckPermissionToReadPublicIdentityInfo();
      query.InitializeSecureObject((ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = this.ProjectId.ToString()
        }
      });
      return query;
    }

    [HttpPost]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("4637D869-3A76-4468-8057-0BB02AA385CF")]
    [ClientExample("POST__test_runs__newRunId__results_3_0.json", null, null, null)]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> AddTestResultsToTestRun(
      int runId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.ResultsHelper.AddTestResultsToTestRun(this.ProjectId.ToString(), runId, results, true);
      if (this.ResultsHelper.IsPlannedTestRun(results))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRun result1 = this.TestResultsHttpClient.GetTestRunByIdAsync(this.ProjectId, runId, new bool?(true), new bool?(), (object) null, new CancellationToken())?.Result;
        int result2;
        if (result1 != null && result1.Plan != null && result1.Plan.Id != null && int.TryParse(result1.Plan.Id, out result2) && result2 > 0)
          results = this.TestManagementRequestContext.WorkItemFieldDataHelper.PopulateTestResultFromWorkItem(this.TestManagementRequestContext, this.ProjectName, results, result2);
      }
      return this.TestResultsHttpClient.AddTestResultsToTestRunAsync(results, this.ProjectId, runId, (object) null, new CancellationToken())?.Result;
    }

    [HttpPatch]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("4637D869-3A76-4468-8057-0BB02AA385CF")]
    [ClientExample("PATCH__test_runs__newRunId__results_3_0.json", null, null, null)]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> UpdateTestResults(
      int runId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results)
    {
      return !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? this.ResultsHelper.UpdateTestResults(this.ProjectId.ToString(), runId, results) : TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.UpdateTestResultsAsync(results, this.ProjectId, runId, (object) null, new CancellationToken())?.Result));
    }

    private List<TestOutcome> GetListOfOutcome(string outcomes)
    {
      List<TestOutcome> listOfOutcome = new List<TestOutcome>();
      if (outcomes != null)
      {
        foreach (string str in (IEnumerable<string>) outcomes.Split(new string[1]
        {
          ","
        }, StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>())
        {
          TestOutcome result;
          if (!Enum.TryParse<TestOutcome>(str, true, out result))
            throw new InvalidPropertyException(string.Format(ServerResources.InvalidArgument, (object) "outcome", (object) str));
          listOfOutcome.Add(result);
        }
      }
      return listOfOutcome;
    }

    internal ResultsHelper ResultsHelper
    {
      get
      {
        if (this.m_resultsHelper == null)
          this.m_resultsHelper = new ResultsHelper(this.TestManagementRequestContext);
        return this.m_resultsHelper;
      }
    }
  }
}
