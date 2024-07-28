// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployFunctionAppToAzureAppServiceResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployFunctionAppToAzureAppServiceResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployFunctionAppToAzureAppServiceResources), IntrospectionExtensions.GetTypeInfo(typeof (deployFunctionAppToAzureAppServiceResources)).Assembly);

    public static ResourceManager Manager => deployFunctionAppToAzureAppServiceResources.s_resMgr;

    private static string Get(string resourceName) => deployFunctionAppToAzureAppServiceResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployFunctionAppToAzureAppServiceResources.Get(resourceName) : deployFunctionAppToAzureAppServiceResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployFunctionAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployFunctionAppToAzureAppServiceResources.GetInt(resourceName) : (int) deployFunctionAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployFunctionAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployFunctionAppToAzureAppServiceResources.GetBool(resourceName) : (bool) deployFunctionAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployFunctionAppToAzureAppServiceResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployFunctionAppToAzureAppServiceResources.Get(resourceName, culture);
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

    public static string ConnectedServiceNameLabel() => deployFunctionAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameLabel));

    public static string ConnectedServiceNameLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameLabel), culture);

    public static string ConnectedServiceNameMarkdown() => deployFunctionAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameMarkdown));

    public static string ConnectedServiceNameMarkdown(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameMarkdown), culture);

    public static string ConnectedServiceTypeMarkdown() => deployFunctionAppToAzureAppServiceResources.Get(nameof (ConnectedServiceTypeMarkdown));

    public static string ConnectedServiceTypeMarkdown(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (ConnectedServiceTypeMarkdown), culture);

    public static string ConnectionTypeLabel() => deployFunctionAppToAzureAppServiceResources.Get(nameof (ConnectionTypeLabel));

    public static string ConnectionTypeLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (ConnectionTypeLabel), culture);

    public static string Description() => deployFunctionAppToAzureAppServiceResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (Description), culture);

    public static string DockerNamespaceLabel() => deployFunctionAppToAzureAppServiceResources.Get(nameof (DockerNamespaceLabel));

    public static string DockerNamespaceLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (DockerNamespaceLabel), culture);

    public static string DockerNamespaceMarkdown() => deployFunctionAppToAzureAppServiceResources.Get(nameof (DockerNamespaceMarkdown));

    public static string DockerNamespaceMarkdown(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (DockerNamespaceMarkdown), culture);

    public static string DockerRepositoryLabel() => deployFunctionAppToAzureAppServiceResources.Get(nameof (DockerRepositoryLabel));

    public static string DockerRepositoryLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (DockerRepositoryLabel), culture);

    public static string DockerRepositoryMarkdown() => deployFunctionAppToAzureAppServiceResources.Get(nameof (DockerRepositoryMarkdown));

    public static string DockerRepositoryMarkdown(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (DockerRepositoryMarkdown), culture);

    public static string Name() => deployFunctionAppToAzureAppServiceResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (Name), culture);

    public static string PublihProfilePasswordlabel() => deployFunctionAppToAzureAppServiceResources.Get(nameof (PublihProfilePasswordlabel));

    public static string PublihProfilePasswordlabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (PublihProfilePasswordlabel), culture);

    public static string PublishProfilePasswordMarkdown() => deployFunctionAppToAzureAppServiceResources.Get(nameof (PublishProfilePasswordMarkdown));

    public static string PublishProfilePasswordMarkdown(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (PublishProfilePasswordMarkdown), culture);

    public static string PublishProfilePathLabel() => deployFunctionAppToAzureAppServiceResources.Get(nameof (PublishProfilePathLabel));

    public static string PublishProfilePathLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (PublishProfilePathLabel), culture);

    public static string StartupCommandLabel() => deployFunctionAppToAzureAppServiceResources.Get(nameof (StartupCommandLabel));

    public static string StartupCommandLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (StartupCommandLabel), culture);

    public static string StartupCommandMarkdown() => deployFunctionAppToAzureAppServiceResources.Get(nameof (StartupCommandMarkdown));

    public static string StartupCommandMarkdown(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (StartupCommandMarkdown), culture);

    public static string WebAppKindLabel() => deployFunctionAppToAzureAppServiceResources.Get(nameof (WebAppKindLabel));

    public static string WebAppKindLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (WebAppKindLabel), culture);

    public static string WebAppNameLabel() => deployFunctionAppToAzureAppServiceResources.Get(nameof (WebAppNameLabel));

    public static string WebAppNameLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (WebAppNameLabel), culture);

    public static string WebAppNameMarkdown() => deployFunctionAppToAzureAppServiceResources.Get(nameof (WebAppNameMarkdown));

    public static string WebAppNameMarkdown(CultureInfo culture) => deployFunctionAppToAzureAppServiceResources.Get(nameof (WebAppNameMarkdown), culture);
  }
}
