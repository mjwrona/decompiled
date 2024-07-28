// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmResultsByRelease4Controller
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

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
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "resultsbyrelease", ResourceVersion = 1)]
  public class TcmResultsByRelease4Controller : TcmControllerBase
  {
    private ResultsHelper _resultsHelper;

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<ShallowTestCaseResult>), null, null)]
    [ClientLocationId("3994B949-77E5-495D-8034-EDF80D95B84E")]
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
