// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployUsingCanaryStrategyToAzureWindowsVMResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployUsingCanaryStrategyToAzureWindowsVMResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployUsingCanaryStrategyToAzureWindowsVMResources), IntrospectionExtensions.GetTypeInfo(typeof (deployUsingCanaryStrategyToAzureWindowsVMResources)).Assembly);

    public static ResourceManager Manager => deployUsingCanaryStrategyToAzureWindowsVMResources.s_resMgr;

    private static string Get(string resourceName) => deployUsingCanaryStrategyToAzureWindowsVMResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployUsingCanaryStrategyToAzureWindowsVMResources.Get(resourceName) : deployUsingCanaryStrategyToAzureWindowsVMResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployUsingCanaryStrategyToAzureWindowsVMResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployUsingCanaryStrategyToAzureWindowsVMResources.GetInt(resourceName) : (int) deployUsingCanaryStrategyToAzureWindowsVMResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployUsingCanaryStrategyToAzureWindowsVMResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployUsingCanaryStrategyToAzureWindowsVMResources.GetBool(resourceName) : (bool) deployUsingCanaryStrategyToAzureWindowsVMResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployUsingCanaryStrategyToAzureWindowsVMResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployUsingCanaryStrategyToAzureWindowsVMResources.Get(resourceName, culture);
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

    public static string Description() => deployUsingCanaryStrategyToAzureWindowsVMResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployUsingCanaryStrategyToAzureWindowsVMResources.Get(nameof (Description), culture);

    public static string Name() => deployUsingCanaryStrategyToAzureWindowsVMResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployUsingCanaryStrategyToAzureWindowsVMResources.Get(nameof (Name), culture);
  }
}
