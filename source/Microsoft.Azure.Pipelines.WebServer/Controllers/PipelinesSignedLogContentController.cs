// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebServer.Controllers.PipelinesSignedLogContentController
// Assembly: Microsoft.Azure.Pipelines.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 734D9167-50DB-4E4F-ADE9-FD0A74DAB1E7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebServer.dll

using Microsoft.Azure.Pipelines.Server;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.WebServer.Controllers
{
  [ClientIgnore]
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "pipelines", ResourceName = "signedlogcontent")]
  public class PipelinesSignedLogContentController : PipelinesProjectApiController
  {
    [HttpGet]
    [MethodInformation(TimeoutSeconds = 1800)]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    [AllowAnonymousProjectLevelRequests]
    [ValidateRequestUrlSignatureFilter]
    public HttpResponseMessage GetSignedLogContent(int pipelineId, int runId, int logId)
    {
      this.CheckUrlSignatureValidated(this.TfsRequestContext);
      IVssRequestContext requestContext = this.TfsRequestContext.Elevate();
      Action<Stream> streamAction = requestContext.GetService<IPipelinesLogService>().GetLogContent(requestContext, this.ProjectId, pipelineId, runId, logId);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new PushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        requestContext.UpdateTimeToFirstPage();
        streamAction(stream);
      }), new MediaTypeHeaderValue("text/plain"));
      return response;
    }

    [HttpGet]
    [MethodInformation(TimeoutSeconds = 1800)]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    [AllowAnonymousProjectLevelRequests]
    [ValidateRequestUrlSignatureFilter]
    public HttpResponseMessage GetSignedLogsContent(int pipelineId, int runId)
    {
      this.CheckUrlSignatureValidated(this.TfsRequestContext);
      IVssRequestContext requestContext = this.TfsRequestContext.Elevate();
      Action<Stream> streamAction = requestContext.GetService<IPipelinesLogService>().GetZippedLogContents(requestContext, this.ProjectId, pipelineId, runId);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new PushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        requestContext.UpdateTimeToFirstPage();
        streamAction(stream);
      }), new MediaTypeHeaderValue("application/zip"));
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(string.Format("logs_{0}.zip", (object) runId));
      return response;
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<RunNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<LogNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<UrlSigningValidationException>(HttpStatusCode.BadRequest);
    }
  }
}
