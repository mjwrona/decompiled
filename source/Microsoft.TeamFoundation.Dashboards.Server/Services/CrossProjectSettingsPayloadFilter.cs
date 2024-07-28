// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.CrossProjectSettingsPayloadFilter
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.CrossProjectSettings;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public class CrossProjectSettingsPayloadFilter
  {
    internal const string BurndownContributionId = "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.BurndownWidget";
    internal const string BurnupContributionId = "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.BurnupWidget";
    internal const string WITChartContributionId = "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.WitChartWidget";
    private const string UserColorsFieldName = "userColors";
    private const string HasCrossProjectSettingsPropertyName = "hasCrossProjectSettings";

    public static void FilterInsecurePublicWidgetConfigurations(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Widget> widgets)
    {
      if (CrossProjectSettingsSecurityHelper.CanUserViewCrossProjectSettings(requestContext))
        return;
      foreach (Widget widget in widgets)
        CrossProjectSettingsPayloadFilter.FilterWidgetImpl(requestContext, projectId, widget);
    }

    public static void FilterWidget(
      IVssRequestContext requestContext,
      Guid projectId,
      Widget widget)
    {
      if (CrossProjectSettingsSecurityHelper.CanUserViewCrossProjectSettings(requestContext))
        return;
      CrossProjectSettingsPayloadFilter.FilterWidgetImpl(requestContext, projectId, widget);
    }

    private static void FilterWidgetImpl(
      IVssRequestContext requestContext,
      Guid projectId,
      Widget widget)
    {
      string contributionId = widget.ContributionId;
      if (widget.ContributionId == "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.WitChartWidget")
      {
        CrossProjectSettingsPayloadFilter.FilterChartSettings(requestContext, widget);
      }
      else
      {
        if (!widget.CanStoreCrossProjectSettings)
          return;
        CrossProjectSettingsPayloadFilter.FilterCrossProjectSettings(requestContext, projectId, widget);
      }
    }

    private static void FilterChartSettings(IVssRequestContext requestContext, Widget widget)
    {
      Dictionary<string, object> dictionary = (Dictionary<string, object>) null;
      if (JsonUtilities.TryDeserialize<Dictionary<string, object>>(widget.Settings, out dictionary, true) && dictionary != null)
        dictionary.Remove("userColors");
      widget.Settings = dictionary != null ? dictionary.Serialize<Dictionary<string, object>>() : (string) null;
    }

    private static void FilterCrossProjectSettings(
      IVssRequestContext requestContext,
      Guid currentProjectId,
      Widget widget)
    {
      bool flag1 = false;
      try
      {
        CrossProjectWidgetConfiguration widgetConfiguration = (CrossProjectWidgetConfiguration) null;
        if (string.IsNullOrWhiteSpace(widget.Settings))
          flag1 = true;
        else if (JsonUtilities.TryDeserialize<CrossProjectWidgetConfiguration>(widget.Settings, out widgetConfiguration, true))
        {
          if (widgetConfiguration.HasCrossProjectSettings.HasValue && !widgetConfiguration.HasCrossProjectSettings.Value)
          {
            flag1 = true;
          }
          else
          {
            if (!(widget.ContributionId == "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.BurndownWidget"))
            {
              if (!(widget.ContributionId == "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.BurnupWidget"))
                goto label_20;
            }
            if (widgetConfiguration.Teams != null)
            {
              if (widgetConfiguration.Teams.Length != 0)
              {
                IEnumerable<string> strings = ((IEnumerable<ProjectTeamTuple>) widgetConfiguration.Teams).Select<ProjectTeamTuple, string>((Func<ProjectTeamTuple, string>) (o => o.ProjectId));
                bool flag2 = true;
                foreach (string input in strings)
                {
                  Guid result;
                  flag2 = ((flag2 ? 1 : 0) & (!Guid.TryParse(input, out result) ? 0 : (result == currentProjectId ? 1 : 0))) != 0;
                }
                flag1 = flag2;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(10017384, "DashboardService", "DashboardService.ValidateCrossProjectSettings", ex);
      }
label_20:
      if (flag1)
        return;
      widget.Settings = (string) null;
      widget.AreSettingsBlockedForUser = true;
    }
  }
}
