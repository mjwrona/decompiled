// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.DashboardSettings
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public class DashboardSettings
  {
    internal const int MaxDashboardsPerGroup = 500;
    internal const int MaxWidgetsPerDashboard = 200;
    internal const int MaxRowNumber = 200;
    internal const int MaxColumnNumber = 25;
    private const string DashboardRegistryPrefix = "/Configuration/Dashboards/";
    internal static readonly RegistryQuery s_maxDashboardsPerGroupRegistryPath = new RegistryQuery("/Configuration/Dashboards/maxDashboardsPerGroup");
    internal static readonly RegistryQuery s_maxWidgetsPerDashboardRegistryPath = new RegistryQuery("/Configuration/Dashboards/maxWidgetsPerDashboard");
    internal static readonly RegistryQuery s_maxRowNumberRegistryPath = new RegistryQuery("/Configuration/Dashboards/maxRowNumber");
    internal static readonly RegistryQuery s_maxColumnNumberRegistryPath = new RegistryQuery("/Configuration/Dashboards/maxColumnNumber");

    public static int GetMaxDashboardsPerGroup(IVssRequestContext requestContext) => DashboardSettings.GetValueFromRegistry<int>(requestContext, DashboardSettings.s_maxDashboardsPerGroupRegistryPath, 500);

    public static int GetMaxWidgetsPerDashboard(IVssRequestContext requestContext) => DashboardSettings.GetValueFromRegistry<int>(requestContext, DashboardSettings.s_maxWidgetsPerDashboardRegistryPath, 200);

    public static int GetMaxRowNumber(IVssRequestContext requestContext) => DashboardSettings.GetValueFromRegistry<int>(requestContext, DashboardSettings.s_maxRowNumberRegistryPath, 200);

    public static int GetMaxColumnNumber(IVssRequestContext requestContext) => DashboardSettings.GetValueFromRegistry<int>(requestContext, DashboardSettings.s_maxColumnNumberRegistryPath, 25);

    private static T GetValueFromRegistry<T>(
      IVssRequestContext requestContext,
      RegistryQuery registryQuery,
      T defaultValue)
    {
      return requestContext.GetService<IVssRegistryService>().GetValue<T>(requestContext, in registryQuery, true, defaultValue);
    }
  }
}
