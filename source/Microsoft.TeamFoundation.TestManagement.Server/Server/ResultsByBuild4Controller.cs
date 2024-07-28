// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultsByBuild4Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ResultsByBuild", ResourceVersion = 1)]
  [DemandFeature("402E4502-9389-420C-BA11-796CDA2E4867", true)]
  public class ResultsByBuild4Controller : TestManagementController
  {
    private ResultsHelper _resultsHelper;

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<ShallowTestCaseResult>), null, null)]
    [FeatureEnabled("TestManagement.Server.TestResultsWithPagination")]
    [ClientLocationId("3C191B88-615B-4BE2-B7D9-5FF9141E91D4")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetTestResultsByBuild(
      int buildId,
      string publishContext = "CI",
      [ClientParameterAsIEnumerable(typeof (TestOutcome), ',')] string outcomes = "",
      [FromUri(Name = "$top")] int top = 10000,
      string continuationToken = null)
    {
      ArgumentUtility.CheckForNull<string>(outcomes, nameof (outcomes));
      IList<ShallowTestCaseResult> collection = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? (IList<ShallowTestCaseResult>) TestManagementController.InvokeAction<PagedList<ShallowTestCaseResult>>((Func<PagedList<ShallowTestCaseResult>>) (() => this.TestResultsHttpClient.GetTestResultsByBuildAsync(this.ProjectInfo.Id, buildId, publishContext, (IEnumerable<TestOutcome>) ParsingHelper.ParseOutcomes(outcomes), new int?(top), continuationToken, (object) null, new CancellationToken())?.Result)) : this.ResultsHelper.GetTestResultsByBuild(this.ProjectInfo, buildId, publishContext, ParsingHelper.ParseOutcomes(outcomes), top, continuationToken);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult secureObject = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = this.ProjectInfo.Id.ToString()
        }
      };
      HttpResponseMessage response = this.GenerateResponse<ShallowTestCaseResult>((IEnumerable<ShallowTestCaseResult>) collection, (ISecuredObject) secureObject);
      if (collection != null && collection.Count == top)
        Utils.SetContinuationToken(response, collection[collection.Count - 1].RunId.ToString() + "_" + collection[collection.Count - 1].Id.ToString());
      return response;
    }

    internal ResultsHelper ResultsHelper
    {
      get => this._resultsHelper ?? new ResultsHelper(this.TestManagementRequestContext);
      set => this._resultsHelper = value;
    }
  }
}
