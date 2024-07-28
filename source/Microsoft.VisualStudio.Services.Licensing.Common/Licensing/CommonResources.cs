// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.CommonResources
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal static class CommonResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (CommonResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => CommonResources.s_resMgr;

    private static string Get(string resourceName) => CommonResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? CommonResources.Get(resourceName) : CommonResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) CommonResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? CommonResources.GetInt(resourceName) : (int) CommonResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) CommonResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? CommonResources.GetBool(resourceName) : (bool) CommonResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => CommonResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = CommonResources.Get(resourceName, culture);
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

    public static string LicensingServiceMigrationInProgress() => CommonResources.Get(nameof (LicensingServiceMigrationInProgress));

    public static string LicensingServiceMigrationInProgress(CultureInfo culture) => CommonResources.Get(nameof (LicensingServiceMigrationInProgress), culture);
  }
}
