// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmResultsByPipelineController
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
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "resultsbypipeline", ResourceVersion = 1)]
  public class TcmResultsByPipelineController : TcmControllerBase
  {
    private ResultsHelper _resultsHelper;
    private const string c_continuationTokenHeaderName = "x-ms-continuationtoken";
    private const string c_continuationTokenParamName = "continuationToken";
    private const string c_continuationTokenDescription = "Header to pass the continuationToken";

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<ShallowTestCaseResult>), null, null)]
    [ClientLocationId("80169DC2-30C3-4C25-84B2-DD67D7FF1F52")]
    [PublicProjectRequestRestrictions]
    [ClientHeaderParameter("x-ms-continuationtoken", typeof (string), "continuationToken", "Header to pass the continuationToken", true, false)]
    public HttpResponseMessage GetTestResultsByPipeline(
      int pipelineId,
      string stageName = "",
      string phaseName = "",
      string jobName = "",
      [ClientParameterAsIEnumerable(typeof (TestOutcome), ',')] string outcomes = "",
      [FromUri(Name = "$top")] int top = 10000)
    {
      string headerValue = this.GetHeaderValue(this.Request?.Headers, "x-ms-continuationtoken");
      IList<ShallowTestCaseResult> resultsByPipeline = this.ResultsHelper.GetTestResultsByPipeline(this.ProjectInfo, new PipelineReference()
      {
        PipelineId = pipelineId,
        StageReference = new StageReference()
        {
          StageName = stageName
        },
        PhaseReference = new PhaseReference()
        {
          PhaseName = phaseName
        },
        JobReference = new JobReference()
        {
          JobName = jobName
        }
      }, ParsingHelper.ParseOutcomes(outcomes), top, headerValue);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult secureObject = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = this.ProjectInfo.Id.ToString()
        }
      };
      HttpResponseMessage response = this.GenerateResponse<ShallowTestCaseResult>((IEnumerable<ShallowTestCaseResult>) resultsByPipeline, (ISecuredObject) secureObject);
      if (resultsByPipeline != null && resultsByPipeline.Count == top)
        Utils.SetContinuationToken(response, resultsByPipeline[resultsByPipeline.Count - 1].RunId.ToString() + "_" + resultsByPipeline[resultsByPipeline.Count - 1].Id.ToString());
      return response;
    }

    internal ResultsHelper ResultsHelper
    {
      get => this._resultsHelper ?? new ResultsHelper(this.TestManagementRequestContext);
      set => this._resultsHelper = value;
    }
  }
}
