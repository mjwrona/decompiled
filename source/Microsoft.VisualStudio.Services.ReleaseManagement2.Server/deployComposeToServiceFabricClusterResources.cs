// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployComposeToServiceFabricClusterResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployComposeToServiceFabricClusterResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployComposeToServiceFabricClusterResources), IntrospectionExtensions.GetTypeInfo(typeof (deployComposeToServiceFabricClusterResources)).Assembly);

    public static ResourceManager Manager => deployComposeToServiceFabricClusterResources.s_resMgr;

    private static string Get(string resourceName) => deployComposeToServiceFabricClusterResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployComposeToServiceFabricClusterResources.Get(resourceName) : deployComposeToServiceFabricClusterResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployComposeToServiceFabricClusterResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployComposeToServiceFabricClusterResources.GetInt(resourceName) : (int) deployComposeToServiceFabricClusterResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployComposeToServiceFabricClusterResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployComposeToServiceFabricClusterResources.GetBool(resourceName) : (bool) deployComposeToServiceFabricClusterResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployComposeToServiceFabricClusterResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployComposeToServiceFabricClusterResources.Get(resourceName, culture);
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

    public static string ApplicationNameLabel() => deployComposeToServiceFabricClusterResources.Get(nameof (ApplicationNameLabel));

    public static string ApplicationNameLabel(CultureInfo culture) => deployComposeToServiceFabricClusterResources.Get(nameof (ApplicationNameLabel), culture);

    public static string ApplicationNameMarkdown() => deployComposeToServiceFabricClusterResources.Get(nameof (ApplicationNameMarkdown));

    public static string ApplicationNameMarkdown(CultureInfo culture) => deployComposeToServiceFabricClusterResources.Get(nameof (ApplicationNameMarkdown), culture);

    public static string ComposeFilePathLabel() => deployComposeToServiceFabricClusterResources.Get(nameof (ComposeFilePathLabel));

    public static string ComposeFilePathLabel(CultureInfo culture) => deployComposeToServiceFabricClusterResources.Get(nameof (ComposeFilePathLabel), culture);

    public static string ComposeFilePathMarkdown() => deployComposeToServiceFabricClusterResources.Get(nameof (ComposeFilePathMarkdown));

    public static string ComposeFilePathMarkdown(CultureInfo culture) => deployComposeToServiceFabricClusterResources.Get(nameof (ComposeFilePathMarkdown), culture);

    public static string Description() => deployComposeToServiceFabricClusterResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployComposeToServiceFabricClusterResources.Get(nameof (Description), culture);

    public static string Name() => deployComposeToServiceFabricClusterResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployComposeToServiceFabricClusterResources.Get(nameof (Name), culture);

    public static string ServiceConnectionNameLabel() => deployComposeToServiceFabricClusterResources.Get(nameof (ServiceConnectionNameLabel));

    public static string ServiceConnectionNameLabel(CultureInfo culture) => deployComposeToServiceFabricClusterResources.Get(nameof (ServiceConnectionNameLabel), culture);

    public static string ServiceConnectionNameMarkdown() => deployComposeToServiceFabricClusterResources.Get(nameof (ServiceConnectionNameMarkdown));

    public static string ServiceConnectionNameMarkdown(CultureInfo culture) => deployComposeToServiceFabricClusterResources.Get(nameof (ServiceConnectionNameMarkdown), culture);
  }
}
