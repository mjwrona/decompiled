// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Runs4Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Runs", ResourceVersion = 2)]
  public class Runs4Controller : RunsController
  {
    [HttpGet]
    [ActionName("Runs")]
    [ClientLocationId("CADB3810-D47D-4A3C-A234-FE5F3BE50138")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [FeatureEnabled("TestManagement.Server.QueryTestRunsFilter")]
    [ClientResponseType(typeof (IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>), null, null)]
    public HttpResponseMessage QueryTestRuns(
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      TestRunState? state = null,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string planIds = "",
      bool? isAutomated = null,
      TestRunPublishContext? publishContext = null,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string buildIds = "",
      [ClientParameterAsIEnumerable(typeof (int), ',')] string buildDefIds = "",
      string branchName = null,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string releaseIds = "",
      [ClientParameterAsIEnumerable(typeof (int), ',')] string releaseDefIds = "",
      [ClientParameterAsIEnumerable(typeof (int), ',')] string releaseEnvIds = "",
      [ClientParameterAsIEnumerable(typeof (int), ',')] string releaseEnvDefIds = "",
      string runTitle = null,
      [FromUri(Name = "$top")] int top = 100,
      string continuationToken = null)
    {
      string mergedContinuationToken = (string) null;
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> collection;
      if (this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
      {
        List<int> planIdList = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, planIds, nameof (planIds));
        List<int> buildIdList = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, buildIds, nameof (buildIds));
        List<int> buildDefIdList = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, buildDefIds, nameof (buildDefIds));
        List<int> releaseIdList = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, releaseIds, nameof (releaseIds));
        List<int> releaseDefIdList = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, releaseDefIds, nameof (releaseDefIds));
        List<int> releaseEnvIdList = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, releaseEnvIds, nameof (releaseEnvIds));
        List<int> releaseEnvDefIdList = RestApiHelper.GetListOfIds(this.TestManagementRequestContext.RequestContext, releaseEnvDefIds, nameof (releaseEnvDefIds));
        IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> source = TestManagementController.InvokeAction<IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>((Func<IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>) (() => this.TestResultsHttpClient.QueryTestRunsAsync2(this.ProjectId.ToString(), minLastUpdatedDate, maxLastUpdatedDate, state, (IEnumerable<int>) planIdList, isAutomated, publishContext, (IEnumerable<int>) buildIdList, (IEnumerable<int>) buildDefIdList, branchName, (IEnumerable<int>) releaseIdList, (IEnumerable<int>) releaseDefIdList, (IEnumerable<int>) releaseEnvIdList, (IEnumerable<int>) releaseEnvDefIdList, runTitle, new int?(top), continuationToken, (object) null, new CancellationToken())?.Result));
        collection = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>();
        mergedContinuationToken = source.ContinuationToken;
      }
      else
        collection = this.RunsHelper.QueryTestRuns(this.ProjectId, this.Url, minLastUpdatedDate, maxLastUpdatedDate, state, planIds, isAutomated, publishContext, buildIds, buildDefIds, branchName, releaseIds, releaseDefIds, releaseEnvIds, releaseEnvDefIds, runTitle, top, continuationToken, out mergedContinuationToken);
      HttpResponseMessage response = this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) collection);
      if (collection != null && collection.Count == top)
        Utils.SetContinuationToken(response, mergedContinuationToken);
      return response;
    }
  }
}
