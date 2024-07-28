// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultDetailsForBuildController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ResultDetailsByBuild", ResourceVersion = 1)]
  public class ResultDetailsForBuildController : TestResultsControllerBase
  {
    private ResultsHelper _resultsHelper;

    [HttpGet]
    [DemandFeature("402E4502-9389-420C-BA11-796CDA2E4867", true)]
    [PublicProjectRequestRestrictions]
    public TestResultsDetails GetTestResultDetailsForBuild(
      int buildId,
      string publishContext = "",
      string groupBy = null,
      [FromUri(Name = "$filter")] string filter = null,
      [FromUri(Name = "$orderby")] string orderBy = null,
      bool shouldIncludeResults = true,
      bool queryRunSummaryForInProgress = false)
    {
      TestResultsDetails resultDetailsForBuild = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? TestManagementController.InvokeAction<TestResultsDetails>((Func<TestResultsDetails>) (() => this.TestResultsHttpClient.GetTestResultDetailsForBuildAsync(this.ProjectId, buildId, publishContext, groupBy, filter, orderBy, new bool?(shouldIncludeResults), new bool?(queryRunSummaryForInProgress))?.Result)) : this.ResultsHelper.GetTestResultDetailsForBuild(this.ProjectId.ToString(), buildId, publishContext, groupBy, filter, orderBy, shouldIncludeResults, queryRunSummaryForInProgress);
      if (resultDetailsForBuild != null && resultDetailsForBuild.ResultsForGroup != null)
      {
        foreach (TestResultsDetailsForGroup resultsDetailsForGroup in (IEnumerable<TestResultsDetailsForGroup>) resultDetailsForBuild.ResultsForGroup)
          resultsDetailsForGroup.Tags = (string[]) null;
      }
      return resultDetailsForBuild;
    }

    internal ResultsHelper ResultsHelper
    {
      get => this._resultsHelper ?? new ResultsHelper(this.TestManagementRequestContext);
      set => this._resultsHelper = value;
    }
  }
}
