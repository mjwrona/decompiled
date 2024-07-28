// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Results1Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Results", ResourceVersion = 1)]
  public class Results1Controller : TestResultsControllerBase
  {
    private ResultsHelper m_resultsHelper;

    [HttpGet]
    [ActionName("Results")]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>), null, null)]
    [ClientLocationId("271C7B73-C3F9-4022-8AD6-AA53B600AFF9")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public HttpResponseMessage GetTestCaseResults(
      string projectId,
      int runId,
      bool includeIterationDetails = false)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.ResultsHelper.GetTestResults(projectId, runId, includeIterationDetails, true, true, (IList<TestOutcome>) null, 0, int.MaxValue));
      this.ResultsHelper.CheckForViewTestResultPermission(this.ProjectName);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.GetTestResultsAsync(this.ProjectId.ToString(), runId, new ResultDetails?(ResultDetails.Iterations | ResultDetails.WorkItems | ResultDetails.Point), new int?(0), new int?(int.MaxValue), (IEnumerable<TestOutcome>) null, new bool?(), (object) null, new CancellationToken())?.Result));
      this.TestManagementRequestContext.PlannedTestResultsHelper.PopulateTestSuiteDetails(testCaseResultList, this.ProjectName);
      testCaseResultList.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (r => this.ResultsHelper.SecureTestResultWebApiObject(r)));
      return this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testCaseResultList);
    }

    [HttpGet]
    [ActionName("Results")]
    [ClientLocationId("271C7B73-C3F9-4022-8AD6-AA53B600AFF9")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult GetTestCaseResultById(
      string projectId,
      int runId,
      int testCaseResultId,
      bool includeIterationDetails = false,
      bool includeAssociatedBugs = false)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.ResultsHelper.GetTestResultById(this.ProjectId.ToString(), runId, testCaseResultId, includeIterationDetails, includeAssociatedBugs);
      this.ResultsHelper.CheckForViewTestResultPermission(this.ProjectName);
      ResultDetails detailsToInclude = RestApiHelper.IncludeVariableToResultDetails(includeIterationDetails, includeAssociatedBugs, false);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testResult = TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (() => this.TestResultsHttpClient.GetTestResultByIdAsync(this.ProjectId.ToString(), runId, testCaseResultId, new ResultDetails?(detailsToInclude))?.Result));
      IPlannedTestResultsHelper testResultsHelper = this.TestManagementRequestContext.PlannedTestResultsHelper;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResults = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      testCaseResults.Add(testResult);
      string projectName = this.ProjectName;
      testResultsHelper.PopulateTestSuiteDetails(testCaseResults, projectName);
      this.ResultsHelper.SecureTestResultWebApiObject(testResult);
      return testResult;
    }

    [HttpGet]
    [ActionName("Attachments")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("A6B80CCB-AF66-4F6E-AE20-BE845CEA3458")]
    public List<TestCaseResultAttachmentModel> GetTestResultAttachments(
      string projectId,
      int runId,
      int testCaseResultId,
      int iterationId)
    {
      return this.ResultsHelper.GetTestResultAttachments(projectId, runId, testCaseResultId, iterationId);
    }

    [HttpPost]
    [ActionName("Results")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("271C7B73-C3F9-4022-8AD6-AA53B600AFF9")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> AddTestResultsToTestRun(
      string projectId,
      int runId,
      TestResultCreateModel[] resultCreateModels)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.ResultsHelper.AddTestResultsToTestRun(this.ProjectId.ToString(), runId, resultCreateModels);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results = this.ResultsHelper.ConvertResultCreateModelToTestResults(resultCreateModels);
      if (this.ResultsHelper.IsPlannedTestRun(results))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun = TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) (() => this.TestResultsHttpClient.GetTestRunByIdAsync(this.ProjectId, runId, new bool?(true), new bool?(), (object) null, new CancellationToken())?.Result));
        int result;
        if (testRun != null && testRun.Plan != null && testRun.Plan.Id != null && int.TryParse(testRun.Plan.Id, out result) && result > 0)
          results = this.TestManagementRequestContext.WorkItemFieldDataHelper.PopulateTestResultFromWorkItem(this.TestManagementRequestContext, this.ProjectName, results, result);
      }
      return TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.AddTestResultsToTestRunAsync(results, this.ProjectId, runId, (object) null, new CancellationToken())?.Result));
    }

    [HttpPatch]
    [ActionName("Results")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("271C7B73-C3F9-4022-8AD6-AA53B600AFF9")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> UpdateTestResults(
      string projectId,
      int runId,
      TestCaseResultUpdateModel[] resultUpdateModels)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.ResultsHelper.UpdateTestResults(this.ProjectId.ToString(), runId, resultUpdateModels);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results = this.ResultsHelper.ConvertResultUpdateModelToTestResults(resultUpdateModels);
      return TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.UpdateTestResultsAsync(results, this.ProjectId, runId, (object) null, new CancellationToken())?.Result));
    }

    [HttpPatch]
    [ActionName("Results")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("271C7B73-C3F9-4022-8AD6-AA53B600AFF9")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> BulkUpdateTestResults(
      string projectId,
      int runId,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string resultIds,
      TestCaseResultUpdateModel resultUpdateModel)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.ResultsHelper.BulkUpdateTestResults(this.ProjectId.ToString(), runId, resultIds, resultUpdateModel);
      ArgumentUtility.CheckForNull<TestCaseResultUpdateModel>(resultUpdateModel, nameof (resultUpdateModel), "Test Results");
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
      ArgumentUtility.CheckStringForNullOrEmpty(resultIds, nameof (resultIds), "Test Results");
      int[] resultIdsFromString = TestManagementServiceUtility.GetDistinctTestResultIdsFromString(resultIds);
      if (resultIdsFromString != null && resultIdsFromString.Length > 200)
        throw new InvalidPropertyException(nameof (resultIds), string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.BulkUpdateResultApiMaxLimitError, (object) 200));
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] testResults = this.ResultsHelper.ConvertResultUpdateModelToTestResults(new TestCaseResultUpdateModel[1]
      {
        resultUpdateModel
      });
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = testResults[0];
      if ((testCaseResult != null ? (testCaseResult.Id > 0 ? 1 : 0) : 0) != 0)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultIdSpecifiedInBulkUpdateTestResults));
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultUpdateModels = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(resultIdsFromString.Length);
      foreach (int num in resultIdsFromString)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult copyOf = TfsRestApiHelper.CreateCopyOf(testResults[0]);
        copyOf.Id = num;
        resultUpdateModels.Add(copyOf);
      }
      return TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.UpdateTestResultsAsync(resultUpdateModels.ToArray(), this.ProjectId, runId, (object) null, new CancellationToken())?.Result));
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
