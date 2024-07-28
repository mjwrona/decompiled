// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.BurndownWidgetAdapter
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public class BurndownWidgetAdapter : WidgetAdapterBase, IWidgetAdapter
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
      if (widget1.Settings != null && widget1.Settings.Length <= 20000)
      {
        if (targetDashboardConsumer.GetScope() == DashboardScope.Project_Team)
        {
          if (targetDashboardConsumer.GetDataspaceId() == sourceDashboardConsumer.GetDataspaceId())
          {
            int num = widget1.Settings.IndexOf(targetDashboardConsumer.GetGroupId().ToString());
            if (sourceDashboardConsumer.GetScope() == DashboardScope.Project_Team && widget1.Settings.IndexOf(sourceDashboardConsumer.GetGroupId().ToString()) > -1 && num == -1)
            {
              widget1.Settings = widget1.Settings.Replace(sourceDashboardConsumer.GetGroupId().ToString(), targetDashboardConsumer.GetGroupId().ToString());
            }
            else
            {
              Dictionary<string, object> dictionary;
              List<ProjectTeamFilter> projectTeamFilterList;
              if (num == -1 && JsonUtilities.TryDeserialize<Dictionary<string, object>>(widget1.Settings, out dictionary, true) && JsonUtilities.TryDeserialize<List<ProjectTeamFilter>>(dictionary["teams"].ToString(), out projectTeamFilterList))
              {
                projectTeamFilterList.Add(new ProjectTeamFilter()
                {
                  projectId = targetDashboardConsumer.GetDataspaceId().ToString(),
                  teamId = targetDashboardConsumer.GetGroupId().ToString()
                });
                dictionary["teams"] = (object) projectTeamFilterList;
                widget1.Settings = dictionary.Serialize<Dictionary<string, object>>(true);
              }
            }
          }
          else
          {
            Dictionary<string, object> dictionary;
            List<ProjectTeamFilter> projectTeamFilterList;
            if (widget1.Settings.IndexOf(targetDashboardConsumer.GetGroupId().ToString()) == -1 && JsonUtilities.TryDeserialize<Dictionary<string, object>>(widget1.Settings, out dictionary, true) && JsonUtilities.TryDeserialize<List<ProjectTeamFilter>>(dictionary["teams"].ToString(), out projectTeamFilterList))
            {
              projectTeamFilterList.Add(new ProjectTeamFilter()
              {
                projectId = targetDashboardConsumer.GetDataspaceId().ToString(),
                teamId = targetDashboardConsumer.GetGroupId().ToString()
              });
              dictionary["teams"] = (object) projectTeamFilterList;
              widget1.Settings = dictionary.Serialize<Dictionary<string, object>>(true);
            }
          }
        }
      }
      else
        widget1.Settings = (string) null;
      return widget1;
    }
  }
}
