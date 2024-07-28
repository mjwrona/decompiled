// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployAzureAppServiceResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployAzureAppServiceResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployAzureAppServiceResources), IntrospectionExtensions.GetTypeInfo(typeof (deployAzureAppServiceResources)).Assembly);

    public static ResourceManager Manager => deployAzureAppServiceResources.s_resMgr;

    private static string Get(string resourceName) => deployAzureAppServiceResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployAzureAppServiceResources.Get(resourceName) : deployAzureAppServiceResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployAzureAppServiceResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployAzureAppServiceResources.GetInt(resourceName) : (int) deployAzureAppServiceResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployAzureAppServiceResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployAzureAppServiceResources.GetBool(resourceName) : (bool) deployAzureAppServiceResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployAzureAppServiceResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployAzureAppServiceResources.Get(resourceName, culture);
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

    public static string ConnectedServiceNameLabel() => deployAzureAppServiceResources.Get(nameof (ConnectedServiceNameLabel));

    public static string ConnectedServiceNameLabel(CultureInfo culture) => deployAzureAppServiceResources.Get(nameof (ConnectedServiceNameLabel), culture);

    public static string ConnectedServiceNameMarkdown() => deployAzureAppServiceResources.Get(nameof (ConnectedServiceNameMarkdown));

    public static string ConnectedServiceNameMarkdown(CultureInfo culture) => deployAzureAppServiceResources.Get(nameof (ConnectedServiceNameMarkdown), culture);

    public static string Description() => deployAzureAppServiceResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployAzureAppServiceResources.Get(nameof (Description), culture);

    public static string DockerNamespaceLabel() => deployAzureAppServiceResources.Get(nameof (DockerNamespaceLabel));

    public static string DockerNamespaceLabel(CultureInfo culture) => deployAzureAppServiceResources.Get(nameof (DockerNamespaceLabel), culture);

    public static string DockerNamespaceMarkdown() => deployAzureAppServiceResources.Get(nameof (DockerNamespaceMarkdown));

    public static string DockerNamespaceMarkdown(CultureInfo culture) => deployAzureAppServiceResources.Get(nameof (DockerNamespaceMarkdown), culture);

    public static string DockerRepositoryLabel() => deployAzureAppServiceResources.Get(nameof (DockerRepositoryLabel));

    public static string DockerRepositoryLabel(CultureInfo culture) => deployAzureAppServiceResources.Get(nameof (DockerRepositoryLabel), culture);

    public static string DockerRepositoryMarkdown() => deployAzureAppServiceResources.Get(nameof (DockerRepositoryMarkdown));

    public static string DockerRepositoryMarkdown(CultureInfo culture) => deployAzureAppServiceResources.Get(nameof (DockerRepositoryMarkdown), culture);

    public static string Name() => deployAzureAppServiceResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployAzureAppServiceResources.Get(nameof (Name), culture);

    public static string StartupCommandLabel() => deployAzureAppServiceResources.Get(nameof (StartupCommandLabel));

    public static string StartupCommandLabel(CultureInfo culture) => deployAzureAppServiceResources.Get(nameof (StartupCommandLabel), culture);

    public static string StartupCommandMarkdown() => deployAzureAppServiceResources.Get(nameof (StartupCommandMarkdown));

    public static string StartupCommandMarkdown(CultureInfo culture) => deployAzureAppServiceResources.Get(nameof (StartupCommandMarkdown), culture);

    public static string WebAppKindLabel() => deployAzureAppServiceResources.Get(nameof (WebAppKindLabel));

    public static string WebAppKindLabel(CultureInfo culture) => deployAzureAppServiceResources.Get(nameof (WebAppKindLabel), culture);

    public static string WebAppNameLabel() => deployAzureAppServiceResources.Get(nameof (WebAppNameLabel));

    public static string WebAppNameLabel(CultureInfo culture) => deployAzureAppServiceResources.Get(nameof (WebAppNameLabel), culture);

    public static string WebAppNameMarkdown() => deployAzureAppServiceResources.Get(nameof (WebAppNameMarkdown));

    public static string WebAppNameMarkdown(CultureInfo culture) => deployAzureAppServiceResources.Get(nameof (WebAppNameMarkdown), culture);
  }
}
