// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.FeatureHelper
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TfsBranding;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.Admin
{
  internal static class FeatureHelper
  {
    public static void SetFeatureConfiguredFlag(
      FeatureType featureType,
      bool isConfigured,
      ITFLogger logger)
    {
      logger.Info(AdminCommonResources.SettingIsConfiguredFlag((object) featureType, (object) isConfigured));
      IFeature feature = InstalledFeatureService.Instance.TryGetFeature(featureType);
      if (feature == null)
        throw new TfsAdminException(AdminCommonResources.FeatureMustBeInstalledToSetIsConfiguredFlag((object) featureType));
      if (feature.IsConfigured == isConfigured)
        return;
      feature.IsConfigured = isConfigured;
      InstalledFeatureService.Instance.SaveFeatureState(feature);
    }

    public static bool GetFeatureConfiguredFlag(FeatureType featureType)
    {
      IFeature feature = InstalledFeatureService.Instance.TryGetFeature(featureType);
      return feature != null && feature.IsConfigured;
    }

    internal static string[] FindWebConfigPaths(FeatureType featureType)
    {
      List<string> stringList = new List<string>();
      switch (featureType)
      {
        case FeatureType.ApplicationTier:
          stringList.Add(FeatureHelper.FindWebConfigPath(ApplicationPoolType.ApplicationTier));
          break;
        case FeatureType.VersionControlProxy:
          stringList.Add(FeatureHelper.FindWebConfigPath(ApplicationPoolType.VersionControlProxy));
          break;
      }
      return stringList.ToArray();
    }

    public static string FindWebConfigPath(ApplicationPoolType webAppType)
    {
      FeatureType featureType;
      string path2;
      if (webAppType != ApplicationPoolType.ApplicationTier)
      {
        if (webAppType != ApplicationPoolType.VersionControlProxy)
          throw new ArgumentException((string) null, nameof (webAppType));
        featureType = FeatureType.VersionControlProxy;
        path2 = "Web Services\\VersionControlProxy\\proxy.config";
      }
      else
      {
        featureType = FeatureType.ApplicationTier;
        path2 = "Web Services\\web.config";
      }
      return Path.Combine(FeatureHelper.GetFeatureInstallPath(featureType), path2);
    }

    public static string TryGetFeatureInstallPath(FeatureType featureType)
    {
      IFeature feature = InstalledFeatureService.Instance.TryGetFeature(featureType);
      return feature == null || string.IsNullOrEmpty(feature.InstallPath) || !Directory.Exists(feature.InstallPath) ? (string) null : feature.InstallPath;
    }

    public static string GetFeatureInstallPath(FeatureType featureType)
    {
      string featureInstallPath = FeatureHelper.TryGetFeatureInstallPath(featureType);
      return !string.IsNullOrEmpty(featureInstallPath) ? featureInstallPath : throw new TfsAdminException(AdminCommonResources.FailedToFindInstallPath((object) featureType));
    }

    public static string GetConfigurationDbConnectionStringFromWebConfig() => FeatureHelper.GetConfigurationDbConnectionString(FeatureHelper.WebConfigPath);

    public static bool IsConfigurationDbOnSqlAzure => File.Exists(FeatureHelper.WebConfigPath) && SqlConnectionInfoFactory.IsSqlAzure(new SqlConnectionStringBuilder(FeatureHelper.GetConfigurationDbConnectionStringFromWebConfig()).DataSource);

    internal static string TfsServicingFilesPath => Path.Combine(FeatureHelper.GetFeatureInstallPath(FeatureType.Tools), "Deploy\\TfsServicingFiles");

    internal static string ReleaseManifestPath => Path.Combine(FeatureHelper.TfsServicingFilesPath, "ReleaseManifest.xml");

    internal static string EulaPath => TfsBrandingResources.GetCurrent("DevOps_server_LicenseFwlink");

    internal static string WebConfigPath => Path.Combine(FeatureHelper.GetFeatureInstallPath(FeatureType.ApplicationTier), "Web Services\\web.config");

    internal static string TfsJobAgentConfigPath => Path.Combine(FeatureHelper.GetFeatureInstallPath(FeatureType.ApplicationTier), "TFSJobAgent\\TfsJobAgent.exe.config");

    internal static string SshSreviceConfigPath => Path.Combine(FeatureHelper.GetFeatureInstallPath(FeatureType.ApplicationTier), "Web Services\\bin\\TeamFoundationSshService.exe.config");

    public static string GetConfigurationDbConnectionString(string webConfigPath)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.XmlResolver = (XmlResolver) null;
      using (FileStream input = File.OpenRead(webConfigPath))
      {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        xmlDocument.Load(XmlReader.Create((Stream) input, settings));
      }
      string inputString = ((xmlDocument.SelectSingleNode("//appSettings/add[@key='applicationDatabase']") ?? throw new TfsAdminException(AdminCommonResources.UnableToFindConfigurationDatabaseSetting((object) webConfigPath))).Attributes["value"] ?? throw new TfsAdminException(AdminCommonResources.ValueAttributeNotFound((object) webConfigPath))).Value;
      if (string.IsNullOrEmpty(inputString))
        throw new TfsAdminException(AdminCommonResources.AppSettingsValueforConfigurationDatabase((object) webConfigPath));
      if (xmlDocument.SelectSingleNode("//appSettings/add[@key='applicationDatabasePassword']") != null)
        throw new NotSupportedException(AdminCommonResources.SqlLoginNotSupportedOnPrem());
      string str = ConnectionStringUtility.DecryptAndNormalizeConnectionString(inputString);
      return !string.IsNullOrEmpty(str) ? str : throw new TfsAdminException(AdminCommonResources.ConfigurationDatabaseValueIsInvalid((object) inputString, (object) webConfigPath));
    }
  }
}
