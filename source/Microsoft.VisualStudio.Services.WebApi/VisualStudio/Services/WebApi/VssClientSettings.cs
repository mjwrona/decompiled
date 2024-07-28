// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssClientSettings
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common.ClientStorage;
using Microsoft.Win32;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal static class VssClientSettings
  {
    private static int? s_clientCacheTimeToLive;
    private static bool s_checkedClientCacheTimeToLive;
    private const string c_cacheSettingsKey = "Services\\CacheSettings";
    private const string c_settingClientCacheTimeToLive = "ClientCacheTimeToLive";

    internal static string ClientCacheDirectory => Path.Combine(VssClientSettings.ClientSettingsDirectory, "Cache");

    internal static string ClientSettingsDirectory => Path.Combine(VssFileStorage.ClientSettingsDirectory, "11.0");

    internal static int? ClientCacheTimeToLive
    {
      get
      {
        if (!VssClientSettings.s_clientCacheTimeToLive.HasValue && !VssClientSettings.s_checkedClientCacheTimeToLive)
        {
          VssClientSettings.s_checkedClientCacheTimeToLive = true;
          RegistryKey registryKey = (RegistryKey) null;
          using (RegistryKey userRegistryRoot = VssClientEnvironment.TryGetUserRegistryRoot())
          {
            if (userRegistryRoot != null)
              registryKey = userRegistryRoot.OpenSubKey("Services\\CacheSettings");
          }
          if (registryKey == null)
          {
            using (RegistryKey applicationRegistryRoot = VssClientEnvironment.TryGetApplicationRegistryRoot())
            {
              if (applicationRegistryRoot != null)
                registryKey = applicationRegistryRoot.OpenSubKey("Services\\CacheSettings");
            }
          }
          if (registryKey != null && registryKey.GetValue(nameof (ClientCacheTimeToLive)) != null && registryKey.GetValueKind(nameof (ClientCacheTimeToLive)) == RegistryValueKind.DWord)
            VssClientSettings.s_clientCacheTimeToLive = new int?(Math.Max(1, (int) registryKey.GetValue(nameof (ClientCacheTimeToLive))));
        }
        return VssClientSettings.s_clientCacheTimeToLive;
      }
      set => VssClientSettings.s_clientCacheTimeToLive = value;
    }

    internal static void GetConnectionOverrides(
      out VssConnectMode? connectModeOverride,
      out string userOverride)
    {
      connectModeOverride = new VssConnectMode?();
      userOverride = VssClientEnvironment.GetSharedConnectedUserValue<string>("FederatedAuthenticationUser");
      string connectedUserValue = VssClientEnvironment.GetSharedConnectedUserValue<string>("FederatedAuthenticationMode");
      VssConnectMode result = VssConnectMode.Automatic;
      if (connectedUserValue == null || !Enum.TryParse<VssConnectMode>(connectedUserValue, out result))
        return;
      connectModeOverride = new VssConnectMode?(result);
    }
  }
}
