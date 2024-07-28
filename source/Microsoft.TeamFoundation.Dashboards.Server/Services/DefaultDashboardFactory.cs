// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.DefaultDashboardFactory
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using System;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public class DefaultDashboardFactory
  {
    public static DashboardGroupEntry CreateDashboard(DashboardScope scope)
    {
      if (scope != DashboardScope.Collection_User)
      {
        if (scope == DashboardScope.Project_Team)
        {
          DashboardGroupEntry dashboard = new DashboardGroupEntry();
          dashboard.Id = new Guid?(Guid.NewGuid());
          dashboard.Name = Microsoft.TeamFoundation.Dashboards.DashboardResources.DefaultTeamDashboardName();
          dashboard.Position = 1;
          dashboard.RefreshInterval = new int?(0);
          return dashboard;
        }
        DashboardGroupEntry dashboard1 = new DashboardGroupEntry();
        dashboard1.Id = new Guid?(Guid.NewGuid());
        dashboard1.Name = Microsoft.TeamFoundation.Dashboards.DashboardResources.DefaultDashboardName();
        dashboard1.Position = 1;
        return dashboard1;
      }
      DashboardGroupEntry dashboard2 = new DashboardGroupEntry();
      dashboard2.Id = new Guid?(Guid.NewGuid());
      dashboard2.Name = Microsoft.TeamFoundation.Dashboards.DashboardResources.DefaultAccoutDashboardName();
      dashboard2.Position = 1;
      dashboard2.RefreshInterval = new int?(0);
      return dashboard2;
    }

    public static Dashboard CreateDashboardForCopy(
      IDashboardConsumer targetDashboardConsumer,
      DashboardGroupEntry sourceDashboard,
      CopyDashboardOptions options,
      int position)
    {
      return new Dashboard()
      {
        Id = new Guid?(Guid.NewGuid()),
        Position = position,
        ETag = "1",
        DashboardScope = targetDashboardConsumer.GetScope(),
        GroupId = targetDashboardConsumer.GetGroupId(),
        Name = options.Name,
        Description = options.Description.IsNullOrEmpty<char>() ? "" : options.Description,
        RefreshInterval = options.RefreshInterval.HasValue ? options.RefreshInterval : sourceDashboard.RefreshInterval
      };
    }
  }
}
