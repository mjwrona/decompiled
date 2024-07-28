// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.TeamDashboardConsumer
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using System;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public class TeamDashboardConsumer : IDashboardConsumer
  {
    private Guid m_projectId;
    private Guid m_teamId;

    public TeamDashboardConsumer(Guid projectId, Guid teamId)
    {
      this.m_projectId = projectId;
      this.m_teamId = teamId;
    }

    public Guid GetDataspaceId() => this.m_projectId;

    public Guid GetGroupId() => this.m_teamId;

    public DashboardScope GetScope() => DashboardScope.Project_Team;
  }
}
