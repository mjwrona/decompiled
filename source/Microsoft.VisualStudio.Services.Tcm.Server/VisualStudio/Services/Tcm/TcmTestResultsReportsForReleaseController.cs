// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TcmTestResultsReportsForReleaseController
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
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "resultsummarybyrelease", ResourceVersion = 1)]
  public class TcmTestResultsReportsForReleaseController : TcmControllerBase
  {
    private ITestReportsHelper _testReportsHelper;

    [HttpGet]
    [ClientLocationId("F10F9577-2C04-45AB-8C99-B26567A7CD55")]
    [PublicProjectRequestRestrictions]
    public TestResultSummary QueryTestResultsReportForRelease(
      int releaseId,
      int releaseEnvId,
      string publishContext = "",
      bool includeFailureDetails = false,
      [FromUri(Name = "releaseToCompare")] Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseToCompare = null)
    {
      ITestReportsHelper testReportsHelper = this.TestReportsHelper;
      GuidAndString projectId = new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id);
      Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference release = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference();
      release.Id = releaseId;
      release.EnvironmentId = releaseEnvId;
      string sourceWorkflow = publishContext;
      Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseToCompare1 = releaseToCompare;
      int num = includeFailureDetails ? 1 : 0;
      return testReportsHelper.QueryTestReportForRelease(projectId, release, sourceWorkflow, releaseToCompare1, true, num != 0);
    }

    [HttpPost]
    [ClientLocationId("F10F9577-2C04-45AB-8C99-B26567A7CD55")]
    [PublicProjectRequestRestrictions]
    public List<TestResultSummary> QueryTestResultsSummaryForReleases(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference> releases)
    {
      return this.TestReportsHelper.QueryTestSummaryForReleases(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), releases);
    }

    internal ITestReportsHelper TestReportsHelper
    {
      get => this._testReportsHelper ?? (ITestReportsHelper) new Microsoft.TeamFoundation.TestManagement.Server.TestReportsHelper(this.TestManagementRequestContext);
      set => this._testReportsHelper = value;
    }
  }
}
