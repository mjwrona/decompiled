// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LegacyResultsByRelease4Controller
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Tcm;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "resultsbyrelease", ResourceVersion = 1)]
  public class LegacyResultsByRelease4Controller : TcmControllerBase
  {
    private ResultsHelper _resultsHelper;

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<ShallowTestCaseResult>), null, null)]
    [ClientLocationId("8FC27F1B-12DB-4C69-BF06-7EC026688C2D")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetTestResultsByRelease(
      int releaseId,
      int releaseEnvid = 0,
      string publishContext = "CI",
      [ClientParameterAsIEnumerable(typeof (TestOutcome), ',')] string outcomes = "",
      [FromUri(Name = "$top")] int top = 10000,
      string continuationToken = null)
    {
      IList<ShallowTestCaseResult> resultsByRelease = this.ResultsHelper.GetTestResultsByRelease(this.ProjectInfo, releaseId, releaseEnvid, publishContext, ParsingHelper.ParseOutcomes(outcomes), top, continuationToken);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult secureObject = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = this.ProjectInfo.Id.ToString()
        }
      };
      HttpResponseMessage response = this.GenerateResponse<ShallowTestCaseResult>((IEnumerable<ShallowTestCaseResult>) resultsByRelease, (ISecuredObject) secureObject);
      if (resultsByRelease != null && resultsByRelease.Count == top)
        Utils.SetContinuationToken(response, resultsByRelease[resultsByRelease.Count - 1].RunId.ToString() + "_" + resultsByRelease[resultsByRelease.Count - 1].Id.ToString());
      return response;
    }

    internal ResultsHelper ResultsHelper
    {
      get => this._resultsHelper ?? new ResultsHelper(this.TestManagementRequestContext);
      set => this._resultsHelper = value;
    }
  }
}
