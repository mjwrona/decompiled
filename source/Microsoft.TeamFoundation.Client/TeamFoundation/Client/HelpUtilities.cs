// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.HelpUtilities
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Client
{
  internal static class HelpUtilities
  {
    private const string HelpKey = "Software\\Microsoft\\Help\\v2.1";
    private const string AppRoot = "AppRoot";
    private const string HlpViewerExe = "HlpViewer.exe";
    private const string CatalogName = "VisualStudio14";
    private const string LaunchingApp = "Microsoft,VisualStudio,17.0";
    private const string VsHelpKey = "Software\\Microsoft\\VisualStudio\\17.0\\Help";
    private const string UseOnlineHelp = "UseOnlineHelp";
    private const string BaseOnlineHelpUrl = "http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&rd=true";

    public static void LaunchHelpTopic(string keyword)
    {
      ArgumentUtility.CheckForNull<string>(keyword, nameof (keyword));
      int num = HelpUtilities.GetOnlineStatus() ? 1 : 0;
      bool flag = false;
      if (num == 0)
        flag = !HelpUtilities.LaunchLocalHelp(keyword);
      if ((num | (flag ? 1 : 0)) == 0)
        return;
      HelpUtilities.LaunchOnlineHelp(keyword);
    }

    private static bool LaunchLocalHelp(string keyword)
    {
      string helpViewerPath = HelpUtilities.GetHelpViewerPath();
      if (helpViewerPath == null)
        return false;
      string arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/catalogName \"{0}\" /helpQuery \"method=f1&query={1}\" /launchingApp \"{2}\"", (object) "VisualStudio14", (object) keyword, (object) "Microsoft,VisualStudio,17.0");
      Process.Start(helpViewerPath, arguments);
      return true;
    }

    private static void LaunchOnlineHelp(string keyword)
    {
      UriBuilder uriBuilder = new UriBuilder("http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&rd=true");
      StringBuilder stringBuilder = new StringBuilder(uriBuilder.Query.Substring(1));
      string upper = CultureInfo.CurrentUICulture.Name.ToUpper();
      stringBuilder.AppendFormat("&l={0}", (object) Uri.EscapeDataString(upper));
      stringBuilder.AppendFormat("&k=k({0})", (object) Uri.EscapeDataString(keyword));
      uriBuilder.Query = stringBuilder.ToString();
      Process.Start(uriBuilder.Uri.AbsoluteUri);
    }

    private static bool GetOnlineStatus()
    {
      bool onlineStatus = true;
      using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\VisualStudio\\17.0\\Help"))
      {
        if (registryKey != null)
          onlineStatus = (int) registryKey.GetValue("UseOnlineHelp", (object) 1) > 0;
      }
      return onlineStatus;
    }

    private static string GetHelpViewerPath()
    {
      string path = (string) null;
      string helpAppRoot = HelpUtilities.GetHelpAppRoot();
      if (helpAppRoot != null)
      {
        path = Path.Combine(helpAppRoot, "HlpViewer.exe");
        if (!File.Exists(path))
        {
          TeamFoundationTrace.Warning("Expected {0} file does not exist in file system", (object) path);
          path = (string) null;
        }
      }
      return path;
    }

    private static string GetHelpAppRoot()
    {
      string helpAppRoot = (string) null;
      string name = RegistryHelper.Get32BitRegistryKeyPath("Software\\Microsoft\\Help\\v2.1");
      using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
      {
        if (registryKey != null)
          helpAppRoot = registryKey.GetValue("AppRoot") as string;
      }
      if (helpAppRoot == null)
        TeamFoundationTrace.Warning("Help system not installed, or AppRoot path not found in registry");
      return helpAppRoot;
    }
  }
}
