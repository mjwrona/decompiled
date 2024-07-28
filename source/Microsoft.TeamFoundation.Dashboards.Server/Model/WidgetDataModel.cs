// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Model.WidgetDataModel
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using System;

namespace Microsoft.TeamFoundation.Dashboards.Model
{
  public class WidgetDataModel : Widget
  {
    public Guid DashboardId { get; set; }

    public bool ReplaceWidget { get; set; }

    public WidgetDataModel(Guid dashboardId, Widget widget)
    {
      this.Id = widget.Id;
      this.DashboardId = dashboardId;
      this.Name = widget.Name;
      this.Position = widget.Position;
      this.Size = widget.Size;
      this.ContributionId = widget.ContributionId;
      this.Settings = widget.Settings;
      this.ArtifactId = widget.ArtifactId;
      this.SettingsVersion = widget.SettingsVersion;
      this.ETag = widget.ETag;
      if (widget.Dashboard == null)
        return;
      this.Dashboard = new Dashboard(widget.Dashboard.ETag);
    }
  }
}
