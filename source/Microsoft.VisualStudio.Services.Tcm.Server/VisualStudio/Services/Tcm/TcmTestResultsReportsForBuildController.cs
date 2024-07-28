// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TcmTestResultsReportsForBuildController
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
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "resultsummarybybuild", ResourceVersion = 1)]
  public class TcmTestResultsReportsForBuildController : TcmControllerBase
  {
    private ITestReportsHelper _testReportsHelper;

    [HttpGet]
    [ClientLocationId("E009FA95-95A5-4AD4-9681-590043CE2423")]
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
