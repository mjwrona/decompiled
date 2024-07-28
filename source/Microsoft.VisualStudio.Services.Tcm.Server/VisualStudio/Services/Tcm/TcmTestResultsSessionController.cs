// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TcmTestResultsSessionController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.Server.RestControllers.Helpers;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "testsession", ResourceVersion = 1)]
  public class TcmTestResultsSessionController : TcmControllerBase
  {
    private OneMRXSessionHelper m_sessionHelper;
    private ResultsHelper m_resultsHelper;

    [HttpPost]
    [ActionName("Session")]
    [ClientLocationId("531E61CE-580D-4962-8591-0B2942B6BF78")]
    [FeatureEnabled("TestManagement.Server.EnableCreateTestSessionApi")]
    public long CreateTestSession(TestResultsSession session)
    {
      try
      {
        return this.SessionHelper.CreateTestSession(this.ProjectId, session);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceError("RestLayer", "TcmTestResultsSessionController.CreateTestSession: Error: " + ex.ToString());
        throw;
      }
    }

    [HttpPost]
    [ActionName("SaveEnvironmentAndConfig")]
    [ClientLocationId("F9C2E9E4-9C9A-4C1D-9A7D-2B4C8A6F0D5F")]
    [FeatureEnabled("TestManagement.Server.EnableCreateTestSessionApi")]
    public void CreateEnvironment(IList<TestSessionEnvironment> environments)
    {
      try
      {
        this.SessionHelper.CreateConfigAndEnvironment(this.ProjectId, environments);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceError("RestLayer", "TcmTestResultsSessionController.CreateConfigAndEnvironment: Error: " + ex.ToString());
        throw;
      }
    }

    [HttpPost]
    [ClientLocationId("EE6D95BF-7506-4C47-8100-9FED82CDC2F7")]
    [FeatureEnabled("TestManagement.Server.EnableCreateTestSessionApi")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> AddTestResultsToTestRunSession(
      int runId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results)
    {
      return this.ResultsHelper.AddTestResultsToTestRun(this.ProjectId.ToString(), runId, results, testSessionProperties: true);
    }

    [HttpGet]
    [ActionName("Session")]
    [ClientLocationId("531E61CE-580D-4962-8591-0B2942B6BF78")]
    [FeatureEnabled("TestManagement.Server.EnableGetTestSessionApi")]
    public List<TestResultsSession> GetTestSession(int buildId) => this.SessionHelper.GetTestSessionByBuildId(this.ProjectId, buildId);

    [HttpGet]
    [ActionName("Session")]
    [ClientLocationId("531E61CE-580D-4962-8591-0B2942B6BF78")]
    [FeatureEnabled("TestManagement.Server.EnableGetTestSessionApi")]
    public List<Layout> GetTestSessionLayout(Guid sessionId) => this.SessionHelper.GetTestSessionLayoutBySessionId(this.ProjectId, sessionId);

    [HttpGet]
    [ClientLocationId("EE6D95BF-7506-4C47-8100-9FED82CDC2F7")]
    [FeatureEnabled("TestManagement.Server.EnableGetTestSessionApi")]
    public IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetTestSessionResults(
      int runId,
      ResultDetails detailsToInclude = ResultDetails.None,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int? top = null,
      [ClientParameterAsIEnumerable(typeof (TestOutcome), ',')] string outcomes = "",
      [FromUri(Name = "$newTestsOnly")] bool newTestsOnly = false)
    {
      bool includeAssociatedWorkItems = (detailsToInclude & ResultDetails.WorkItems) == ResultDetails.WorkItems;
      bool includeIterationDetails = (detailsToInclude & ResultDetails.Iterations) == ResultDetails.Iterations;
      bool includePoint = (detailsToInclude & ResultDetails.Point) == ResultDetails.Point;
      int top1 = this.ResultsHelper.ValidateAndSetMaxPageSizeForRunArtifacts(top, includeAssociatedWorkItems | includeIterationDetails, nameof (top), 1000);
      return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.ResultsHelper.GetTestResults(this.ProjectId.ToString(), runId, includeIterationDetails, includeAssociatedWorkItems, includePoint, (IList<TestOutcome>) Utils.GetListOfOutcome(outcomes), skip, top1, detailsToInclude, newTestsOnly: newTestsOnly, testSessionProperties: true);
    }

    internal OneMRXSessionHelper SessionHelper
    {
      get
      {
        if (this.m_sessionHelper == null)
          this.m_sessionHelper = new OneMRXSessionHelper(this.TestManagementRequestContext);
        return this.m_sessionHelper;
      }
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
