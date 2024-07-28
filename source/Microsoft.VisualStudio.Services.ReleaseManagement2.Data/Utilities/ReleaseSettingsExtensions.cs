// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseSettingsExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ReleaseSettingsExtensions
  {
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSettings ToWebApiReleaseSettings(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSettings releaseSettings)
    {
      if (releaseSettings == null)
        throw new ArgumentNullException(nameof (releaseSettings));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSettings apiReleaseSettings = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSettings) null;
      if (releaseSettings.RetentionSettings != null)
        apiReleaseSettings = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSettings()
        {
          RetentionSettings = releaseSettings.RetentionSettings.ToWebApiRetentionSettings()
        };
      if (releaseSettings.ComplianceSettings != null)
      {
        if (apiReleaseSettings == null)
          apiReleaseSettings = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSettings();
        apiReleaseSettings.ComplianceSettings = releaseSettings.ComplianceSettings.ToWebApiComplianceSettings();
      }
      return apiReleaseSettings;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSettings ToServerReleaseSettings(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSettings releaseSettings)
    {
      if (releaseSettings == null)
        throw new ArgumentNullException(nameof (releaseSettings));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSettings serverReleaseSettings = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSettings) null;
      if (releaseSettings.RetentionSettings != null)
        serverReleaseSettings = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSettings()
        {
          RetentionSettings = releaseSettings.RetentionSettings.ToServerRetentionSettings()
        };
      if (releaseSettings.ComplianceSettings != null)
      {
        if (serverReleaseSettings == null)
          serverReleaseSettings = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSettings();
        serverReleaseSettings.ComplianceSettings = releaseSettings.ComplianceSettings.ToServerComplianceSettings();
      }
      return serverReleaseSettings;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.RetentionSettings ToWebApiRetentionSettings(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.RetentionSettings retentionSettings)
    {
      if (retentionSettings == null)
        throw new ArgumentNullException(nameof (retentionSettings));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.RetentionSettings()
      {
        DefaultEnvironmentRetentionPolicy = retentionSettings.DefaultEnvironmentRetentionPolicy.ConvertToWebApiEnvironmentRetentionPolicy(),
        MaximumEnvironmentRetentionPolicy = retentionSettings.MaximumEnvironmentRetentionPolicy.ConvertToWebApiEnvironmentRetentionPolicy(),
        DaysToKeepDeletedReleases = retentionSettings.DaysToKeepDeletedReleases
      };
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ComplianceSettings ToWebApiComplianceSettings(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ComplianceSettings complianceSettings)
    {
      return complianceSettings != null ? new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ComplianceSettings()
      {
        CheckForCredentialsAndOtherSecrets = complianceSettings.CheckForCredentialsAndOtherSecrets.GetValueOrDefault()
      } : throw new ArgumentNullException(nameof (complianceSettings));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.RetentionSettings ToServerRetentionSettings(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.RetentionSettings retentionSettings)
    {
      if (retentionSettings == null)
        throw new ArgumentNullException(nameof (retentionSettings));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.RetentionSettings()
      {
        DefaultEnvironmentRetentionPolicy = retentionSettings.DefaultEnvironmentRetentionPolicy.ConvertToServerEnvironmentRetentionPolicy(),
        MaximumEnvironmentRetentionPolicy = retentionSettings.MaximumEnvironmentRetentionPolicy.ConvertToServerEnvironmentRetentionPolicy(),
        DaysToKeepDeletedReleases = retentionSettings.DaysToKeepDeletedReleases
      };
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ComplianceSettings ToServerComplianceSettings(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ComplianceSettings complianceSettings)
    {
      return complianceSettings != null ? new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ComplianceSettings()
      {
        CheckForCredentialsAndOtherSecrets = new bool?(complianceSettings.CheckForCredentialsAndOtherSecrets)
      } : throw new ArgumentNullException(nameof (complianceSettings));
    }
  }
}
