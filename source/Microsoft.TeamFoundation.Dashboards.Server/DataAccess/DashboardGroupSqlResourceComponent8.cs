// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.DashboardGroupSqlResourceComponent8
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Dashboards.DataAccess
{
  public class DashboardGroupSqlResourceComponent8 : DashboardGroupSqlResourceComponent7
  {
    public override List<DashboardGroupEntry> GetDashboardsByProjectId(Guid projectId)
    {
      this.PrepareStoredProcedure("Dashboards.prc_GetDashboardsByProjectId");
      this.BindDataspaceId(projectId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DashboardGroupEntry>((ObjectBinder<DashboardGroupEntry>) new DashboardGroupBinder3());
      return resultCollection.GetCurrent<DashboardGroupEntry>().Items;
    }

    public override List<DashboardGroupEntry> GetDashboardsByIds(
      Guid projectId,
      IEnumerable<Guid> dashboardIds)
    {
      this.PrepareStoredProcedure("Dashboards.prc_GetDashboardsByIds");
      this.BindGuidTable("@dashboardIds", dashboardIds);
      this.BindDataspaceId(projectId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DashboardGroupEntry>((ObjectBinder<DashboardGroupEntry>) new DashboardGroupBinder3());
      return resultCollection.GetCurrent<DashboardGroupEntry>().Items;
    }
  }
}
