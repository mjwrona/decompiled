// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.SprintBurndownWidgetAdapter
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public class SprintBurndownWidgetAdapter : WidgetAdapterBase, IWidgetAdapter
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
      widget1.Settings = widget1.Settings == null || !JsonUtilities.TryDeserialize<Dictionary<string, object>>(widget1.Settings, out settings, true) || !settings.Keys.Contains<string>("isLegacy") || bool.Parse(settings["isLegacy"].ToString()) ? (string) null : (!(targetDashboardConsumer.GetDataspaceId() == sourceDashboardConsumer.GetDataspaceId()) ? this.HandleCrossProjectSettingsForNewSprintBurndown(settings, targetDashboardConsumer) : this.HandleSameProjectSettingsForNewSprintBurndown(settings, sourceDashboardConsumer, targetDashboardConsumer));
      return widget1;
    }

    protected virtual string HandleSameProjectSettingsForNewSprintBurndown(
      Dictionary<string, object> settings,
      IDashboardConsumer sourceDashboardConsumer,
      IDashboardConsumer targetDashboardConsumer)
    {
      settings["totalScopeTrendlineEnabled"] = (object) (targetDashboardConsumer.GetScope() == DashboardScope.Project_Team).ToString();
      if (targetDashboardConsumer.GetScope() == DashboardScope.Project_Team && targetDashboardConsumer.GetGroupId() != sourceDashboardConsumer.GetGroupId())
      {
        settings["iterationId"] = (object) string.Empty;
        settings["iterationPath"] = (object) string.Empty;
        settings["timePeriodConfiguration"] = (object) string.Empty;
        Dictionary<string, string> dictionary;
        if (JsonUtilities.TryDeserialize<Dictionary<string, string>>(settings["team"].ToString(), out dictionary, true))
        {
          dictionary["projectId"] = targetDashboardConsumer.GetDataspaceId().ToString();
          dictionary["teamId"] = targetDashboardConsumer.GetGroupId().ToString();
          settings["team"] = (object) dictionary;
        }
        else
          settings["team"] = (object) string.Empty;
      }
      return settings.Serialize<Dictionary<string, object>>(true);
    }

    protected virtual string HandleCrossProjectSettingsForNewSprintBurndown(
      Dictionary<string, object> settings,
      IDashboardConsumer targetDashboardConsumer)
    {
      Dictionary<string, object> dictionary;
      string str;
      if (targetDashboardConsumer.GetScope() == DashboardScope.Project_Team && JsonUtilities.TryDeserialize<Dictionary<string, object>>(settings["team"].ToString(), out dictionary, true))
      {
        settings["iterationId"] = (object) string.Empty;
        settings["iterationPath"] = (object) string.Empty;
        settings["timePeriodConfiguration"] = (object) string.Empty;
        settings["totalScopeTrendlineEnabled"] = (object) (targetDashboardConsumer.GetScope() == DashboardScope.Project_Team).ToString();
        dictionary["projectId"] = (object) targetDashboardConsumer.GetDataspaceId().ToString();
        dictionary["teamId"] = (object) targetDashboardConsumer.GetGroupId().ToString();
        settings["team"] = (object) dictionary;
        str = settings.Serialize<Dictionary<string, object>>(true);
      }
      else
        str = (string) null;
      return str;
    }
  }
}
