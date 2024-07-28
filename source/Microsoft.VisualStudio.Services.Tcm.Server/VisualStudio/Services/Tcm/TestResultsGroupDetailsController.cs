// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TestResultsGroupDetailsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "resultsgroupDetails", ResourceVersion = 1)]
  public class TestResultsGroupDetailsController : TcmControllerBase
  {
    private ITeamFoundationTestManagementResultService testManagementResultService;

    [HttpGet]
    [ClientLocationId("F903B850-06AF-4B50-A344-D7BBFB19E93B")]
    [PublicProjectRequestRestrictions]
    public TestResultsDetails TestResultsGroupDetails(
      int pipelineId,
      string stageName = "",
      string phaseName = "",
      string jobName = "",
      bool shouldIncludeFailedAndAbortedResults = false,
      bool queryGroupSummaryForInProgress = false)
    {
      return this.TestManagementResultService.GetTestResultsGroupDetails(this.TestManagementRequestContext, this.ProjectInfo, new PipelineReference()
      {
        PipelineId = pipelineId,
        StageReference = new StageReference()
        {
          StageName = stageName
        },
        PhaseReference = new PhaseReference()
        {
          PhaseName = phaseName
        },
        JobReference = new JobReference()
        {
          JobName = jobName
        }
      }, shouldIncludeFailedAndAbortedResults, queryGroupSummaryForInProgress);
    }

    internal ITeamFoundationTestManagementResultService TestManagementResultService
    {
      get
      {
        if (this.testManagementResultService == null)
          this.testManagementResultService = this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementResultService>();
        return this.testManagementResultService;
      }
    }
  }
}
