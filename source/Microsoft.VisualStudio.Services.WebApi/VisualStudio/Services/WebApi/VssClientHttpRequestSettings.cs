// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssClientHttpRequestSettings
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Utilities.Internal;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Security;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VssClientHttpRequestSettings : VssHttpRequestSettings
  {
    private static Lazy<VssClientHttpRequestSettings> s_defaultSettings = new Lazy<VssClientHttpRequestSettings>(new Func<VssClientHttpRequestSettings>(VssClientHttpRequestSettings.ConstructDefaultSettings));
    private const string c_settingsKey = "Services\\RequestSettings";
    private const string c_settingBypassProxyOnLocal = "BypassProxyOnLocal";
    private const string c_settingEnableCompression = "EnableCompression";
    private const string c_settingsDefaultTimeout = "DefaultTimeout";
    private const string c_settingsAgentId = "AgentId";

    public VssClientHttpRequestSettings()
    {
    }

    private VssClientHttpRequestSettings(VssClientHttpRequestSettings settingsToBeCloned)
      : base((VssHttpRequestSettings) settingsToBeCloned)
    {
    }

    public static VssClientHttpRequestSettings Default => VssClientHttpRequestSettings.s_defaultSettings.Value;

    public VssClientHttpRequestSettings Clone() => new VssClientHttpRequestSettings(this);

    internal static void ResetDefaultSettings() => VssClientHttpRequestSettings.s_defaultSettings = new Lazy<VssClientHttpRequestSettings>(VssClientHttpRequestSettings.\u003C\u003EO.\u003C0\u003E__ConstructDefaultSettings ?? (VssClientHttpRequestSettings.\u003C\u003EO.\u003C0\u003E__ConstructDefaultSettings = new Func<VssClientHttpRequestSettings>(VssClientHttpRequestSettings.ConstructDefaultSettings)));

    private static VssClientHttpRequestSettings ConstructDefaultSettings()
    {
      VssClientHttpRequestSettings httpRequestSettings = new VssClientHttpRequestSettings();
      try
      {
        RegistryKey registryKey = (RegistryKey) null;
        using (RegistryKey userRegistryRoot = VssClientEnvironment.TryGetUserRegistryRoot())
        {
          if (userRegistryRoot != null)
            registryKey = userRegistryRoot.OpenSubKey("Services\\RequestSettings");
        }
        if (registryKey == null)
        {
          using (RegistryKey applicationRegistryRoot = VssClientEnvironment.TryGetApplicationRegistryRoot())
          {
            if (applicationRegistryRoot != null)
              registryKey = applicationRegistryRoot.OpenSubKey("Services\\RequestSettings");
          }
        }
        if (registryKey == null)
        {
          using (RegistryKey userRegistryRoot = VssClientEnvironment.TryGetUserRegistryRoot())
          {
            if (userRegistryRoot != null)
              registryKey = userRegistryRoot.CreateSubKey("Services\\RequestSettings");
          }
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "VSS: {0}", (object) Guid.NewGuid().ToString("D"));
          registryKey.SetValue("AgentId", (object) str);
        }
        if (registryKey != null)
        {
          using (registryKey)
          {
            bool result;
            if (bool.TryParse(registryKey.GetValue("BypassProxyOnLocal") as string, out result))
              httpRequestSettings.BypassProxyOnLocal = result;
            if (bool.TryParse(registryKey.GetValue("EnableCompression") as string, out result))
              httpRequestSettings.CompressionEnabled = result;
            if (registryKey.GetValue("DefaultTimeout") != null && registryKey.GetValueKind("DefaultTimeout") == RegistryValueKind.DWord)
              httpRequestSettings.SendTimeout = TimeSpan.FromMilliseconds((double) Math.Max(1, (int) registryKey.GetValue("DefaultTimeout")));
            if (registryKey.GetValue("AgentId") != null)
            {
              if (registryKey.GetValueKind("AgentId") == RegistryValueKind.String)
                httpRequestSettings.AgentId = (string) registryKey.GetValue("AgentId");
            }
          }
        }
        string environmentVariable = Environment.GetEnvironmentVariable("TFS_BYPASS_PROXY_ON_LOCAL");
        if (!string.IsNullOrEmpty(environmentVariable))
          httpRequestSettings.BypassProxyOnLocal = string.Equals(environmentVariable, "1", StringComparison.Ordinal);
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case SecurityException _:
          case UnauthorizedAccessException _:
            break;
          default:
            Trace.WriteLine("An exception was encountered and ignored while reading settings: " + ex?.ToString());
            break;
        }
      }
      httpRequestSettings.UserAgent = UserAgentUtility.GetDefaultRestUserAgent();
      httpRequestSettings.ClientCertificateManager = (IVssClientCertificateManager) VssClientCertificateManager.Instance;
      return httpRequestSettings;
    }
  }
}
