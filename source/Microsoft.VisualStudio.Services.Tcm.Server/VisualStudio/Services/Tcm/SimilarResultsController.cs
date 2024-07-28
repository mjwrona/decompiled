// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.SimilarResultsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "SimilarTestResults", ResourceVersion = 1)]
  public class SimilarResultsController : TcmControllerBase
  {
    private const string c_continuationTokenHeaderName = "x-ms-continuationtoken";
    private const string c_continuationTokenParamName = "continuationToken";
    private const string c_continuationTokenDescription = "Header to pass the continuationToken";

    [HttpGet]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>), null, null)]
    [ClientLocationId("67d0a074-b255-4902-a639-e3e6de7a3de6")]
    [FeatureEnabled("TestManagement.Server.GroupSimilarTestResultsFeature")]
    [ClientHeaderParameter("x-ms-continuationtoken", typeof (string), "continuationToken", "Header to pass the continuationToken", true, false)]
    public HttpResponseMessage GetSimilarTestResults(
      int runId,
      int testResultId,
      int testSubResultId,
      [FromUri(Name = "$top")] int top = 10000)
    {
      string headerValue = this.GetHeaderValue(this.Request?.Headers, "x-ms-continuationtoken");
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> similarTestCaseResults = this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementResultService>().GetSimilarTestCaseResults(this.TestManagementRequestContext, this.ProjectInfo, runId, testResultId, testSubResultId, top, headerValue);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult secureObject = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = this.ProjectInfo.Id.ToString()
        }
      };
      HttpResponseMessage response = this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) similarTestCaseResults, (ISecuredObject) secureObject);
      if (similarTestCaseResults != null && similarTestCaseResults.Count == top)
        Utils.SetContinuationToken(response, similarTestCaseResults[similarTestCaseResults.Count - 1].TestRun.Id + "_" + similarTestCaseResults[similarTestCaseResults.Count - 1].Id.ToString());
      return response;
    }
  }
}
