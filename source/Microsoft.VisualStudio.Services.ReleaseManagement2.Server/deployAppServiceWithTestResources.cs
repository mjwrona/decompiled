// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployAppServiceWithTestResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployAppServiceWithTestResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployAppServiceWithTestResources), IntrospectionExtensions.GetTypeInfo(typeof (deployAppServiceWithTestResources)).Assembly);

    public static ResourceManager Manager => deployAppServiceWithTestResources.s_resMgr;

    private static string Get(string resourceName) => deployAppServiceWithTestResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployAppServiceWithTestResources.Get(resourceName) : deployAppServiceWithTestResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployAppServiceWithTestResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployAppServiceWithTestResources.GetInt(resourceName) : (int) deployAppServiceWithTestResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployAppServiceWithTestResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployAppServiceWithTestResources.GetBool(resourceName) : (bool) deployAppServiceWithTestResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployAppServiceWithTestResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployAppServiceWithTestResources.Get(resourceName, culture);
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

    public static string ConnectedServiceNameLabel() => deployAppServiceWithTestResources.Get(nameof (ConnectedServiceNameLabel));

    public static string ConnectedServiceNameLabel(CultureInfo culture) => deployAppServiceWithTestResources.Get(nameof (ConnectedServiceNameLabel), culture);

    public static string ConnectedServiceNameMarkdown() => deployAppServiceWithTestResources.Get(nameof (ConnectedServiceNameMarkdown));

    public static string ConnectedServiceNameMarkdown(CultureInfo culture) => deployAppServiceWithTestResources.Get(nameof (ConnectedServiceNameMarkdown), culture);

    public static string Description() => deployAppServiceWithTestResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployAppServiceWithTestResources.Get(nameof (Description), culture);

    public static string DockerNamespaceLabel() => deployAppServiceWithTestResources.Get(nameof (DockerNamespaceLabel));

    public static string DockerNamespaceLabel(CultureInfo culture) => deployAppServiceWithTestResources.Get(nameof (DockerNamespaceLabel), culture);

    public static string DockerNamespaceMarkdown() => deployAppServiceWithTestResources.Get(nameof (DockerNamespaceMarkdown));

    public static string DockerNamespaceMarkdown(CultureInfo culture) => deployAppServiceWithTestResources.Get(nameof (DockerNamespaceMarkdown), culture);

    public static string DockerRepositoryLabel() => deployAppServiceWithTestResources.Get(nameof (DockerRepositoryLabel));

    public static string DockerRepositoryLabel(CultureInfo culture) => deployAppServiceWithTestResources.Get(nameof (DockerRepositoryLabel), culture);

    public static string DockerRepositoryMarkdown() => deployAppServiceWithTestResources.Get(nameof (DockerRepositoryMarkdown));

    public static string DockerRepositoryMarkdown(CultureInfo culture) => deployAppServiceWithTestResources.Get(nameof (DockerRepositoryMarkdown), culture);

    public static string Name() => deployAppServiceWithTestResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployAppServiceWithTestResources.Get(nameof (Name), culture);

    public static string StartupCommandLabel() => deployAppServiceWithTestResources.Get(nameof (StartupCommandLabel));

    public static string StartupCommandLabel(CultureInfo culture) => deployAppServiceWithTestResources.Get(nameof (StartupCommandLabel), culture);

    public static string StartupCommandMarkdown() => deployAppServiceWithTestResources.Get(nameof (StartupCommandMarkdown));

    public static string StartupCommandMarkdown(CultureInfo culture) => deployAppServiceWithTestResources.Get(nameof (StartupCommandMarkdown), culture);

    public static string WebAppKindLabel() => deployAppServiceWithTestResources.Get(nameof (WebAppKindLabel));

    public static string WebAppKindLabel(CultureInfo culture) => deployAppServiceWithTestResources.Get(nameof (WebAppKindLabel), culture);

    public static string WebAppNameLabel() => deployAppServiceWithTestResources.Get(nameof (WebAppNameLabel));

    public static string WebAppNameLabel(CultureInfo culture) => deployAppServiceWithTestResources.Get(nameof (WebAppNameLabel), culture);

    public static string WebAppNameMarkdown() => deployAppServiceWithTestResources.Get(nameof (WebAppNameMarkdown));

    public static string WebAppNameMarkdown(CultureInfo culture) => deployAppServiceWithTestResources.Get(nameof (WebAppNameMarkdown), culture);
  }
}
