// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.DashboardResponse
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [DataContract]
  public class DashboardResponse : DashboardGroupEntry
  {
    public DashboardResponse()
    {
    }

    public DashboardResponse(
      IEnumerable<WidgetResponse> widgets,
      DashboardGroupEntry entry,
      string url)
    {
      this.Id = entry.Id;
      this.Name = entry.Name;
      this.Position = entry.Position;
      this.Widgets = (IEnumerable<Widget>) widgets;
      this.Url = url;
      this.RefreshInterval = entry.RefreshInterval;
      this.ETag = entry.ETag;
      this.Description = entry.Description;
      this.GroupId = entry.GroupId;
      this.OwnerId = entry.OwnerId;
      this.DashboardScope = entry.DashboardScope;
    }
  }
}
