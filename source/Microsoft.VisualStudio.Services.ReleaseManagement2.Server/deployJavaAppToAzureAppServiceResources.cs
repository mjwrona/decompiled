// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployJavaAppToAzureAppServiceResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployJavaAppToAzureAppServiceResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployJavaAppToAzureAppServiceResources), IntrospectionExtensions.GetTypeInfo(typeof (deployJavaAppToAzureAppServiceResources)).Assembly);

    public static ResourceManager Manager => deployJavaAppToAzureAppServiceResources.s_resMgr;

    private static string Get(string resourceName) => deployJavaAppToAzureAppServiceResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployJavaAppToAzureAppServiceResources.Get(resourceName) : deployJavaAppToAzureAppServiceResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployJavaAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployJavaAppToAzureAppServiceResources.GetInt(resourceName) : (int) deployJavaAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployJavaAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployJavaAppToAzureAppServiceResources.GetBool(resourceName) : (bool) deployJavaAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployJavaAppToAzureAppServiceResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployJavaAppToAzureAppServiceResources.Get(resourceName, culture);
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

    public static string ConnectedServiceNameLabel() => deployJavaAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameLabel));

    public static string ConnectedServiceNameLabel(CultureInfo culture) => deployJavaAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameLabel), culture);

    public static string ConnectedServiceNameMarkdown() => deployJavaAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameMarkdown));

    public static string ConnectedServiceNameMarkdown(CultureInfo culture) => deployJavaAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameMarkdown), culture);

    public static string Description() => deployJavaAppToAzureAppServiceResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployJavaAppToAzureAppServiceResources.Get(nameof (Description), culture);

    public static string DockerNamespaceLabel() => deployJavaAppToAzureAppServiceResources.Get(nameof (DockerNamespaceLabel));

    public static string DockerNamespaceLabel(CultureInfo culture) => deployJavaAppToAzureAppServiceResources.Get(nameof (DockerNamespaceLabel), culture);

    public static string DockerNamespaceMarkdown() => deployJavaAppToAzureAppServiceResources.Get(nameof (DockerNamespaceMarkdown));

    public static string DockerNamespaceMarkdown(CultureInfo culture) => deployJavaAppToAzureAppServiceResources.Get(nameof (DockerNamespaceMarkdown), culture);

    public static string DockerRepositoryLabel() => deployJavaAppToAzureAppServiceResources.Get(nameof (DockerRepositoryLabel));

    public static string DockerRepositoryLabel(CultureInfo culture) => deployJavaAppToAzureAppServiceResources.Get(nameof (DockerRepositoryLabel), culture);

    public static string DockerRepositoryMarkdown() => deployJavaAppToAzureAppServiceResources.Get(nameof (DockerRepositoryMarkdown));

    public static string DockerRepositoryMarkdown(CultureInfo culture) => deployJavaAppToAzureAppServiceResources.Get(nameof (DockerRepositoryMarkdown), culture);

    public static string Name() => deployJavaAppToAzureAppServiceResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployJavaAppToAzureAppServiceResources.Get(nameof (Name), culture);

    public static string StartupCommandLabel() => deployJavaAppToAzureAppServiceResources.Get(nameof (StartupCommandLabel));

    public static string StartupCommandLabel(CultureInfo culture) => deployJavaAppToAzureAppServiceResources.Get(nameof (StartupCommandLabel), culture);

    public static string StartupCommandMarkdown() => deployJavaAppToAzureAppServiceResources.Get(nameof (StartupCommandMarkdown));

    public static string StartupCommandMarkdown(CultureInfo culture) => deployJavaAppToAzureAppServiceResources.Get(nameof (StartupCommandMarkdown), culture);

    public static string WebAppKindLabel() => deployJavaAppToAzureAppServiceResources.Get(nameof (WebAppKindLabel));

    public static string WebAppKindLabel(CultureInfo culture) => deployJavaAppToAzureAppServiceResources.Get(nameof (WebAppKindLabel), culture);

    public static string WebAppNameLabel() => deployJavaAppToAzureAppServiceResources.Get(nameof (WebAppNameLabel));

    public static string WebAppNameLabel(CultureInfo culture) => deployJavaAppToAzureAppServiceResources.Get(nameof (WebAppNameLabel), culture);

    public static string WebAppNameMarkdown() => deployJavaAppToAzureAppServiceResources.Get(nameof (WebAppNameMarkdown));

    public static string WebAppNameMarkdown(CultureInfo culture) => deployJavaAppToAzureAppServiceResources.Get(nameof (WebAppNameMarkdown), culture);
  }
}
