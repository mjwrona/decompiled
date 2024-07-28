// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ProjectSettings.ReportingSettings
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Reporting;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client.ProjectSettings
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ReportingSettings
  {
    public static bool IsReportingConfigured(TfsTeamProjectCollection tpc)
    {
      TpcSettingsStore service = tpc.GetService<TpcSettingsStore>();
      return service.ProjectCollectionSettings != null && !string.IsNullOrEmpty(service.ProjectCollectionSettings.ReportFolder) && !(service.ProjectCollectionSettings.ReportServerServiceLocation == (Uri) null) && !(service.ProjectCollectionSettings.ReportsManagerServiceLocation == (Uri) null);
    }

    public static bool IsReportingConfigured(TfsTeamProjectCollection tpc, Uri projectUri)
    {
      if (!ReportingSettings.IsReportingConfigured(tpc))
        return false;
      TeamProjectSettings teamProjectSettings = tpc.GetService<TpcSettingsStore>().GetTeamProjectSettings(projectUri);
      return teamProjectSettings != null && !string.IsNullOrEmpty(teamProjectSettings.ReportFolder);
    }

    public static bool IsReportingCubeConfigured(
      TfsTeamProjectCollection tpc,
      out string server,
      out string database,
      out string connectionStr)
    {
      server = string.Empty;
      database = string.Empty;
      connectionStr = string.Empty;
      TpcSettingsStore service = tpc.GetService<TpcSettingsStore>();
      if (service.ProjectCollectionSettings == null || string.IsNullOrEmpty(service.ProjectCollectionSettings.CubeServerName) || string.IsNullOrEmpty(service.ProjectCollectionSettings.CubeDatabaseName) || string.IsNullOrEmpty(service.ProjectCollectionSettings.CubeConnectionString))
        return false;
      server = service.ProjectCollectionSettings.CubeServerName;
      database = service.ProjectCollectionSettings.CubeDatabaseName;
      connectionStr = service.ProjectCollectionSettings.CubeConnectionString;
      return true;
    }

    public static Uri GetReportServiceUri(TfsTeamProjectCollection tpc)
    {
      if (ReportingSettings.IsReportingConfigured(tpc))
      {
        Uri serverServiceLocation = tpc.GetService<TpcSettingsStore>().ProjectCollectionSettings.ReportServerServiceLocation;
        if (serverServiceLocation != (Uri) null)
          return UriUtility.Combine(serverServiceLocation, "ReportService2005.asmx", false);
      }
      return (Uri) null;
    }

    public static string GetBaseProjectCollectionPath(TfsTeamProjectCollection tpc)
    {
      TpcSettingsStore service = tpc.GetService<TpcSettingsStore>();
      return service.ProjectCollectionSettings == null ? string.Empty : service.ProjectCollectionSettings.ReportFolder ?? string.Empty;
    }

    public static string GetBaseProjectPath(TfsTeamProjectCollection tpc, string projectUri)
    {
      TpcSettingsStore service = tpc.GetService<TpcSettingsStore>();
      TeamProjectSettings teamProjectSettings = service.GetTeamProjectSettings(new Uri(projectUri));
      return service.ProjectCollectionSettings == null || teamProjectSettings == null ? string.Empty : teamProjectSettings.ReportFolder ?? string.Empty;
    }

    public static Uri GetReportSiteUri(TfsTeamProjectCollection tpc, string projectUri)
    {
      TeamProjectSettings teamProjectSettings = tpc.GetService<TpcSettingsStore>().GetTeamProjectSettings(new Uri(projectUri));
      return teamProjectSettings != null && !string.IsNullOrEmpty(teamProjectSettings.ReportFolder) ? ReportingUtilities.GetReportManagerItemUrl(ReportingSettings.GetReportManagerUri(tpc), teamProjectSettings.ReportFolder) : new Uri(NonConfiguredSiteHelper.GenerateNotFoundUri(NonConfiguredSiteHelper.SiteType.Reporting));
    }

    public static Uri GetReportManagerUri(TfsTeamProjectCollection tpc) => ReportingSettings.IsReportingConfigured(tpc) ? tpc.GetService<TpcSettingsStore>().ProjectCollectionSettings.ReportsManagerServiceLocation : (Uri) null;
  }
}
