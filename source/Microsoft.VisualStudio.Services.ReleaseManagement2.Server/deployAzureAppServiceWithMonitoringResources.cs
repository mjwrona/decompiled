// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployAzureAppServiceWithMonitoringResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployAzureAppServiceWithMonitoringResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployAzureAppServiceWithMonitoringResources), IntrospectionExtensions.GetTypeInfo(typeof (deployAzureAppServiceWithMonitoringResources)).Assembly);

    public static ResourceManager Manager => deployAzureAppServiceWithMonitoringResources.s_resMgr;

    private static string Get(string resourceName) => deployAzureAppServiceWithMonitoringResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployAzureAppServiceWithMonitoringResources.Get(resourceName) : deployAzureAppServiceWithMonitoringResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployAzureAppServiceWithMonitoringResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployAzureAppServiceWithMonitoringResources.GetInt(resourceName) : (int) deployAzureAppServiceWithMonitoringResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployAzureAppServiceWithMonitoringResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployAzureAppServiceWithMonitoringResources.GetBool(resourceName) : (bool) deployAzureAppServiceWithMonitoringResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployAzureAppServiceWithMonitoringResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployAzureAppServiceWithMonitoringResources.Get(resourceName, culture);
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

    public static string AppInsightsResourceGroupNameLabel() => deployAzureAppServiceWithMonitoringResources.Get(nameof (AppInsightsResourceGroupNameLabel));

    public static string AppInsightsResourceGroupNameLabel(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (AppInsightsResourceGroupNameLabel), culture);

    public static string AppInsightsResourceGroupNameMarkdown() => deployAzureAppServiceWithMonitoringResources.Get(nameof (AppInsightsResourceGroupNameMarkdown));

    public static string AppInsightsResourceGroupNameMarkdown(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (AppInsightsResourceGroupNameMarkdown), culture);

    public static string ApplicationInsightsResourceNameLabel() => deployAzureAppServiceWithMonitoringResources.Get(nameof (ApplicationInsightsResourceNameLabel));

    public static string ApplicationInsightsResourceNameLabel(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (ApplicationInsightsResourceNameLabel), culture);

    public static string ApplicationInsightsResourceNameMarkdown() => deployAzureAppServiceWithMonitoringResources.Get(nameof (ApplicationInsightsResourceNameMarkdown));

    public static string ApplicationInsightsResourceNameMarkdown(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (ApplicationInsightsResourceNameMarkdown), culture);

    public static string ConnectedServiceNameLabel() => deployAzureAppServiceWithMonitoringResources.Get(nameof (ConnectedServiceNameLabel));

    public static string ConnectedServiceNameLabel(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (ConnectedServiceNameLabel), culture);

    public static string ConnectedServiceNameMarkdown() => deployAzureAppServiceWithMonitoringResources.Get(nameof (ConnectedServiceNameMarkdown));

    public static string ConnectedServiceNameMarkdown(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (ConnectedServiceNameMarkdown), culture);

    public static string Description() => deployAzureAppServiceWithMonitoringResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (Description), culture);

    public static string DockerNamespaceLabel() => deployAzureAppServiceWithMonitoringResources.Get(nameof (DockerNamespaceLabel));

    public static string DockerNamespaceLabel(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (DockerNamespaceLabel), culture);

    public static string DockerNamespaceMarkdown() => deployAzureAppServiceWithMonitoringResources.Get(nameof (DockerNamespaceMarkdown));

    public static string DockerNamespaceMarkdown(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (DockerNamespaceMarkdown), culture);

    public static string DockerRepositoryLabel() => deployAzureAppServiceWithMonitoringResources.Get(nameof (DockerRepositoryLabel));

    public static string DockerRepositoryLabel(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (DockerRepositoryLabel), culture);

    public static string DockerRepositoryMarkdown() => deployAzureAppServiceWithMonitoringResources.Get(nameof (DockerRepositoryMarkdown));

    public static string DockerRepositoryMarkdown(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (DockerRepositoryMarkdown), culture);

    public static string Name() => deployAzureAppServiceWithMonitoringResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (Name), culture);

    public static string StartupCommandLabel() => deployAzureAppServiceWithMonitoringResources.Get(nameof (StartupCommandLabel));

    public static string StartupCommandLabel(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (StartupCommandLabel), culture);

    public static string StartupCommandMarkdown() => deployAzureAppServiceWithMonitoringResources.Get(nameof (StartupCommandMarkdown));

    public static string StartupCommandMarkdown(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (StartupCommandMarkdown), culture);

    public static string WebAppKindLabel() => deployAzureAppServiceWithMonitoringResources.Get(nameof (WebAppKindLabel));

    public static string WebAppKindLabel(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (WebAppKindLabel), culture);

    public static string WebAppNameLabel() => deployAzureAppServiceWithMonitoringResources.Get(nameof (WebAppNameLabel));

    public static string WebAppNameLabel(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (WebAppNameLabel), culture);

    public static string WebAppNameMarkdown() => deployAzureAppServiceWithMonitoringResources.Get(nameof (WebAppNameMarkdown));

    public static string WebAppNameMarkdown(CultureInfo culture) => deployAzureAppServiceWithMonitoringResources.Get(nameof (WebAppNameMarkdown), culture);
  }
}
