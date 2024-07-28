// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TcmRuns5Controller
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "runs", ResourceVersion = 1)]
  public class TcmRuns5Controller : TcmControllerBase
  {
    private RunsHelper m_runsHelper;

    [HttpGet]
    [ClientLocationId("364538F9-8062-4CE0-B024-75A0FB463F0D")]
    public IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> GetTestRuns(
      string buildUri = "",
      string owner = "",
      string tmiRunId = "",
      int planId = -1,
      bool includeRunDetails = false,
      bool? automated = null,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647)
    {
      return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) this.RunsHelper.GetTestRuns(this.ProjectId.ToString(), buildUri, owner, tmiRunId, planId, includeRunDetails, automated, skip, top);
    }

    [HttpGet]
    [ClientLocationId("364538F9-8062-4CE0-B024-75A0FB463F0D")]
    [PublicProjectRequestRestrictions]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun GetTestRunById(
      int runId,
      bool includeDetails = true,
      bool includeTags = false)
    {
      return this.RunsHelper.GetTestRunById(this.ProjectId.ToString(), runId, includeDetails, includeTags);
    }

    [HttpDelete]
    [ClientLocationId("364538F9-8062-4CE0-B024-75A0FB463F0D")]
    public void DeleteTestRun(int runId) => this.RunsHelper.DeleteTestRun(this.ProjectId.ToString(), runId);

    [HttpPatch]
    [ClientLocationId("364538F9-8062-4CE0-B024-75A0FB463F0D")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun UpdateTestRun(
      int runId,
      RunUpdateModel runUpdateModel)
    {
      return this.RunsHelper.PatchTestRun(this.ProjectId.ToString(), runId, runUpdateModel);
    }

    [HttpPost]
    [ClientLocationId("364538F9-8062-4CE0-B024-75A0FB463F0D")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun CreateTestRun(
      RunCreateModel testRun)
    {
      NewProjectStepsPerformer.InitializeNewProject(this.TestManagementRequestContext, this.ProjectId);
      return this.RunsHelper.CreateTestRun(this.ProjectId.ToString(), testRun);
    }

    [HttpGet]
    [ClientLocationId("364538F9-8062-4CE0-B024-75A0FB463F0D")]
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
      string mergedContinuationToken;
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> collection = this.RunsHelper.QueryTestRuns(this.ProjectId, this.Url, minLastUpdatedDate, maxLastUpdatedDate, state, planIds, isAutomated, publishContext, buildIds, buildDefIds, branchName, releaseIds, releaseDefIds, releaseEnvIds, releaseEnvDefIds, runTitle, top, continuationToken, out mergedContinuationToken);
      HttpResponseMessage response = this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) collection);
      if (collection != null && !string.IsNullOrEmpty(mergedContinuationToken))
        Utils.SetContinuationToken(response, mergedContinuationToken);
      return response;
    }

    internal RunsHelper RunsHelper
    {
      get
      {
        if (this.m_runsHelper == null)
          this.m_runsHelper = new RunsHelper(this.TestManagementRequestContext);
        return this.m_runsHelper;
      }
    }
  }
}
