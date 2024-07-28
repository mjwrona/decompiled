// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TfsRequestSettings
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.WebApi.Utilities.Internal;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Security;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Client.Channels
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TfsRequestSettings
  {
    private static object s_objectLock;
    private static TfsRequestSettings s_defaultSettings;
    private static Encoding s_requestEncoding = (Encoding) new UTF8Encoding(false);
    private static Dictionary<string, TfsRequestSettings> s_settingsCache;
    private const string c_settingsKey = "TeamFoundation\\RequestSettings";
    private const string c_settingConnectionLimit = "ConnectionLimit";
    private const string c_settingBypassProxyOnLocal = "BypassProxyOnLocal";
    private const string c_settingEnableCompression = "EnableCompression";
    private const string c_settingCompressRequestBody = "CompressRequestBody";
    private const string c_settingsDefaultTimeout = "DefaultTimeout";
    private const string c_settingsAgentId = "AgentId";
    private const string c_settingEnableSoapTrace = "EnableSoapTracing";
    private const string c_defaultComponentName = "TeamFoundationSoapProxy";
    private const string c_traceSwitchComment = "TF Client Proxy Tracing";
    private const int m_minConnectionLimit = 32;

    static TfsRequestSettings()
    {
      TfsRequestSettings.s_objectLock = new object();
      TfsRequestSettings.s_settingsCache = new Dictionary<string, TfsRequestSettings>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public TfsRequestSettings()
    {
      this.BypassProxyOnLocal = true;
      this.CompressionEnabled = true;
      this.CompressRequestBody = false;
      this.ConnectionLimit = 32;
      this.SendTimeout = TimeSpan.FromMinutes(5.0);
    }

    private TfsRequestSettings(TfsRequestSettings settingsToBeCloned)
    {
      this.BypassProxyOnLocal = settingsToBeCloned.BypassProxyOnLocal;
      this.CompressionEnabled = settingsToBeCloned.CompressionEnabled;
      this.CompressRequestBody = settingsToBeCloned.CompressRequestBody;
      this.ConnectionLimit = settingsToBeCloned.ConnectionLimit;
      this.SendTimeout = settingsToBeCloned.SendTimeout;
      this.SoapTraceEnabled = settingsToBeCloned.SoapTraceEnabled;
      this.Tracing = new TraceSwitch(settingsToBeCloned.Tracing.DisplayName, settingsToBeCloned.Tracing.Description, ((int) settingsToBeCloned.Tracing.Level).ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.UserAgent = settingsToBeCloned.UserAgent;
      this.AgentId = settingsToBeCloned.AgentId;
    }

    public bool BypassProxyOnLocal { get; set; }

    public bool CompressionEnabled { get; set; }

    public bool CompressRequestBody { get; set; }

    public int ConnectionLimit { get; private set; }

    public TimeSpan SendTimeout { get; set; }

    public bool SoapTraceEnabled { get; private set; }

    public TraceSwitch Tracing { get; private set; }

    public string UserAgent { get; private set; }

    public string AgentId { get; set; }

    public static TfsRequestSettings Default
    {
      get
      {
        TfsRequestSettings.EnsureDefaultSettings();
        return TfsRequestSettings.s_defaultSettings;
      }
    }

    public static Encoding RequestEncoding => TfsRequestSettings.s_requestEncoding;

    public static TimeSpan TestDelay { get; set; }

    public TfsRequestSettings Clone() => new TfsRequestSettings(this);

    public static TfsRequestSettings GetSettings(string componentName)
    {
      TfsRequestSettings settings;
      if (!TfsRequestSettings.s_settingsCache.TryGetValue(componentName, out settings))
      {
        lock (TfsRequestSettings.s_objectLock)
        {
          if (!TfsRequestSettings.s_settingsCache.TryGetValue(componentName, out settings))
          {
            settings = TfsRequestSettings.Default.Clone();
            settings.Tracing = new TraceSwitch(componentName, "TF Client Proxy Tracing", ((int) settings.Tracing.Level).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            settings.SoapTraceEnabled = TFCommonUtil.GetAppSettingAsBool(componentName + "." + "EnableSoapTracing", settings.SoapTraceEnabled);
            TfsRequestSettings.s_settingsCache.Add(componentName, settings);
          }
        }
      }
      return settings;
    }

    private static void EnsureDefaultSettings()
    {
      if (TfsRequestSettings.s_defaultSettings != null)
        return;
      lock (TfsRequestSettings.s_objectLock)
      {
        if (TfsRequestSettings.s_defaultSettings != null)
          return;
        TfsRequestSettings.s_defaultSettings = new TfsRequestSettings();
        try
        {
          RegistryKey registryKey = (RegistryKey) null;
          using (RegistryKey userRegistryRoot = UIHost.TryGetUserRegistryRoot())
          {
            if (userRegistryRoot != null)
              registryKey = userRegistryRoot.OpenSubKey("TeamFoundation\\RequestSettings");
          }
          if (registryKey == null)
          {
            using (RegistryKey applicationRegistryRoot = UIHost.TryGetApplicationRegistryRoot())
            {
              if (applicationRegistryRoot != null)
                registryKey = applicationRegistryRoot.OpenSubKey("TeamFoundation\\RequestSettings");
            }
          }
          if (registryKey == null)
          {
            using (RegistryKey userRegistryRoot = UIHost.TryGetUserRegistryRoot())
            {
              if (userRegistryRoot != null)
                registryKey = userRegistryRoot.CreateSubKey("TeamFoundation\\RequestSettings");
            }
            string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TFS: {0}", (object) Guid.NewGuid().ToString("D"));
            registryKey.SetValue("AgentId", (object) str);
          }
          if (registryKey != null)
          {
            using (registryKey)
            {
              if (registryKey.GetValue("ConnectionLimit") != null && registryKey.GetValueKind("ConnectionLimit") == RegistryValueKind.DWord)
                TfsRequestSettings.s_defaultSettings.ConnectionLimit = (int) registryKey.GetValue("ConnectionLimit");
              bool result;
              if (bool.TryParse(registryKey.GetValue("BypassProxyOnLocal") as string, out result))
                TfsRequestSettings.s_defaultSettings.BypassProxyOnLocal = result;
              if (bool.TryParse(registryKey.GetValue("EnableCompression") as string, out result))
                TfsRequestSettings.s_defaultSettings.CompressionEnabled = result;
              if (bool.TryParse(registryKey.GetValue("CompressRequestBody") as string, out result))
                TfsRequestSettings.s_defaultSettings.CompressRequestBody = result;
              if (registryKey.GetValue("DefaultTimeout") != null && registryKey.GetValueKind("DefaultTimeout") == RegistryValueKind.DWord)
                TfsRequestSettings.s_defaultSettings.SendTimeout = TimeSpan.FromMilliseconds((double) Math.Max(1, (int) registryKey.GetValue("DefaultTimeout")));
              if (registryKey.GetValue("AgentId") != null)
              {
                if (registryKey.GetValueKind("AgentId") == RegistryValueKind.String)
                  TfsRequestSettings.s_defaultSettings.AgentId = (string) registryKey.GetValue("AgentId");
              }
            }
          }
          string environmentVariable1 = Environment.GetEnvironmentVariable("TFS_CONNECTION_LIMIT");
          int result1;
          if (!string.IsNullOrEmpty(environmentVariable1) && int.TryParse(environmentVariable1, out result1))
            TfsRequestSettings.s_defaultSettings.ConnectionLimit = result1;
          TfsRequestSettings.s_defaultSettings.ConnectionLimit = Math.Max(TfsRequestSettings.s_defaultSettings.ConnectionLimit, 32);
          string environmentVariable2 = Environment.GetEnvironmentVariable("TFS_BYPASS_PROXY_ON_LOCAL");
          if (!string.IsNullOrEmpty(environmentVariable2))
            TfsRequestSettings.s_defaultSettings.BypassProxyOnLocal = string.Equals(environmentVariable2, "1", StringComparison.Ordinal);
        }
        catch (Exception ex)
        {
          if (!(ex is SecurityException))
          {
            UnauthorizedAccessException unauthorizedAccessException = ex as UnauthorizedAccessException;
          }
        }
        TfsRequestSettings.s_defaultSettings.UserAgent = UserAgentUtility.GetDefaultSoapUserAgent();
        TfsRequestSettings.s_defaultSettings.Tracing = new TraceSwitch("TeamFoundationSoapProxy", "TF Client Proxy Tracing");
        TfsRequestSettings.s_defaultSettings.SoapTraceEnabled = TFCommonUtil.GetAppSettingAsBool("TeamFoundationSoapProxy" + "." + "EnableSoapTracing", false);
        if (ServicePointManager.DefaultConnectionLimit != 2)
          return;
        ServicePointManager.DefaultConnectionLimit = TfsRequestSettings.s_defaultSettings.ConnectionLimit;
      }
    }

    internal static void SetDefaultQuotas(XmlDictionaryReaderQuotas readerQuotas)
    {
      readerQuotas.MaxBytesPerRead = 65536;
      readerQuotas.MaxStringContentLength = 1048576;
    }
  }
}
