// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TcmResults6_0Controller
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "results", ResourceVersion = 2)]
  [AccessReadConsistencyFilter("TestManagement.Server.EnableSqlReadReplicaUsageInTcm")]
  public class TcmResults6_0Controller : TcmControllerBase
  {
    private ResultsHelper m_resultsHelper;

    [HttpGet]
    [ClientLocationId("02AFA165-E79A-4D70-8F0C-2AF0F35B4E07")]
    public IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetTestResults(
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
      return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.ResultsHelper.GetTestResults(this.ProjectId.ToString(), runId, includeIterationDetails, includeAssociatedWorkItems, includePoint, (IList<TestOutcome>) Utils.GetListOfOutcome(outcomes), skip, top1, detailsToInclude, newTestsOnly: newTestsOnly);
    }

    [HttpGet]
    [ClientLocationId("02AFA165-E79A-4D70-8F0C-2AF0F35B4E07")]
    [PublicProjectRequestRestrictions]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult GetTestResultById(
      int runId,
      int testResultId,
      ResultDetails detailsToInclude = ResultDetails.None)
    {
      bool includeAssociatedBugs = (detailsToInclude & ResultDetails.WorkItems) == ResultDetails.WorkItems;
      bool includeIterationDetails = (detailsToInclude & ResultDetails.Iterations) == ResultDetails.Iterations;
      bool includeSubResultDetails = (detailsToInclude & ResultDetails.SubResults) == ResultDetails.SubResults;
      return this.ResultsHelper.GetTestResultById(this.ProjectId.ToString(), runId, testResultId, includeIterationDetails, includeAssociatedBugs, includeSubResultDetails);
    }

    [HttpPost]
    [ClientLocationId("02AFA165-E79A-4D70-8F0C-2AF0F35B4E07")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> AddTestResultsToTestRun(
      int runId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results)
    {
      return this.ResultsHelper.AddTestResultsToTestRun(this.ProjectId.ToString(), runId, results);
    }

    [HttpPost]
    [ActionName("QueryResults")]
    [ClientLocationId("14033A2C-AF25-4AF1-9E39-8EF6900482E3")]
    [PublicProjectRequestRestrictions]
    public TestResultsQuery GetTestResultsByQuery(TestResultsQuery query) => this.ResultsHelper.GetTestResults(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), query);

    [HttpPost]
    [ActionName("QueryResultsWiql")]
    [ClientLocationId("5ea78be3-2f5a-4110-8034-c27f24c62db1")]
    public IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetTestResultsByQueryWiql(
      QueryModel queryModel,
      bool includeResultDetails = false,
      bool includeIterationDetails = false,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647)
    {
      return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.ResultsHelper.GetTestResultsByQuery(this.ProjectId.ToString(), queryModel, includeResultDetails, includeIterationDetails, skip, top);
    }

    [HttpPatch]
    [ClientLocationId("02AFA165-E79A-4D70-8F0C-2AF0F35B4E07")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> UpdateTestResults(
      int runId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results)
    {
      return this.ResultsHelper.UpdateTestResults(this.ProjectId.ToString(), runId, results);
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
