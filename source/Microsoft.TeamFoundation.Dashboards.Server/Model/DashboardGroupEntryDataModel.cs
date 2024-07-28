// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Model.DashboardGroupEntryDataModel
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using System;

namespace Microsoft.TeamFoundation.Dashboards.Model
{
  public class DashboardGroupEntryDataModel : DashboardGroupEntry
  {
    public DashboardGroupEntryDataModel(Guid groupId, Dashboard dashboard)
    {
      this.GroupId = groupId;
      this.Id = dashboard.Id;
      this.Name = dashboard.Name;
      this.Position = dashboard.Position;
      this.RefreshInterval = dashboard.RefreshInterval;
      this.ETag = dashboard.ETag;
      this.Description = dashboard.Description;
      this.OwnerId = dashboard.OwnerId;
      this.LastAccessedDate = dashboard.LastAccessedDate;
      this.ModifiedDate = dashboard.ModifiedDate;
      this.ModifiedBy = dashboard.ModifiedBy;
    }
  }
}
