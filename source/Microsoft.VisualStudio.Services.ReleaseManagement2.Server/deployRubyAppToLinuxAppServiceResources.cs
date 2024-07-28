// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployRubyAppToLinuxAppServiceResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployRubyAppToLinuxAppServiceResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployRubyAppToLinuxAppServiceResources), IntrospectionExtensions.GetTypeInfo(typeof (deployRubyAppToLinuxAppServiceResources)).Assembly);

    public static ResourceManager Manager => deployRubyAppToLinuxAppServiceResources.s_resMgr;

    private static string Get(string resourceName) => deployRubyAppToLinuxAppServiceResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployRubyAppToLinuxAppServiceResources.Get(resourceName) : deployRubyAppToLinuxAppServiceResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployRubyAppToLinuxAppServiceResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployRubyAppToLinuxAppServiceResources.GetInt(resourceName) : (int) deployRubyAppToLinuxAppServiceResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployRubyAppToLinuxAppServiceResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployRubyAppToLinuxAppServiceResources.GetBool(resourceName) : (bool) deployRubyAppToLinuxAppServiceResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployRubyAppToLinuxAppServiceResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployRubyAppToLinuxAppServiceResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string ConnectedServiceNameLabel() => deployRubyAppToLinuxAppServiceResources.Get(nameof (ConnectedServiceNameLabel));

    public static string ConnectedServiceNameLabel(CultureInfo culture) => deployRubyAppToLinuxAppServiceResources.Get(nameof (ConnectedServiceNameLabel), culture);

    public static string ConnectedServiceNameMarkdown() => deployRubyAppToLinuxAppServiceResources.Get(nameof (ConnectedServiceNameMarkdown));

    public static string ConnectedServiceNameMarkdown(CultureInfo culture) => deployRubyAppToLinuxAppServiceResources.Get(nameof (ConnectedServiceNameMarkdown), culture);

    public static string Description() => deployRubyAppToLinuxAppServiceResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployRubyAppToLinuxAppServiceResources.Get(nameof (Description), culture);

    public static string Name() => deployRubyAppToLinuxAppServiceResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployRubyAppToLinuxAppServiceResources.Get(nameof (Name), culture);

    public static string StartupCommandLabel() => deployRubyAppToLinuxAppServiceResources.Get(nameof (StartupCommandLabel));

    public static string StartupCommandLabel(CultureInfo culture) => deployRubyAppToLinuxAppServiceResources.Get(nameof (StartupCommandLabel), culture);

    public static string StartupCommandMarkdown() => deployRubyAppToLinuxAppServiceResources.Get(nameof (StartupCommandMarkdown));

    public static string StartupCommandMarkdown(CultureInfo culture) => deployRubyAppToLinuxAppServiceResources.Get(nameof (StartupCommandMarkdown), culture);

    public static string WebAppKindLabel() => deployRubyAppToLinuxAppServiceResources.Get(nameof (WebAppKindLabel));

    public static string WebAppKindLabel(CultureInfo culture) => deployRubyAppToLinuxAppServiceResources.Get(nameof (WebAppKindLabel), culture);

    public static string WebAppNameLabel() => deployRubyAppToLinuxAppServiceResources.Get(nameof (WebAppNameLabel));

    public static string WebAppNameLabel(CultureInfo culture) => deployRubyAppToLinuxAppServiceResources.Get(nameof (WebAppNameLabel), culture);

    public static string WebAppNameMarkdown() => deployRubyAppToLinuxAppServiceResources.Get(nameof (WebAppNameMarkdown));

    public static string WebAppNameMarkdown(CultureInfo culture) => deployRubyAppToLinuxAppServiceResources.Get(nameof (WebAppNameMarkdown), culture);
  }
}
