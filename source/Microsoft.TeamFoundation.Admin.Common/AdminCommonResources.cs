// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.AdminCommonResources
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Admin
{
  internal static class AdminCommonResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (AdminCommonResources), typeof (AdminCommonResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => AdminCommonResources.s_resMgr;

    private static string Get(string resourceName) => AdminCommonResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? AdminCommonResources.Get(resourceName) : AdminCommonResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) AdminCommonResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? AdminCommonResources.GetInt(resourceName) : (int) AdminCommonResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) AdminCommonResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? AdminCommonResources.GetBool(resourceName) : (bool) AdminCommonResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => AdminCommonResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = AdminCommonResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string FeatureNotInstalled(object arg0) => AdminCommonResources.Format(nameof (FeatureNotInstalled), arg0);

    public static string FeatureNotInstalled(object arg0, CultureInfo culture) => AdminCommonResources.Format(nameof (FeatureNotInstalled), culture, arg0);

    public static string FeatureMustBeInstalledToSetIsConfiguredFlag(object arg0) => AdminCommonResources.Format(nameof (FeatureMustBeInstalledToSetIsConfiguredFlag), arg0);

    public static string FeatureMustBeInstalledToSetIsConfiguredFlag(
      object arg0,
      CultureInfo culture)
    {
      return AdminCommonResources.Format(nameof (FeatureMustBeInstalledToSetIsConfiguredFlag), culture, arg0);
    }

    public static string IgnoringFormatexceptionParsingVersion(object arg0, object arg1) => AdminCommonResources.Format(nameof (IgnoringFormatexceptionParsingVersion), arg0, arg1);

    public static string IgnoringFormatexceptionParsingVersion(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdminCommonResources.Format(nameof (IgnoringFormatexceptionParsingVersion), culture, arg0, arg1);
    }

    public static string IgnoringExceptionDuringLoadpermachinefeature(object arg0) => AdminCommonResources.Format(nameof (IgnoringExceptionDuringLoadpermachinefeature), arg0);

    public static string IgnoringExceptionDuringLoadpermachinefeature(
      object arg0,
      CultureInfo culture)
    {
      return AdminCommonResources.Format(nameof (IgnoringExceptionDuringLoadpermachinefeature), culture, arg0);
    }

    public static string LogDisplayConfiguration() => AdminCommonResources.Get(nameof (LogDisplayConfiguration));

    public static string LogDisplayConfiguration(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplayConfiguration), culture);

    public static string LogDisplayTeamProjectCollection() => AdminCommonResources.Get(nameof (LogDisplayTeamProjectCollection));

    public static string LogDisplayTeamProjectCollection(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplayTeamProjectCollection), culture);

    public static string LogDisplayDeploy() => AdminCommonResources.Get(nameof (LogDisplayDeploy));

    public static string LogDisplayDeploy(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplayDeploy), culture);

    public static string LogDisplayUpgrade() => AdminCommonResources.Get(nameof (LogDisplayUpgrade));

    public static string LogDisplayUpgrade(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplayUpgrade), culture);

    public static string LogDisplayAccount() => AdminCommonResources.Get(nameof (LogDisplayAccount));

    public static string LogDisplayAccount(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplayAccount), culture);

    public static string LogDisplaySettings() => AdminCommonResources.Get(nameof (LogDisplaySettings));

    public static string LogDisplaySettings(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplaySettings), culture);

    public static string LogDisplayApplicationTier() => AdminCommonResources.Get(nameof (LogDisplayApplicationTier));

    public static string LogDisplayApplicationTier(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplayApplicationTier), culture);

    public static string LogDisplayBuild() => AdminCommonResources.Get(nameof (LogDisplayBuild));

    public static string LogDisplayBuild(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplayBuild), culture);

    public static string LogDisplayProxy() => AdminCommonResources.Get(nameof (LogDisplayProxy));

    public static string LogDisplayProxy(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplayProxy), culture);

    public static string LogDisplayLab() => AdminCommonResources.Get(nameof (LogDisplayLab));

    public static string LogDisplayLab(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplayLab), culture);

    public static string LogDisplaySharePoint() => AdminCommonResources.Get(nameof (LogDisplaySharePoint));

    public static string LogDisplaySharePoint(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplaySharePoint), culture);

    public static string LogDisplayReporting() => AdminCommonResources.Get(nameof (LogDisplayReporting));

    public static string LogDisplayReporting(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplayReporting), culture);

    public static string LogDisplayURLs() => AdminCommonResources.Get(nameof (LogDisplayURLs));

    public static string LogDisplayURLs(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplayURLs), culture);

    public static string LogDisplayError() => AdminCommonResources.Get(nameof (LogDisplayError));

    public static string LogDisplayError(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplayError), culture);

    public static string LogDisplayServicing() => AdminCommonResources.Get(nameof (LogDisplayServicing));

    public static string LogDisplayServicing(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplayServicing), culture);

    public static string FeatureRegistryKeyNotFound(object arg0) => AdminCommonResources.Format(nameof (FeatureRegistryKeyNotFound), arg0);

    public static string FeatureRegistryKeyNotFound(object arg0, CultureInfo culture) => AdminCommonResources.Format(nameof (FeatureRegistryKeyNotFound), culture, arg0);

    public static string FeatureMustBeInstalledToAccessInformation(object arg0) => AdminCommonResources.Format(nameof (FeatureMustBeInstalledToAccessInformation), arg0);

    public static string FeatureMustBeInstalledToAccessInformation(object arg0, CultureInfo culture) => AdminCommonResources.Format(nameof (FeatureMustBeInstalledToAccessInformation), culture, arg0);

    public static string IgnoringArgumentexceptionParsingVersion(object arg0, object arg1) => AdminCommonResources.Format(nameof (IgnoringArgumentexceptionParsingVersion), arg0, arg1);

    public static string IgnoringArgumentexceptionParsingVersion(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdminCommonResources.Format(nameof (IgnoringArgumentexceptionParsingVersion), culture, arg0, arg1);
    }

    public static string SettingIsConfiguredFlag(object arg0, object arg1) => AdminCommonResources.Format(nameof (SettingIsConfiguredFlag), arg0, arg1);

    public static string SettingIsConfiguredFlag(object arg0, object arg1, CultureInfo culture) => AdminCommonResources.Format(nameof (SettingIsConfiguredFlag), culture, arg0, arg1);

    public static string FailedToFindInstallPath(object arg0) => AdminCommonResources.Format(nameof (FailedToFindInstallPath), arg0);

    public static string FailedToFindInstallPath(object arg0, CultureInfo culture) => AdminCommonResources.Format(nameof (FailedToFindInstallPath), culture, arg0);

    public static string UnableToFindConfigurationDatabaseSetting(object arg0) => AdminCommonResources.Format(nameof (UnableToFindConfigurationDatabaseSetting), arg0);

    public static string UnableToFindConfigurationDatabaseSetting(object arg0, CultureInfo culture) => AdminCommonResources.Format(nameof (UnableToFindConfigurationDatabaseSetting), culture, arg0);

    public static string ValueAttributeNotFound(object arg0) => AdminCommonResources.Format(nameof (ValueAttributeNotFound), arg0);

    public static string ValueAttributeNotFound(object arg0, CultureInfo culture) => AdminCommonResources.Format(nameof (ValueAttributeNotFound), culture, arg0);

    public static string AppSettingsValueforConfigurationDatabase(object arg0) => AdminCommonResources.Format(nameof (AppSettingsValueforConfigurationDatabase), arg0);

    public static string AppSettingsValueforConfigurationDatabase(object arg0, CultureInfo culture) => AdminCommonResources.Format(nameof (AppSettingsValueforConfigurationDatabase), culture, arg0);

    public static string ConfigurationDatabaseValueIsInvalid(object arg0, object arg1) => AdminCommonResources.Format(nameof (ConfigurationDatabaseValueIsInvalid), arg0, arg1);

    public static string ConfigurationDatabaseValueIsInvalid(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdminCommonResources.Format(nameof (ConfigurationDatabaseValueIsInvalid), culture, arg0, arg1);
    }

    public static string SqlLoginNotSupportedOnPrem() => AdminCommonResources.Get(nameof (SqlLoginNotSupportedOnPrem));

    public static string SqlLoginNotSupportedOnPrem(CultureInfo culture) => AdminCommonResources.Get(nameof (SqlLoginNotSupportedOnPrem), culture);

    public static string SqlServer2016SP1OrAbove() => AdminCommonResources.Get(nameof (SqlServer2016SP1OrAbove));

    public static string SqlServer2016SP1OrAbove(CultureInfo culture) => AdminCommonResources.Get(nameof (SqlServer2016SP1OrAbove), culture);

    public static string SqlServer2017PreRelease() => AdminCommonResources.Get(nameof (SqlServer2017PreRelease));

    public static string SqlServer2017PreRelease(CultureInfo culture) => AdminCommonResources.Get(nameof (SqlServer2017PreRelease), culture);

    public static string SqlServer2016PreRelease() => AdminCommonResources.Get(nameof (SqlServer2016PreRelease));

    public static string SqlServer2016PreRelease(CultureInfo culture) => AdminCommonResources.Get(nameof (SqlServer2016PreRelease), culture);

    public static string SqlServer2014PreRelease() => AdminCommonResources.Get(nameof (SqlServer2014PreRelease));

    public static string SqlServer2014PreRelease(CultureInfo culture) => AdminCommonResources.Get(nameof (SqlServer2014PreRelease), culture);

    public static string SqlServer2012PreRelease() => AdminCommonResources.Get(nameof (SqlServer2012PreRelease));

    public static string SqlServer2012PreRelease(CultureInfo culture) => AdminCommonResources.Get(nameof (SqlServer2012PreRelease), culture);

    public static string SqlServer2008R2PreRelease() => AdminCommonResources.Get(nameof (SqlServer2008R2PreRelease));

    public static string SqlServer2008R2PreRelease(CultureInfo culture) => AdminCommonResources.Get(nameof (SqlServer2008R2PreRelease), culture);

    public static string SqlServer2008PreRelease() => AdminCommonResources.Get(nameof (SqlServer2008PreRelease));

    public static string SqlServer2008PreRelease(CultureInfo culture) => AdminCommonResources.Get(nameof (SqlServer2008PreRelease), culture);

    public static string LogDisplayDiagnose() => AdminCommonResources.Get(nameof (LogDisplayDiagnose));

    public static string LogDisplayDiagnose(CultureInfo culture) => AdminCommonResources.Get(nameof (LogDisplayDiagnose), culture);
  }
}
