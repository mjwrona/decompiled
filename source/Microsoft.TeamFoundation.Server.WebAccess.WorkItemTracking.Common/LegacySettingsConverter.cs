// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.LegacySettingsConverter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal static class LegacySettingsConverter
  {
    public static CommonProjectConfiguration GetCommonSettings(
      IVssRequestContext requestContext,
      string projectUri)
    {
      return LegacySettingsConverter.GetCommonSettings(requestContext, projectUri, true);
    }

    public static CommonProjectConfiguration GetCommonSettings(
      IVssRequestContext requestContext,
      string projectUri,
      bool validateSettings)
    {
      return ProjectConfigurationCompatibilityEngine.GetCommonProjectConfiguration(requestContext.GetService<ProjectConfigurationService>().GetProcessSettings(requestContext, projectUri, validateSettings));
    }

    public static void SetCommonSettings(
      IVssRequestContext requestContext,
      string projectUri,
      CommonProjectConfiguration settings)
    {
      AgileProjectConfiguration agileSettings = LegacySettingsConverter.GetAgileSettings(requestContext, projectUri, false);
      settings.Validate(requestContext, projectUri, false);
      requestContext.GetService<ProjectConfigurationService>().SetProcessSettings(requestContext, projectUri, ProjectConfigurationCompatibilityEngine.GetProjectProcessConfiguration(settings, agileSettings), agileSettings.IsDefault);
    }

    public static AgileProjectConfiguration GetAgileSettings(
      IVssRequestContext requestContext,
      string projectUri)
    {
      return LegacySettingsConverter.GetAgileSettings(requestContext, projectUri, true);
    }

    public static AgileProjectConfiguration GetAgileSettings(
      IVssRequestContext requestContext,
      string projectUri,
      bool validateSettings)
    {
      return ProjectConfigurationCompatibilityEngine.GetAgileProjectConfiguration(requestContext.GetService<ProjectConfigurationService>().GetProcessSettings(requestContext, projectUri, validateSettings));
    }

    public static void SetAgileSettings(
      IVssRequestContext requestContext,
      string projectUri,
      AgileProjectConfiguration settings)
    {
      CommonProjectConfiguration commonSettings = LegacySettingsConverter.GetCommonSettings(requestContext, projectUri, false);
      settings.Validate(requestContext, projectUri, false);
      requestContext.GetService<ProjectConfigurationService>().SetProcessSettings(requestContext, projectUri, ProjectConfigurationCompatibilityEngine.GetProjectProcessConfiguration(commonSettings, settings));
    }
  }
}
