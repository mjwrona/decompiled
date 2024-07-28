// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TFProxyServerFactory
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TFProxyServerFactory
  {
    private static TFProxyServer s_tfsProxyServer;
    private static readonly string s_registryPath = "TeamFoundation\\SourceControl\\Proxy";
    private static readonly string s_proxyEnabled = "Enabled";
    private static readonly string s_proxyUrl = "Url";
    private static readonly string s_proxyRetryInterval = "RetryInterval";
    private static readonly string s_proxyAutoConfigured = "AutoConfigured";
    private static readonly string s_proxyLastConfigureTime = "LastConfigureTime";
    private static readonly string s_proxyLastCheckTime = "LastCheckTime";
    private static readonly string s_tfsProxy = "TFSPROXY";
    private static readonly string s_proxyVDir = "/VersionControlProxy";
    private static readonly string s_slashString = "/";
    private const int DefaultProxyRetryIntervalMinutes = 5;

    public static TFProxyServer GetProxyServer()
    {
      bool result1 = false;
      int result2 = 5;
      bool result3 = false;
      DateTime lastConfigureTime = new DateTime();
      DateTime lastCheckTime = new DateTime();
      string proxyUrl = Environment.GetEnvironmentVariable(TFProxyServerFactory.s_tfsProxy);
      if (!string.IsNullOrEmpty(proxyUrl))
      {
        result1 = true;
      }
      else
      {
        string proxyEnabled;
        string proxyRetryInterval;
        string proxyAutoConfigured;
        string proxyLastConfigureTime;
        string proxyLastCheckTime;
        TFProxyServerFactory.LoadProxySettings(out proxyEnabled, out proxyUrl, out proxyRetryInterval, out proxyAutoConfigured, out proxyLastConfigureTime, out proxyLastCheckTime);
        if (!bool.TryParse(proxyEnabled, out result1) || string.IsNullOrEmpty(proxyUrl))
          result1 = false;
        if (!int.TryParse(proxyRetryInterval, out result2))
          result2 = 5;
        if (!bool.TryParse(proxyAutoConfigured, out result3) || string.IsNullOrEmpty(proxyUrl))
          result3 = false;
        long result4;
        lastConfigureTime = !long.TryParse(proxyLastConfigureTime, out result4) || string.IsNullOrEmpty(proxyUrl) ? new DateTime() : DateTime.FromBinary(result4);
        long result5;
        lastCheckTime = long.TryParse(proxyLastCheckTime, out result5) ? DateTime.FromBinary(result5) : new DateTime();
      }
      if (result1)
        proxyUrl = TFProxyServerFactory.AppendVirtualDirectory(proxyUrl);
      if (TFProxyServerFactory.s_tfsProxyServer == null)
      {
        TFProxyServerFactory.s_tfsProxyServer = new TFProxyServer(proxyUrl, result1, result2, result3, lastConfigureTime, lastCheckTime);
      }
      else
      {
        TFProxyServerFactory.s_tfsProxyServer.Url = proxyUrl;
        TFProxyServerFactory.s_tfsProxyServer.IsEnabled = result1;
        TFProxyServerFactory.s_tfsProxyServer.RetryIntervalMinutes = result2;
        TFProxyServerFactory.s_tfsProxyServer.WasAutoConfigured = result3;
        TFProxyServerFactory.s_tfsProxyServer.LastConfigureTime = lastConfigureTime;
        TFProxyServerFactory.s_tfsProxyServer.LastCheckTime = lastCheckTime;
      }
      return TFProxyServerFactory.s_tfsProxyServer;
    }

    public static TFProxyServer GetProxyServerWithAutoConfigure(
      TfsConnection tfsConnection,
      string siteName = null)
    {
      TFProxyServer proxyServer = TFProxyServerFactory.GetProxyServer();
      if (!proxyServer.NeedsAutoConfigure)
        return proxyServer;
      if (string.IsNullOrWhiteSpace(siteName))
        siteName = TFProxyServerFactory.GetWorkstationSiteName();
      TFProxyServerFactory.AutoConfigureProxy(tfsConnection, siteName);
      return proxyServer;
    }

    public static void LoadProxySettings(out string proxyEnabled, out string proxyUrl) => TFProxyServerFactory.LoadProxySettings(out proxyEnabled, out proxyUrl, out string _, out string _, out string _, out string _);

    public static void LoadProxySettings(
      out string proxyEnabled,
      out string proxyUrl,
      out string proxyRetryInterval,
      out string proxyAutoConfigured,
      out string proxyLastConfigureTime,
      out string proxyLastCheckTime)
    {
      proxyEnabled = bool.FalseString;
      proxyUrl = string.Empty;
      proxyRetryInterval = string.Empty;
      proxyAutoConfigured = bool.FalseString;
      ref string local1 = ref proxyLastConfigureTime;
      DateTime dateTime = new DateTime();
      string str1 = dateTime.ToBinary().ToString();
      local1 = str1;
      ref string local2 = ref proxyLastCheckTime;
      dateTime = new DateTime();
      string str2 = dateTime.ToBinary().ToString();
      local2 = str2;
      using (RegistryKey userRegistryRoot = UIHost.TryGetUserRegistryRoot())
      {
        if (userRegistryRoot == null)
          return;
        using (RegistryKey registryKey1 = userRegistryRoot.OpenSubKey(TFProxyServerFactory.s_registryPath))
        {
          if (registryKey1 == null)
            return;
          proxyUrl = registryKey1.GetValue(TFProxyServerFactory.s_proxyUrl, (object) string.Empty) as string;
          proxyEnabled = registryKey1.GetValue(TFProxyServerFactory.s_proxyEnabled, (object) bool.FalseString) as string;
          proxyRetryInterval = registryKey1.GetValue(TFProxyServerFactory.s_proxyRetryInterval, (object) string.Empty) as string;
          proxyAutoConfigured = registryKey1.GetValue(TFProxyServerFactory.s_proxyAutoConfigured, (object) bool.FalseString) as string;
          ref string local3 = ref proxyLastConfigureTime;
          RegistryKey registryKey2 = registryKey1;
          string lastConfigureTime = TFProxyServerFactory.s_proxyLastConfigureTime;
          dateTime = new DateTime();
          string defaultValue1 = dateTime.ToBinary().ToString();
          string str3 = registryKey2.GetValue(lastConfigureTime, (object) defaultValue1) as string;
          local3 = str3;
          ref string local4 = ref proxyLastCheckTime;
          RegistryKey registryKey3 = registryKey1;
          string proxyLastCheckTime1 = TFProxyServerFactory.s_proxyLastCheckTime;
          dateTime = new DateTime();
          string defaultValue2 = dateTime.ToBinary().ToString();
          string str4 = registryKey3.GetValue(proxyLastCheckTime1, (object) defaultValue2) as string;
          local4 = str4;
        }
      }
    }

    public static void StoreProxySettings(
      string proxyEnabled,
      string proxyUrl,
      string proxyAutoConfigured)
    {
      using (RegistryKey userRegistryRoot = UIHost.TryGetUserRegistryRoot())
      {
        if (userRegistryRoot == null)
          return;
        using (RegistryKey subKey = userRegistryRoot.CreateSubKey(TFProxyServerFactory.s_registryPath))
        {
          if (subKey == null)
            return;
          subKey.SetValue(TFProxyServerFactory.s_proxyEnabled, (object) proxyEnabled);
          subKey.SetValue(TFProxyServerFactory.s_proxyUrl, (object) proxyUrl);
          subKey.SetValue(TFProxyServerFactory.s_proxyAutoConfigured, (object) proxyAutoConfigured);
          subKey.SetValue(TFProxyServerFactory.s_proxyLastConfigureTime, (object) DateTime.UtcNow.ToBinary().ToString());
          if (TFProxyServerFactory.s_tfsProxyServer == null)
            return;
          TFProxyServerFactory.s_tfsProxyServer.ResetDisableTime();
        }
      }
    }

    public static void MarkProxyChecked()
    {
      using (RegistryKey userRegistryRoot = UIHost.TryGetUserRegistryRoot())
      {
        if (userRegistryRoot == null)
          return;
        using (RegistryKey subKey = userRegistryRoot.CreateSubKey(TFProxyServerFactory.s_registryPath))
        {
          if (subKey == null)
            return;
          DateTime utcNow = DateTime.UtcNow;
          subKey.SetValue(TFProxyServerFactory.s_proxyLastCheckTime, (object) utcNow.ToBinary().ToString());
          if (TFProxyServerFactory.s_tfsProxyServer == null)
            return;
          TFProxyServerFactory.s_tfsProxyServer.LastCheckTime = utcNow;
        }
      }
    }

    private static void AutoConfigureProxy(TfsConnection tfsConnection, string siteName)
    {
      IList<Proxy> list = (IList<Proxy>) tfsConnection.GetClient<ProxyHttpClient>().GetProxiesAsync().Result.ToList<Proxy>();
      List<Proxy> bestProxies = TFProxyServerFactory.GetBestProxies(siteName, list);
      TFProxyServerFactory.MarkProxyChecked();
      foreach (Proxy proxy in bestProxies)
      {
        try
        {
          TFProxyServerFactory.ConfigureProxy(tfsConnection, proxy.Url, true);
          break;
        }
        catch (Exception ex)
        {
        }
      }
    }

    private static string GetWorkstationSiteName()
    {
      try
      {
        using (ActiveDirectorySite computerSite = ActiveDirectorySite.GetComputerSite())
          return computerSite.Name;
      }
      catch
      {
      }
      return (string) null;
    }

    private static List<Proxy> GetBestProxies(string site, IList<Proxy> proxyList)
    {
      Proxy proxy1 = (Proxy) null;
      Proxy proxy2 = (Proxy) null;
      bool flag = false;
      List<Proxy> bestProxies = new List<Proxy>();
      if (proxyList == null || proxyList.Count == 0)
        return bestProxies;
      bool? nullable;
      foreach (Proxy proxy3 in (IEnumerable<Proxy>) proxyList)
      {
        if (!string.IsNullOrEmpty(site) && StringComparer.CurrentCultureIgnoreCase.Equals(proxy3.Site, site))
        {
          nullable = proxy3.SiteDefault;
          if (nullable.GetValueOrDefault())
            proxy1 = proxy3;
          nullable = proxy3.GlobalDefault;
          if (nullable.GetValueOrDefault())
          {
            proxy2 = proxy3;
            flag = true;
          }
          nullable = proxy3.SiteDefault;
          if (!nullable.GetValueOrDefault())
          {
            nullable = proxy3.GlobalDefault;
            if (!nullable.GetValueOrDefault())
              bestProxies.Add(proxy3);
          }
        }
        else
        {
          nullable = proxy3.GlobalDefault;
          if (nullable.GetValueOrDefault())
          {
            proxy2 = proxy3;
            flag = false;
          }
        }
      }
      if (proxy1 != null && proxy2 != null && proxy1.Equals((object) proxy2))
      {
        bestProxies.Insert(0, proxy1);
      }
      else
      {
        if (proxy2 != null)
        {
          if (flag)
            bestProxies.Insert(0, proxy2);
          else
            bestProxies.Add(proxy2);
        }
        if (proxy1 != null)
          bestProxies.Insert(0, proxy1);
      }
      return bestProxies;
    }

    private static void ConfigureProxy(
      TfsConnection tfsConnection,
      string proxyUrl,
      bool proxyAutoConfigured)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(proxyUrl, nameof (proxyUrl));
      try
      {
        proxyUrl = proxyUrl.Trim().TrimEnd('/');
        if (new Uri(proxyUrl).Segments.Length == 1)
          proxyUrl = TFProxyServerFactory.AppendVirtualDirectory(proxyUrl);
        Uri url = new Uri(proxyUrl + "/V1.0/ProxyStatistics.asmx");
        new ProxyStatistics(tfsConnection, url)
        {
          Timeout = 60000
        }.QueryProxyStatistics();
        TFProxyServerFactory.StoreProxySettings(bool.TrueString, proxyUrl, proxyAutoConfigured.ToString());
        TFProxyServerFactory.GetProxyServer();
      }
      catch (TeamFoundationServerUnauthorizedException ex)
      {
      }
      catch (Exception ex)
      {
        throw new ArgumentException(ClientResources.InvalidProxyUrl((object) proxyUrl));
      }
    }

    private static string AppendVirtualDirectory(string proxyUrl) => !string.IsNullOrEmpty(proxyUrl) && new Uri(proxyUrl).AbsolutePath.Equals(TFProxyServerFactory.s_slashString, StringComparison.OrdinalIgnoreCase) ? proxyUrl + TFProxyServerFactory.s_proxyVDir : proxyUrl;
  }
}
