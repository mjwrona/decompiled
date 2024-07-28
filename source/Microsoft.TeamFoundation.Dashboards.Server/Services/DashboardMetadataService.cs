// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.DashboardMetadataService
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Dashboards.Telemetry;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public class DashboardMetadataService : IDashboardMetadataService, IVssFrameworkService
  {
    public IEnumerable<DashboardOwner> GetDashboardOwners(
      IVssRequestContext requestContext,
      IEnumerable<Dashboard> dashboards)
    {
      using (TelemetryCollector.TraceMonitor(requestContext, 10017414, "WidgetTypesService", "AppStoreWidgetMetadataService.GetWidgets"))
      {
        ITeamService service1 = requestContext.GetService<ITeamService>();
        IEnumerable<Guid> guids = dashboards.Where<Dashboard>((Func<Dashboard, bool>) (d => d.DashboardScope == DashboardScope.Project_Team)).Select<Dashboard, Guid>((Func<Dashboard, Guid>) (d => d.OwnerId)).Distinct<Guid>();
        IVssRequestContext requestContext1 = requestContext;
        IEnumerable<Guid> teamIds = guids;
        IReadOnlyCollection<WebApiTeam> teamsByGuid = service1.GetTeamsByGuid(requestContext1, teamIds);
        IdentityService service2 = requestContext.GetService<IdentityService>();
        IEnumerable<Guid> source1 = dashboards.Where<Dashboard>((Func<Dashboard, bool>) (d => d.DashboardScope == DashboardScope.Project)).Select<Dashboard, Guid>((Func<Dashboard, Guid>) (d => d.OwnerId)).Distinct<Guid>();
        IVssRequestContext requestContext2 = requestContext;
        List<Guid> list = source1.ToList<Guid>();
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source2 = service2.ReadIdentities(requestContext2, (IList<Guid>) list, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null));
        List<DashboardOwner> dashboardOwners = new List<DashboardOwner>();
        dashboardOwners.AddRange(teamsByGuid.Select<WebApiTeam, DashboardOwner>((Func<WebApiTeam, DashboardOwner>) (t => new DashboardOwner(t))));
        dashboardOwners.AddRange(source2.Select<Microsoft.VisualStudio.Services.Identity.Identity, DashboardOwner>((Func<Microsoft.VisualStudio.Services.Identity.Identity, DashboardOwner>) (o => new DashboardOwner(o))));
        return (IEnumerable<DashboardOwner>) dashboardOwners;
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
