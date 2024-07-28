// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployAzureVmssResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployAzureVmssResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployAzureVmssResources), IntrospectionExtensions.GetTypeInfo(typeof (deployAzureVmssResources)).Assembly);

    public static ResourceManager Manager => deployAzureVmssResources.s_resMgr;

    private static string Get(string resourceName) => deployAzureVmssResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployAzureVmssResources.Get(resourceName) : deployAzureVmssResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployAzureVmssResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployAzureVmssResources.GetInt(resourceName) : (int) deployAzureVmssResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployAzureVmssResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployAzureVmssResources.GetBool(resourceName) : (bool) deployAzureVmssResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployAzureVmssResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployAzureVmssResources.Get(resourceName, culture);
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

    public static string AzureResourceGroupLabel() => deployAzureVmssResources.Get(nameof (AzureResourceGroupLabel));

    public static string AzureResourceGroupLabel(CultureInfo culture) => deployAzureVmssResources.Get(nameof (AzureResourceGroupLabel), culture);

    public static string AzureResourceGroupMarkdown() => deployAzureVmssResources.Get(nameof (AzureResourceGroupMarkdown));

    public static string AzureResourceGroupMarkdown(CultureInfo culture) => deployAzureVmssResources.Get(nameof (AzureResourceGroupMarkdown), culture);

    public static string ConnectedServiceNameLabel() => deployAzureVmssResources.Get(nameof (ConnectedServiceNameLabel));

    public static string ConnectedServiceNameLabel(CultureInfo culture) => deployAzureVmssResources.Get(nameof (ConnectedServiceNameLabel), culture);

    public static string ConnectedServiceNameMarkdown() => deployAzureVmssResources.Get(nameof (ConnectedServiceNameMarkdown));

    public static string ConnectedServiceNameMarkdown(CultureInfo culture) => deployAzureVmssResources.Get(nameof (ConnectedServiceNameMarkdown), culture);

    public static string CustomTemplateLocationLabel() => deployAzureVmssResources.Get(nameof (CustomTemplateLocationLabel));

    public static string CustomTemplateLocationLabel(CultureInfo culture) => deployAzureVmssResources.Get(nameof (CustomTemplateLocationLabel), culture);

    public static string CustomTemplateLocationMarkdown() => deployAzureVmssResources.Get(nameof (CustomTemplateLocationMarkdown));

    public static string CustomTemplateLocationMarkdown(CultureInfo culture) => deployAzureVmssResources.Get(nameof (CustomTemplateLocationMarkdown), culture);

    public static string DeployScriptPathLabel() => deployAzureVmssResources.Get(nameof (DeployScriptPathLabel));

    public static string DeployScriptPathLabel(CultureInfo culture) => deployAzureVmssResources.Get(nameof (DeployScriptPathLabel), culture);

    public static string DeployScriptPathMarkdown() => deployAzureVmssResources.Get(nameof (DeployScriptPathMarkdown));

    public static string DeployScriptPathMarkdown(CultureInfo culture) => deployAzureVmssResources.Get(nameof (DeployScriptPathMarkdown), culture);

    public static string Description() => deployAzureVmssResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployAzureVmssResources.Get(nameof (Description), culture);

    public static string LocationLabel() => deployAzureVmssResources.Get(nameof (LocationLabel));

    public static string LocationLabel(CultureInfo culture) => deployAzureVmssResources.Get(nameof (LocationLabel), culture);

    public static string LocationMarkdown() => deployAzureVmssResources.Get(nameof (LocationMarkdown));

    public static string LocationMarkdown(CultureInfo culture) => deployAzureVmssResources.Get(nameof (LocationMarkdown), culture);

    public static string Name() => deployAzureVmssResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployAzureVmssResources.Get(nameof (Name), culture);

    public static string PackagePathLabel() => deployAzureVmssResources.Get(nameof (PackagePathLabel));

    public static string PackagePathLabel(CultureInfo culture) => deployAzureVmssResources.Get(nameof (PackagePathLabel), culture);

    public static string PackagePathMarkdown() => deployAzureVmssResources.Get(nameof (PackagePathMarkdown));

    public static string PackagePathMarkdown(CultureInfo culture) => deployAzureVmssResources.Get(nameof (PackagePathMarkdown), culture);

    public static string StorageAccountNameLabel() => deployAzureVmssResources.Get(nameof (StorageAccountNameLabel));

    public static string StorageAccountNameLabel(CultureInfo culture) => deployAzureVmssResources.Get(nameof (StorageAccountNameLabel), culture);

    public static string StorageAccountNameMarkdown() => deployAzureVmssResources.Get(nameof (StorageAccountNameMarkdown));

    public static string StorageAccountNameMarkdown(CultureInfo culture) => deployAzureVmssResources.Get(nameof (StorageAccountNameMarkdown), culture);

    public static string TemplateTypeLabel() => deployAzureVmssResources.Get(nameof (TemplateTypeLabel));

    public static string TemplateTypeLabel(CultureInfo culture) => deployAzureVmssResources.Get(nameof (TemplateTypeLabel), culture);

    public static string TemplateTypeMarkdown() => deployAzureVmssResources.Get(nameof (TemplateTypeMarkdown));

    public static string TemplateTypeMarkdown(CultureInfo culture) => deployAzureVmssResources.Get(nameof (TemplateTypeMarkdown), culture);

    public static string VmssNameLabel() => deployAzureVmssResources.Get(nameof (VmssNameLabel));

    public static string VmssNameLabel(CultureInfo culture) => deployAzureVmssResources.Get(nameof (VmssNameLabel), culture);

    public static string VmssNameMarkdown() => deployAzureVmssResources.Get(nameof (VmssNameMarkdown));

    public static string VmssNameMarkdown(CultureInfo culture) => deployAzureVmssResources.Get(nameof (VmssNameMarkdown), culture);
  }
}
