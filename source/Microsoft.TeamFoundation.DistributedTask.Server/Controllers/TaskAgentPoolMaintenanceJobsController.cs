// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentPoolMaintenanceJobsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "maintenancejobs")]
  public sealed class TaskAgentPoolMaintenanceJobsController : DistributedTaskApiController
  {
    [HttpGet]
    public IList<TaskAgentPoolMaintenanceJob> GetAgentPoolMaintenanceJobs(
      int poolId,
      int? definitionId = null)
    {
      return this.ResourceService.GetAgentPoolMaintenanceJobs(this.TfsRequestContext, poolId, definitionId);
    }

    [HttpPost]
    public TaskAgentPoolMaintenanceJob QueueAgentPoolMaintenanceJob(
      int poolId,
      TaskAgentPoolMaintenanceJob job)
    {
      ArgumentUtility.CheckForNull<TaskAgentPoolMaintenanceJob>(job, nameof (job));
      ArgumentUtility.CheckForNull<TaskAgentPoolReference>(job.Pool, "Pool");
      ArgumentUtility.CheckGreaterThanZero((float) job.Pool.Id, "Id");
      ArgumentUtility.CheckGreaterThanZero((float) job.DefinitionId, "DefinitionId");
      return this.ResourceService.QueueAgentPoolMaintenanceJob(this.TfsRequestContext, job.Pool.Id, job.DefinitionId);
    }

    [HttpPatch]
    public TaskAgentPoolMaintenanceJob UpdateAgentPoolMaintenanceJob(
      int poolId,
      int jobId,
      TaskAgentPoolMaintenanceJob job)
    {
      ArgumentUtility.CheckForNull<TaskAgentPoolMaintenanceJob>(job, nameof (job));
      ArgumentUtility.CheckForNull<TaskAgentPoolReference>(job.Pool, "Pool");
      ArgumentUtility.CheckGreaterThanZero((float) job.Pool.Id, "Id");
      ArgumentUtility.CheckGreaterThanZero((float) job.JobId, "JobId");
      return this.ResourceService.UpdateAgentPoolMaintenanceJob(this.TfsRequestContext, poolId, jobId, job);
    }

    [HttpGet]
    [ClientResponseType(typeof (TaskAgentPoolMaintenanceJob), null, null, MethodName = "GetAgentPoolMaintenanceJob")]
    [ClientResponseType(typeof (Stream), null, null, MethodName = "GetAgentPoolMaintenanceJobLogs", MediaType = "application/zip")]
    public HttpResponseMessage GetAgentPoolMaintenanceJob(int poolId, int jobId)
    {
      if (!this.ZipFileRequested())
        return this.Request.CreateResponse<TaskAgentPoolMaintenanceJob>(HttpStatusCode.OK, this.ResourceService.GetAgentPoolMaintenanceJob(this.TfsRequestContext, poolId, jobId));
      string fileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "maintenance_log_{0}.zip", (object) jobId);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new PushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          this.ResourceService.GetAgentPoolMaintenanceJobLogs(this.TfsRequestContext, poolId, jobId, stream);
        }
        finally
        {
          stream?.Dispose();
        }
      }), new MediaTypeHeaderValue("application/zip"));
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(fileName);
      this.TfsRequestContext.UpdateTimeToFirstPage();
      return response;
    }

    [HttpDelete]
    public void DeleteAgentPoolMaintenanceJob(int poolId, int jobId) => this.ResourceService.DeleteAgentPoolMaintenanceJobs(this.TfsRequestContext, poolId, (IEnumerable<int>) new int[1]
    {
      jobId
    });
  }
}
