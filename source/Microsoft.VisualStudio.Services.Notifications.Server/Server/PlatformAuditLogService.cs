// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.PlatformAuditLogService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.Audit;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class PlatformAuditLogService : IAuditLogService, IVssFrameworkService
  {
    public const int DefaultEventExpirationInDays = 30;
    internal static readonly Guid s_processingJobId = new Guid("7B22D235-9147-40B0-98E8-9F97FBDB051A");

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void Log(
      IVssRequestContext requestContext,
      string actionId,
      IDictionary<string, object> data,
      Guid targetHostId = default (Guid),
      Guid projectId = default (Guid))
    {
      this.Log(requestContext, actionId, data, new AuditLogContextOverride(targetHostId, projectId));
    }

    public void Log(
      IVssRequestContext requestContext,
      string actionId,
      IDictionary<string, object> data,
      AuditLogContextOverride contextOverride)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!requestContext.ShouldAuditLogEvents())
        return;
      AuditLogEntry auditLogEntryRaw = requestContext.CreateAuditLogEntryRaw(actionId, data, contextOverride);
      this.LogInternal(requestContext, auditLogEntryRaw);
    }

    internal virtual void LogInternal(IVssRequestContext requestContext, AuditLogEntry entry)
    {
      VssNotificationEvent notificationEvent = new VssNotificationEvent()
      {
        EventType = "ms.vss-notifications.audit-event",
        Data = (object) entry,
        ExpiresIn = TimeSpan.FromDays(30.0)
      };
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        requestContext.TraceAlways(31000090, TraceLevel.Info, nameof (PlatformAuditLogService), "Notifications", entry.ToString() ?? "");
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent, true);
    }

    public void HandlePostLog(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.NotifyAuditEventLogged)));

    internal void NotifyAuditEventLogged(IVssRequestContext requestContext, object taskArgs) => requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
    {
      PlatformAuditLogService.s_processingJobId
    });
  }
}
