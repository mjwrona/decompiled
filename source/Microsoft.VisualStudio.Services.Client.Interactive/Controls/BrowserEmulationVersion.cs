// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.BrowserEmulationVersion
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Win32;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Client.Controls
{
  internal static class BrowserEmulationVersion
  {
    private const string IERootKey = "Software\\Microsoft\\Internet Explorer";
    private const string BrowserEmulationKey = "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION";

    public static void TrySetBrowserVersion(BrowserVersion version = BrowserVersion.IE9Force)
    {
      try
      {
        int ieVersion = BrowserEmulationVersion.GetIEVersion();
        if (version == BrowserVersion.Default)
          version = BrowserEmulationVersion.GetEmulationVersionByBrowserVersion(ieVersion);
        int num = (int) version / 1000;
        if (ieVersion != 0 && num != 0 && num > ieVersion)
          throw new ArgumentException(string.Format("Requested version: {0} is greater than actual IE version: {1}", (object) num, (object) ieVersion));
        RegistryKey registryKey = BrowserEmulationVersion.OpenOrCreateRegistryKey("Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION");
        if (registryKey == null)
          return;
        string appName = BrowserEmulationVersion.GetAppName();
        if (string.IsNullOrEmpty(appName))
          return;
        registryKey.SetValue(appName, (object) (int) version, RegistryValueKind.DWord);
      }
      catch (Exception ex)
      {
      }
    }

    private static string GetAppName()
    {
      string appName = string.Empty;
      try
      {
        appName = Path.GetFileName(Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.GetModuleFileName());
      }
      catch
      {
      }
      return appName;
    }

    private static int GetIEVersion()
    {
      int ieVersion = 0;
      try
      {
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Internet Explorer");
        if (registryKey != null)
        {
          object obj = registryKey.GetValue("svcversion") ?? registryKey.GetValue("Version");
          if (obj != null)
          {
            string str = obj.ToString();
            ieVersion = int.Parse(str.Substring(0, str.IndexOf('.')));
          }
        }
      }
      catch
      {
      }
      return ieVersion;
    }

    private static RegistryKey OpenOrCreateRegistryKey(string registryPath) => Registry.CurrentUser.OpenSubKey(registryPath, true) ?? Registry.CurrentUser.CreateSubKey(registryPath);

    private static BrowserVersion GetEmulationVersionByBrowserVersion(int browserVersion)
    {
      switch (browserVersion)
      {
        case 7:
          return BrowserVersion.IE7;
        case 8:
          return BrowserVersion.IE8Force;
        case 9:
          return BrowserVersion.IE9Force;
        case 10:
          return BrowserVersion.IE10Force;
        case 11:
          return BrowserVersion.IE11Edge;
        default:
          return BrowserVersion.IE11Edge;
      }
    }
  }
}
