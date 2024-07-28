// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployPhpAppToAzureAppServiceResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployPhpAppToAzureAppServiceResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployPhpAppToAzureAppServiceResources), IntrospectionExtensions.GetTypeInfo(typeof (deployPhpAppToAzureAppServiceResources)).Assembly);

    public static ResourceManager Manager => deployPhpAppToAzureAppServiceResources.s_resMgr;

    private static string Get(string resourceName) => deployPhpAppToAzureAppServiceResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployPhpAppToAzureAppServiceResources.Get(resourceName) : deployPhpAppToAzureAppServiceResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployPhpAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployPhpAppToAzureAppServiceResources.GetInt(resourceName) : (int) deployPhpAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployPhpAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployPhpAppToAzureAppServiceResources.GetBool(resourceName) : (bool) deployPhpAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployPhpAppToAzureAppServiceResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployPhpAppToAzureAppServiceResources.Get(resourceName, culture);
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

    public static string ConnectedServiceNameLabel() => deployPhpAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameLabel));

    public static string ConnectedServiceNameLabel(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameLabel), culture);

    public static string ConnectedServiceNameMarkdown() => deployPhpAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameMarkdown));

    public static string ConnectedServiceNameMarkdown(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameMarkdown), culture);

    public static string DatabaseNameLabel() => deployPhpAppToAzureAppServiceResources.Get(nameof (DatabaseNameLabel));

    public static string DatabaseNameLabel(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (DatabaseNameLabel), culture);

    public static string DatabaseNameMarkdown() => deployPhpAppToAzureAppServiceResources.Get(nameof (DatabaseNameMarkdown));

    public static string DatabaseNameMarkdown(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (DatabaseNameMarkdown), culture);

    public static string Description() => deployPhpAppToAzureAppServiceResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (Description), culture);

    public static string DockerNamespaceLabel() => deployPhpAppToAzureAppServiceResources.Get(nameof (DockerNamespaceLabel));

    public static string DockerNamespaceLabel(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (DockerNamespaceLabel), culture);

    public static string DockerNamespaceMarkdown() => deployPhpAppToAzureAppServiceResources.Get(nameof (DockerNamespaceMarkdown));

    public static string DockerNamespaceMarkdown(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (DockerNamespaceMarkdown), culture);

    public static string DockerRepositoryLabel() => deployPhpAppToAzureAppServiceResources.Get(nameof (DockerRepositoryLabel));

    public static string DockerRepositoryLabel(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (DockerRepositoryLabel), culture);

    public static string DockerRepositoryMarkdown() => deployPhpAppToAzureAppServiceResources.Get(nameof (DockerRepositoryMarkdown));

    public static string DockerRepositoryMarkdown(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (DockerRepositoryMarkdown), culture);

    public static string Name() => deployPhpAppToAzureAppServiceResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (Name), culture);

    public static string ServerNameLabel() => deployPhpAppToAzureAppServiceResources.Get(nameof (ServerNameLabel));

    public static string ServerNameLabel(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (ServerNameLabel), culture);

    public static string ServerNameMarkdown() => deployPhpAppToAzureAppServiceResources.Get(nameof (ServerNameMarkdown));

    public static string ServerNameMarkdown(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (ServerNameMarkdown), culture);

    public static string SqlFileLabel() => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlFileLabel));

    public static string SqlFileLabel(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlFileLabel), culture);

    public static string SqlFileMarkdown() => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlFileMarkdown));

    public static string SqlFileMarkdown(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlFileMarkdown), culture);

    public static string SqlInlineLabel() => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlInlineLabel));

    public static string SqlInlineLabel(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlInlineLabel), culture);

    public static string SqlInlineMarkdown() => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlInlineMarkdown));

    public static string SqlInlineMarkdown(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlInlineMarkdown), culture);

    public static string SqlPasswordLabel() => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlPasswordLabel));

    public static string SqlPasswordLabel(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlPasswordLabel), culture);

    public static string SqlPasswordMarkdown() => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlPasswordMarkdown));

    public static string SqlPasswordMarkdown(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlPasswordMarkdown), culture);

    public static string SqlUsernameLabel() => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlUsernameLabel));

    public static string SqlUsernameLabel(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlUsernameLabel), culture);

    public static string SqlUsernameMarkdown() => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlUsernameMarkdown));

    public static string SqlUsernameMarkdown(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (SqlUsernameMarkdown), culture);

    public static string StartupCommandLabel() => deployPhpAppToAzureAppServiceResources.Get(nameof (StartupCommandLabel));

    public static string StartupCommandLabel(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (StartupCommandLabel), culture);

    public static string StartupCommandMarkdown() => deployPhpAppToAzureAppServiceResources.Get(nameof (StartupCommandMarkdown));

    public static string StartupCommandMarkdown(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (StartupCommandMarkdown), culture);

    public static string TaskNameSelectorLabel() => deployPhpAppToAzureAppServiceResources.Get(nameof (TaskNameSelectorLabel));

    public static string TaskNameSelectorLabel(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (TaskNameSelectorLabel), culture);

    public static string TaskNameSelectorMarkdown() => deployPhpAppToAzureAppServiceResources.Get(nameof (TaskNameSelectorMarkdown));

    public static string TaskNameSelectorMarkdown(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (TaskNameSelectorMarkdown), culture);

    public static string WebAppKindLabel() => deployPhpAppToAzureAppServiceResources.Get(nameof (WebAppKindLabel));

    public static string WebAppKindLabel(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (WebAppKindLabel), culture);

    public static string WebAppNameLabel() => deployPhpAppToAzureAppServiceResources.Get(nameof (WebAppNameLabel));

    public static string WebAppNameLabel(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (WebAppNameLabel), culture);

    public static string WebAppNameMarkdown() => deployPhpAppToAzureAppServiceResources.Get(nameof (WebAppNameMarkdown));

    public static string WebAppNameMarkdown(CultureInfo culture) => deployPhpAppToAzureAppServiceResources.Get(nameof (WebAppNameMarkdown), culture);
  }
}
