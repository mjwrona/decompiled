// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.Resources
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.NuGet.WebApi.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.Get(resourceName, culture);
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

    public static string Error_InvalidPackage() => Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.Get(nameof (Error_InvalidPackage));

    public static string Error_InvalidPackage(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.Get(nameof (Error_InvalidPackage), culture);

    public static string Error_PackageContentBlobNotFound() => Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.Get(nameof (Error_PackageContentBlobNotFound));

    public static string Error_PackageContentBlobNotFound(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.Get(nameof (Error_PackageContentBlobNotFound), culture);

    public static string Error_PathPartMissing(object arg0) => Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.Format(nameof (Error_PathPartMissing), arg0);

    public static string Error_PathPartMissing(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.WebApi.Resources.Format(nameof (Error_PathPartMissing), culture, arg0);
  }
}
