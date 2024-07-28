// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ZeusWebApiResources
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal static class ZeusWebApiResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ZeusWebApiResources), typeof (ZeusWebApiResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ZeusWebApiResources.s_resMgr;

    private static string Get(string resourceName) => ZeusWebApiResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ZeusWebApiResources.Get(resourceName) : ZeusWebApiResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ZeusWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ZeusWebApiResources.GetInt(resourceName) : (int) ZeusWebApiResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ZeusWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ZeusWebApiResources.GetBool(resourceName) : (bool) ZeusWebApiResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ZeusWebApiResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ZeusWebApiResources.Get(resourceName, culture);
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

    public static string BlobCopyRequestNotFoundException(object arg0) => ZeusWebApiResources.Format(nameof (BlobCopyRequestNotFoundException), arg0);

    public static string BlobCopyRequestNotFoundException(object arg0, CultureInfo culture) => ZeusWebApiResources.Format(nameof (BlobCopyRequestNotFoundException), culture, arg0);

    public static string DatabaseMigrationNotFoundException(object arg0) => ZeusWebApiResources.Format(nameof (DatabaseMigrationNotFoundException), arg0);

    public static string DatabaseMigrationNotFoundException(object arg0, CultureInfo culture) => ZeusWebApiResources.Format(nameof (DatabaseMigrationNotFoundException), culture, arg0);
  }
}
