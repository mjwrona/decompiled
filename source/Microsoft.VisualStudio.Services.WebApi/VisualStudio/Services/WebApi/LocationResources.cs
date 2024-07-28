// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.LocationResources
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class LocationResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (LocationResources), typeof (LocationResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => LocationResources.s_resMgr;

    private static string Get(string resourceName) => LocationResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? LocationResources.Get(resourceName) : LocationResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) LocationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? LocationResources.GetInt(resourceName) : (int) LocationResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) LocationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? LocationResources.GetBool(resourceName) : (bool) LocationResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => LocationResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = LocationResources.Get(resourceName, culture);
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

    public static string CannotChangeParentDefinition(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return LocationResources.Format(nameof (CannotChangeParentDefinition), arg0, arg1, arg2, arg3);
    }

    public static string CannotChangeParentDefinition(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return LocationResources.Format(nameof (CannotChangeParentDefinition), culture, arg0, arg1, arg2, arg3);
    }

    public static string ParentDefinitionNotFound(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return LocationResources.Format(nameof (ParentDefinitionNotFound), arg0, arg1, arg2, arg3);
    }

    public static string ParentDefinitionNotFound(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return LocationResources.Format(nameof (ParentDefinitionNotFound), culture, arg0, arg1, arg2, arg3);
    }
  }
}
