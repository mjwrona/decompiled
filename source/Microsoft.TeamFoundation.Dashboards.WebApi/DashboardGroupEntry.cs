// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.DashboardGroupEntry
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [DataContract]
  public class DashboardGroupEntry : Dashboard
  {
    public DashboardGroupEntry()
    {
    }

    public DashboardGroupEntry(Dashboard dashboard)
    {
      this.ETag = dashboard.ETag;
      this.Id = dashboard.Id;
      this.Name = dashboard.Name;
      this.RefreshInterval = dashboard.RefreshInterval;
      this.Position = dashboard.Position;
      this.GroupId = dashboard.GroupId;
      this.OwnerId = dashboard.OwnerId;
      this.Description = dashboard.Description;
      this.DashboardScope = dashboard.DashboardScope;
      this.ModifiedBy = dashboard.ModifiedBy;
      this.ModifiedDate = dashboard.ModifiedDate;
      this.LastAccessedDate = dashboard.LastAccessedDate;
    }
  }
}
