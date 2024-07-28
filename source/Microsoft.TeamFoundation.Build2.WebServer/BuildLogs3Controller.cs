// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildLogs3Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
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
using System.Text;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "logs", ResourceVersion = 2)]
  [ClientGroupByResource("builds")]
  public class BuildLogs3Controller : BuildApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (List<BuildLog>), null, null)]
    [ClientResponseType(typeof (Stream), "GetBuildLogsZip", "application/zip")]
    public virtual HttpResponseMessage GetBuildLogs(int buildId)
    {
      RequestMediaType requestMediaType = MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, new List<RequestMediaType>()
      {
        RequestMediaType.Json,
        RequestMediaType.Zip
      }).FirstOrDefault<RequestMediaType>();
      BuildData buildById = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId);
      if (buildById == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      TaskOrchestrationPlan plan = this.TfsRequestContext.GetService<IBuildOrchestrator>().GetPlan(this.TfsRequestContext, buildById.ProjectId, buildById.OrchestrationPlan.PlanId);
      if (plan == null)
        return this.Request.CreateResponse(HttpStatusCode.OK);
      if (requestMediaType != RequestMediaType.Zip)
        return this.Request.CreateResponse<List<BuildLog>>(HttpStatusCode.OK, buildById.GetLogsMetadata(plan, this.ProjectId, this.TfsRequestContext).ToList<BuildLog>());
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromHours(1.0);
      return buildById.GetLogsZip(this.TfsRequestContext, plan, this.Request);
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetBuildLog", "text/plain")]
    [ClientResponseType(typeof (List<string>), "GetBuildLogLines", "application/json")]
    public virtual HttpResponseMessage GetBuildLog(
      int buildId,
      int logId,
      long? startLine = null,
      long? endLine = null)
    {
      int buildId1 = buildId;
      int logId1 = logId;
      List<RequestMediaType> supportedTypes = new List<RequestMediaType>();
      supportedTypes.Add(RequestMediaType.Json);
      supportedTypes.Add(RequestMediaType.Text);
      long? startLine1 = startLine;
      long? endLine1 = endLine;
      return this.GetBuildLogInternal(buildId1, logId1, supportedTypes, startLine1, endLine1);
    }

    protected HttpResponseMessage GetBuildLogInternal(
      int buildId,
      int logId,
      List<RequestMediaType> supportedTypes,
      long? startLine = null,
      long? endLine = null)
    {
      RequestMediaType requestMediaType = MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, supportedTypes).FirstOrDefault<RequestMediaType>();
      BuildData buildById = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId, includeDeleted: true);
      if (buildById == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      Guid? nullable = new Guid?();
      if (buildById.Deleted)
      {
        IBuildOrchestrator service = this.TfsRequestContext.GetService<IBuildOrchestrator>();
        Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference orchestrationPlanReference = buildById.Plans.Select<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference, Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>((Func<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference, Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>) (x => x)).Where<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>((Func<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference, bool>) (x =>
        {
          int? orchestrationType = x.OrchestrationType;
          int num = 2;
          return orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue;
        })).FirstOrDefault<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>();
        if (orchestrationPlanReference != null)
        {
          TaskOrchestrationPlan plan = service.GetPlan(this.TfsRequestContext, buildById.ProjectId, orchestrationPlanReference.PlanId);
          if (plan != null)
            nullable = new Guid?(plan.PlanId);
        }
      }
      else if (buildById.OrchestrationPlan != null)
        nullable = new Guid?(buildById.OrchestrationPlan.PlanId);
      if (!nullable.HasValue)
        return this.Request.CreateResponse(HttpStatusCode.OK);
      long startLine1 = 0;
      long maxValue = long.MaxValue;
      if (startLine.HasValue && startLine.Value > 0L)
        startLine1 = startLine.Value;
      if (endLine.HasValue)
        maxValue = endLine.Value;
      BuildLogLinesResult logResult = this.TfsRequestContext.GetService<IBuildOrchestrator>().GetLogLines(this.TfsRequestContext, this.ProjectId, nullable.Value, buildById.ToSecuredObject(), logId, startLine1, maxValue);
      this.AddDisposableResource((IDisposable) logResult);
      long totalLines = logResult.TotalLines;
      long startLine2 = logResult.StartLine;
      long endLine1 = logResult.EndLine;
      HttpResponseMessage response;
      switch (requestMediaType)
      {
        case RequestMediaType.Json:
          response = this.Request.CreateResponse<List<string>>(HttpStatusCode.OK, logResult.Lines.ToList<string>());
          break;
        case RequestMediaType.Zip:
          this.TfsRequestContext.RequestTimeout = TimeSpan.FromHours(1.0);
          return buildById.GetLogZip(this.TfsRequestContext, logId.ToString(), logResult.Lines, this.Request);
        default:
          response = this.Request.CreateResponse(HttpStatusCode.OK);
          HttpResponseMessage httpResponseMessage = response;
          Action<Stream, HttpContent, TransportContext> onStreamAvailable = (Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
          {
            try
            {
              using (StreamWriter streamWriter = new StreamWriter(stream))
              {
                foreach (string line in logResult.Lines)
                  streamWriter.WriteLine(line);
              }
            }
            finally
            {
              stream?.Dispose();
            }
          });
          MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("text/plain");
          mediaType.CharSet = Encoding.UTF8.WebName;
          ISecuredObject securedObject = buildById.ToSecuredObject();
          VssServerPushStreamContent pushStreamContent = new VssServerPushStreamContent(onStreamAvailable, mediaType, (object) securedObject);
          httpResponseMessage.Content = (HttpContent) pushStreamContent;
          break;
      }
      if (totalLines > 0L && startLine2 <= endLine1)
        response.Content.Headers.ContentRange = new ContentRangeHeaderValue(startLine2, endLine1, totalLines)
        {
          Unit = "lines"
        };
      this.TfsRequestContext.UpdateTimeToFirstPage();
      return response;
    }
  }
}
