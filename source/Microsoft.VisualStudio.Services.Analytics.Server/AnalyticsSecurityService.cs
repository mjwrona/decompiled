// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsSecurityService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class AnalyticsSecurityService : IAnalyticsSecurityService, IVssFrameworkService
  {
    private static readonly Guid AnalyticsProjectPermissionsJobId = new Guid("315C619A-C4F5-4721-A850-4E86F98588F5");

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.RegisterNotification(systemRequestContext, "Default", new Guid("FCA37BCC-5502-408C-8E4D-B73A3DCFA21B"), new SqlNotificationHandler(this.OnProjectCreated), false);
      service.RegisterNotification(systemRequestContext, "Default", new Guid("BF5C56C8-849B-4BC3-A604-83128F921352"), new SqlNotificationHandler(this.OnProjectDeleted), false);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(systemRequestContext, "Default", new Guid("FCA37BCC-5502-408C-8E4D-B73A3DCFA21B"), new SqlNotificationHandler(this.OnProjectCreated), true);
      service.UnregisterNotification(systemRequestContext, "Default", new Guid("BF5C56C8-849B-4BC3-A604-83128F921352"), new SqlNotificationHandler(this.OnProjectDeleted), true);
    }

    public ICollection<ProjectInfo> GetAccessibleProjects(IVssRequestContext requestContext) => (ICollection<ProjectInfo>) requestContext.GetService<IProjectService>().GetProjects(requestContext, ProjectState.WellFormed).Where<ProjectInfo>((Func<ProjectInfo, bool>) (p => this.HasReadPermission(requestContext, p.Id))).ToList<ProjectInfo>();

    public bool HasStagingPermission(IVssRequestContext requestContext) => AnalyticsSecurityService.GetSecurityNamespace(requestContext).HasPermission(requestContext, "$", 4, false);

    public bool HasExecuteUnrestrictedQueryPermission(IVssRequestContext requestContext) => AnalyticsSecurityService.GetSecurityNamespace(requestContext).HasPermission(requestContext, "$", 8);

    public bool HasReadPermission(IVssRequestContext requestContext, Guid projectId) => AnalyticsSecurityService.GetSecurityNamespace(requestContext).HasPermission(requestContext, AnalyticsSecurityNamespace.GetSecurityToken(projectId), 1);

    public bool HasReadEuiiPermission(IVssRequestContext requestContext) => AnalyticsSecurityService.GetSecurityNamespace(requestContext).HasPermission(requestContext, "$", 16);

    private static IVssSecurityNamespace GetSecurityNamespace(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, AnalyticsSecurityNamespace.Id).Secured();

    private void OnProjectCreated(IVssRequestContext requestContext, NotificationEventArgs args) => requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
    {
      AnalyticsSecurityService.AnalyticsProjectPermissionsJobId
    });

    private void OnProjectDeleted(IVssRequestContext requestContext, NotificationEventArgs args) => requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
    {
      AnalyticsSecurityService.AnalyticsProjectPermissionsJobId
    });
  }
}
