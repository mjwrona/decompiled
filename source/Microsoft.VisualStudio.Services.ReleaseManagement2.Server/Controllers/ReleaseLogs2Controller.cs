// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.ReleaseLogs2Controller
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "logs", ResourceVersion = 2)]
  [ClientGroupByResource("releases")]
  public class ReleaseLogs2Controller : ReleaseLogsController
  {
    [ClientLocationId("17C91AF7-09FD-4256-BFF1-C24EE4F73BC0")]
    [ClientResponseType(typeof (Stream), null, "text/plain")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    [PublicProjectRequestRestrictions]
    [MethodInformation(IsLongRunning = true)]
    [ClientExample("GET__GetLogsOfReleaseTask.json", "Get logs of a task", null, null)]
    public HttpResponseMessage GetTaskLog(
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId,
      int taskId,
      long? startLine = null,
      long? endLine = null)
    {
      return this.GetTaskLogInternal(releaseId, environmentId, releaseDeployPhaseId, taskId, startLine, endLine);
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "As per research it is safe to suppress this. For more info refer to: http://stackoverflow.com/questions/10139718/code-anlysis-rule-ca2000-ca2202")]
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Instance is disposed elsewhere")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required use of types")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Suppressing to see the errors thrown by framework during a unique case")]
    [ClientLocationId("DEC7CA5A-7F7F-4797-8BF1-8EFC0DC93B28")]
    [ClientResponseType(typeof (Stream), null, "text/plain")]
    [ClientInternalUseOnly(false)]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetGateLog(
      int releaseId,
      int environmentId,
      int gateId,
      int taskId)
    {
      if (releaseId <= 0 || environmentId <= 0 || gateId <= 0 || taskId <= 0)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new VssServerPushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          this.TfsRequestContext.UpdateTimeToFirstPage();
          using (StreamWriter streamWriter = new StreamWriter((Stream) new PositionedStreamWrapper(stream)))
            this.TfsRequestContext.GetService<ReleaseLogsService>().DownloadGreenlightingLog(this.TfsRequestContext, this.ProjectId, releaseId, environmentId, gateId, taskId, streamWriter);
        }
        catch (Exception ex)
        {
          this.TfsRequestContext.TraceException(1900047, this.TraceArea, "Service", ex);
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

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "VSSF want a different name of function for route registration")]
    [ClientLocationId("2577E6C3-6999-4400-BC69-FE1D837755FE")]
    [ClientResponseType(typeof (Stream), null, "text/plain")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    [ClientInternalUseOnly(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetTaskLog2(
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId,
      int taskId,
      long? startLine = null,
      long? endLine = null)
    {
      return this.GetTaskLogInternal(releaseId, environmentId, attemptId, timelineId, taskId, startLine, endLine);
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "As per research it is safe to suppress this. For more info refer to: http://stackoverflow.com/questions/10139718/code-anlysis-rule-ca2000-ca2202")]
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Instance is disposed elsewhere")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Suppressing to see the errors thrown by framework during a unique case")]
    protected HttpResponseMessage GetTaskLogInternal(
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId,
      int taskId,
      long? startLine,
      long? endLine)
    {
      if (releaseId <= 0 || environmentId <= 0 || taskId <= 0 || attemptId <= 0)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      this.TfsRequestContext.TraceEnter(1900046, this.TraceArea, "Service", "ReleaseLogs2Controller::GetTaskLogInternal");
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new VssServerPushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          this.TfsRequestContext.UpdateTimeToFirstPage();
          using (StreamWriter streamWriter = new StreamWriter((Stream) new PositionedStreamWrapper(stream)))
            this.TfsRequestContext.GetService<ReleaseLogsService>().DownloadLog(this.TfsRequestContext, this.ProjectId, releaseId, environmentId, attemptId, timelineId, taskId, startLine, endLine, streamWriter);
        }
        catch (Exception ex)
        {
          this.TfsRequestContext.TraceException(1900047, this.TraceArea, "Service", ex);
        }
        finally
        {
          stream?.Dispose();
        }
      }), new MediaTypeHeaderValue("text/plain"), (object) this.TfsRequestContext.GetSecuredObject());
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "tasklog_{0}.log", (object) taskId));
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalDownloads").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_DownloadSec").Increment();
      this.TfsRequestContext.TraceLeave(1900046, this.TraceArea, "Service", "ReleaseLogs2Controller::GetTaskLogInternal");
      return response;
    }
  }
}
