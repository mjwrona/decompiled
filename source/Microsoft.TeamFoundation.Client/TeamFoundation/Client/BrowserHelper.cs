// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.BrowserHelper
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class BrowserHelper
  {
    public static void LaunchBrowser(string url) => BrowserHelper.LaunchBrowser(url, (NetworkCredential) null);

    public static void LaunchBrowser(string url, NetworkCredential credentials)
    {
      try
      {
        BrowserHelper.AttempStartBrowser(url, credentials);
      }
      catch (Exception ex)
      {
        if (credentials != null)
          BrowserHelper.AttempStartBrowser(url, (NetworkCredential) null);
        else
          throw;
      }
    }

    private static void AttempStartBrowser(string url, NetworkCredential credentials)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(url, nameof (url));
      Uri result;
      if (Uri.TryCreate(url, UriKind.Absolute, out result))
        url = result.AbsoluteUri;
      try
      {
        string str1 = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\http\\UserChoice", "Progid", (object) "HTTP") as string;
        using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(str1 + "\\shell\\open\\command", false))
        {
          string str2 = registryKey.GetValue((string) null).ToString().ToLower(CultureInfo.InvariantCulture).Replace("\"", "");
          string browserPath = str2.Substring(0, str2.LastIndexOf(".exe", StringComparison.InvariantCultureIgnoreCase) + 4);
          BrowserHelper.StartBrowserProcess(url, credentials, browserPath);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        string browserPath = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\IEXPLORE.EXE", "", (object) null) as string;
        BrowserHelper.StartBrowserProcess(url, credentials, browserPath);
      }
    }

    public static bool IsWellFormedUri(string url, UriKind uriKind) => Uri.TryCreate(url, uriKind, out Uri _);

    private static void StartBrowserProcess(
      string url,
      NetworkCredential credentials,
      string browserPath)
    {
      ProcessStartInfo startInfo = new ProcessStartInfo();
      startInfo.FileName = browserPath;
      startInfo.WorkingDirectory = Path.GetDirectoryName(browserPath);
      startInfo.Arguments = url;
      startInfo.UseShellExecute = false;
      startInfo.LoadUserProfile = true;
      if (credentials != null)
      {
        startInfo.UserName = credentials.UserName;
        startInfo.Password = CredentialsCacheManager.CreateSecureString(credentials.Password);
        startInfo.Domain = credentials.Domain;
      }
      using (Process.Start(startInfo))
        ;
    }
  }
}
