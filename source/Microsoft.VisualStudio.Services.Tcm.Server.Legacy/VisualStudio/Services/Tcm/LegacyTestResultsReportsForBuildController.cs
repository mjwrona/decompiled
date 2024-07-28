// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.LegacyTestResultsReportsForBuildController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "resultsummarybybuild", ResourceVersion = 1)]
  public class LegacyTestResultsReportsForBuildController : TcmControllerBase
  {
    private ITestReportsHelper _testReportsHelper;

    [HttpGet]
    [ClientLocationId("2DA0C8DF-4AB9-4F4D-9AA8-E7859D63FDF1")]
    [PublicProjectRequestRestrictions]
    public TestResultSummary QueryTestResultsReportForBuild(
      int buildId,
      string publishContext = "",
      bool includeFailureDetails = false,
      [FromUri(Name = "buildToCompare")] BuildReference buildToCompare = null)
    {
      ITestReportsHelper testReportsHelper = this.TestReportsHelper;
      GuidAndString projectId = new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id);
      BuildReference build = new BuildReference();
      build.Id = buildId;
      string sourceWorkflow = publishContext;
      BuildReference buildToCompare1 = buildToCompare;
      int num = includeFailureDetails ? 1 : 0;
      return testReportsHelper.QueryTestReportForBuild(projectId, build, sourceWorkflow, buildToCompare1, true, num != 0);
    }

    internal ITestReportsHelper TestReportsHelper
    {
      get => this._testReportsHelper ?? (ITestReportsHelper) new Microsoft.TeamFoundation.TestManagement.Server.TestReportsHelper(this.TestManagementRequestContext);
      set => this._testReportsHelper = value;
    }
  }
}
