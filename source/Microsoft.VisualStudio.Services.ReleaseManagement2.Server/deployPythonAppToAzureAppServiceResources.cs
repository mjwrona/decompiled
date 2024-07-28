// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployPythonAppToAzureAppServiceResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployPythonAppToAzureAppServiceResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployPythonAppToAzureAppServiceResources), IntrospectionExtensions.GetTypeInfo(typeof (deployPythonAppToAzureAppServiceResources)).Assembly);

    public static ResourceManager Manager => deployPythonAppToAzureAppServiceResources.s_resMgr;

    private static string Get(string resourceName) => deployPythonAppToAzureAppServiceResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployPythonAppToAzureAppServiceResources.Get(resourceName) : deployPythonAppToAzureAppServiceResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployPythonAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployPythonAppToAzureAppServiceResources.GetInt(resourceName) : (int) deployPythonAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployPythonAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployPythonAppToAzureAppServiceResources.GetBool(resourceName) : (bool) deployPythonAppToAzureAppServiceResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployPythonAppToAzureAppServiceResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployPythonAppToAzureAppServiceResources.Get(resourceName, culture);
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

    public static string ConnectedServiceNameLabel() => deployPythonAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameLabel));

    public static string ConnectedServiceNameLabel(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameLabel), culture);

    public static string ConnectedServiceNameMarkdown() => deployPythonAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameMarkdown));

    public static string ConnectedServiceNameMarkdown(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (ConnectedServiceNameMarkdown), culture);

    public static string DatabaseNameLabel() => deployPythonAppToAzureAppServiceResources.Get(nameof (DatabaseNameLabel));

    public static string DatabaseNameLabel(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (DatabaseNameLabel), culture);

    public static string DatabaseNameMarkdown() => deployPythonAppToAzureAppServiceResources.Get(nameof (DatabaseNameMarkdown));

    public static string DatabaseNameMarkdown(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (DatabaseNameMarkdown), culture);

    public static string Description() => deployPythonAppToAzureAppServiceResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (Description), culture);

    public static string DockerNamespaceLabel() => deployPythonAppToAzureAppServiceResources.Get(nameof (DockerNamespaceLabel));

    public static string DockerNamespaceLabel(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (DockerNamespaceLabel), culture);

    public static string DockerNamespaceMarkdown() => deployPythonAppToAzureAppServiceResources.Get(nameof (DockerNamespaceMarkdown));

    public static string DockerNamespaceMarkdown(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (DockerNamespaceMarkdown), culture);

    public static string DockerRepositoryLabel() => deployPythonAppToAzureAppServiceResources.Get(nameof (DockerRepositoryLabel));

    public static string DockerRepositoryLabel(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (DockerRepositoryLabel), culture);

    public static string DockerRepositoryMarkdown() => deployPythonAppToAzureAppServiceResources.Get(nameof (DockerRepositoryMarkdown));

    public static string DockerRepositoryMarkdown(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (DockerRepositoryMarkdown), culture);

    public static string Name() => deployPythonAppToAzureAppServiceResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (Name), culture);

    public static string PythonAppFramework() => deployPythonAppToAzureAppServiceResources.Get(nameof (PythonAppFramework));

    public static string PythonAppFramework(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (PythonAppFramework), culture);

    public static string ServerNameLabel() => deployPythonAppToAzureAppServiceResources.Get(nameof (ServerNameLabel));

    public static string ServerNameLabel(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (ServerNameLabel), culture);

    public static string ServerNameMarkdown() => deployPythonAppToAzureAppServiceResources.Get(nameof (ServerNameMarkdown));

    public static string ServerNameMarkdown(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (ServerNameMarkdown), culture);

    public static string SqlFileLabel() => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlFileLabel));

    public static string SqlFileLabel(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlFileLabel), culture);

    public static string SqlFileMarkdown() => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlFileMarkdown));

    public static string SqlFileMarkdown(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlFileMarkdown), culture);

    public static string SqlInlineLabel() => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlInlineLabel));

    public static string SqlInlineLabel(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlInlineLabel), culture);

    public static string SqlInlineMarkdown() => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlInlineMarkdown));

    public static string SqlInlineMarkdown(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlInlineMarkdown), culture);

    public static string SqlPasswordLabel() => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlPasswordLabel));

    public static string SqlPasswordLabel(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlPasswordLabel), culture);

    public static string SqlPasswordMarkdown() => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlPasswordMarkdown));

    public static string SqlPasswordMarkdown(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlPasswordMarkdown), culture);

    public static string SqlUsernameLabel() => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlUsernameLabel));

    public static string SqlUsernameLabel(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlUsernameLabel), culture);

    public static string SqlUsernameMarkdown() => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlUsernameMarkdown));

    public static string SqlUsernameMarkdown(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (SqlUsernameMarkdown), culture);

    public static string StartupCommandLabel() => deployPythonAppToAzureAppServiceResources.Get(nameof (StartupCommandLabel));

    public static string StartupCommandLabel(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (StartupCommandLabel), culture);

    public static string StartupCommandMarkdown() => deployPythonAppToAzureAppServiceResources.Get(nameof (StartupCommandMarkdown));

    public static string StartupCommandMarkdown(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (StartupCommandMarkdown), culture);

    public static string TaskNameSelectorLabel() => deployPythonAppToAzureAppServiceResources.Get(nameof (TaskNameSelectorLabel));

    public static string TaskNameSelectorLabel(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (TaskNameSelectorLabel), culture);

    public static string TaskNameSelectorMarkdown() => deployPythonAppToAzureAppServiceResources.Get(nameof (TaskNameSelectorMarkdown));

    public static string TaskNameSelectorMarkdown(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (TaskNameSelectorMarkdown), culture);

    public static string WebAppKindLabel() => deployPythonAppToAzureAppServiceResources.Get(nameof (WebAppKindLabel));

    public static string WebAppKindLabel(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (WebAppKindLabel), culture);

    public static string WebAppNameLabel() => deployPythonAppToAzureAppServiceResources.Get(nameof (WebAppNameLabel));

    public static string WebAppNameLabel(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (WebAppNameLabel), culture);

    public static string WebAppNameMarkdown() => deployPythonAppToAzureAppServiceResources.Get(nameof (WebAppNameMarkdown));

    public static string WebAppNameMarkdown(CultureInfo culture) => deployPythonAppToAzureAppServiceResources.Get(nameof (WebAppNameMarkdown), culture);
  }
}
