// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.ReleaseLogsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "logs")]
  [ClientGroupByResource("releases")]
  public class ReleaseLogsController : ReleaseManagementProjectControllerBase
  {
    [ClientInternalUseOnly(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [ClientLocationId("E71BA1ED-C0A4-4A28-A61F-2DD5F68CF3FD")]
    [ClientResponseType(typeof (Stream), null, "text/plain")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    public HttpResponseMessage GetLog(int releaseId, int environmentId, int taskId, int attemptId = 1) => this.GetTaskLogInternal(releaseId, environmentId, 0, taskId, attemptId, new long?(), new long?());

    [ClientLocationId("C37FBAB5-214B-48E4-A55B-CB6B4F6E4038")]
    [ClientResponseType(typeof (Stream), null, "application/zip")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    [PublicProjectRequestRestrictions]
    [ClientExample("GET__GetLogsOfReleaseTask.json", "Get logs of a task", null, null)]
    public HttpResponseMessage GetLogs(int releaseId) => this.GetLogsForRelease(releaseId);

    protected HttpResponseMessage GetTaskLogInternal(
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId,
      int taskId,
      long? startLine,
      long? endLine)
    {
      return this.GetTaskLogInternal(releaseId, environmentId, releaseDeployPhaseId, taskId, 0, startLine, endLine);
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "As per research it is safe to suppress this. For more info refer to: http://stackoverflow.com/questions/10139718/code-anlysis-rule-ca2000-ca2202")]
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Instance is disposed elsewhere")]
    protected HttpResponseMessage GetTaskLogInternal(
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId,
      int taskId,
      int attemptId,
      long? startLine,
      long? endLine)
    {
      if (releaseId <= 0 || environmentId <= 0 || taskId <= 0 || releaseDeployPhaseId <= 0 && attemptId <= 0)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new VssServerPushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          this.TfsRequestContext.UpdateTimeToFirstPage();
          using (StreamWriter streamWriter = new StreamWriter((Stream) new PositionedStreamWrapper(stream)))
            this.TfsRequestContext.GetService<ReleaseLogsService>().DownloadLog(this.TfsRequestContext, this.ProjectId, releaseId, environmentId, releaseDeployPhaseId, taskId, attemptId, startLine, endLine, streamWriter);
        }
        finally
        {
          stream?.Dispose();
        }
      }), new MediaTypeHeaderValue("text/plain"), (object) this.TfsRequestContext.GetSecuredObject());
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "tasklog_{0}.log", (object) taskId));
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalDownloads").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_DownloadSec").Increment();
      return response;
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "As per research it is safe to suppress this. For more info refer to: http://stackoverflow.com/questions/10139718/code-anlysis-rule-ca2000-ca2202")]
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Stream dispose should be sufficient. For more info refer to: https://aspnetwebstack.codeplex.com/discussions/461495")]
    protected HttpResponseMessage GetLogsForRelease(int releaseId)
    {
      if (releaseId <= 0)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new VssServerPushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          this.TfsRequestContext.UpdateTimeToFirstPage();
          using (SmartPushStreamContentStream streamContentStream = new SmartPushStreamContentStream(stream))
          {
            using (ZipArchive zipArchive = new ZipArchive((Stream) streamContentStream, ZipArchiveMode.Create))
              this.TfsRequestContext.GetService<ReleaseLogsService>().DownloadLogs(this.TfsRequestContext, this.ProjectId, releaseId, zipArchive);
          }
        }
        finally
        {
          stream?.Dispose();
        }
      }), new MediaTypeHeaderValue("application/zip"), (object) this.TfsRequestContext.GetSecuredObject());
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "ReleaseLogs_{0}.zip", (object) releaseId));
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalDownloads");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_DownloadSec");
      performanceCounter.Increment();
      return response;
    }
  }
}
