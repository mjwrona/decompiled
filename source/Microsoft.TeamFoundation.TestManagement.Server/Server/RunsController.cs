// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.RunsController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Runs", ResourceVersion = 2)]
  public class RunsController : Runs1Controller
  {
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
      return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) this.GetTestRunsInternal(this.ProjectId.ToString(), buildUri, owner, tmiRunId, planId, includeRunDetails, automated, skip, top);
    }

    [HttpGet]
    [ActionName("Runs")]
    [DemandFeature("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9", true)]
    [ClientLocationId("CADB3810-D47D-4A3C-A234-FE5F3BE50138")]
    [PublicProjectRequestRestrictions]
    [ClientExample("GET_test_run_id.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun GetTestRunById(int runId) => this.GetTestRunById(this.ProjectId.ToString(), runId);

    [HttpDelete]
    [ActionName("Runs")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("CADB3810-D47D-4A3C-A234-FE5F3BE50138")]
    [ClientExample("DELETE_test_runs_id.json", null, null, null)]
    public void DeleteTestRun(int runId) => this.DeleteTestRun(this.ProjectId.ToString(), runId);

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
      return this.PatchTestRun(this.ProjectId.ToString(), runId, runUpdateModel);
    }

    [HttpPost]
    [ActionName("Runs")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("CADB3810-D47D-4A3C-A234-FE5F3BE50138")]
    [ClientExample("POST__test__projectName__runs.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun CreateTestRun(
      RunCreateModel testRun)
    {
      return this.CreateTestRun(this.ProjectId.ToString(), testRun);
    }

    [HttpGet]
    [ActionName("Statistics")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("0A42C424-D764-4A16-A2D5-5C85F87D0AE8")]
    [ClientExample("GET_run_statistics.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic GetTestRunStatistics(
      int runId)
    {
      return this.GetTestRunStatistics(this.ProjectId.ToString(), runId);
    }

    [HttpGet]
    [ActionName("Coverage")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>), null, null)]
    [ClientLocationId("D370B94C-B134-489A-93B1-497FCB399680")]
    public HttpResponseMessage GetBuildCoverage(int runId, string buildUri, int flags) => this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>) this.BuildCoverageHelper.GetBuildCoverage(this.ProjectId.ToString(), runId, buildUri, (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) flags));
  }
}
