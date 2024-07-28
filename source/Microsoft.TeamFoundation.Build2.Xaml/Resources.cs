// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Xaml.Resources
// Assembly: Microsoft.TeamFoundation.Build2.Xaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 48A241AC-D20F-49E0-A581-C219E1ED7760
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Xaml.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Xaml
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.TeamFoundation.Build2.Xaml.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.TeamFoundation.Build2.Xaml.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.TeamFoundation.Build2.Xaml.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Build2.Xaml.Resources.Get(resourceName) : Microsoft.TeamFoundation.Build2.Xaml.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.TeamFoundation.Build2.Xaml.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Build2.Xaml.Resources.GetInt(resourceName) : (int) Microsoft.TeamFoundation.Build2.Xaml.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.TeamFoundation.Build2.Xaml.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Build2.Xaml.Resources.GetBool(resourceName) : (bool) Microsoft.TeamFoundation.Build2.Xaml.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.TeamFoundation.Build2.Xaml.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.TeamFoundation.Build2.Xaml.Resources.Get(resourceName, culture);
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

    public static string BuildNotFound(object arg0) => Microsoft.TeamFoundation.Build2.Xaml.Resources.Format(nameof (BuildNotFound), arg0);

    public static string BuildNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.Xaml.Resources.Format(nameof (BuildNotFound), culture, arg0);

    public static string DefinitionDisabled(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.Xaml.Resources.Format(nameof (DefinitionDisabled), arg0, arg1);

    public static string DefinitionDisabled(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Build2.Xaml.Resources.Format(nameof (DefinitionDisabled), culture, arg0, arg1);

    public static string DefinitionNotFound(object arg0) => Microsoft.TeamFoundation.Build2.Xaml.Resources.Format(nameof (DefinitionNotFound), arg0);

    public static string DefinitionNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.Xaml.Resources.Format(nameof (DefinitionNotFound), culture, arg0);

    public static string RestApiInvalidLogLocation() => Microsoft.TeamFoundation.Build2.Xaml.Resources.Get(nameof (RestApiInvalidLogLocation));

    public static string RestApiInvalidLogLocation(CultureInfo culture) => Microsoft.TeamFoundation.Build2.Xaml.Resources.Get(nameof (RestApiInvalidLogLocation), culture);

    public static string RequestPropertyInvalid(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.Xaml.Resources.Format(nameof (RequestPropertyInvalid), arg0, arg1);

    public static string RequestPropertyInvalid(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Build2.Xaml.Resources.Format(nameof (RequestPropertyInvalid), culture, arg0, arg1);
  }
}
