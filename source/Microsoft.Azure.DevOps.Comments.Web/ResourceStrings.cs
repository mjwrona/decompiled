// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.Web.ResourceStrings
// Assembly: Microsoft.Azure.DevOps.Comments.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6538262-E3F2-45F5-B799-587642D68EAC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.Web.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.DevOps.Comments.Web
{
  internal static class ResourceStrings
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ResourceStrings), typeof (ResourceStrings).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ResourceStrings.s_resMgr;

    private static string Get(string resourceName) => ResourceStrings.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.Get(resourceName) : ResourceStrings.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.GetInt(resourceName) : (int) ResourceStrings.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.GetBool(resourceName) : (bool) ResourceStrings.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ResourceStrings.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ResourceStrings.Get(resourceName, culture);
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

    public static string NullOrEmptyParameter(object arg0) => ResourceStrings.Format(nameof (NullOrEmptyParameter), arg0);

    public static string NullOrEmptyParameter(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (NullOrEmptyParameter), culture, arg0);

    public static string QueryParameterOutOfRange(object arg0) => ResourceStrings.Format(nameof (QueryParameterOutOfRange), arg0);

    public static string QueryParameterOutOfRange(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (QueryParameterOutOfRange), culture, arg0);

    public static string QueryParameterOutOfRangeWithRangeValues(
      object arg0,
      object arg1,
      object arg2)
    {
      return ResourceStrings.Format(nameof (QueryParameterOutOfRangeWithRangeValues), arg0, arg1, arg2);
    }

    public static string QueryParameterOutOfRangeWithRangeValues(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (QueryParameterOutOfRangeWithRangeValues), culture, arg0, arg1, arg2);
    }

    public static string UnknownUser() => ResourceStrings.Get(nameof (UnknownUser));

    public static string UnknownUser(CultureInfo culture) => ResourceStrings.Get(nameof (UnknownUser), culture);

    public static string IdsOutOfRange(object arg0) => ResourceStrings.Format(nameof (IdsOutOfRange), arg0);

    public static string IdsOutOfRange(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (IdsOutOfRange), culture, arg0);
  }
}
