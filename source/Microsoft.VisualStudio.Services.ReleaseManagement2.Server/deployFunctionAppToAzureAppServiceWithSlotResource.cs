// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployFunctionAppToAzureAppServiceWithSlotResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployFunctionAppToAzureAppServiceWithSlotResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployFunctionAppToAzureAppServiceWithSlotResources), IntrospectionExtensions.GetTypeInfo(typeof (deployFunctionAppToAzureAppServiceWithSlotResources)).Assembly);

    public static ResourceManager Manager => deployFunctionAppToAzureAppServiceWithSlotResources.s_resMgr;

    private static string Get(string resourceName) => deployFunctionAppToAzureAppServiceWithSlotResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployFunctionAppToAzureAppServiceWithSlotResources.Get(resourceName) : deployFunctionAppToAzureAppServiceWithSlotResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployFunctionAppToAzureAppServiceWithSlotResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployFunctionAppToAzureAppServiceWithSlotResources.GetInt(resourceName) : (int) deployFunctionAppToAzureAppServiceWithSlotResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployFunctionAppToAzureAppServiceWithSlotResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployFunctionAppToAzureAppServiceWithSlotResources.GetBool(resourceName) : (bool) deployFunctionAppToAzureAppServiceWithSlotResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployFunctionAppToAzureAppServiceWithSlotResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployFunctionAppToAzureAppServiceWithSlotResources.Get(resourceName, culture);
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

    public static string AppNameLabel() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (AppNameLabel));

    public static string AppNameLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (AppNameLabel), culture);

    public static string AppNameMarkdown() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (AppNameMarkdown));

    public static string AppNameMarkdown(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (AppNameMarkdown), culture);

    public static string AppTypeLabel() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (AppTypeLabel));

    public static string AppTypeLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (AppTypeLabel), culture);

    public static string AzureSubscriptionNameLabel() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (AzureSubscriptionNameLabel));

    public static string AzureSubscriptionNameLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (AzureSubscriptionNameLabel), culture);

    public static string AzureSubscriptionNameMarkdown() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (AzureSubscriptionNameMarkdown));

    public static string AzureSubscriptionNameMarkdown(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (AzureSubscriptionNameMarkdown), culture);

    public static string ConnectedServiceTypeMarkdown() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (ConnectedServiceTypeMarkdown));

    public static string ConnectedServiceTypeMarkdown(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (ConnectedServiceTypeMarkdown), culture);

    public static string ConnectionTypeLabel() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (ConnectionTypeLabel));

    public static string ConnectionTypeLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (ConnectionTypeLabel), culture);

    public static string DeployToSlotOrASEFlagLabel() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (DeployToSlotOrASEFlagLabel));

    public static string DeployToSlotOrASEFlagLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (DeployToSlotOrASEFlagLabel), culture);

    public static string DeployToSlotOrASEFlagMarkdown() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (DeployToSlotOrASEFlagMarkdown));

    public static string DeployToSlotOrASEFlagMarkdown(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (DeployToSlotOrASEFlagMarkdown), culture);

    public static string Description() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (Description), culture);

    public static string Name() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (Name), culture);

    public static string ResourceGroupNameLabel() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (ResourceGroupNameLabel));

    public static string ResourceGroupNameLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (ResourceGroupNameLabel), culture);

    public static string ResourceGroupNameMarkdown() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (ResourceGroupNameMarkdown));

    public static string ResourceGroupNameMarkdown(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (ResourceGroupNameMarkdown), culture);

    public static string SlotNameLabel() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (SlotNameLabel));

    public static string SlotNameLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (SlotNameLabel), culture);

    public static string SlotNameMarkdown() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (SlotNameMarkdown));

    public static string SlotNameMarkdown(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (SlotNameMarkdown), culture);

    public static string StartupCommandLabel() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (StartupCommandLabel));

    public static string StartupCommandLabel(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (StartupCommandLabel), culture);

    public static string StartupCommandMarkdown() => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (StartupCommandMarkdown));

    public static string StartupCommandMarkdown(CultureInfo culture) => deployFunctionAppToAzureAppServiceWithSlotResources.Get(nameof (StartupCommandMarkdown), culture);
  }
}
