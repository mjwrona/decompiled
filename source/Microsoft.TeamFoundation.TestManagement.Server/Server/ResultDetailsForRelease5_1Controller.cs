// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultDetailsForRelease5_1Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ResultDetailsByRelease", ResourceVersion = 2)]
  public class ResultDetailsForRelease5_1Controller : TestResultsControllerBase
  {
    private ResultsHelper _resultsHelper;

    [HttpGet]
    [DemandFeature("402E4502-9389-420C-BA11-796CDA2E4867", true)]
    [PublicProjectRequestRestrictions]
    public TestResultsDetails GetTestResultDetailsForRelease(
      int releaseId,
      int releaseEnvId,
      string publishContext = "",
      string groupBy = null,
      [FromUri(Name = "$filter")] string filter = null,
      [FromUri(Name = "$orderby")] string orderBy = null,
      bool shouldIncludeResults = true,
      bool queryRunSummaryForInProgress = false)
    {
      return !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? this.ResultsHelper.GetTestResultDetailsForRelease(this.ProjectId.ToString(), releaseId, releaseEnvId, publishContext, groupBy, filter, orderBy, shouldIncludeResults, queryRunSummaryForInProgress) : TestManagementController.InvokeAction<TestResultsDetails>((Func<TestResultsDetails>) (() => this.TestResultsHttpClient.GetTestResultDetailsForReleaseAsync(this.ProjectId, releaseId, releaseEnvId, publishContext, groupBy, filter, orderBy, new bool?(shouldIncludeResults), new bool?(queryRunSummaryForInProgress))?.Result));
    }

    internal ResultsHelper ResultsHelper
    {
      get => this._resultsHelper ?? new ResultsHelper(this.TestManagementRequestContext);
      set => this._resultsHelper = value;
    }
  }
}
