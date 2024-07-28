// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebServer.Controllers.PipelinesLogsController
// Assembly: Microsoft.Azure.Pipelines.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 734D9167-50DB-4E4F-ADE9-FD0A74DAB1E7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebServer.dll

using Microsoft.Azure.Pipelines.Server;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.Azure.Pipelines.WebApiConverters;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.WebServer.Controllers
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "pipelines", ResourceName = "logs")]
  public class PipelinesLogsController : PipelinesProjectApiController
  {
    [HttpGet]
    public Log GetLog(int pipelineId, int runId, int logId, [FromUri(Name = "$expand")] GetLogExpandOptions expandOptions = GetLogExpandOptions.None)
    {
      TaskLog log = this.TfsRequestContext.GetService<IPipelinesLogService>().GetLog(this.TfsRequestContext, this.ProjectId, pipelineId, runId, logId);
      return this.GetClientConverter<LogConverter>().ToWebApiLog(this.TfsRequestContext, this.ProjectId, log, pipelineId, runId, expandOptions.HasFlag((System.Enum) GetLogExpandOptions.SignedContent), (Action<IVssRequestContext, int>) ((context, id) => { }));
    }

    [HttpGet]
    public LogCollection ListLogs(int pipelineId, int runId, [FromUri(Name = "$expand")] GetLogExpandOptions expandOptions = GetLogExpandOptions.None)
    {
      IEnumerable<TaskLog> logs = this.TfsRequestContext.GetService<IPipelinesLogService>().GetLogs(this.TfsRequestContext, this.ProjectId, pipelineId, runId);
      return this.GetClientConverter<LogCollectionConverter>().ToWebApiLogCollection(this.TfsRequestContext, this.ProjectId, logs, pipelineId, runId, expandOptions.HasFlag((System.Enum) GetLogExpandOptions.SignedContent), (Action<IVssRequestContext, int>) ((context, id) => { }));
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<RunNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<LogNotFoundException>(HttpStatusCode.NotFound);
    }
  }
}
