// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.DashboardGroupEntryResponse
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [DataContract]
  public class DashboardGroupEntryResponse : DashboardGroupEntry
  {
    public DashboardGroupEntryResponse()
    {
    }

    public DashboardGroupEntryResponse(DashboardGroupEntry d, string url)
    {
      this.Url = url;
      this.Id = d.Id;
      this.Name = d.Name;
      this.Position = d.Position;
      this.RefreshInterval = d.RefreshInterval;
      this.Description = d.Description;
      this.GroupId = d.GroupId;
      this.OwnerId = d.OwnerId;
      this.DashboardScope = d.DashboardScope;
    }
  }
}
