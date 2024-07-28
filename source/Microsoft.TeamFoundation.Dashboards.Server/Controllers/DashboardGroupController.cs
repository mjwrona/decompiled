// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Controllers.DashboardGroupController
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.Services;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Dashboards.Controllers
{
  [VersionedApiControllerCustomName("Dashboard", "Groups", 1)]
  public class DashboardGroupController : DashboardApiControllerBase
  {
    private IDashboardService m_DashboardService;

    public override string TraceArea => "Groups";

    public IDashboardService DashboardService
    {
      get
      {
        if (this.m_DashboardService == null)
          this.m_DashboardService = this.TfsRequestContext.GetService<IDashboardService>();
        return this.m_DashboardService;
      }
    }

    [ClientExample("GET__dashboard_groups__groupId__.json", null, null, null)]
    [TraceFilter(10017010, 10017019)]
    [HttpGet]
    public DashboardGroup GetDashboardGroup(Guid groupId)
    {
      ArgumentUtility.CheckForEmptyGuid(groupId, nameof (groupId), "Dashboards");
      DashboardGroup dashboardGroup = this.DashboardService.GetDashboardGroup(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(groupId)));
      return this.CreateDashboardGroupResponse(groupId, dashboardGroup);
    }

    [ClientExample("PUT__dashboard_groups__groupId__.json", null, null, null)]
    [TraceFilter(10017000, 10017091)]
    [HttpPut]
    public DashboardGroup ReplaceDashboardGroup(Guid groupId, DashboardGroup group)
    {
      ArgumentUtility.CheckForEmptyGuid(groupId, nameof (groupId), "Dashboards");
      ArgumentUtility.CheckForNull<DashboardGroup>(group, nameof (group), "Dashboards");
      DashboardGroup dashboardGroup = this.DashboardService.UpdateDashboards(this.TfsRequestContext, DashboardConsumerFactory.CreateDashboardConsumer(this.TfsRequestContext, this.ProjectId, new Guid?(groupId)), group);
      return this.CreateDashboardGroupResponse(groupId, dashboardGroup);
    }
  }
}
