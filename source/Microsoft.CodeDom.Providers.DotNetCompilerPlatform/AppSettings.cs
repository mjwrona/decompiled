// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeDom.Providers.DotNetCompilerPlatform.AppSettings
// Assembly: Microsoft.CodeDom.Providers.DotNetCompilerPlatform, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 4D7629DB-EBA2-4B9A-A3BA-C2E83ED1F745
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.dll

using System.Collections.Specialized;
using System.Web.Configuration;

namespace Microsoft.CodeDom.Providers.DotNetCompilerPlatform
{
  internal static class AppSettings
  {
    private static volatile bool _settingsInitialized;
    private static object _lock = new object();
    private static bool _disableProfilingDuringCompilation = true;
    private static string _roslynCompilerLocation = string.Empty;

    private static void LoadSettings(NameValueCollection appSettings)
    {
      if (!bool.TryParse(appSettings["aspnet:DisableProfilingDuringCompilation"], out AppSettings._disableProfilingDuringCompilation))
        AppSettings._disableProfilingDuringCompilation = true;
      AppSettings._roslynCompilerLocation = appSettings["aspnet:RoslynCompilerLocation"];
    }

    private static void EnsureSettingsLoaded()
    {
      if (AppSettings._settingsInitialized)
        return;
      lock (AppSettings._lock)
      {
        if (AppSettings._settingsInitialized)
          return;
        try
        {
          AppSettings.LoadSettings(WebConfigurationManager.AppSettings);
        }
        finally
        {
          AppSettings._settingsInitialized = true;
        }
      }
    }

    public static bool DisableProfilingDuringCompilation
    {
      get
      {
        AppSettings.EnsureSettingsLoaded();
        return AppSettings._disableProfilingDuringCompilation;
      }
    }

    public static string RoslynCompilerLocation
    {
      get
      {
        AppSettings.EnsureSettingsLoaded();
        return AppSettings._roslynCompilerLocation;
      }
    }
  }
}
