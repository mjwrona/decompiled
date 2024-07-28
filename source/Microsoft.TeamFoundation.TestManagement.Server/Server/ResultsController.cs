// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultsController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Results", ResourceVersion = 2)]
  public class ResultsController : Results1Controller
  {
    [HttpGet]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>), null, null)]
    [ClientLocationId("4637D869-3A76-4468-8057-0BB02AA385CF")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public HttpResponseMessage GetTestResults(
      int runId,
      ResultDetails detailsToInclude = ResultDetails.None,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int? top = null)
    {
      bool includeAssociatedWorkItems = (detailsToInclude & ResultDetails.WorkItems) == ResultDetails.WorkItems;
      bool includeIterationDetails = (detailsToInclude & ResultDetails.Iterations) == ResultDetails.Iterations;
      bool includePoint = (detailsToInclude & ResultDetails.Point) == ResultDetails.Point;
      int validatedTop = this.ResultsHelper.ValidateAndSetMaxPageSizeForRunArtifacts(top, includeAssociatedWorkItems | includeIterationDetails, nameof (top));
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.ResultsHelper.GetTestResults(this.ProjectId.ToString(), runId, includeIterationDetails, includeAssociatedWorkItems, includePoint, (IList<TestOutcome>) null, skip, validatedTop, detailsToInclude, true));
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.GetTestResultsAsync(this.ProjectId.ToString(), runId, new ResultDetails?(detailsToInclude), new int?(skip), validatedTop == int.MaxValue ? new int?() : new int?(validatedTop), (IEnumerable<TestOutcome>) null, new bool?(), (object) null, new CancellationToken())?.Result));
      if ((detailsToInclude & ResultDetails.Point) == ResultDetails.Point)
        this.TestManagementRequestContext.PlannedTestResultsHelper.PopulateTestSuiteDetails(testCaseResultList, this.ProjectName);
      TfsRestApiHelper.UpdateConfigurgationNameForResults(this.TestManagementRequestContext, testCaseResultList, this.ProjectName);
      testCaseResultList.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (r => this.ResultsHelper.SecureTestResultWebApiObject(r)));
      return this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testCaseResultList);
    }

    [HttpGet]
    [ClientLocationId("4637D869-3A76-4468-8057-0BB02AA385CF")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
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

    [HttpGet]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>), null, null)]
    [ClientLocationId("4637D869-3A76-4468-8057-0BB02AA385CF")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public HttpResponseMessage GetTestCaseResults(int runId, bool includeIterationDetails) => this.GetTestCaseResults(this.ProjectId.ToString(), runId, includeIterationDetails);

    [HttpGet]
    [ClientLocationId("4637D869-3A76-4468-8057-0BB02AA385CF")]
    [DemandFeature("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9", true)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult GetTestCaseResultById(
      int runId,
      int testCaseResultId,
      bool includeIterationDetails,
      bool includeAssociatedBugs = false)
    {
      return this.GetTestCaseResultById(this.ProjectId.ToString(), runId, testCaseResultId, includeIterationDetails, includeAssociatedBugs);
    }

    [HttpGet]
    [ActionName("Attachments")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("7BF39F1D-7847-4449-A3F4-87F21A5BD41D")]
    public List<TestCaseResultAttachmentModel> GetTestResultAttachments(
      int runId,
      int testCaseResultId,
      int iterationId)
    {
      return this.GetTestResultAttachments(this.ProjectId.ToString(), runId, testCaseResultId, iterationId);
    }
  }
}
