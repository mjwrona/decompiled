// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.PipelinesWebApiResources
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Pipelines.WebApi
{
  internal static class PipelinesWebApiResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (PipelinesWebApiResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => PipelinesWebApiResources.s_resMgr;

    private static string Get(string resourceName) => PipelinesWebApiResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? PipelinesWebApiResources.Get(resourceName) : PipelinesWebApiResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) PipelinesWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? PipelinesWebApiResources.GetInt(resourceName) : (int) PipelinesWebApiResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) PipelinesWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? PipelinesWebApiResources.GetBool(resourceName) : (bool) PipelinesWebApiResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => PipelinesWebApiResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = PipelinesWebApiResources.Get(resourceName, culture);
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

    public static string FlagEnumTypeRequired() => PipelinesWebApiResources.Get(nameof (FlagEnumTypeRequired));

    public static string FlagEnumTypeRequired(CultureInfo culture) => PipelinesWebApiResources.Get(nameof (FlagEnumTypeRequired), culture);

    public static string InvalidFlagsEnumValue(object arg0, object arg1) => PipelinesWebApiResources.Format(nameof (InvalidFlagsEnumValue), arg0, arg1);

    public static string InvalidFlagsEnumValue(object arg0, object arg1, CultureInfo culture) => PipelinesWebApiResources.Format(nameof (InvalidFlagsEnumValue), culture, arg0, arg1);

    public static string NonEmptyEnumElementsRequired(object arg0) => PipelinesWebApiResources.Format(nameof (NonEmptyEnumElementsRequired), arg0);

    public static string NonEmptyEnumElementsRequired(object arg0, CultureInfo culture) => PipelinesWebApiResources.Format(nameof (NonEmptyEnumElementsRequired), culture, arg0);
  }
}
