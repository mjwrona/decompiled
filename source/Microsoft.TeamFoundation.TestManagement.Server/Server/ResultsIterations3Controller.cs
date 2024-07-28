// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultsIterations3Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Iterations", ResourceVersion = 3)]
  public class ResultsIterations3Controller : TestResultsControllerBase
  {
    private ResultsHelper m_resultsHelper;

    [HttpGet]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("73EB9074-3446-4C44-8296-2F811950FF8D")]
    [ClientExample("GET_testmanagement_iterationresults.json", null, null, null)]
    public List<TestIterationDetailsModel> GetTestIterations(
      int runId,
      int testCaseResultId,
      bool includeActionResults = false)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.ResultsHelper.GetTestIterations(this.ProjectId.ToString(), runId, testCaseResultId, includeActionResults);
      ResultDetails detailsToInclude = RestApiHelper.IncludeVariableToResultDetails(true, false, false);
      return TestManagementController.InvokeAction<List<TestIterationDetailsModel>>((Func<List<TestIterationDetailsModel>>) (() => this.TestResultsHttpClient.GetTestResultByIdAsync(this.ProjectId.ToString(), runId, testCaseResultId, new ResultDetails?(detailsToInclude))?.Result.IterationDetails));
    }

    [HttpGet]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("73EB9074-3446-4C44-8296-2F811950FF8D")]
    [ClientExample("GET__test__projectName__runs__runId__results_100000_iterations_1.json", "Get test Iteration with id", null, null)]
    [ClientExample("GET__test__projectName__runs__runId__results_100000_iterations_1_includeActionResults-true.json", "Get test Iteration With Action Result", null, null)]
    public TestIterationDetailsModel GetTestIteration(
      int runId,
      int testCaseResultId,
      int iterationId,
      bool includeActionResults = false)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.ResultsHelper.GetTestIteration(this.ProjectId.ToString(), runId, testCaseResultId, iterationId, includeActionResults);
      ResultDetails detailsToInclude = RestApiHelper.IncludeVariableToResultDetails(true, false, false);
      foreach (TestIterationDetailsModel testIteration in TestManagementController.InvokeAction<List<TestIterationDetailsModel>>((Func<List<TestIterationDetailsModel>>) (() => this.TestResultsHttpClient.GetTestResultByIdAsync(this.ProjectId.ToString(), runId, testCaseResultId, new ResultDetails?(detailsToInclude))?.Result.IterationDetails)))
      {
        if (testIteration.Id == iterationId)
          return testIteration;
      }
      return new TestIterationDetailsModel();
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
