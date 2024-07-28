// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.RunSummaryController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "runsummary", ResourceVersion = 1)]
  public class RunSummaryController : TcmControllerBase
  {
    [HttpGet]
    [ClientLocationId("5C6A250C-53B7-4851-990C-42A7A00C8B39")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic GetTestRunSummaryByOutcome(
      int runId)
    {
      return this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementRunService>().GetTestRunSummaryById(this.TestManagementRequestContext, runId, this.ProjectInfo);
    }
  }
}
