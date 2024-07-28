// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployUsingRollingStrategyToAzureLinuxVMResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployUsingRollingStrategyToAzureLinuxVMResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployUsingRollingStrategyToAzureLinuxVMResources), IntrospectionExtensions.GetTypeInfo(typeof (deployUsingRollingStrategyToAzureLinuxVMResources)).Assembly);

    public static ResourceManager Manager => deployUsingRollingStrategyToAzureLinuxVMResources.s_resMgr;

    private static string Get(string resourceName) => deployUsingRollingStrategyToAzureLinuxVMResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployUsingRollingStrategyToAzureLinuxVMResources.Get(resourceName) : deployUsingRollingStrategyToAzureLinuxVMResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployUsingRollingStrategyToAzureLinuxVMResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployUsingRollingStrategyToAzureLinuxVMResources.GetInt(resourceName) : (int) deployUsingRollingStrategyToAzureLinuxVMResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployUsingRollingStrategyToAzureLinuxVMResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployUsingRollingStrategyToAzureLinuxVMResources.GetBool(resourceName) : (bool) deployUsingRollingStrategyToAzureLinuxVMResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployUsingRollingStrategyToAzureLinuxVMResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployUsingRollingStrategyToAzureLinuxVMResources.Get(resourceName, culture);
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

    public static string Description() => deployUsingRollingStrategyToAzureLinuxVMResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployUsingRollingStrategyToAzureLinuxVMResources.Get(nameof (Description), culture);

    public static string Name() => deployUsingRollingStrategyToAzureLinuxVMResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployUsingRollingStrategyToAzureLinuxVMResources.Get(nameof (Name), culture);
  }
}
