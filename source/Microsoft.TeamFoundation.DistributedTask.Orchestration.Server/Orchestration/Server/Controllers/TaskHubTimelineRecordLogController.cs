// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubTimelineRecordLogController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers
{
  [ControllerApiVersion(2.0)]
  [RequestContentTypeRestriction(AllowStream = true, AllowEmptyOrPlainTextContentTypeForCompatServiceCalls = true)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "logs")]
  public sealed class TaskHubTimelineRecordLogController : TaskHubApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (TaskLog), null, null)]
    [ClientRequestBodyIsStream]
    [ClientExample("POST__distributedtask_AppendLogContent_.json", "Append content to a log", null, null)]
    public async Task<HttpResponseMessage> AppendLogContent(Guid planId, int logId)
    {
      TaskHubTimelineRecordLogController recordLogController = this;
      HttpContent content1 = recordLogController.Request.Content;
      if (content1 == null)
        return recordLogController.Request.CreateResponse<string>(HttpStatusCode.BadRequest, TaskResources.LogWithNoContentError());
      if (!content1.Headers.ContentLength.HasValue)
        return recordLogController.Request.CreateResponse<string>(HttpStatusCode.BadRequest, TaskResources.LogWithNoContentLengthError());
      bool useBlob = recordLogController.TfsRequestContext.IsFeatureEnabled("DistributedTask.AppendLogContentToBlobStore");
      Stream content2 = await content1.ReadAsStreamAsync();
      TaskLog taskLog = recordLogController.Hub.AppendLog(recordLogController.TfsRequestContext, recordLogController.ScopeIdentifier, planId, logId, content2, useBlob);
      return recordLogController.Request.CreateResponse<TaskLog>(HttpStatusCode.OK, taskLog);
    }

    [HttpPost]
    [ClientTemporarySwaggerExclusion]
    [ClientResponseType(typeof (TaskLog), null, null)]
    public async Task<HttpResponseMessage> AssociateLogAsync(
      Guid planId,
      int logId,
      string serializedBlobId,
      int lineCount)
    {
      TaskHubTimelineRecordLogController recordLogController = this;
      TaskLog taskLog = await recordLogController.Hub.AssociateLogAsync(recordLogController.TfsRequestContext, recordLogController.ScopeIdentifier, planId, logId, serializedBlobId, lineCount);
      return recordLogController.Request.CreateResponse<TaskLog>(HttpStatusCode.OK, taskLog);
    }

    [HttpPost]
    [ClientResponseType(typeof (TaskLog), null, null)]
    [ClientExample("POST__distributedtask_CreateLog_.json", "Create a log", null, null)]
    public Task<TaskLog> CreateLog(Guid planId, TaskLog log) => this.Hub != null && log != null ? this.Hub.CreateLogAsync(this.TfsRequestContext, this.ScopeIdentifier, planId, log.Path) : (Task<TaskLog>) null;

    [HttpGet]
    [ClientTemporarySwaggerExclusion]
    [ClientResponseType(typeof (IEnumerable<string>), null, null)]
    public HttpResponseMessage GetLog(Guid planId, int logId, [ClientQueryParameter] long? startLine = null, [ClientQueryParameter] long? endLine = null)
    {
      long startLine1 = 0;
      long maxValue = long.MaxValue;
      if (startLine.HasValue && startLine.Value > 0L)
        startLine1 = startLine.Value;
      if (endLine.HasValue)
        maxValue = endLine.Value;
      long totalLines;
      IEnumerable<string> list;
      using (TeamFoundationDataReader logLines = this.Hub.GetLogLines(this.TfsRequestContext, this.ScopeIdentifier, planId, logId, (ISecuredObject) null, ref startLine1, ref maxValue, out totalLines))
        list = (IEnumerable<string>) logLines.CurrentEnumerable<string>().ToList<string>();
      HttpResponseMessage response = this.Request.CreateResponse<IEnumerable<string>>(HttpStatusCode.OK, list);
      if (totalLines > 0L && startLine1 <= maxValue)
        response.Content.Headers.ContentRange = new ContentRangeHeaderValue(startLine1, maxValue, totalLines)
        {
          Unit = "lines"
        };
      return response;
    }

    [HttpGet]
    [ClientTemporarySwaggerExclusion]
    public IEnumerable<TaskLog> GetLogs(Guid planId)
    {
      IEnumerable<TaskLog> logs = this.Hub.GetLogs(this.TfsRequestContext, this.ScopeIdentifier, planId);
      foreach (TaskLog log in logs)
        log.UpdateLocations(this.TfsRequestContext, planId);
      return logs;
    }
  }
}
