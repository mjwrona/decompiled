// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Controllers.ApiMonitoringController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Monitoring, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2931506-B8BC-4923-B99C-2CD8E1087ABB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Monitoring.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Charts;
using Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Model;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Controllers
{
  [SupportedRouteArea("Api", NavigationContextLevels.Deployment | NavigationContextLevels.Application)]
  [OutputCache(CacheProfile = "NoCache")]
  public class ApiMonitoringController : JobMonitoringAreaController
  {
    private const string c_ie8SearchString = "Trident/4.0";

    private IVssRequestContext RequestContext => this.TfsRequestContext.To(TeamFoundationHostType.Deployment);

    private bool IsLimitiedBrowser() => this.Request.UserAgent.IndexOf("Trident/4.0", 0, StringComparison.InvariantCultureIgnoreCase) >= 0;

    [AcceptVerbs(HttpVerbs.Get)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult Get24HourAverageChart()
    {
      using (AvgRunQueueTimeAndTotalJobsChart andTotalJobsChart = new AvgRunQueueTimeAndTotalJobsChart(this.RequestContext, this.IsLimitiedBrowser(), new Guid?()))
        return (ActionResult) this.Json((object) andTotalJobsChart.Chart.ToJson(), JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult GetAverageChartForJob(Guid? jobId)
    {
      using (AvgRunQueueTimeAndTotalJobsChart andTotalJobsChart = new AvgRunQueueTimeAndTotalJobsChart(this.RequestContext, this.IsLimitiedBrowser(), jobId))
        return (ActionResult) this.Json((object) andTotalJobsChart.Chart.ToJson(), JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult GetJobResultsOverTimeChart() => (ActionResult) this.Json((object) new JobResultsOverTimeChart(this.RequestContext, this.IsLimitiedBrowser()).Chart.ToJson(), JsonRequestBehavior.AllowGet);

    [AcceptVerbs(HttpVerbs.Get)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult GetTotalRunTimePieChart() => (ActionResult) this.Json((object) new TotalRunTimePieChart(this.RequestContext, this.IsLimitiedBrowser()).Chart.ToJson(), JsonRequestBehavior.AllowGet);

    [AcceptVerbs(HttpVerbs.Get)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult GetTotalRunTimeChart()
    {
      using (TotalRunTimeChart totalRunTimeChart = new TotalRunTimeChart(this.RequestContext, this.IsLimitiedBrowser()))
        return (ActionResult) this.Json((object) totalRunTimeChart.Chart.ToJson(), JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult GetJobQueuePositionCountChart() => (ActionResult) this.Json((object) new JobQueuePositionCountChart(this.RequestContext, this.IsLimitiedBrowser()).Chart.ToJson(), JsonRequestBehavior.AllowGet);

    [AcceptVerbs(HttpVerbs.Get)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult GetJobQueue(int position)
    {
      DataContractJsonResult jobQueue = new DataContractJsonResult((object) this.RequestContext.GetService<TeamFoundationJobReportingService>().QueryQueueEntries(this.RequestContext, position).Select<TeamFoundationJobReportingQueuePositions, JobQueueModel>((Func<TeamFoundationJobReportingQueuePositions, JobQueueModel>) (item => new JobQueueModel(this.RequestContext, item))).ToArray<JobQueueModel>());
      jobQueue.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) jobQueue;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult GetJobHistory(string jobId, int? resultType)
    {
      Guid result;
      if (!Guid.TryParse(jobId, out result))
        result = Guid.Empty;
      TeamFoundationJobReportingService service = this.RequestContext.GetService<TeamFoundationJobReportingService>();
      DataContractJsonResult jobHistory = new DataContractJsonResult((object) service.QueryHistory(this.RequestContext, service.MaxNumberOfHistoryResults, new Guid?(result), resultType).Select<TeamFoundationJobReportingHistory, JobHistoryModel>((Func<TeamFoundationJobReportingHistory, JobHistoryModel>) (item => new JobHistoryModel(this.RequestContext, item))).ToArray<JobHistoryModel>());
      jobHistory.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) jobHistory;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult GetJobResultsOverTime()
    {
      DataContractJsonResult jobResultsOverTime = new DataContractJsonResult((object) this.RequestContext.GetService<TeamFoundationJobReportingService>().QueryResultsOverTime(this.RequestContext).Select<TeamFoundationJobReportingResultsOverTime, JobResultsOverTimeModel>((Func<TeamFoundationJobReportingResultsOverTime, JobResultsOverTimeModel>) (item => new JobResultsOverTimeModel(this.RequestContext, item))).ToArray<JobResultsOverTimeModel>());
      jobResultsOverTime.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) jobResultsOverTime;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult GetJobName(Guid? jobId) => !jobId.HasValue ? (ActionResult) this.Json((object) MonitoringServerResources.AllJobsSelected, JsonRequestBehavior.AllowGet) : (ActionResult) this.Json((object) this.RequestContext.GetService<TeamFoundationJobReportingService>().GetJobName(this.RequestContext, jobId.Value), JsonRequestBehavior.AllowGet);

    [AcceptVerbs(HttpVerbs.Get)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult GetJobResultName(int? resultId) => !resultId.HasValue ? (ActionResult) this.Json((object) MonitoringServerResources.NonSuccessfulJobs, JsonRequestBehavior.AllowGet) : (ActionResult) this.Json((object) ((TeamFoundationJobResult) resultId.Value).ToString(), JsonRequestBehavior.AllowGet);

    [AcceptVerbs(HttpVerbs.Get)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult GetJobDefinitionDetails(Guid hostId, Guid jobId)
    {
      TeamFoundationHostManagementService service1 = this.RequestContext.GetService<TeamFoundationHostManagementService>();
      JobDefinitionDetailModel data = (JobDefinitionDetailModel) null;
      IVssRequestContext requestContext = this.RequestContext;
      Guid instanceId = hostId;
      using (IVssRequestContext vssRequestContext = service1.BeginRequest(requestContext, instanceId, RequestContextType.SystemContext, true, false))
      {
        TeamFoundationJobDefinition job = vssRequestContext.GetService<TeamFoundationJobService>().QueryJobDefinition(vssRequestContext, jobId);
        data = new JobDefinitionDetailModel(vssRequestContext, job);
      }
      TeamFoundationJobReportingService service2 = this.RequestContext.GetService<TeamFoundationJobReportingService>();
      data.JobName = service2.GetJobName(this.RequestContext, jobId);
      DataContractJsonResult definitionDetails = new DataContractJsonResult((object) data);
      definitionDetails.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) definitionDetails;
    }
  }
}
