// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.LegacyResultDetailsForReleaseController
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
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "resultdetailsbyrelease", ResourceVersion = 1)]
  public class LegacyResultDetailsForReleaseController : TcmControllerBase
  {
    private ResultsHelper _resultsHelper;

    [HttpGet]
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
      return this.ResultsHelper.GetTestResultDetailsForRelease(this.ProjectId.ToString(), releaseId, releaseEnvId, publishContext, groupBy, filter, orderBy, shouldIncludeResults, queryRunSummaryForInProgress);
    }

    internal ResultsHelper ResultsHelper
    {
      get => this._resultsHelper ?? new ResultsHelper(this.TestManagementRequestContext);
      set => this._resultsHelper = value;
    }
  }
}
