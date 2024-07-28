// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultsIterations1Controller
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
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Iterations", ResourceVersion = 1)]
  public class ResultsIterations1Controller : TestResultsControllerBase
  {
    private ResultsHelper m_resultsHelper;

    [HttpGet]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("5710D5F0-D129-4E85-A830-F8EA22968964")]
    public List<TestIterationDetailsModel> GetTestIterations(
      string projectId,
      int runId,
      int testCaseResultId,
      bool includeActionResults = false)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.ResultsHelper.GetTestIterations(projectId, runId, testCaseResultId, includeActionResults);
      ResultDetails detailsToInclude = RestApiHelper.IncludeVariableToResultDetails(true, false, false);
      return TestManagementController.InvokeAction<List<TestIterationDetailsModel>>((Func<List<TestIterationDetailsModel>>) (() => this.TestResultsHttpClient.GetTestResultByIdAsync(projectId, runId, testCaseResultId, new ResultDetails?(detailsToInclude))?.Result.IterationDetails));
    }

    [HttpGet]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("5710D5F0-D129-4E85-A830-F8EA22968964")]
    public TestIterationDetailsModel GetTestIteration(
      string projectId,
      int runId,
      int testCaseResultId,
      int iterationId,
      bool includeActionResults = false)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.ResultsHelper.GetTestIteration(projectId, runId, testCaseResultId, iterationId, includeActionResults);
      ResultDetails detailsToInclude = RestApiHelper.IncludeVariableToResultDetails(true, false, false);
      foreach (TestIterationDetailsModel testIteration in TestManagementController.InvokeAction<List<TestIterationDetailsModel>>((Func<List<TestIterationDetailsModel>>) (() => this.TestResultsHttpClient.GetTestResultByIdAsync(projectId, runId, testCaseResultId, new ResultDetails?(detailsToInclude))?.Result.IterationDetails)))
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
