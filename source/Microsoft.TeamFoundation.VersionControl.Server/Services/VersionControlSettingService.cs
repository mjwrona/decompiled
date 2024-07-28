// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Services.VersionControlSettingService
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Settings;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server.Services
{
  public static class VersionControlSettingService
  {
    public static bool ReadDisableOldTfvcCheckinPolicies(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return requestContext.GetService<ISettingsService>().GetValue<bool>(requestContext, SettingsUserScope.AllUsers, "Project", projectId.ToString(), "VersionControl/Repositories/DisableOldTfvcCheckinPolicies");
    }

    public static void UpdateDisableOldTfvcCheckinPolicies(
      IVssRequestContext requestContext,
      Guid projectId,
      bool value)
    {
      if (!requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "SourceControl.NewToggleToSwitchOldCheckinPoliciesSaving"))
        return;
      requestContext.GetService<ISettingsService>().SetValue(requestContext, SettingsUserScope.AllUsers, "Project", projectId.ToString(), "VersionControl/Repositories/DisableOldTfvcCheckinPolicies", (object) value);
      VersionControlSettingService.WriteUpdatedSettingToTelemetry(requestContext, "TFVC", "CheckinPoliciesSavingChanged", value, new Guid?(projectId));
    }

    private static void WriteUpdatedSettingToTelemetry(
      IVssRequestContext requestContext,
      string feature,
      string eventType,
      bool value,
      Guid? projectId)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("EventType", eventType);
      properties.Add("Level", projectId.HasValue ? "Project" : "Organization");
      properties.Add("ProjectId", projectId.HasValue ? (object) projectId.Value : (object) null);
      properties.Add("NewValue", value);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, TraceArea.VersionControlSetting, feature, properties);
    }
  }
}
