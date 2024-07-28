// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Results2Controller
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
using System.Threading;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Results", ResourceVersion = 2)]
  public class Results2Controller : ResultsController
  {
    [HttpPost]
    [ActionName("QueryResults")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("D03F4BFD-0863-441A-969F-6BBBD42443CA")]
    public IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetTestResultsByQuery(
      QueryModel query,
      bool includeResultDetails = false,
      bool includeIterationDetails = false,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.ResultsHelper.GetTestResultsByQuery(this.ProjectId.ToString(), query, includeResultDetails, includeIterationDetails, skip, top);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testResultsByQuery = TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.GetTestResultsByQueryWiqlAsync(query, this.ProjectId, new bool?(includeResultDetails), new bool?(includeIterationDetails), new int?(skip), new int?(top))?.Result));
      testResultsByQuery.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (r => this.ResultsHelper.SecureTestResultWebApiObject(r)));
      return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testResultsByQuery;
    }

    [HttpPost]
    [ActionName("Results")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("4637D869-3A76-4468-8057-0BB02AA385CF")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> AddTestResultsToTestRun(
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
    [ClientLocationId("4637D869-3A76-4468-8057-0BB02AA385CF")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> UpdateTestResults(
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
    [ClientLocationId("4637D869-3A76-4468-8057-0BB02AA385CF")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> BulkUpdateTestResults(
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
  }
}
