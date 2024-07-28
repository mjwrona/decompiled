// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Utils.TeamSettingsControlHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Utils
{
  public static class TeamSettingsControlHelpers
  {
    public static BugsBehaviorControlStateOptions GetBugBehaviorState(
      IVssRequestContext requestContext,
      ProjectInfo project)
    {
      string str = (string) null;
      string typeName = (string) null;
      ProjectProcessConfiguration processSettings = requestContext.GetService<IProjectConfigurationService>().GetProcessSettings(requestContext, project.Uri, false);
      BugsBehaviorSettingUIStateEnum settingUiStateEnum;
      if (!processSettings.IsConfigValidForBugsBehavior(requestContext, project.Uri))
      {
        settingUiStateEnum = !WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) ? BugsBehaviorSettingUIStateEnum.ShowInvalidConfigurationMessage : BugsBehaviorSettingUIStateEnum.ShowInvalidConfigurationMessageHosted;
      }
      else
      {
        IEnumerable<string> fieldsForBugBehavior = ProjectProcessConfigurationHelpers.GetMissingFieldsForBugBehavior(requestContext, processSettings, project.Name, out typeName);
        if (fieldsForBugBehavior.Any<string>())
        {
          settingUiStateEnum = BugsBehaviorSettingUIStateEnum.ShowWithDegradedExperienceWarning;
          str = string.Join(", ", fieldsForBugBehavior);
        }
        else
          settingUiStateEnum = BugsBehaviorSettingUIStateEnum.Show;
      }
      return new BugsBehaviorControlStateOptions()
      {
        uiState = settingUiStateEnum,
        missingFields = str,
        workItemType = typeName
      };
    }
  }
}
