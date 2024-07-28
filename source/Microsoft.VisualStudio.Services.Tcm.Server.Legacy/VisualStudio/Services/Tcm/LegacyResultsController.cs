// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.LegacyResultsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "results", ResourceVersion = 1)]
  public class LegacyResultsController : TcmControllerBase
  {
    private ResultsHelper m_resultsHelper;

    [HttpGet]
    [ClientLocationId("0655A660-543A-4408-93D7-A28D79CF1560")]
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
      int top1 = this.ResultsHelper.ValidateAndSetMaxPageSizeForRunArtifacts(top, includeAssociatedWorkItems | includeIterationDetails, nameof (top), 1000);
      return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) this.ResultsHelper.GetTestResults(this.ProjectId.ToString(), runId, includeIterationDetails, includeAssociatedWorkItems, includePoint, (IList<TestOutcome>) Utils.GetListOfOutcome(outcomes), skip, top1, detailsToInclude);
    }

    [HttpGet]
    [ClientLocationId("0655A660-543A-4408-93D7-A28D79CF1560")]
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
    [ClientLocationId("0655A660-543A-4408-93D7-A28D79CF1560")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> AddTestResultsToTestRun(
      int runId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results)
    {
      return this.ResultsHelper.AddTestResultsToTestRun(this.ProjectId.ToString(), runId, results);
    }

    [HttpPost]
    [ClientLocationId("5645F3B7-C0BE-4E21-9CBB-CA91DB31A422")]
    [PublicProjectRequestRestrictions]
    public TestResultsQuery GetTestResultsByQuery(TestResultsQuery query) => this.ResultsHelper.GetTestResults(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), query);

    [HttpPatch]
    [ClientLocationId("0655A660-543A-4408-93D7-A28D79CF1560")]
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
