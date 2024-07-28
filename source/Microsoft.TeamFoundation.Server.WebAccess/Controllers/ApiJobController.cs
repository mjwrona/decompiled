// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.ApiJobController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers
{
  [SupportedRouteArea("Api", NavigationContextLevels.All)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiJobController : TfsController
  {
    private const string c_logFilePrefix = "JobLog";

    [HttpGet]
    [TfsTraceFilter(503060, 503070)]
    public ActionResult MonitorJobProgress(Guid? hostId, Guid? jobId)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      JobProgressModel jobProgressModel = new JobProgressModel();
      if (!jobId.HasValue)
      {
        jobProgressModel.State = JobProgressState.NotStarted;
        return (ActionResult) this.Json((object) jobProgressModel.ToJson(), JsonRequestBehavior.AllowGet);
      }
      if (!hostId.HasValue)
        hostId = new Guid?(this.TfsRequestContext.ServiceHost.InstanceId);
      else if (!this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        Guid? nullable = hostId;
        Guid instanceId = this.TfsRequestContext.ServiceHost.InstanceId;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != instanceId ? 1 : 0) : 0) : 1) != 0)
        {
          jobProgressModel.State = JobProgressState.NotStarted;
          return (ActionResult) this.Json((object) jobProgressModel.ToJson(), JsonRequestBehavior.AllowGet);
        }
      }
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      ServicingJobDetail jobDetails;
      vssRequestContext.GetService<TeamFoundationServicingService>().GetServicingDetails(vssRequestContext, hostId.Value, jobId.Value, ServicingStepDetailFilterOptions.LastStepDetails, long.MaxValue, out jobDetails);
      if (jobDetails != null)
      {
        switch (jobDetails.JobStatus)
        {
          case ServicingJobStatus.Queued:
            jobProgressModel.State = JobProgressState.NotStarted;
            return (ActionResult) this.Json((object) jobProgressModel.ToJson(), JsonRequestBehavior.AllowGet);
          case ServicingJobStatus.Running:
            int num = (int) jobDetails.CompletedStepCount * 100 / (int) jobDetails.TotalStepCount;
            jobProgressModel.PercentComplete = num;
            jobProgressModel.State = JobProgressState.InProgress;
            return (ActionResult) this.Json((object) jobProgressModel.ToJson(), JsonRequestBehavior.AllowGet);
          case ServicingJobStatus.Complete:
            jobProgressModel.ResultMessage = this.GetJobResultMessage(jobId.Value);
            switch (jobDetails.Result)
            {
              case ServicingJobResult.Failed:
                this.LogError(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, WACommonResources.JobFailed, (object) jobId.Value.ToString("D")), TeamFoundationEventId.AccountException);
                jobProgressModel.State = JobProgressState.Error;
                return (ActionResult) this.Json((object) jobProgressModel.ToJson(), JsonRequestBehavior.AllowGet);
              case ServicingJobResult.PartiallySucceeded:
              case ServicingJobResult.Succeeded:
                jobProgressModel.State = JobProgressState.Complete;
                return (ActionResult) this.Json((object) jobProgressModel.ToJson(), JsonRequestBehavior.AllowGet);
              default:
                this.LogError(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, WACommonResources.JobFailed, (object) jobId.Value.ToString("D")), TeamFoundationEventId.AccountException);
                jobProgressModel.State = JobProgressState.Error;
                return (ActionResult) this.Json((object) jobProgressModel.ToJson(), JsonRequestBehavior.AllowGet);
            }
          case ServicingJobStatus.Failed:
            this.LogError(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, WACommonResources.JobFailed, (object) jobId.Value.ToString("D")), TeamFoundationEventId.AccountException);
            jobProgressModel.State = JobProgressState.Error;
            jobProgressModel.ResultMessage = this.GetJobResultMessage(jobId.Value);
            return (ActionResult) this.Json((object) jobProgressModel.ToJson(), JsonRequestBehavior.AllowGet);
          default:
            return (ActionResult) new EmptyResult();
        }
      }
      else
      {
        this.LogError(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, WACommonResources.JobFailed, (object) jobId.Value.ToString("D")), TeamFoundationEventId.AccountException);
        jobProgressModel.State = JobProgressState.Error;
        return (ActionResult) this.Json((object) jobProgressModel.ToJson(), JsonRequestBehavior.AllowGet);
      }
    }

    [HttpGet]
    public ActionResult DownloadJobLog(Guid jobId, string fileName = null)
    {
      fileName = fileName ?? string.Format("{0}_{1}.log", (object) "JobLog", (object) jobId);
      return (ActionResult) this.File(Encoding.UTF8.GetBytes(this.GetJobLog(jobId)), "text/plain;charset=utf-8", fileName);
    }

    private string GetJobResultMessage(Guid jobId) => this.TfsRequestContext.GetService<ITeamFoundationJobService>().QueryLatestJobHistory(this.TfsRequestContext, jobId)?.ResultMessage;

    private string GetJobLog(Guid jobId)
    {
      List<ServicingStepDetail> servicingDetails = this.TfsRequestContext.GetService<ITeamFoundationServicingService>().GetServicingDetails(this.TfsRequestContext, this.TfsRequestContext.ServiceHost.InstanceId, jobId, ServicingStepDetailFilterOptions.AllStepDetails);
      StringBuilder stringBuilder = new StringBuilder();
      foreach (ServicingStepDetail servicingStepDetail in servicingDetails)
        stringBuilder.AppendLine(servicingStepDetail.ToLogEntryLine());
      return stringBuilder.ToString();
    }
  }
}
