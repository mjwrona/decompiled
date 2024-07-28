// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.AzureFunctionsForPowershellResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class AzureFunctionsForPowershellResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (AzureFunctionsForPowershellResources), typeof (AzureFunctionsForPowershellResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => AzureFunctionsForPowershellResources.s_resMgr;

    private static string Get(string resourceName) => AzureFunctionsForPowershellResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? AzureFunctionsForPowershellResources.Get(resourceName) : AzureFunctionsForPowershellResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) AzureFunctionsForPowershellResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? AzureFunctionsForPowershellResources.GetInt(resourceName) : (int) AzureFunctionsForPowershellResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) AzureFunctionsForPowershellResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? AzureFunctionsForPowershellResources.GetBool(resourceName) : (bool) AzureFunctionsForPowershellResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => AzureFunctionsForPowershellResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = AzureFunctionsForPowershellResources.Get(resourceName, culture);
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

    public static string Description() => AzureFunctionsForPowershellResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => AzureFunctionsForPowershellResources.Get(nameof (Description), culture);

    public static string Name() => AzureFunctionsForPowershellResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => AzureFunctionsForPowershellResources.Get(nameof (Name), culture);
  }
}
