// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.AnalyticsWebApiResources
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  internal static class AnalyticsWebApiResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (AnalyticsWebApiResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => AnalyticsWebApiResources.s_resMgr;

    private static string Get(string resourceName) => AnalyticsWebApiResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? AnalyticsWebApiResources.Get(resourceName) : AnalyticsWebApiResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) AnalyticsWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? AnalyticsWebApiResources.GetInt(resourceName) : (int) AnalyticsWebApiResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) AnalyticsWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? AnalyticsWebApiResources.GetBool(resourceName) : (bool) AnalyticsWebApiResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => AnalyticsWebApiResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = AnalyticsWebApiResources.Get(resourceName, culture);
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

    public static string NoteNotFoundException(object arg0) => AnalyticsWebApiResources.Format(nameof (NoteNotFoundException), arg0);

    public static string NoteNotFoundException(object arg0, CultureInfo culture) => AnalyticsWebApiResources.Format(nameof (NoteNotFoundException), culture, arg0);

    public static string StageFailedException(object arg0, object arg1) => AnalyticsWebApiResources.Format(nameof (StageFailedException), arg0, arg1);

    public static string StageFailedException(object arg0, object arg1, CultureInfo culture) => AnalyticsWebApiResources.Format(nameof (StageFailedException), culture, arg0, arg1);

    public static string StageStreamDisabledException(object arg0, object arg1, object arg2) => AnalyticsWebApiResources.Format(nameof (StageStreamDisabledException), arg0, arg1, arg2);

    public static string StageStreamDisabledException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AnalyticsWebApiResources.Format(nameof (StageStreamDisabledException), culture, arg0, arg1, arg2);
    }

    public static string StageStreamNotFoundException(object arg0, object arg1, object arg2) => AnalyticsWebApiResources.Format(nameof (StageStreamNotFoundException), arg0, arg1, arg2);

    public static string StageStreamNotFoundException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AnalyticsWebApiResources.Format(nameof (StageStreamNotFoundException), culture, arg0, arg1, arg2);
    }

    public static string StageStreamThrottledException(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return AnalyticsWebApiResources.Format(nameof (StageStreamThrottledException), arg0, arg1, arg2, arg3);
    }

    public static string StageStreamThrottledException(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return AnalyticsWebApiResources.Format(nameof (StageStreamThrottledException), culture, arg0, arg1, arg2, arg3);
    }

    public static string StageTableInMaintenanceException(object arg0, object arg1) => AnalyticsWebApiResources.Format(nameof (StageTableInMaintenanceException), arg0, arg1);

    public static string StageTableInMaintenanceException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsWebApiResources.Format(nameof (StageTableInMaintenanceException), culture, arg0, arg1);
    }

    public static string AnalyticsViewCreationFailedException() => AnalyticsWebApiResources.Get(nameof (AnalyticsViewCreationFailedException));

    public static string AnalyticsViewCreationFailedException(CultureInfo culture) => AnalyticsWebApiResources.Get(nameof (AnalyticsViewCreationFailedException), culture);

    public static string AnalyticsViewDoesNotExistException(object arg0) => AnalyticsWebApiResources.Format(nameof (AnalyticsViewDoesNotExistException), arg0);

    public static string AnalyticsViewDoesNotExistException(object arg0, CultureInfo culture) => AnalyticsWebApiResources.Format(nameof (AnalyticsViewDoesNotExistException), culture, arg0);

    public static string WorkItemNotFoundException(object arg0) => AnalyticsWebApiResources.Format(nameof (WorkItemNotFoundException), arg0);

    public static string WorkItemNotFoundException(object arg0, CultureInfo culture) => AnalyticsWebApiResources.Format(nameof (WorkItemNotFoundException), culture, arg0);

    public static string StageKeysOnlyNotSupportedException(object arg0, object arg1, object arg2) => AnalyticsWebApiResources.Format(nameof (StageKeysOnlyNotSupportedException), arg0, arg1, arg2);

    public static string StageKeysOnlyNotSupportedException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AnalyticsWebApiResources.Format(nameof (StageKeysOnlyNotSupportedException), culture, arg0, arg1, arg2);
    }

    public static string AnalyticsViewPermissionException() => AnalyticsWebApiResources.Get(nameof (AnalyticsViewPermissionException));

    public static string AnalyticsViewPermissionException(CultureInfo culture) => AnalyticsWebApiResources.Get(nameof (AnalyticsViewPermissionException), culture);
  }
}
