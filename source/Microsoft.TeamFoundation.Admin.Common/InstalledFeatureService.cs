// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.InstalledFeatureService
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Admin
{
  internal class InstalledFeatureService : IInstalledFeatureService
  {
    private static InstalledFeatureService s_instance;
    private static string s_buildNumber;
    private static string s_installPath;
    internal const string c_installedComponents = "InstalledComponents";
    internal const string c_installPath = "InstallPath";
    internal const string c_buildNumber = "BuildNumber";
    internal const string c_isConfigured = "IsConfigured";

    private InstalledFeatureService()
    {
    }

    public static InstalledFeatureService Instance
    {
      get
      {
        if (InstalledFeatureService.s_instance == null)
          InstalledFeatureService.s_instance = new InstalledFeatureService();
        return InstalledFeatureService.s_instance;
      }
    }

    public static string BuildNumber
    {
      get
      {
        if (InstalledFeatureService.s_buildNumber == null)
        {
          using (SafeHandle registryKey = OnPremRegistryUtil.OpenRegistrySubKey(OnPremRegistryUtil.GetCurrentVersionRootPath(), false))
            InstalledFeatureService.s_buildNumber = RegistryHelper.GetValue(registryKey, nameof (BuildNumber), (object) null) as string;
        }
        return InstalledFeatureService.s_buildNumber;
      }
    }

    public List<IFeature> ConfiguredFeatures => this.InstalledFeatures.Where<IFeature>((Func<IFeature, bool>) (feature => feature.IsConfigured)).ToList<IFeature>();

    public static string InstallPath
    {
      get
      {
        if (InstalledFeatureService.s_installPath == null)
          InstalledFeatureService.s_installPath = OnPremRegistryUtil.InstallPath;
        return InstalledFeatureService.s_installPath;
      }
    }

    public IFeature GetFeature(FeatureType type) => this.TryGetFeature(type) ?? throw new TfsAdminException(AdminCommonResources.FeatureNotInstalled((object) type));

    public IFeature TryGetFeature(FeatureType type)
    {
      foreach (IFeature installedFeature in InstalledFeatureService.Instance.InstalledFeatures)
      {
        if (installedFeature.Type == type)
          return installedFeature;
      }
      return (IFeature) null;
    }

    public event EventHandler FeatureStateChanged;

    public event EventHandler<ConfigurationStateChangedEventArgs> ConfigurationStateChanged;

    private void OnFeatureChanged()
    {
      EventHandler featureStateChanged = this.FeatureStateChanged;
      if (featureStateChanged == null)
        return;
      featureStateChanged((object) this, EventArgs.Empty);
    }

    private void RaiseConfigurationStateChanged(FeatureType type, bool isConfigured)
    {
      EventHandler<ConfigurationStateChangedEventArgs> configurationStateChanged = this.ConfigurationStateChanged;
      if (configurationStateChanged == null)
        return;
      configurationStateChanged((object) this, new ConfigurationStateChangedEventArgs(type, isConfigured));
    }

    public bool IsInstalled(FeatureType type) => this.TryGetFeature(type) != null;

    public IEnumerable<IFeature> InstalledFeatures => (IEnumerable<IFeature>) this.LoadFeatures();

    public void SaveFeatureState(IFeature feature)
    {
      using (SafeHandle registryKey = OnPremRegistryUtil.OpenRegistrySubKey(InstalledFeatureService.GetFeatureKeyPath(feature.Version.ToString(), feature.Name), true))
      {
        if (registryKey == null)
          throw new TfsAdminException(AdminCommonResources.FeatureMustBeInstalledToSetIsConfiguredFlag((object) feature.Type));
        int num = feature.IsConfigured ? 1 : 0;
        RegistryHelper.SetValue(registryKey, "IsConfigured", num);
        this.RaiseConfigurationStateChanged(feature.Type, feature.IsConfigured);
        this.OnFeatureChanged();
      }
    }

    public static void WriteFeatureValue(
      FeatureType featureType,
      string version,
      string key,
      string value)
    {
      using (SafeHandle registryKey = OnPremRegistryUtil.OpenRegistrySubKey(InstalledFeatureService.GetFeatureKeyPath(version, featureType.ToString()), true))
      {
        RegistryHelper.SetValue(registryKey, key, value);
        InstalledFeatureService.Instance.OnFeatureChanged();
      }
    }

    public static void DeleteFeatureValue(FeatureType featureType, string version, string key)
    {
      using (SafeHandle registryKey = OnPremRegistryUtil.OpenRegistrySubKey(InstalledFeatureService.GetFeatureKeyPath(version, featureType.ToString()), true))
      {
        if (registryKey == null)
          return;
        RegistryHelper.DeleteKeyValue(registryKey, key);
        InstalledFeatureService.Instance.OnFeatureChanged();
      }
    }

    public static string ReadFeatureValue(FeatureType featureType, string version, string key)
    {
      using (SafeHandle registryKey = OnPremRegistryUtil.OpenRegistrySubKey(InstalledFeatureService.GetFeatureKeyPath(version, featureType.ToString()), false))
      {
        if (registryKey != null)
        {
          object obj = RegistryHelper.GetValue(registryKey, key, (object) null);
          if (obj != null)
            return obj.ToString();
        }
        return (string) null;
      }
    }

    internal static string GetInstalledComponentsPath(string version) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}\\{2}", (object) "Software\\Microsoft\\TeamFoundationServer", (object) version, (object) "InstalledComponents");

    internal static string GetFeatureKeyPath(string version, string featureName) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}\\{2}\\{3}", (object) "Software\\Microsoft\\TeamFoundationServer", (object) version, (object) "InstalledComponents", (object) featureName);

    private List<IFeature> LoadFeatures()
    {
      List<IFeature> features = new List<IFeature>();
      try
      {
        this.LoadInstalledVersion(features, new Version("19.0"));
      }
      catch (FormatException ex)
      {
        AdminTrace.Warning(AdminCommonResources.IgnoringFormatexceptionParsingVersion((object) "19.0", (object) ex.Message));
      }
      catch (ArgumentException ex)
      {
        AdminTrace.Warning(AdminCommonResources.IgnoringArgumentexceptionParsingVersion((object) "19.0", (object) ex.Message));
      }
      return features;
    }

    private void LoadInstalledVersion(List<IFeature> features, Version version)
    {
      try
      {
        using (SafeHandle safeHandle = OnPremRegistryUtil.OpenRegistrySubKey(InstalledFeatureService.GetInstalledComponentsPath(version.ToString()), false))
        {
          if (safeHandle == null)
            return;
          foreach (string subKeyName in RegistryHelper.GetSubKeyNames(safeHandle))
          {
            try
            {
              FeatureType featureType = (FeatureType) Enum.Parse(typeof (FeatureType), subKeyName);
              this.LoadPerMachineFeature(features, safeHandle, subKeyName, InstalledFeatureService.GetDisplayName(featureType), version);
            }
            catch (Exception ex)
            {
              AdminTrace.Warning(AdminCommonResources.IgnoringExceptionDuringLoadpermachinefeature((object) ex.Message));
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
    }

    private void LoadPerMachineFeature(
      List<IFeature> features,
      SafeHandle installedComponents,
      string feature,
      string displayName,
      Version version)
    {
      using (SafeHandle featureKey = OnPremRegistryUtil.OpenRegistrySubKey(InstalledFeatureService.GetFeatureKeyPath(version.ToString(), feature), false))
      {
        string installPath = this.GetInstallPath(featureKey);
        if (installPath == null)
          return;
        bool isConfigured = this.IsConfigured(featureKey);
        InstalledFeature installedFeature = new InstalledFeature(version, feature, displayName, installPath, isConfigured);
        features.Add((IFeature) installedFeature);
      }
    }

    private string GetInstallPath(SafeHandle featureKey) => RegistryHelper.GetValue(featureKey, "InstallPath", (object) null) as string;

    private bool IsConfigured(SafeHandle featureKey)
    {
      object obj = RegistryHelper.GetValue(featureKey, nameof (IsConfigured), (object) null);
      return obj != null && obj is int num && num == 1;
    }

    private SafeHandle GetFeatureRegistryKey(IFeature feature, bool writable) => OnPremRegistryUtil.OpenRegistrySubKey(InstalledFeatureService.GetFeatureKeyPath(feature.Version.ToString(), feature.Name), writable) ?? throw new TfsAdminException(AdminCommonResources.FeatureRegistryKeyNotFound((object) feature.Type));

    public void WriteRootInfo<T>(string name, T value)
    {
      using (SafeHandle registryKey = OnPremRegistryUtil.OpenRegistrySubKey(OnPremRegistryUtil.GetCurrentVersionRootPath(), true))
      {
        if ((object) value is short || (object) value is ushort || (object) value is int || (object) value is uint)
          RegistryHelper.SetValue(registryKey, name, Convert.ToInt32((object) value));
        else
          RegistryHelper.SetValue(registryKey, name, (object) value == null ? string.Empty : value.ToString());
      }
    }

    public void RemoveRootInfo(string name)
    {
      using (SafeHandle registryKey = OnPremRegistryUtil.OpenRegistrySubKey(OnPremRegistryUtil.GetCurrentVersionRootPath(), true))
        RegistryHelper.DeleteKeyValue(registryKey, name);
    }

    public void WriteFeatureInfo<T>(FeatureType featureType, string name, T value)
    {
      IFeature feature = this.GetFeature(featureType);
      if (feature == null)
        throw new TfsAdminException(AdminCommonResources.FeatureMustBeInstalledToAccessInformation((object) featureType));
      using (SafeHandle featureRegistryKey = this.GetFeatureRegistryKey(feature, true))
      {
        if ((object) value is short || (object) value is ushort || (object) value is int || (object) value is uint)
          RegistryHelper.SetValue(featureRegistryKey, name, Convert.ToInt32((object) value));
        else
          RegistryHelper.SetValue(featureRegistryKey, name, (object) value == null ? string.Empty : value.ToString());
        this.OnFeatureChanged();
      }
    }

    public bool TryReadRootInfo<T>(string name, ref T value)
    {
      using (SafeHandle key = OnPremRegistryUtil.OpenRegistrySubKey(OnPremRegistryUtil.GetCurrentVersionRootPath(), false))
        return this.TryReadRegistryValue<T>(key, name, ref value);
    }

    public bool TryReadFeatureInfo<T>(FeatureType featureType, string name, ref T value)
    {
      IFeature feature = this.GetFeature(featureType);
      if (feature == null)
        throw new TfsAdminException(AdminCommonResources.FeatureMustBeInstalledToAccessInformation((object) featureType));
      using (SafeHandle featureRegistryKey = this.GetFeatureRegistryKey(feature, false))
        return this.TryReadRegistryValue<T>(featureRegistryKey, name, ref value);
    }

    private bool TryReadRegistryValue<T>(SafeHandle key, string name, ref T value)
    {
      object obj = RegistryHelper.GetValue(key, name, (object) null);
      if (obj == null)
        return false;
      try
      {
        value = (T) Convert.ChangeType(obj, typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
        return true;
      }
      catch (InvalidCastException ex)
      {
      }
      catch (FormatException ex)
      {
      }
      catch (OverflowException ex)
      {
      }
      return false;
    }

    private static string GetDisplayName(FeatureType featureType)
    {
      string displayName;
      switch (featureType)
      {
        case FeatureType.None:
          displayName = "None";
          break;
        case FeatureType.ApplicationTier:
          displayName = FeatureDisplayName.ApplicationTier();
          break;
        case FeatureType.VersionControlProxy:
          displayName = FeatureDisplayName.VersionControlProxy();
          break;
        case FeatureType.Tools:
          displayName = FeatureDisplayName.Tools();
          break;
        case FeatureType.ObjectModel:
          displayName = FeatureDisplayName.ObjectModel();
          break;
        case FeatureType.Search:
          displayName = FeatureDisplayName.Search();
          break;
        default:
          displayName = featureType.ToString();
          break;
      }
      return displayName;
    }
  }
}
