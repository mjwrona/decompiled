// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.LegacyRunsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

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
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "runs", ResourceVersion = 1)]
  public class LegacyRunsController : TcmControllerBase
  {
    private RunsHelper m_runsHelper;
    private StatisticsHelper m_statisticsHelper;

    [HttpGet]
    [ActionName("runs")]
    [ClientLocationId("86337575-7EF2-4612-A60E-03CFB8C455B7")]
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
    [ActionName("runs")]
    [ClientLocationId("86337575-7EF2-4612-A60E-03CFB8C455B7")]
    [PublicProjectRequestRestrictions]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun GetTestRunById(int runId) => this.RunsHelper.GetTestRunById(this.ProjectId.ToString(), runId, true);

    [HttpDelete]
    [ActionName("runs")]
    [ClientLocationId("86337575-7EF2-4612-A60E-03CFB8C455B7")]
    public void DeleteTestRun(int runId) => this.RunsHelper.DeleteTestRun(this.ProjectId.ToString(), runId);

    [HttpPatch]
    [ActionName("runs")]
    [ClientLocationId("86337575-7EF2-4612-A60E-03CFB8C455B7")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun UpdateTestRun(
      int runId,
      RunUpdateModel runUpdateModel)
    {
      return this.RunsHelper.PatchTestRun(this.ProjectId.ToString(), runId, runUpdateModel);
    }

    [HttpPost]
    [ActionName("runs")]
    [ClientLocationId("86337575-7EF2-4612-A60E-03CFB8C455B7")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun CreateTestRun(
      RunCreateModel testRun)
    {
      NewProjectStepsPerformer.InitializeNewProject(this.TestManagementRequestContext, this.ProjectId);
      return this.RunsHelper.CreateTestRun(this.ProjectId.ToString(), testRun);
    }

    [HttpGet]
    [ActionName("runs")]
    [ClientLocationId("86337575-7EF2-4612-A60E-03CFB8C455B7")]
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
      if (collection != null && collection.Count == top)
        Utils.SetContinuationToken(response, mergedContinuationToken);
      return response;
    }

    [HttpGet]
    [ActionName("statistics")]
    [ClientLocationId("EE95FF93-38AA-4F10-88F6-74323C933711")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic GetTestRunStatistics(
      int runId)
    {
      return this.StatisticsHelper.GetTestRunStatisticsForRunId(this.ProjectId.ToString(), runId);
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

    internal StatisticsHelper StatisticsHelper
    {
      get
      {
        if (this.m_statisticsHelper == null)
          this.m_statisticsHelper = new StatisticsHelper(this.TestManagementRequestContext);
        return this.m_statisticsHelper;
      }
    }
  }
}
