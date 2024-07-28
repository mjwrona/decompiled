// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployAzureAppServiceWithSlotResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployAzureAppServiceWithSlotResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployAzureAppServiceWithSlotResources), IntrospectionExtensions.GetTypeInfo(typeof (deployAzureAppServiceWithSlotResources)).Assembly);

    public static ResourceManager Manager => deployAzureAppServiceWithSlotResources.s_resMgr;

    private static string Get(string resourceName) => deployAzureAppServiceWithSlotResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployAzureAppServiceWithSlotResources.Get(resourceName) : deployAzureAppServiceWithSlotResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployAzureAppServiceWithSlotResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployAzureAppServiceWithSlotResources.GetInt(resourceName) : (int) deployAzureAppServiceWithSlotResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployAzureAppServiceWithSlotResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployAzureAppServiceWithSlotResources.GetBool(resourceName) : (bool) deployAzureAppServiceWithSlotResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployAzureAppServiceWithSlotResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployAzureAppServiceWithSlotResources.Get(resourceName, culture);
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

    public static string ConnectedServiceNameLabel() => deployAzureAppServiceWithSlotResources.Get(nameof (ConnectedServiceNameLabel));

    public static string ConnectedServiceNameLabel(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (ConnectedServiceNameLabel), culture);

    public static string ConnectedServiceNameMarkdown() => deployAzureAppServiceWithSlotResources.Get(nameof (ConnectedServiceNameMarkdown));

    public static string ConnectedServiceNameMarkdown(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (ConnectedServiceNameMarkdown), culture);

    public static string DeployToSlotOrASEFlagLabel() => deployAzureAppServiceWithSlotResources.Get(nameof (DeployToSlotOrASEFlagLabel));

    public static string DeployToSlotOrASEFlagLabel(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (DeployToSlotOrASEFlagLabel), culture);

    public static string DeployToSlotOrASEFlagMarkdown() => deployAzureAppServiceWithSlotResources.Get(nameof (DeployToSlotOrASEFlagMarkdown));

    public static string DeployToSlotOrASEFlagMarkdown(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (DeployToSlotOrASEFlagMarkdown), culture);

    public static string Description() => deployAzureAppServiceWithSlotResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (Description), culture);

    public static string DockerNamespaceLabel() => deployAzureAppServiceWithSlotResources.Get(nameof (DockerNamespaceLabel));

    public static string DockerNamespaceLabel(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (DockerNamespaceLabel), culture);

    public static string DockerNamespaceMarkdown() => deployAzureAppServiceWithSlotResources.Get(nameof (DockerNamespaceMarkdown));

    public static string DockerNamespaceMarkdown(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (DockerNamespaceMarkdown), culture);

    public static string DockerRepositoryLabel() => deployAzureAppServiceWithSlotResources.Get(nameof (DockerRepositoryLabel));

    public static string DockerRepositoryLabel(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (DockerRepositoryLabel), culture);

    public static string DockerRepositoryMarkdown() => deployAzureAppServiceWithSlotResources.Get(nameof (DockerRepositoryMarkdown));

    public static string DockerRepositoryMarkdown(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (DockerRepositoryMarkdown), culture);

    public static string Name() => deployAzureAppServiceWithSlotResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (Name), culture);

    public static string ResourceGroupNameLabel() => deployAzureAppServiceWithSlotResources.Get(nameof (ResourceGroupNameLabel));

    public static string ResourceGroupNameLabel(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (ResourceGroupNameLabel), culture);

    public static string ResourceGroupNameMarkdown() => deployAzureAppServiceWithSlotResources.Get(nameof (ResourceGroupNameMarkdown));

    public static string ResourceGroupNameMarkdown(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (ResourceGroupNameMarkdown), culture);

    public static string SlotNameLabel() => deployAzureAppServiceWithSlotResources.Get(nameof (SlotNameLabel));

    public static string SlotNameLabel(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (SlotNameLabel), culture);

    public static string SlotNameMarkdown() => deployAzureAppServiceWithSlotResources.Get(nameof (SlotNameMarkdown));

    public static string SlotNameMarkdown(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (SlotNameMarkdown), culture);

    public static string StartupCommandLabel() => deployAzureAppServiceWithSlotResources.Get(nameof (StartupCommandLabel));

    public static string StartupCommandLabel(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (StartupCommandLabel), culture);

    public static string StartupCommandMarkdown() => deployAzureAppServiceWithSlotResources.Get(nameof (StartupCommandMarkdown));

    public static string StartupCommandMarkdown(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (StartupCommandMarkdown), culture);

    public static string WebAppKindLabel() => deployAzureAppServiceWithSlotResources.Get(nameof (WebAppKindLabel));

    public static string WebAppKindLabel(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (WebAppKindLabel), culture);

    public static string WebAppNameLabel() => deployAzureAppServiceWithSlotResources.Get(nameof (WebAppNameLabel));

    public static string WebAppNameLabel(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (WebAppNameLabel), culture);

    public static string WebAppNameLabelMarkdown() => deployAzureAppServiceWithSlotResources.Get(nameof (WebAppNameLabelMarkdown));

    public static string WebAppNameLabelMarkdown(CultureInfo culture) => deployAzureAppServiceWithSlotResources.Get(nameof (WebAppNameLabelMarkdown), culture);
  }
}
