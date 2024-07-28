// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.LegacySprintBurndownWidgetAdapter
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public class LegacySprintBurndownWidgetAdapter : SprintBurndownWidgetAdapter, IWidgetAdapter
  {
    public override Widget ProcessAndCopyWidget(
      Widget widget,
      IVssRequestContext requestContext,
      IDashboardConsumer sourceDashboardConsumer,
      IDashboardConsumer targetDashboardConsumer,
      bool purgeSettings = false,
      CopyDashboardParameterHandler copyDashboardParameterHandler = null)
    {
      Widget widget1 = base.ProcessAndCopyWidget(widget, requestContext, sourceDashboardConsumer, targetDashboardConsumer, purgeSettings, copyDashboardParameterHandler);
      Dictionary<string, object> settings;
      if (widget1.Settings != null && JsonUtilities.TryDeserialize<Dictionary<string, object>>(widget1.Settings, out settings, true))
      {
        if (settings.Keys.Contains<string>("isLegacy") && bool.Parse(settings["isLegacy"].ToString()))
        {
          Dictionary<string, string> dictionary;
          if (!settings.Keys.Contains<string>("team") || !JsonUtilities.TryDeserialize<Dictionary<string, string>>(settings["team"].ToString(), out dictionary, true))
            dictionary = new Dictionary<string, string>();
          if (targetDashboardConsumer.GetScope() == DashboardScope.Project_Team || sourceDashboardConsumer.GetScope() == DashboardScope.Project_Team && targetDashboardConsumer.GetDataspaceId() == sourceDashboardConsumer.GetDataspaceId())
          {
            dictionary["projectId"] = targetDashboardConsumer.GetDataspaceId().ToString();
            dictionary["teamId"] = targetDashboardConsumer.GetScope() == DashboardScope.Project_Team ? targetDashboardConsumer.GetGroupId().ToString() : sourceDashboardConsumer.GetGroupId().ToString();
            settings["team"] = (object) dictionary;
            widget1.Settings = settings.Serialize<Dictionary<string, object>>(true);
          }
          else if (sourceDashboardConsumer.GetDataspaceId() != targetDashboardConsumer.GetDataspaceId())
            widget1.Settings = (string) null;
        }
        else
          widget1.Settings = !settings.Keys.Contains<string>("isLegacy") || bool.Parse(settings["isLegacy"].ToString()) ? (string) null : (!(targetDashboardConsumer.GetDataspaceId() == sourceDashboardConsumer.GetDataspaceId()) ? this.HandleCrossProjectSettingsForNewSprintBurndown(settings, targetDashboardConsumer) : this.HandleSameProjectSettingsForNewSprintBurndown(settings, sourceDashboardConsumer, targetDashboardConsumer));
      }
      else if (targetDashboardConsumer.GetScope() == DashboardScope.Project_Team || targetDashboardConsumer.GetDataspaceId() == sourceDashboardConsumer.GetDataspaceId())
        widget1.Settings = new Dictionary<string, object>()
        {
          ["isLegacy"] = ((object) true),
          ["team"] = ((object) new Dictionary<string, string>()
          {
            ["projectId"] = targetDashboardConsumer.GetDataspaceId().ToString(),
            ["teamId"] = targetDashboardConsumer.GetGroupId().ToString()
          })
        }.Serialize<Dictionary<string, object>>(true);
      else
        widget1.Settings = (string) null;
      return widget1;
    }
  }
}
