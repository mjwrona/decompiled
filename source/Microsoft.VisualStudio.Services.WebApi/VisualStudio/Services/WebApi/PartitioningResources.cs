// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.PartitioningResources
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal static class PartitioningResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (PartitioningResources), typeof (PartitioningResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => PartitioningResources.s_resMgr;

    private static string Get(string resourceName) => PartitioningResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? PartitioningResources.Get(resourceName) : PartitioningResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) PartitioningResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? PartitioningResources.GetInt(resourceName) : (int) PartitioningResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) PartitioningResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? PartitioningResources.GetBool(resourceName) : (bool) PartitioningResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => PartitioningResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = PartitioningResources.Get(resourceName, culture);
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

    public static string PartitionContainerMustBeOfflineError(object arg0) => PartitioningResources.Format(nameof (PartitionContainerMustBeOfflineError), arg0);

    public static string PartitionContainerMustBeOfflineError(object arg0, CultureInfo culture) => PartitioningResources.Format(nameof (PartitionContainerMustBeOfflineError), culture, arg0);
  }
}
