// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubCompatTimelineRecordLogController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
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
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowStream = true, AllowEmptyOrPlainTextContentTypeForCompatServiceCalls = true)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "logs")]
  public sealed class TaskHubCompatTimelineRecordLogController : TaskHubCompatApiController
  {
    [HttpPost]
    public async Task<HttpResponseMessage> AppendLog(Guid planId, int logId)
    {
      TaskHubCompatTimelineRecordLogController recordLogController = this;
      HttpContent content1 = recordLogController.Request.Content;
      if (content1 == null)
        return recordLogController.Request.CreateResponse<string>(HttpStatusCode.BadRequest, TaskResources.LogWithNoContentError());
      if (!content1.Headers.ContentLength.HasValue)
        return recordLogController.Request.CreateResponse<string>(HttpStatusCode.BadRequest, TaskResources.LogWithNoContentLengthError());
      bool useBlob = recordLogController.TfsRequestContext.IsFeatureEnabled("DistributedTask.AppendLogContentToBlobStore");
      Stream content2 = await content1.ReadAsStreamAsync();
      TaskLog taskLog = recordLogController.Hub.AppendLog(recordLogController.TfsRequestContext, Guid.Empty, planId, logId, content2, useBlob);
      return recordLogController.Request.CreateResponse<TaskLog>(HttpStatusCode.OK, taskLog);
    }

    [HttpPost]
    public Task<TaskLog> CreateLog(Guid planId, TaskLog log) => this.Hub.CreateLogAsync(this.TfsRequestContext, Guid.Empty, planId, log.Path);

    [HttpGet]
    public HttpResponseMessage GetLog(Guid planId, int logId, long? startLine = null, long? endLine = null)
    {
      long startLine1 = 0;
      long maxValue = long.MaxValue;
      if (startLine.HasValue && startLine.Value > 0L)
        startLine1 = startLine.Value;
      if (endLine.HasValue)
        maxValue = endLine.Value;
      long totalLines;
      IEnumerable<string> list;
      using (TeamFoundationDataReader logLines = this.Hub.GetLogLines(this.TfsRequestContext, Guid.Empty, planId, logId, (ISecuredObject) null, ref startLine1, ref maxValue, out totalLines))
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
    public IEnumerable<TaskLog> GetLogs(Guid planId)
    {
      IEnumerable<TaskLog> logs = this.Hub.GetLogs(this.TfsRequestContext, Guid.Empty, planId);
      foreach (TaskLog log in logs)
        log.UpdateLocations(this.TfsRequestContext, planId);
      return logs;
    }
  }
}
